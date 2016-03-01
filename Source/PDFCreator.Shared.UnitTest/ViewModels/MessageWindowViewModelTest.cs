using NUnit.Framework;
using pdfforge.PDFCreator.Shared.ViewModels;
using pdfforge.PDFCreator.Shared.Views;

namespace PDFCreator.Shared.Test.ViewModels
{
    [TestFixture]
    internal class MessageWindowViewModelTest
    {
        private bool? _closeDialogResult;
        
        private void CloseAction(bool? result)
        {
            _closeDialogResult = result;
        }

        [Test]
        public void InitTest()
        {
            var model = new MessageWindowViewModel();
            Assert.AreEqual(MessageWindowResponse.Cancel, model.Response);
        }
        
        [Test]
        public void ButtonsOkTest()
        {
            var model = new MessageWindowViewModel(MessageWindowButtons.OK);
            model.CloseViewAction = CloseAction;

            Assert.IsFalse(model.ButtonLeftCommand.IsExecutable);
            Assert.IsFalse(model.ButtonMiddleCommand.IsExecutable);
            Assert.IsTrue(model.ButtonRightCommand.IsExecutable);

            model.ButtonRightCommand.Execute(null);

            Assert.AreEqual(MessageWindowResponse.OK, model.Response);
            Assert.AreEqual(true, _closeDialogResult);
        }

        [Test]
        public void ButtonsOkCancelTest()
        {
            var model = new MessageWindowViewModel(MessageWindowButtons.OKCancel);
            model.CloseViewAction = CloseAction;

            Assert.IsTrue(model.ButtonLeftCommand.IsExecutable);
            Assert.IsFalse(model.ButtonMiddleCommand.IsExecutable);
            Assert.IsTrue(model.ButtonRightCommand.IsExecutable);

            model.ButtonLeftCommand.Execute(null);
            Assert.AreEqual(MessageWindowResponse.OK, model.Response);
            Assert.AreEqual(true, _closeDialogResult);

            model.ButtonRightCommand.Execute(null);
            Assert.AreEqual(MessageWindowResponse.Cancel, model.Response);
            Assert.AreEqual(false, _closeDialogResult);
        }

        [Test]
        public void ButtonsRetryCancelTest()
        {
            var model = new MessageWindowViewModel(MessageWindowButtons.RetryCancel);
            model.CloseViewAction = CloseAction;

            Assert.IsTrue(model.ButtonLeftCommand.IsExecutable);
            Assert.IsFalse(model.ButtonMiddleCommand.IsExecutable);
            Assert.IsTrue(model.ButtonRightCommand.IsExecutable);

            model.ButtonLeftCommand.Execute(null);
            Assert.AreEqual(MessageWindowResponse.Retry, model.Response);
            Assert.AreEqual(true, _closeDialogResult);

            model.ButtonRightCommand.Execute(null);
            Assert.AreEqual(MessageWindowResponse.Cancel, model.Response);
            Assert.AreEqual(false, _closeDialogResult);
        }

        [Test]
        public void ButtonsYesLaterNoTest()
        {
            var model = new MessageWindowViewModel(MessageWindowButtons.YesLaterNo);
            model.CloseViewAction = CloseAction;

            Assert.IsTrue(model.ButtonLeftCommand.IsExecutable);
            Assert.IsTrue(model.ButtonMiddleCommand.IsExecutable);
            Assert.IsTrue(model.ButtonRightCommand.IsExecutable);

            model.ButtonLeftCommand.Execute(null);
            Assert.AreEqual(MessageWindowResponse.Yes, model.Response);
            Assert.AreEqual(true, _closeDialogResult);

            model.ButtonMiddleCommand.Execute(null);
            Assert.AreEqual(MessageWindowResponse.Later, model.Response);
            Assert.AreEqual(false, _closeDialogResult);

            model.ButtonRightCommand.Execute(null);
            Assert.AreEqual(MessageWindowResponse.No, model.Response);
            Assert.AreEqual(false, _closeDialogResult);
        }

        [Test]
        public void ButtonsYesNoTest()
        {
            var model = new MessageWindowViewModel(MessageWindowButtons.YesNo);
            model.CloseViewAction = CloseAction;

            Assert.IsTrue(model.ButtonLeftCommand.IsExecutable);
            Assert.IsFalse(model.ButtonMiddleCommand.IsExecutable);
            Assert.IsTrue(model.ButtonRightCommand.IsExecutable);

            model.ButtonLeftCommand.Execute(null);
            Assert.AreEqual(MessageWindowResponse.Yes, model.Response);
            Assert.AreEqual(true, _closeDialogResult);

            model.ButtonRightCommand.Execute(null);
            Assert.AreEqual(MessageWindowResponse.No, model.Response);
            Assert.AreEqual(false, _closeDialogResult);
        }
    }
}
