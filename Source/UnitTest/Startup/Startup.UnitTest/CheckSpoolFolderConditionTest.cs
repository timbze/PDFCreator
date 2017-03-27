using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Jobs.FolderProvider;
using pdfforge.PDFCreator.Core.Startup.StartConditions;
using pdfforge.PDFCreator.Core.StartupInterface;
using pdfforge.PDFCreator.UI.ViewModels.Assistants;
using pdfforge.PDFCreator.UI.ViewModels.Translations;

namespace pdfforge.PDFCreator.UnitTest.Startup
{
    [TestFixture]
    public class CheckSpoolFolderConditionTest
    {
        private ISpoolFolderAccess _spoolFolderAccess;
        private IRepairSpoolFolderAssistant _repairSpoolFolderAssistant;
        private ISpoolerProvider _spoolerProvider;

        [SetUp]
        public void Setup()
        {
            _spoolFolderAccess = Substitute.For<ISpoolFolderAccess>();
            _repairSpoolFolderAssistant = Substitute.For<IRepairSpoolFolderAssistant>();
            _spoolerProvider = Substitute.For<ISpoolerProvider>();
        }

        [Test]
        public void WhenSpoolFolderIsAccessible_Success()
        {
            _spoolFolderAccess.CanAccess().Returns(true);
            var spoolFolderCondition = new CheckSpoolFolderCondition(_spoolFolderAccess, null, new ApplicationTranslation(), _spoolerProvider);

            var result = spoolFolderCondition.Check();

            Assert.IsTrue(result.IsSuccessful);
        }

        [Test]
        public void RepairDoesNotRegainAccess_FailsWithSpoolFolderInaccessible()
        {
            _spoolFolderAccess.CanAccess().Returns(false);
            var spoolFolderCondition = new CheckSpoolFolderCondition(_spoolFolderAccess, _repairSpoolFolderAssistant, new ApplicationTranslation(), _spoolerProvider);

            var result = spoolFolderCondition.Check();

            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual((int)ExitCode.SpoolFolderInaccessible, result.ExitCode);
        }

        [Test]
        public void RepairWasSuccessful_Success()
        {
            bool isAccessible = false;
            _spoolFolderAccess.CanAccess().Returns(x => isAccessible);

            // After repair was called, the spool folder shall be accessible
            _repairSpoolFolderAssistant
                .When(x => x.TryRepairSpoolPath())
                .Do(x => isAccessible = true);

            var spoolFolderCondition = new CheckSpoolFolderCondition(_spoolFolderAccess, _repairSpoolFolderAssistant, new ApplicationTranslation(), _spoolerProvider);

            var result = spoolFolderCondition.Check();

            Assert.IsTrue(result.IsSuccessful);
        }
    }
}
