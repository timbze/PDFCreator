using pdfforge.PDFCreator.Conversion.Settings.Enums;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SecureTab.Sign
{
    public class SignUserControlTranslation : ITranslatable
    {
        public string SignatureTabHeader { get; private set; } = "Signature";
        public string CertificateFileLabel { get; private set; } = "Certificate File:";
        public string SignaturePasswordButton { get; private set; } = "Set Password";
        public string ReasonLabel { get; private set; } = "Reason:";
        public string ContactLabel { get; private set; } = "Contact:";
        public string LocationLabel { get; private set; } = "Location:";
        public string AllowMultiSigningCheckBox { get; private set; } = "Allow multiple signing";
        public string SelectTimeServerLabel { get; private set; } = "Select Time Server:";
        public string DisplaySignatureCheckBox { get; private set; } = "Display signature in document";
        public string SignaturePageLabel { get; private set; } = "Page:";
        public string LeftXLabel { get; private set; } = "Left X:";
        public string RightXLabel { get; private set; } = "Right X:";
        public string LeftYLabel { get; private set; } = "Left Y:";
        public string RightYLabel { get; private set; } = "Right Y:";
        public string PDFSignature { get; private set; } = "PDF Signature";
        public string SelectCertFile { get; private set; } = "Select certificate file";
        public string PfxP12Files { get; private set; } = "PFX/P12 files";
        public string AllFiles { get; private set; } = "All files";
        public string CertificateDoesNotExist { get; private set; } = "The certificate file does not exist.";
        public string SelectOrAddTimeServerAccount { get; protected set; } = "Select account or create a new one ->";

        public EnumTranslation<SignaturePage>[] SignaturePageValues { get; private set; } = EnumTranslation<SignaturePage>.CreateDefaultEnumTranslation();
    }
}
