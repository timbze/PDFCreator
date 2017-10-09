using iTextSharp.text.pdf;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Processing.PdfProcessingInterface;
using pdfforge.PDFCreator.Conversion.Settings;
using System;
using System.IO;
using System.Linq;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.Conversion.Processing.ITextProcessing
{
    public class ITextPdfProcessor : PdfProcessorBase
    {
        public ITextPdfProcessor(IFile file) : base(file)
        { }

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
            if (intendedPdfVersion == "1.4")
                return PdfWriter.VERSION_1_4;

            return PdfWriter.VERSION_1_3;
        }

        protected override void DoProcessPdf(Job job)
        {
            var pdfFile = job.TempOutputFiles.First();
            var processedFile = Path.ChangeExtension(pdfFile, "_processed.pdf");
            var version = DeterminePdfVersionForPdfStamper(job.Profile);

            var stamperBundle = StamperCreator.CreateStamperAccordingToEncryptionAndSignature(pdfFile, processedFile, job.Profile, version);
            var stamper = stamperBundle.Item1;
            var outputStream = stamperBundle.Item2;

            try
            {
                //Encryption before adding Background and Signing!
                var encrypter = new Encrypter();
                encrypter.SetEncryption(stamper, job.Profile, job.Passwords);

                var metadataUpdater = new XmpMetadataUpdater();
                metadataUpdater.UpdateXmpMetadata(stamper, job.Profile);

                var backgroundAdder = new BackgroundAdder();
                backgroundAdder.AddBackground(stamper, job.Profile);

                //Signing after adding background and update metadata
                var signer = new ITextSigner();
                signer.SignPdfFile(stamper, job.Profile, job.Passwords, job.Accounts);
            }
            finally
            {
                try
                {
                    stamper?.Close();
                }
                catch (Exception ex)
                {
                    Logger.Warn($"Could not close iText pdf stamper.\r\n{ex}");
                }
                outputStream?.Close();
            }

            //Set the final output pdf
            job.TempOutputFiles.Clear();
            job.TempOutputFiles.Add(processedFile);
        }
    }
}
