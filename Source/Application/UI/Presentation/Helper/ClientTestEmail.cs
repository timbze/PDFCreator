using NLog;
using pdfforge.Mail;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Workflow;
using System;
using SystemInterface.IO;
using SystemWrapper.IO;

namespace pdfforge.PDFCreator.UI.Presentation.Helper
{
    public interface IClientTestEmail
    {
        bool SendTestEmail(EmailClientSettings clientSettings);
    }

    public class ClientTestEmail : IClientTestEmail
    {
        private readonly IEmailClientFactory _emailClientFactory;
        private readonly IMailSignatureHelper _mailSignatureHelper;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IPath _path;
        private readonly IPathSafe _pathSafe = new PathWrapSafe();
        private readonly IDirectory _directory;
        private readonly IFile _file;
        private readonly TokenHelper _tokenHelper;

        public ClientTestEmail(IEmailClientFactory emailClientFactory, IMailSignatureHelper mailSignatureHelper, IPath path, IDirectory directory, IFile file, TokenHelper tokenHelper)
        {
            _emailClientFactory = emailClientFactory;
            _mailSignatureHelper = mailSignatureHelper;
            _path = path;
            _directory = directory;
            _file = file;
            _tokenHelper = tokenHelper;
        }

        public bool SendTestEmail(EmailClientSettings clientSettings)
        {
            var emailClient = _emailClientFactory.CreateEmailClient();

            if (emailClient == null)
            {
                return false;
            }

            var eMail = new Email();
            var tokenReplacer = _tokenHelper.TokenReplacerWithPlaceHolders;

            clientSettings.Recipients = tokenReplacer.ReplaceTokens(clientSettings.Recipients);
            foreach (var recipient in clientSettings.Recipients.Replace(',', ';').Split(';'))
            {
                if (!string.IsNullOrWhiteSpace(recipient))
                    eMail.To.Add(recipient.Trim());
            }

            eMail.Subject = tokenReplacer.ReplaceTokens(clientSettings.Subject);
            eMail.Html = clientSettings.Html;
            eMail.Body = tokenReplacer.ReplaceTokens(clientSettings.Content);

            if (clientSettings.AddSignature)
            {
                var signature = _mailSignatureHelper.ComposeMailSignature();
                if (clientSettings.Html)
                    signature = signature.Replace(Environment.NewLine, "<br/>");
                eMail.Body += signature;
            }

            try
            {
                var tempFolder = _path.GetTempPath();
                var tmpTestFolder = _pathSafe.Combine(tempFolder, "PDFCreator-Test\\SendSmtpTestmail");
                _directory.CreateDirectory(tmpTestFolder);
                var tmpFile = _pathSafe.Combine(tmpTestFolder, "PDFCreator Mail Client Test.pdf");
                _file.WriteAllText(tmpFile, "");
                eMail.Attachments.Add(new Attachment(tmpFile));

                emailClient.ShowEmailClient(eMail);
            }
            catch (Exception ex)
            {
                _logger.Warn(ex, "Exception while creating mail");
                return false;
            }
            return true;
        }
    }
}
