using System.Collections.Generic;

namespace pdfforge.PDFCreator.Conversion.Mail
{
    public class Email
    {
        public Email()
        {
            To = new List<string>();
            Attachments = new List<Attachment>();
        }

        public ICollection<string> To { get; private set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public ICollection<Attachment> Attachments { get; private set; }
        public bool Html { get; set; }
    }
}