using Microsoft.Win32;
using NLog;
using pdfforge.DataStorage;
using pdfforge.DataStorage.Storage;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.Windows;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.Threading;
using Prism.Events;
using System;
using System.IO;
using System.Net;
using System.Threading;

namespace pdfforge.PDFCreator.UI.Presentation.Assistants.Update
{
    public class UpdateAssistant : IUpdateAssistant
    {
        private readonly IUpdateLauncher _updateLauncher;
        private readonly UpdateInformationProvider _updateInformationProvider;
        private readonly IGpoSettings _gpoSettings;
        private readonly IEventAggregator _eventAggregator;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly ISettingsProvider _settingsProvider;

        private readonly string _skipVersionRegistryPath;
        private readonly IThreadManager _threadManager;
        private UpdateManagerTranslation _translation;
        private readonly IVersionHelper _versionHelper;

        public UpdateAssistant(ISettingsProvider settingsProvider,
            ITranslationUpdater translationUpdater, IThreadManager threadManager, IVersionHelper versionHelper,
            IInstallationPathProvider installationPathProvider, IUpdateLauncher updateLauncher, UpdateInformationProvider updateInformationProvider,
            IGpoSettings gpoSettings, IEventAggregator eventAggregator)
        {
            _settingsProvider = settingsProvider;
            translationUpdater.RegisterAndSetTranslation(tf => _translation = tf.UpdateOrCreateTranslation(_translation));
            _threadManager = threadManager;
            _versionHelper = versionHelper;
            _updateLauncher = updateLauncher;
            _updateInformationProvider = updateInformationProvider;
            _gpoSettings = gpoSettings;
            _eventAggregator = eventAggregator;
            _skipVersionRegistryPath = @"HKEY_CURRENT_USER\" + installationPathProvider.ApplicationRegistryPath;
            OnlineVersion = new ApplicationVersion(new Version(0, 0, 0, 0), "", "");
        }

        /// <summary>
        ///     onlineVersion of the last UpdateProcedure
        /// </summary>
        public ApplicationVersion OnlineVersion { get; private set; }

        /// <summary>
        ///     Current time for the next automated updateCheck
        /// </summary>
        private DateTime NextUpdate
        {
            get
            {
                if (_settingsProvider.Settings != null)
                {
                    return _settingsProvider.Settings.ApplicationProperties.NextUpdate;
                }

                return DateTime.Now;
            }
            set { _settingsProvider.Settings.ApplicationProperties.NextUpdate = value; }
        }

        public bool ShowUpdate => NextUpdate <= DateTime.Now && IsUpdateAvailable();

        private UpdateInterval UpdateInterval
        {
            get
            {
                var appSettings = _settingsProvider.Settings.ApplicationSettings;
                var gpoSettings = _gpoSettings;

                if (gpoSettings?.UpdateInterval == null)
                    return appSettings.UpdateInterval;
                return UpdateIntervalHelper.ParseUpdateInterval(gpoSettings.UpdateInterval);
            }
        }

        /// <summary>
        ///     Flag to report, if UpdateProcedure(Thread) is already running
        /// </summary>
        public bool UpdateProcedureIsRunning { get; private set; }

        public bool UpdatesEnabled => true;

        /// <summary>
        ///     Initialize with NextUpdate and UpdateInterval from Settings
        /// </summary>
        /// <summary>
        ///     Launch UpdateProcedure in separate Thread.
        ///     UpdateManager must be initialized!
        ///     Downloads update-info.txt and compares the recent (online) version to the current version
        ///     and launches assigned events.
        ///     Specific Events must be set in advance.
        ///     Can only be launched once at the time, reported in UpdateProcedureIsRunning flag.
        ///     Resets the UpdateManager afterwards.
        /// </summary>
        /// <param name="checkNecessity"></param>
        public void UpdateProcedure(bool checkNecessity)
        {
            var thread = new SynchronizedThread(() => UpdateThread(checkNecessity));
            thread.Name = "UpdateThread";
            thread.SetApartmentState(ApartmentState.STA);
            _threadManager.StartSynchronizedThread(thread);
        }

        private void UpdateThread(bool checkNecessity)
        {
            _logger.Debug("Launched UpdateThread");
            var eventArgs = new UpdateEventArgs();
            if (UpdateProcedureIsRunning)
            {
                _logger.Debug("UpdateThread is already running");
                return;
            }
            UpdateProcedureIsRunning = true;

            try
            {
                Update(eventArgs, checkNecessity);
            }
            catch (Exception e)
            {
                _logger.Error("Exception in UpdateProcedure:\r\n" + e.Message);
            }

            UpdateProcedureIsRunning = false;
        }

        private void Update(UpdateEventArgs eventArgs, bool checkNecessity)
        {
            if (checkNecessity)
            {
                if (UpdateInterval == UpdateInterval.Never)
                {
                    _logger.Debug("Automatic UpdateCheck is disabled");
                    return;
                }

                if (DateTime.Compare(DateTime.Now, NextUpdate) < 0)
                {
                    _logger.Debug("Update period did not pass. Next Update is set to: " + NextUpdate);
                    return;
                }
                _logger.Debug("Update period has passed (set to: " + NextUpdate + ")");
            }

            _logger.Debug("Start UpdateCheck");

            var thisVersion = _versionHelper.ApplicationVersion;
            var onlineVersion = GetOnlineVersion();
            if (onlineVersion == null)
            {
                _logger.Error("OnlineVersion not available");
                return;
            }

            if (thisVersion.CompareTo(onlineVersion.Version) < 0 && VersionShouldNotBeSkipped(checkNecessity, onlineVersion.Version))
            {
                _logger.Info("New Version available");
                OnlineVersion = onlineVersion;
                _eventAggregator.GetEvent<SetShowUpdateEvent>().Publish(true);
            }

            if (eventArgs.SkipVersion)
            {
                SetSkipVersionInRegistry(OnlineVersion.Version);
            }

            _settingsProvider.Settings.ApplicationProperties.NextUpdate = NextUpdate;

            _logger.Info("NextUpdate dated to " + NextUpdate);
        }

        private bool VersionShouldNotBeSkipped(bool checkNecessity, Version onlineVersion)
        {
            if (!checkNecessity)
                return true;

            var skipVersion = ReadSkipVersionFromRegistry();
            return skipVersion.CompareTo(onlineVersion) < 0;
        }

        private Version ReadSkipVersionFromRegistry()
        {
            var versionString = Registry.GetValue(_skipVersionRegistryPath, "SkipVersion", "").ToString();
            if (string.IsNullOrWhiteSpace(versionString))
                return new Version(0, 0);

            Version version;

            var success = Version.TryParse(versionString, out version);

            return success ? version : new Version(0, 0);
        }

        private void SetSkipVersionInRegistry(Version onlineVersion)
        {
            Registry.SetValue(_skipVersionRegistryPath, "SkipVersion", onlineVersion.ToString());
        }

        public void SetNewUpdateTime()
        {
            switch (UpdateInterval)
            {
                default:
                    NextUpdate = DateTime.Now.AddDays(7);
                    break;

                case UpdateInterval.Daily:
                    NextUpdate = DateTime.Now.AddDays(1);
                    break;

                case UpdateInterval.Weekly:
                    NextUpdate = DateTime.Now.AddDays(7);
                    break;

                case UpdateInterval.Monthly:
                    NextUpdate = DateTime.Now.AddMonths(1);
                    break;

                case UpdateInterval.Never:
                    NextUpdate = DateTime.Now.AddYears(10);
                    break;
            }

            _eventAggregator.GetEvent<SetShowUpdateEvent>().Publish(false);
        }

        private ApplicationVersion GetOnlineVersion()
        {
            _logger.Debug("Get online Version");

            var url = _updateInformationProvider.UpdateInfoUrl;
            var sectionName = _updateInformationProvider.SectionName;

            using (var webClient = new WebClient())
            {
                var contents = webClient.DownloadString(url);
                using (var stream = CreateStreamFromString(contents))
                {
                    _logger.Debug("Loading update-info.txt");
                    var data = Data.CreateDataStorage();
                    var iniStorage = new IniStorage();
                    iniStorage.Data = data;
                    iniStorage.ReadData(stream, true);

                    var onlineVersion = new Version(iniStorage.Data.GetValue(sectionName + "\\Version"));
                    var downloadUrl = iniStorage.Data.GetValue(sectionName + "\\DownloadUrl");
                    var fileHash = iniStorage.Data.GetValue(sectionName + "\\FileHash");
                    _logger.Info("Online Version: " + onlineVersion);

                    return new ApplicationVersion(onlineVersion, downloadUrl, fileHash);
                }
            }
        }

        private Stream CreateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        public void InstallNewUpdate()
        {
            _updateLauncher.LaunchUpdate(OnlineVersion);
        }

        public void SkipVersion()
        {
            if (OnlineVersion != null)
                SetSkipVersionInRegistry(OnlineVersion.Version);
            SetNewUpdateTime();
            _settingsProvider.Settings.ApplicationProperties.NextUpdate = NextUpdate;
            _eventAggregator.GetEvent<SetShowUpdateEvent>().Publish(false);
        }

        public bool IsOnlineUpdateAvailable()
        {
            NextUpdate = DateTime.Now;
            OnlineVersion = GetOnlineVersion();

            return IsUpdateAvailable();
        }

        public bool IsUpdateAvailable()
        {
            if (OnlineVersion == null)
                return false;

            var thisVersion = _versionHelper.ApplicationVersion;

            var updateAvailable = thisVersion.CompareTo(OnlineVersion.Version) < 0;

            return updateAvailable;
        }
    }
}
