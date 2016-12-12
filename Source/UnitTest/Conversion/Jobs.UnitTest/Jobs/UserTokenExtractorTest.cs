using System.Collections.Generic;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PsParser;
using PsParser;

namespace pdfforge.PDFCreator.UnitTest.Conversion.Jobs.Jobs
{
    [TestFixture]
    public class UserTokenExtractorTest
    {
        private const string PsFile = "file.ps";
        private UserTokenExtractor _userTokenExtractor;
        private IPsParser _psParser;

        [SetUp]
        public void SetUp()
        {
            _psParser = Substitute.For<IPsParser>();
            _psParser.UserTokens.Returns(new List<UserToken>());
            var psParserFactory = Substitute.For<IPsParserFactory>();
            psParserFactory.BuildPsParser(PsFile).Returns(_psParser);
            
            _userTokenExtractor = new UserTokenExtractor(psParserFactory);
        }

        [Test]
        public void ExtractUserTokenFromPsFileTest_CorrectUseOfPsParser()
        {
            _userTokenExtractor.ExtractUserTokenFromPsFile(PsFile);

            Received.InOrder(() => {
                _psParser.Analyse();
                _psParser.RemoveParameterLinesFromPSFile();
                _psParser.CloseStream();
            });
        }

        [Test]
        public void ExtractUserTokenFromPsFileTest_PsParserUserTokenIsNull_ReturnsEmptyUserToken()
        {
            _psParser.UserTokens.ReturnsNull();

            var userToken = _userTokenExtractor.ExtractUserTokenFromPsFile(PsFile);

            Assert.IsEmpty(userToken.KeyValueDict);
        }

        [Test]
        public void ExtractUserTokenFromPsFileTest_PsParserExxtractsUserTokens_ReturnedUserTokenKeyValueDictIncludesTokens()
        {
            var userTokensList = new List<UserToken>();
            var ut1 = new UserToken("key1", "value1");
            userTokensList.Add(ut1);
            var ut2 = new UserToken("key2", "value2");
            userTokensList.Add(ut2);
            _psParser.UserTokens.Returns(userTokensList);

            var userToken = _userTokenExtractor.ExtractUserTokenFromPsFile(PsFile);

            Assert.AreEqual(2, userToken.KeyValueDict.Count, "Wrong number of key value pairs in user token");
            Assert.AreEqual(ut1.Value, userToken.KeyValueDict[ut1.Key], "Wrong value for user token key");
            Assert.AreEqual(ut2.Value, userToken.KeyValueDict[ut2.Key], "Wrong value for user token key");
        }
    }
}
