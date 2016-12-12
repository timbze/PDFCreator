namespace pdfforge.PDFCreator.Conversion.Mail
{
    public interface IEmailClientFactory
    {
        IEmailClient CreateEmailClient();
    }
}