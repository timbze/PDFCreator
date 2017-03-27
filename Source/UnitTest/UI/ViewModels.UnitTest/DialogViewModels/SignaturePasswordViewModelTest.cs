using System;
using System.Collections.Generic;
using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.ViewModels.DialogViewModels;
using pdfforge.PDFCreator.UI.ViewModels.DialogViewModels.Translations;

namespace pdfforge.PDFCreator.UnitTest.UI.ViewModels.DialogViewModels
{
    [TestFixture]
    public class SignaturePasswordViewModelTest
    {
        private const string WrongPassword = "WrongPassword";
        private const string CorrectPassword = "CorrectPassword";

        private SignaturePasswordViewModel BuildViewModel(string expectedPassword = CorrectPassword)
        {
            var signaturePasswordCheck = Substitute.For<ISignaturePasswordCheck>();

            if (expectedPassword != null)
                signaturePasswordCheck.IsValidPassword(Arg.Any<string>(), expectedPassword).Returns(true);

            var viewModel = new SignaturePasswordViewModel(signaturePasswordCheck, new SignaturePasswordWindowTranslation());
            var interaction = new SignaturePasswordInteraction(PasswordMiddleButton.Skip, "");

            viewModel.SetInteraction(interaction);
            viewModel.FinishInteraction = () => { };

            return viewModel;
        }

        [Test]
        public void EmptyModel_PasswordNotValid()
        {
            var model = BuildViewModel();

            Assert.IsFalse(model.StorePasswordCommand.CanExecute(null));
        }

        [Test]
        public void EmptyModel_ResultIsCancel()
        {
            var model = BuildViewModel();

            Assert.AreEqual(PasswordResult.Cancel, model.Interaction.Result);
        }

        [Test]
        public void ExecuteRemove_PasswordIsEmpty()
        {
            var model = BuildViewModel();
            model.Password = "1234";

            model.RemovePasswordCommand.Execute(null);

            Assert.IsEmpty(model.Password, "Password is not empty after remove.");
        }

        [Test]
        public void ExecuteSkip_PasswordIsEmpty()
        {
            var model = BuildViewModel();
            model.Password = "1234";

            model.SkipCommand.Execute(null);

            Assert.IsEmpty(model.Password, "Password is not empty after skip.");
        }

        [Test]
        public void ExecuteStore_PasswordIsSettedPassword()
        {
            var model = BuildViewModel();
            model.Password = CorrectPassword;

            model.StorePasswordCommand.Execute(null);

            Assert.AreEqual(CorrectPassword, model.Password, "Password is not setted password after storing.");
        }

        [Test]
        public void PasswordCheck_AfterExecuteRemoveCommand_ResultIsRemove()
        {
            var model = BuildViewModel();

            model.RemovePasswordCommand.Execute(null);

            Assert.AreEqual(PasswordResult.RemovePassword, model.Interaction.Result);
        }

        [Test]
        public void PasswordCheck_AfterExecuteSkipCommand_ResultIsSkip()
        {
            var model = BuildViewModel();

            model.SkipCommand.Execute(null);

            Assert.AreEqual(PasswordResult.Skip, model.Interaction.Result);
        }

        [Test]
        public void PasswordCheck_WithCorrectPassword_PasswordIsValid()
        {
            var model = BuildViewModel();

            model.Password = CorrectPassword;

            Assert.IsTrue(model.StorePasswordCommand.CanExecute(null));
        }

        [Test]
        public void PasswordCheck_WithWrongPassword_PasswordNotValid()
        {
            var model = BuildViewModel();

            Assert.IsFalse(model.StorePasswordCommand.CanExecute(null));
        }

        [Test]
        public void PasswordCheckWithCorrectPassword_AfterExecuteStoreCommand_ResultIsStorePassword()
        {
            var model = BuildViewModel();

            model.Password = CorrectPassword;

            model.StorePasswordCommand.Execute(null);

            Assert.AreEqual(PasswordResult.StorePassword, model.Interaction.Result);
        }

        [Test]
        public void PasswordCheckWithWrongPassword_ExecuteStoreCommand_ThrowsInvalidOperationException()
        {
            var model = BuildViewModel(CorrectPassword);

            model.Password = WrongPassword;

            Assert.Throws<InvalidOperationException>(() => model.StorePasswordCommand.Execute(null));
        }
    }

    [TestFixture]
    public class SignaturePasswordEventTests
    {
        [SetUp]
        public void Setup()
        {
            _eventWasRaised = false;
        }

        private bool _eventWasRaised;

        private SignaturePasswordViewModel BuildViewModel(string expectedPassword = null)
        {
            var signaturePasswordCheck = Substitute.For<ISignaturePasswordCheck>();

            if (expectedPassword != null)
                signaturePasswordCheck.IsValidPassword(Arg.Any<string>(), expectedPassword).Returns(true);

            var viewModel = new SignaturePasswordViewModel(signaturePasswordCheck, new SignaturePasswordWindowTranslation());
            var interaction = new SignaturePasswordInteraction(PasswordMiddleButton.Skip, "");

            viewModel.SetInteraction(interaction);

            return viewModel;
        }

        [Test]
        public void SetInteraction_RaisesPropertyChanged()
        {
            var propertyChangedCalls = new List<string>();

            var viewModel = new SignaturePasswordViewModel(null, new SignaturePasswordWindowTranslation());
            var interaction = new SignaturePasswordInteraction(PasswordMiddleButton.Skip, "");

            viewModel.PropertyChanged += (sender, args) => propertyChangedCalls.Add(args.PropertyName);

            viewModel.SetInteraction(interaction);

            var expectedProperties = new[] {nameof(viewModel.Interaction), nameof(viewModel.Password), nameof(viewModel.CanRemovePassword), nameof(viewModel.CanSkip)};

            CollectionAssert.AreEquivalent(expectedProperties, propertyChangedCalls);
        }

        [Test]
        public void SettingPassword_RaisesCanExecuteChanged()
        {
            var model = BuildViewModel();
            model.StorePasswordCommand.CanExecuteChanged += delegate { _eventWasRaised = true; };
            model.Password = "NewPassword";

            Assert.IsTrue(_eventWasRaised);
        }

        [Test]
        public void SettingPassword_RemovePasswordCommand_CallsCloseViewAction()
        {
            var model = BuildViewModel();
            var interaction = new SignaturePasswordInteraction(PasswordMiddleButton.Skip, "");
            model.SetInteraction(interaction);
            model.FinishInteraction = () => _eventWasRaised = true;

            model.RemovePasswordCommand.Execute(null);

            Assert.IsTrue(_eventWasRaised);
        }

        [Test]
        public void SettingPassword_SkipCommand_CallsCloseViewAction()
        {
            var model = BuildViewModel();
            var interaction = new SignaturePasswordInteraction(PasswordMiddleButton.Skip, "");
            model.SetInteraction(interaction);
            model.FinishInteraction = () => _eventWasRaised = true;

            model.SkipCommand.Execute(null);

            Assert.IsTrue(_eventWasRaised);
        }

        [Test]
        public void SettingPassword_StoreCommand_CallsCloseViewAction()
        {
            var model = BuildViewModel("NewPassword");
            var interaction = new SignaturePasswordInteraction(PasswordMiddleButton.Skip, "");
            model.SetInteraction(interaction);
            model.FinishInteraction = () => _eventWasRaised = true;

            model.Password = "NewPassword";

            model.StorePasswordCommand.Execute(null);

            Assert.IsTrue(_eventWasRaised);
        }
    }
}