namespace pdfforge.PDFCreator.Conversion.Mail
{
    public class Attachment
    {
        public Attachment(string filename)
        {
            Filename = filename;
        }

        public string Filename { get; set; }
    }
}