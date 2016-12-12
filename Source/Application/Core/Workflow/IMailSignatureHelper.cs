using pdfforge.PDFCreator.Conversion.Settings;

namespace pdfforge.PDFCreator.Core.Workflow
{
    public interface IMailSignatureHelper
    {
        string ComposeMailSignature(EmailClientSettings emailClientSettings);
        string ComposeMailSignature(EmailSmtpSettings emailSmtpSettings);
        string ComposeMailSignature(bool addSignature = true);
    }
}