using System.Linq;
using SystemInterface.IO;
using iTextSharp.text.pdf;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Processing.PdfProcessingInterface;

namespace pdfforge.PDFCreator.Conversion.Processing.ITextProcessing
{
    public class ITextPdfProcessor : PdfProcessorBase
    {
        public ITextPdfProcessor(IFile file, IProcessingPasswordsProvider passwordsProvider)
        {
            File = file;
            PasswordsProvider = passwordsProvider;
        }

        protected override IFile File { get; }
        protected override IProcessingPasswordsProvider PasswordsProvider { get; }

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

        protected override void DoProcessPdf(Job job)
        {
            string preProcessFile = null;
            try
            {
                var pdfFile = job.TempOutputFiles.First();
                preProcessFile = MoveFileToPreProcessFile(pdfFile);
                var version = DeterminePdfVersion(job.Profile.PdfSettings);
                using (var stamper = StamperCreator.CreateStamperAccordingToEncryptionAndSignature(preProcessFile, pdfFile, job.Profile, version))
                {
                    var encrypter = new Encrypter();
                    encrypter.SetEncryption(stamper, job.Profile, job.Passwords);
                    //Encryption before adding Background and Signing!

                    var metadataUpdater = new XmpMetadataUpdater();
                    metadataUpdater.UpdateXmpMetadata(stamper, job.Profile);

                    var backgroundAdder = new BackgroundAdder();
                    backgroundAdder.AddBackground(stamper, job.Profile);

                    var signer = new Signer();
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