using System;
using pdfforge.PDFCreator.Conversion.Jobs.FolderProvider;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.Core.Workflow;

namespace pdfforge.PDFCreator.Core.DirectConversion
{
    public interface IDirectConversionHelper
    {
        bool CanConvertDirectly(string file);
        void ConvertDirectly(string file);
    }

    public class DirectConversionHelper : IDirectConversionHelper
    {
        private readonly IJobInfoQueue _jobInfoQueue;
        private readonly IDirectConversionProvider _provider;
        private readonly ISettingsProvider _settingsProvider;
        private readonly ISpoolerProvider _spoolerProvider;
        private readonly IJobInfoManager _jobInfoManager;

        public DirectConversionHelper(IDirectConversionProvider provider, IJobInfoQueue jobInfoQueue, ISettingsProvider settingsProvider, ISpoolerProvider spoolerProvider, IJobInfoManager jobInfoManager)
        {
            _provider = provider;
            _jobInfoQueue = jobInfoQueue;
            _settingsProvider = settingsProvider;
            _spoolerProvider = spoolerProvider;
            _jobInfoManager = jobInfoManager;
        }

        public bool CanConvertDirectly(string file)
        {
            return IsPsFile(file) || IsPdfFile(file);
        }

        public void ConvertDirectly(string file)
        {
            if (!CanConvertDirectly(file))
                return;

            var converter = GetCorrectConverterForFile(file);

            var infFile = converter.TransformToInfFile(file, _spoolerProvider.SpoolFolder, _settingsProvider.Settings.ApplicationSettings.PrimaryPrinter);

            if (string.IsNullOrWhiteSpace(infFile))
                return;


            var jobInfo = _jobInfoManager.ReadFromInfFile(infFile);
            _jobInfoQueue.Add(jobInfo);
        }

        private bool IsPsFile(string file)
        {
            return file.EndsWith(".ps", StringComparison.InvariantCultureIgnoreCase);
        }

        private bool IsPdfFile(string file)
        {
            return file.EndsWith(".pdf", StringComparison.InvariantCultureIgnoreCase);
        }

        private IDirectConversion GetCorrectConverterForFile(string file)
        {
            return IsPdfFile(file) ? _provider.GetPdfConversion() : _provider.GetPsConversion();
        }
    }
}