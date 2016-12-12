using NLog;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Query;
using pdfforge.PDFCreator.Conversion.Processing.PdfProcessingInterface;
using pdfforge.PDFCreator.Core.Workflow.Exceptions;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;

namespace pdfforge.PDFCreator.UI.ViewModels.WorkflowQuery
{
    public class InteractiveProcessingPasswordsProvider : IProcessingPasswordsProvider
    {
        private readonly IInteractionInvoker _interactionInvoker;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public InteractiveProcessingPasswordsProvider(IInteractionInvoker interactionInvoker)
        {
            _interactionInvoker = interactionInvoker;
        }

        public void SetSignaturePassword(Job job)
        {
            job.Passwords.PdfSignaturePassword = job.Profile.PdfSettings.Signature.SignaturePassword;

            if (string.IsNullOrWhiteSpace(job.Passwords.PdfSignaturePassword))
            {
                var result = QuerySignaturePassword(job.Profile.PdfSettings.Signature.CertificateFile);

                if (result.Success)
                {
                    job.Passwords.PdfSignaturePassword = result.Data;
                }
                else
                {
                    job.Profile.PdfSettings.Signature.Enabled = false;
                    _logger.Info("User skipped Signature Password. Signing disabled.");
                }
            }
        }

        public void SetEncryptionPasswords(Job job)
        {
            job.Passwords.PdfOwnerPassword = job.Profile.PdfSettings.Security.OwnerPassword;
            job.Passwords.PdfUserPassword = job.Profile.PdfSettings.Security.UserPassword;

            var askOwnerPw = string.IsNullOrEmpty(job.Profile.PdfSettings.Security.OwnerPassword);
            var askUserPw = job.Profile.PdfSettings.Security.RequireUserPassword && string.IsNullOrWhiteSpace(job.Profile.PdfSettings.Security.UserPassword);

            if (askOwnerPw || askUserPw)
            {
                var result = QueryEncryptionPassword(askOwnerPw, askUserPw);
                if (result.Success)
                {
                    if (askOwnerPw)
                        job.Passwords.PdfOwnerPassword = result.Data.OwnerPassword;

                    if (askUserPw)
                        job.Passwords.PdfUserPassword = result.Data.UserPassword;
                }
                else
                {
                    job.Profile.PdfSettings.Security.Enabled = false;
                    _logger.Info("User skipped encryption password dialog. Encryption disabled.");
                }
            }
        }

        private QueryResult<EncryptionPasswords> QueryEncryptionPassword(bool askOwnerPassword, bool askUserPassword)
        {
            var pwInteraction = new EncryptionPasswordInteraction(true, askOwnerPassword, askUserPassword);

            _interactionInvoker.Invoke(pwInteraction);

            var result = new QueryResult<EncryptionPasswords>();

            switch (pwInteraction.Response)
            {
                case PasswordResult.Skip:
                    _logger.Info("User skipped encryption password dialog. Encryption disabled.");
                    return result;

                case PasswordResult.StorePassword:
                    var password = new EncryptionPasswords
                    {
                        OwnerPassword = pwInteraction.OwnerPassword,
                        UserPassword = pwInteraction.UserPassword
                    };
                    result.Data = password;

                    result.Success = true;
                    return result;
            }

            throw new AbortWorkflowException("Cancelled setting encryption passwords.");
        }

        private QueryResult<string> QuerySignaturePassword(string certificateFile)
        {
            var pwInteraction = new SignaturePasswordInteraction(PasswordMiddleButton.Skip, certificateFile);

            _interactionInvoker.Invoke(pwInteraction);

            var result = new QueryResult<string>();

            switch (pwInteraction.Result)
            {
                case PasswordResult.Skip:
                    _logger.Info("User skipped Signature Password. Signing disabled.");
                    return result;

                case PasswordResult.StorePassword:
                    result.Data = pwInteraction.Password;
                    result.Success = true;
                    return result;
            }

            throw new AbortWorkflowException("Cancelled the signature password dialog.");
        }

        private class EncryptionPasswords
        {
            public string OwnerPassword { get; set; }
            public string UserPassword { get; set; }
        }
    }
}
