using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Ghostscript;
using pdfforge.PDFCreator.Core.Startup.StartConditions;
using pdfforge.PDFCreator.Core.Startup.Translations;
using pdfforge.PDFCreator.Core.StartupInterface;

namespace pdfforge.PDFCreator.UnitTest.Startup
{
    [TestFixture]
    public class GhostscriptConditionTest
    {
        [Test]
        public void WhenGhostscriptFound_Successful()
        {
            var gsDiscovery = Substitute.For<IGhostscriptDiscovery>();
            gsDiscovery.GetGhostscriptInstance().Returns(new GhostscriptVersion("", "", ""));

            var ghostscriptCondition = new GhostscriptCondition(gsDiscovery, new StartupTranslation());

            var result = ghostscriptCondition.Check();

            Assert.IsTrue(result.IsSuccessful);
            Assert.AreEqual((int)ExitCode.Ok, result.ExitCode);
        }

        [Test]
        public void WhenNoGhostscriptFound_ReturnsError()
        {
            var gsDiscovery = Substitute.For<IGhostscriptDiscovery>();
            gsDiscovery.GetGhostscriptInstance().Returns((GhostscriptVersion) null);

            var ghostscriptCondition = new GhostscriptCondition(gsDiscovery, new StartupTranslation());

            var result = ghostscriptCondition.Check();

            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual((int)ExitCode.GhostScriptNotFound, result.ExitCode);
        }
    }
}
