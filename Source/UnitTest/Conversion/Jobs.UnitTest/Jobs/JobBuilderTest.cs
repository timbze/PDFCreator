using NSubstitute;
using NUnit.Framework;
using pdfforge.DataStorage.Storage;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.Core.Workflow;
using System.Linq;

namespace pdfforge.PDFCreator.UnitTest.Conversion.Jobs.Jobs
{
    [TestFixture]
    public class JobBuilderTest
    {
        private IMailSignatureHelper _mailSignatureHelper;
        private IParametersManager _parametersManager;
        private PdfCreatorSettings _settings;
        private IStorage _storage;
        private JobInfo _jobInfo;
        private ConversionProfile _conversionProfile;

        [SetUp]
        public void SetUp()
        {
            _mailSignatureHelper = Substitute.For<IMailSignatureHelper>();
            _parametersManager = Substitute.For<IParametersManager>();

            _jobInfo = new JobInfo();
            _storage = Substitute.For<IStorage>();
            _settings = new PdfCreatorSettings(_storage);
            _conversionProfile = new ConversionProfile();
            _settings.ConversionProfiles.Add(_conversionProfile);
        }

        [Test]
        public void JobBuilderFree_SkipSaveFileDialogIsAlwaysFalse()
        {
            var jobBuilder = new JobBuilderFree(_mailSignatureHelper, _parametersManager);

            jobBuilder.BuildJobFromJobInfo(_jobInfo, _settings);

            Assert.IsFalse(_settings.ConversionProfiles.First().SkipPrintDialog);
        }

        [Test]
        public void JobBuilderPlus_SkipSaveFileDialogIsTrue_ReturnsTrue()
        {
            var jobBuilder = new JobBuilderPlus(_mailSignatureHelper, _parametersManager);
            _settings.ConversionProfiles.First().SkipPrintDialog = true;

            jobBuilder.BuildJobFromJobInfo(_jobInfo, _settings);

            Assert.IsTrue(_settings.ConversionProfiles.First().SkipPrintDialog);
        }

        [Test]
        public void JobBuilderPlus_SkipSaveFileDialogIsFalse_ReturnsFalse()
        {
            var jobBuilder = new JobBuilderPlus(_mailSignatureHelper, _parametersManager);
            _settings.ConversionProfiles.First().SkipPrintDialog = false;

            jobBuilder.BuildJobFromJobInfo(_jobInfo, _settings);

            Assert.IsFalse(_settings.ConversionProfiles.First().SkipPrintDialog);
        }
    }
}
