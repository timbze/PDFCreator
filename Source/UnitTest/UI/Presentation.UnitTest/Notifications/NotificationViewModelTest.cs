using NUnit.Framework;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.Presentation.Notifications;

namespace Presentation.UnitTest.Notifications
{
    [TestFixture]
    internal class NotificationViewModelTest
    {
        [Test]
        public void NotificationIsSuccess_SetIconAndColor_NotificationColorIsSuccessAndIconIsPDFCreator()
        {
            var viewModel = new NotificationViewModel("this title", "this message", MessageIcon.PDFCreator);

            Assert.AreEqual(NotificationColor.Success, viewModel.ColorName);
            Assert.AreEqual(MessageIcon.PDFCreator, viewModel.Icon);
        }

        [Test]
        public void NotificationIsError_SetIconAndColor_NotificationColorAndIconIsError()
        {
            var viewModel = new NotificationViewModel("this title", "this message", MessageIcon.Error);

            Assert.AreEqual(NotificationColor.Error, viewModel.ColorName);
            Assert.AreEqual(MessageIcon.Error, viewModel.Icon);
        }

        [Test]
        public void NotificationIsWarning_SetIconAndColor_NotificationColorAndIconIsWarning()
        {
            var viewModel = new NotificationViewModel("this title", "this message", MessageIcon.Warning);

            Assert.AreEqual(NotificationColor.Warning, viewModel.ColorName);
            Assert.AreEqual(MessageIcon.Warning, viewModel.Icon);
        }
    }
}
