using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.UI.Presentation.Commands;
using pdfforge.PDFCreator.UI.Presentation.Commands.UserGuide;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.Help;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Welcome;
using System.Windows.Input;

namespace Presentation.UnitTest.UserControls.Welcome
{
    [TestFixture]
    public class WelcomeWindowViewModelTest
    {
        private WelcomeViewModel _welcomeViewModel;
        private ICommand _whatsNewCommand;
        private ICommand _blogCommand;
        private ICommand _prioritySupportCommand;
        private WelcomeWindowTranslation _translation;

        [SetUp]
        public void Setup()
        {
            var commandLocator = Substitute.For<ICommandLocator>();
            _whatsNewCommand = Substitute.For<ICommand>();
            commandLocator.GetInitializedCommand<ShowUserGuideCommand, HelpTopic>(HelpTopic.WhatsNew).Returns(_whatsNewCommand);
            _blogCommand = Substitute.For<ICommand>();
            commandLocator.GetInitializedCommand<UrlOpenCommand, string>(Urls.Blog).Returns(_blogCommand);
            _prioritySupportCommand = Substitute.For<ICommand>();
            commandLocator.GetCommand<PrioritySupportUrlOpenCommand>().Returns(_prioritySupportCommand);

            _welcomeViewModel = new WelcomeViewModel(commandLocator, new DesignTimeTranslationUpdater(), new EditionHintOptionProvider(false, false));

            _translation = new WelcomeWindowTranslation();
        }

        [Test]
        public void Commands_GetInitialized()
        {
            Assert.AreSame(_whatsNewCommand, _welcomeViewModel.WhatsNewCommand, "WhatsNewCommand");
            Assert.AreSame(_blogCommand, _welcomeViewModel.BlogCommand, "BlogCommand");
            Assert.AreSame(_prioritySupportCommand, _welcomeViewModel.PrioritySupportCommand, "PrioritySupportCommand");
        }

        [Test]
        public void Title_IstranslationTitle()
        {
            Assert.AreEqual(_translation.Title, _welcomeViewModel.Title);
        }
    }
}
