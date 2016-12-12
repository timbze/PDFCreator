using SystemInterface.Diagnostics;
using SystemInterface.IO;
using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.Services.Licensing;
using pdfforge.PDFCreator.UI.ViewModels.Helper;
using pdfforge.PDFCreator.UI.ViewModels.WindowViewModels;
using pdfforge.PDFCreator.Utilities.Process;

namespace pdfforge.PDFCreator.UnitTest.UI.ViewModels.WindowViewModels
{
    [TestFixture]
    public class WelcomeWindowViewModelTest
    {
        [SetUp]
        public void Setup()
        {
            _processStarter = Substitute.For<IProcessStarter>();

            var fileWrap = Substitute.For<IFile>();
            fileWrap.Exists(Arg.Any<string>()).Returns(true);

            _userGuideHelper = Substitute.For<IUserGuideHelper>();
        }

        private IProcessStarter _processStarter;
        private IUserGuideHelper _userGuideHelper;

        [Test]
        public void FacebookCommand_IsExecutable_StartsProcessWithDonateUrl()
        {
            var vm = new WelcomeWindowViewModel(_processStarter, new ButtonDisplayOptions(false, false), _userGuideHelper);
            Assert.IsTrue(vm.FacebookCommand.IsExecutable);

            vm.FacebookCommand.Execute(null);
            _processStarter.Received().Start(Urls.Facebook);
        }

        [Test]
        public void GooglePlusCommand_IsExecutable_StartsProcessWithDonateUrl()
        {
            var vm = new WelcomeWindowViewModel(_processStarter, new ButtonDisplayOptions(false, false), _userGuideHelper);
            Assert.IsTrue(vm.GooglePlusCommand.IsExecutable);

            vm.GooglePlusCommand.Execute(null);
            _processStarter.Received().Start(Urls.GooglePlus);
        }

        [Test]
        public void WhatsNewCommand_IsExecutable_OpensWhatsNewHelpTopic()
        {
            var vm = new WelcomeWindowViewModel(_processStarter, new ButtonDisplayOptions(false, false), _userGuideHelper);
            Assert.IsTrue(vm.WhatsNewCommand.IsExecutable);

            vm.WhatsNewCommand.Execute(null);
            _userGuideHelper.Received().ShowHelp(HelpTopic.WhatsNew);
        }
    }
}