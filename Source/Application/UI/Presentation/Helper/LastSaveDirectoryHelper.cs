using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Core.SettingsManagement;
using System;
using SystemInterface.Microsoft.Win32;

namespace pdfforge.PDFCreator.UI.Presentation.Helper
{
    public interface ILastSaveDirectoryHelper
    {
        string ReadFromRegistry(string defaultValue);

        void SaveInRegistry(string directory);

        bool Apply(Job job);
    }

    public class LastSaveDirectoryHelper : ILastSaveDirectoryHelper
    {
        private const string RegistryKeyForLastSaveDirectory = "LastSaveDirectory";
        private readonly IRegistry _registry;
        private readonly string _registryKeyForLastSaveDirectorySetting;

        public LastSaveDirectoryHelper(IRegistry registry, IInstallationPathProvider installationPathProvider)
        {
            _registry = registry;
            _registryKeyForLastSaveDirectorySetting = @"HKEY_CURRENT_USER\" + installationPathProvider.ApplicationRegistryPath;
        }

        public string ReadFromRegistry(string defaultValue)
        {
            try
            {
                return _registry.GetValue(_registryKeyForLastSaveDirectorySetting, RegistryKeyForLastSaveDirectory, defaultValue).ToString();
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        public void SaveInRegistry(string directory)
        {
            _registry.SetValue(_registryKeyForLastSaveDirectorySetting, RegistryKeyForLastSaveDirectory, directory);
        }

        public bool Apply(Job job)
        {
            return string.IsNullOrWhiteSpace(job.Profile.TargetDirectory);
        }
    }
}
