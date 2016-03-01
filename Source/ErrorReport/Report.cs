using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace pdfforge.PDFCreator.ErrorReport
{
    [Serializable]
    public class Report
    {
        public string Product { get; set; } = "";
        public string Version { get; set; } = "";
        public string ErrorKey { get; set; } = "";
        public DateTime OccurredAt { get; set; } = DateTime.Now;
        public string ErrorType { get; set; } = "";
        public string ErrorMessage { get; set; } = "";
        public List<string> StackTrace { get; set; } = new List<string>();
        public string WindowsVersion { get; set; } = "";
        public string Platform { get; set; } = "";
        public IDictionary<string, string> EnvironmentVariables { get; set; } = new Dictionary<string, string>();
        public IDictionary<string, string> AdditionalEntries { get; set; } = new Dictionary<string, string>();

        public static Report BuildReport(string product, string version, Exception ex)
        {
            Report report = null;
            // Create report in own thread, which allows us to set the culture to english to receive an english stack trace
            var t = new System.Threading.Thread(() => { report = BuildReportInThread(product, version, ex); });
            t.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
            t.Start();
            t.Join();
            return report;
        }

        private static Report BuildReportInThread(string product, string version, Exception ex)
        {
            var report = new Report();

            var stackTrace = ex.StackTrace.Split('\n').Select(x => x.Trim()).ToList();

            report.Product = product;
            report.Version = version;

            report.ErrorKey = ex.GetType().Name;

            if (stackTrace.Any())
                report.ErrorKey += " " + stackTrace.First();

            report.OccurredAt = DateTime.Now;
            report.ErrorType = ex.GetType().Name;
            report.ErrorMessage = ex.Message;
            report.StackTrace = stackTrace;

            if (ex.InnerException != null)
                report.AdditionalEntries.Add("InnerException", ex.InnerException.ToString());

            report.WindowsVersion = Environment.OSVersion.VersionString;

            report.Platform = Environment.Is64BitOperatingSystem ? "x64" : "x86";

            foreach (DictionaryEntry de in Environment.GetEnvironmentVariables())
            {
                report.EnvironmentVariables.Add((string)de.Key, (string)de.Value);
            }

            return report;
        }

        public string ComposeErrorText()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("Error Report for PDFCreator " + Version);

            sb.AppendLine();
            sb.AppendLine("Exception:");
            sb.AppendLine(ErrorType);
            sb.AppendLine(ErrorMessage);
            sb.AppendLine(string.Join("\r\n", StackTrace));

            sb.AppendLine();
            sb.AppendLine("Environment:");
            sb.AppendLine(WindowsVersion);

            sb.AppendLine();
            sb.AppendLine("Platform:");
            sb.AppendLine(Platform);

            sb.AppendLine();
            sb.AppendLine("Environment variables:");
            foreach (KeyValuePair<string, string> additionalEntry in EnvironmentVariables)
            {
                sb.AppendLine($"{additionalEntry.Key}={additionalEntry.Value}");
            }

            sb.AppendLine();
            sb.AppendLine("Additional Data:");
            foreach (KeyValuePair<string, string> additionalEntry in AdditionalEntries)
            {
                sb.AppendLine($"{additionalEntry.Key}={additionalEntry.Value}");
            }

            return sb.ToString();
        }
    }
}
