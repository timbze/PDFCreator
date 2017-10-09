using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.Core.Workflow;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoNSubstitute;
using System.Linq;

namespace pdfforge.PDFCreator.Core.DirectConversion.UnitTest
{
    [TestFixture]
    public class DirectConversionHelperTest
    {
        private IFixture _fixture;
        private IDirectConversionProvider _directConversionProvider;
        private IJobInfoQueue _jobInfoQueue;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            _directConversionProvider = _fixture.Freeze<IDirectConversionProvider>();
            _jobInfoQueue = _fixture.Freeze<IJobInfoQueue>();
            var settingsProvider = _fixture.Freeze<ISettingsProvider>();
            settingsProvider.Settings.Returns(new PdfCreatorSettings(null));
        }

        [TestCase(@"C:\Test.pdf")]
        [TestCase(@"C:\Test.PDF")]
        [TestCase(@"C:\Test.ps")]
        [TestCase(@"C:\Test.PS")]
        [TestCase(@"C:\Some weird folder name\WithDot.doc\Test.PS")]
        public void CanConvertDirectly_WithAppropriateFilenames_IsTrue(string filename)
        {
            var directConversionHelper = _fixture.Create<DirectConversionHelper>();

            Assert.IsTrue(directConversionHelper.CanConvertDirectly(filename));
        }

        [TestCase(@"C:\Test.doc")]
        [TestCase(@"C:\Test")]
        [TestCase(@"C:\Some weird folder name\WithDot.doc\Test")]
        public void CanConvertDirectly_WithInappropriateFilenames_IsFalse(string filename)
        {
            var directConversionHelper = _fixture.Create<DirectConversionHelper>();

            Assert.IsFalse(directConversionHelper.CanConvertDirectly(filename));
        }

        [Test]
        public void ConvertDirectly_WithInvalidFile_DoesNothing()
        {
            var directConversionHelper = _fixture.Create<DirectConversionHelper>();

            directConversionHelper.ConvertDirectly(@"C:\InvalidFile.docx");

            Assert.IsFalse(_directConversionProvider.ReceivedCalls().Any(), "The DirectConversionProvider was called though it should not have been!");
            Assert.IsFalse(_jobInfoQueue.ReceivedCalls().Any(), "The JobInfoQueue was called though it should not have been!");
        }

        [Test]
        public void ConvertDirectly_WithValidFile_AddsJobToJobInfoQueue()
        {
            const string expectedInfFile = "X:\\MyFile.inf";

            var pdfDirectConversion = _fixture.Freeze<IDirectConversion>();
            pdfDirectConversion.TransformToInfFile(Arg.Any<string>(), Arg.Any<string>()).Returns(expectedInfFile);
            _directConversionProvider.GetPdfConversion().Returns(pdfDirectConversion);

            var jobInfoManager = _fixture.Freeze<IJobInfoManager>();
            jobInfoManager.ReadFromInfFile(Arg.Any<string>()).Returns(x => new JobInfo { InfFile = x.Arg<string>() });

            var directConversionHelper = _fixture.Create<DirectConversionHelper>();

            directConversionHelper.ConvertDirectly(@"C:\InvalidFile.pdf");

            _jobInfoQueue.Received(1).Add(Arg.Is<JobInfo>(jobInfo => jobInfo.InfFile == expectedInfFile));
        }

        [Test]
        public void ConvertDirectly_WhenInfFileIsNull_DoesNothing()
        {
            var directConversionHelper = _fixture.Create<DirectConversionHelper>();

            directConversionHelper.ConvertDirectly(@"C:\InvalidFile.pdf");

            Assert.IsFalse(_jobInfoQueue.ReceivedCalls().Any(), "The JobInfoQueue was called though it should not have been!");
        }

        [Test]
        public void ConvertDirectly_WithPdfFile_UsesPdfConverter()
        {
            var directConversionHelper = _fixture.Create<DirectConversionHelper>();

            directConversionHelper.ConvertDirectly(@"C:\InvalidFile.pdf");

            _directConversionProvider.Received(1).GetPdfConversion();
        }

        [Test]
        public void ConvertDirectly_WithPsFile_UsesPsConverter()
        {
            var directConversionHelper = _fixture.Create<DirectConversionHelper>();

            directConversionHelper.ConvertDirectly(@"C:\InvalidFile.ps");

            _directConversionProvider.Received(1).GetPsConversion();
        }
    }
}
