using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Services.Macros;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.Presentation.Commands;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles;
using pdfforge.PDFCreator.UnitTest.UnitTestHelper;
using pdfforge.PDFCreator.Utilities.Threading;
using System.Collections.ObjectModel;
using System.Text;
using Translatable;

namespace Presentation.UnitTest.Commands.AccountsCommands
{
    [TestFixture]
    public class FtpAccountRemoveCommandTest
    {
        private FtpAccountRemoveCommand _ftpAccountRemoveCommand;
        private UnitTestInteractionRequest _interactionRequest;
        private FtpActionTranslation _translation;
        private ObservableCollection<FtpAccount> _ftpAccounts;
        private FtpAccount _usedAccount;
        private FtpAccount _unusedAccount;
        private ObservableCollection<ConversionProfile> _profiles;
        private ConversionProfile _profileWithFtpAccountEnabled;
        private ConversionProfile _profileWithFtpAccountDisabled;

        [SetUp]
        public void SetUp()
        {
            _interactionRequest = new UnitTestInteractionRequest();
            _translation = new FtpActionTranslation();

            _ftpAccounts = new ObservableCollection<FtpAccount>();

            _usedAccount = new FtpAccount();
            _usedAccount.AccountId = nameof(_usedAccount);
            _usedAccount.UserName = "UN1";
            _usedAccount.Server = "SV1";
            _ftpAccounts.Add(_usedAccount);

            _unusedAccount = new FtpAccount();
            _unusedAccount.AccountId = nameof(_unusedAccount);
            _unusedAccount.UserName = "UN2";
            _unusedAccount.Server = "SV2";
            _ftpAccounts.Add(_unusedAccount);

            _profiles = new ObservableCollection<ConversionProfile>();

            _profileWithFtpAccountEnabled = new ConversionProfile();
            _profileWithFtpAccountEnabled.Name = nameof(_profileWithFtpAccountEnabled);
            _profileWithFtpAccountEnabled.Ftp.Enabled = true;
            _profileWithFtpAccountEnabled.Ftp.AccountId = _usedAccount.AccountId;
            _profiles.Add(_profileWithFtpAccountEnabled);

            _profileWithFtpAccountDisabled = new ConversionProfile();
            _profileWithFtpAccountDisabled.Name = nameof(_profileWithFtpAccountDisabled);
            _profileWithFtpAccountDisabled.Ftp.Enabled = false;
            _profileWithFtpAccountDisabled.Ftp.AccountId = _usedAccount.AccountId;
            _profiles.Add(_profileWithFtpAccountDisabled);

            var settings = new PdfCreatorSettings(null);
            settings.ApplicationSettings.Accounts.FtpAccounts = _ftpAccounts;
            settings.ConversionProfiles = _profiles;
            var currentSettingsProvider = Substitute.For<ICurrentSettingsProvider>();
            currentSettingsProvider.Settings.Returns(settings);
            currentSettingsProvider.Profiles.Returns(_profiles);

            var translationUpdater = new TranslationUpdater(new TranslationFactory(), new ThreadManager());

            _ftpAccountRemoveCommand = new FtpAccountRemoveCommand(_interactionRequest, currentSettingsProvider, translationUpdater);
        }

        [Test]
        public void CanExecute_AccountsListIsNull_DoesNotThrowException()
        {
            _ftpAccounts = null;
            Assert.DoesNotThrow(() => _ftpAccountRemoveCommand.CanExecute(null));
        }

        [Test]
        public void CanExcute_AccountListIsEmpty_ReturnsFalse()
        {
            _ftpAccounts.Clear();

            Assert.IsFalse(_ftpAccountRemoveCommand.CanExecute(null));
        }

        [Test]
        public void CanExcute_AccountListNotEmpty_ReturnsTrue()
        {
            Assert.IsTrue(_ftpAccountRemoveCommand.CanExecute(null));
        }

        [Test]
        public void Execute_GivenParameterIsNoFtpAccount_NoInteraction()
        {
            _ftpAccountRemoveCommand.Execute(null);

            _interactionRequest.AssertWasNotRaised<MessageInteraction>();
        }

        [Test]
        public void Execute_WithUnusedFtpAccount_UserCancelsInteraction_AccountRemainsInAccountsList()
        {
            _interactionRequest.RegisterInteractionHandler<MessageInteraction>(i => i.Response = MessageResponse.Cancel);

            _ftpAccountRemoveCommand.Execute(_unusedAccount);

            Assert.AreEqual(2, _ftpAccounts.Count);
            Assert.Contains(_unusedAccount, _ftpAccounts);
        }

        [Test]
        public void Execute_WithUnusedFtpAccount_RaisedMessageInteractionHasCorrectTitleIconButtonsAndMessage()
        {
            _ftpAccountRemoveCommand.Execute(_unusedAccount);

            var interaction = _interactionRequest.AssertWasRaised<MessageInteraction>();

            Assert.AreEqual(_translation.RemoveFtpAccount, interaction.Title, "Title");
            Assert.AreEqual(MessageOptions.YesNo, interaction.Buttons, "Buttons");
            Assert.AreEqual(MessageIcon.Question, interaction.Icon, "Icon");

            var sb = new StringBuilder();
            sb.AppendLine(_unusedAccount.AccountInfo);
            sb.AppendLine(_translation.SureYouWantToDeleteAccount);
            var message = sb.ToString();
            Assert.AreEqual(message, interaction.Text);
        }

        [Test]
        public void Execute_WithUnusedFtpAccount_UserAppliesInteraction_AccountGetsDeletedInAccountsList()
        {
            _interactionRequest.RegisterInteractionHandler<MessageInteraction>(i => i.Response = MessageResponse.Yes);

            _ftpAccountRemoveCommand.Execute(_unusedAccount);

            Assert.AreEqual(1, _ftpAccounts.Count);
            Assert.IsFalse(_ftpAccounts.Contains(_unusedAccount));
        }

        [Test]
        public void Execute_WithUsedFtpAccount_RaisedMessageInteractionHasCorrectTitleIconButtonsAndMessage()
        {
            var numberOfProfiles = _profiles.Count;

            _ftpAccountRemoveCommand.Execute(_usedAccount);

            var interaction = _interactionRequest.AssertWasRaised<MessageInteraction>();

            Assert.AreEqual(_translation.RemoveFtpAccount, interaction.Title, "Title");
            Assert.AreEqual(MessageOptions.YesNo, interaction.Buttons, "Buttons");
            Assert.AreEqual(MessageIcon.Warning, interaction.Icon, "Icon");

            var sb = new StringBuilder();
            sb.AppendLine(_usedAccount.AccountInfo);
            sb.AppendLine(_translation.SureYouWantToDeleteAccount);
            sb.AppendLine();
            sb.AppendLine(_translation.GetAccountIsUsedInFollowingMessage(numberOfProfiles));
            sb.AppendLine();
            sb.AppendLine(_profileWithFtpAccountEnabled.Name);
            sb.AppendLine(_profileWithFtpAccountDisabled.Name);
            sb.AppendLine();
            sb.AppendLine(_translation.GetFtpGetsDisabledMessage(numberOfProfiles));
            var message = sb.ToString();
            Assert.AreEqual(message, interaction.Text);
        }

        [Test]
        public void Execute_WithUsedFtpAccount_UserCancelsInteraction_AccountRemainsInAccountsListAndProfiles()
        {
            var numberOfProfiles = _profiles.Count;

            _interactionRequest.RegisterInteractionHandler<MessageInteraction>(i => i.Response = MessageResponse.Cancel);

            _ftpAccountRemoveCommand.Execute(_usedAccount);

            Assert.AreEqual(numberOfProfiles, _ftpAccounts.Count);
            Assert.Contains(_unusedAccount, _ftpAccounts);
            Assert.IsTrue(_profileWithFtpAccountEnabled.Ftp.Enabled);
            Assert.AreEqual(_usedAccount.AccountId, _profileWithFtpAccountEnabled.Ftp.AccountId);
            Assert.IsFalse(_profileWithFtpAccountDisabled.Ftp.Enabled);
            Assert.AreEqual(_usedAccount.AccountId, _profileWithFtpAccountDisabled.Ftp.AccountId);
        }

        [Test]
        public void Execute_WithUsedFtpAccount_UserAppliesInteraction_AccountsDeletedInAccountsListAndFromProfiles()
        {
            var numberOfProfiles = _profiles.Count;

            _interactionRequest.RegisterInteractionHandler<MessageInteraction>(i => i.Response = MessageResponse.Yes);

            _ftpAccountRemoveCommand.Execute(_usedAccount);

            Assert.AreEqual(numberOfProfiles - 1, _ftpAccounts.Count);
            Assert.IsFalse(_ftpAccounts.Contains(_usedAccount));
            Assert.IsFalse(_profileWithFtpAccountEnabled.Ftp.Enabled);
            Assert.AreEqual("", _profileWithFtpAccountEnabled.Ftp.AccountId);
            Assert.IsFalse(_profileWithFtpAccountDisabled.Ftp.Enabled);
            Assert.AreEqual("", _profileWithFtpAccountDisabled.Ftp.AccountId);
        }

        [Test]
        public void ChangeAccountsCollection_TriggersRaiseCanExecuteChanged()
        {
            var newAccount = new FtpAccount();
            _ftpAccounts.Add(newAccount);
            var wasRaised = false;
            _ftpAccountRemoveCommand.CanExecuteChanged += (sender, args) => wasRaised = true;
            _ftpAccounts.Remove(newAccount);
            Assert.IsTrue(wasRaised);
        }

        [Test]
        public void Execute_UserCancelsInteraction_IsDoneCalledWithCancel()
        {
            var wasCalled = false;
            _ftpAccountRemoveCommand.IsDone += (sender, args) => wasCalled = args.ResponseStatus == ResponseStatus.Cancel;
            _interactionRequest.RegisterInteractionHandler<MessageInteraction>(i => i.Response = MessageResponse.Cancel);

            _ftpAccountRemoveCommand.Execute(_unusedAccount);

            Assert.IsTrue(wasCalled);
        }

        [Test]
        public void Execute_UserAppliesInteraction_IsDoneCalledWithSuccess()
        {
            var wasCalled = false;
            _ftpAccountRemoveCommand.IsDone += (sender, args) => wasCalled = args.ResponseStatus == ResponseStatus.Success;
            _interactionRequest.RegisterInteractionHandler<MessageInteraction>(i => i.Response = MessageResponse.Yes);

            _ftpAccountRemoveCommand.Execute(_unusedAccount);

            Assert.IsTrue(wasCalled);
        }
    }
}
