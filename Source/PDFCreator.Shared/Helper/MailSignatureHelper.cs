using pdfforge.PDFCreator.Core.Settings;

namespace pdfforge.PDFCreator.Shared.Helper
{
    public static class MailSignatureHelper
    {
        public static string ComposeMailSignature(EmailClient mailSettings)
        {
            return ComposeMailSignature(mailSettings.AddSignature);
        }

        public static string ComposeMailSignature(EmailSmtp mailSettings)
        {
            return ComposeMailSignature(mailSettings.AddSignature);
        }

        public static string ComposeMailSignature(bool addSignature = true)
        {
            if (!addSignature)
                return "";

            return "\r\n\r\n______________________________\r\n\r\n"
                                 +
                                 TranslationHelper.Instance.TranslatorInstance.GetTranslation("EditEmailTextWindow", "PdfForgeSignature",
                                     "E-mail automatically created by the free PDFCreator")
                                 + "\r\nwww.pdfforge.org";
        }
    }
}
