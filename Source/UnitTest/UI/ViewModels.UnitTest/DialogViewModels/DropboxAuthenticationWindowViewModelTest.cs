using System;
using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Actions.Actions.Dropbox;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.ViewModels.DialogViewModels;
using pdfforge.PDFCreator.UI.ViewModels.DialogViewModels.Translations;

namespace pdfforge.PDFCreator.UnitTest.UI.ViewModels.DialogViewModels
{
    [TestFixture]
    internal class DropboxAuthenticationWindowViewModelTest
    {
        [SetUp]
        public void Setup()
        {
            _dropboxService = Substitute.For<IDropboxService>();
            var x  = new DropboxAppData("", redirectUri);
            _viewModel = new DropboxAuthenticationWindowViewModel(_dropboxService,x, new DropboxAuthenticationWindowTranslation());
            _interaction = new DropboxInteraction();
            _viewModel.SetInteraction(_interaction);

        }

        private IDropboxService _dropboxService;
        private DropboxAuthenticationWindowViewModel _viewModel;
        private DropboxInteraction _interaction;
        private string redirectUri = "https://local";

        [Test]
        public void CancelCommand_WhenExecuted_SettSuccessToFalse()
        {
            var wascalled = false;
            _viewModel.FinishInteraction = () => wascalled = true;
            _viewModel.CancelDialogCommand.Execute(null);
            Assert.IsFalse(_interaction.Success);
            Assert.IsTrue(wascalled);
        }
        [Test]
        public void GetUserInfo_WhenExecucted_SetInteractionToSuccess()
        {
            var wascalled = false;
            _viewModel.FinishInteraction = () => wascalled = true;
            _dropboxService.GetDropUserInfo().Returns(new DropboxUserInfo() { AccessToken = "", AccountId = "", AccountInfo = "" });
            _viewModel.SetAccessTokenAndUserInfo(new Uri(redirectUri));
            Assert.IsTrue(_interaction.Success);
            Assert.IsTrue(wascalled);
        }

        [Test]
        public void CheckIsRedirectUriFalse_InteractionNotFinished()
        {
            var wascalled = false;
            _viewModel.FinishInteraction = () => wascalled = true;
            _viewModel.SetAccessTokenAndUserInfo(new Uri("http://wwww.pdfforge.org"));
            Assert.IsFalse(_interaction.Success);
            Assert.IsFalse(wascalled);
        }

        [Test]
        public void CheckIsDropboxUserData_WhenExecucted_InsideInteraction()
        {
            _viewModel.FinishInteraction = () => { };
            var currentDropboxUser = new DropboxUserInfo()
            {
                AccessToken = "AccessToken",
                AccountId = "AccountId",
                AccountInfo = "AccountInfo"
            };
            _dropboxService.GetDropUserInfo().Returns(currentDropboxUser);
            _viewModel.SetAccessTokenAndUserInfo(new Uri(redirectUri));
            Assert.AreEqual(_interaction.AccessToken, currentDropboxUser.AccessToken);
            Assert.AreEqual(_interaction.AccountId, currentDropboxUser.AccountId);
            Assert.AreEqual(_interaction.AccountInfo, currentDropboxUser.AccountInfo);
        }
    }
}