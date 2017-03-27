using NUnit.Framework;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.ViewModels.DialogViewModels;
using pdfforge.PDFCreator.UI.ViewModels.DialogViewModels.Translations;
using pdfforge.PDFCreator.UnitTest.UnitTestHelper;

namespace pdfforge.PDFCreator.UnitTest.UI.ViewModels.DialogViewModels
{
    [TestFixture]
    public class PasswordViewModelTest
    {
        [Test]
        public void CanRemovePassword_WithRemovePasswordEnabled_IsTrue()
        {
            var viewModel = new PasswordViewModel(new PasswordWindowTranslation());
            var interaction = new PasswordInteraction(PasswordMiddleButton.Remove, "", "");
            var helper = new InteractionHelper<PasswordInteraction>(viewModel, interaction);

            Assert.IsTrue(viewModel.CanRemovePassword);
        }

        [Test]
        public void CanRemovePassword_WithSkipEnabled_IsFalse()
        {
            var viewModel = new PasswordViewModel(new PasswordWindowTranslation());
            var interaction = new PasswordInteraction(PasswordMiddleButton.Skip, "", "");
            var helper = new InteractionHelper<PasswordInteraction>(viewModel, interaction);

            Assert.IsFalse(viewModel.CanRemovePassword);
        }

        [Test]
        public void CanSkip_WithRemovePasswordEnabled_IsFalse()
        {
            var viewModel = new PasswordViewModel(new PasswordWindowTranslation());
            var interaction = new PasswordInteraction(PasswordMiddleButton.Remove, "", "");
            var helper = new InteractionHelper<PasswordInteraction>(viewModel, interaction);

            Assert.IsFalse(viewModel.CanSkip);
        }

        [Test]
        public void CanSkip_WithSkipEnabled_IsTrue()
        {
            var viewModel = new PasswordViewModel(new PasswordWindowTranslation());
            var interaction = new PasswordInteraction(PasswordMiddleButton.Skip, "", "");
            var helper = new InteractionHelper<PasswordInteraction>(viewModel, interaction);

            Assert.IsTrue(viewModel.CanSkip);
        }

        [Test]
        public void FtpPassword_OnSet_WritesToInteractionAndCallCanExecuteChanged()
        {
            var interacton = new PasswordInteraction(PasswordMiddleButton.Skip, "", "");
            var viewModel = new PasswordViewModel(new PasswordWindowTranslation());
            viewModel.SetInteraction(interacton);

            var canExecuteChanged = false;
            viewModel.OkCommand.CanExecuteChanged += (sender, args) => canExecuteChanged = true;

            viewModel.Password = "MyPassword";

            Assert.AreEqual("MyPassword", interacton.Password);
            Assert.IsTrue(canExecuteChanged);
        }

        [Test]
        public void OkCommand_CanExecute_WithoutInteraction_IsFalse()
        {
            var viewModel = new PasswordViewModel(new PasswordWindowTranslation());
            Assert.IsFalse(viewModel.OkCommand.CanExecute(null));
        }

        [Test]
        public void OkCommand_CanExecute_WithPassword_IsTrue()
        {
            var viewModel = new PasswordViewModel(new PasswordWindowTranslation());
            viewModel.SetInteraction(new PasswordInteraction(PasswordMiddleButton.Remove, "", "") {Password = "myPassword"});
            Assert.IsTrue(viewModel.OkCommand.CanExecute(null));
        }

        [Test]
        public void OkCommand_OnExecute_CompletesInteraction()
        {
            var viewModel = new PasswordViewModel(new PasswordWindowTranslation());
            var interaction = new PasswordInteraction(PasswordMiddleButton.Skip, "", "");
            var helper = new InteractionHelper<PasswordInteraction>(viewModel, interaction);

            viewModel.OkCommand.Execute(null);

            Assert.AreEqual(PasswordResult.StorePassword, interaction.Result);
            Assert.IsTrue(helper.InteractionIsFinished);
        }

        [Test]
        public void OnInteractionSet_SetsPasswordsInView()
        {
            var viewModel = new PasswordViewModel(new PasswordWindowTranslation());
            var canExecuteChanged = false;
            viewModel.OkCommand.CanExecuteChanged += (sender, args) => canExecuteChanged = true;
            var interaction = new PasswordInteraction(PasswordMiddleButton.Skip, "", "") {Password = "thePassword"};

            var actionWasCalled = false;
            viewModel.SetPasswordAction = x => actionWasCalled = true;

            viewModel.SetInteraction(interaction);
            Assert.IsTrue(actionWasCalled);
            Assert.IsTrue(canExecuteChanged);
        }

        [Test]
        public void RemoveCommand_OnExecute_FinishesInteraction()
        {
            var viewModel = new PasswordViewModel(new PasswordWindowTranslation());
            var interaction = new PasswordInteraction(PasswordMiddleButton.Remove, "", "") {Password = "MyPassword"};
            var helper = new InteractionHelper<PasswordInteraction>(viewModel, interaction);

            viewModel.RemoveCommand.Execute(null);

            Assert.AreEqual(PasswordResult.RemovePassword, interaction.Result);
            Assert.AreEqual("", interaction.Password);
            Assert.IsTrue(helper.InteractionIsFinished);
        }

        [Test]
        public void SkipCommand_OnExecute_FinishesInteraction()
        {
            var viewModel = new PasswordViewModel(new PasswordWindowTranslation());
            var interaction = new PasswordInteraction(PasswordMiddleButton.Skip, "", "");
            var helper = new InteractionHelper<PasswordInteraction>(viewModel, interaction);
            viewModel.Password = "myPassword";

            viewModel.SkipCommand.Execute(null);

            Assert.AreEqual(PasswordResult.Skip, interaction.Result);
            Assert.AreEqual("", interaction.Password);
            Assert.IsTrue(helper.InteractionIsFinished);
        }
    }
}