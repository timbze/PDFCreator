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
using pdfforge.PDFCreator.UI.ViewModels.ActionViewModels.Translations;

namespace pdfforge.PDFCreator.UI.ViewModels.WorkflowQuery
{
    public class InteractiveSmtpPasswordProvider : ISmtpPasswordProvider
    {
        private readonly IInteractionInvoker _interactionInvoker;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly SmtpSettingsAndActionControlTranslation _translation;

        public InteractiveSmtpPasswordProvider(SmtpSettingsAndActionControlTranslation translation, IInteractionInvoker interactionInvoker)
        {
            _translation = translation;
            _interactionInvoker = interactionInvoker;
        }

        public bool SetPassword(Job job)
        {
            job.Passwords.SmtpPassword = job.Profile.EmailSmtpSettings.Password;

            if (!string.IsNullOrWhiteSpace(job.Passwords.SmtpPassword))
                return true;

            var result = QuerySmtpPassword(job);
            if (result.Success)
            {
                job.Passwords.SmtpPassword = result.Data;
                return true;
            }
            job.Profile.EmailSmtpSettings.Enabled = false;
            return false;
        }

        public ActionResult RetypePassword(Job job)
        {
            _logger.Debug("Launched E-mail password Form");

            var interaction = CreateAndInvokeInteraction(job, true);

            if (interaction.Result != PasswordResult.StorePassword)
                return new ActionResult(ErrorCode.Smtp_UserCancelled);

            job.Passwords.SmtpPassword = interaction.Password;
            return new ActionResult();
        }

        private QueryResult<string> QuerySmtpPassword(Job job)
        {
            var result = new QueryResult<string>();

            var interaction = CreateAndInvokeInteraction(job, false);

            if (interaction.Result == PasswordResult.Skip)
            {
                _logger.Info("User skipped Smtp Password. Smtp Email disabled.");
                return result;
            }

            if (interaction.Result == PasswordResult.StorePassword)
            {
                result.Success = true;
                result.Data = interaction.Password;
                return result;
            }

            throw new AbortWorkflowException("Cancelled the SMTP dialog.");
        }

        private PasswordInteraction CreateAndInvokeInteraction(Job job, bool retype)
        {
            var sb = new StringBuilder();
            sb.AppendLine(_translation.RecipientsText);
            sb.AppendLine(job.Profile.EmailSmtpSettings.Recipients);
            if (retype)
            {
                sb.AppendLine();
                sb.AppendLine(_translation.RetypeSmtpPwMessage);
            }

            var title = _translation.SmtpPasswordTitle;
            var description = _translation.SmtpPasswordDescription;

            var button = retype ? PasswordMiddleButton.None : PasswordMiddleButton.Skip;
            var interaction = new PasswordInteraction(button, title, description, false)
            {
                Password = job.Passwords.SmtpPassword,
                IntroText = sb.ToString()
            };

            _interactionInvoker.Invoke(interaction);

            return interaction;
        }
    }
}
