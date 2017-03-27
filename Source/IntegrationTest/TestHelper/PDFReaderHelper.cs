using System.Text;
using iTextSharp.text.pdf;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;

namespace PDFCreator.TestUtilities
{
    public static class PdfReaderHelper
    {
        public static PdfReader BuildPdfReader(Job job)
        {
            return BuildPdfReader(job.OutputFiles[0], job.Profile, job.Passwords);
        }
       
        public static PdfReader BuildPdfReader(string testFile, ConversionProfile profile, JobPasswords passwords)
        {
            PdfReader pdfReader;
            if (profile.PdfSettings.Security.Enabled)
            {
                if (profile.PdfSettings.Security.RequireUserPassword)
                    pdfReader = new PdfReader(testFile, Encoding.Default.GetBytes(passwords.PdfUserPassword));
                else
                    pdfReader = new PdfReader(testFile, Encoding.Default.GetBytes(passwords.PdfOwnerPassword));
            }
            else
            {
                pdfReader = new PdfReader(testFile);
            }

            return pdfReader;
        }
    }
}
