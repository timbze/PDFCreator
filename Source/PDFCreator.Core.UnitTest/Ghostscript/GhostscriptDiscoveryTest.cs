using System;
using SystemInterface.IO;
using SystemInterface.Microsoft.Win32;
using SystemWrapper.IO;
using NUnit.Framework;
using pdfforge.PDFCreator.Core.Ghostscript;
using pdfforge.PDFCreator.Utilities;
using Rhino.Mocks;

namespace PDFCreator.Core.UnitTest.Ghostscript
{
    [TestFixture]
    public class GhostscriptDiscoveryTest
    {
        private IFile _fileMock;
        private IRegistry _registryMock;

        [SetUp]
        public void SetUp()
        {
            _fileMock = MockRepository.GenerateStub<IFile>();
            _registryMock = MockRepository.GenerateStub<IRegistry>();
        }

        [Test]
        public void Initialize_AppPath_NotEmpty()
        {
            GhostscriptDiscovery ghostscriptDiscovery = CreateGhostscriptDiscoveryWithMocks();
            
            Assert.IsNotNullOrEmpty(ghostscriptDiscovery.ApplicationPath);
        }

        [Test]
        public void Initialize_OnX86_GivesCorrectRegistryKey()
        {
            GhostscriptDiscovery ghostscriptDiscovery = CreateGhostscriptDiscoveryWithMocks(x64: false);

            StringAssert.StartsWith("SOFTWARE\\GPL Ghostscript", ghostscriptDiscovery.RegistryPath);
        }

        [Test]
        public void Initialize_OnX64_GivesCorrectRegistryKey()
        {
            GhostscriptDiscovery ghostscriptDiscovery = CreateGhostscriptDiscoveryWithMocks(x64: true);

            StringAssert.StartsWith("SOFTWARE\\Wow6432Node\\GPL Ghostscript", ghostscriptDiscovery.RegistryPath);
        }

        [Test]
        public void FindInternalInstance_WithoutGs_ReturnsNull()
        {
            GhostscriptDiscovery ghostscriptDiscovery = CreateGhostscriptDiscoveryWithMocks();

            var hklm = MockRepository.GenerateStub<IRegistryKey>();
            _registryMock.Stub(x => x.LocalMachine).Return(hklm);

            AddGhostscriptVersions(new string[] { }, ghostscriptDiscovery.RegistryPath, _fileMock, hklm);


            var gs = ghostscriptDiscovery.FindInternalInstance();

            Assert.IsNull(gs);
        }
        
        [Test]
        public void FindInternalInstance_WithGsInSubfolder_FindsInstance()
        {
            const string appPath = @"C:\MyApp";
            const string gsPath = appPath + @"\Ghostscript";
            const string gsExe = gsPath + @"\Bin\gswin32c.exe";

            _fileMock.Stub(x => x.Exists(gsExe)).Return(true);
            GhostscriptDiscovery ghostscriptDiscovery = CreateGhostscriptDiscoveryWithMocks(appPath);

            var gs = ghostscriptDiscovery.FindInternalInstance();

            Assert.IsNotNull(gs);
            Assert.AreEqual(gsExe, gs.ExePath);
            Assert.AreEqual("<internal>", gs.Version);
        }

        [Test]
        public void FindInternalInstance_WithGsInSubfolder_BuildsCorrectLibPaths()
        {
            const string appPath = @"C:\MyApp";
            const string gsPath = appPath + @"\Ghostscript";
            const string gsExe = gsPath + @"\Bin\gswin32c.exe";

            _fileMock.Stub(x => x.Exists(gsExe)).Return(true);
            GhostscriptDiscovery ghostscriptDiscovery = CreateGhostscriptDiscoveryWithMocks(appPath);

            var gs = ghostscriptDiscovery.FindInternalInstance();
            string[] libPaths = gs.LibPaths.Split(';');

            Assert.Contains(gsPath + @"\Bin", libPaths);
            Assert.Contains(gsPath + @"\Fonts", libPaths);
            Assert.Contains(gsPath + @"\Lib", libPaths);
        }

        [Test]
        public void FindInternalInstance_WithGsInSubFolderLikeInVisualStudio_FindsInstance()
        {
            const string appPath = @"C:\MyApp\bin\Debug";
            const string gsPath = appPath + @"\..\..\Ghostscript";
            const string gsExe = gsPath + @"\Bin\gswin32c.exe";

            _fileMock.Stub(x => x.Exists(gsExe)).Return(true);
            GhostscriptDiscovery ghostscriptDiscovery = CreateGhostscriptDiscoveryWithMocks(appPath);

            var gs = ghostscriptDiscovery.FindInternalInstance();

            Assert.IsNotNull(gs);
            Assert.AreEqual(gsExe, gs.ExePath);
            Assert.AreEqual("<internal>", gs.Version);
        }

        [Test]
        public void GetBestGhostscriptInstance_WithInternalAndNoRegistryInstance_ReturnsInternalInstance()
        {
            const string appPath = @"C:\MyApp";
            const string gsPath = appPath + @"\Ghostscript";
            const string gsExe = gsPath + @"\Bin\gswin32c.exe";

            _fileMock.Stub(x => x.Exists(gsExe)).Return(true);
            GhostscriptDiscovery ghostscriptDiscovery = CreateGhostscriptDiscoveryWithMocks(appPath);

            var gs = ghostscriptDiscovery.GetBestGhostscriptInstance("9.10");

            Assert.AreEqual("<internal>", gs.Version);
        }

        [Test]
        public void GetBestGhostscriptInstance_WithRegistryInstance_ReturnsRegistryInstance()
        {
            var hklm = MockRepository.GenerateStub<IRegistryKey>();
            _registryMock.Stub(x => x.LocalMachine).Return(hklm);

            GhostscriptDiscovery ghostscriptDiscovery = CreateGhostscriptDiscoveryWithMocks();
            AddGhostscriptVersions(new[] { "9.15" }, ghostscriptDiscovery.RegistryPath, _fileMock, hklm);

            var gs = ghostscriptDiscovery.GetBestGhostscriptInstance("9.15");

            Assert.AreEqual("9.15", gs.Version);
        }

        [TestCase("9.14")]
        [TestCase("9.15")]
        public void GetBestGhostscriptInstance_WithMultipleRegistryInstances_ReturnsGivenRegistryInstance(string gsVersion)
        {
            var hklm = MockRepository.GenerateStub<IRegistryKey>();
            _registryMock.Stub(x => x.LocalMachine).Return(hklm);

            GhostscriptDiscovery ghostscriptDiscovery = CreateGhostscriptDiscoveryWithMocks();
            AddGhostscriptVersions(new[] { "9.14", "9.15" }, ghostscriptDiscovery.RegistryPath, _fileMock, hklm);

            var gs = ghostscriptDiscovery.GetBestGhostscriptInstance(gsVersion);

            Assert.AreEqual(gsVersion, gs.Version);
        }

        [Test]
        public void GetBestGhostscriptInstance_WithUnsupportedRegistryInstances_ReturnsNull()
        {
            var hklm = MockRepository.GenerateStub<IRegistryKey>();
            _registryMock.Stub(x => x.LocalMachine).Return(hklm);

            GhostscriptDiscovery ghostscriptDiscovery = CreateGhostscriptDiscoveryWithMocks();
            AddGhostscriptVersions(new[] { "9.14", "9.15" }, ghostscriptDiscovery.RegistryPath, _fileMock, hklm);

            var gs = ghostscriptDiscovery.GetBestGhostscriptInstance("9.20");

            Assert.IsNull(gs);
        }

        [Test]
        public void GetBestGhostscriptInstance_WithoutParameter_ReturnsDefaultVersion()
        {
            var hklm = MockRepository.GenerateStub<IRegistryKey>();
            _registryMock.Stub(x => x.LocalMachine).Return(hklm);

            GhostscriptDiscovery ghostscriptDiscovery = CreateGhostscriptDiscoveryWithMocks();
            AddGhostscriptVersions(new[] { "9.14", "9.10" }, ghostscriptDiscovery.RegistryPath, _fileMock, hklm);

            var gs = ghostscriptDiscovery.GetBestGhostscriptInstance();

            Assert.AreEqual(GhostscriptDiscovery.GhostscriptVersion, gs.Version);
        }

        [Test]
        public void GetBestGhostscriptInstance_WhenExceptionIsThrownInRegistry_ReturnsNull()
        {
            var hklm = MockRepository.GenerateStub<IRegistryKey>();
            _registryMock.Stub(x => x.LocalMachine).Return(hklm);

            GhostscriptDiscovery ghostscriptDiscovery = CreateGhostscriptDiscoveryWithMocks();

            AddGhostscriptVersionsWithException(new[] { "9.15" }, ghostscriptDiscovery.RegistryPath, _fileMock, hklm);
            
            var gs = ghostscriptDiscovery.GetBestGhostscriptInstance("9.10");

            Assert.IsNull(gs);
        }

        [Test]
        public void GetBestGhostscriptInstance_WithInternalAndRegistryInstance_ReturnsInternalInstance()
        {
            const string appPath = @"C:\MyApp";
            const string gsPath = appPath + @"\Ghostscript";
            const string gsExe = gsPath + @"\Bin\gswin32c.exe";

            var hklm = MockRepository.GenerateStub<IRegistryKey>();
            _registryMock.Stub(x => x.LocalMachine).Return(hklm);

            _fileMock.Stub(x => x.Exists(gsExe)).Return(true);
            GhostscriptDiscovery ghostscriptDiscovery = CreateGhostscriptDiscoveryWithMocks(appPath);

            AddGhostscriptVersions(new []{"9.15"}, ghostscriptDiscovery.RegistryPath, _fileMock, hklm);

            var gs = ghostscriptDiscovery.GetBestGhostscriptInstance("9.10");

            Assert.AreEqual("<internal>", gs.Version);
        }

        [Test]
        public void GetBestGhostscriptInstance_WithNoGhostscriptRegistryKey_ReturnsNull()
        {
            var hklm = MockRepository.GenerateStub<IRegistryKey>();
            _registryMock.Stub(x => x.LocalMachine).Return(hklm);

            GhostscriptDiscovery ghostscriptDiscovery = CreateGhostscriptDiscoveryWithMocks();

            hklm.Stub(x => x.OpenSubKey(ghostscriptDiscovery.RegistryPath)).Return(null);

            var gs = ghostscriptDiscovery.GetBestGhostscriptInstance("9.10");

            Assert.IsNull(gs);
        }

        private GhostscriptDiscovery CreateGhostscriptDiscoveryWithMocks(string appPath = @"C:\Program Files\MyApp", bool x64 = true)
        {
            var pathWrapSafe = new PathWrapSafe();
            var assemblyHelper = MockRepository.GenerateStub<IAssemblyHelper>();

            assemblyHelper.Stub(x => x.GetCurrentAssemblyDirectory()).Return(appPath);

            var osHelper = MockRepository.GenerateStub<IOsHelper>();
            osHelper.Stub(x => x.Is64BitOperatingSystem).Return(x64);

            return new GhostscriptDiscovery(_fileMock, _registryMock, assemblyHelper, osHelper);
        }

        private void AddGhostscriptVersionsWithException(string[] gsVersions, string registryPath, IFile fileMock, IRegistryKey hklmMock)
        {
            var regKeyMock = MockRepository.GenerateStub<IRegistryKey>();

            // Set list of GS versions
            hklmMock.Stub(x => x.OpenSubKey(registryPath)).Return(regKeyMock);
            regKeyMock.Stub(x => x.GetSubKeyNames()).Return(gsVersions);


            foreach (var gsVersionX in gsVersions)
            {
                // copy to local variable for usage in closures
                string gsVersion = gsVersionX;

                // Add Subkey for GS Version
                var versionRegKeyMock = MockRepository.GenerateStub<IRegistryKey>();
                versionRegKeyMock.Stub(x => x.GetValue("GS_DLL")).Return(string.Format(@"C:\Ghostscript\{0}\gs.dll", gsVersion));
                versionRegKeyMock.Stub(x => x.GetValue("GS_DLL")).Return(string.Format(@"C:\Ghostscript\{0}\Lib;C:\Ghostscript\{0}\Fonts;C:\Ghostscript\{0}\Bin", gsVersion));
                hklmMock.Stub(x => x.OpenSubKey(registryPath + "\\" + gsVersion)).Throw(new Exception());

                fileMock.Stub(x => x.Exists(string.Format(@"C:\Ghostscript\{0}\gswin32c.exe", gsVersion))).Return(true);
            }
        }

        private void AddGhostscriptVersions(string[] gsVersions, string registryPath, IFile fileMock, IRegistryKey hklmMock)
        {
            var regKeyMock = MockRepository.GenerateStub<IRegistryKey>();
            
            // Set list of GS versions
            hklmMock.Stub(x => x.OpenSubKey(registryPath)).Return(regKeyMock);
            regKeyMock.Stub(x => x.GetSubKeyNames()).Return(gsVersions);


            foreach (var gsVersionX in gsVersions)
            {
                // copy to local variable for usage in closures
                string gsVersion = gsVersionX; 
                
                // Add Subkey for GS Version
                var versionRegKeyMock = MockRepository.GenerateStub<IRegistryKey>();
                versionRegKeyMock.Stub(x => x.GetValue("GS_DLL")).Return(string.Format(@"C:\Ghostscript\{0}\gs.dll", gsVersion));
                versionRegKeyMock.Stub(x => x.GetValue("GS_DLL")).Return(string.Format(@"C:\Ghostscript\{0}\Lib;C:\Ghostscript\{0}\Fonts;C:\Ghostscript\{0}\Bin", gsVersion));
                hklmMock.Stub(x => x.OpenSubKey(registryPath + "\\" + gsVersion)).Return(versionRegKeyMock);

                fileMock.Stub(x => x.Exists(string.Format(@"C:\Ghostscript\{0}\gswin32c.exe", gsVersion))).Return(true);
            }
        }
    }
}
