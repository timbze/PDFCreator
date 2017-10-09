using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Processing.PdfToolsProcessing;
using pdfforge.PDFCreator.Core.Startup.Translations;
using pdfforge.PDFCreator.Core.StartupInterface;
using pdfforge.PDFCreator.Utilities;
using Translatable;

namespace pdfforge.PDFCreator.UnitTest.Conversion.Processing.PdfToolsProcessingUnitTest
{
    [TestFixture]
    public class PdfToolsLicensingStartUpConditionTest
    {
        private IPdfToolsLicensing _pdfToolsLicensing;
        private PdfToolsLicensingStartUpCondition _startUpCondition;
        private ProgramTranslation _translation;
        private IOsHelper _osHelper;
        private string _assemblyPath;

        [SetUp]
        public void SetUp()
        {
            _assemblyPath = @"X:\Some\path";
            _pdfToolsLicensing = Substitute.For<IPdfToolsLicensing>();
            var assemblyHelper = Substitute.For<IAssemblyHelper>();
            assemblyHelper.GetAssemblyDirectory().Returns(_assemblyPath);
            _osHelper = Substitute.For<IOsHelper>();
            _translation = new ProgramTranslation();
            _startUpCondition = new PdfToolsLicensingStartUpCondition(_pdfToolsLicensing, new TranslationFactory(), _osHelper, assemblyHelper);
        }

        [Test]
        public void Check_ApplyLicensingIsSuccessful_ReturnsSuccesfulResult()
        {
            _pdfToolsLicensing.Apply().Returns(true);

            var result = _startUpCondition.Check();

            Assert.IsTrue(result.IsSuccessful);
        }

        [Test]
        public void Check_ApplyLicensingFails_ReturnsFailedResult()
        {
            _pdfToolsLicensing.Apply().Returns(false);
            _pdfToolsLicensing.ExitCode = ExitCode.InvalidPdfToolsDocumentLicense;

            var result = _startUpCondition.Check();

            Assert.IsFalse(result.IsSuccessful, "IsSuccessful");
            var exitCode = (int)_pdfToolsLicensing.ExitCode;
            Assert.AreEqual(exitCode, result.ExitCode, "ExitCode");
            Assert.IsTrue(result.ShowMessage, "ShowMessage");
            Assert.AreEqual(_translation.GetFormattedErrorWithLicensedComponentTranslation(exitCode), _translation.GetFormattedErrorWithLicensedComponentTranslation(exitCode), "Message");
        }

        [Test]
        public void Check_WithX64Process_Registers64BitLibPath()
        {
            _osHelper.Is64BitProcess.Returns(true);

            _startUpCondition.Check();

            _osHelper.Received().AddDllDirectorySearchPath(_assemblyPath + @"\lib\x64");
        }

        [Test]
        public void Check_WithX86Process_Registers86BitLibPath()
        {
            _osHelper.Is64BitProcess.Returns(false);

            _startUpCondition.Check();

            _osHelper.Received().AddDllDirectorySearchPath(_assemblyPath + @"\lib\x86");
        }
    }
}
