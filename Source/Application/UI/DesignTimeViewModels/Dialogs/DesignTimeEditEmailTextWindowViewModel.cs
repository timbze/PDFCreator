using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.ViewModels.DialogViewModels;
using pdfforge.PDFCreator.Utilities.Tokens;

namespace pdfforge.PDFCreator.UI.DesignTimeViewModels.Dialogs
{
    public class DesignTimeEditEmailTextWindowViewModel : EditEmailTextViewModel
    {
        public DesignTimeEditEmailTextWindowViewModel() : base(new TranslationProxy())
        {
            var interaction = new EditEmailTextInteraction("This is the mail subject <Counter>", "This is my content <Counter>", true);

            TokenReplacer = new TokenReplacer();
            TokenReplacer.AddStringToken("Counter", "123");

            SetInteraction(interaction);
        }
    }
}