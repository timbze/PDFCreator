using System.Linq;
using SystemInterface.IO;
using iTextSharp.text.pdf;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Processing.PdfProcessingInterface;
using pdfforge.PDFCreator.Conversion.Settings;

namespace pdfforge.PDFCreator.Conversion.Processing.ITextProcessing
{
    public class ITextPdfProcessor : PdfProcessorBase
    {
        public ITextPdfProcessor(IFile file, IProcessingPasswordsProvider passwordsProvider) : base(file, passwordsProvider)
        {  }

        /// <summary>
        ///     Determines number of pages in PDF file
        /// </summary>
        /// <param name="pdfFile">Full path to PDF file</param>
        /// <returns>Number of pages in pdf file</returns>
        public override int GetNumberOfPages(string pdfFile)
        {
            var pdfReader = new PdfReader(pdfFile);
            var numberOfPages = pdfReader.NumberOfPages;
            pdfReader.Close();
            return numberOfPages;
        }

        private char DeterminePdfVersionForPdfStamper(ConversionProfile profile)
        {
            var intendedPdfVersion = DeterminePdfVersion(profile);
            if (intendedPdfVersion == "1.7")
                return PdfWriter.VERSION_1_7;
            if (intendedPdfVersion == "1.6")
                return PdfWriter.VERSION_1_6;
            if (intendedPdfVersion == "1.5")
                return PdfWriter.VERSION_1_5;

            return PdfWriter.VERSION_1_4;
        }

        protected override void DoProcessPdf(Job job)
        {
            string preProcessFile = null;
            try
            {
                var pdfFile = job.TempOutputFiles.First();
                preProcessFile = MoveFileToPreProcessFile(pdfFile, "PreProcess");
                var version = DeterminePdfVersionForPdfStamper(job.Profile);
                using (var stamper = StamperCreator.CreateStamperAccordingToEncryptionAndSignature(preProcessFile, pdfFile, job.Profile, version))
                {
                    var encrypter = new Encrypter();
                    encrypter.SetEncryption(stamper, job.Profile, job.Passwords);
                    //Encryption before adding Background and Signing!

                    var metadataUpdater = new XmpMetadataUpdater();
                    metadataUpdater.UpdateXmpMetadata(stamper, job.Profile);

                    var backgroundAdder = new BackgroundAdder();
                    backgroundAdder.AddBackground(stamper, job.Profile);

                    var signer = new ITextSigner();
                    signer.SignPdfFile(stamper, job.Profile, job.Passwords);
                    //Signing after adding background and update metadata

                    stamper.Close();
                }
            }
            finally
            {
                //delete copy of original file
                if (!string.IsNullOrEmpty(preProcessFile))
                    if (File.Exists(preProcessFile))
                        File.Delete(preProcessFile);
            }
        }
    }
}