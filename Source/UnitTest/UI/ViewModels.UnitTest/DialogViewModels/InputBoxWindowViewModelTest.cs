using NUnit.Framework;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.ViewModels.DialogViewModels;
using pdfforge.PDFCreator.UI.ViewModels.DialogViewModels.Translations;
using pdfforge.PDFCreator.UnitTest.UnitTestHelper;

namespace pdfforge.PDFCreator.UnitTest.UI.ViewModels.DialogViewModels
{
    [TestFixture]
    public class InputBoxWindowViewModelTest
    {
        [SetUp]
        public void Setup()
        {
            _viewModel = new InputBoxWindowViewModel(new InputBoxWindowTranslation());
            _interaction = new InputInteraction("Some Title", "Please answer this question:");
            _interactionHelper = new InteractionHelper<InputInteraction>(_viewModel, _interaction);
        }

        private InputInteraction _interaction;
        private InputBoxWindowViewModel _viewModel;
        private InteractionHelper<InputInteraction> _interactionHelper;

        [Test]
        public void CancelCommand_WhenExecuted_CallsFinishInteraction()
        {
            _viewModel.CancelDialogCommand.Execute(null);
            Assert.IsTrue(_interactionHelper.InteractionIsFinished);
        }

        [Test]
        public void CancelCommand_WhenExecuted_SetsSuccessToFalse()
        {
            _viewModel.CancelDialogCommand.Execute(null);
            Assert.IsFalse(_interaction.Success);
        }

        [Test]
        public void ConfirmCommand_WhenExecuted_CallsFinishInteraction()
        {
            _viewModel.ConfirmDialogCommand.Execute(null);
            Assert.IsTrue(_interactionHelper.InteractionIsFinished);
        }

        [Test]
        public void ConfirmCommand_WhenExecuted_SetsSuccessToTrue()
        {
            _viewModel.ConfirmDialogCommand.Execute(null);
            Assert.IsTrue(_interaction.Success);
        }

        [Test]
        public void ConfirmCommand_WithoutValidation_IsAlwaysExecutable()
        {
            Assert.IsTrue(_viewModel.ConfirmDialogCommand.CanExecute(null));
        }

        [Test]
        public void ConfirmCommandWithValidation_WithInvalidString_NotExecutable()
        {
            _interaction.IsValidInput = s => new InputValidation(!string.IsNullOrWhiteSpace(s), "INVALID");

            Assert.IsFalse(_viewModel.ConfirmDialogCommand.CanExecute(null));
            Assert.AreEqual("INVALID", _viewModel.ValidationMessage);
        }

        [Test]
        public void ConfirmCommandWithValidation_WithValidString_IsExecutable()
        {
            _interaction.IsValidInput = s => new InputValidation(!string.IsNullOrWhiteSpace(s));
            _interaction.InputText = "some string";

            Assert.IsTrue(_viewModel.ConfirmDialogCommand.CanExecute(null));
            Assert.IsTrue(string.IsNullOrWhiteSpace(_viewModel.ValidationMessage));
        }

        [Test]
        public void TextChangedCommand_WhenCalled_ChecksString()
        {
            var canExecutedChangedWasRaised = false;
            _interaction.IsValidInput = s => new InputValidation(false, "INVALID");
            _viewModel.ConfirmDialogCommand.CanExecuteChanged += (sender, args) => canExecutedChangedWasRaised = true;

            _viewModel.TextChangedCommand.Execute(null);

            Assert.IsTrue(canExecutedChangedWasRaised);
        }

        [Test]
        public void TextChangedCommandWithoutValidation_WhenCalled_DoesNothing()
        {
            _viewModel.TextChangedCommand.Execute(null);
            Assert.IsTrue(string.IsNullOrWhiteSpace(_viewModel.ValidationMessage));
        }
    }
}