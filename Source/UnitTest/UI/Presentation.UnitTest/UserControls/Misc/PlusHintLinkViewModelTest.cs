using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.UI.Presentation.Commands;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Misc;
using System.Windows.Input;

namespace Presentation.UnitTest.UserControls.Misc
{
    [TestFixture]
    public class PlusHintLinkViewModelTest
    {
        private PlusHintViewModel _plusHintViewModel;
        private ITranslationUpdater _translationUpdater;
        private ICommandLocator _commandLocator;

        [SetUp]
        public void Setup()
        {
            _translationUpdater = Substitute.For<ITranslationUpdater>();
            _commandLocator = Substitute.For<ICommandLocator>();
        }

        private void InitPlusHintViewModel()
        {
            _plusHintViewModel = new PlusHintViewModel(_translationUpdater, _commandLocator);
        }

        [Test]
        public void Initialize_CommandLocatorSetsPlusHintUrlWithCorrectUrl()
        {
            var plusHintUrlCommand = Substitute.For<ICommand>();
            _commandLocator.GetInitializedCommand<UrlOpenCommand, string>(Urls.PlusHintLink).Returns(plusHintUrlCommand);
            InitPlusHintViewModel();
            Assert.AreSame(plusHintUrlCommand, _plusHintViewModel.UrlOpenCommand);
        }
    }
}
