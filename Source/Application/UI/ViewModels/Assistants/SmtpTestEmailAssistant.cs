using System.Linq;
using System.Text;
using SystemInterface.IO;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Actions.Actions;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.Core.Workflow;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.ViewModels.ActionViewModels.Translations;

namespace pdfforge.PDFCreator.UI.ViewModels.Assistants
{
    public interface ISmtpTest
    {
        void SendTestMail(ConversionProfile profile, Accounts accounts);
    }

    public class SmtpTestEmailAssistant : ISmtpTest
    {
        private readonly IFile _file;
        private readonly IInteractionInvoker _interactionInvoker;
        private readonly IPath _path;
        private readonly IMailSignatureHelper _mailSignatureHelper;
        private readonly ErrorCodeInterpreter _errorCodeInterpreter;
        private readonly ISmtpMailAction _smtpMailAction;
        private readonly SmtpSettingsAndActionControlTranslation _translation;

        public SmtpTestEmailAssistant(SmtpSettingsAndActionControlTranslation translation, IInteractionInvoker interactionInvoker, IFile file,
            ISmtpMailAction smtpMailAction, IPath path, IMailSignatureHelper mailSignatureHelper, ErrorCodeInterpreter errorCodeInterpreter)
        {
            _file = file;
            _translation = translation;
            _interactionInvoker = interactionInvoker;
            _smtpMailAction = smtpMailAction;
            _path = path;
            _mailSignatureHelper = mailSignatureHelper;
            _errorCodeInterpreter = errorCodeInterpreter;
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

            var testFile = _path.Combine(_path.GetTempPath(), "testfile.txt");
            _file.WriteAllText(testFile, @"PDFCreator", Encoding.GetEncoding("Unicode"));
            job.OutputFiles.Add(testFile);

            var recipients = currentProfile.EmailSmtpSettings.Recipients;

            actionResult = _smtpMailAction.ProcessJob(job);

            _file.Delete(testFile);
            DisplayResult(actionResult, recipients);
        }

        private bool TrySetJobPasswords(Job job, ConversionProfile profile)
        {
            if (!string.IsNullOrWhiteSpace(profile.EmailSmtpSettings.Password))
            {
                job.Passwords.SmtpPassword = profile.EmailSmtpSettings.Password;
                return true;
            }

            var sb = new StringBuilder();
            sb.AppendLine(_translation.RecipientsText);
            sb.AppendLine(profile.EmailSmtpSettings.Recipients);

            var title = _translation.SmtpPasswordTitle;
            var description = _translation.SmtpPasswordDescription;

            var interaction = new PasswordInteraction(PasswordMiddleButton.None, title, description, false);
            interaction.IntroText = sb.ToString();

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
            _interactionInvoker.Invoke(interaction);
        }
    }
}
