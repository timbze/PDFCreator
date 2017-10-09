using NUnit.Framework;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Overlay.Password;
using pdfforge.PDFCreator.UnitTest.UnitTestHelper;
using pdfforge.PDFCreator.Utilities.Threading;
using Translatable;

namespace Presentation.UnitTest.Overlay.Password
{
    [TestFixture]
    public class PasswordOverlayViewModelTest
    {
        private PasswordOverlayViewModel CreateViewModel()
        {
            return new PasswordOverlayViewModel(new TranslationUpdater(new TranslationFactory(), new ThreadManager()));
        }

        [Test]
        public void CanRemovePassword_WithRemovePasswordEnabled_IsTrue()
        {
            var viewModel = CreateViewModel();
            var interaction = new PasswordOverlayInteraction(PasswordMiddleButton.Remove, "", "");
            var helper = new InteractionHelper<PasswordOverlayInteraction>(viewModel, interaction);

            Assert.IsTrue(viewModel.CanRemovePassword);
        }

        [Test]
        public void CanRemovePassword_WithSkipEnabled_IsFalse()
        {
            var viewModel = CreateViewModel();
            var interaction = new PasswordOverlayInteraction(PasswordMiddleButton.Skip, "", "");
            var helper = new InteractionHelper<PasswordOverlayInteraction>(viewModel, interaction);

            Assert.IsFalse(viewModel.CanRemovePassword);
        }

        [Test]
        public void CanSkip_WithRemovePasswordEnabled_IsFalse()
        {
            var viewModel = CreateViewModel();
            var interaction = new PasswordOverlayInteraction(PasswordMiddleButton.Remove, "", "");
            var helper = new InteractionHelper<PasswordOverlayInteraction>(viewModel, interaction);

            Assert.IsFalse(viewModel.CanSkip);
        }

        [Test]
        public void CanSkip_WithSkipEnabled_IsTrue()
        {
            var viewModel = CreateViewModel();
            var interaction = new PasswordOverlayInteraction(PasswordMiddleButton.Skip, "", "");
            var helper = new InteractionHelper<PasswordOverlayInteraction>(viewModel, interaction);

            Assert.IsTrue(viewModel.CanSkip);
        }

        [Test]
        public void FtpPassword_OnSet_WritesToInteractionAndCallCanExecuteChanged()
        {
            var interacton = new PasswordOverlayInteraction(PasswordMiddleButton.Skip, "", "");
            var viewModel = CreateViewModel();
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
            var viewModel = CreateViewModel();
            Assert.IsFalse(viewModel.OkCommand.CanExecute(null));
        }

        [Test]
        public void OkCommand_CanExecute_WithPassword_IsTrue()
        {
            var viewModel = CreateViewModel();
            viewModel.SetInteraction(new PasswordOverlayInteraction(PasswordMiddleButton.Remove, "", "") { Password = "myPassword" });
            Assert.IsTrue(viewModel.OkCommand.CanExecute(null));
        }

        [Test]
        public void OkCommand_OnExecute_CompletesInteraction()
        {
            var viewModel = CreateViewModel();
            var interaction = new PasswordOverlayInteraction(PasswordMiddleButton.Skip, "", "");
            var helper = new InteractionHelper<PasswordOverlayInteraction>(viewModel, interaction);

            viewModel.OkCommand.Execute(null);

            Assert.AreEqual(PasswordResult.StorePassword, interaction.Result);
            Assert.IsTrue(helper.InteractionIsFinished);
        }

        [Test]
        public void OnInteractionSet_SetsPasswordsInView()
        {
            var viewModel = CreateViewModel();
            var canExecuteChanged = false;
            viewModel.OkCommand.CanExecuteChanged += (sender, args) => canExecuteChanged = true;
            var interaction = new PasswordOverlayInteraction(PasswordMiddleButton.Skip, "", "") { Password = "thePassword" };

            var actionWasCalled = false;
            viewModel.SetPasswordAction = x => actionWasCalled = true;

            viewModel.SetInteraction(interaction);
            Assert.IsTrue(actionWasCalled);
            Assert.IsTrue(canExecuteChanged);
        }

        [Test]
        public void RemoveCommand_OnExecute_FinishesInteraction()
        {
            var viewModel = CreateViewModel();
            var interaction = new PasswordOverlayInteraction(PasswordMiddleButton.Remove, "", "") { Password = "MyPassword" };
            var helper = new InteractionHelper<PasswordOverlayInteraction>(viewModel, interaction);

            viewModel.RemoveCommand.Execute(null);

            Assert.AreEqual(PasswordResult.RemovePassword, interaction.Result);
            Assert.AreEqual("", interaction.Password);
            Assert.IsTrue(helper.InteractionIsFinished);
        }

        [Test]
        public void SkipCommand_OnExecute_FinishesInteraction()
        {
            var viewModel = CreateViewModel();
            var interaction = new PasswordOverlayInteraction(PasswordMiddleButton.Skip, "", "");
            var helper = new InteractionHelper<PasswordOverlayInteraction>(viewModel, interaction);
            viewModel.Password = "myPassword";

            viewModel.SkipCommand.Execute(null);

            Assert.AreEqual(PasswordResult.Skip, interaction.Result);
            Assert.AreEqual("", interaction.Password);
            Assert.IsTrue(helper.InteractionIsFinished);
        }

        [Test]
        public void CancelCommand_OnExecute_FinishesInteraction()
        {
            var viewModel = CreateViewModel();
            var interaction = new PasswordOverlayInteraction(PasswordMiddleButton.Remove, "", "");
            var helper = new InteractionHelper<PasswordOverlayInteraction>(viewModel, interaction);
            viewModel.Password = "myPassword";

            viewModel.CancelCommand.Execute(null);

            Assert.AreEqual(PasswordResult.Cancel, interaction.Result);
            Assert.AreEqual("", interaction.Password);
            Assert.IsTrue(helper.InteractionIsFinished);
        }

        [Test]
        public void PasswordOverlayTranslation_FormatInvalidPasswordMessage_SetsTextWithActionName()
        {
            var viewModel = CreateViewModel();
            var pwOVtr = new PasswordOverlayTranslation();
            var interaction = new PasswordOverlayInteraction(PasswordMiddleButton.Remove, "", pwOVtr.FormatInvalidPasswordMessage("ActionName"), false);
            var sometext = "Invalid password for 'ActionName'";

            Assert.AreEqual(sometext, interaction.PasswordDescription);
        }
    }
}
