namespace pdfforge.PDFCreator.Core.DirectConversion
{
    public interface IDirectConversionProvider
    {
        IDirectConversion GetPdfConversion();

        IDirectConversion GetPsConversion();
    }

    public class DirectConversionProvider : IDirectConversionProvider
    {
        private readonly PdfDirectConversion _pdfConversion;
        private readonly PsDirectConversion _psConversion;

        public DirectConversionProvider(PdfDirectConversion pdfConversion, PsDirectConversion psConversion)
        {
            _pdfConversion = pdfConversion;
            _psConversion = psConversion;
        }

        public IDirectConversion GetPdfConversion()
        {
            return _pdfConversion;
        }

        public IDirectConversion GetPsConversion()
        {
            return _psConversion;
        }
    }
}
