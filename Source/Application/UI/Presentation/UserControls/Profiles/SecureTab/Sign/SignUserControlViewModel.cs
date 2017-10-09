using pdfforge.Obsidian;
using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.Services.Macros;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.Presentation.Commands;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SecureTab.Sign
{
    public class SignUserControlViewModel : ProfileUserControlViewModel<SignUserControlTranslation>
    {
        private readonly IFile _file;
        private readonly IInteractionRequest _interactionRequest;
        private readonly IOpenFileInteractionHelper _openFileInteractionHelper;
        private readonly ICurrentSettingsProvider _currentSettingsProvider;

        public ICollectionView TimeServerAccountsView { get; }

        private readonly ObservableCollection<TimeServerAccount> _timeServerAccounts;

        public IMacroCommand EditTimeServerAccountCommand { get; }
        public IMacroCommand AddTimeServerAccountCommand { get; }
        public Signature Signature => CurrentProfile?.PdfSettings.Signature;
        public DelegateCommand ChooseCertificateFileCommand { get; }
        public DelegateCommand SignaturePasswordCommand { get; }
        public bool OnlyForPlusAndBusiness { get; }

        public SignUserControlViewModel(IInteractionRequest interactionRequest, IFile file,
            IOpenFileInteractionHelper openFileInteractionHelper, EditionHintOptionProvider editionHintOptionProvider,
            ITranslationUpdater translationUpdater, ISelectedProfileProvider selectedProfile,
            ICurrentSettingsProvider currentSettingsProvider, ICommandLocator commandLocator)
        : base(translationUpdater, selectedProfile)
        {
            _file = file;
            _openFileInteractionHelper = openFileInteractionHelper;
            _interactionRequest = interactionRequest;
            _currentSettingsProvider = currentSettingsProvider;

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
            SignaturePasswordCommand = new DelegateCommand(SignaturePasswordExecute);

            AddTimeServerAccountCommand = commandLocator.GetMacroCommand()
                .AddCommand<TimeServerAccountAddCommand>()
                .AddCommand(new DelegateCommand(o => SelectNewAccountInView()));

            EditTimeServerAccountCommand = commandLocator.GetMacroCommand()
                .AddCommand<TimeServerAccountEditCommand>()
                .AddCommand(new DelegateCommand(o => RefreshAccountsView()));
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
                Signature.CertificateFile = s;
                RaisePropertyChanged(nameof(Signature));
            });
        }

        protected override void OnCurrentProfileChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            base.OnCurrentProfileChanged(sender, propertyChangedEventArgs);
            RaisePropertyChanged(nameof(Signature));
            RaisePropertyChanged(nameof(TimeServerAccountsView));
        }

        private void SignaturePasswordExecute(object obj)
        {
            var certFile = CurrentProfile.PdfSettings.Signature.CertificateFile;

            if (!_file.Exists(certFile))
            {
                ShowCertFileMessage();
                return;
            }

            var signaturePasswordInteraction = new SignaturePasswordInteraction(PasswordMiddleButton.Remove, certFile);
            signaturePasswordInteraction.Password = CurrentProfile.PdfSettings.Signature.SignaturePassword;

            _interactionRequest.Raise(signaturePasswordInteraction, interaction =>
            {
                if (interaction.Result == PasswordResult.StorePassword)
                {
                    CurrentProfile.PdfSettings.Signature.SignaturePassword = interaction.Password;
                }
                else if (interaction.Result == PasswordResult.RemovePassword)
                {
                    CurrentProfile.PdfSettings.Signature.SignaturePassword = "";
                }
            });
        }

        private void ShowCertFileMessage()
        {
            var caption = Translation.PDFSignature;
            var message = Translation.CertificateDoesNotExist;

            var interaction = new MessageInteraction(message, caption, MessageOptions.OK, MessageIcon.Error);
            _interactionRequest.Raise(interaction);
        }
    }
}
