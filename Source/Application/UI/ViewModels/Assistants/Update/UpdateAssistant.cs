using System;
using System.IO;
using System.Net;
using System.Threading;
using Microsoft.Win32;
using NLog;
using pdfforge.DataStorage;
using pdfforge.DataStorage.Storage;
using pdfforge.DynamicTranslator;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.Threading;

namespace pdfforge.PDFCreator.UI.ViewModels.Assistants.Update
{
    public interface IUpdateAssistant
    {
        bool UpdateProcedureIsRunning { get; }
        bool UpdatesEnabled { get; }

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
        /// /// <param name="onlyNotifyOnNewUpdate">When true, a message is only displayed to the user, if an update is available. Otherwise, a message will be displayed that no update is available</param>

        void UpdateProcedure(bool checkNecessity, bool onlyNotifyOnNewUpdate);
    }

    public class UpdateAssistant : IUpdateAssistant
    {
        private readonly IUpdateLauncher _updateLauncher;
        private readonly UpdateInformationProvider _updateInformationProvider;
        private readonly IInteractionInvoker _interactionInvoker;

        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly ISettingsProvider _settingsProvider;

        private readonly string _skipVersionRegistryPath;
        private readonly IThreadManager _threadManager;
        private readonly ITranslator _translator;
        private readonly IVersionHelper _versionHelper;

        private bool _checkNecessity = true;

        public UpdateAssistant(ISettingsProvider settingsProvider, IInteractionInvoker interactionInvoker, ITranslator translator, IThreadManager threadManager, IVersionHelper versionHelper, IInstallationPathProvider installationPathProvider, IUpdateLauncher updateLauncher, UpdateInformationProvider updateInformationProvider)
        {
            _settingsProvider = settingsProvider;
            _interactionInvoker = interactionInvoker;
            _translator = translator;
            _threadManager = threadManager;
            _versionHelper = versionHelper;
            _updateLauncher = updateLauncher;
            _updateInformationProvider = updateInformationProvider;
            _skipVersionRegistryPath = @"HKEY_CURRENT_USER\" + installationPathProvider.ApplicationRegistryPath;
        }

        /// <summary>
        ///     onlineVersion of the last UpdateProcedure
        /// </summary>
        private ApplicationVersion OnlineVersion { get; set; }

        /// <summary>
        ///     Current time for the next automated updateCheck
        /// </summary>
        private DateTime NextUpdate
        {
            get { return _settingsProvider.Settings.ApplicationProperties.NextUpdate; }
            set { _settingsProvider.Settings.ApplicationProperties.NextUpdate = value; }
        }

        private UpdateInterval UpdateInterval
        {
            get
            {
                var appSettings = _settingsProvider.Settings.ApplicationSettings;
                var gpoSettings = _settingsProvider.GpoSettings;

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
        /// <param name="onlyNotifyOnNewUpdate">When true, a message is only displayed to the user, if an update is available. Otherwise, a message will be displayed that no update is available</param>
        public void UpdateProcedure(bool checkNecessity, bool onlyNotifyOnNewUpdate)
        {
            _checkNecessity = checkNecessity;

            var thread = new SynchronizedThread(() => UpdateThread(onlyNotifyOnNewUpdate));
            thread.Name = "UpdateThread";
            thread.SetApartmentState(ApartmentState.STA);
            _threadManager.StartSynchronizedThread(thread);
        }

        private void UpdateThread(bool onlyNotifyOnNewUpdate)
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
                Update(eventArgs, onlyNotifyOnNewUpdate);
            }
            catch (Exception e)
            {
                _logger.Error("Exception in UpdateProcedure:\r\n" + e.Message);
                if (!onlyNotifyOnNewUpdate)
                {
                    _logger.Debug("Launch error window");
                    LaunchErrorForm();
                }
            }

            UpdateProcedureIsRunning = false;
        }

        private void Update(UpdateEventArgs eventArgs, bool onlyNotifyOnNewUpdate)
        {
            if (_checkNecessity)
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
            OnlineVersion = GetOnlineVersion();
            if (OnlineVersion == null)
            {
                _logger.Error("OnlineVersion not available");
                return;
            }

            if (thisVersion.CompareTo(OnlineVersion.Version) < 0 && VersionShouldNotBeSkipped(OnlineVersion.Version))
            {
                _logger.Info("New Version available");

                LaunchNewUpdateForm(eventArgs);
            }
            else
            {
                if (!onlyNotifyOnNewUpdate)
                {
                    _logger.Debug("Launch no new version window");
                    LaunchNoUpdateForm();
                }
            }

            if (eventArgs.SkipVersion)
            {
                SetSkipVersionInRegistry(OnlineVersion.Version);
            }

            SetNewUpdateTime();
            _settingsProvider.Settings.ApplicationProperties.NextUpdate = NextUpdate;

            _logger.Info("NextUpdate dated to " + NextUpdate);
        }

        private bool VersionShouldNotBeSkipped(Version onlineVersion)
        {
            if (!_checkNecessity)
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

        private void SetNewUpdateTime()
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
                    iniStorage.SetData(data);
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

        private void LaunchNewUpdateForm(UpdateEventArgs eventArgs)
        {
            var interaction = new UpdateAvailableInteraction(Urls.PDFCreatorWhatsNewUrl, OnlineVersion.Version.ToString());
            _interactionInvoker.Invoke(interaction);

            switch (interaction.Response)
            {
                case UpdateAvailableResponse.Install:
                    _updateLauncher.LaunchUpdate(OnlineVersion);
                    break;
                case UpdateAvailableResponse.Skip:
                    eventArgs.SkipVersion = true;
                    break;
            }
        }

        private void LaunchNoUpdateForm()
        {
            var caption = _translator.GetTranslation("UpdateManager", "PDFCreatorUpdate");
            var message = _translator.GetTranslation("UpdateManager", "NoUpdateMessage");
            ShowMessage(message, caption, MessageOptions.OK, MessageIcon.PDFCreator);
        }

        private void LaunchErrorForm()
        {
            var caption = _translator.GetTranslation("UpdateManager", "PDFCreatorUpdate");
            var message = _translator.GetTranslation("UpdateManager", "ErrorMessage");
            ShowMessage(message, caption, MessageOptions.OK, MessageIcon.Error);
        }

        private MessageResponse ShowMessage(string message, string title, MessageOptions buttons, MessageIcon icon)
        {
            var interaction = new MessageInteraction(message, title, buttons, icon);
            _interactionInvoker.Invoke(interaction);
            return interaction.Response;
        }
    }
}