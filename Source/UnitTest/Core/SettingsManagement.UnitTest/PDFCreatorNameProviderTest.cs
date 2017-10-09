using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.Utilities;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoNSubstitute;
using System;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.UnitTest.Core.SettingsManagement
{
    [TestFixture]
    public class PDFCreatorNameProviderTest
    {
        private IAssemblyHelper _assemblyHelper;
        private string _applicationPath;
        private IFixture _fixture;
        private IDirectory _directory;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture().Customize(new AutoConfiguredNSubstituteCustomization());
            _applicationPath = @"X:\Program Files\PDFCreator";
            _assemblyHelper = _fixture.Freeze<IAssemblyHelper>();
            _assemblyHelper.GetAssemblyDirectory().Returns(x => _applicationPath);
            _directory = _fixture.Freeze<IDirectory>();
        }

        [Test]
        public void GetExeName_WithOnePDFCreatorFile_ReturnsThatFile()
        {
            var exeName = "PDFCreator.exe";
            _directory.EnumerateFiles(_applicationPath, "PDFCreator*.exe")
                .Returns(new[] { exeName });
            var nameProvider = BuildNameProvider();

            var file = nameProvider.GetExeName();

            Assert.AreEqual(exeName, file);
        }

        [Test]
        public void GetExeName_IgnoresIrrelevantFiles_ReturnsThatFile()
        {
            var exeName = "PDFCreator.exe";
            _directory.EnumerateFiles(_applicationPath, "PDFCreator*.exe")
                .Returns(new[] { exeName, "PDFCreator.Test.exe" });
            var nameProvider = BuildNameProvider();

            var file = nameProvider.GetExeName();

            Assert.AreEqual(exeName, file);
        }

        [Test]
        public void GetExeName_WithTwoPDFCreatorFiles_ThrowsException()
        {
            _directory.EnumerateFiles(_applicationPath, Arg.Any<string>())
                .Returns(new[] { "PDFCreator.exe", "PDFCreator2.exe" });
            var nameProvider = BuildNameProvider();

            Assert.Throws<ApplicationException>(() => nameProvider.GetExeName());
        }

        [Test]
        public void GetExeName_WithNoMatchingFiles_ThrowsException()
        {
            _directory.EnumerateFiles(_applicationPath, Arg.Any<string>()).Returns(new string[] { });
            var nameProvider = BuildNameProvider();

            Assert.Throws<ApplicationException>(() => nameProvider.GetExeName());
        }

        [Test]
        public void GetExePath_WithOnePDFCreatorFile_ReturnsThatFullPath()
        {
            var exeName = "PDFCreator.exe";

            _directory.EnumerateFiles(_applicationPath, "PDFCreator*.exe")
                .Returns(new[] { exeName });
            var nameProvider = BuildNameProvider();

            var path = nameProvider.GetExePath();

            Assert.AreEqual($"{_applicationPath}\\{exeName}", path);
        }

        private PDFCreatorNameProvider BuildNameProvider()
        {
            return _fixture.Create<PDFCreatorNameProvider>();
        }
    }
}
