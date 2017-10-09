namespace pdfforge.PDFCreator.Conversion.Settings
{
    partial class PdfCreatorSettings
    {
        public PdfCreatorSettings CopyAndPreserveApplicationSettings()
        {
            var copy = Copy();

            copy.ApplicationProperties = ApplicationProperties;
            copy.ApplicationSettings = ApplicationSettings;

            return copy;
        }
    }
}
