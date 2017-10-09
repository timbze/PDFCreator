using pdfforge.Obsidian;
using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Conversion.Actions.Actions;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.Core.Workflow;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Overlay.Password;
using System.Linq;
using System.Text;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.UI.Presentation.Assistants
{
    public interface ISmtpTest
    {
        void SendTestMail(ConversionProfile profile, Accounts accounts);
    }

    public class SmtpTestEmailAssistant : ISmtpTest
    {
        private readonly IInteractionRequest _interactionRequest;
        private readonly IFile _file;
        private readonly IPath _path;
        private readonly IMailSignatureHelper _mailSignatureHelper;
        private readonly ErrorCodeInterpreter _errorCodeInterpreter;
        private readonly IInteractionInvoker _interactionInvoker;
        private readonly ISmtpMailAction _smtpMailAction;
        private SmtpTranslation _translation;

        public SmtpTestEmailAssistant(ITranslationUpdater translationUpdater, IInteractionRequest interactionRequest, IFile file,
            ISmtpMailAction smtpMailAction, IPath path, IMailSignatureHelper mailSignatureHelper, ErrorCodeInterpreter errorCodeInterpreter, IInteractionInvoker interactionInvoker)
        {
            _interactionRequest = interactionRequest;
            _file = file;
            translationUpdater.RegisterAndSetTranslation(tf => _translation = tf.UpdateOrCreateTranslation(_translation));
            _smtpMailAction = smtpMailAction;
            _path = path;
            _mailSignatureHelper = mailSignatureHelper;
            _errorCodeInterpreter = errorCodeInterpreter;
            _interactionInvoker = interactionInvoker;
        }

        public void SendTestMail(ConversionProfile profile, Accounts accounts)
        {
            var currentProfile = profile.Copy();

            currentProfile.AutoSave.Enabled = false;

            var actionResult = _smtpMailAction.Check(currentProfile, accounts);

            if (!actionResult.IsSuccess)
            {
                DisplayErrorMessage(actionResult);
                return;
            }

            var jobTranslations = new JobTranslations();
            jobTranslations.EmailSignature = _mailSignatureHelper.ComposeMailSignature();

            var job = CreateJob(jobTranslations, currentProfile, accounts);

            var success = TrySetJobPasswords(job, profile);
            if (!success)
                return;

            var testFile = _path.Combine(_path.GetTempPath(), _translation.AttachmentFile + ".pdf");
            _file.WriteAllText(testFile, @"PDFCreator", Encoding.GetEncoding("Unicode"));
            job.OutputFiles.Add(testFile);

            var recipients = currentProfile.EmailSmtpSettings.Recipients;

            actionResult = _smtpMailAction.ProcessJob(job);

            _file.Delete(testFile);
            DisplayResult(actionResult, recipients);
        }

        private bool TrySetJobPasswords(Job job, ConversionProfile profile)
        {
            var smtpAccount = job.Accounts.GetSmtpAccount(profile);
            if (smtpAccount == null)
            {
                var message = new MessageInteraction(_translation.NoAccount, "PDFCreator", MessageOptions.OK, MessageIcon.Error);
                _interactionRequest.Raise(message);
                return false;
            }

            job.Passwords.SmtpPassword = smtpAccount.Password;

            if (!string.IsNullOrWhiteSpace(job.Passwords.SmtpPassword))
            {
                return true;
            }

            var sb = new StringBuilder();
            sb.AppendLine(_translation.RecipientsLabel);
            sb.AppendLine(profile.EmailSmtpSettings.Recipients);

            var title = _translation.SetSmtpServerPassword;
            var description = _translation.SmtpServerPasswordLabel;

            var interaction = new PasswordOverlayInteraction(PasswordMiddleButton.None, title, description, false);
            interaction.IntroText = sb.ToString();

            // TODO: Can this be done with IInteractionRequest? (it's hard to wait for the response!)
            _interactionInvoker.Invoke(interaction);

            if (interaction.Result != PasswordResult.StorePassword)
                return false;

            job.Passwords.SmtpPassword = interaction.Password;
            return true;
        }

        private Job CreateJob(JobTranslations jobTranslations, ConversionProfile currentProfile, Accounts accounts)
        {
            var jobInfo = new JobInfo();
            var job = new Job(jobInfo, currentProfile, jobTranslations, accounts);

            return job;
        }

        private void DisplayResult(ActionResult actionResult, string recipients)
        {
            if (actionResult.IsSuccess)
            {
                var title = _translation.SendTestMail;
                var message = _translation.GetTestMailSentFormattedTranslation(recipients);
                DisplayMessage(message, title, MessageIcon.Info);
            }
            else
            {
                DisplayErrorMessage(actionResult);
            }
        }

        private void DisplayErrorMessage(ActionResult actionResult)
        {
            var title = _translation.SendTestMail;
            var message = GetErrorMessage(actionResult.First());
            DisplayMessage(message, title, MessageIcon.Error);
        }

        private string GetErrorMessage(ErrorCode errorCode)
        {
            return _errorCodeInterpreter.GetFirstErrorText(new ActionResult(errorCode), withNumber: false);
        }

        private void DisplayMessage(string message, string title, MessageIcon icon)
        {
            var interaction = new MessageInteraction(message, title, MessageOptions.OK, icon);
            _interactionRequest.Raise(interaction);
        }
    }
}
