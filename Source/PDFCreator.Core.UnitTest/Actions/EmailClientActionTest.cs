using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using pdfforge.PDFCreator.Core.Actions;
using pdfforge.PDFCreator.Core.Jobs;
using pdfforge.PDFCreator.Core.Settings;
using pdfforge.PDFCreator.Mail;
using pdfforge.PDFCreator.Utilities.Tokens;
using Rhino.Mocks;

namespace PDFCreator.Core.UnitTest.Actions
{
    [TestFixture]
    public class EmailClientActionTest
    {
        private IJob _job;
        private ConversionProfile _profile;
        private IEmailClientFactory _emailClientFactory;
        private MockMailClient _mockMailClient;
        private TokenReplacer _tokenReplacer;

        private const string SignatureText = "Email automatically created by the free PDFCreator";

        [SetUp]
        public void SetUp()
        {
            _tokenReplacer = new TokenReplacer();

            _profile = new ConversionProfile();
            _profile.EmailClient.Enabled = true;
            _profile.EmailClient.Subject = "testsubject";
            _profile.EmailClient.Content = "This is content\r\nwith line breaks";
            _profile.EmailClient.AddSignature = false;
            _profile.EmailClient.Recipients = "test@local";

            _job = MockRepository.GenerateStub<IJob>();
            _job.TokenReplacer = _tokenReplacer;
            _job.OutputFiles = new[] { @"C:\Temp\file1.pdf" }.ToList();
            _job.Profile =_profile;

            _mockMailClient = new MockMailClient();

            _emailClientFactory = MockRepository.GenerateStub<IEmailClientFactory>();
            _emailClientFactory.Stub(x => x.CreateEmailClient()).Return(_mockMailClient);
        }

        [Test]
        public void EmailClientAction_WithSimpleJob_EmailIsProcessedByClient()
        {
            var action = new EMailClientAction(_emailClientFactory, SignatureText);

            action.ProcessJob(_job);

            Assert.IsNotEmpty(_mockMailClient.Mails);
        }

        [Test]
        public void EmailClientAction_CouldNotStartClient_ReturnsActionresultWithId100()
        {
            var action = new EMailClientAction(_emailClientFactory, SignatureText);
            _mockMailClient.WillFail = true;

            var result = action.ProcessJob(_job);

            Assert.AreEqual(11100, result[0]);
        }

        [Test]
        public void EmailClientAction_NoClientInstalled_ReturnsActionresultWithId101()
        {
            _emailClientFactory = MockRepository.GenerateStub<IEmailClientFactory>();
            _emailClientFactory.Stub(x => x.CreateEmailClient()).Return(null);
            var action = new EMailClientAction(_emailClientFactory, SignatureText);

            var result = action.ProcessJob(_job);

            Assert.AreEqual(11101, result[0]);
        }

        [Test]
        public void EmailClientAction_WhenExceptionIsThrown_ReturnsActionresultWithId999()
        {
            var action = new EMailClientAction(_emailClientFactory, SignatureText);
            _mockMailClient.ExceptionThrown = new Exception();

            var result = action.ProcessJob(_job);

            Assert.AreEqual(11999, result[0]);
        }

        [Test]
        public void EmailClientAction_WithMultipleRecipientsSeperatedBySemicolons_AllRecipientsListedInMail()
        {
            var action = new EMailClientAction(_emailClientFactory, SignatureText);
            _profile.EmailClient.Recipients = "a@local; b@local; c@local";

            action.ProcessJob(_job);

            var mail = _mockMailClient.Mails[0];
            Assert.AreEqual(new[] { "a@local", "b@local", "c@local" }.ToList(), mail.To);
        }

        [Test]
        public void EmailClientAction_WithMultipleRecipientsSeperatedByCommas_AllRecipientsListedInMail()
        {
            var action = new EMailClientAction(_emailClientFactory, SignatureText);
            _profile.EmailClient.Recipients = "a@local, b@local, c@local";

            action.ProcessJob(_job);

            var mail = _mockMailClient.Mails[0];
            Assert.AreEqual(new[] { "a@local", "b@local", "c@local" }.ToList(), mail.To);
        }

        [Test]
        public void EmailClientAction_WithEmptyRecipients_OnlyContainsValidInMail()
        {
            var action = new EMailClientAction(_emailClientFactory, SignatureText);
            _profile.EmailClient.Recipients = "a@local; ; b@local";

            action.ProcessJob(_job);

            var mail = _mockMailClient.Mails[0];
            Assert.AreEqual(new[] { "a@local", "b@local" }.ToList(), mail.To);
        }

        [Test]
        public void EmailClientAction_WithSubject_MailContainsSubject()
        {
            var action = new EMailClientAction(_emailClientFactory, SignatureText);
            _profile.EmailClient.Subject = "my subject";

            action.ProcessJob(_job);

            Assert.AreEqual(_mockMailClient.Mails[0].Subject, _profile.EmailClient.Subject);
        }

        [Test]
        public void EmailClientAction_SubjectWithToken_MailContainsReplacedSubject()
        {
            var action = new EMailClientAction(_emailClientFactory, SignatureText);
            _profile.EmailClient.Subject = "my subject <foo>";
            _tokenReplacer.AddStringToken("foo", "bar");

            action.ProcessJob(_job);

            Assert.AreEqual(_mockMailClient.Mails[0].Subject, "my subject bar");
        }

        [Test]
        public void EmailClientAction_WithBody_MailContainsBody()
        {
            var action = new EMailClientAction(_emailClientFactory, SignatureText);
            _profile.EmailClient.Content = "some content \r\nwith line breaks";

            action.ProcessJob(_job);

            Assert.AreEqual(_mockMailClient.Mails[0].Body, _profile.EmailClient.Content);
        }

        [Test]
        public void EmailClientAction_BodyWithToken_MailContainsReplacedBody()
        {
            var action = new EMailClientAction(_emailClientFactory, SignatureText);
            _profile.EmailClient.Content = "some content \r\nwith line breaks <foo>";
            _tokenReplacer.AddStringToken("foo", "bar");

            action.ProcessJob(_job);

            Assert.AreEqual(_mockMailClient.Mails[0].Body, "some content \r\nwith line breaks bar");
        }

        [Test]
        public void EmailClientAction_WithSignature_MailBodyContainsSignature()
        {
            var action = new EMailClientAction(_emailClientFactory, SignatureText);
            _profile.EmailClient.AddSignature = true;

            action.ProcessJob(_job);

            Assert.IsTrue(_mockMailClient.Mails[0].Body.Contains(SignatureText));
        }

        [Test]
        public void EmailClientAction_WithoutSignature_MailBodyDoesNotContainSignature()
        {
            var action = new EMailClientAction(_emailClientFactory, SignatureText);
            _profile.EmailClient.AddSignature = false;

            action.ProcessJob(_job);

            Assert.IsFalse(_mockMailClient.Mails[0].Body.Contains(SignatureText));
        }
    }

    class MockMailClient : IEmailClient
    {
        private bool _isClientInstalled = true;
        private IList<Email> _mails = new List<Email>();

        /// <summary>
        /// If true, the result of ShowEmailClient will be false.
        /// </summary>
        public bool WillFail { get; set; }

        /// <summary>
        /// If not null, this Exception will be thrown during ShowEmailClient
        /// </summary>
        public Exception ExceptionThrown { get; set; }

        /// <summary>
        /// List of mails that were "sent"
        /// </summary>
        public IList<Email> Mails
        {
            get { return _mails; }
        }

        public bool ShowEmailClient(Email email)
        {
            Mails.Add(email);

            if (ExceptionThrown != null)
                throw ExceptionThrown;

            return !WillFail;
        }

        public bool IsClientInstalled   
        {
            get { return _isClientInstalled; }
            set { _isClientInstalled = value; }
        }
    }
}
