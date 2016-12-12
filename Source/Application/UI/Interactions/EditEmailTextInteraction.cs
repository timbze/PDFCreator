using pdfforge.Obsidian.Interaction;

namespace pdfforge.PDFCreator.UI.Interactions
{
    public class EditEmailTextInteraction : IInteraction
    {
        public EditEmailTextInteraction(string subject, string content, bool addSignature)
        {
            Subject = subject;
            Content = content;
            AddSignature = addSignature;
        }

        public bool AddSignature { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }

        public bool Success { get; set; }
    }
}