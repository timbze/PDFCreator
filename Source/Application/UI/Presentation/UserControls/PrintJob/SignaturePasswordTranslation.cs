namespace pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob
{
    public class SignaturePasswordTranslation : PdfPasswordTranslation
    {
        public string SiganturePasswordTitle { get; private set; } = "Signature";
        public string CertificatePassword { get; private set; } = "Certificate _Password:";
        public string CertificateFile { get; private set; } = "Certificate File:";
    }
}
