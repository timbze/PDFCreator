using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SecureTab
{
    public class ProfileSecureTranslation : ITranslatable
    {
        public string Secure { get; private set; } = "Security";
        public string Encryption { get; private set; } = "Encryption";
        public string Signature { get; private set; } = "Signature";
    }
}
