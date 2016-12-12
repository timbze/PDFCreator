using System.Collections.Generic;
using SystemInterface.Microsoft.Win32;

namespace pdfforge.PDFCreator.Core.SettingsManagement
{
    public interface ILanguageDetector
    {
        /// <summary>
        ///     Finds the default language that was defined during the setup.
        /// </summary>
        /// <returns>The name of the language. If it was not set or the language does not exist, null is returned.</returns>
        string FindDefaultLanguage();
    }

    public class LanguageDetector : ILanguageDetector
    {
        private readonly string _appGuid;
        private readonly string _appSettingsRegPath;
        private readonly IRegistry _registry;

        public LanguageDetector(IRegistry registry, IInstallationPathProvider installationPathProvider)
        {
            _registry = registry;
            _appGuid = installationPathProvider.ApplicationGuid;
            _appSettingsRegPath = installationPathProvider.SettingsRegistryPath;
        }

        /// <summary>
        ///     Finds the default language that was defined during the setup.
        /// </summary>
        /// <returns>The name of the language. If it was not set or the language does not exist, null is returned.</returns>
        public string FindDefaultLanguage()
        {
            string language = null;

            //look for language in intended registry location 
            var key = @"HKEY_CURRENT_USER\" + _appSettingsRegPath + @"\ApplicationSettings";
            var o = _registry.GetValue(key, "Language", null);
            if (o != null)
                language = o.ToString();
            else
            //Get inno setup language
            {
                var regKeys = new List<string>
                {
                    @"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall\" + _appGuid,
                    @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\" + _appGuid
                };

                foreach (var regKey in regKeys)
                {
                    if (language == null)
                    {
                        o = _registry.GetValue(regKey, "Inno Setup: SetupLanguage", null);
                        if (o != null)
                            language = o.ToString();
                    }
                }
            }

            return language;
        }
    }
}