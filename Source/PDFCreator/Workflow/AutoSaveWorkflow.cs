using System;
using System.IO;
using NLog;
using pdfforge.PDFCreator.Core.Actions;
using pdfforge.PDFCreator.Core.Jobs;
using pdfforge.PDFCreator.Core.Settings;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.Tokens;

namespace pdfforge.PDFCreator.Workflow
{
    /// <summary>
    /// The autosave workflow implements the workflow steps with auto save. Most interaction requests are answered with default values or are rejected, as no interaction is possible.
    /// </summary>
    internal class AutoSaveWorkflow : ConversionWorkflow
    {
        /// <summary>
        /// Create a new Workflow object with the given job info
        /// </summary>
        /// <param name="job">Job to use for the conversion</param>
        /// <param name="settings">Settings to use during the conversion workflow</param>
        public AutoSaveWorkflow(IJob job, PdfCreatorSettings settings)
        {
            WorkflowStep = WorkflowStep.Init;

            JobInfo = job.JobInfo;
            Job = job;
            Settings = settings;
        }

        protected override void QueryTargetFile()
        {
            TokenReplacer tr = Job.TokenReplacer;

            string outputFolder = FileUtil.Instance.MakeValidFolderName(tr.ReplaceTokens(Job.Profile.AutoSave.TargetDirectory));
            string filePath = Path.Combine(outputFolder, Job.ComposeOutputFilename());
            
            try
            {
                filePath = FileUtil.Instance.EllipsisForTooLongPath(filePath);
                Logger.Debug("FilePath after ellipsis: " + filePath);
            }
            catch (ArgumentException)
            {
                Logger.Error("Autosave filepath is only a directory or the directory itself is already too long to append a filename under the limits of Windows (max " 
                    + FileUtil.MAX_PATH + " characters): " + filePath);
                Cancel = true;
                return;
            }

            Job.OutputFilenameTemplate = filePath;
        }

        protected override bool QueryEmailSmtpPassword()
        {
            Job.Passwords.SmtpPassword = Job.Profile.EmailSmtp.Password;      
  
            return true;
        }

        protected override bool QueryFtpPassword()
        {
            Job.Passwords.FtpPassword = Job.Profile.Ftp.Password;

            return true;
        }

        protected override bool QueryEncryptionPasswords()
        {
            Job.Passwords.PdfOwnerPassword = Job.Profile.PdfSettings.Security.OwnerPassword;
            Job.Passwords.PdfUserPassword = Job.Profile.PdfSettings.Security.UserPassword;

            return true;
        }

        protected override bool QuerySignaturePassword()
        {
            Job.Passwords.PdfSignaturePassword = Job.Profile.PdfSettings.Signature.SignaturePassword;
            
            return true;
        }

        protected override void NotifyUserAboutFailedJob()
        {
            // maybe do something here?
        }

        protected override bool EvaluateActionResult(ActionResult actionResult)
        {
            return actionResult.Success;
        }

        protected override void RetypeSmtpPassword(object sender, QueryPasswordEventArgs e)
        {
            Logger.Error("No retype smtp password event in auto save mode.");
            e.Cancel = true;
        }

        protected override void RecommendPdfArchitect(object sender, EventArgs e)
        {
            Logger.Info("No recommend of PDF Architect in auto save mode.");
        }
    }
}
