using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Advanced.UserToken
{
    public class UserTokenTranslation : ITranslatable
    {
        public string UserTokenTitle { get; private set; } = "Extract UserTokens";
        public string DescriptionText { get; private set; } = "Define your own tokens directly in the original document with the following pattern: \"[[[UserTokenName: User token value]]]\". Afterwards the definied value can be called via the following token: \"<User:UserTokenName>\". The original pattern will be removed from the created document. You can define any number of user tokens. You can find more information in the user guide:";
        public string OnlyForBusinessText { get; private set; } = "The extraction of user tokens is available in the business editions of PDFCreator";
        public string UserGuideButtonText { get; private set; } = "More on user tokens";
    }
}
