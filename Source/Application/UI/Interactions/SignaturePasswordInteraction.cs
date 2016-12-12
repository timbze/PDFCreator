using pdfforge.Obsidian.Interaction;
using pdfforge.PDFCreator.UI.Interactions.Enums;

namespace pdfforge.PDFCreator.UI.Interactions
{
    public class SignaturePasswordInteraction : IInteraction
    {
        public SignaturePasswordInteraction(PasswordMiddleButton middleButtonAction, string certificateFile)
        {
            MiddleButtonAction = middleButtonAction;
            CertificateFile = certificateFile;
        }

        public PasswordMiddleButton MiddleButtonAction { get; set; }
        public string CertificateFile { get; set; }

        public string Password { get; set; }

        public PasswordResult Result { get; set; }
    }
}