using System.Drawing;

namespace pdfforge.PDFCreator.UI.Views
{
    public class ViewCustomization
    {
        private ViewCustomization()
        {
            
        }

        public ViewCustomization(string aboutDialogText, string mainWindowText, string printJobWindowCaption, Bitmap customlogo)
        {
            ApplyCustomization = true;
            AboutDialogText = aboutDialogText;
            MainWindowText = mainWindowText;
            PrintJobWindowCaption = printJobWindowCaption;
            CustomLogo = customlogo;
        }

        public static ViewCustomization DefaultCustomization => new ViewCustomization();

        public bool ApplyCustomization { get; private set; } = false;

        public string AboutDialogText { get; private set; } = "";

        public string MainWindowText { get; private set; } = "";

        public string PrintJobWindowCaption { get; private set; } = "PDFCreator";

        public Bitmap CustomLogo { get; private set; }
    }
}
