using Translatable;

namespace pdfforge.PDFCreator.UI.ViewModels.ActionViewModels.Translations
{
    public class UserTokenActionViewModelTranslation : ITranslatable
    {
        public string Description { get; private set; } = "Parse the document for UserTokens and set their values.";
        public string DisplayName { get; private set; } = "Activate UserTokens";
        public string BetaNoteText { get; private set; } = "Please note:";
        public string BetaNoteTextText { get; private set; } = "The user tokens feature is currently in beta status, and it is not considered stable. You are welcome to test and use it. If you should encounter any questions or problems, please feel free to contact our support. If you can also attach the document in which you experience the problems, we can help you in solving them.";
        public string DescriptionText { get; private set; } = "Define your own tokens directly in the original document with the following pattern: \"[[[UserTokenName: User token value]]]\". Afterwards the definied value can be called via the following token: \"<User:UserTokenName>\". The original pattern will be removed from the created document. You can define any number of user tokens. You can find more information in the user guide:";
        public string OnlyForBusinessText { get; private set; } = "The extraction of user tokens is available in the business editions of PDFCreator";
        public string UserGuideButtonText { get; private set; } = "More on user tokens";
    }
}
