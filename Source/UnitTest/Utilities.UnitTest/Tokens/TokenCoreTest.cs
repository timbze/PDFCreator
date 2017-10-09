using NUnit.Framework;
using pdfforge.PDFCreator.Utilities.Tokens;
using System;
using System.Collections.Generic;
using System.Globalization;
using SystemInterface;
using SystemWrapper;

namespace pdfforge.PDFCreator.Utilities.UnitTest.Tokens
{
    [TestFixture]
    public class TokenCoreTest
    {
        private readonly IDateTime _expectedDate = new DateTimeWrap(2012, 7, 24, 4, 36, 6);

        [SetUp]
        public void InitTest()
        {
            _testTokenReplacer = new TokenReplacer();
            _testTokenReplacer.AddToken(new StringToken("Author", "Christophe"));
            _testTokenReplacer.AddToken(new StringToken("Title", "Lord of the Rings"));
            _testTokenReplacer.AddToken(new NumberToken("Counter", 23));
            _testTokenReplacer.AddToken(new DateToken("DateTime", _expectedDate.DateTimeInstance));
            _testTokenReplacer.AddToken(new ListToken("List", new[] { "filename1", "filename2" }));
            _testTokenReplacer.AddToken(new EnvironmentToken("Environment"));
            var userToken = new UserToken();
            userToken.AddKeyValuePair("TokenByUser", "User token value");
            userToken.AddKeyValuePair("TokenByUserWithEmptyValue", "");
            _testTokenReplacer.AddToken(userToken);
        }

        private TokenReplacer _testTokenReplacer;

        [Test]
        public void EmptyTokenReplacer_GetToken_ReturnsNull()
        {
            var tr = new TokenReplacer();
            Assert.IsNull(tr.GetToken("<nonExistentName>"));
        }

        [Test]
        public void EnvironmentToken_GetValueWithNullArgument_ReturnsEmptyString()
        {
            var t = new EnvironmentToken();
            Assert.AreEqual("", t.GetValueWithFormat(null));
        }

        [Test]
        public void EnvironmentToken_GetValueWithoutFormat_ReturnsEmptyString()
        {
            var t = new EnvironmentToken();
            Assert.AreEqual("", t.GetValue());
        }

        [TestCase("", null)]
        [TestCase("No angel-brakets in this String...", "No angel-brakets in this String...")]
        [TestCase("Only Open < no Close", "Only Open < no Close")]
        [TestCase("Only Close > no Open", "Only Close > no Open")]
        [TestCase("An <Unknown> Token", "An <Unknown> Token")]
        [TestCase("An Christophe becomes Christophe", "An <Author> becomes Christophe")]
        [TestCase("Christophe", "<Author>")]
        [TestCase("Christophe", "<AuTHOR>")]
        [TestCase("A 23 becomes 23", "A <Counter> becomes 23")]
        [TestCase("<BLA<>BLA>", "<BLA<>BLA>")]
        [TestCase("<<NoToken>ChristopheSomethingBehind>", "<<NoToken><Author>SomethingBehind>")]
        [TestCase("NumberToken with Format 0023", "NumberToken with Format <Counter:0000>")]
        [TestCase("NumberToken with wrong Format BLA", "NumberToken with wrong Format <Counter:BLA>")]
        [TestCase("Token without Format with: Format Christophe", "Token without Format with: Format <Author: 500>")]
        [TestCase("Unknown Token with: Format:<Christophe: Bla>", "Unknown Token with: Format:<Christophe: Bla>")]
        [TestCase("--Christophe-Christophe1234Christophe--", "--<Author>-<Author>1234<Author>--")]
        [TestCase("Christophe>Christophe<Christophe", "<Author>><Author><<Author>")]
        [TestCase("This is a real special case > this should not be doublicated < ", "This is a real special case > this should not be doublicated < ")]
        [TestCase("This is a real special case > this Christophe should not be doublicated < something", "This is a real special case > this <Author> should not be doublicated < something")]
        [TestCase("Futher test of last case Christophe > this should not be doublicated <", "Futher test of last case <Author> > this should not be doublicated <")]
        [TestCase("And to be sure: Christophe > this Christophe should not be doublicated < Christophe", "And to be sure: <Author> > this <Author> should not be doublicated < <Author>")]
        [TestCase("filename1, filename2", "<List>")]
        [TestCase("filename1 - filename2", "<List: - >")]
        [TestCase("filename1\nfilename2", "<List:\\n>")]
        [TestCase("Lord of the RingsChristophe", "<Title><Author>")]
        public void Test(string expected, string tokenizedString)
        {
            Assert.AreEqual(expected, _testTokenReplacer.ReplaceTokens(tokenizedString));
        }

        [Test]
        public void TestDateToken()
        {
            var expectedDateUICulture = _expectedDate.ToString(CultureInfo.CurrentUICulture);

            Assert.AreEqual($"DateToken without Format {expectedDateUICulture}",
                _testTokenReplacer.ReplaceTokens("DateToken without Format <DateTime>"));

            Assert.AreEqual("DateToken with Format 20120724043606",
                _testTokenReplacer.ReplaceTokens("DateToken with Format <DateTime:yyyyMMddHHmmss>"));

            Assert.AreEqual("DateToken with Format 24 07 2012",
                _testTokenReplacer.ReplaceTokens("DateToken with Format <DateTime:dd MM yyyy>"));

            Assert.AreEqual($"DateToken with wrong Format {expectedDateUICulture}",
                _testTokenReplacer.ReplaceTokens("DateToken with wrong Format <DateTime:1>"));
        }

        [TestCase("", "<Environment:NonExistantToken>")]
        [TestCase("C:", "<Environment:SystemDrive>")]
        [TestCase("", "<Environment:|||Invalid|||>")]
        public void TestEnvironmentToken(string expected, string tokenizedString)
        {
            Assert.AreEqual(expected, _testTokenReplacer.ReplaceTokens(tokenizedString));
        }

        [TestCase("<TokenByUser>", "<TokenByUser>", "User token name without leading 'User:' should return original token.")]
        [TestCase("User token value", "<User:TokenByUser>", "User token should returns its value.")]
        [TestCase("User token value", "<User:TokenByUser:Default Value>", "User token with non empty value should return its value even if default value is set.")]
        [TestCase("", "<User:TokenByUserWithEmptyValue:Default Value>", "User token with empty value should return empty string even if default is set")]
        [TestCase("Default Value", "<User:UnknownTokenName:Default Value>", "User token with unknown key should return default value")]
        [TestCase("", "<User:UnknownTokenName>", "User token with unknown key without default value should return empty string")]
        [TestCase("", "<User>", "<User> should trigger GetValue which returns empty key.")]
        public void TestUserToken(string expected, string tokenizedString, string message)
        {
            Assert.AreEqual(expected, _testTokenReplacer.ReplaceTokens(tokenizedString), message);
        }

        [Test]
        public void UserToken_Merge_NullParameterDoesNotThrowException()
        {
            var userToken1 = new UserToken();
            Assert.DoesNotThrow(() => { userToken1.Merge(null); });
        }

        [Test]
        public void UserToken_Merge_MergedUserTokenContainsKeysAndValuesOfBothDicts()
        {
            var userToken1 = new UserToken();
            userToken1.AddKeyValuePair("key1", "value1");
            var userToken2 = new UserToken();
            userToken2.AddKeyValuePair("key2", "value2");

            userToken1.Merge(userToken2);

            Assert.AreEqual(2, userToken1.KeyValueDict.Count);
            Assert.AreEqual("value1", userToken1.KeyValueDict["key1"]);
            Assert.AreEqual("value2", userToken1.KeyValueDict["key2"]);
        }

        [Test]
        public void UserToken_Merge_MergeWithSameKeysContainsOlderKey()
        {
            var userToken1 = new UserToken();
            userToken1.AddKeyValuePair("samekey", "value old");
            var userToken2 = new UserToken();
            userToken2.AddKeyValuePair("samekey", "value new");

            userToken1.Merge(userToken2);

            Assert.AreEqual("value old", userToken1.KeyValueDict["samekey"]);
        }

        [Test]
        public void TokenReplace_GetTokenWithNull_ThrowsArgumentNullException()
        {
            var tr = new TokenReplacer();
            Assert.Throws<ArgumentNullException>(() => tr.GetToken(null));
        }

        [Test]
        public void TokenReplacer_AddStringToken_ContainsStringToken()
        {
            var tr = new TokenReplacer();
            tr.AddStringToken("testName", "testValue");
        }

        [Test]
        public void TokenReplacer_GetTokenNames_ContainsTokenName()
        {
            var tr = new TokenReplacer();
            var t = new StringToken("testName", "testValue");
            tr.AddToken(t);
            Assert.Contains(t.GetName(), tr.GetTokenNames(false));
        }

        [Test]
        public void TokenReplacer_GetTokenNames_ContainsTokenNameWithDelimiter()
        {
            var tr = new TokenReplacer();
            var t = new StringToken("testName", "testValue");
            tr.AddToken(t);
            Assert.Contains("<" + t.GetName() + ">", tr.GetTokenNames());
        }

        [Test]
        public void TokenReplacerWithSingleDateToken_GetToken_ReturnsDateToken()
        {
            var tr = new TokenReplacer();
            tr.AddDateToken("testName", DateTime.Now);
            Assert.IsTrue(tr.GetToken("testName") is DateToken);
        }

        [Test]
        public void TokenReplacerWithSingleEnvironmentToken_GetToken_ReturnsTokenWithNameEnvironment()
        {
            var tr = new TokenReplacer();
            tr.AddListToken("testName", new List<string>(new string[] { }));
            Assert.IsTrue(tr.GetToken("testName") is ListToken);
        }

        [Test]
        public void TokenReplacerWithSingleListToken_GetToken_ReturnsListToken()
        {
            var tr = new TokenReplacer();
            tr.AddListToken("testName", new List<string>(new string[] { }));
            Assert.IsTrue(tr.GetToken("testName") is ListToken);
        }

        [Test]
        public void TokenReplacerWithSingleNumberToken_GetToken_ReturnsNumberToken()
        {
            var tr = new TokenReplacer();
            tr.AddNumberToken("testName", 42);
            Assert.IsTrue(tr.GetToken("testName") is NumberToken);
        }

        [Test]
        public void TokenReplacerWithSingleStringToken_GetToken_ReturnsStringToken()
        {
            var tr = new TokenReplacer();
            tr.AddStringToken("testName", "testValue");
            Assert.IsTrue(tr.GetToken("testName") is StringToken);
        }
    }
}
