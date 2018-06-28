using NUnit.Framework;

namespace pdfforge.PDFCreator.Utilities.UnitTest
{
    public class ShellCommandTest
    {
        private ShellCommand BuildCommand(string[] args)
        {
            return new ShellCommand("", args, "print");
        }

        [Test]
        public void EmptyArgList_ReturnsEmptyString()
        {
            var argString = BuildCommand(new[] { "" })
                .GetReplacedCommandArgs();

            Assert.AreEqual("", argString);
        }

        [Test]
        public void WithArgList_ReturnsArgs()
        {
            var argString = BuildCommand(new[] { "/arg1", "-arg2" })
                .GetReplacedCommandArgs();

            Assert.AreEqual("/arg1 -arg2", argString);
        }

        [Test]
        public void WithArgsAndPlaceholders_ReturnsReplacedAndEscapedString()
        {
            var argString = BuildCommand(new[] { "/arg1", "%1" })
                .GetReplacedCommandArgs("test");

            Assert.AreEqual(@"/arg1 ""test""", argString);
        }

        [Test]
        public void WithPlaceholders_NonExistentPlaceholdersAreEmpty()
        {
            var argString = BuildCommand(new[] { "%1", "%2" })
                .GetReplacedCommandArgs("test");

            Assert.AreEqual(@"""test""", argString);
        }

        [Test]
        public void WithInvalidPlaceholder_DoesNotReplacePlaceholder()
        {
            var argString = BuildCommand(new[] { "%x" })
                .GetReplacedCommandArgs("test");

            Assert.AreEqual(@"%x", argString);
        }
    }
}
