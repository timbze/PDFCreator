using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.ViewModels.DialogViewModels;
using pdfforge.PDFCreator.UI.ViewModels.DialogViewModels.Translations;

namespace pdfforge.PDFCreator.UI.DesignTimeViewModels.Dialogs
{
    public class DesignTimePasswordViewModel : PasswordViewModel
    {
        public DesignTimePasswordViewModel() : base(new PasswordWindowTranslation())
        {
            var interaction = new PasswordInteraction(PasswordMiddleButton.Remove, "Set password (Design-Time value)", "Please enter your password:");
            interaction.Password = "123456789";
            interaction.IntroText = "An intro text can be displayed here.";
            SetInteraction(interaction);
        }
    }
}