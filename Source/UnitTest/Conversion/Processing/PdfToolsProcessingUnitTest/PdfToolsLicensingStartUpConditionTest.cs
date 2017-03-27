using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Processing.PdfToolsProcessing;
using pdfforge.PDFCreator.Core.Startup.Translations;
using pdfforge.PDFCreator.Core.StartupInterface;

namespace pdfforge.PDFCreator.UnitTest.Conversion.Processing.PdfToolsProcessingUnitTest
{
    [TestFixture]
    public class PdfToolsLicensingStartUpConditionTest
    {
        private IPdfToolsLicensing _pdfToolsLicensing;
        private PdfToolsLicensingStartUpCondition _startUpCondition;
        private ProgramTranslation _translation;

        [SetUp]
        public void SetUp()
        {
            _pdfToolsLicensing = Substitute.For<IPdfToolsLicensing>();
            _translation = new ProgramTranslation();
            _startUpCondition = new PdfToolsLicensingStartUpCondition(_pdfToolsLicensing, _translation);
        }

        [Test]
        public void Check_ApplyLicensingIsSuccessful_ReturnsSuccesfullResult()
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
    }
}
