using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using pdfforge.PDFCreator.ErrorReport;
using pdfforge.PDFCreator.Shared.Helper;
using pdfforge.PDFCreator.Shared.Licensing;
using pdfforge.PDFCreator.Utilities;

namespace pdfforge.PDFCreator.Assistants
{
    internal static class ErrorAssistant
    {
        private static string ComposeErrorText(Exception ex)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("Error Report for PDFCreator v" + UpdateAssistant.CurrentVersion);

            sb.AppendLine();
            sb.AppendLine("Exception:");
            sb.AppendLine(ex.ToString());

            sb.AppendLine();
            sb.AppendLine("Environment:");
            sb.AppendLine(Environment.OSVersion.VersionString);

            sb.AppendLine();
            sb.AppendLine("Environment variables:");
            var env = Environment.GetEnvironmentVariables();

            foreach (var item in env.Keys)
            {
                sb.AppendLine(item + "=" + env[item]);
            }

            return sb.ToString();
        }

        private static Report BuildReport(Exception ex)
        {
            var versionHelper = new VersionHelper();
            var report = Report.BuildReport("pdfcreator", versionHelper.FormatWithBuildNumber(), ex);

            var activation = EditionFactory.Instance.Edition.Activation;
            if (activation != null && activation.Product != 0)
            {
                report.AdditionalEntries["LicenseKey"] = activation.Key;
                report.AdditionalEntries["MachineID"] = activation.MachineId;
            }

            return report;
        }

        public static void ShowErrorWindow(Exception ex, out bool terminateApplication)
        {
            var report = BuildReport(ex);

            var err = new ErrorReportWindow(report, true);

            if (err.ShowDialog() == true)
                terminateApplication = true;
            else
                terminateApplication = false;
        }

        public static bool ShowErrorWindowInNewProcess(Exception ex)
        {
            var assemblyHelper = new AssemblyHelper();
            string errorReporterPath = assemblyHelper.GetCurrentAssemblyDirectory();
            errorReporterPath = Path.Combine(errorReporterPath, "ErrorReport.exe");

            if (!File.Exists(errorReporterPath))
                return false;

            try
            {
                string errorFile = Path.GetTempPath() + Guid.NewGuid() + ".err";

                ReportSerializer.SaveReport(BuildReport(ex), errorFile);

                Process.Start(errorReporterPath, "\"" + errorFile + "\"");
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
