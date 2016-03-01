using System.IO;
using Newtonsoft.Json;

namespace pdfforge.PDFCreator.ErrorReport
{
    public static class ReportSerializer
    {
        public static string ConvertToJson(Report report)
        {
            return JsonConvert.SerializeObject(report, Formatting.Indented);
        }

        public static void SaveReport(Report report, string filename)
        {
            string json = ConvertToJson(report);
            File.WriteAllText(filename, json);

        }

        public static Report DeserializeReport(string json)
        {
            return JsonConvert.DeserializeObject<Report>(json);
        }

        public static Report LoadReport(string filename)
        {
            var json = File.ReadAllText(filename);
            return DeserializeReport(json);
        }
    }
}
