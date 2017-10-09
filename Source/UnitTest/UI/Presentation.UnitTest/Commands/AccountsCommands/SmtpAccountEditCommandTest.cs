using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Services.Macros;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Presentation.Commands;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles;
using pdfforge.PDFCreator.UnitTest.UnitTestHelper;
using pdfforge.PDFCreator.Utilities.Threading;
using System.Collections.ObjectModel;
using Translatable;

namespace Presentation.UnitTest.Commands.AccountsCommands
{
    [TestFixture]
    public class SmtpAccountEditCommandTest
    {
        private IWaitableCommand _smtpAccountEditCommand;
        private UnitTestInteractionRequest _interactionRequest;
        private ObservableCollection<SmtpAccount> _smtpAccounts;
        private SmtpAccount _currentSmtpAccount;
        private SmtpTranslation _translation;

        [SetUp]
        public void SetUp()
        {
            _interactionRequest = new UnitTestInteractionRequest();
            _translation = new SmtpTranslation();
            _currentSmtpAccount = new SmtpAccount { UserName = "CurrentUserName" };
            _smtpAccounts = new ObservableCollection<SmtpAccount>();
            _smtpAccounts.Add(_currentSmtpAccount);
            var settings = new PdfCreatorSettings(null);
            settings.ApplicationSettings.Accounts.SmtpAccounts = _smtpAccounts;
            var currentSettingsProvider = Substitute.For<ICurrentSettingsProvider>();
            currentSettingsProvider.Settings.Returns(settings);

            var translationUpdater = new TranslationUpdater(new TranslationFactory(), new ThreadManager());

            _smtpAccountEditCommand = new SmtpAccountEditCommand(_interactionRequest, currentSettingsProvider, translationUpdater);
        }

        [Test]
        public void CanExecute_AccountsListIsNull_DoesNotThrowException()
        {
            _smtpAccounts = null;

            Assert.DoesNotThrow(() => _smtpAccountEditCommand.CanExecute(null));
        }

        [Test]
        public void CanExecute_AccountListIsEmpty_ReturnsFalse()
        {
            _smtpAccounts.Clear();

            Assert.IsFalse(_smtpAccountEditCommand.CanExecute(null));
        }

        [Test]
        public void CanExecute_AccountListHasAccount_ReturnsTrue()
        {
            Assert.IsTrue(_smtpAccountEditCommand.CanExecute(null));
        }

        [Test]
        public void EditAccount_GivenParameterIsNotSmtpAccount_NoInteraction()
        {
            _smtpAccountEditCommand.Execute(null);

            _interactionRequest.AssertWasNotRaised<SmtpAccountInteraction>();
        }

        [Test]
        public void EditAccount_CurrentAccountsDoNotContainGivenAccount_NoInteracion()
        {
            _smtpAccountEditCommand.Execute(new SmtpAccount());

            _interactionRequest.AssertWasNotRaised<SmtpAccountInteraction>();
        }

        [Test]
        public void EditAccount_ExecuteWithCurrentAccount_RaisesInteraction()
        {
            _smtpAccountEditCommand.Execute(_currentSmtpAccount);

            _interactionRequest.AssertWasRaised<SmtpAccountInteraction>();
        }

        [Test]
        public void EditAccount_ExecuteWithCurrentAccount_RaisedInteractionHasCorrectTitle()
        {
            _smtpAccountEditCommand.Execute(_currentSmtpAccount);

            var interaction = _interactionRequest.AssertWasRaised<SmtpAccountInteraction>();
            Assert.AreEqual(_translation.EditSmtpAccount, interaction.Title);
        }

        [Test]
        public void EditAccount_UserCancelsInteraction_CurrentAccountRemainsUnchanged()
        {
            var currentAccountCopy = _currentSmtpAccount.Copy();
            _interactionRequest.RegisterInteractionHandler<SmtpAccountInteraction>(i =>
            {
                i.SmtpAccount.UserName = "Unwanted Changes"; // User makes changes
                i.Success = false; // User cancels
            });

            _smtpAccountEditCommand.Execute(_currentSmtpAccount);

            Assert.AreEqual(_currentSmtpAccount, currentAccountCopy);
        }

        [Test]
        public void EditAccount_UserAppliesInteraction_CurrentAccountContainsChanges()
        {
            _interactionRequest.RegisterInteractionHandler<SmtpAccountInteraction>(i =>
            {
                i.SmtpAccount.UserName = "Changed Username"; // User makes changes
                i.Success = true;
            });

            _smtpAccountEditCommand.Execute(_currentSmtpAccount);

            var interaction = _interactionRequest.AssertWasRaised<SmtpAccountInteraction>();
            Assert.AreEqual(_currentSmtpAccount, interaction.SmtpAccount);
            Assert.AreEqual("Changed Username", _currentSmtpAccount.UserName);
        }

        [Test]
        public void ChangeSmtpAccountsCollection_TriggersRaiseCanExecuteChanged()
        {
            var wasRaised = false;
            _smtpAccountEditCommand.CanExecuteChanged += (sender, args) => wasRaised = true;

            _smtpAccounts.Add(new SmtpAccount());

            Assert.IsTrue(wasRaised);
        }

        [Test]
        public void EditAccount_UserApplies_IsDoneCalledWithSuccess()
        {
            var wasCalled = false;
            _smtpAccountEditCommand.IsDone += (sender, args) => wasCalled = args.ResponseStatus == ResponseStatus.Success;
            _interactionRequest.RegisterInteractionHandler<SmtpAccountInteraction>(i =>
            {
                i.Success = true; //User applies
            });

            _smtpAccountEditCommand.Execute(_currentSmtpAccount);

            Assert.IsTrue(wasCalled);
        }

        [Test]
        public void EditAccount_UserCancels_IsDoneCalledWithCancel()
        {
            var wasCalled = false;
            _smtpAccountEditCommand.IsDone += (sender, args) => wasCalled = args.ResponseStatus == ResponseStatus.Cancel;
            _interactionRequest.RegisterInteractionHandler<SmtpAccountInteraction>(i =>
            {
                i.Success = false; //User cancels
            });

            _smtpAccountEditCommand.Execute(_currentSmtpAccount);

            Assert.IsTrue(wasCalled);
        }
    }
}
