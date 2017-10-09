namespace pdfforge.PDFCreator.UI.Presentation.Customization
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
