using System.Text;
using iTextSharp.text.pdf;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Processing.PdfProcessingInterface;
using pdfforge.PDFCreator.Conversion.Settings;

namespace PDFCreator.TestUtilities
{
    public static class PdfVersionTester
    {
        public static void CheckPDFVersion(Job job, IPdfProcessor pdfProcessor)
        {
            CheckPDFVersion(job.OutputFiles[0], job.Profile, job.Passwords, pdfProcessor);
        }

        public static void CheckPDFVersion(string testFile, ConversionProfile profile, JobPasswords passwords, IPdfProcessor pdfProcessor)
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

            CheckPDFVersion(pdfReader, profile, pdfProcessor);

            pdfReader.Close();
        }

        public static void CheckPDFVersion(PdfReader pdfReader, ConversionProfile profile, IPdfProcessor pdfProcessor)
        {
            var expectedVersion = pdfProcessor.DeterminePdfVersion(profile);
            if (expectedVersion == "1.7")
                Assert.AreEqual(PdfWriter.VERSION_1_7, pdfReader.PdfVersion, "Not PDF-Version 1.7");
            else if (expectedVersion == "1.6")
                Assert.AreEqual(PdfWriter.VERSION_1_6, pdfReader.PdfVersion, "Not PDF-Version 1.6");
            else if (expectedVersion == "1.5")
                Assert.AreEqual(PdfWriter.VERSION_1_5, pdfReader.PdfVersion, "Not PDF-Version 1.5");
            else
                Assert.AreEqual(PdfWriter.VERSION_1_4, pdfReader.PdfVersion, "Not PDF-Version 1.4");
        }
    }
}
