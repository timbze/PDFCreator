using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.ViewModels.WindowViewModels;
using pdfforge.PDFCreator.UI.ViewModels.WindowViewModels.Translations;
using pdfforge.PDFCreator.Utilities.Process;

namespace pdfforge.PDFCreator.UI.DesignTimeViewModels.Windows
{
    public class DesignTimePlusHintViewModel : PlusHintWindowViewModel
    {
        public DesignTimePlusHintViewModel() : base(new PlusHintWindowTranslation(), new ProcessStarter())
        {
            SetInteraction(new PlusHintInteraction(213));
            ThankYouText = $"You have already converted {Interaction.CurrentJobCount} files!";
        }
    }
}