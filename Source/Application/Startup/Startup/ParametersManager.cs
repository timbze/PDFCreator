using Microsoft.Win32;
using pdfforge.DataStorage.Storage;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.SettingsManagement;
using System;

namespace pdfforge.PDFCreator.Core.Startup
{
    public class ParametersManager : IParametersManager
    {
        private readonly IInstallationPathProvider _installationPathProvider;
        private readonly IDataStorageFactory _dataStorageFactory;

        public ParametersManager(IInstallationPathProvider installationPathProvider, IDataStorageFactory dataStorageFactory)
        {
            _installationPathProvider = installationPathProvider;
            _dataStorageFactory = dataStorageFactory;
        }

        private ParameterSettings LoadParameterSettings()
        {
            var regStorage = BuildStorage();
            var parametersettings = new ParameterSettings(regStorage);

            parametersettings.LoadData(regStorage, "");

            return parametersettings;
        }

        private IStorage BuildStorage()
        {
            return _dataStorageFactory.BuildRegistryStorage(RegistryHive.CurrentUser, _installationPathProvider.ApplicationRegistryPath);
        }

        private bool ParametersAreSet(Parameters parameters)
        {
            return !string.IsNullOrWhiteSpace(parameters.Outputfile) || !string.IsNullOrWhiteSpace(parameters.Profile);
        }

        public bool HasPredefinedParameters()
        {
            var parameters = LoadParameterSettings();

            return ParametersAreSet(parameters.Parameters);
        }

        public Parameters GetAndResetParameters()
        {
            var parameterSettings = LoadParameterSettings();

            if (!ParametersAreSet(parameterSettings.Parameters))
                throw new InvalidOperationException();

            var parameters = parameterSettings.Parameters;

            parameterSettings.Parameters = new Parameters();
            parameterSettings.SaveData("");

            return parameters;
        }

        public void SaveParameterSettings(string outputFile, string profileParameter)
        {
            var regStorage = BuildStorage();
            ParameterSettings parameterSettings = new ParameterSettings(regStorage);

            parameterSettings.Parameters.Outputfile = outputFile;
            parameterSettings.Parameters.Profile = profileParameter;
            parameterSettings.SaveData("");
        }
    }
}
