using NUnit.Framework;

namespace pdfforge.PDFCreator.Utilities.UnitTest
{
    [TestFixture]
    public class LicenseKeySyntaxCheckerTest
    {
        [SetUp]
        public void SetUp()
        {
            _licenseKeySyntaxChecker = new LicenseKeySyntaxChecker();
        }

        private LicenseKeySyntaxChecker _licenseKeySyntaxChecker;

        [TestCase(null, "")]
        [TestCase("ABCDE-FGHIJ-KLMNO-PQRST-12345-67890", "ABCDE-FGHIJ-KLMNO-PQRST-12345-67890")]
        [TestCase("ABCDEFGHIJKLMNOPQRST1234567890", "ABCDE-FGHIJ-KLMNO-PQRST-12345-67890")]
        [TestCase("abcdeFGHIJKLMNOpqRST1234567890", "ABCDE-FGHIJ-KLMNO-PQRST-12345-67890")]
        public void NormalizeLicenseKey_ReturnsNormalizedKey(string key, string normalizedKey)
        {
            Assert.AreEqual(normalizedKey, _licenseKeySyntaxChecker.NormalizeLicenseKey(key));
        }

        [TestCase("ABCDE-FGHIJ-KLMNO-PQRST-12345-67890")]
        [TestCase("ABCDEFGHIJKLMNOPQRST1234567890")]
        [TestCase("abcde-12345-12345-12345-12345-12345")]
        [TestCase("abcde1234512345123451234512345")]
        public void ValidateLicenseKey_ValidKey_ResturnsValidResult(string key)
        {
            var result = _licenseKeySyntaxChecker.ValidateLicenseKey(key);
            Assert.AreEqual(ValidationResult.Valid, result);
        }

        [TestCase(null)]
        [TestCase("ABCDE-FGHIJ-KLMNO-PQRST-12345")]
        [TestCase("ABCDE-FGHIJ-KLMNO-PQRST-12345-67890-UVWXY")]
        public void ValidateLicenseKey_WrongFormat_ReturnsWrongFormatResult(string key)
        {
            var result = _licenseKeySyntaxChecker.ValidateLicenseKey(key);
            Assert.AreEqual(ValidationResult.WrongFormat, result);
        }

        [TestCase("!!!!!")]
        [TestCase("!!!!!-12345-12345-12345-12345-12345")]
        [TestCase("?????1234512345123451234512345")]
        public void ValidateLicenseKey_InvalidCharacters_ReturnsInvalidCharactersResult(string key)
        {
            var result = _licenseKeySyntaxChecker.ValidateLicenseKey(key);
            Assert.AreEqual(ValidationResult.InvalidCharacters, result);
        }
    }
}
