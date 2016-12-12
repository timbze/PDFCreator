namespace pdfforge.PDFCreator.UI.ViewModels.WindowViewModels
{
    public class ButtonDisplayOptions
    {
        public ButtonDisplayOptions(bool hideSocialMediaButtons, bool hideDonateButton)
        {
            HideSocialMediaButtons = hideSocialMediaButtons;
            HideDonateButton = hideDonateButton;
        }

        public bool HideSocialMediaButtons { get; }
        public bool HideDonateButton { get; }
    }
}