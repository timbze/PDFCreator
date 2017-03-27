using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Win32;
using NLog;
using pdfforge.DataStorage;
using pdfforge.DataStorage.Storage;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Printing.Printer;
using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.Core.SettingsManagement;

namespace pdfforge.PDFCreator.UI.ViewModels
{
    public interface ISettingsLoader
    {
        PdfCreatorSettings LoadPdfCreatorSettings();
        void SaveSettingsInRegistry(PdfCreatorSettings settings);
    }

    public class SettingsLoader : ISettingsLoader
    {
        private readonly IInstallationPathProvider _installationPathProvider;
        private readonly ILanguageProvider _languageProvider;

        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IPrinterHelper _printerHelper;
        private readonly ISettingsMover _settingsMover;

        public SettingsLoader(ILanguageProvider languageProvider, ISettingsMover settingsMover, IInstallationPathProvider installationPathProvider, IPrinterHelper printerHelper)
        {
            _languageProvider = languageProvider;
            _settingsMover = settingsMover;
            _installationPathProvider = installationPathProvider;
            _printerHelper = printerHelper;
        }

        public int SettingsVersion => new ApplicationProperties().SettingsVersion;

        public void SaveSettingsInRegistry(PdfCreatorSettings settings)
        {
            CheckGuids(settings);
            var regStorage = BuildStorage();
            _logger.Debug("Saving settings");
            settings.SaveData(regStorage, "");
            LogProfiles(settings);
        }

        public PdfCreatorSettings LoadPdfCreatorSettings()
        {
            MoveSettingsIfRequired();
            var regStorage = BuildStorage();

            var profileBuilder = new DefaultProfileBuilder();
            var settings = profileBuilder.CreateEmptySettings(regStorage);

            var settingsUpgrader = new SettingsUpgradeHelper(SettingsVersion);

            if (UserSettingsExist())
            {
                settings.LoadData(regStorage, "", settingsUpgrader.UpgradeSettings);
            }

            if (!_languageProvider.HasTranslation(settings.ApplicationSettings.Language))
            {
                var language = _languageProvider.FindBestLanguage(CultureInfo.CurrentCulture);
                settings.ApplicationSettings.Language = language.Iso2;
            }

            if (!CheckValidSettings(settings))
            {
                var defaultSettings = profileBuilder.CreateDefaultSettings(FindPrimaryPrinter(), regStorage, settings.ApplicationSettings.Language);

                if (DefaultUserSettingsExist())
                {
                    settings = LoadDefaultUserSettings(defaultSettings, profileBuilder, regStorage);
                }
                else
                {
                    settings = defaultSettings;
                }
            }

            CheckAndAddMissingDefaultProfile(settings, profileBuilder);
            CheckPrinterMappings(settings);
            CheckTitleReplacement(settings);

            LogProfiles(settings);

            return settings;
        }

        private PdfCreatorSettings LoadDefaultUserSettings(PdfCreatorSettings defaultSettings, DefaultProfileBuilder profileBuilder,
            IStorage regStorage)
        {
            var defaultUserStorage = new RegistryStorage(RegistryHive.Users,
                @".DEFAULT\" + _installationPathProvider.SettingsRegistryPath);

            var data = Data.CreateDataStorage();
            defaultUserStorage.SetData(data);

            // Store default settings and then load the machine defaults from HKEY_USERS\.DEFAULT to give them prefrence
            defaultSettings.StoreValues(data, "");
            defaultUserStorage.ReadData("", false);

            // And then load the combined settings with default user overriding our defaults
            var settings = profileBuilder.CreateEmptySettings(regStorage);
            settings.ReadValues(data, "");

            return settings;
        }

        private void LogProfiles(PdfCreatorSettings settings)
        {
            if (!_logger.IsTraceEnabled)
                return;

            _logger.Trace("Profiles:");
            foreach (var conversionProfile in settings.ConversionProfiles)
            {
                _logger.Trace(conversionProfile.Name);
            }
        }

        private void MoveSettingsIfRequired()
        {
            if (!_settingsMover.MoveRequired())
                return;
            _settingsMover.MoveSettings();
        }

        private IStorage BuildStorage()
        {
            var storage = new RegistryStorage(RegistryHive.CurrentUser, _installationPathProvider.SettingsRegistryPath);
            storage.ClearOnWrite = true;

            return storage;
        }

        private bool UserSettingsExist()
        {
            using (var k = Registry.CurrentUser.OpenSubKey(_installationPathProvider.SettingsRegistryPath))
                return k != null;
        }

        private bool DefaultUserSettingsExist()
        {
            using (var k = Registry.Users.OpenSubKey(@".DEFAULT\" + _installationPathProvider.SettingsRegistryPath))
                return k != null;
        }

        public bool CheckValidSettings(PdfCreatorSettings settings)
        {
            return settings.ConversionProfiles.Count > 0;
        }

        /// <summary>
        ///     Finds the primary printer by checking the printer setting from the setup
        /// </summary>
        /// <returns>
        ///     The name of the printer that was defined in the setup. If it is empty or does not exist, the return value is
        ///     "PDFCreator"
        /// </returns>
        private string FindPrimaryPrinter()
        {
            var regKeys = new List<string>
            {
                @"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall\" + _installationPathProvider.ApplicationGuid,
                @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\" + _installationPathProvider.ApplicationGuid
            };

            string printer = null;

            foreach (var regKey in regKeys)
            {
                if (printer == null)
                {
                    var o = Registry.GetValue(regKey, "Printername", null);
                    if (o != null)
                    {
                        printer = o.ToString();
                        if (!string.IsNullOrEmpty(printer))
                            return printer;
                    }
                }
            }

            return "PDFCreator";
        }

        /// <summary>
        ///     Functions checks, if a default profile exists and adds one.
        /// </summary>
        private void CheckAndAddMissingDefaultProfile(PdfCreatorSettings settings, DefaultProfileBuilder profileBuilder)
        {
            var defaultProfile = settings.GetProfileByGuid(ProfileGuids.DEFAULT_PROFILE_GUID);
            if (defaultProfile == null)
            {
                defaultProfile = profileBuilder.CreateDefaultProfile();
                settings.ConversionProfiles.Add(defaultProfile);
            }
            else
            {
                defaultProfile.Properties.Deletable = false;
            } 
        }

        /// <summary>
        ///     Sets new random GUID for profiles if the GUID is empty or exists twice
        /// </summary>
        private void CheckGuids(PdfCreatorSettings settings)
        {
            var guidList = new List<string>();
            foreach (var profile in settings.ConversionProfiles)
            {
                if (string.IsNullOrWhiteSpace(profile.Guid)
                    || guidList.Contains(profile.Guid))
                {
                    profile.Guid = Guid.NewGuid().ToString();
                }
                guidList.Add(profile.Guid);
            }
        }

        private void CheckTitleReplacement(PdfCreatorSettings settings)
        {
            var titleReplacements = settings.ApplicationSettings.TitleReplacement as List<TitleReplacement>;
            if (titleReplacements == null)
                return;

            titleReplacements.RemoveAll(x => !x.IsValid());
            titleReplacements.Sort((a, b) => string.Compare(b.Search, a.Search, StringComparison.InvariantCultureIgnoreCase));
        }

        private void CheckPrinterMappings(PdfCreatorSettings settings)
        {
            var printers = _printerHelper.GetPDFCreatorPrinters();

            // if there are no printers, something is broken and we need to fix that first
            if (!printers.Any())
                return;

            //Assign DefaultProfile for all installed printers without mapped profile. 
            foreach (var printer in printers)
            {
                if (settings.ApplicationSettings.PrinterMappings.All(o => o.PrinterName != printer))
                    settings.ApplicationSettings.PrinterMappings.Add(new PrinterMapping(printer,
                        ProfileGuids.DEFAULT_PROFILE_GUID));
            }
            //Remove uninstalled printers from mapping
            foreach (var mapping in settings.ApplicationSettings.PrinterMappings.ToArray())
            {
                if (printers.All(o => o != mapping.PrinterName))
                    settings.ApplicationSettings.PrinterMappings.Remove(mapping);
            }
            //Check primary printer
            if (
                settings.ApplicationSettings.PrinterMappings.All(
                    o => o.PrinterName != settings.ApplicationSettings.PrimaryPrinter))
            {
                settings.ApplicationSettings.PrimaryPrinter =
                    _printerHelper.GetApplicablePDFCreatorPrinter("PDFCreator", "PDFCreator") ?? "";
            }
        }
    }
}