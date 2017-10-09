using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Ghostscript;
using pdfforge.PDFCreator.Utilities;
using Rhino.Mocks;
using System.Collections.Generic;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.UnitTest.Conversion.Ghostscript
{
    [TestFixture]
    public class GhostscriptDiscoveryTest
    {
        [SetUp]
        public void SetUp()
        {
            _fileMock = MockRepository.GenerateStub<IFile>();
        }

        private IFile _fileMock;

        private GhostscriptDiscovery CreateGhostscriptDiscoveryWithMocks(string appPath = @"C:\Program Files\MyApp", bool x64 = true)
        {
            var assemblyHelper = MockRepository.GenerateStub<IAssemblyHelper>();

            assemblyHelper.Stub(x => x.GetAssemblyDirectory()).Return(appPath);

            var osHelper = MockRepository.GenerateStub<IOsHelper>();
            osHelper.Stub(x => x.Is64BitOperatingSystem).Return(x64);

            return new GhostscriptDiscovery(_fileMock, assemblyHelper);
        }

        [Test]
        public void GetGhostscriptInstance_WithGsInSubfolder_BuildsCorrectLibPaths()
        {
            const string appPath = @"C:\MyApp";
            const string gsPath = appPath + @"\Ghostscript";
            const string gsExe = gsPath + @"\Bin\gswin32c.exe";

            _fileMock.Stub(x => x.Exists(gsExe)).Return(true);
            var ghostscriptDiscovery = CreateGhostscriptDiscoveryWithMocks(appPath);

            var gs = ghostscriptDiscovery.GetGhostscriptInstance();
            var libPaths = gs.LibPaths.Split(';');

            Assert.Contains(gsPath + @"\Bin", libPaths);
            Assert.Contains(gsPath + @"\Fonts", libPaths);
            Assert.Contains(gsPath + @"\Lib", libPaths);
        }

        [Test]
        public void GetGhostscriptInstance_WithGsInSubfolder_FindsInstance()
        {
            const string appPath = @"C:\MyApp";
            const string gsPath = appPath + @"\Ghostscript";
            const string gsExe = gsPath + @"\Bin\gswin32c.exe";

            _fileMock.Stub(x => x.Exists(gsExe)).Return(true);
            var ghostscriptDiscovery = CreateGhostscriptDiscoveryWithMocks(appPath);

            var gs = ghostscriptDiscovery.GetGhostscriptInstance();

            Assert.IsNotNull(gs);
            Assert.AreEqual(gsExe, gs.ExePath);
            Assert.AreEqual("<internal>", gs.Version);
        }

        [Test]
        public void GetGhostscriptInstance_WithoutGs_ReturnsNull()
        {
            var ghostscriptDiscovery = CreateGhostscriptDiscoveryWithMocks();

            var gs = ghostscriptDiscovery.GetGhostscriptInstance();

            Assert.IsNull(gs);
        }

        [Test]
        public void GetGhostscript_IfPossibleGhostscriptPathsWereReplaced_FindsIntance()
        {
            var otherGhostscriptPath = new List<string>() { "SomeOtherFolder" };

            const string appPath = @"C:\MyApp";
            const string gsPath = appPath + @"\SomeOtherFolder";
            const string gsExe = gsPath + @"\Bin\gswin32c.exe";

            _fileMock.Stub(x => x.Exists(gsExe)).Return(true);

            var ghostscriptDiscovery = CreateGhostscriptDiscoveryWithMocks(appPath);
            ghostscriptDiscovery.PossibleGhostscriptPaths = otherGhostscriptPath;

            var gs = ghostscriptDiscovery.GetGhostscriptInstance();

            Assert.IsNotNull(gs);
            Assert.AreEqual(gsExe, gs.ExePath);
            Assert.AreEqual("<internal>", gs.Version);
        }
    }
}
