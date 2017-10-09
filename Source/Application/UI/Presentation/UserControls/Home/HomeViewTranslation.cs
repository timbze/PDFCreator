using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Home
{
    public class HomeViewTranslation : ITranslatable
    {
        private string CallToAction { get; set; } = "Print to '{0}' to get started!";
        public string ChooseFileButton { get; private set; } = "Choose a File to convert";
        public string DragDropText { get; private set; } = "... or simply drop files here!";

        public string FormatCallToAction(string printerName)
        {
            return string.Format(CallToAction, printerName);
        }
    }
}
