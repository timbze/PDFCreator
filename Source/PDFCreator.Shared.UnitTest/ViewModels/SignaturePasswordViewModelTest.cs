using System;
using NUnit.Framework;
using pdfforge.PDFCreator.Shared.ViewModels;
using Rhino.Mocks;

namespace PDFCreator.Shared.Test.ViewModels
{
    [TestFixture]
    public class SignaturePasswordViewModelTest
    {
        private const string WrongPassword = "WrongPassword";
        private const string CorrectPassword = "CorrectPassword";

        private ISignaturePasswordCheck CreatePasswordCheckStub()
        {
            var pwd = MockRepository.GenerateStub<ISignaturePasswordCheck>();

            pwd.Stub(x => x.IsValidPassword(CorrectPassword)).Return(true);
            pwd.Stub(x => x.IsValidPassword(WrongPassword)).Return(false);

            return pwd;
        }

        [Test]
        public void EmptyModel_PasswordNotValid()
        {
            var model = new SignaturePasswordViewModel();

            Assert.IsFalse(model.StorePasswordCommand.CanExecute(null));
        }

        [Test]
        public void EmptyModel_ResultIsCancel()
        {
            var model = new SignaturePasswordViewModel();

            Assert.AreEqual(SignaturePasswordResult.Cancel, model.Result);
        }

        [Test]
        public void PasswordCheck_WithWrongPassword_PasswordNotValid()
        {
            var model = new SignaturePasswordViewModel(CreatePasswordCheckStub());

            Assert.IsFalse(model.StorePasswordCommand.CanExecute(null));
        }

        [Test]
        public void PasswordCheck_WithCorrectPassword_PasswordIsValid()
        {
            var model = new SignaturePasswordViewModel(CreatePasswordCheckStub());

            model.Password = CorrectPassword;

            Assert.IsTrue(model.StorePasswordCommand.CanExecute(null));
        }

        [Test]
        public void PasswordCheckWithWrongPassword_ExecuteStoreCommand_ThrowsInvalidOperationException()
        {
            var model = new SignaturePasswordViewModel(CreatePasswordCheckStub());

            model.Password = WrongPassword;

            Assert.Throws<InvalidOperationException>(() => model.StorePasswordCommand.Execute(null));
        }

        [Test]
        public void PasswordCheckWithCorrectPassword_AfterExecuteStoreCommand_ResultIsStorePassword()
        {
            var model = new SignaturePasswordViewModel(CreatePasswordCheckStub());

            model.Password = CorrectPassword;

            model.StorePasswordCommand.Execute(null);

            Assert.AreEqual(SignaturePasswordResult.StorePassword, model.Result);
        }

        [Test]
        public void PasswordCheck_AfterExecuteSkipCommand_ResultIsSkip()
        {
            var model = new SignaturePasswordViewModel();

            model.SkipCommand.Execute(null);

            Assert.AreEqual(SignaturePasswordResult.Skip, model.Result);
        }

        [Test]
        public void PasswordCheck_AfterExecuteRemoveCommand_ResultIsRemove()
        {
            var model = new SignaturePasswordViewModel();

            model.RemovePasswordCommand.Execute(null);

            Assert.AreEqual(SignaturePasswordResult.RemovePassword, model.Result);
        }

        [Test]
        public void ExecuteStore_PasswordIsSettedPassword()
        {
            var model = new SignaturePasswordViewModel(CreatePasswordCheckStub());
            model.Password = CorrectPassword;

            model.StorePasswordCommand.Execute(null);

            Assert.AreEqual(CorrectPassword, model.Password, "Password is not setted password after storing.");
        }

        [Test]
        public void ExecuteSkip_PasswordIsEmpty()
        {
            var model = new SignaturePasswordViewModel();
            model.Password = "1234";
            
            model.SkipCommand.Execute(null);

            Assert.IsEmpty(model.Password, "Password is not empty after skip.");           
        }

        [Test]
        public void ExecuteRemove_PasswordIsEmpty()
        {
            var model = new SignaturePasswordViewModel();
            model.Password = "1234";

            model.RemovePasswordCommand.Execute(null);

            Assert.IsEmpty(model.Password, "Password is not empty after remove.");
        }
    }

    [TestFixture]
    public class SignaturePasswordEventTests
    {
        private bool _eventWasRaised;

        [SetUp]
        public void Setup()
        {
            _eventWasRaised = false;
        }

        [Test]
        public void SettingPassword_RaisesCanExecuteChanged()
        {
            var model = new SignaturePasswordViewModel();
            model.StorePasswordCommand.CanExecuteChanged += delegate { _eventWasRaised = true; };
            model.Password = "NewPassword";

            Assert.IsTrue(_eventWasRaised);
        }

        [Test]
        public void SettingPassword_StoreCommand_CallsCloseViewAction()
        {
            var pwd = MockRepository.GenerateStub<ISignaturePasswordCheck>();
            pwd.Stub(x => x.IsValidPassword("NewPassword")).Return(true);

            var model = new SignaturePasswordViewModel(pwd);
            model.CloseViewAction = delegate { _eventWasRaised = true; };
            model.Password = "NewPassword";

            model.StorePasswordCommand.Execute(null);

            Assert.IsTrue(_eventWasRaised);
        }

        [Test]
        public void SettingPassword_SkipCommand_CallsCloseViewAction()
        {
            var model = new SignaturePasswordViewModel();
            model.CloseViewAction = delegate { _eventWasRaised = true; };

            model.SkipCommand.Execute(null);

            Assert.IsTrue(_eventWasRaised);
        }

        [Test]
        public void SettingPassword_RemovePasswordCommand_CallsCloseViewAction()
        {
            var model = new SignaturePasswordViewModel();
            model.CloseViewAction = delegate { _eventWasRaised = true; };

            model.RemovePasswordCommand.Execute(null);

            Assert.IsTrue(_eventWasRaised);
        }
    }
}