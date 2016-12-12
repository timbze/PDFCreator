using System.Linq;
using System.Text;
using SystemInterface.IO;
using pdfforge.DynamicTranslator;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Actions.Actions;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.ViewModels.Helper;

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
        private readonly ISmtpMailAction _smtpMailAction;
        private readonly ITranslator _translator;

        public SmtpTestEmailAssistant(ITranslator translator, IInteractionInvoker interactionInvoker, IFile file,
            ISmtpMailAction smtpMailAction, IPath path)
        {
            _file = file;
            _translator = translator;
            _interactionInvoker = interactionInvoker;
            _smtpMailAction = smtpMailAction;
            _path = path;
        }

        public void SendTestMail(ConversionProfile profile, Accounts accounts)
        {
            var currentProfile = profile.Copy();

            currentProfile.AutoSave.Enabled = false;

            var signatureHelper = new MailSignatureHelper(_translator);

            var actionResult = _smtpMailAction.Check(currentProfile, accounts);

            if (!actionResult.IsSuccess)
            {
                DisplayErrorMessage(actionResult);
                return;
            }

            var jobTranslations = new JobTranslations();
            jobTranslations.EmailSignature = signatureHelper.ComposeMailSignature(currentProfile.EmailSmtpSettings);

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
            sb.AppendLine(
                _translator.GetTranslation("pdfforge.PDFCreator.UI.Views.ActionControls.EmailClientActionControl",
                    "RecipientsText.Text"));
            sb.AppendLine(profile.EmailSmtpSettings.Recipients);

            var title = _translator.GetTranslation("EmailClientActionSettings", "SmtpPasswordTitle");
            var description = _translator.GetTranslation("EmailClientActionSettings", "SmtpPasswordDescription");

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
                var title = _translator.GetTranslation("SmtpEmailActionSettings", "SendTestMail");
                var message = _translator.GetFormattedTranslation("SmtpEmailActionSettings", "TestMailSent", recipients);
                DisplayMessage(message, title, MessageIcon.Info);
            }
            else
            {
                DisplayErrorMessage(actionResult);
            }
        }

        private void DisplayErrorMessage(ActionResult actionResult)
        {
            var title = _translator.GetTranslation("SmtpEmailActionSettings", "SendTestMail");
            var message = GetErrorMessage(actionResult.First());
            DisplayMessage(message, title, MessageIcon.Error);
        }

        private string GetErrorMessage(ErrorCode errorCode)
        {
            var errorCodeInterpreter = new ErrorCodeInterpreter(_translator);
            return errorCodeInterpreter.GetFirstErrorText(new ActionResult(errorCode), withNumber: false);
        }

        private void DisplayMessage(string message, string title, MessageIcon icon)
        {
            var interaction = new MessageInteraction(message, title, MessageOptions.OK, icon);
            _interactionInvoker.Invoke(interaction);
        }
    }
}
