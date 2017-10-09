using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Misc;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class DesignTimeMessageUserControlViewModel : MessageUserControlViewModel
    {
        public DesignTimeMessageUserControlViewModel() : base(new DesignTimeTranslationUpdater(), new SoundPlayer())
        {
            SetInteraction(new MessageInteraction("The message goes here.", "The Title", MessageOptions.OKCancel, MessageIcon.PDFForge));

            RightButtonContent = "Cancel";
            LeftButtonContent = "Ok";
        }
    }
}
