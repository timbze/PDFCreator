using pdfforge.Obsidian.Interaction;
using pdfforge.PDFCreator.UI.Interactions.Enums;

namespace pdfforge.PDFCreator.UI.Interactions
{
    public class PasswordInteraction : IInteraction
    {
        public PasswordInteraction(PasswordMiddleButton middleButtonAction, string title, string passwordDescription, bool showQueryPasswordHint = true)
        {
            PasswordDescription = passwordDescription;
            MiddleButtonAction = middleButtonAction;
            Title = title;
            ShowQueryPasswordHint = showQueryPasswordHint;
        }

        public string PasswordDescription { get; set; }
        public PasswordMiddleButton MiddleButtonAction { get; set; }
        public string Title { get; set; }
        public string Password { get; set; } = "";

        public string IntroText { get; set; } = "";

        public bool ShowQueryPasswordHint { get; set; } = true;

        public PasswordResult Result { get; set; } = PasswordResult.Cancel;
    }
}