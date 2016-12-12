using pdfforge.PDFCreator.Conversion.ConverterInterface;

namespace pdfforge.PDFCreator.Conversion.Ghostscript.Conversion
{
    public class GhostscriptConverterFactory : IPsConverterFactory
    {
        private readonly GhostscriptVersion _ghostscriptVersion;

        public GhostscriptConverterFactory(IGhostscriptDiscovery ghostscriptDiscovery)
        {
            _ghostscriptVersion = ghostscriptDiscovery.GetGhostscriptInstance();
        }

        public IConverter BuildPsConverter()
        {
            return new GhostscriptConverter(_ghostscriptVersion);
        }
    }
}