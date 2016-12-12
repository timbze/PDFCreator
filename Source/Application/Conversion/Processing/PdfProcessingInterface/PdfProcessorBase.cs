using System;
using System.Linq;
using SystemInterface.IO;
using SystemWrapper.IO;
using NLog;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;

namespace pdfforge.PDFCreator.Conversion.Processing.PdfProcessingInterface
{
    public abstract class PdfProcessorBase : IPdfProcessor
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IPathSafe _pathSafe = new PathWrapSafe();

        protected abstract IFile File { get; }
        protected abstract IProcessingPasswordsProvider PasswordsProvider { get; }

        public void Init(Job job)
        {
            _logger.Trace("Init Processor");

            if ((job.Profile.OutputFormat == OutputFormat.Pdf)
               || (job.Profile.OutputFormat == OutputFormat.PdfA1B)
               || (job.Profile.OutputFormat == OutputFormat.PdfA2B)
               || (job.Profile.OutputFormat == OutputFormat.PdfX))
            {
                if (job.Profile.PdfSettings.Security.Enabled)
                {
                    _logger.Debug("Querying encryption passwords");
                    PasswordsProvider.SetEncryptionPasswords(job);
                }

                if (job.Profile.PdfSettings.Signature.Enabled)
                {
                    _logger.Debug("Querying signature password");
                    PasswordsProvider.SetSignaturePassword(job);
                }
            }
        }

        /// <summary>
        ///     Check if processing is required.
        ///     Therefore the output format must be pdf and security or background page or signing must be enabled.
        ///     Furthermore PDF/A requires processing the metadata.
        /// </summary>
        /// <param name="profile">Profile with PDF settings</param>
        /// <returns>True if processing is required</returns>
        public bool ProcessingRequired(ConversionProfile profile)
        {
            switch (profile.OutputFormat)
            {
                case OutputFormat.Pdf:
                case OutputFormat.PdfA2B:
                case OutputFormat.PdfX:
                    return profile.PdfSettings.Security.Enabled
                           || profile.BackgroundPage.Enabled
                           || profile.OutputFormat == OutputFormat.PdfA2B
                           || profile.PdfSettings.Signature.Enabled;
            }

            return false;
        }

        /// <summary>
        ///     Process PDF with updating metadata (for PDF/A), Encryption, adding background and signing according to profile.
        /// </summary>
        public void ProcessPdf(Job job)
        {
            if (!ProcessingRequired(job.Profile))
            {
                _logger.Debug("No processing required.");
                return;
            }
            var pdfFile = job.TempOutputFiles.First();
            _logger.Debug("Start processing of " + pdfFile);

            if (!File.Exists(pdfFile))
            {
                throw new ProcessingException("File in PdfProcessor does not exist: " + pdfFile, ErrorCode.Processing_OutputFileMissing);
            }

            try
            {
                DoProcessPdf(job);
            }
            catch (ProcessingException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.GetType() + " while processing file:" + pdfFile + Environment.NewLine + ex.Message);
                throw new ProcessingException(
                    ex.GetType() + " while processing file:" + pdfFile + Environment.NewLine + ex.Message, ErrorCode.Processing_GenericError);
            }
        }

        /// <summary>
        ///     Determines number of pages in PDF file
        /// </summary>
        /// <param name="pdfFile">Full path to PDF file</param>
        /// <returns>Number of pages in pdf file</returns>
        public abstract int GetNumberOfPages(string pdfFile);

        /// <summary>
        ///     Determine PDF-Version according to settings in conversion profile.
        /// </summary>
        /// <param name="settings">ConversionProfile</param>
        /// <returns>PDF Version as string, i.e. 1.6</returns>
        public string DeterminePdfVersion(PdfSettings settings)
        {
            var pdfVersion = "1.4";
            if (settings.Security.Enabled && (settings.Security.EncryptionLevel == EncryptionLevel.Aes128Bit))
                pdfVersion = "1.6";
            return pdfVersion;
        }


        /// <summary>
        ///     Moves original file to preprocess file, which is the original file with an appended "_PrePdfProcessor.pdf".
        /// </summary>
        /// <param name="pdfFile">Full path to PDF file</param>
        /// <returns>Path to preprocess file</returns>
        protected string MoveFileToPreProcessFile(string pdfFile)
        {
            string preProcessFile;
            try
            {
                //create copy of original file 
                preProcessFile = _pathSafe.ChangeExtension(pdfFile, "_PrePdfProcessor.pdf").Replace("._", "_");

                File.Move(pdfFile, preProcessFile);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.GetType() + " while creating pdf preprocess file:" + Environment.NewLine + ex.Message);
                throw new ProcessingException(
                    ex.GetType() + " while creating pdf preprocess file:" + Environment.NewLine + ex.Message, ErrorCode.Processing_GenericError);
            }

            return preProcessFile;
        }

        protected abstract void DoProcessPdf(Job job);
    }
}