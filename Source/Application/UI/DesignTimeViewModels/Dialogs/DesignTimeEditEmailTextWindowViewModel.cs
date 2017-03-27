using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.ViewModels.DialogViewModels;
using pdfforge.PDFCreator.UI.ViewModels.DialogViewModels.Translations;
using pdfforge.PDFCreator.UI.ViewModels.Helper;
using pdfforge.PDFCreator.UI.ViewModels.Translations;
using pdfforge.PDFCreator.Utilities.Tokens;

namespace pdfforge.PDFCreator.UI.DesignTimeViewModels.Dialogs
{
    public class DesignTimeEditEmailTextWindowViewModel : EditEmailTextViewModel
    {
        public DesignTimeEditEmailTextWindowViewModel() : base(new EditEmailTextWindowTranslation(), 
            new MailSignatureHelperFreeVersion(new MailSignatureHelperTranslation()), new TokenHelper(new TokenPlaceHoldersTranslation()))
        {
            var interaction = new EditEmailTextInteraction("This is the mail subject <Counter>", "This is my content <Counter>", true, true);

            TokenReplacer = new TokenReplacer();
            TokenReplacer.AddStringToken("Counter", "123");

            SetInteraction(interaction);
        }
    }
}