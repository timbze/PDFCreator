using NUnit.Framework;
using pdfforge.PDFCreator.Utilities;

namespace PDFCreator.Utilities.IntegrationTest
{
    [TestFixture]
    internal class CommandLineUtilTest
    {
        [SetUp]
        public void SetUp()
        {
            _commandLineUtil = new CommandLineUtil();
        }

        private ICommandLineUtil _commandLineUtil;

        [Test]
        public void CommandLineToArgs_GivenSimpleCommandLine_ReturnsGoodArray()
        {
            const string commandLine = "/Test /Quote=\"This is a Test\"";
            var expected = new[] { "/Test", "/Quote=This is a Test" };
            Assert.AreEqual(expected, _commandLineUtil.CommandLineToArgs(commandLine));
        }
    }
}
