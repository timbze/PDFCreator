using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.Services.Macros;
using pdfforge.PDFCreator.UI.Presentation.Commands;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Tokens;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.Utilities;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SecureTab.Sign
{
    public class SignUserControlViewModel : ProfileUserControlViewModel<SignUserControlTranslation>
    {
        private readonly IOpenFileInteractionHelper _openFileInteractionHelper;
        private readonly ISignaturePasswordCheck _signaturePasswordCheck;
        private readonly IFile _file;

        public TokenViewModel<ConversionProfile> SignReasonTokenViewModel { get; private set; }
        public TokenViewModel<ConversionProfile> SignContactTokenViewModel { get; private set; }
        public TokenViewModel<ConversionProfile> SignLocationTokenViewModel { get; private set; }

        public ICollectionView TimeServerAccountsView { get; }

        private readonly ObservableCollection<TimeServerAccount> _timeServerAccounts;

        public IMacroCommand EditTimeServerAccountCommand { get; }
        public IMacroCommand AddTimeServerAccountCommand { get; }
        public Signature Signature => CurrentProfile?.PdfSettings.Signature;
        public DelegateCommand ChooseCertificateFileCommand { get; }
        public bool OnlyForPlusAndBusiness { get; }

        public string Password
        {
            get { return Signature?.SignaturePassword; }
            set
            {
                Signature.SignaturePassword = value;
                RaisePropertyChanged(nameof(CertificatePasswordIsValid));
            }
        }

        public string CertificateFile
        {
            get { return Signature?.CertificateFile; }
            set
            {
                Signature.CertificateFile = value;
                RaisePropertyChanged(nameof(CertificateFile));
                RaisePropertyChanged(nameof(CertificatePasswordIsValid));
            }
        }

        public SignUserControlViewModel(
            IOpenFileInteractionHelper openFileInteractionHelper, EditionHintOptionProvider editionHintOptionProvider,
            ITranslationUpdater translationUpdater, ICurrentSettingsProvider currentSettingsProvider,
            ICommandLocator commandLocator, ISignaturePasswordCheck signaturePasswordCheck,
            IFile file, ITokenViewModelFactory tokenViewModelFactory, IDispatcher dispatcher)
        : base(translationUpdater, currentSettingsProvider, dispatcher)
        {
            _openFileInteractionHelper = openFileInteractionHelper;

            _signaturePasswordCheck = signaturePasswordCheck;
            _file = file;

            translationUpdater.RegisterAndSetTranslation(tf => SetTokenViewModels(tokenViewModelFactory));
            currentSettingsProvider.SelectedProfileChanged += (sender, args) => SetTokenViewModels(tokenViewModelFactory);

            if (editionHintOptionProvider != null)
            {
                OnlyForPlusAndBusiness = editionHintOptionProvider.ShowOnlyForPlusAndBusinessHint;
            }

            if (currentSettingsProvider?.Settings != null)
            {
                _timeServerAccounts = currentSettingsProvider.Settings.ApplicationSettings.Accounts.TimeServerAccounts;
                TimeServerAccountsView = new ListCollectionView(_timeServerAccounts);
                TimeServerAccountsView.SortDescriptions.Add(new SortDescription(nameof(TimeServerAccount.AccountInfo), ListSortDirection.Ascending));
            }

            ChooseCertificateFileCommand = new DelegateCommand(ChooseCertificateFileExecute);

            if (Signature != null)
                AskForPasswordLater = string.IsNullOrEmpty(Password);

            AddTimeServerAccountCommand = commandLocator.CreateMacroCommand()
                .AddCommand<TimeServerAccountAddCommand>()
                .AddCommand(new DelegateCommand(o => SelectNewAccountInView()))
                .Build();

            EditTimeServerAccountCommand = commandLocator.CreateMacroCommand()
                .AddCommand<TimeServerAccountEditCommand>()
                .AddCommand(new DelegateCommand(o => RefreshAccountsView()))
                .Build();
        }

        private void SetTokenViewModels(ITokenViewModelFactory tokenViewModelFactory)
        {
            var builder = tokenViewModelFactory
                .BuilderWithSelectedProfile()
                .WithDefaultTokenReplacerPreview(th => th.GetTokenListWithFormatting());

            SignReasonTokenViewModel = builder
                .WithSelector(p => p.PdfSettings.Signature.SignReason)
                .Build();

            SignContactTokenViewModel = builder
                .WithSelector(p => p.PdfSettings.Signature.SignContact)
                .Build();

            SignLocationTokenViewModel = builder
                .WithSelector(p => p.PdfSettings.Signature.SignLocation)
                .Build();

            RaisePropertyChanged(nameof(SignReasonTokenViewModel));
            RaisePropertyChanged(nameof(SignContactTokenViewModel));
            RaisePropertyChanged(nameof(SignLocationTokenViewModel));
        }

        private void SelectNewAccountInView()
        {
            var latestAccount = _timeServerAccounts.Last();
            TimeServerAccountsView.MoveCurrentTo(latestAccount);
        }

        private void RefreshAccountsView()
        {
            TimeServerAccountsView.Refresh();
        }

        private void ChooseCertificateFileExecute(object obj)
        {
            var title = Translation.SelectCertFile;
            var filter = Translation.PfxP12Files
                         + @" (*.pfx, *.p12)|*.pfx;*.p12|"
                         + Translation.AllFiles
                         + @" (*.*)|*.*";

            var interactionResult = _openFileInteractionHelper.StartOpenFileInteraction(Signature.CertificateFile, title, filter);

            interactionResult.MatchSome(s =>
            {
                CertificateFile = s;
                RaisePropertyChanged(nameof(Signature));
            });
        }

        protected override void OnCurrentProfileChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            base.OnCurrentProfileChanged(sender, propertyChangedEventArgs);
            RaisePropertyChanged(nameof(Signature));
            RaisePropertyChanged(nameof(TimeServerAccountsView));
            RaisePropertyChanged(nameof(Password));
            RaisePropertyChanged(nameof(CertificateFile));
            RaisePropertyChanged(nameof(CertificatePasswordIsValid));
            RaisePropertyChanged(nameof(AskForPasswordLater));

            if (string.IsNullOrEmpty(Password))
                AskForPasswordLater = true;
        }

        public bool CertificatePasswordIsValid
        {
            get
            {
                if (string.IsNullOrEmpty(CertificateFile))
                    return false;

                if (!_file.Exists(CertificateFile))
                    return false;

                if (AskForPasswordLater)
                    return true;

                return _signaturePasswordCheck.IsValidPassword(Signature.CertificateFile, Password);
            }
        }

        private bool _askForPasswordLater;

        public bool AskForPasswordLater
        {
            get { return _askForPasswordLater; }
            set
            {
                _askForPasswordLater = value;
                if (value)
                {
                    Password = "";
                    RaisePropertyChanged(nameof(Password));
                }
                RaisePropertyChanged(nameof(AskForPasswordLater));
                RaisePropertyChanged(nameof(CertificatePasswordIsValid));
            }
        }
    }
}
