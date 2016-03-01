using NUnit.Framework;
using pdfforge.PDFCreator.Shared.Licensing;

namespace PDFCreator.Shared.Test.Licensing
{
    [TestFixture]
    class TerminalServerDetectionTest
    {
        private TerminalServerDetection _terminalServerDetection;
        private TerminalServerDetection.OSVERSIONINFOEX _osvi;

        private TerminalServerDetection.OSVERSIONINFOEX Return_osvi()
        {
            return _osvi;
        }

        [SetUp]
        public void SetUp()
        {
            _osvi = new TerminalServerDetection.OSVERSIONINFOEX();
            _terminalServerDetection = new TerminalServerDetection(Return_osvi);
        }

        [Test]
        public void IsTerminalServer_SuiteMaskIsTerminal_ReturnsTrue()
        {
            _osvi.wSuiteMask = TerminalServerDetection.SuiteMask.VER_SUITE_TERMINAL;
            Assert.IsTrue(_terminalServerDetection.IsTerminalServer());
        }

        [Test]
        public void IsTerminalServer_SuiteMaskIsSingeUserTs_ReturnsFalse()
        {
            _osvi.wSuiteMask = TerminalServerDetection.SuiteMask.VER_SUITE_SINGLEUSERTS;
            Assert.IsFalse(_terminalServerDetection.IsTerminalServer());
        }

        [Test]
        public void IsTerminalServer_SuiteMaskIsTerminalAndNotSingeUserTs_ReturnsTrue()
        {
            _osvi.wSuiteMask = TerminalServerDetection.SuiteMask.VER_SUITE_TERMINAL & ~TerminalServerDetection.SuiteMask.VER_SUITE_SINGLEUSERTS;
            Assert.IsTrue(_terminalServerDetection.IsTerminalServer());
        }

        [Test]
        public void IsTerminalServer_SuiteMaskIsTerminalAndSingeUserTs_ReturnsFalse()
        {
            _osvi.wSuiteMask = TerminalServerDetection.SuiteMask.VER_SUITE_TERMINAL & TerminalServerDetection.SuiteMask.VER_SUITE_SINGLEUSERTS;
            Assert.IsFalse(_terminalServerDetection.IsTerminalServer());
        }

        [Test]
        public void IsTerminalServer_SuiteMaskIsNotTerminalAndNotSingeUserTs_ReturnsFalse()
        {
            _osvi.wSuiteMask = ~TerminalServerDetection.SuiteMask.VER_SUITE_TERMINAL & ~TerminalServerDetection.SuiteMask.VER_SUITE_SINGLEUSERTS;
            Assert.IsFalse(_terminalServerDetection.IsTerminalServer());
        }
    }
}
