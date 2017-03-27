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
        protected readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IPathSafe _pathSafe = new PathWrapSafe();
        protected readonly IFile File;
        private IProcessingPasswordsProvider PasswordsProvider { get; set; }

        protected PdfProcessorBase(IFile file, IProcessingPasswordsProvider passwordsProvider)
        {
            File = file;
            PasswordsProvider = passwordsProvider;
        }

        /// <summary>
        /// Inits the Profile 
        /// </summary>
        /// <param name="job"></param>
        public void Init(Job job)
        {
            Logger.Trace("Init Processor");
            
            //Must be applied before determining passwords
            ApplyRestrictionsToProfile(job);
            DeterminePasswords(job);
        }

        private void ApplyRestrictionsToProfile(Job job)
        {
            switch (job.Profile.OutputFormat)
            {
                case OutputFormat.PdfA1B:
                    if (job.Profile.PdfSettings.Security.Enabled)
                    {
                        job.Profile.PdfSettings.Security.Enabled = false;
                        Logger.Warn("Encryption automatically disabled for PDF/A1-b");
                    }
                    if (job.Profile.PdfSettings.Signature.Enabled)
                    {
                        if (!job.Profile.PdfSettings.Signature.AllowMultiSigning)
                        {
                            job.Profile.PdfSettings.Signature.AllowMultiSigning = true;
                            Logger.Warn("Allow multiple signing automatically enabled for PDF/A1-b");
                        }
                    }
                    return;

                case OutputFormat.PdfA2B:
                    if (job.Profile.PdfSettings.Security.Enabled)
                    {
                        job.Profile.PdfSettings.Security.Enabled = false;
                        Logger.Warn("Encryption automatically disabled for PDF/A2-b");
                    }
                    return;

                case OutputFormat.PdfX:
                    if (job.Profile.PdfSettings.Security.Enabled)
                    {
                        job.Profile.PdfSettings.Security.Enabled = false;
                        Logger.Warn("Encryption automatically disabled for PDF/X");
                    }
                    return;
            }
        }

        private void DeterminePasswords(Job job)
        {
            if ((job.Profile.OutputFormat == OutputFormat.Pdf)
               || (job.Profile.OutputFormat == OutputFormat.PdfA1B)
               || (job.Profile.OutputFormat == OutputFormat.PdfA2B)
               || (job.Profile.OutputFormat == OutputFormat.PdfX))
            {
                if (job.Profile.PdfSettings.Security.Enabled)
                {
                    Logger.Debug("Querying encryption passwords");
                    PasswordsProvider.SetEncryptionPasswords(job);
                }

                if (job.Profile.PdfSettings.Signature.Enabled)
                {
                    Logger.Debug("Querying signature password");
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
                case OutputFormat.PdfA1B:
                case OutputFormat.PdfA2B:
                    return true; //Always true because of the metadata update

                case OutputFormat.Pdf:
                    return profile.PdfSettings.Security.Enabled
                           || profile.BackgroundPage.Enabled
                           || profile.PdfSettings.Signature.Enabled;
         
                case OutputFormat.PdfX:
                    return profile.BackgroundPage.Enabled
                           || profile.PdfSettings.Signature.Enabled;
            }

            return false;
        }

        /// <summary>
        ///     Process PDF with updating metadata (for PDF/A), Encryption, adding background and signing according to profile.
        /// </summary>
        public void ProcessPdf(Job job)
        {
            var pdfFile = job.TempOutputFiles.First();
            Logger.Debug("Start processing of " + pdfFile);

            if (!File.Exists(pdfFile))
            {
                throw new ProcessingException("_file in PdfProcessor does not exist: " + pdfFile, ErrorCode.Processing_OutputFileMissing);
            }

            try
            {
                DoProcessPdf(job);
            }
            catch (ProcessingException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.GetType() + " while processing file:" + pdfFile + Environment.NewLine + ex.Message);
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
        /// <param name="profile">ConversionProfile</param>
        /// <returns>PDF Version as string, i.e. 1.4</returns>
        public string DeterminePdfVersion(ConversionProfile profile)
        {
            var pdfVersion = "1.4";

            if (profile.OutputFormat == OutputFormat.Pdf)
                if (profile.PdfSettings.Security.Enabled)
                    if (profile.PdfSettings.Security.EncryptionLevel == EncryptionLevel.Aes128Bit)
                        pdfVersion = "1.6";

            if (profile.OutputFormat != OutputFormat.PdfA1B)
                if (profile.PdfSettings.Signature.Enabled)
                    if (!profile.PdfSettings.Signature.AllowMultiSigning)
                        pdfVersion = "1.6";

            if (profile.OutputFormat == OutputFormat.Pdf)
                if (profile.PdfSettings.Security.Enabled)
                    if (profile.PdfSettings.Security.EncryptionLevel == EncryptionLevel.Aes256Bit)
                        pdfVersion = "1.7";

            if (profile.OutputFormat == OutputFormat.PdfA2B)
                pdfVersion = "1.7";

            return pdfVersion;
        }

        /// <summary>
        ///     Moves original file to preprocess file, which is the original file with an appended "_PrePdfProcessor.pdf".
        /// </summary>
        /// <param name="pdfFile">Full path to PDF file</param>
        /// <param name="appendix">filename is oldname_APPENDIX.pdf</param>
        /// <returns>Path to preprocess file</returns>
        public string MoveFileToPreProcessFile(string pdfFile, string appendix)
        {
            string preProcessFile;
            try
            {
                preProcessFile = _pathSafe.ChangeExtension(pdfFile, "_" + appendix + ".pdf").Replace("._", "_");
                File.Move(pdfFile, preProcessFile);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.GetType() + " while creating pdf preprocess file:" + Environment.NewLine + ex.Message);
                throw new ProcessingException(
                    ex.GetType() + " while creating pdf preprocess file:" + Environment.NewLine + ex.Message, ErrorCode.Processing_GenericError);
            }

            return preProcessFile;
        }

        protected abstract void DoProcessPdf(Job job);
    }
}