using NUnit.Framework;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Tokens;
using pdfforge.PDFCreator.Utilities.Tokens;

namespace Presentation.UnitTest.Helper
{
    [TestFixture]
    public class ParameterPreviewTokenTest
    {
        private const string TokenName = "MyToken";
        private readonly TokenPlaceHoldersTranslation _translation = new TokenPlaceHoldersTranslation();

        private ParameterPreviewToken BuildToken(string tokenName = "SomeToken")
        {
            return new ParameterPreviewToken(tokenName, _translation.FormatTokenPreviewText);
        }

        [Test]
        public void GetName_ReturnsTokenName()
        {
            var token = BuildToken(TokenName);

            Assert.AreEqual(TokenName, token.GetName());
        }

        [Test]
        public void GetValue_WithoutFormatString_ReturnsEmptyString()
        {
            var token = BuildToken(TokenName);

            Assert.AreEqual("", token.GetValue());
        }

        [Test]
        public void GetValue_WithFormatString_ReturnsPlaceholderString()
        {
            var tokenParameter = "test";
            var expectedString = _translation.FormatTokenPreviewText(tokenParameter);
            var token = BuildToken(TokenName);

            Assert.AreEqual(expectedString, token.GetValueWithFormat(tokenParameter));
        }

        [Test]
        public void TokenReplacer_TokenWithParameter_ReturnsPlaceholder()
        {
            const string tokenName = "MyToken";

            var tr = new TokenReplacer();
            tr.AddToken(BuildToken(tokenName));

            var str = tr.ReplaceTokens("<MyToken:Test>");

            Assert.AreEqual("Value for 'Test'", str);
        }
    }
}
