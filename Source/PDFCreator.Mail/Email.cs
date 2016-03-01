using System.Collections.Generic;

namespace pdfforge.PDFCreator.Mail
{
    public class Email
    {
        public ICollection<string> To { get; private set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public ICollection<Attachment> Attachments { get; private set; }

        public Email()
        {
            To = new List<string>();
            Attachments = new List<Attachment>();
        }
    }
}