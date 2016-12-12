using pdfforge.DynamicTranslator;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Workflow;

namespace pdfforge.PDFCreator.UI.ViewModels.Helper
{
    public class MailSignatureHelper : IMailSignatureHelper
    {
        private readonly ITranslator _translator;

        public MailSignatureHelper(ITranslator translator)
        {
            _translator = translator;
        }

        public string ComposeMailSignature(EmailClientSettings mailSettings)
        {
            return ComposeMailSignature(mailSettings.AddSignature);
        }

        public string ComposeMailSignature(EmailSmtpSettings mailSettings)
        {
            return ComposeMailSignature(mailSettings.AddSignature);
        }

        public string ComposeMailSignature(bool addSignature = true)
        {
            if (!addSignature)
                return "";

            return "\r\n\r\n______________________________\r\n\r\n"
                   +
                   _translator.GetTranslation("EditEmailTextWindow", "PdfForgeSignature")
                   + "\r\nwww.pdfforge.org";
        }
    }
}