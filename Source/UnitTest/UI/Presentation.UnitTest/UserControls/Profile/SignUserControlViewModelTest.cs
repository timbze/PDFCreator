using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.Services.Macros;
using pdfforge.PDFCreator.UI.Presentation.Commands;
using pdfforge.PDFCreator.UI.Presentation.DesignTime;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Tokens;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SecureTab.Sign;
using pdfforge.PDFCreator.UnitTest.UnitTestHelper;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.Threading;
using pdfforge.PDFCreator.Utilities.Tokens;
using Ploeh.AutoFixture;
using System.Collections.Generic;
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
        private ICommand _timeServerAddCommand;
        private ICommand _timeServerEditCommand;

        private IOpenFileInteractionHelper _openFileInteractionHelper;
        private EditionHintOptionProvider _editionHintOptionProvider;
        private TranslationFactory _translationFactory;
        private TranslationUpdater _translationUpdater;
        private ICurrentSettingsProvider _currentSettingsProvider;
        private readonly IFixture _fixture = new Fixture();
        private ConversionProfile _profile;
        private TokenHelper _tokenHelper;
        private TokenReplacer _tokenReplacer;
        private ICommandLocator _commandLocator;
        private ISignaturePasswordCheck _signaturePasswordCheck;
        private IFile _file;

        [SetUp]
        public void SetUp()
        {
            _openFileInteractionHelper = Substitute.For<IOpenFileInteractionHelper>();
            _editionHintOptionProvider = new EditionHintOptionProvider(false, false);

            _translationFactory = new TranslationFactory();
            _translationUpdater = new TranslationUpdater(_translationFactory, new ThreadManager());

            _currentSettingsProvider = Substitute.For<ICurrentSettingsProvider>();
            _profile = new ConversionProfile();
            _currentSettingsProvider.SelectedProfile.Returns(_profile);

            var settings = new PdfCreatorSettings(null);
            _timeServerAccounts = new ObservableCollection<TimeServerAccount>();
            settings.ApplicationSettings.Accounts.TimeServerAccounts = _timeServerAccounts;
            _currentSettingsProvider.Settings.Returns(settings);
            _currentSettingsProvider.SelectedProfile.Returns(new ConversionProfile());

            _tokenHelper = new TokenHelper(_translationUpdater);
            _tokenReplacer = _tokenHelper.TokenReplacerWithPlaceHolders;

            _commandLocator = Substitute.For<ICommandLocator>();
            _commandLocator.CreateMacroCommand().Returns(x => new MacroCommandBuilder(_commandLocator));

            _timeServerAddCommand = Substitute.For<ICommand>();
            _commandLocator.GetCommand<TimeServerAccountAddCommand>().Returns(_timeServerAddCommand);

            _timeServerEditCommand = Substitute.For<ICommand>();
            _commandLocator.GetCommand<TimeServerAccountEditCommand>().Returns(_timeServerEditCommand);

            _signaturePasswordCheck = Substitute.For<ISignaturePasswordCheck>();

            _file = Substitute.For<IFile>();

            InitViewModel();
        }

        private void InitViewModel()
        {
            _viewModel = new SignUserControlViewModel(_openFileInteractionHelper,
                _editionHintOptionProvider, _translationUpdater, _currentSettingsProvider,
                _commandLocator, _signaturePasswordCheck, _file, new TokenViewModelFactory(_currentSettingsProvider, _tokenHelper));
        }

        [Test]
        public void DesignTimeViewModel_IsNewabled()
        {
            var dtvm = new DesignTimeSignUserControlViewModel();
            Assert.NotNull(dtvm);
        }

        [Test]
        public void SignReasonToken_UsesSignReasonTemplateProperty()
        {
            var expectedString = _fixture.Create<string>();

            _currentSettingsProvider.SelectedProfile.PdfSettings.Signature.SignReason = expectedString;

            Assert.AreEqual(expectedString, _viewModel.SignReasonTokenViewModel.Text);
        }

        [Test]
        public void SignReasonTokenViewModel_TokensAreTokenListWithFormatting()
        {
            var tokenList = _tokenHelper.GetTokenListWithFormatting();

            foreach (var tokenWithCommand in _viewModel.SignReasonTokenViewModel.Tokens)
                tokenList.Remove(tokenWithCommand.Name);

            Assert.IsEmpty(tokenList);
        }

        [Test]
        public void SingReasonTokenViewModel_Preview_IsTextWithReplacedTokens()
        {
            var tokenName = _tokenHelper.GetTokenListWithFormatting()[0];
            var tokenValue = _tokenReplacer.ReplaceTokens(tokenName);

            _viewModel.SignReasonTokenViewModel.Text = tokenName;

            Assert.AreEqual(tokenValue, _viewModel.SignReasonTokenViewModel.Preview);
        }

        [Test]
        public void SignContentToken_UsesSignContentTemplateProperty()
        {
            var expectedString = _fixture.Create<string>();

            _currentSettingsProvider.SelectedProfile.PdfSettings.Signature.SignContact = expectedString;

            Assert.AreEqual(expectedString, _viewModel.SignContactTokenViewModel.Text);
        }

        [Test]
        public void SignContactTokenViewModel_TokensAreTokenListWithFormatting()
        {
            var tokenList = _tokenHelper.GetTokenListWithFormatting();

            foreach (var tokenWithCommand in _viewModel.SignContactTokenViewModel.Tokens)
                tokenList.Remove(tokenWithCommand.Name);

            Assert.IsEmpty(tokenList);
        }

        [Test]
        public void SingContactTokenViewModel_Preview_IsTextWithReplacedTokens()
        {
            var tokenName = _tokenHelper.GetTokenListWithFormatting()[0];
            var tokenValue = _tokenReplacer.ReplaceTokens(tokenName);

            _viewModel.SignContactTokenViewModel.Text = tokenName;

            Assert.AreEqual(tokenValue, _viewModel.SignContactTokenViewModel.Preview);
        }

        [Test]
        public void SignLocationToken_UsesSignLocationTemplateProperty()
        {
            var expectedString = _fixture.Create<string>();

            _currentSettingsProvider.SelectedProfile.PdfSettings.Signature.SignLocation = expectedString;

            Assert.AreEqual(expectedString, _viewModel.SignLocationTokenViewModel.Text);
        }

        [Test]
        public void SignLocationTokenViewModel_TokensAreTokenListWithFormatting()
        {
            var tokenList = _tokenHelper.GetTokenListWithFormatting();

            foreach (var tokenWithCommand in _viewModel.SignLocationTokenViewModel.Tokens)
                tokenList.Remove(tokenWithCommand.Name);

            Assert.IsEmpty(tokenList);
        }

        [Test]
        public void SignLocationTokenViewModel_Preview_IsTextWithReplacedTokens()
        {
            var tokenName = _tokenHelper.GetTokenListWithFormatting()[0];
            var tokenValue = _tokenReplacer.ReplaceTokens(tokenName);

            _viewModel.SignLocationTokenViewModel.Text = tokenName;

            Assert.AreEqual(tokenValue, _viewModel.SignLocationTokenViewModel.Preview);
        }

        [Test]
        public void ViewModel_WhenTranslationIsUpdated_UpdatesTokenViewModels()
        {
            var signReasonProperty = new PropertyChangedListenerMock(_viewModel, nameof(_viewModel.SignReasonTokenViewModel));
            var signContactProperty = new PropertyChangedListenerMock(_viewModel, nameof(_viewModel.SignContactTokenViewModel));
            var signLocationProperty = new PropertyChangedListenerMock(_viewModel, nameof(_viewModel.SignLocationTokenViewModel));

            _translationFactory.TranslationSource = Substitute.For<ITranslationSource>();

            Assert.IsTrue(signReasonProperty.WasCalled, "SignReasonTokenViewModel");
            Assert.IsTrue(signContactProperty.WasCalled, "SignContactTokenViewModel");
            Assert.IsTrue(signLocationProperty.WasCalled, "SignLocationTokenViewModel");
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
        public void Init_CertificatePAsswordIsEmpty_AskForPasswordLaterIsEnabled()
        {
            var profile = new ConversionProfile();
            profile.PdfSettings.Signature.SignaturePassword = string.Empty;
            _currentSettingsProvider.SelectedProfile.Returns(profile);

            InitViewModel();

            Assert.IsTrue(_viewModel.AskForPasswordLater);
        }

        [Test]
        public void Init_CertificatePAsswordIsNotEmpty_AskForPasswordLaterIsDisabled()
        {
            var profile = new ConversionProfile();
            profile.PdfSettings.Signature.SignaturePassword = "Not empty";
            _currentSettingsProvider.SelectedProfile.Returns(profile);

            InitViewModel();

            Assert.IsFalse(_viewModel.AskForPasswordLater);
        }

        [Test]
        public void AskForPasswordLater_EnabelingClearsprofile()
        {
            _viewModel.Password = "Not empty";

            _viewModel.AskForPasswordLater = false;
            Assert.IsNotEmpty(_viewModel.Password, "Password should not be cleared for disabled AskForPasswordLater");

            _viewModel.AskForPasswordLater = true;
            Assert.IsEmpty(_viewModel.Password, "Password should be cleared for enabled AskForPasswordLater");
        }

        [Test]
        public void AskForPasswordLater_Enable_RaisesPropertyChangedForPassword_AskForPasswordLater_CertificatePasswordIsValid()
        {
            var calledProperties = new List<string>();
            _viewModel.PropertyChanged += (sender, args) => calledProperties.Add(args.PropertyName);

            _viewModel.AskForPasswordLater = true;

            Assert.Contains(nameof(_viewModel.Password), calledProperties);
            Assert.Contains(nameof(_viewModel.AskForPasswordLater), calledProperties);
            Assert.Contains(nameof(_viewModel.CertificatePasswordIsValid), calledProperties);
        }

        [Test]
        public void AskForPasswordLater_Disable_RaisesPropertyChangedForAskForPasswordLaterAndCertificatePasswordIsValid()
        {
            var calledProperties = new List<string>();
            _viewModel.PropertyChanged += (sender, args) => calledProperties.Add(args.PropertyName);

            _viewModel.AskForPasswordLater = false;

            Assert.Contains(nameof(_viewModel.AskForPasswordLater), calledProperties);
            Assert.Contains(nameof(_viewModel.CertificatePasswordIsValid), calledProperties);
        }

        [Test]
        public void CertificatePasswordIsValid_CertificateFileIsEmpty_ReturnsFalse()
        {
            var profile = new ConversionProfile();
            profile.PdfSettings.Signature.CertificateFile = string.Empty;
            _currentSettingsProvider.SelectedProfile.Returns(profile);

            Assert.IsFalse(_viewModel.CertificatePasswordIsValid);
        }

        [Test]
        public void CertificatePasswordIsValid_CertificateFileDoesNotExist_ReturnsFalse()
        {
            var certifiateFile = "CertificateFile.psx";
            _file.Exists(certifiateFile).Returns(false);
            var profile = new ConversionProfile();
            profile.PdfSettings.Signature.CertificateFile = certifiateFile;
            _currentSettingsProvider.SelectedProfile.Returns(profile);

            Assert.IsFalse(_viewModel.CertificatePasswordIsValid);
        }

        [Test]
        public void CertificatePasswordIsValid_CertificateFileExistsAskForPasswordLaterDisabledPasswordCheckFails_ReturnsFalse()
        {
            var certificateFile = "CertificateFile.psx";
            _file.Exists(certificateFile).Returns(true);
            var profile = new ConversionProfile();
            profile.PdfSettings.Signature.CertificateFile = certificateFile;
            _currentSettingsProvider.SelectedProfile.Returns(profile);
            _viewModel.AskForPasswordLater = false;
            _viewModel.Password = "SomePassword";
            _signaturePasswordCheck.IsValidPassword(certificateFile, _viewModel.Password).Returns(false);

            Assert.IsFalse(_viewModel.CertificatePasswordIsValid);
        }

        [Test]
        public void CertificatePasswordIsValid_CertificateFileExistsAskForPasswordLaterEnabledPasswordCheckFails_ReturnsTrue()
        {
            var certificateFile = "CertificateFile.psx";
            _file.Exists(certificateFile).Returns(true);
            var profile = new ConversionProfile();
            profile.PdfSettings.Signature.CertificateFile = certificateFile;
            _currentSettingsProvider.SelectedProfile.Returns(profile);
            _viewModel.AskForPasswordLater = true; //<<<<<<<<<<<<<<<<<<<<<<<<
            _signaturePasswordCheck.IsValidPassword(certificateFile, _viewModel.Password).Returns(false);

            Assert.IsTrue(_viewModel.CertificatePasswordIsValid);
        }

        [Test]
        public void CertificatePasswordIsValid_CertificateFileExistsAskForPasswordLaterDisabledPasswordCheckSuccessful_ReturnsTrue()
        {
            var certificateFile = "CertificateFile.psx";
            _file.Exists(certificateFile).Returns(true);
            var profile = new ConversionProfile();
            profile.PdfSettings.Signature.CertificateFile = certificateFile;
            _currentSettingsProvider.SelectedProfile.Returns(profile);
            _viewModel.AskForPasswordLater = false;
            _viewModel.Password = "SomePassword";
            _signaturePasswordCheck.IsValidPassword(certificateFile, _viewModel.Password).Returns(true);

            Assert.IsTrue(_viewModel.CertificatePasswordIsValid);
        }

        [Test]
        public void SetCertificateFile_CallsRaisePropertyChangedOfCertificatePasswordIsValid()
        {
            var calledProperties = new List<string>();
            _viewModel.PropertyChanged += (sender, args) => calledProperties.Add(args.PropertyName);

            _viewModel.CertificateFile = "Something";

            Assert.Contains(nameof(_viewModel.CertificatePasswordIsValid), calledProperties);
        }

        [Test]
        public void SetPassword_CallsRaisePropertyChangedOfCertificatePasswordIsValid()
        {
            var calledProperties = new List<string>();
            _viewModel.PropertyChanged += (sender, args) => calledProperties.Add(args.PropertyName);

            _viewModel.Password = "Something";

            Assert.Contains(nameof(_viewModel.CertificatePasswordIsValid), calledProperties);
        }

        [Test]
        public void SetPassword_DoesNotCallRaisePropertyChangedOfPassword()
        {
            var calledProperties = new List<string>();
            _viewModel.PropertyChanged += (sender, args) => calledProperties.Add(args.PropertyName);

            _viewModel.Password = "Something";

            Assert.IsFalse(calledProperties.Contains(nameof(_viewModel.Password)), "Set Password must not call RaiseProprtyChanged for Password. It causes circular calls.");
        }
    }
}
