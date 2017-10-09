using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.Services.Macros;
using pdfforge.PDFCreator.UI.Presentation.Commands;
using pdfforge.PDFCreator.UI.Presentation.DesignTime;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SecureTab.Sign;
using pdfforge.PDFCreator.UnitTest.UnitTestHelper;
using pdfforge.PDFCreator.Utilities.Threading;
using System.Collections.ObjectModel;
using System.Windows.Input;
using SystemInterface.IO;
using Translatable;

namespace Presentation.UnitTest.UserControls.Profile
{
    [TestFixture]
    public class SignUserControlViewModelTest
    {
        private SignUserControlViewModel _viewModel;
        private ObservableCollection<TimeServerAccount> _timeServerAccounts;
        private UnitTestInteractionRequest _interactionRequest;
        private ICommand _timeServerAddCommand;
        private ICommand _timeServerEditCommand;

        [SetUp]
        public void SetUp()
        {
            _interactionRequest = new UnitTestInteractionRequest();
            var fileWrap = Substitute.For<IFile>();
            var openFileInteractionHelper = Substitute.For<IOpenFileInteractionHelper>();
            var editionHintOptionProvider = new EditionHintOptionProvider(false, false);
            var translationUpdater = new TranslationUpdater(new TranslationFactory(null), new ThreadManager());
            var settingsProvider = Substitute.For<ICurrentSettingsProvider>();

            var settings = new PdfCreatorSettings(null);
            _timeServerAccounts = new ObservableCollection<TimeServerAccount>();
            settings.ApplicationSettings.Accounts.TimeServerAccounts = _timeServerAccounts;
            settingsProvider.Settings.Returns(settings);

            var commandLocator = Substitute.For<ICommandLocator>();
            commandLocator = Substitute.For<ICommandLocator>();
            commandLocator.GetMacroCommand().Returns(x => new MacroCommand(commandLocator));

            _timeServerAddCommand = Substitute.For<ICommand>();
            commandLocator.GetCommand<TimeServerAccountAddCommand>().Returns(_timeServerAddCommand);

            _timeServerEditCommand = Substitute.For<ICommand>();
            commandLocator.GetCommand<TimeServerAccountEditCommand>().Returns(_timeServerEditCommand);

            _viewModel = new SignUserControlViewModel(_interactionRequest, fileWrap, openFileInteractionHelper,
                editionHintOptionProvider, translationUpdater, settingsProvider,
                settingsProvider, commandLocator);
        }

        [Test]
        public void Initialize_AddTimeServerAccountCommandGetsSetByCommandLocator()
        {
            Assert.AreSame(_timeServerAddCommand, _viewModel.AddTimeServerAccountCommand.GetCommand(0));
            Assert.NotNull(_viewModel.AddTimeServerAccountCommand.GetCommand(1), "Missing UpdateView Command");
        }

        [Test]
        public void Initialize_EditTimeServerAccountCommandGetsSetByCommandLocator()
        {
            Assert.AreSame(_timeServerEditCommand, _viewModel.EditTimeServerAccountCommand.GetCommand(0));
            Assert.IsNotNull(_viewModel.EditTimeServerAccountCommand.GetCommand(1), "Missing UpdateView Command");
        }

        [Test]
        public void AddAccount_LatestAccountIsCurrentItemInView()
        {
            _timeServerAccounts.Add(new TimeServerAccount());

            var latestAccount = new TimeServerAccount();
            _timeServerAddCommand.Execute(Arg.Do<object>(o => _timeServerAccounts.Add(latestAccount)));

            _viewModel.AddTimeServerAccountCommand.Execute(null);

            Assert.AreSame(latestAccount, _viewModel.TimeServerAccountsView.CurrentItem, "Latest Account is not selected Item");
        }

        [Test]
        public void AddAccount_AccountsListisSorted()
        {
            var account1 = new TimeServerAccount() { Url = "1" };
            var account2 = new TimeServerAccount() { Url = "2" };
            var account3 = new TimeServerAccount() { Url = "3" };
            _timeServerAccounts.Add(account3);
            _timeServerAccounts.Add(account1);
            _timeServerAddCommand.Execute(Arg.Do<object>(o => _timeServerAccounts.Add(account2)));

            _viewModel.AddTimeServerAccountCommand.Execute(null);

            _viewModel.TimeServerAccountsView.MoveCurrentToPosition(0);
            Assert.AreSame(account1, _viewModel.TimeServerAccountsView.CurrentItem);
            _viewModel.TimeServerAccountsView.MoveCurrentToPosition(1);
            Assert.AreSame(account2, _viewModel.TimeServerAccountsView.CurrentItem);
            _viewModel.TimeServerAccountsView.MoveCurrentToPosition(2);
            Assert.AreSame(account3, _viewModel.TimeServerAccountsView.CurrentItem);
        }

        [Test]
        public void EditAccount_EditedAccountRemainsCurrentItemInView()
        {
            _timeServerAccounts.Add(new TimeServerAccount());
            var editAccount = new TimeServerAccount();
            _timeServerAccounts.Add(editAccount);
            _viewModel.TimeServerAccountsView.MoveCurrentTo(editAccount);
            _timeServerEditCommand.Execute(Arg.Do<object>(o => editAccount.UserName = "Edit"));

            _viewModel.EditTimeServerAccountCommand.Execute(null);

            Assert.AreSame(editAccount, _viewModel.TimeServerAccountsView.CurrentItem, "Latest Account is not selected Item");
        }

        [Test]
        public void EditAccount_AccountsViewIsSorted()
        {
            var account1 = new TimeServerAccount() { Url = "1" };
            var editAccount = new TimeServerAccount() { Url = "0" }; //for editing
            var account3 = new TimeServerAccount() { Url = "3" };
            _timeServerAccounts.Add(account3);
            _timeServerAccounts.Add(editAccount);
            _timeServerAccounts.Add(account1);
            _timeServerEditCommand.Execute(Arg.Do<object>(o =>
            {
                var editAccountAsParameter = o as TimeServerAccount;
                editAccountAsParameter.Url = "2";
            }));

            _viewModel.EditTimeServerAccountCommand.Execute(editAccount);

            _viewModel.TimeServerAccountsView.MoveCurrentToPosition(0);
            Assert.AreSame(account1, _viewModel.TimeServerAccountsView.CurrentItem);
            _viewModel.TimeServerAccountsView.MoveCurrentToPosition(1);
            Assert.AreSame(editAccount, _viewModel.TimeServerAccountsView.CurrentItem);
            _viewModel.TimeServerAccountsView.MoveCurrentToPosition(2);
            Assert.AreSame(account3, _viewModel.TimeServerAccountsView.CurrentItem);
        }

        [Test]
        public void DesignTimeViewModel_IsNewabled()
        {
            var dtvm = new DesignTimeSignUserControlViewModel();
            Assert.NotNull(dtvm);
        }
    }
}
