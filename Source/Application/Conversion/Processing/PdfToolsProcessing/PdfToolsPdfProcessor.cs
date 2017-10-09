using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Processing.PdfProcessingInterface;
using pdfforge.PDFCreator.Utilities;
using Pdftools.Pdf;
using Pdftools.PdfSecure;
using PdfTools.Pdf;
using System.IO;
using System.Linq;
using SystemInterface.IO;
using Path = System.IO.Path;

namespace pdfforge.PDFCreator.Conversion.Processing.PdfToolsProcessing
{
    public class PdfToolsPdfProcessor : PdfProcessorBase
    {
        private readonly ICertificateManager _certificateManager;
        private readonly IVersionHelper _versionHelper;

        public PdfToolsPdfProcessor(IFile file, ICertificateManager certificateManager, IVersionHelper versionHelper) : base(file)
        {
            _certificateManager = certificateManager;
            _versionHelper = versionHelper;
        }

        /// <summary>
        /// Determines number of pages in PDF file
        /// </summary>
        /// <param name="pdfFile">Full path to PDF file</param>
        /// <returns>Number of pages in pdf file</returns>
        public override int GetNumberOfPages(string pdfFile)
        {
            using (Stream docStream = new FileStream(pdfFile, FileMode.Open, FileAccess.Read))
            using (var doc = Document.Open(docStream, null))
            {
                return doc.Pages.Count;
            }
        }

        protected override void DoProcessPdf(Job job)
        {
            var profile = job.Profile;

            var pdfFile = job.TempOutputFiles.First();

            var backgroundAdder = new BackgroundAdder(this);
            pdfFile = backgroundAdder.AddBackground(pdfFile, profile);

            var pdfAConverter = new PdfAConverter(this);
            pdfFile = pdfAConverter.ConvertToPdfA(pdfFile, profile);

            using (var pdf = new Secure())
            {
                pdf.Open(pdfFile, "");

                pdf.SetInfoEntry("Producer", "PDFCreator " + _versionHelper.ApplicationVersion);

                //Bug in PDF Tools, Linearize breaks encryption
                //pdf.Linearize = true;

                var signer = new Signer(_certificateManager);
                var success = signer.SignPdfFile(pdf, profile, job.Passwords, job.Accounts);

                if (success)
                {
                    pdfFile = Path.ChangeExtension(pdfFile, "_secured.pdf");
                    var encrypter = new Encrypter();
                    success = encrypter.SaveEncryptedFileAs(pdf, profile, job.Passwords, pdfFile);
                }

                if (success)
                {
                    //Set the final output pdf
                    job.TempOutputFiles.Clear();
                    job.TempOutputFiles.Add(pdfFile);
                }
                else
                {
                    EvaluateError(pdf);
                }
            }
        }

        private void EvaluateError(Secure pdf)
        {
            switch (pdf.ErrorCode)
            {
                case PDFErrorCode.BSE_INFO_SUCCESS:
                    return;

                case PDFErrorCode.XMP_I_SREP_CHPREFIX: //Redundant XMP Metadata is acceptable.
                    return;

                case PDFErrorCode.SIG_CREA_E_INVCERT:
                    throw new ProcessingException("The signature is invalid or has expired.", ErrorCode.Signature_Invalid);
                case PDFErrorCode.SIG_CREA_E_TSP:
                    throw new ProcessingException("The time server is not available.", ErrorCode.Signature_NoTimeServerConnection);
                default:
                    throw new ProcessingException($"Error during Processing: {pdf.ErrorMessage} (ErrorCode {pdf.ErrorCode})", ErrorCode.Processing_GenericError);
            }
        }
    }
}
