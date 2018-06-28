using NUnit.Framework;
using pdfforge.PDFCreator.Utilities.Tokens;

namespace pdfforge.PDFCreator.Utilities.UnitTest.Tokens
{
    [TestFixture]
    public class TokenIdentifierTest
    {
        [TestCase("<Token>")]
        [TestCase("<Token:>")]
        [TestCase("<DateTime:yyyyMMddHHmmss>")]
        [TestCase("<DateTime:yyyy MM dd HH mm ss>")]
        [TestCase("<User:NameDefinedByUser>")]
        [TestCase("<User:NameDefinedByUser:>")]
        [TestCase("<User:NameDefinedByUser:Default>")]
        [TestCase("<User:NameDefinedByUser:Default Value>")]
        public void ContainsToken_ValidInputTest(string input)
        {
            Assert.IsTrue(TokenIdentifier.ContainsTokens(input));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        [TestCase("KeineKlammern")]
        [TestCase("<Token")]
        [TestCase("Token>")]
        public void ContainsToken_InvalidInputTest(string input)
        {
            Assert.IsFalse(TokenIdentifier.ContainsTokens(input));
        }
    }
}
