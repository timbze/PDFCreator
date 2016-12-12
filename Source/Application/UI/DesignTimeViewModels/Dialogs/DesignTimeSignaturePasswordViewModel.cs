using pdfforge.PDFCreator.UI.DesignTimeViewModels.Helper;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.ViewModels.DialogViewModels;

namespace pdfforge.PDFCreator.UI.DesignTimeViewModels.Dialogs
{
    public class DesignTimeSignaturePasswordViewModel : SignaturePasswordViewModel
    {
        public DesignTimeSignaturePasswordViewModel() : base(new DesignTimeSignaturePasswordCheck())
        {
            SetInteraction(new SignaturePasswordInteraction(PasswordMiddleButton.Remove, ""));
        }
    }
}