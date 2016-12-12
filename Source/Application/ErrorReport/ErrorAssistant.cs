using System;
using System.Collections.Generic;
using Tartaros;

namespace pdfforge.PDFCreator.ErrorReport
{
    public class ErrorAssistant
    {
        public ErrorAssistant(string productName, Version version)
        {
            Tartaros = new TartarosClient(productName, version, "pe2de9KLvfs6gArNEAdLKGmw");
        }

        internal TartarosClient Tartaros { get; }

        public Report BuildReport(Exception ex, Dictionary<string, string> additionalEntries)
        {
            var report = Tartaros.BuildErrorReport(ex);

            foreach (var key in additionalEntries.Keys)
            {
                report.AdditionalEntries[key] = additionalEntries[key];
            }

            return report;
        }

        public void ShowErrorWindow(Report report)
        {
            var err = new ErrorReportWindow(report, Tartaros);
            err.ShowDialog();
        }

        public bool SaveReport(Report report, string reportFile)
        {
            try
            {
                Tartaros.SaveReport(report, reportFile);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}