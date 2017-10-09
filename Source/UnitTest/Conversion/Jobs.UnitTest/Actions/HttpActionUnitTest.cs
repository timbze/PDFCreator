using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Actions.Actions;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;

namespace pdfforge.PDFCreator.UnitTest.Conversion.Jobs.Actions
{
    [TestFixture]
    internal class HttpActionUnitTest
    {
        private HttpAction _httpAction;
        private ConversionProfile _profile;
        private Accounts _accounts;
        private HttpAccount _httpAccount;

        [SetUp]
        public void SetUp()
        {
            _accounts = new Accounts();

            _httpAccount = new HttpAccount();
            _httpAccount.AccountId = "1";
            _httpAccount.Url = "http://http_URL";
            _httpAccount.IsBasicAuthentication = true;
            _httpAccount.UserName = "TestUserName";
            _httpAccount.Password = "Swordfish";

            _accounts.HttpAccounts.Add(_httpAccount);

            _profile = new ConversionProfile();
            _profile.HttpSettings.AccountId = _httpAccount.AccountId;
            _profile.HttpSettings.Enabled = true;

            _httpAction = new HttpAction();
        }

        [Test]
        public void IsEnabled_ReturnsProfileHttpEnabled()
        {
            _profile.HttpSettings.Enabled = true;
            Assert.IsTrue(_httpAction.IsEnabled(_profile));

            _profile.HttpSettings.Enabled = false;
            Assert.IsFalse(_httpAction.IsEnabled(_profile));
        }

        [Test]
        public void Check_ActionIsDisabled_ReturnsSucessfulActionResult()
        {
            _profile.HttpSettings.Enabled = false;
            var result = _httpAction.Check(_profile, _accounts);
            Assert.IsTrue(result);
        }

        [Test]
        public void Check_AccountIdIsNotSet_ActionResultContainsCorrespondingErrorCode()
        {
            _profile.HttpSettings.AccountId = " ";

            var actionResult = _httpAction.Check(_profile, _accounts);
            Assert.Contains(ErrorCode.HTTP_NoAccount, actionResult);
        }

        [Test]
        public void Check_SetAccountDoesNotExist_ActionResultContainsCorrespondingErrorCode()
        {
            _accounts.HttpAccounts.Clear();
            var actionResult = _httpAction.Check(_profile, _accounts);
            Assert.Contains(ErrorCode.HTTP_NoAccount, actionResult);
        }

        [Test]
        public void Check_MissingUrl_ActionResultContainsCorrespondingErrorCode()
        {
            _httpAccount.Url = "";

            var result = _httpAction.Check(_profile, _accounts);

            Assert.Contains(ErrorCode.HTTP_NoUrl, result);
        }

        [Test]
        public void Check_AuthIsEnabledMissingUserName_ActionResultContainsCorrespondingErrorCode()
        {
            _httpAccount.IsBasicAuthentication = true;
            _httpAccount.UserName = "";

            var result = _httpAction.Check(_profile, _accounts);

            Assert.Contains(ErrorCode.HTTP_NoUserNameForAuth, result);
        }

        [Test]
        public void Check_AutosaveWithAuthAndMissingPassword_ActionResultContainsCorrespondingErrorCode()
        {
            _profile.AutoSave.Enabled = true;
            _httpAccount.IsBasicAuthentication = true;
            _httpAccount.Password = "";

            var result = _httpAction.Check(_profile, _accounts);

            Assert.Contains(ErrorCode.HTTP_NoPasswordForAuthWithAutoSave, result);
        }

        [Test]
        public void SetURLWithoutHttpPrefix_Check_ReturnsMustStartWithHttpError()
        {
            _httpAccount.Url = "noHttp.com";
            var result = _httpAction.Check(_profile, _accounts);
            Assert.Contains(ErrorCode.HTTP_NoUrl, result);
        }

        [Test]
        public void SetWithHTTPs_Check_ReturnValid()
        {
            _httpAccount.Url = "https://SomeAdress.com";
            var result = _httpAction.Check(_profile, _accounts);
            Assert.IsTrue(result);
        }

        [Test]
        public void SetURLWithCaseInsensitiveHttp_Check_ReturnsValid()
        {
            _httpAccount.Url = "hTtP://someUrl.com";
            var result = _httpAction.Check(_profile, _accounts);
            Assert.IsTrue(result);
        }

        [Test]
        public void Check_ValidSettings_ReturnsSucessfulActionResult()
        {
            var result = _httpAction.Check(_profile, _accounts);
            Assert.IsTrue(result);
        }

        [Test]
        public void Check_AutosaveisDiabledNoPasswordIsSet_ReturnsSucessfulActionResult()
        {
            _profile.AutoSave.Enabled = false;
            _httpAccount.IsBasicAuthentication = true;
            var result = _httpAction.Check(_profile, _accounts);
            Assert.IsTrue(result);
        }

        [Test]
        public void CheckWithValidUrl_UrlIsFtp_ReturnsMustStartWithHttpError()
        {
            _httpAccount.Url = "ftp://www.validurlbutnothttp.com";

            var result = _httpAction.Check(_profile, _accounts);

            Assert.Contains(ErrorCode.HTTP_MustStartWithHttp, result);
        }

        [Test]
        public void CheckWithValidUrl_UrlIsLdap_ReturnsMustStartWithHttpError()
        {
            _httpAccount.Url = "ldap://www.ldapurl.com";

            var result = _httpAction.Check(_profile, _accounts);

            Assert.Contains(ErrorCode.HTTP_MustStartWithHttp, result);
        }

        [Test]
        public void CheckWithValidUrl_UrlIsMailto_ReturnsMustStartWithHttpError()
        {
            _httpAccount.Url = "mailto:someone@example.com";

            var result = _httpAction.Check(_profile, _accounts);

            Assert.Contains(ErrorCode.HTTP_MustStartWithHttp, result);
        }

        [Test]
        public void CheckWithValidUrl_UrlIsFile_ReturnsMustStartWithHttpError()
        {
            _httpAccount.Url = "file://localhost.com";

            var result = _httpAction.Check(_profile, _accounts);

            Assert.Contains(ErrorCode.HTTP_MustStartWithHttp, result);
        }

        [Test]
        public void CheckWithValidUrl_UrlIsIrc_ReturnsMustStartWithHttpError()
        {
            _httpAccount.Url = "irc://irc.dal.net";

            var result = _httpAction.Check(_profile, _accounts);

            Assert.Contains(ErrorCode.HTTP_MustStartWithHttp, result);
        }
    }
}
