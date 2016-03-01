using System;
using System.Collections.Generic;
using SystemWrapper;
using NUnit.Framework;
using pdfforge.PDFCreator.Utilities.Tokens;

namespace PDFCreator.Utilities.UnitTest.Tokens
{
    [TestFixture]
    public class TokenCoreTest
    {
        private TokenReplacer _testTokenReplacer;

        [SetUp]
        public void InitTest()
        {
            _testTokenReplacer = new TokenReplacer();
            _testTokenReplacer.AddToken(new StringToken("Author", "Christophe"));
            _testTokenReplacer.AddToken(new NumberToken("Counter", 23));
            _testTokenReplacer.AddToken(new DateToken("DateTime", new DateTimeWrap(2012, 7, 24, 4, 36, 6)));
            _testTokenReplacer.AddToken(new ListToken("List", new[] {"filename1", "filename2"}));
            _testTokenReplacer.AddToken(new EnvironmentToken("Environment"));
        }

        [Test]
        public void Test()
        {
            Assert.AreEqual("No angel-brakets in this String...",
                _testTokenReplacer.ReplaceTokens("No angel-brakets in this String..."));
            Assert.AreEqual("Only Open < no Close", _testTokenReplacer.ReplaceTokens("Only Open < no Close"));
            Assert.AreEqual("Only Close > no Open", _testTokenReplacer.ReplaceTokens("Only Close > no Open"));
            Assert.AreEqual("An <Unknown> Token", _testTokenReplacer.ReplaceTokens("An <Unknown> Token"));
            Assert.AreEqual("An Christophe becomes Christophe",
                _testTokenReplacer.ReplaceTokens("An <Author> becomes Christophe"));
            Assert.AreEqual("Christophe", _testTokenReplacer.ReplaceTokens("<Author>"));
            Assert.AreEqual("Christophe", _testTokenReplacer.ReplaceTokens("<AuTHOR>"));
            Assert.AreEqual("A 23 becomes 23", _testTokenReplacer.ReplaceTokens("A <Counter> becomes 23"));
            Assert.AreEqual("<BLA<>BLA>", _testTokenReplacer.ReplaceTokens("<BLA<>BLA>"));

            Assert.AreEqual("<<NoToken>ChristopheSomethingBehind>",
                _testTokenReplacer.ReplaceTokens("<<NoToken><Author>SomethingBehind>"));

            Assert.AreEqual("NumberToken with Format 0023",
                _testTokenReplacer.ReplaceTokens("NumberToken with Format <Counter:0000>"));

            Assert.AreEqual("NumberToken with wrong Format BLA",
                _testTokenReplacer.ReplaceTokens("NumberToken with wrong Format <Counter:BLA>"));

            Assert.AreEqual("DateToken without Format 24.07.2012 04:36:06",
                _testTokenReplacer.ReplaceTokens("DateToken without Format <DateTime>"));

            Assert.AreEqual("DateToken with Format 20120724043606",
                _testTokenReplacer.ReplaceTokens("DateToken with Format <DateTime:yyyyMMddHHmmss>"));

            Assert.AreEqual("DateToken with Format 24 07 2012",
                _testTokenReplacer.ReplaceTokens("DateToken with Format <DateTime:dd MM yyyy>"));

            Assert.AreEqual("DateToken with wrong Format 24.07.2012 04:36:06",
                _testTokenReplacer.ReplaceTokens("DateToken with wrong Format <DateTime:1>"));

            Assert.AreEqual("Token without Format with: Format Christophe",
                _testTokenReplacer.ReplaceTokens("Token without Format with: Format <Author: 500>"));

            Assert.AreEqual("Unknown Token with: Format:<Christophe: Bla>",
                _testTokenReplacer.ReplaceTokens("Unknown Token with: Format:<Christophe: Bla>"));

            Assert.AreEqual("--Christophe-Christophe1234Christophe--",
                _testTokenReplacer.ReplaceTokens("--<Author>-<Author>1234<Author>--"));

            Assert.AreEqual("Christophe>Christophe<Christophe",
                _testTokenReplacer.ReplaceTokens("<Author>><Author><<Author>"));

            Assert.AreEqual("This is a real special case > this should not be doublicated < ",
                _testTokenReplacer.ReplaceTokens("This is a real special case > this should not be doublicated < "));

            Assert.AreEqual("This is a real special case > this Christophe should not be doublicated < something",
                _testTokenReplacer.ReplaceTokens("This is a real special case > this <Author> should not be doublicated < something"));

            Assert.AreEqual("Futher test of last case Christophe > this should not be doublicated <",
                _testTokenReplacer.ReplaceTokens("Futher test of last case <Author> > this should not be doublicated <"));

            Assert.AreEqual("And to be sure: Christophe > this Christophe should not be doublicated < Christophe",
                _testTokenReplacer.ReplaceTokens("And to be sure: <Author> > this <Author> should not be doublicated < <Author>"));

            Assert.AreEqual("filename1, filename2", _testTokenReplacer.ReplaceTokens("<List>"));
            Assert.AreEqual("filename1 - filename2", _testTokenReplacer.ReplaceTokens("<List: - >"));
            Assert.AreEqual("filename1\nfilename2", _testTokenReplacer.ReplaceTokens("<List:\\n>"));
        }

        [Test]
        public void TestEnvironmentToken()
        {
            Assert.AreEqual("", _testTokenReplacer.ReplaceTokens("<Environment:NonExistantToken>"));
            Assert.AreEqual("C:", _testTokenReplacer.ReplaceTokens("<Environment:SystemDrive>"));
            Assert.AreEqual("", _testTokenReplacer.ReplaceTokens("<Environment:|||Invalid|||>"));
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
        public void TokenReplacer_AddStringToken_ContainsStringToken()
        {
            var tr = new TokenReplacer();
            tr.AddStringToken("testName", "testValue");
        }

        [Test]
        public void EmptyTokenReplacer_GetToken_ReturnsNull()
        {
            var tr = new TokenReplacer();
            Assert.IsNull(tr.GetToken("<nonExistentName>"));
        }

        [Test]
        public void TokenReplacerWithSingleStringToken_GetToken_ReturnsStringToken()
        {
            var tr = new TokenReplacer();
            tr.AddStringToken("testName", "testValue");
            Assert.IsTrue(tr.GetToken("testName") is StringToken);
        }

        [Test]
        public void TokenReplacerWithSingleDateToken_GetToken_ReturnsDateToken()
        {
            var tr = new TokenReplacer();
            tr.AddDateToken("testName", new DateTimeWrap().Now);
            Assert.IsTrue(tr.GetToken("testName") is DateToken);
        }

        [Test]
        public void TokenReplacerWithSingleNumberToken_GetToken_ReturnsNumberToken()
        {
            var tr = new TokenReplacer();
            tr.AddNumberToken("testName", 42);
            Assert.IsTrue(tr.GetToken("testName") is NumberToken);
        }

        [Test]
        public void TokenReplacerWithSingleListToken_GetToken_ReturnsListToken()
        {
            var tr = new TokenReplacer();
            tr.AddListToken("testName", new List<string>(new string[]{}));
            Assert.IsTrue(tr.GetToken("testName") is ListToken);
        }

        [Test]
        public void TokenReplace_GetTokenWithNull_ThrowsArgumentNullException()
        {
            var tr = new TokenReplacer();

            Assert.Throws<ArgumentNullException>(() => tr.GetToken(null));
        }

        [Test]
        public void TokenReplacerWithSingleEnvironmentToken_GetToken_ReturnsTokenWithNameEnvironment()
        {
            var tr = new TokenReplacer();
            tr.AddListToken("testName", new List<string>(new string[] { }));
            Assert.IsTrue(tr.GetToken("testName") is ListToken);
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
    }
}
