using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Settings;
using System.Collections.ObjectModel;

namespace SettingsUnitTest
{
    [TestFixture]
    public class AccountsTest
    {
        private Accounts _accounts;
        private ConversionProfile _profile;
        private FtpAccount _existingFtpAccount;
        private SmtpAccount _existingSmtpAccount;
        private HttpAccount _existingHttpAccount;
        private DropboxAccount _existingDropboxAccount;
        private TimeServerAccount _existingTimeServerAccount;

        [SetUp]
        public void SetUp()
        {
            _accounts = new Accounts();
            _profile = new ConversionProfile();

            _existingFtpAccount = new FtpAccount();
            _existingFtpAccount.AccountId = "ftpAccountID";

            _existingSmtpAccount = new SmtpAccount();
            _existingSmtpAccount.AccountId = "smtpAccountID";

            _existingHttpAccount = new HttpAccount();
            _existingHttpAccount.AccountId = "httpAccountID";

            _existingDropboxAccount = new DropboxAccount();
            _existingDropboxAccount.AccountId = "dropboxAccountID";

            _existingTimeServerAccount = new TimeServerAccount();
            _existingTimeServerAccount.AccountId = "timeserverAccountID";

            _accounts.FtpAccounts.Add(_existingFtpAccount);
            _accounts.SmtpAccounts.Add(_existingSmtpAccount);
            _accounts.HttpAccounts.Add(_existingHttpAccount);
            _accounts.DropboxAccounts.Add(_existingDropboxAccount);
            _accounts.TimeServerAccounts.Add(_existingTimeServerAccount);
        }

        #region Ftp

        [Test]
        public void GetFtpAccount_FtpAccountsAreEmpty_ReturnsNull()
        {
            _accounts.FtpAccounts = new ObservableCollection<FtpAccount>(); //FtpAccounts are null
            _profile.Ftp.AccountId = "DoesNotMatterButMayNotBeEmpty";

            var fetchedFtpAccount = _accounts.GetFtpAccount(_profile);

            Assert.IsNull(fetchedFtpAccount);
        }

        [Test]
        public void GetFtpAccount_FtpAccountsNotEmpty_GetFtpAccountCalledWithEmptyString_ReturnsNull()
        {
            var ftpAccountWithEmptyID = new FtpAccount();
            ftpAccountWithEmptyID.AccountId = ""; //this should never happen and is set in this test to avoid ugly errors ...
            _accounts.FtpAccounts.Add(ftpAccountWithEmptyID);
            _profile.Ftp.AccountId = ""; //... in case the AccountID is empty

            var fetchedFtpAccount = _accounts.GetFtpAccount(_profile);

            Assert.IsNull(fetchedFtpAccount);
        }

        [Test]
        public void GetFtpAccount_FtpAccountsNotEmpty_GetFtpAccountCalledWithNonExistingID_ReturnsNull()
        {
            _profile.Ftp.AccountId = "NonExistingID";

            var fetchedFtpAccount = _accounts.GetFtpAccount(_profile);

            Assert.IsNull(fetchedFtpAccount);
        }

        [Test]
        public void GetFtpAccount_FtpAccountsNotEmpty__GetFtpAccountCalledWitExistingID_ReturnsRequestedAccount()
        {
            _profile.Ftp.AccountId = _existingFtpAccount.AccountId;

            var fetchedFtpAccount = _accounts.GetFtpAccount(_profile);

            Assert.AreSame(_existingFtpAccount, fetchedFtpAccount);
        }

        #endregion Ftp

        #region Smtp

        [Test]
        public void GetSmtpAccount_SmtpAccountsAreEmpty_ReturnsNull()
        {
            _accounts.SmtpAccounts = new ObservableCollection<SmtpAccount>();
            _profile.EmailSmtpSettings.AccountId = "DoesNotMatterButMayNotBeEmpty";

            var fetchedSmtpAccount = _accounts.GetSmtpAccount(_profile);

            Assert.IsNull(fetchedSmtpAccount);
        }

        [Test]
        public void GetSmtpAccount_SmtpAccountIsNotEmpty_CallGetSmtpAccountWithEmptyString_ReturnsNull()
        {
            var smtpAccountWithEmptyID = new SmtpAccount();
            smtpAccountWithEmptyID.AccountId = "";
            _accounts.SmtpAccounts.Add(smtpAccountWithEmptyID);
            _profile.EmailSmtpSettings.AccountId = "";

            var fetchedSmtpAccount = _accounts.GetSmtpAccount(_profile);

            Assert.IsNull(fetchedSmtpAccount);
        }

        [Test]
        public void GetSmtpAccount_SmtpAccountsIsNotEmpty_GetSmtpAccountWithNonExistingID_ReturnsNull()
        {
            _profile.EmailSmtpSettings.AccountId = "NonExistingID";

            var fetchedSmtpAccount = _accounts.GetSmtpAccount(_profile);

            Assert.IsNull(fetchedSmtpAccount);
        }

        [Test]
        public void GetSmtpAccount_SmtpAccountIsNotEmpty_GetSmtpAccountCalledWithExistingID_ReturnsRequestedAccount()
        {
            _profile.EmailSmtpSettings.AccountId = _existingSmtpAccount.AccountId;

            var fetchedSmtpAccount = _accounts.GetSmtpAccount(_profile);

            Assert.AreSame(_existingSmtpAccount, fetchedSmtpAccount);
        }

        #endregion Smtp

        #region Http

        [Test]
        public void GetHttpAccount_HttpAccountsAreEmpty_ReturnsNull()
        {
            _accounts.HttpAccounts = new ObservableCollection<HttpAccount>();
            _profile.HttpSettings.AccountId = "DoesNotMatterButMayNotBeEmpty";

            var fetchedHttpAccount = _accounts.GetHttpAccount(_profile);

            Assert.IsNull(fetchedHttpAccount);
        }

        [Test]
        public void GetHttpAccount_HttpAccountIsNotEmpty_GetHttpAccountIsCalledWithEmptyString_ReturnsNull()
        {
            var httpAccountWithEmptyId = new HttpAccount();
            httpAccountWithEmptyId.AccountId = "";
            _accounts.HttpAccounts.Add(httpAccountWithEmptyId);
            _profile.HttpSettings.AccountId = "";

            var fetchedHttpAccount = _accounts.GetHttpAccount(_profile);

            Assert.IsNull(fetchedHttpAccount);
        }

        [Test]
        public void GetHttpAccount_HttpAccountIsNotEmpty_GetHttpAccountIsCalledWithNonExistingID_ReturnsNull()
        {
            _profile.HttpSettings.AccountId = "NonExistingID";

            var fetchedHttpAccount = _accounts.GetHttpAccount(_profile);

            Assert.IsNull(fetchedHttpAccount);
        }

        [Test]
        public void GetHttpAccount_HttpAccountIsNotEmpty_GetHttpAccountIsCalledWithExistingID_ReturnsRequesteHttpAccount()
        {
            _profile.HttpSettings.AccountId = _existingHttpAccount.AccountId;

            var fetchedHttpAccount = _accounts.GetHttpAccount(_profile);

            Assert.AreSame(_existingHttpAccount, fetchedHttpAccount);
        }

        #endregion Http

        #region Dropbox

        [Test]
        public void GetDropboxAccount_DropboxAccountsAreEmpty_ReturnsNull()
        {
            _accounts.DropboxAccounts = new ObservableCollection<DropboxAccount>();
            _profile.DropboxSettings.AccountId = "DoesNotMatterButMayNotBeEmpty";

            var fetchedDropboxAccount = _accounts.GetDropboxAccount(_profile);

            Assert.IsNull(fetchedDropboxAccount);
        }

        [Test]
        public void GetDropboxAccount_DropboxAccountsNotEmpty_GetDropboxAccountCalledWithEmptyString_ReturnsNull()
        {
            var dropboxAccountWithEmptyID = new DropboxAccount();
            dropboxAccountWithEmptyID.AccountId = ""; //this should never happen and is set in this test to avoid ugly errors ...
            _accounts.DropboxAccounts.Add(dropboxAccountWithEmptyID);
            _profile.DropboxSettings.AccountId = ""; //... in case the AccountID is empty

            var fetchedDropboxAccount = _accounts.GetDropboxAccount(_profile);

            Assert.IsNull(fetchedDropboxAccount);
        }

        [Test]
        public void GetDropboxAccount_DropboxAccountsNotEmpty_GetDropboxAccountCalledWithNonExistingID_ReturnsNull()
        {
            _profile.DropboxSettings.AccountId = "NonExistingID";

            var fetchedDropboxAccount = _accounts.GetDropboxAccount(_profile);

            Assert.IsNull(fetchedDropboxAccount);
        }

        [Test]
        public void GetDropboxAccount_DropboxAccountsNotEmpty_GetDropboxAccountCalledWitExistingID_ReturnsRequestedAccount()
        {
            _profile.DropboxSettings.AccountId = _existingDropboxAccount.AccountId;

            var fetchedDropboxAccount = _accounts.GetDropboxAccount(_profile);

            Assert.AreSame(_existingDropboxAccount, fetchedDropboxAccount);
        }

        #endregion Dropbox

        #region TimeServer

        [Test]
        public void GetTimeServerAccount_TimeServerAccountsAreEmpty_ReturnsNull()
        {
            _accounts.TimeServerAccounts = new ObservableCollection<TimeServerAccount>();
            _profile.PdfSettings.Signature.TimeServerAccountId = "DoesNotMatterButMayNotBeEmpty";

            var fetchedTimeServerAccount = _accounts.GetTimeServerAccount(_profile);

            Assert.IsNull(fetchedTimeServerAccount);
        }

        [Test]
        public void GetTimeServerAccount_TimeServerAccountIsNotEmpty_CallGetTimeServerAccountWithEmptyString_ReturnsNull()
        {
            var timeServerAccountWithEmptyId = new TimeServerAccount();
            timeServerAccountWithEmptyId.AccountId = "";
            _accounts.TimeServerAccounts.Add(timeServerAccountWithEmptyId);
            _profile.PdfSettings.Signature.TimeServerAccountId = "";

            var fetchedTimeServerAccount = _accounts.GetTimeServerAccount(_profile);

            Assert.IsNull(fetchedTimeServerAccount);
        }

        [Test]
        public void GetTimeServerAccount_TimeServerAccountIsNotEmpty_GetTimeServerAccountWithNonExistingID_ReturnsNull()
        {
            _profile.PdfSettings.Signature.TimeServerAccountId = "NonExistingID";

            var fetchedTimeServerAccount = _accounts.GetTimeServerAccount(_profile);

            Assert.IsNull(fetchedTimeServerAccount);
        }

        [Test]
        public void GetTimeServerAccount_TimeServerAccountIsNotEmpty_GetTimeServerAccountCalledWithExistingID_ReturnsRequestedAccount()
        {
            _profile.PdfSettings.Signature.TimeServerAccountId = _existingTimeServerAccount.AccountId;

            var fetchedTimeServerAccount = _accounts.GetTimeServerAccount(_profile);

            Assert.AreSame(_existingTimeServerAccount, fetchedTimeServerAccount);
        }

        #endregion TimeServer
    }
}
