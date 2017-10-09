namespace pdfforge.PDFCreator.UI.Interactions
{
    public class SignaturePasswordInteraction : BasicPasswordOverlayInteraction
    {
        public SignaturePasswordInteraction(PasswordMiddleButton middleButtonAction, string certificateFile)
            : base(middleButtonAction)
        {
            MiddleButtonAction = middleButtonAction;
            CertificateFile = certificateFile;
        }

        public string CertificateFile { get; set; }
    }
}
