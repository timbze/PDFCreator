using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Actions.Actions.Dropbox;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Presentation.DesignTime;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews;
using pdfforge.PDFCreator.Utilities.Threading;
using System;
using System.Linq;
using Translatable;

namespace Presentation.UnitTest.UserControls.AccountViewModels
{
    [TestFixture]
    internal class DropboxAccountViewModelTest
    {
        private IDropboxService _dropboxService;
        private DropboxAccountViewModel _viewModel;
        private DropboxAccountInteraction _accountInteraction;
        private DropboxAppData _dropboxAppData;
        private DropboxTranslation _translation;
        private const string _redirectUri = "https://local";

        [SetUp]
        public void Setup()
        {
            var translationUpdater = new TranslationUpdater(new TranslationFactory(), new ThreadManager());
            _translation = new DropboxTranslation();
            _dropboxService = Substitute.For<IDropboxService>();
            _dropboxService.ParseAccessToken(Arg.Any<Uri>()).Returns("AccessAccount");
            _dropboxAppData = new DropboxAppData("DummieAppKeyForTest", _redirectUri);
            _viewModel = new DropboxAccountViewModel(_dropboxService, _dropboxAppData, translationUpdater);
            _accountInteraction = new DropboxAccountInteraction();
            _viewModel.SetInteraction(_accountInteraction);
        }

        [Test]
        public void Title_IsSetCorrectly()
        {
            Assert.AreEqual(_translation.AddDropboxAccount, _viewModel.Title);
        }

        [Test]
        public void GetAuthorizeUri_CallsDropboxServiceGetAuthorizeUriWithCurrentDropboxAppData()
        {
            var correctUri = new Uri("https://CorrectUri");
            _dropboxService.GetAuthorizeUri(_dropboxAppData.AppKey, _dropboxAppData.RedirectUri).Returns(correctUri);

            Assert.AreSame(correctUri, _viewModel.GetAuthorizeUri());
        }

        [Test]
        public void SetAccessTokenAndUserInfo_ArgumentUriIsNotRedirectUrl_DoesNothing()
        {
            var wascalled = false;
            _viewModel.FinishInteraction = () => wascalled = true;

            _viewModel.SetAccessTokenAndUserInfo(new Uri("http://Does-not-start-with-Redirect-Url"));

            Assert.AreEqual(0, _dropboxService.ReceivedCalls().Count(), "Unwanted DropboxServive Calls");
            Assert.IsFalse(wascalled, "Unwanted call of FinishInteraction");
        }

        [Test]
        public void SetAccessTokenAndUserInfo_ArgumentUriIsRedirectUrl_ParseAccessTokenBeforeGetDropUserInfo()
        {
            var redirectUri = new Uri(_redirectUri + "/SomethingSomethingDarkSide");
            _viewModel.FinishInteraction = () => { };
            _dropboxService.GetDropUserInfo(Arg.Any<string>()).Returns(new DropboxUserInfo());

            _viewModel.SetAccessTokenAndUserInfo(redirectUri);

            Received.InOrder(() =>
            {
                var accessToken = _dropboxService.ParseAccessToken(redirectUri);
                _dropboxService.GetDropUserInfo(accessToken);
            });
        }

        [Test]
        public void SetAccessTokenAndUserInfo_ArgumentUriIsRedirectUrl_InInteractionContainsNewDropboxAccount()
        {
            _viewModel.FinishInteraction = () => { };
            var dropboxUserInfo = new DropboxUserInfo()
            {
                AccessToken = "AccessToken",
                AccountId = "AccountId",
                AccountInfo = "AccountInfo"
            };
            _dropboxService.GetDropUserInfo(Arg.Any<string>()).Returns(dropboxUserInfo);

            _viewModel.SetAccessTokenAndUserInfo(new Uri(_redirectUri));

            Assert.AreEqual(_accountInteraction.DropboxAccount.AccessToken, dropboxUserInfo.AccessToken);
            Assert.AreEqual(_accountInteraction.DropboxAccount.AccountId, dropboxUserInfo.AccountId);
            Assert.AreEqual(_accountInteraction.DropboxAccount.AccountInfo, dropboxUserInfo.AccountInfo);
        }

        [Test]
        public void SetAccessTokenAndUserInfo_ArgumentUriIsRedirectUrl_CallsFinishInteractionAndResultIsSucces()
        {
            var redirectUri = new Uri(_redirectUri + "/SomethingSomethingDarkSide");
            var wascalled = false;
            _viewModel.FinishInteraction = () => wascalled = true;
            _dropboxService.GetDropUserInfo(Arg.Any<string>()).Returns(new DropboxUserInfo() { AccessToken = "AccessToken", AccountId = "AccountId", AccountInfo = "AccountInfo" });

            _viewModel.SetAccessTokenAndUserInfo(redirectUri);

            Assert.AreEqual(DropboxAccountInteractionResult.Success, _accountInteraction.Result);
            Assert.IsTrue(wascalled, "FinishInteraction");
        }

        [Test]
        public void SetAccessTokenAndUserInfo_ArgumentUriIsRedirectUrl_DropboxServiceThrowsArgumentException_ExceptionIsCatchedAndResultIsAccesTokenParsingError()
        {
            var redirectUri = new Uri(_redirectUri + "/SomethingSomethingDarkSide");
            var wasCalled = false;
            _viewModel.FinishInteraction = () => { wasCalled = true; };
            _dropboxService.ParseAccessToken(redirectUri).Throws(new ArgumentException());
            _dropboxService.GetDropUserInfo(Arg.Any<string>()).Throws(new ArgumentException());

            Assert.DoesNotThrow(() => _viewModel.SetAccessTokenAndUserInfo(redirectUri));
            Assert.AreEqual(DropboxAccountInteractionResult.AccesTokenParsingError, _accountInteraction.Result);
            Assert.IsTrue(wasCalled, "FinishInteraction");
        }

        [Test]
        public void SetAccessTokenAndUserInfo_ArgumentUriIsRedirectUrl_DropboxServiceThrowsException_ExceptionIsCatchedAndResultIsError()
        {
            var redirectUri = new Uri(_redirectUri + "/SomethingSomethingDarkSide");
            var wasCalled = false;
            _viewModel.FinishInteraction = () => { wasCalled = true; };
            _dropboxService.ParseAccessToken(redirectUri).Throws(new Exception());
            _dropboxService.GetDropUserInfo(Arg.Any<string>()).Throws(new Exception());

            Assert.DoesNotThrow(() => _viewModel.SetAccessTokenAndUserInfo(redirectUri));
            Assert.AreEqual(DropboxAccountInteractionResult.Error, _accountInteraction.Result);
            Assert.IsTrue(wasCalled, "FinishInteraction");
        }

        [Test]
        public void DesignTimeViewModel_IsNewable()
        {
            var dt = new DesignTimeDropboxAccountViewModel();
            Assert.NotNull(dt);
        }
    }
}
