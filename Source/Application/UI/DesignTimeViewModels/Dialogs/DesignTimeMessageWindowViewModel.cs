using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.ViewModels.DialogViewModels;
using pdfforge.PDFCreator.UI.ViewModels.Helper;

namespace pdfforge.PDFCreator.UI.DesignTimeViewModels.Dialogs
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class DesignTimeMessageWindowViewModel : MessageWindowViewModel
    {
        public DesignTimeMessageWindowViewModel() : base(new TranslationProxy(), new SoundPlayer())
        {
            SetInteraction(new MessageInteraction("The message goes here.", "The Title", MessageOptions.OKCancel, MessageIcon.PDFForge));

            RightButtonContent = "Cancel";
            LeftButtonContent = "Ok";
        }
    }
}