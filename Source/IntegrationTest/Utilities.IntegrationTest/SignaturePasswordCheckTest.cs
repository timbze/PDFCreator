using NUnit.Framework;
using PDFCreator.TestUtilities;
using pdfforge.PDFCreator.Utilities;

namespace PDFCreator.Utilities.IntegrationTest
{
    [TestFixture]
    public class SignaturePasswordCheckTest
    {
        private TestHelper _th;
        private ISignaturePasswordCheck _signaturePasswordCheck;
        private string _certificateFile;
        private string _password;

        [SetUp]
        public void SetUp()
        {
            _th = new TestHelper(null);
            _th.InitTempFolder(nameof(SignaturePasswordCheckTest));
            _certificateFile = _th.GenerateTestFile(TestFile.CertificationFileP12);
            _password = "Test1";

            _signaturePasswordCheck = new SignaturePasswordCheck();
        }

        [TearDown]
        public void CleanUp()
        {
            _th.CleanUp();
        }

        [Test]
        public void IsValidPassword_CertificateFileWithMatchingPassword_ReturnsTrue()
        {
            var result = _signaturePasswordCheck.IsValidPassword(_certificateFile, _password);

            Assert.IsTrue(result);
        }

        [Test]
        public void IsValidPassword_CertificateFileIsNull_ReturnsFalse()
        {
            _certificateFile = null;

            var result = _signaturePasswordCheck.IsValidPassword(_certificateFile, _password);

            Assert.IsFalse(result);
        }

        [Test]
        public void IsValidPassword_CertificateFileDoesNotExist_ReturnsFalse()
        {
            _certificateFile = "This file hopefully never exists.p12";

            var result = _signaturePasswordCheck.IsValidPassword(_certificateFile, _password);

            Assert.IsFalse(result);
        }

        [Test]
        public void IsValidPassword_PasswordIsNull_ReturnsFalse()
        {
            _password = null;

            var result = _signaturePasswordCheck.IsValidPassword(_certificateFile, _password);

            Assert.IsFalse(result);
        }

        [Test]
        public void IsValidPassword_PasswordIsInvalid_ReturnsFalse()
        {
            _password = "Invalid Password";

            var result = _signaturePasswordCheck.IsValidPassword(_certificateFile, _password);

            Assert.IsFalse(result);
        }
    }
}
