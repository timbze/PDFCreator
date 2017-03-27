using System.Text;
using NLog;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Actions.Queries;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Query;
using pdfforge.PDFCreator.Core.Workflow.Exceptions;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.ViewModels.Translations;

namespace pdfforge.PDFCreator.UI.ViewModels.WorkflowQuery
{
    public class InteractiveFtpPasswordProvider : IFtpPasswordProvider
    {
        private readonly IInteractionInvoker _interactionInvoker;
        private readonly InteractiveWorkflowTranslation _translation;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public InteractiveFtpPasswordProvider(IInteractionInvoker interactionInvoker, InteractiveWorkflowTranslation translation)
        {
            _interactionInvoker = interactionInvoker;
            _translation = translation;
        }

        public bool SetPassword(Job job)
        {
            job.Passwords.FtpPassword = job.Profile.Ftp.Password;

            if (!string.IsNullOrEmpty(job.Passwords.FtpPassword))
                return true;

            var result = QueryFtpPassword(job);

            if (result.Success)
            {
                job.Passwords.FtpPassword = result.Data;
                return true;
            }

            job.Profile.Ftp.Enabled = false;
            return false;
        }

        public ActionResult RetypePassword(Job job)
        {
            _logger.Debug("Retype FTP password started");

            var interaction = CreateAndInvokeInteraction(job, true);

            if (interaction.Result != PasswordResult.StorePassword)
                return new ActionResult(ErrorCode.Ftp_UserCancelled);  

            job.Passwords.FtpPassword = interaction.Password;
            return new ActionResult();
        }

        private QueryResult<string> QueryFtpPassword(Job job)
        {
            var result = new QueryResult<string>();

            var interaction = CreateAndInvokeInteraction(job, false);

            if (interaction.Result == PasswordResult.Skip)
            {
                _logger.Info("User skipped ftp password. Ftp upload disabled.");
                return result;
            }

            if (interaction.Result == PasswordResult.StorePassword)
            {
                result.Success = true;
                result.Data = interaction.Password;
                return result;
            }

            throw new AbortWorkflowException("Cancelled the FTP password dialog.");
        }

        private PasswordInteraction CreateAndInvokeInteraction(Job job, bool retype)
        {
            var sb = new StringBuilder();
            if (retype)
                sb.AppendLine(_translation.RetypeSmtpPwMessage);
            
            var title = _translation.PasswordTitle;
            var description = _translation.PasswordDescription;

            var button = retype ? PasswordMiddleButton.None : PasswordMiddleButton.Skip;
            var interaction = new PasswordInteraction(button, title, description, false)
            {
                Password = job.Passwords.FtpPassword,
                IntroText = sb.ToString()
            };

            _interactionInvoker.Invoke(interaction);

            return interaction;
        }
    }
}
