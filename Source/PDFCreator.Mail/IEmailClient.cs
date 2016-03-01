namespace pdfforge.PDFCreator.Mail
{
    public interface IEmailClient
    {
        bool ShowEmailClient(Email email);
        bool IsClientInstalled { get; }
    }
}
