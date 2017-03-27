using SystemInterface.IO;
using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.UI.ViewModels.Helper;
using pdfforge.PDFCreator.UI.ViewModels.WindowViewModels;
using pdfforge.PDFCreator.UI.ViewModels.WindowViewModels.Translations;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.Process;

namespace pdfforge.PDFCreator.UnitTest.UI.ViewModels.WindowViewModels
{
    [TestFixture]
    public class AboutWindowViewModelTest
    {
        [SetUp]
        public void Setup()
        {
            _process = Substitute.For<IProcessStarter>();
            _versionHelper = Substitute.For<IVersionHelper>();
            _versionHelper.FormatWithBuildNumber().Returns(Version);

            var fileWrap = Substitute.For<IFile>();
            fileWrap.Exists(Arg.Any<string>()).Returns(true);

            _userGuideHelper = Substitute.For<IUserGuideHelper>();
        }

        private IProcessStarter _process;
        private IVersionHelper _versionHelper;
        private IUserGuideHelper _userGuideHelper;
        private const string Version = "v1.2.3";

        private AboutWindowViewModel BuildAboutWindowViewModel(ButtonDisplayOptions buttonDisplayOptions = null)
        {
            if (buttonDisplayOptions == null)
                buttonDisplayOptions = new ButtonDisplayOptions(false, false);

            return new AboutWindowViewModel(_process, new ApplicationNameProvider("PDFCreator"),
                _versionHelper, _userGuideHelper, buttonDisplayOptions, new AboutWindowTranslation());
        }

        [Test]
        public void DonateCommand_IsExecutable_StartsProcessWithDonateUrl()
        {
            var vm = BuildAboutWindowViewModel();
            Assert.IsTrue(vm.DonateCommand.IsExecutable);

            vm.DonateCommand.Execute(null);
            _process.Received().Start(Urls.DonateUrl);
        }

        [Test]
        public void FacebookCommand_IsExecutable_StartsProcessWithDonateUrl()
        {
            var vm = BuildAboutWindowViewModel();
            Assert.IsTrue(vm.FacebookCommand.IsExecutable);

            vm.FacebookCommand.Execute(null);
            _process.Received().Start(Urls.Facebook);
        }

        [Test]
        public void GooglePlusCommand_IsExecutable_StartsProcessWithDonateUrl()
        {
            var vm = BuildAboutWindowViewModel();
            Assert.IsTrue(vm.GooglePlusCommand.IsExecutable);

            vm.GooglePlusCommand.Execute(null);
            _process.Received().Start(Urls.GooglePlus);
        }

        [Test]
        public void InitializeTest()
        {
            var vm = BuildAboutWindowViewModel();
            Assert.AreEqual(Version, vm.VersionText, "VersionText");
        }

        [Test]
        public void PdfforgeWebsiteCommand_IsExecutable_StartsProcessWithDonateUrl()
        {
            var vm = BuildAboutWindowViewModel();
            Assert.IsTrue(vm.PdfforgeWebsiteCommand.IsExecutable);

            vm.PdfforgeWebsiteCommand.Execute(null);
            _process.Received().Start(Urls.PdfforgeWebsiteUrl);
        }

        [Test]
        public void ShowLicenseCommand_IsExecutablee_OpensLicenseHelpTopic()
        {
            var vm = BuildAboutWindowViewModel();
            Assert.IsTrue(vm.ShowLicenseCommand.IsExecutable);

            vm.ShowLicenseCommand.Execute(null);
            _userGuideHelper.Received().ShowHelp(HelpTopic.License);
        }

        [Test]
        public void ShowManualCommand_IsExecutable_OpensGeneralHelpTopic()
        {
            var vm = BuildAboutWindowViewModel();
            Assert.IsTrue(vm.ShowManualCommand.IsExecutable);

            vm.ShowManualCommand.Execute(null);
            _userGuideHelper.Received().ShowHelp(HelpTopic.General);
        }

        [Test]
        public void HideSocialMediaButtons_SetsPropertyTrue()
        {
            var vm = BuildAboutWindowViewModel(new ButtonDisplayOptions(true, false));

            Assert.IsTrue(vm.HideSocialMediaButtons);
            Assert.IsFalse(vm.HideDonateButton);
        }

        [Test]
        public void HideDonateButton_SetsPropertyTrue()
        {
            var vm = BuildAboutWindowViewModel(new ButtonDisplayOptions(false, true));

            Assert.IsFalse(vm.HideSocialMediaButtons);
            Assert.IsTrue(vm.HideDonateButton);
        }
    }
}