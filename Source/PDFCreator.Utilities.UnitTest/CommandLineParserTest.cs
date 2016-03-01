using NUnit.Framework;
using pdfforge.PDFCreator.Utilities;

namespace PDFCreator.Utilities.UnitTest
{
    [TestFixture]
    class CommandLineParserTest
    {
        [Test]
        public void TestBasicArgs()
        {
            string[] args = { "/abc", "/myTEST=123", "-xyz" };
            var clp = new CommandLineParser(args);

            Assert.IsTrue(clp.HasArgument("ABC"));
            Assert.IsTrue(clp.HasArgument("abc"));
            Assert.IsTrue(clp.HasArgument("myTEST"));
            Assert.IsTrue(clp.HasArgument("mytest"));
            Assert.IsTrue(clp.HasArgument("xyz"));
            Assert.IsTrue(clp.HasArgument("XYZ"));
            Assert.IsNull(clp.GetArgument("abc"));
            Assert.AreEqual("123", clp.GetArgument("mytest"));
        }

        [Test]
        public void EmptyArgument_Regression()
        {
            string[] args = { "", "/abc" };
            var clp = new CommandLineParser(args);

            Assert.IsTrue(clp.HasArgument("abc"));
        }
    }
}
