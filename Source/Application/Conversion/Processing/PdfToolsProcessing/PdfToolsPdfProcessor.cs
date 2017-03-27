using System.IO;
using System.Linq;
using SystemInterface.IO;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Processing.PdfProcessingInterface;
using pdfforge.PDFCreator.Utilities;
using Pdftools.Pdf;
using Pdftools.PdfSecure;
using PdfTools.Pdf;

namespace pdfforge.PDFCreator.Conversion.Processing.PdfToolsProcessing
{
    public class PdfToolsPdfProcessor : PdfProcessorBase
    {
        public PdfToolsPdfProcessor(IFile file, IProcessingPasswordsProvider passwordsProvider) : base(file, passwordsProvider)
        { }

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
            string preProcessFile = null;
            Secure pdf = null;
            try
            {
                var pdfFile = job.TempOutputFiles.First();
                preProcessFile = MoveFileToPreProcessFile(pdfFile, "PreProcess");
                //CertRegistrar may only be desposed after encryption 
                using (var certRegistrar = new CertificateRegistrar(profile.PdfSettings.Signature, job.Passwords.PdfSignaturePassword))
                {
                    var backgroundAdder = new BackgroundAdder(this);
                    backgroundAdder.AddBackground(preProcessFile, profile);

                    var pdfAConverter = new PdfAConverter(this);
                    pdfAConverter.ConvertToPdfA(preProcessFile, profile);

                    pdf = new Secure();
                    pdf.Open(preProcessFile, "");

                    AssemblyHelper assemblyHelper = new AssemblyHelper();
                    pdf.SetInfoEntry("Producer", "PDFCreator " + assemblyHelper.GetPdfforgeAssemblyVersion());

                    //Bug in PDF Tools, Linearize breaks encryption
                    //pdf.Linearize = true;

                    var signer = new Signer();
                    var success = signer.SignPdfFile(pdf, profile, certRegistrar.Certificate);

                    if (success)
                    {
                        var encrypter = new Encrypter();
                        success = encrypter.SaveEncryptedFileAs(pdf, profile, job.Passwords, pdfFile);
                    }

                    if (!success)
                        Validate(pdf);

                    pdf.Close();
                }
            }
            finally
            {
                pdf?.Close();
                //delete preprocess file
                if (!string.IsNullOrEmpty(preProcessFile))
                    if (File.Exists(preProcessFile))
                        File.Delete(preProcessFile);
            }
        }

        private void Validate(Secure pdf)
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
