using Translatable;

namespace pdfforge.PDFCreator.Core.Services.Translation
{
    public class ProfileNameByGuidTranslation : ITranslatable
    {
        public string DefaultGuid { get; private set; } = "<Default Profile>";
        public string HighCompressionGuid { get; private set; } = "High Compression (small file)";
        public string HighQualityGuid { get; private set; } = "High Quality (large file)";
        public string JpegGuid { get; private set; } = "JPEG (graphic file)";
        public string PdfaGuid { get; private set; } = "PDF/A (long term preservation)";
        public string PngGuid { get; private set; } = "PNG (graphic file)";
        public string PrintGuid { get; private set; } = "Print after saving";
        public string TiffGuid { get; private set; } = "TIFF (multipage graphic file)";

        public string GetProfileGuidTranslation(string profileGuid)
        {
            if (profileGuid == "DefaultGuid")
                return DefaultGuid;

            if (profileGuid == "HighCompressionGuid")
                return HighCompressionGuid;
            if (profileGuid == "HighQualityGuid")
                return HighQualityGuid;
            if (profileGuid == "JpegGuid")
                return JpegGuid;
            if (profileGuid == "PdfaGuid")
                return PdfaGuid;
            if (profileGuid == "PrintGuid")
                return PrintGuid;
            if (profileGuid == "PngGuid")
                return PngGuid;
            if (profileGuid == "TiffGuid")
                return TiffGuid;

            return "";
        }
    }
}
