using pdfforge.Obsidian.Interaction;

namespace pdfforge.PDFCreator.UI.Interactions
{
    public class EditEmailTextInteraction : IInteraction
    {
        public EditEmailTextInteraction(string subject, string content, bool addSignature, bool html)
        {
            Subject = subject;
            Content = content;
            AddSignature = addSignature;
            Html = html;
        }

        public bool AddSignature { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public bool Html { get; set; }
        public bool Success { get; set; }
    }
}