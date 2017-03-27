using System;
using System.Collections.Generic;
using System.Linq;
using NSubstitute;
using NUnit.Framework;
using pdfforge.Mail;
using pdfforge.PDFCreator.Conversion.Actions.Actions;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Utilities.Tokens;

namespace pdfforge.PDFCreator.UnitTest.Conversion.Jobs.Actions
{
    [TestFixture]
    public class EmailClientActionTest
    {
        [SetUp]
        public void SetUp()
        {
            _tokenReplacer = new TokenReplacer();

            _profile = new ConversionProfile();
            _profile.EmailClientSettings.Enabled = true;
            _profile.EmailClientSettings.Subject = "testsubject";
            _profile.EmailClientSettings.Content = "This is content\r\nwith line breaks";
            _profile.EmailClientSettings.AddSignature = false;
            _profile.EmailClientSettings.Recipients = "test@local";

            _job = new Job(new JobInfo(), _profile, new JobTranslations(), new Accounts());
            _job.TokenReplacer = _tokenReplacer;
            _job.OutputFiles = new[] {@"C:\Temp\file1.pdf"}.ToList();
            _job.Profile = _profile;
            _job.JobTranslations = new JobTranslations();
            _job.JobTranslations.EmailSignature = SignatureText;

            _mockMailClient = new MockMailClient();

            _emailClientFactory = Substitute.For<IEmailClientFactory>();
            _emailClientFactory.CreateEmailClient().Returns(_mockMailClient);
        }

        private Job _job;
        private ConversionProfile _profile;
        private IEmailClientFactory _emailClientFactory;
        private MockMailClient _mockMailClient;
        private TokenReplacer _tokenReplacer;

        private const string SignatureText = "Email automatically created by the free PDFCreator";

        [Test]
        public void EmailClientAction_BodyWithToken_MailContainsReplacedBody()
        {
            var action = new EMailClientAction(_emailClientFactory);
            _profile.EmailClientSettings.Content = "some content \r\nwith line breaks <foo>";
            _tokenReplacer.AddStringToken("foo", "bar");

            action.ProcessJob(_job);

            Assert.AreEqual(_mockMailClient.Mails[0].Body, "some content \r\nwith line breaks bar");
        }

        [Test]
        public void EmailClientAction_CouldNotStartClient_ReturnsActionresultWithId100()
        {
            var action = new EMailClientAction(_emailClientFactory);
            _mockMailClient.WillFail = true;

            var result = action.ProcessJob(_job);

            Assert.AreEqual(ErrorCode.MailClient_GenericError, result[0]);
        }

        [Test]
        public void EmailClientAction_NoClientInstalled_ReturnsActionresultWithId101()
        {
            _emailClientFactory = Substitute.For<IEmailClientFactory>();
            _emailClientFactory.CreateEmailClient().Returns(x => null);
            var action = new EMailClientAction(_emailClientFactory);

            var result = action.ProcessJob(_job);

            Assert.AreEqual(ErrorCode.MailClient_NoCompatibleEmailClientInstalled, result[0]);
        }

        [Test]
        public void EmailClientAction_SubjectWithToken_MailContainsReplacedSubject()
        {
            var action = new EMailClientAction(_emailClientFactory);
            _profile.EmailClientSettings.Subject = "my subject <foo>";
            _tokenReplacer.AddStringToken("foo", "bar");

            action.ProcessJob(_job);

            Assert.AreEqual(_mockMailClient.Mails[0].Subject, "my subject bar");
        }

        [Test]
        public void EmailClientAction_WhenExceptionIsThrown_ReturnsActionresultWithId999()
        {
            var action = new EMailClientAction(_emailClientFactory);
            _mockMailClient.ExceptionThrown = new Exception();

            var result = action.ProcessJob(_job);

            Assert.AreEqual(ErrorCode.MailClient_GenericError, result[0]);
        }

        [Test]
        public void EmailClientAction_WithBody_MailContainsBody()
        {
            var action = new EMailClientAction(_emailClientFactory);
            _profile.EmailClientSettings.Content = "some content \r\nwith line breaks";

            action.ProcessJob(_job);

            Assert.AreEqual(_mockMailClient.Mails[0].Body, _profile.EmailClientSettings.Content);
        }

        [Test]
        public void EmailClientAction_WithEmptyRecipients_OnlyContainsValidInMail()
        {
            var action = new EMailClientAction(_emailClientFactory);
            _profile.EmailClientSettings.Recipients = "a@local; ; b@local";

            action.ProcessJob(_job);

            var mail = _mockMailClient.Mails[0];
            Assert.AreEqual(new[] {"a@local", "b@local"}.ToList(), mail.To);
        }

        [Test]
        public void EmailClientAction_WithMultipleRecipientsSeperatedByCommas_AllRecipientsListedInMail()
        {
            var action = new EMailClientAction(_emailClientFactory);
            _profile.EmailClientSettings.Recipients = "a@local, b@local, c@local";

            action.ProcessJob(_job);

            var mail = _mockMailClient.Mails[0];
            Assert.AreEqual(new[] {"a@local", "b@local", "c@local"}.ToList(), mail.To);
        }

        [Test]
        public void EmailClientAction_WithMultipleRecipientsSeperatedBySemicolons_AllRecipientsListedInMail()
        {
            var action = new EMailClientAction(_emailClientFactory);
            _profile.EmailClientSettings.Recipients = "a@local; b@local; c@local";

            action.ProcessJob(_job);

            var mail = _mockMailClient.Mails[0];
            Assert.AreEqual(new[] {"a@local", "b@local", "c@local"}.ToList(), mail.To);
        }

        [Test]
        public void EmailClientAction_WithoutSignature_MailBodyDoesNotContainSignature()
        {
            var action = new EMailClientAction(_emailClientFactory);
            _profile.EmailClientSettings.AddSignature = false;

            action.ProcessJob(_job);

            Assert.IsFalse(_mockMailClient.Mails[0].Body.Contains(SignatureText));
        }

        [Test]
        public void EmailClientAction_WithSignature_MailBodyContainsSignature()
        {
            var action = new EMailClientAction(_emailClientFactory);
            _profile.EmailClientSettings.AddSignature = true;

            action.ProcessJob(_job);

            Assert.IsTrue(_mockMailClient.Mails[0].Body.Contains(SignatureText));
        }

        [Test]
        public void EmailClientAction_WithSimpleJob_EmailIsProcessedByClient()
        {
            var action = new EMailClientAction(_emailClientFactory);

            action.ProcessJob(_job);

            Assert.IsNotEmpty(_mockMailClient.Mails);
        }

        [Test]
        public void EmailClientAction_WithSubject_MailContainsSubject()
        {
            var action = new EMailClientAction(_emailClientFactory);
            _profile.EmailClientSettings.Subject = "my subject";

            action.ProcessJob(_job);

            Assert.AreEqual(_mockMailClient.Mails[0].Subject, _profile.EmailClientSettings.Subject);
        }

        [Test]
        public void EmailClientAction_AttachesFiles()
        {
            var action = new EMailClientAction(_emailClientFactory);

            _profile.EmailClientSettings.Subject = "my subject";

            action.ProcessJob(_job);

            Assert.AreEqual(1, _mockMailClient.Mails[0].Attachments.Count);
        }

        [Test]
        public void EmailClientAction_DropboxShareLinks_DoesNotAttachFiles()
        {
            var action = new EMailClientAction(_emailClientFactory);
            _profile.DropboxSettings.Enabled = true;
            _profile.DropboxSettings.CreateShareLink = true;

            _profile.EmailClientSettings.Subject = "my subject";
            _profile.EmailClientSettings.Content = "<DropboxFullLinks>";

            action.ProcessJob(_job);

            Assert.AreEqual(0, _mockMailClient.Mails[0].Attachments.Count);
        }

        [Test]
        public void EmailClientAction_DropboxEnabledWithoutShareLinks_AttachesFiles()
        {
            var action = new EMailClientAction(_emailClientFactory);
            _profile.DropboxSettings.Enabled = true;
            _profile.DropboxSettings.CreateShareLink = false;

            _profile.EmailClientSettings.Subject = "my subject";
            _profile.EmailClientSettings.Content = "<DropboxFullLinks>";

            action.ProcessJob(_job);

            Assert.AreEqual(1, _mockMailClient.Mails[0].Attachments.Count);
        }

        [Test]
        public void EmailClientAction_DropboxEnabledWithoutToken_AttachesFiles()
        {
            var action = new EMailClientAction(_emailClientFactory);
            _profile.DropboxSettings.Enabled = true;
            _profile.DropboxSettings.CreateShareLink = true;

            _profile.EmailClientSettings.Subject = "my subject";

            action.ProcessJob(_job);

            Assert.AreEqual(1, _mockMailClient.Mails[0].Attachments.Count);
        }

        [Test]
        public void CheckEmailClientInstalled_ClientCanBeCreated_ReturnsTrue()
        {
            var action = new EMailClientAction(_emailClientFactory);
            
            Assert.IsTrue(action.CheckEmailClientInstalled());
        }

        [Test]
        public void CheckEmailClientInstalled_ClientCannotBeCreated_ReturnsFalse()
        {
            _emailClientFactory.CreateEmailClient().Returns(x => null);
            var action = new EMailClientAction(_emailClientFactory);

            Assert.IsFalse(action.CheckEmailClientInstalled());
        }

        [Test]
        public void IsEnabled_WhenEnabled_ReturnsTrue()
        {
            var action = new EMailClientAction(_emailClientFactory);
            _profile.EmailClientSettings.Enabled = true;

            Assert.IsTrue(action.IsEnabled(_profile));
        }

        [Test]
        public void IsEnabled_WhenNotEnabled_ReturnsFalse()
        {
            var action = new EMailClientAction(_emailClientFactory);
            _profile.EmailClientSettings.Enabled = false;

            Assert.IsFalse(action.IsEnabled(_profile));
        }
    }

    internal class MockMailClient : IEmailClient
    {
        /// <summary>
        ///     If true, the result of ShowEmailClient will be false.
        /// </summary>
        public bool WillFail { get; set; }

        /// <summary>
        ///     If not null, this Exception will be thrown during ShowEmailClient
        /// </summary>
        public Exception ExceptionThrown { get; set; }

        /// <summary>
        ///     List of mails that were "sent"
        /// </summary>
        public IList<Email> Mails { get; } = new List<Email>();

        public bool ShowEmailClient(Email email)
        {
            Mails.Add(email);

            if (ExceptionThrown != null)
                throw ExceptionThrown;

            return !WillFail;
        }

        public bool IsClientInstalled { get; set; } = true;
    }
}