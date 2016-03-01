using NUnit.Framework;
using pdfforge.PDFCreator.Shared.ViewModels;

namespace PDFCreator.Shared.Test.ViewModels
{
    [TestFixture]
    class EncryptionPasswordViewModelTest
    {
        private bool? _closeDialogResult;

        private void CloseAction(bool? result)
        {
            _closeDialogResult = result;
        }

        [Test]
        public void Skip_ResponseIsInitializedWithCancel()
        {
            var vm = new EncryptionPasswordViewModelwithSkip(true,true);
            Assert.AreEqual(vm.Response, EncryptionPasswordResponse.Cancel);
        }

        [Test]
        public void Skip_AskOwnerPW_SetOwnerPassword_AskUserPassword_SetUserPassword_OkButtonIsExecutable_SkipAndRemoveAreExecutable()
        {
            var vm = new EncryptionPasswordViewModelwithSkip(true, true);
            vm.OwnerPassword = "ownerpw";
            vm.UserPassword = "userpw";
            Assert.IsTrue(vm.OkCommand.IsExecutable, "OK Command is not executable");
            Assert.IsTrue(vm.RemoveCommand.IsExecutable, "Skip Command is not executable");
            Assert.IsTrue(vm.SkipCommand.IsExecutable, "SkipCommand is not executable");
        }

        [Test]
        public void Skip_AskOwnerPW_DoNotSetOwnerPassword_AskUserPassword_SetUserPassword_OkButtonIsNotExecutable_SkipAndRemoveAreExecutable()
        {
            var vm = new EncryptionPasswordViewModelwithSkip(true, true);
            vm.OwnerPassword = "";
            vm.UserPassword = "userpw";
            Assert.IsFalse(vm.OkCommand.IsExecutable, "OK Command should not be executable");
            Assert.IsTrue(vm.RemoveCommand.IsExecutable, "Skip Command is not executable");
            Assert.IsTrue(vm.SkipCommand.IsExecutable, "SkipCommand is not executable");
        }

        [Test]
        public void Skip_AskOwnerPW_SetOwnerPassword_AskUserPassword_DoNotSetUserPassword_OkButtonIsNotExecutable_SkipAndRemoveAreExecutable()
        {
            var vm = new EncryptionPasswordViewModelwithSkip(true, true);
            vm.OwnerPassword = "ownerpw";
            vm.UserPassword = "";
            Assert.IsFalse(vm.OkCommand.IsExecutable, "OK Command should not be executable");
            Assert.IsTrue(vm.RemoveCommand.IsExecutable, "Skip Command is not executable");
            Assert.IsTrue(vm.SkipCommand.IsExecutable, "SkipCommand is not executable");
        }

        [Test]
        public void Skip_DoNotAskOwnerPW_DoNotSetOwnerPassword_AskUserPassword_SetUserPassword_OkButtonIsExecutable_SkipAndRemoveAreExecutable()
        {
            var vm = new EncryptionPasswordViewModelwithSkip(false, true);
            vm.OwnerPassword = "";
            vm.UserPassword = "userpw";
            Assert.IsTrue(vm.OkCommand.IsExecutable, "OK Command is not executable");
            Assert.IsTrue(vm.RemoveCommand.IsExecutable, "Skip Command is not executable");
            Assert.IsTrue(vm.SkipCommand.IsExecutable, "SkipCommand is not executable");
        }

        [Test]
        public void Skip_DoNotAskOwnerPW_DoNotSetOwnerPassword_DoNotAskUserPassword_DoNotSetUserPassword_OkButtonIsExecutable_SkipAndRemoveAreExecutable()
        {
            var vm = new EncryptionPasswordViewModelwithSkip(true, false);
            vm.OwnerPassword = "ownerpw";
            vm.UserPassword = "";
            Assert.IsTrue(vm.OkCommand.IsExecutable, "OK Command is not executable");
            Assert.IsTrue(vm.RemoveCommand.IsExecutable, "Skip Command is not executable");
            Assert.IsTrue(vm.SkipCommand.IsExecutable, "SkipCommand is not executable");
        }

        [Test]
        public void Skip_DoNotAskOwnerPW_SetOwnerPassword_DoNotAskUserPassword_DoNotSetUserPassword_OkButtonIsExecutable_SkipAndRemoveAreExecutable()
        {
            var vm = new EncryptionPasswordViewModelwithSkip(false, false);
            vm.OwnerPassword = "";
            vm.UserPassword = "";
            Assert.IsTrue(vm.OkCommand.IsExecutable, "OK Command is not executable");
            Assert.IsTrue(vm.RemoveCommand.IsExecutable, "Skip Command is not executable");
            Assert.IsTrue(vm.SkipCommand.IsExecutable, "SkipCommand is not executable");
        }

        [Test]
        public void Skip_CheckResponses()
        {
            var vm = new EncryptionPasswordViewModelwithSkip(true, true);
            vm.OwnerPassword = "ownerpw";
            vm.UserPassword = "userpw";

            vm.OkCommand.Execute(null);
            Assert.AreEqual(EncryptionPasswordResponse.OK, vm.Response, "Wrong Response for OkCommand");
            vm.RemoveCommand.Execute(null);
            Assert.AreEqual(EncryptionPasswordResponse.Remove, vm.Response, "Wrong Response for RemoveCommand");
            vm.SkipCommand.Execute(null);
            Assert.AreEqual(EncryptionPasswordResponse.Skip, vm.Response, "Wrong Response for SkipCommand");
        }

        [Test]
        public void Skip_CheckDialogResultInCloseActionForOkCommand()
        {
            var vm = new EncryptionPasswordViewModelwithSkip(true, true);
            vm.CloseViewAction = CloseAction;
            vm.OwnerPassword = "ownerpw";
            vm.UserPassword = "userpw";

            vm.OkCommand.Execute(null);
            Assert.AreEqual(_closeDialogResult, true, "Wrong DialogResult for OK command");
        }

        [Test]
        public void Skip_CheckDialogResultInCloseActionForSkipCommand()
        {
            var vm = new EncryptionPasswordViewModelwithSkip(true, true);
            vm.CloseViewAction = CloseAction;
            vm.OwnerPassword = "ownerpw";
            vm.UserPassword = "userpw";

            vm.SkipCommand.Execute(null);
            Assert.AreEqual(_closeDialogResult, true, "Wrong DialogResult for Skip command");
        }

        [Test]
        public void Skip_CheckDialogResultInCloseActionForRemoveCommand()
        {
            var vm = new EncryptionPasswordViewModelwithSkip(true, true);
            vm.CloseViewAction = CloseAction;
            vm.OwnerPassword = "ownerpw";
            vm.UserPassword = "userpw";

            vm.RemoveCommand.Execute(null);
            Assert.AreEqual(_closeDialogResult, true, "Wrong DialogResult for Remove command");
        }

        [Test]
        public void Skip_ExecuteOk_PasswordsAreTheSettedPasswords()
        {
            var vm = new EncryptionPasswordViewModelwithSkip(true, true);
            vm.OwnerPassword = "ownerpw";
            vm.UserPassword = "userpw";

            vm.OkCommand.Execute(null);

            Assert.AreEqual("ownerpw", vm.OwnerPassword, "Owner password is not setted password.");
            Assert.AreEqual("userpw", vm.UserPassword, "User password is not setted password.");
        }

        [Test]
        public void Skip_ExecuteSkip_PasswordsAreEmpty()
        {
            var vm = new EncryptionPasswordViewModelwithSkip(true, true);
            vm.OwnerPassword = "ownerpw";
            vm.UserPassword = "userpw";

            vm.SkipCommand.Execute(null);

            Assert.IsEmpty(vm.OwnerPassword, "Owner password is not empty after skipping.");
            Assert.IsEmpty(vm.UserPassword, "User password is not empty after skipping.");
        }

        [Test]
        public void Remove_InitTest()
        {
            var vm = new EncryptionPasswordViewModelwithRemove();
            Assert.AreEqual(vm.Response, EncryptionPasswordResponse.Cancel);
        }

        [Test]
        public void Remove_CanExecuteAllCommands()
        {
            var vm = new EncryptionPasswordViewModelwithRemove();
            Assert.IsTrue(vm.OkCommand.IsExecutable, "OK Command ist not executable");
            Assert.IsTrue(vm.RemoveCommand.IsExecutable, "Skip Command is not executable");
            Assert.IsTrue(vm.SkipCommand.IsExecutable, "SkipCommand is not executable");
        }

        [Test]
        public void Remove_WithAskUserPassword_RequiresUserPassword()
        {
            var vm = new EncryptionPasswordViewModelwithRemove(true);

            Assert.IsTrue(vm.AskUserPassword);
        }

        [Test]
        public void Remove_WithoutAskUserPassword_DoesNotRequireUserPassword()
        {
            var vm = new EncryptionPasswordViewModelwithRemove(false);

            Assert.IsFalse(vm.AskUserPassword);
        }

        [Test]
        public void Remove_CheckResponses()
        {
            var vm = new EncryptionPasswordViewModelwithRemove();

            vm.OkCommand.Execute(null);
            Assert.AreEqual(EncryptionPasswordResponse.OK, vm.Response, "Wrong Response for OkCommand");
            vm.RemoveCommand.Execute(null);
            Assert.AreEqual(EncryptionPasswordResponse.Remove, vm.Response, "Wrong Response for RemoveCommand");
            vm.SkipCommand.Execute(null);
            Assert.AreEqual(EncryptionPasswordResponse.Skip, vm.Response, "Wrong Response for SkipCommand");
        }

        [Test]
        public void Remove_CheckDialogResultInCloseActionForOkCommand()
        {
            var vm = new EncryptionPasswordViewModelwithRemove();
            vm.CloseViewAction = CloseAction;

            vm.OkCommand.Execute(null);
            Assert.AreEqual(true, _closeDialogResult, "Wrong DialogResult for OkCommand");
        }

        [Test]
        public void Remove_CheckDialogResultInCloseActionForSkipCommand()
        {
            var vm = new EncryptionPasswordViewModelwithRemove();
            vm.CloseViewAction = CloseAction;

            vm.SkipCommand.Execute(null);
            Assert.AreEqual(_closeDialogResult, true, "Wrong DialogResult for Skip command");
        }

        [Test]
        public void Remove_CheckDialogResultInCloseActionForRemoveCommand()
        {
            var vm = new EncryptionPasswordViewModelwithRemove();
            vm.CloseViewAction = CloseAction;

            vm.RemoveCommand.Execute(null);
            Assert.AreEqual(_closeDialogResult, true, "Wrong DialogResult for Remove command");
        }

        [Test]
        public void Remove_ExecuteOk_PasswordsAreTheSettedPasswords()
        {
            var vm = new EncryptionPasswordViewModelwithRemove(true);
            vm.OwnerPassword = "ownerpw";
            vm.UserPassword = "userpw";

            vm.OkCommand.Execute(null);

            Assert.AreEqual("ownerpw", vm.OwnerPassword, "OwnerPassword is not setted password.");
            Assert.AreEqual("userpw", vm.UserPassword, "UserPassword is not setted password.");
        }

        [Test]
        public void Remove_ExecuteSkip_PasswordsAreEmpty()
        {
            var vm = new EncryptionPasswordViewModelwithRemove(true);
            vm.OwnerPassword = "ownerpw";
            vm.UserPassword = "userpw";

            vm.RemoveCommand.Execute(null);

            Assert.IsEmpty(vm.OwnerPassword, "OwnerPassword is not empty after removing.");
            Assert.IsEmpty(vm.UserPassword, "UserPassword is not empty after removing.");
        }
    }
}
