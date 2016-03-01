namespace pdfforge.PDFCreator.Mail
{
    public interface IEmailClientFactory
    {
        IEmailClient CreateEmailClient();
    }
}
