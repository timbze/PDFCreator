using NSubstitute;
using NUnit.Framework;
using Optional;
using pdfforge.LicenseValidator.Data;
using pdfforge.LicenseValidator.Interface;
using pdfforge.LicenseValidator.Interface.Data;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.UI.Presentation.Commands;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.Process;

namespace Presentation.UnitTest.Commands
{
    [TestFixture]
    public class PrioritySupportUrlOpenCommandTest
    {
        [Test]
        public void PrioritySupportUrlOpenCommand_Execute_ProcessStartWithExpectedUrl()
        {
            var processStarter = Substitute.For<IProcessStarter>();
            var versionHelper = Substitute.For<IVersionHelper>();
            versionHelper.FormatWithBuildNumber().Returns("1.2.3 (Preview)");
            var licenseChecker = Substitute.For<ILicenseChecker>();
            licenseChecker.GetSavedLicenseKey().Returns("AAAAABBBBB".Some<string, LicenseError>());
            var config = new Configuration(Product.PdfCreatorBusiness, "1.2.3 (Preview)", @"SOFTWARE\PDFCreatorTest||||");
            var expectedUrl = $"{Urls.PrioritySupport}?edition=pdfcreator_business&version=1.2.3_Preview&license_key=AAAAA-BBBBB";
            var prioritySupportUrlOpenCommand = new PrioritySupportUrlOpenCommand(processStarter, versionHelper, licenseChecker, new LicenseKeySyntaxChecker(), config);

            prioritySupportUrlOpenCommand.Execute(null);

            Assert.IsTrue(prioritySupportUrlOpenCommand.CanExecute(null), "CanExecute");
            processStarter.Received().Start(expectedUrl);
        }

        [Test]
        public void CustomPrioritySupportUrlOpenCommand_Execute_ProcessStartWithExpectedUrl()
        {
            var processStarter = Substitute.For<IProcessStarter>();
            var versionHelper = Substitute.For<IVersionHelper>();
            versionHelper.FormatWithBuildNumber().Returns("1.2.3 (Preview)");
            var expectedUrl = $"{Urls.PrioritySupport}?edition=pdfcreator_custom&version=1.2.3_Preview&license_key=";
            var prioritySupportUrlOpenCommand = new CustomPrioritySupportUrlOpenCommand(processStarter, versionHelper);

            prioritySupportUrlOpenCommand.Execute(null);

            Assert.IsTrue(prioritySupportUrlOpenCommand.CanExecute(null), "CanExecute");
            processStarter.Received().Start(expectedUrl);
        }

        [Test]
        public void DisabledPrioritySupportUrlOpenCommand_CanExecute_ReturnsFalse()
        {
            var processStarter = Substitute.For<IProcessStarter>();
            var versionHelper = Substitute.For<IVersionHelper>();
            versionHelper.FormatWithBuildNumber().Returns("1.2.3 (Preview)");
            var expectedUrl = "";
            var prioritySupportUrlOpenCommand = new DisabledPrioritySupportUrlOpenCommand(processStarter, versionHelper);

            prioritySupportUrlOpenCommand.Execute(null);

            Assert.IsFalse(prioritySupportUrlOpenCommand.CanExecute(null), "CanExecute");
            processStarter.Received().Start(expectedUrl);
        }
    }
}
