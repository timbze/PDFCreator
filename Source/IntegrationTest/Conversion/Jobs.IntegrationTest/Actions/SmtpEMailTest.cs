using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Authentication;
using System.Text;
using System.Threading;
using ImapX;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using PDFCreator.TestUtilities;

namespace pdfforge.PDFCreator.IntegrationTest.Conversion.Jobs.Actions
{
    [TestFixture]
    [Category("LongRunning")]
    internal class SmtpEmailTest
    {
        [SetUp]
        public void SetUp()
        {
            var bootstrapper = new IntegrationTestBootstrapper();
            var container = bootstrapper.ConfigureContainer();
            _th = container.GetInstance<TestHelper>();
            _th.InitTempFolder("SmtpEmailTest");
        }

        [TearDown]
        public void CleanUp()
        {
            _th.CleanUp();
        }

        private TestHelper _th;

        private readonly string _mailServer = ParameterHelper.GetPassword("smtp_server");
        private readonly string _userName = ParameterHelper.GetPassword("smtp_user");
        private readonly string _eMailAddress = ParameterHelper.GetPassword("smtp_email");
        private readonly string _smtpPassword = ParameterHelper.GetPassword("smtp_password");
        private readonly int _smtpPort = int.Parse(ParameterHelper.GetPassword("smtp_port"));

        private readonly string _imapServer = ParameterHelper.GetPassword("imap_server");
        private readonly string _imapUserName = ParameterHelper.GetPassword("imap_user");
        private readonly string _imapPassword = ParameterHelper.GetPassword("imap_password");
        private readonly int _imapPort = int.Parse(ParameterHelper.GetPassword("imap_port"));

        private const string InboxName = "Inbox";

        private void InitTest()
        {
            _th.Profile.EmailSmtpSettings.Enabled = true;
            _th.Profile.EmailSmtpSettings.UserName = _userName;
            _th.Profile.EmailSmtpSettings.Address = _eMailAddress;
            _th.Profile.EmailSmtpSettings.Server = _mailServer;
            _th.Profile.EmailSmtpSettings.Port = _smtpPort;
            _th.Profile.EmailSmtpSettings.Ssl = true;
            _th.Profile.EmailSmtpSettings.Recipients = _eMailAddress;
        }

        private ImapClient CreateImapClient()
        {
            return new ImapClient(_imapServer, _imapPort, true, SslProtocols.Tls);
        }

        private void TestSmtpEmail(string outputFilenameTemplate)
        {
            #region Delete Mails in Inbox

            var imapClient = CreateImapClient();
            Assert.IsTrue(imapClient.IsConnected, "Could not connect to Imap");
            Assert.IsTrue(imapClient.LogIn(_imapUserName, _imapPassword), "Could no login to Imap");
            foreach (var msg in imapClient.Folders[InboxName].Messages)
                imapClient.Folders[InboxName].Messages.Remove(msg);
            imapClient.LogOut();
            imapClient.Disconnect();

            #endregion

            _th.GenerateGsJob(PSfiles.ThreePDFCreatorTestpages, _th.Profile.OutputFormat);
            _th.Job.Passwords.SmtpPassword = _smtpPassword;
            if (!string.IsNullOrEmpty(outputFilenameTemplate))
            {
                _th.SetFilenameTemplate(outputFilenameTemplate + Path.GetExtension(_th.Job.OutputFilenameTemplate));
            }
            _th.RunGsJob();

            #region Wait for new Mail in Inbox

            imapClient = CreateImapClient();
            Assert.IsTrue(imapClient.Connection(), "Could not connect to Imap");
            Assert.IsTrue(imapClient.LogIn(_imapUserName, _imapPassword), "Could no login to Imap-Server");
            var i = 0;
            while (imapClient.Folders[InboxName].Messages.Count == 0)
            {
                Thread.Sleep(2000);
                imapClient = CreateImapClient();
                Assert.IsTrue(imapClient.Connection(), "Could not connect to Imap");
                Assert.IsTrue(imapClient.LogIn(_imapUserName, _imapPassword), "Could no login to Imap-Server");
                Assert.Less(i++, 60, "Mail did not arrive within 2 Minutes");
            }

            #endregion

            var mail = imapClient.Folders[InboxName].Messages[0];
            mail.Process();

            Assert.AreEqual(_th.Profile.EmailSmtpSettings.Address, mail.From.Address, "Wrong sender");

            Assert.AreEqual(_th.Job.TokenReplacer.ReplaceTokens(_th.Profile.EmailSmtpSettings.Subject), mail.Subject.Replace("\n ", ""), "Incorrect mail-subject");

            var mailContent = mail.TextBody.ContentStream;
            if (mail.TextBody.ContentTransferEncoding == "base64")
                mailContent = Encoding.UTF8.GetString(Convert.FromBase64String(mailContent));

            Assert.AreEqual(_th.Job.TokenReplacer.ReplaceTokens(_th.Profile.EmailSmtpSettings.Content),
                mailContent.Replace("=\r\n", "").Replace("=0A", "\n").Replace("=0D", "\r"), "Incorrect mail-content");

            var recipientsList = new List<string>(_th.Profile.EmailSmtpSettings.Recipients.Trim().Replace(";", ",").Split(','));
            var recipientsMail = new List<string>(mail.To[0].Address.Trim().Replace("\n", "").Split(','));
            Assert.AreEqual(recipientsList, recipientsMail, "Not all the recipients were added in mail");

            Assert.AreEqual(_th.Job.OutputFiles.Count, mail.Attachments.Count, "Incorrect number of attached files");
            for (var j = 0; j < _th.Job.OutputFiles.Count; j++)
            {
                Assert.AreEqual(mail.Attachments[j].FileData, File.ReadAllBytes(_th.Job.OutputFiles[j]), "Data of " + _th.Job.OutputFiles[j] + " has changed");
            }
            for (var j = 0; j < _th.Job.OutputFiles.Count; j++)
            {
                Assert.AreEqual(mail.Attachments[j].FileName, Path.GetFileName(_th.Job.OutputFiles[j]), "Name of " + _th.Job.OutputFiles[j] + " has changed. Data was identical.");
            }

            imapClient.LogOut();
            imapClient.Disconnect();
        }

        [Test]
        [Ignore("Not working")]
        public void OnePdfFileAttached()
        {
            InitTest();
            _th.Profile.EmailSmtpSettings.Subject = "One Pdf-File attached";
            _th.Profile.EmailSmtpSettings.Content = "The following files are attached: <OutputFilenames:\r\n>";
            _th.Profile.EmailSmtpSettings.AddSignature = false;

            _th.Profile.OutputFormat = OutputFormat.Pdf;

            TestSmtpEmail("");
        }

        [Test]
        [Ignore("Not working")]
        public void ThreeJpegFilesAttached()
        {
            InitTest();
            _th.Profile.EmailSmtpSettings.Subject = "Three Jpeg-Files attached";
            _th.Profile.EmailSmtpSettings.Content = "The following files are attached: <OutputFilenames:\r\n>";
            _th.Profile.EmailSmtpSettings.AddSignature = false;

            _th.Profile.OutputFormat = OutputFormat.Jpeg;

            TestSmtpEmail("");
        }
    }
}