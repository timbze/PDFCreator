namespace pdfforge.PDFCreator.Core.SettingsManagement
{
    public interface IInstallationPathProvider
    {
        /// <summary>
        ///     The registry path where the settings are stored, without the registry hive.
        ///     i.e. "Software\pdfforge\PDFCreator\Settings"
        /// </summary>
        string SettingsRegistryPath { get; }

        /// <summary>
        ///     The registry path where the application data are stored, without the registry hive.
        ///     i.e. "Software\pdfforge\PDFCreator"
        /// </summary>
        string ApplicationRegistryPath { get; }

        /// <summary>
        ///     The GUID with curly braces, i.e. {00000000-0000-0000-0000-000000000000}
        /// </summary>
        string ApplicationGuid { get; }
    }

    public class InstallationPathProvider : IInstallationPathProvider
    {
        public InstallationPathProvider(string applicationRegistryPath, string settingsRegistryPath,
            string applicationGuid)
        {
            SettingsRegistryPath = settingsRegistryPath;
            ApplicationGuid = applicationGuid;
            ApplicationRegistryPath = applicationRegistryPath;
        }

        public string SettingsRegistryPath { get; }
        public string ApplicationRegistryPath { get; }
        public string ApplicationGuid { get; }
    }
}