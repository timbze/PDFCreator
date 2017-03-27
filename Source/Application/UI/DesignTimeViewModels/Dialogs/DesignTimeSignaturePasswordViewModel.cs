using pdfforge.PDFCreator.UI.DesignTimeViewModels.Helper;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.ViewModels.DialogViewModels;
using pdfforge.PDFCreator.UI.ViewModels.DialogViewModels.Translations;

namespace pdfforge.PDFCreator.UI.DesignTimeViewModels.Dialogs
{
    public class DesignTimeSignaturePasswordViewModel : SignaturePasswordViewModel
    {
        public DesignTimeSignaturePasswordViewModel() : base(new DesignTimeSignaturePasswordCheck(), new SignaturePasswordWindowTranslation())
        {
            SetInteraction(new SignaturePasswordInteraction(PasswordMiddleButton.Remove, ""));
        }
    }
}