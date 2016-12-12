using System;
using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Actions.Actions;
using pdfforge.PDFCreator.Conversion.Actions.Queries;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;

namespace pdfforge.PDFCreator.UnitTest.Conversion.Jobs.Actions
{
    [TestFixture]
    public class SmtpActionTest
    {
        [SetUp]
        public void SetUp()
        {
            _profile = new ConversionProfile();
            _smtpAction = new SmtpMailAction(Substitute.For<ISmtpPasswordProvider>());
        }

        private ConversionProfile _profile;
        private SmtpMailAction _smtpAction;

        private void SetValidAutoSaveSettings()
        {
            _profile.AutoSave.Enabled = true;
            _profile.AutoSave.TargetDirectory = "random autosave directory";
            _profile.FileNameTemplate = "random autosave filename";
        }

        [Test]
        public void SmtpSettings_AutoSave_MultipleErrors()
        {
            _profile.EmailSmtpSettings.Enabled = true;
            _profile.EmailSmtpSettings.Ssl = false;
            _profile.EmailSmtpSettings.Subject = "";
            _profile.EmailSmtpSettings.Content = "";
            _profile.EmailSmtpSettings.AddSignature = false;

            _profile.EmailSmtpSettings.Address = "";
            _profile.EmailSmtpSettings.UserName = "";
            _profile.EmailSmtpSettings.Server = "";
            _profile.EmailSmtpSettings.Port = -1;
            _profile.EmailSmtpSettings.Recipients = "";
            SetValidAutoSaveSettings();
            _profile.EmailSmtpSettings.Password = "";
            var result = _smtpAction.Check(_profile, new Accounts());
            Assert.Contains(ErrorCode.Smtp_NoEmailAddress, result, "ProfileCheck did not detect missing SMTP adress.");
            result.Remove(ErrorCode.Smtp_NoEmailAddress);
            Assert.Contains(ErrorCode.Smtp_NoUserSpecified, result, "ProfileCheck did not detect missing SMTP username.");
            result.Remove(ErrorCode.Smtp_NoUserSpecified);
            Assert.Contains(ErrorCode.Smtp_NoServerSpecified, result, "ProfileCheck did not detect missing SMTP host.");
            result.Remove(ErrorCode.Smtp_NoServerSpecified);
            Assert.Contains(ErrorCode.Smtp_InvalidPort, result, "ProfileCheck did not detect invalid SMTP port.");
            result.Remove(ErrorCode.Smtp_InvalidPort);
            Assert.Contains(ErrorCode.Smtp_NoRecipients, result, "ProfileCheck did not detect missing SMTP recipients.");
            result.Remove(ErrorCode.Smtp_NoRecipients);
            Assert.Contains(ErrorCode.Smtp_NoPasswordSpecified, result, "ProfileCheck did not detect missing SMTP password for autosaving.");
            result.Remove(ErrorCode.Smtp_NoPasswordSpecified);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void SmtpSettings_AutoSave_NoPassword()
        {
            _profile.EmailSmtpSettings.Enabled = true;
            _profile.EmailSmtpSettings.Ssl = false;
            _profile.EmailSmtpSettings.Subject = "";
            _profile.EmailSmtpSettings.Content = "";
            _profile.EmailSmtpSettings.AddSignature = false;

            _profile.EmailSmtpSettings.Address = "randomAdress@randomHost.random";
            _profile.EmailSmtpSettings.UserName = "randomUsername";
            _profile.EmailSmtpSettings.Server = "randomHost";
            _profile.EmailSmtpSettings.Port = 25;
            _profile.EmailSmtpSettings.Recipients = "randomRecipient@RandomHost.random";
            SetValidAutoSaveSettings();
            _profile.EmailSmtpSettings.Password = "";
            var result = _smtpAction.Check(_profile, new Accounts());
            Assert.Contains(ErrorCode.Smtp_NoPasswordSpecified, result, "ProfileCheck did not detect missing SMTP password for autosaving.");
            result.Remove(ErrorCode.Smtp_NoPasswordSpecified);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void SmtpSettings_AutoSave_valid()
        {
            _profile.EmailSmtpSettings.Enabled = true;
            _profile.EmailSmtpSettings.Ssl = false;
            _profile.EmailSmtpSettings.Subject = "";
            _profile.EmailSmtpSettings.Content = "";
            _profile.EmailSmtpSettings.AddSignature = false;

            _profile.EmailSmtpSettings.Address = "randomAdress@randomHost.random";
            _profile.EmailSmtpSettings.UserName = "randomUsername";
            _profile.EmailSmtpSettings.Server = "randomHost";
            _profile.EmailSmtpSettings.Port = 25;
            _profile.EmailSmtpSettings.Recipients = "randomRecipient@RandomHost.random";
            SetValidAutoSaveSettings();
            _profile.EmailSmtpSettings.Password = "1234";
            var result = _smtpAction.Check(_profile, new Accounts());
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void SmtpSettings_NoAutoSave_InvalidPort()
        {
            _profile.EmailSmtpSettings.Enabled = true;
            _profile.EmailSmtpSettings.Ssl = false;
            _profile.EmailSmtpSettings.Subject = "";
            _profile.EmailSmtpSettings.Content = "";
            _profile.EmailSmtpSettings.AddSignature = false;

            _profile.EmailSmtpSettings.Address = "randomAdress@randomHost.random";
            _profile.EmailSmtpSettings.UserName = "randomUsername";
            _profile.EmailSmtpSettings.Server = "randomHost";
            _profile.EmailSmtpSettings.Port = -1;
            _profile.EmailSmtpSettings.Recipients = "randomRecipient@RandomHost.random";
            _profile.AutoSave.Enabled = false;
            _profile.EmailSmtpSettings.Password = "";
            var result = _smtpAction.Check(_profile, new Accounts());
            Assert.Contains(ErrorCode.Smtp_InvalidPort, result, "ProfileCheck did not detect invalid SMTP port.");
            result.Remove(ErrorCode.Smtp_InvalidPort);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void SmtpSettings_NoAutoSave_MultipleErrors()
        {
            _profile.EmailSmtpSettings.Enabled = true;
            _profile.EmailSmtpSettings.Ssl = false;
            _profile.EmailSmtpSettings.Subject = "";
            _profile.EmailSmtpSettings.Content = "";
            _profile.EmailSmtpSettings.AddSignature = false;

            _profile.EmailSmtpSettings.Address = "";
            _profile.EmailSmtpSettings.UserName = "";
            _profile.EmailSmtpSettings.Server = "";
            _profile.EmailSmtpSettings.Port = -1;
            _profile.EmailSmtpSettings.Recipients = "";
            _profile.AutoSave.Enabled = false;
            _profile.EmailSmtpSettings.Password = "";
            var result = _smtpAction.Check(_profile, new Accounts());
            Assert.Contains(ErrorCode.Smtp_NoEmailAddress, result, "ProfileCheck did not detect missing SMTP adress.");
            result.Remove(ErrorCode.Smtp_NoEmailAddress);
            Assert.Contains(ErrorCode.Smtp_NoUserSpecified, result, "ProfileCheck did not detect missing SMTP username.");
            result.Remove(ErrorCode.Smtp_NoUserSpecified);
            Assert.Contains(ErrorCode.Smtp_NoServerSpecified, result, "ProfileCheck did not detect missing SMTP host.");
            result.Remove(ErrorCode.Smtp_NoServerSpecified);
            Assert.Contains(ErrorCode.Smtp_InvalidPort, result, "ProfileCheck did not detect invalid SMTP port.");
            result.Remove(ErrorCode.Smtp_InvalidPort);
            Assert.Contains(ErrorCode.Smtp_NoRecipients, result, "ProfileCheck did not detect missing SMTP recipients.");
            result.Remove(ErrorCode.Smtp_NoRecipients);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void SmtpSettings_NoAutoSave_NoAdress()
        {
            _profile.EmailSmtpSettings.Enabled = true;
            _profile.EmailSmtpSettings.Ssl = false;
            _profile.EmailSmtpSettings.Subject = "";
            _profile.EmailSmtpSettings.Content = "";
            _profile.EmailSmtpSettings.AddSignature = false;

            _profile.EmailSmtpSettings.Address = "";
            _profile.EmailSmtpSettings.UserName = "randomUsername";
            _profile.EmailSmtpSettings.Server = "randomHost";
            _profile.EmailSmtpSettings.Port = 25;
            _profile.EmailSmtpSettings.Recipients = "randomRecipient@RandomHost.random";
            _profile.AutoSave.Enabled = false;
            _profile.EmailSmtpSettings.Password = "";
            var result = _smtpAction.Check(_profile, new Accounts());
            Assert.Contains(ErrorCode.Smtp_NoEmailAddress, result, "ProfileCheck did not detect missing SMTP adress.");
            result.Remove(ErrorCode.Smtp_NoEmailAddress);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void SmtpSettings_NoAutoSave_NoHost()
        {
            _profile.EmailSmtpSettings.Enabled = true;
            _profile.EmailSmtpSettings.Ssl = false;
            _profile.EmailSmtpSettings.Subject = "";
            _profile.EmailSmtpSettings.Content = "";
            _profile.EmailSmtpSettings.AddSignature = false;

            _profile.EmailSmtpSettings.Address = "randomAdress@randomHost.random";
            _profile.EmailSmtpSettings.UserName = "randomUsername";
            _profile.EmailSmtpSettings.Server = "";
            _profile.EmailSmtpSettings.Port = 25;
            _profile.EmailSmtpSettings.Recipients = "randomRecipient@RandomHost.random";
            _profile.AutoSave.Enabled = false;
            _profile.EmailSmtpSettings.Password = "";
            var result = _smtpAction.Check(_profile, new Accounts());
            Assert.Contains(ErrorCode.Smtp_NoServerSpecified, result, "ProfileCheck did not detect missing SMTP host.");
            result.Remove(ErrorCode.Smtp_NoServerSpecified);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void SmtpSettings_NoAutoSave_NoRecipients()
        {
            _profile.EmailSmtpSettings.Enabled = true;
            _profile.EmailSmtpSettings.Ssl = false;
            _profile.EmailSmtpSettings.Subject = "";
            _profile.EmailSmtpSettings.Content = "";
            _profile.EmailSmtpSettings.AddSignature = false;

            _profile.EmailSmtpSettings.Address = "randomAdress@randomHost.random";
            _profile.EmailSmtpSettings.UserName = "randomUsername";
            _profile.EmailSmtpSettings.Server = "randomHost";
            _profile.EmailSmtpSettings.Port = 25;
            _profile.EmailSmtpSettings.Recipients = "";
            _profile.AutoSave.Enabled = false;
            _profile.EmailSmtpSettings.Password = "";
            var result = _smtpAction.Check(_profile, new Accounts());
            Assert.Contains(ErrorCode.Smtp_NoRecipients, result, "ProfileCheck did not detect missing SMTP recipients.");
            result.Remove(ErrorCode.Smtp_NoRecipients);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void SmtpSettings_NoAutoSave_NoUserName()
        {
            _profile.EmailSmtpSettings.Enabled = true;
            _profile.EmailSmtpSettings.Ssl = false;
            _profile.EmailSmtpSettings.Subject = "";
            _profile.EmailSmtpSettings.Content = "";
            _profile.EmailSmtpSettings.AddSignature = false;

            _profile.EmailSmtpSettings.Address = "randomAdress@randomHost.random";
            _profile.EmailSmtpSettings.UserName = "";
            _profile.EmailSmtpSettings.Server = "randomHost";
            _profile.EmailSmtpSettings.Port = 25;
            _profile.EmailSmtpSettings.Recipients = "randomRecipient@RandomHost.random";
            _profile.AutoSave.Enabled = false;
            _profile.EmailSmtpSettings.Password = "";
            var result = _smtpAction.Check(_profile, new Accounts());
            Assert.Contains(ErrorCode.Smtp_NoUserSpecified, result, "ProfileCheck did not detect missing SMTP username.");
            result.Remove(ErrorCode.Smtp_NoUserSpecified);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void SmtpSettings_NoAutoSave_valid()
        {
            _profile.EmailSmtpSettings.Enabled = true;
            _profile.EmailSmtpSettings.Ssl = false;
            _profile.EmailSmtpSettings.Subject = "";
            _profile.EmailSmtpSettings.Content = "";
            _profile.EmailSmtpSettings.AddSignature = false;

            _profile.EmailSmtpSettings.Address = "randomAdress@randomHost.random";
            _profile.EmailSmtpSettings.UserName = "randomUsername";
            _profile.EmailSmtpSettings.Server = "randomHost";
            _profile.EmailSmtpSettings.Port = 25;
            _profile.EmailSmtpSettings.Recipients = "randomRecipient@RandomHost.random";
            _profile.AutoSave.Enabled = false;
            _profile.EmailSmtpSettings.Password = "";
            var result = _smtpAction.Check(_profile, new Accounts());
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }
    }
}
