using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.Workflow.Exceptions;
using pdfforge.PDFCreator.UI.Presentation.Commands;
using pdfforge.PDFCreator.UI.Presentation.DesignTime;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob.PlusHint;
using pdfforge.PDFCreator.Utilities.Threading;
using System.Windows.Input;
using Translatable;

namespace Presentation.UnitTest.UserControls
{
    [TestFixture]
    public class PlusHintViewModelTest
    {
        private PlusHintViewModel _plusHintViewModel;
        private ITranslationUpdater _translationUpdater;
        private ICommandLocator _commandLocator;
        private IPlusHintHelper _plusHintHelper;

        [SetUp]
        public void Setup()
        {
            _translationUpdater = new TranslationUpdater(new TranslationFactory(), new ThreadManager());
            _plusHintHelper = Substitute.For<IPlusHintHelper>();
            _commandLocator = Substitute.For<ICommandLocator>();
            InitViewModel();
        }

        private void InitViewModel()
        {
            _plusHintViewModel = new PlusHintViewModel(_translationUpdater, _commandLocator, _plusHintHelper);
        }

        [Test]
        public void Initialize_ThankYouTextGetsSetCorrectly()
        {
            var currentNumberOfPrintJobs = 10;
            _plusHintHelper.CurrentJobCounter.Returns(currentNumberOfPrintJobs);

            InitViewModel();

            Assert.AreEqual($"You have already converted {currentNumberOfPrintJobs} files!", _plusHintViewModel.ThankYouText);
        }

        [Test]
        public void FinishStepCommand_OnExecute_CallsFinishesInteractionAndThrowsAbortWorkflowException()
        {
            var wasCalled = false;
            _plusHintViewModel.StepFinished += (sender, args) => wasCalled = true;

            Assert.Throws<AbortWorkflowException>(() => _plusHintViewModel.FinishStepCommand.Execute(null));
            Assert.IsTrue(wasCalled, "StepFinished was not invoked");
        }

        [Test]
        public void PlusHintCommand_Execute_ExcutesPlusHintWebsiteCommandAndCallsFinishesInteraction_DoesNotThrowAbortWorkflowException()
        {
            var plusHintWebsiteCommand = Substitute.For<ICommand>();
            _commandLocator.GetInitializedCommand<UrlOpenCommand, string>(Urls.PlusHintLink).Returns(plusHintWebsiteCommand);
            InitViewModel();
            var wasCalled = false;
            _plusHintViewModel.StepFinished += (sender, args) => wasCalled = true;

            Assert.DoesNotThrow(() => _plusHintViewModel.PlusButtonCommand.Execute(null), "May not throw AbortWorkflowException!");

            plusHintWebsiteCommand.Received().Execute(Arg.Any<object>());
            Assert.IsTrue(wasCalled, "StepFinished was not invoked");
        }

        [Test]
        public void DesigntimePlusHintViewModelTest()
        {
            var designTimePlusHintViewModel = new DesignTimePlusHintViewModel();

            Assert.NotNull(designTimePlusHintViewModel);
        }
    }
}
