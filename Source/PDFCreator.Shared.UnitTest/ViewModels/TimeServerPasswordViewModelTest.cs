using NUnit.Framework;
using pdfforge.PDFCreator.Shared.ViewModels;

namespace PDFCreator.Shared.Test.ViewModels
{
    [TestFixture]
    internal class TimeServerPasswordViewModelTest
    {
        private bool? _closeDialogResult;

        private void CloseAction(bool? result)
        {
            _closeDialogResult = result;
        }

        [Test]
        public void InitTest()
        {
            var vm = new TimeServerPasswordViewModel();
            Assert.AreEqual(TimeServerPasswordResponse.Cancel, vm.Response, "Initial Response is not Cancel");
        }

        [Test]
        public void LoginNameIsSet_PasswordIsSet_OkIsExecutable_RemoveIsExecutable()
        {
            var vm = new TimeServerPasswordViewModel();
            vm.TimeServerLoginName = "TimeServerLoginName";
            vm.TimeServerPassword = "TimeServerPassword";
            Assert.IsTrue(vm.OkCommand.IsExecutable, "OkCommand not executable.");
            Assert.IsTrue(vm.RemoveCommand.IsExecutable, "RemoveCommand is not executable.");
        }

        [Test]
        public void LoginNameIsSet_NoPasswordIsSet_OkIsNotExecutable_RemoveIsExecutable()
        {
            var vm = new TimeServerPasswordViewModel();
            vm.TimeServerLoginName = "TimeServerLoginName";
            vm.TimeServerPassword = "";
            Assert.IsFalse(vm.OkCommand.IsExecutable, "OkCommand should not be executable.");
            Assert.IsTrue(vm.RemoveCommand.IsExecutable, "RemoveCommand is not executable.");
        }

        [Test]
        public void NoLoginNameIsSet_PasswordIsSet_OkIsNotExecutable_RemoveIsExecutable()
        {
            var vm = new TimeServerPasswordViewModel();
            vm.TimeServerLoginName = "";
            vm.TimeServerPassword = "TimeServerPassword";
            Assert.IsFalse(vm.OkCommand.IsExecutable, "OkCommand should not be executable.");
            Assert.IsTrue(vm.RemoveCommand.IsExecutable, "RemoveCommand is not executable.");
        }

        [Test]
        public void NoLoginNameIsSet_NoPasswordIsSet_OkIsNotExecutable_RemoveIsExecutable()
        {
            var vm = new TimeServerPasswordViewModel();
            vm.TimeServerLoginName = "";
            vm.TimeServerPassword = "";
            Assert.IsFalse(vm.OkCommand.IsExecutable, "OkCommand should not be executable.");
            Assert.IsTrue(vm.RemoveCommand.IsExecutable, "RemoveCommand is not executable.");
        }

        [Test]
        public void CheckResponses()
        {
            var vm = new TimeServerPasswordViewModel();
            vm.TimeServerPassword = "TimeServerPassword";

            vm.OkCommand.Execute(null);
            Assert.AreEqual(TimeServerPasswordResponse.OK, vm.Response, "Wrong Response for OkCommand.");
            vm.RemoveCommand.Execute(null);
            Assert.AreEqual(TimeServerPasswordResponse.Remove, vm.Response, "Wrong Response for RemoveCommand.");
        }

        [Test]
        public void CheckDialogResultInCloseActionForOkCommand()
        {
            var vm = new TimeServerPasswordViewModel();
            vm.TimeServerPassword = "TimeServerPassword";
            vm.CloseViewAction = CloseAction;

            vm.OkCommand.Execute(null);
            Assert.AreEqual(true, _closeDialogResult, "Wrong DialogResult for OkCommand");
        }

        [Test]
        public void CheckDialogResultInCloseActionForRemoveCommand()
        {
            var vm = new TimeServerPasswordViewModel();
            vm.TimeServerPassword = "TimeServerPassword";
            vm.CloseViewAction = CloseAction;

            vm.RemoveCommand.Execute(null);
            Assert.AreEqual(true, _closeDialogResult, "Wrong DialogResult for RemoveCommand");
        }

        [Test]
        public void ExecuteOK_PasswordIsSettedPassword()
        {
            var vm = new TimeServerPasswordViewModel();
            vm.TimeServerLoginName = "TimeserverLoginName";
            vm.TimeServerPassword = "TimeServerPassword";

            vm.OkCommand.Execute(null);

            Assert.AreEqual("TimeserverLoginName", vm.TimeServerLoginName, "LoginName is not setted value.");
            Assert.AreEqual("TimeServerPassword", vm.TimeServerPassword, "Password is not setted value.");
        }

        [Test]
        public void ExecuteRemove_PasswordAndLoginNameIsEmpty()
        {
            var vm = new TimeServerPasswordViewModel();
            vm.TimeServerLoginName = "TimeserverLoginName";
            vm.TimeServerPassword = "TimeServerPassword";

            vm.RemoveCommand.Execute(null);

            Assert.IsEmpty(vm.TimeServerLoginName, "LoginName is not empty after removing.");
            Assert.IsEmpty(vm.TimeServerPassword, "Password is not empty after removing.");
        }

    }
}