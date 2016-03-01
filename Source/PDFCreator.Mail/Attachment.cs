namespace pdfforge.PDFCreator.Mail
{
    public class Attachment
    {
        public string Filename { get; set; }

        public Attachment(string filename)
        {
            Filename = filename;
        }
    }
}