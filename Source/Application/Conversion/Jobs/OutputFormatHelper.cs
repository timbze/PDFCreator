using pdfforge.PDFCreator.Conversion.Settings.Enums;
using System;
using System.IO;
using System.Linq;

namespace pdfforge.PDFCreator.Conversion.Jobs
{
    public class OutputFormatHelper
    {
        public bool HasValidExtension(string file, OutputFormat outputFormat)
        {
            var extension = Path.GetExtension(file);

            if (extension == null)
                return false;

            var validExtensions = GetValidExtensions(outputFormat);

            return validExtensions.Contains(extension.ToLower());
        }

        public string EnsureValidExtension(string file, OutputFormat outputFormat)
        {
            if (HasValidExtension(file, outputFormat))
                return file;

            var validExtensions = GetValidExtensions(outputFormat);

            return Path.ChangeExtension(file, validExtensions.First());
        }

        public bool IsPdfFormat(OutputFormat outputFormat)
        {
            switch (outputFormat)
            {
                case OutputFormat.Pdf:
                case OutputFormat.PdfA1B:
                case OutputFormat.PdfA2B:
                case OutputFormat.PdfX:
                    return true;

                case OutputFormat.Jpeg:
                case OutputFormat.Png:
                case OutputFormat.Tif:
                case OutputFormat.Txt:
                    return false;
            }
            throw new NotImplementedException($"OutputFormat '{outputFormat}' is not known to {nameof(OutputFormatHelper)}!");
        }

        private string[] GetValidExtensions(OutputFormat outputFormat)
        {
            if (IsPdfFormat(outputFormat))
                return new[] { ".pdf" };

            switch (outputFormat)
            {
                case OutputFormat.Jpeg:
                    return new[] { ".jpg", ".jpeg" };

                case OutputFormat.Png:
                    return new[] { ".png" };

                case OutputFormat.Tif:
                    return new[] { ".tif", ".tiff" };

                case OutputFormat.Txt:
                    return new[] { ".txt" };
            }

            throw new NotImplementedException($"OutputFormat '{outputFormat}' is not known to {nameof(OutputFormatHelper)}!");
        }
    }
}
