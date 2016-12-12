using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using pdfforge.DynamicTranslator;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Core.Services.Translation;

namespace pdfforge.PDFCreator.IntegrationTest.TranslationTest
{
    [TestFixture]
    public class ErrorCodeInterpreterTest
    {
        private TranslationTestHelper _translationTestHelper;

        [SetUp]
        public void Setup()
        {
            _translationTestHelper = new TranslationTestHelper();
        }

        private bool IsInt(string value)
        {
            try
            {
                int.Parse(value);
                return true;
            }
            catch
            {
                return false;
            }
        }

        [Test]
        public void AllErrorCodes_AreMappedByErrorCodeInterpreter()
        {
            var section = "ErrorCodes";

            var translator = new BasicTranslator(Path.Combine(_translationTestHelper.FindTranslationFolder(), "english.ini"));

            var errorCodes = translator.GetKeysForSection(section)
                .Where(IsInt)
                .Select(int.Parse)
                .Select(x => (ErrorCode) x)
                .ToList();

            Assert.IsTrue(errorCodes.Any(), $"There are no entries in the section [{section}]");

            var errorCodeInterpreter = new ErrorCodeInterpreter(translator);

            foreach (var errorCode in errorCodes)
            {
                Assert.IsTrue(Enum.IsDefined(typeof(ErrorCode), errorCode), $"The error code {(int)errorCode} is not defined");
                var message = errorCodeInterpreter.GetErrorText(errorCode, true);
                StringAssert.DoesNotContain("Default", message, $"Error code {errorCode} contains the word 'default', which indicates that it has not been translated.");
                StringAssert.DoesNotContain(translator.GetTranslation(section, "Default"), message, $"Error code {errorCode} has the default translation!");
            }
        }
    }
}
