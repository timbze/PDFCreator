using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Workflow;

namespace pdfforge.PDFCreator.UnitTest.Core.Workflow
{
    [TestFixture]
    public class JobPasswordHelperTest
    {
        private Accounts _accounts;
        private HttpAccount _httpAccount;
        private FtpAccount _ftpAccount;
        private SmtpAccount _smtpAccount;
        private ConversionProfile _profile;

        [SetUp]
        public void SetUp()
        {
            _accounts = new Accounts();

            _httpAccount = new HttpAccount();
            _httpAccount.AccountId = "httpAccountId";
            _httpAccount.Password = "httpAccountPassword";

            _ftpAccount = new FtpAccount();
            _ftpAccount.AccountId = "ftpAccountId";
            _ftpAccount.Password = "ftpAccountPassword";

            _smtpAccount = new SmtpAccount();
            _smtpAccount.AccountId = "smtpAccountId";
            _smtpAccount.Password = "smtpAccountPassword";

            _accounts.HttpAccounts.Add(_httpAccount);
            _accounts.FtpAccounts.Add(_ftpAccount);
            _accounts.SmtpAccounts.Add(_smtpAccount);

            _profile = new ConversionProfile();
        }

        [Test]
        public void GetJobPasswords_ReturnsJobPasswordsWithPdfOwnerPassword()
        {
            _profile.PdfSettings.Security.OwnerPassword = "SomeCreativeOwnerPassword";

            var jobPasswords = JobPasswordHelper.GetJobPasswords(_profile, _accounts);

            Assert.AreEqual(_profile.PdfSettings.Security.OwnerPassword, jobPasswords.PdfOwnerPassword);
        }

        [Test]
        public void GetJobPasswords_ReturnsJobPasswordsWithPdfUserPassword()
        {
            _profile.PdfSettings.Security.UserPassword = "SomeCreativeUserPassword";

            var jobPasswords = JobPasswordHelper.GetJobPasswords(_profile, _accounts);

            Assert.AreEqual(_profile.PdfSettings.Security.UserPassword, jobPasswords.PdfUserPassword);
        }

        [Test]
        public void GetJobPasswords_ReturnsJobPasswordsWithPdfSignaturePassword()
        {
            _profile.PdfSettings.Signature.SignaturePassword = "SomeCreativeSignaturePassword";

            var jobPasswords = JobPasswordHelper.GetJobPasswords(_profile, _accounts);

            Assert.AreEqual(_profile.PdfSettings.Signature.SignaturePassword, jobPasswords.PdfSignaturePassword);
        }

        [Test]
        public void GetJobPasswords_ProfileContainsNotExistingHttpAccountId_ReturnsJobPasswordsWithEmptyHttpPassword()
        {
            _profile.HttpSettings.AccountId = "NotExistingId";

            var jobPasswords = JobPasswordHelper.GetJobPasswords(_profile, _accounts);

            Assert.AreEqual("", jobPasswords.HttpPassword);
        }

        [Test]
        public void GetJobPasswords_ProfileContainsExistingHttpAccountId_ReturnsJobPasswordsWithHttpPassword()
        {
            _profile.HttpSettings.AccountId = _httpAccount.AccountId;

            var jobPasswords = JobPasswordHelper.GetJobPasswords(_profile, _accounts);

            Assert.AreEqual(_httpAccount.Password, jobPasswords.HttpPassword);
        }

        [Test]
        public void GetJobPasswords_ProfileContainsNotExistingFtpAccountId_ReturnsJobPasswordsWithEmptyFtpPassword()
        {
            _profile.Ftp.AccountId = "NotExistingId";

            var jobPasswords = JobPasswordHelper.GetJobPasswords(_profile, _accounts);

            Assert.AreEqual("", jobPasswords.FtpPassword);
        }

        [Test]
        public void GetJobPasswords_ProfileContainsExistingFtpAccountId_ReturnsJobPasswordsWithFtpPassword()
        {
            _profile.Ftp.AccountId = _ftpAccount.AccountId;

            var jobPasswords = JobPasswordHelper.GetJobPasswords(_profile, _accounts);

            Assert.AreEqual(_ftpAccount.Password, jobPasswords.FtpPassword);
        }

        [Test]
        public void GetJobPasswords_ProfileContainsNotExistingSmtpAccountId_ReturnsJobPasswordsWithEmptySmtpPassword()
        {
            _profile.EmailSmtpSettings.AccountId = "NotExistingId";

            var jobPasswords = JobPasswordHelper.GetJobPasswords(_profile, _accounts);

            Assert.AreEqual("", jobPasswords.SmtpPassword);
        }

        [Test]
        public void GetJobPasswords_ProfileContainsExistingSmtpAccountId_ReturnsJobPasswordsWithSmtpPassword()
        {
            _profile.EmailSmtpSettings.AccountId = _smtpAccount.AccountId;

            var jobPasswords = JobPasswordHelper.GetJobPasswords(_profile, _accounts);

            Assert.AreEqual(_smtpAccount.Password, jobPasswords.SmtpPassword);
        }
    }
}
