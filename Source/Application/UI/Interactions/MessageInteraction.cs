using pdfforge.Obsidian.Interaction;
using pdfforge.PDFCreator.UI.Interactions.Enums;

namespace pdfforge.PDFCreator.UI.Interactions
{
    public class MessageInteraction : IInteraction
    {
        public MessageInteraction(string text, string title, MessageOptions buttons, MessageIcon icon)
        {
            Text = text;
            Title = title;
            Buttons = buttons;
            Icon = icon;
        }

        public string Text { get; set; }
        public string Title { get; set; }
        public MessageOptions Buttons { get; set; }
        public MessageIcon Icon { get; set; }

        public MessageResponse Response { get; set; } = MessageResponse.Cancel;
    }
}