using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using pdfforge.PDFCreator.Core.Services.Licensing;
using pdfforge.PDFCreator.Core.Services.Logging;
using pdfforge.PDFCreator.ErrorReport;
using pdfforge.PDFCreator.Utilities;
using Tartaros;

namespace pdfforge.PDFCreator.Core.Services
{
    public class ErrorReportHelper
    {
        private readonly InMemoryLogger _inMemoryLogger;

        public ErrorReportHelper(InMemoryLogger inMemoryLogger)
        {
            _inMemoryLogger = inMemoryLogger;
        }

        public static IActivationHelper ActivationHelper { private get; set; }

        private Dictionary<string, string> BuildAdditionalEntries()
        {
            var additionalEntries = new Dictionary<string, string>();

            var activation = ActivationHelper.Activation;
            if (activation != null && activation.Product != 0)
            {
                additionalEntries["LicenseKey"] = activation.Key;
                additionalEntries["MachineID"] = activation.MachineId;
            }

            return additionalEntries;
        }

        private ErrorAssistant CreateErrorAssistant()
        {
            var assemblyHelper = new AssemblyHelper();
            return new ErrorAssistant("pdfcreator", assemblyHelper.GetPdfforgeAssemblyVersion());
        }

        private Report CreateReport(ErrorAssistant errorAssistant, Exception ex)
        {
            var report = errorAssistant.BuildReport(ex, BuildAdditionalEntries());

            report.Log.AddRange(_inMemoryLogger.LogEntries);

            return report;
        }

        public void ShowErrorReport(Exception ex)
        {
            var errorAssistant = CreateErrorAssistant();
            var report = CreateReport(errorAssistant, ex);

            errorAssistant.ShowErrorWindow(report);
        }

        public void ShowErrorReportInNewProcess(Exception ex)
        {
            var errorAssistant = CreateErrorAssistant();
            var report = CreateReport(errorAssistant, ex);

            var assemblyHelper = new AssemblyHelper();
            var errorReporterPath = assemblyHelper.GetPdfforgeAssemblyDirectory();
            errorReporterPath = Path.Combine(errorReporterPath, "ErrorReport.exe");

            if (!File.Exists(errorReporterPath))
                return;

            try
            {
                var errorFile = Path.GetTempPath() + Guid.NewGuid() + ".err";
                errorAssistant.SaveReport(report, errorFile);
                Process.Start(errorReporterPath, "\"" + errorFile + "\"");
            }
            catch
            {
            }
        }
    }
}