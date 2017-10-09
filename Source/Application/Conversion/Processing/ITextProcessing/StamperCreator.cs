using iTextSharp.text.pdf;
using NLog;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using System;
using System.IO;

namespace pdfforge.PDFCreator.Conversion.Processing.ITextProcessing
{
    public static class StamperCreator
    {
        //ActionId = 25;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        ///     Create a PdfStamper with content of source file aiming to destination file.
        ///     If encryption is enabled, the stamper will be created with an corresponding signature,
        ///     which influences the PDF version. (1.4 for 40bit and 128bit encryption, 1.6 for 128bitAes).
        /// </summary>
        /// <param name="sourceFile">Full path to the source file</param>
        /// <param name="destinationFile">Full patch to the destination file</param>
        /// <param name="profile">Profile with encryption settings</param>
        /// <param name="pdfVersion">PDF Version as string, i.e. "1.6"</param>
        /// <returns>Stamper with content of source file stream, aiming to destination file</returns>
        /// <exception cref="ProcessingException">In case of any error</exception>
        internal static Tuple<PdfStamper, FileStream> CreateStamperAccordingToEncryptionAndSignature(string sourceFile, string destinationFile, ConversionProfile profile,
            char pdfVersion)
        {
            Tuple<PdfStamper, FileStream> stamper;

            try
            {
                stamper = DoCreateStamperAccordingToEncryptionAndSignature(sourceFile, destinationFile, profile, pdfVersion);
            }
            catch (ProcessingException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ProcessingException(ex.GetType() + " while creating stamper:" + Environment.NewLine + ex.Message, ErrorCode.Encryption_GenericError, ex);
            }

            return stamper;
        }

        private static Tuple<PdfStamper, FileStream> DoCreateStamperAccordingToEncryptionAndSignature(string sourceFilename, string destinationFilename, ConversionProfile profile, char pdfVersion)
        {
            Logger.Debug("Started creating PdfStamper according to Encryption.");

            var reader = new PdfReader(sourceFilename);
            var fileStream = new FileStream(destinationFilename, FileMode.Create, FileAccess.Write);
            PdfStamper stamper = null;

            if (profile.PdfSettings.Signature.Enabled)
                stamper = PdfStamper.CreateSignature(reader, fileStream, pdfVersion);
            else
                stamper = new PdfStamper(reader, fileStream, pdfVersion);

            if (stamper == null)
            {
                throw new ProcessingException("PDFStamper could not be created", ErrorCode.Encryption_Error);
            }
            return new Tuple<PdfStamper, FileStream>(stamper, fileStream);
        }
    }
}
