using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;

namespace pdfforge.PDFCreator.Conversion.Actions.Queries
{
    public interface ISmtpPasswordProvider
    {
        bool SetPassword(Job job);
        ActionResult RetypePassword(Job job);
    }
    
}