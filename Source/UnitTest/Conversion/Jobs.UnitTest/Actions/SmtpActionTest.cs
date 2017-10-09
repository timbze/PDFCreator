using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Actions.Actions;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using System;

namespace pdfforge.PDFCreator.UnitTest.Conversion.Jobs.Actions
{
    [TestFixture]
    public class SmtpActionTest
    {
        [SetUp]
        public void SetUp()
        {
            _smtpTestAccount = new SmtpAccount();
            _smtpTestAccount.AccountId = "TestAccountId";

            _profile = new ConversionProfile();
            _profile.EmailSmtpSettings.Enabled = true;
            _profile.EmailSmtpSettings.AccountId = _smtpTestAccount.AccountId;

            _accounts = new Accounts();
            _accounts.SmtpAccounts.Add(_smtpTestAccount);

            _smtpAction = new SmtpMailAction();
        }

        private ConversionProfile _profile;
        private SmtpMailAction _smtpAction;
        private Accounts _accounts;
        private SmtpAccount _smtpTestAccount;

        private void SetValidAutoSaveSettings()
        {
            _profile.AutoSave.Enabled = true;
            _profile.TargetDirectory = "random autosave directory";
            _profile.FileNameTemplate = "random autosave filename";
        }

        [Test]
        public void Check_NoAccount_ResultContainsAssociatedCode()
        {
            _profile.EmailSmtpSettings.AccountId = "Some unavailable Account ID";

            var results = _smtpAction.Check(_profile, _accounts);

            Assert.AreEqual(ErrorCode.Smtp_NoAccount, results[0]);
        }

        [Test]
        public void Check_AutoSave_MultipleErrors()
        {
            _profile.EmailSmtpSettings.Enabled = true;
            _profile.EmailSmtpSettings.Subject = "";
            _profile.EmailSmtpSettings.Content = "";
            _profile.EmailSmtpSettings.AddSignature = false;

            _smtpTestAccount.Ssl = false;
            _smtpTestAccount.Address = "";
            _smtpTestAccount.UserName = "";
            _smtpTestAccount.Server = "";
            _smtpTestAccount.Port = -1;
            _smtpTestAccount.Password = "";

            _profile.EmailSmtpSettings.Recipients = "";
            SetValidAutoSaveSettings();

            var result = _smtpAction.Check(_profile, _accounts);
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
        public void Check_AutoSave_NoPassword()
        {
            _profile.EmailSmtpSettings.Enabled = true;
            _profile.EmailSmtpSettings.Subject = "";
            _profile.EmailSmtpSettings.Content = "";
            _profile.EmailSmtpSettings.AddSignature = false;

            _smtpTestAccount.Address = "randomAdress@randomHost.random";
            _smtpTestAccount.UserName = "randomUsername";
            _smtpTestAccount.Server = "randomHost";
            _smtpTestAccount.Port = 25;
            _smtpTestAccount.Ssl = false;
            _smtpTestAccount.Password = "";

            _profile.EmailSmtpSettings.Recipients = "randomRecipient@RandomHost.random";
            SetValidAutoSaveSettings();
            var result = _smtpAction.Check(_profile, _accounts);
            Assert.Contains(ErrorCode.Smtp_NoPasswordSpecified, result, "ProfileCheck did not detect missing SMTP password for autosaving.");
            result.Remove(ErrorCode.Smtp_NoPasswordSpecified);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void Check_AutoSave_valid()
        {
            _profile.EmailSmtpSettings.Enabled = true;
            _profile.EmailSmtpSettings.Subject = "";
            _profile.EmailSmtpSettings.Content = "";
            _profile.EmailSmtpSettings.AddSignature = false;

            _smtpTestAccount.Address = "randomAdress@randomHost.random";
            _smtpTestAccount.UserName = "randomUsername";
            _smtpTestAccount.Server = "randomHost";
            _smtpTestAccount.Port = 25;
            _smtpTestAccount.Ssl = false;
            _smtpTestAccount.Password = "1234";

            _profile.EmailSmtpSettings.Recipients = "randomRecipient@RandomHost.random";
            SetValidAutoSaveSettings();
            var result = _smtpAction.Check(_profile, _accounts);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void Check_NoAutoSave_InvalidPort()
        {
            _profile.EmailSmtpSettings.Enabled = true;
            _profile.EmailSmtpSettings.Subject = "";
            _profile.EmailSmtpSettings.Content = "";
            _profile.EmailSmtpSettings.AddSignature = false;

            _smtpTestAccount.Address = "randomAdress@randomHost.random";
            _smtpTestAccount.UserName = "randomUsername";
            _smtpTestAccount.Server = "randomHost";
            _smtpTestAccount.Port = -1;
            _smtpTestAccount.Password = "";
            _smtpTestAccount.Ssl = false;

            _profile.EmailSmtpSettings.Recipients = "randomRecipient@RandomHost.random";
            _profile.AutoSave.Enabled = false;

            var result = _smtpAction.Check(_profile, _accounts);
            Assert.Contains(ErrorCode.Smtp_InvalidPort, result, "ProfileCheck did not detect invalid SMTP port.");
            result.Remove(ErrorCode.Smtp_InvalidPort);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void Check_NoAutoSave_MultipleErrors()
        {
            _profile.EmailSmtpSettings.Enabled = true;
            _profile.EmailSmtpSettings.Subject = "";
            _profile.EmailSmtpSettings.Content = "";
            _profile.EmailSmtpSettings.AddSignature = false;

            _smtpTestAccount.Address = "";
            _smtpTestAccount.UserName = "";
            _smtpTestAccount.Server = "";
            _smtpTestAccount.Port = -1;
            _smtpTestAccount.Ssl = false;
            _smtpTestAccount.Password = "";

            _profile.EmailSmtpSettings.Recipients = "";
            _profile.AutoSave.Enabled = false;

            var result = _smtpAction.Check(_profile, _accounts);
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
        public void Check_NoAutoSave_NoAdress()
        {
            _profile.EmailSmtpSettings.Enabled = true;
            _profile.EmailSmtpSettings.Subject = "";
            _profile.EmailSmtpSettings.Content = "";
            _profile.EmailSmtpSettings.AddSignature = false;

            _smtpTestAccount.Address = "";
            _smtpTestAccount.UserName = "randomUsername";
            _smtpTestAccount.Server = "randomHost";
            _smtpTestAccount.Port = 25;
            _smtpTestAccount.Ssl = false;
            _smtpTestAccount.Password = "";

            _profile.EmailSmtpSettings.Recipients = "randomRecipient@RandomHost.random";
            _profile.AutoSave.Enabled = false;

            var result = _smtpAction.Check(_profile, _accounts);
            Assert.Contains(ErrorCode.Smtp_NoEmailAddress, result, "ProfileCheck did not detect missing SMTP adress.");
            result.Remove(ErrorCode.Smtp_NoEmailAddress);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void Check_NoAutoSave_NoHost()
        {
            _profile.EmailSmtpSettings.Enabled = true;
            _profile.EmailSmtpSettings.Subject = "";
            _profile.EmailSmtpSettings.Content = "";
            _profile.EmailSmtpSettings.AddSignature = false;

            _smtpTestAccount.Address = "randomAdress@randomHost.random";
            _smtpTestAccount.UserName = "randomUsername";
            _smtpTestAccount.Server = "";
            _smtpTestAccount.Port = 25;
            _smtpTestAccount.Ssl = false;
            _smtpTestAccount.Password = "";

            _profile.EmailSmtpSettings.Recipients = "randomRecipient@RandomHost.random";
            _profile.AutoSave.Enabled = false;

            var result = _smtpAction.Check(_profile, _accounts);
            Assert.Contains(ErrorCode.Smtp_NoServerSpecified, result, "ProfileCheck did not detect missing SMTP host.");
            result.Remove(ErrorCode.Smtp_NoServerSpecified);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void Check_NoAutoSave_NoRecipients()
        {
            _profile.EmailSmtpSettings.Enabled = true;
            _profile.EmailSmtpSettings.Subject = "";
            _profile.EmailSmtpSettings.Content = "";
            _profile.EmailSmtpSettings.AddSignature = false;

            _smtpTestAccount.Address = "randomAdress@randomHost.random";
            _smtpTestAccount.UserName = "randomUsername";
            _smtpTestAccount.Server = "randomHost";
            _smtpTestAccount.Port = 25;
            _smtpTestAccount.Ssl = false;
            _smtpTestAccount.Password = "";

            _profile.EmailSmtpSettings.Recipients = "";
            _profile.AutoSave.Enabled = false;

            var result = _smtpAction.Check(_profile, _accounts);
            Assert.Contains(ErrorCode.Smtp_NoRecipients, result, "ProfileCheck did not detect missing SMTP recipients.");
            result.Remove(ErrorCode.Smtp_NoRecipients);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void Check_NoAutoSave_NoUserName()
        {
            _profile.EmailSmtpSettings.Enabled = true;
            _profile.EmailSmtpSettings.Subject = "";
            _profile.EmailSmtpSettings.Content = "";
            _profile.EmailSmtpSettings.AddSignature = false;

            _smtpTestAccount.Address = "randomAdress@randomHost.random";
            _smtpTestAccount.UserName = "";
            _smtpTestAccount.Server = "randomHost";
            _smtpTestAccount.Port = 25;
            _smtpTestAccount.Ssl = false;
            _smtpTestAccount.Password = "";

            _profile.EmailSmtpSettings.Recipients = "randomRecipient@RandomHost.random";
            _profile.AutoSave.Enabled = false;

            var result = _smtpAction.Check(_profile, _accounts);
            Assert.Contains(ErrorCode.Smtp_NoUserSpecified, result, "ProfileCheck did not detect missing SMTP username.");
            result.Remove(ErrorCode.Smtp_NoUserSpecified);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }

        [Test]
        public void Check_NoAutoSave_valid()
        {
            _profile.EmailSmtpSettings.Enabled = true;
            _profile.EmailSmtpSettings.Subject = "";
            _profile.EmailSmtpSettings.Content = "";
            _profile.EmailSmtpSettings.AddSignature = false;

            _smtpTestAccount.Address = "randomAdress@randomHost.random";
            _smtpTestAccount.UserName = "randomUsername";
            _smtpTestAccount.Server = "randomHost";
            _smtpTestAccount.Port = 25;
            _smtpTestAccount.Ssl = false;
            _smtpTestAccount.Password = "";

            _profile.EmailSmtpSettings.Recipients = "randomRecipient@RandomHost.random";
            _profile.AutoSave.Enabled = false;
            var result = _smtpAction.Check(_profile, _accounts);
            Assert.IsTrue(result, "Unexpected errorcodes:" + Environment.NewLine + result);
        }
    }
}
