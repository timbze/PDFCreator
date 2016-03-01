using NUnit.Framework;
using pdfforge.PDFCreator.Shared.ViewModels;

namespace PDFCreator.Shared.Test.ViewModels
{
    [TestFixture]
    class FtpPasswordViewModelTest
    {
        private bool? _closeDialogResult;

        private void CloseAction(bool? result)
        {
            _closeDialogResult = result;
        }
        
        [Test]
        public void InitTest()
        {
            var vm = new FtpPasswordViewModel();
            Assert.AreEqual(FtpPasswordResponse.Cancel, vm.Response, "Initial Response is not Cancel");
        }

        [Test]
        public void PasswordIsNull_OkIsNotExecutable_SkipAndRemoveAreExecutable()
        {
            var vm = new FtpPasswordViewModel();
            vm.FtpPassword = null;
            Assert.IsFalse(vm.OkCommand.IsExecutable, "OkCommand should not be executable.");
            Assert.IsTrue(vm.SkipCommand.IsExecutable, "SkipCommand is not executable.");
            Assert.IsTrue(vm.RemoveCommand.IsExecutable, "RemoveCommand is not executable.");
        }

        [Test]
        public void NoPasswordIsSet_OkIsNotExecutable_SkipAndRemoveAreExecutable()
        {
            var vm = new FtpPasswordViewModel();
            vm.FtpPassword = "";
            Assert.IsFalse(vm.OkCommand.IsExecutable, "OkCommand should not be executable.");
            Assert.IsTrue(vm.SkipCommand.IsExecutable, "SkipCommand is not executable.");
            Assert.IsTrue(vm.RemoveCommand.IsExecutable, "RemoveCommand is not executable.");
        }

        [Test]
        public void PasswordIsSet_OkIsExecutable_SkipAndRemoveAreExecutable()
        {
            var vm = new FtpPasswordViewModel();
            vm.FtpPassword = "FtpPassword";
            Assert.IsTrue(vm.OkCommand.IsExecutable, "OkCommand not executable.");
            Assert.IsTrue(vm.SkipCommand.IsExecutable, "SkipCommand is not executable.");
            Assert.IsTrue(vm.RemoveCommand.IsExecutable, "RemoveCommand is not executable.");
        }

        [Test]
        public void CheckResponses()
        {
            var vm = new FtpPasswordViewModel();
            vm.FtpPassword = "FtpPassword";

            vm.OkCommand.Execute(null);
            Assert.AreEqual(FtpPasswordResponse.OK, vm.Response, "Wrong Response for OkCommand.");
            vm.RemoveCommand.Execute(null);
            Assert.AreEqual(FtpPasswordResponse.Remove, vm.Response, "Wrong Response for RemoveCommand.");
            vm.SkipCommand.Execute(null);
            Assert.AreEqual(FtpPasswordResponse.Skip, vm.Response, "Wrong Response for SkipCommand.");
        }

        [Test]
        public void CheckDialogResultInCloseActionForOkCommand()
        {
            var vm = new FtpPasswordViewModel();
            vm.FtpPassword = "FtpPassword";
            vm.CloseViewAction = CloseAction;

            vm.OkCommand.Execute(null);
            Assert.AreEqual(true, _closeDialogResult, "Wrong DialogResult for OkCommand");
        }

        [Test]
        public void CheckDialogResultInCloseActionForSkipCommand()
        {
            var vm = new FtpPasswordViewModel();
            vm.FtpPassword = "FtpPassword";
            vm.CloseViewAction = CloseAction;

            vm.SkipCommand.Execute(null);
            Assert.AreEqual(true, _closeDialogResult, "Wrong DialogResult for SkipCommand");
        }

        [Test]
        public void CheckDialogResultInCloseActionForRemoveCommand()
        {
            var vm = new FtpPasswordViewModel();
            vm.FtpPassword = "FtpPassword";
            vm.CloseViewAction = CloseAction;

            vm.RemoveCommand.Execute(null);
            Assert.AreEqual(true, _closeDialogResult, "Wrong DialogResult for RemoveCommand");
        }

        [Test]
        public void ExecuteOk_PasswordIsSettedPassword()
        {
            var vm = new FtpPasswordViewModel();
            vm.FtpPassword = "FtpPassword";

            vm.OkCommand.Execute(null);
            Assert.AreEqual("FtpPassword", vm.FtpPassword);
        }

        [Test]
        public void ExecuteSkip_PasswordIsEmpty()
        {
            var vm = new FtpPasswordViewModel();
            vm.FtpPassword = "FtpPassword";
  
            vm.SkipCommand.Execute(null);
            Assert.AreEqual("", vm.FtpPassword);
        }

        [Test]
        public void ExecuteRemove_PasswordIsEmpty()
        {
            var vm = new FtpPasswordViewModel();
            vm.FtpPassword = "FtpPassword";

            vm.RemoveCommand.Execute(null);
            Assert.AreEqual("", vm.FtpPassword);
        }
    }
}
