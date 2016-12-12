using System;
using System.Windows.Input;
using pdfforge.DynamicTranslator;
using pdfforge.Obsidian;
using pdfforge.Obsidian.Interaction;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.Services.Licensing;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.ViewModels.Helper;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.Process;

namespace pdfforge.PDFCreator.UI.ViewModels.DialogViewModels
{
    public class OfflineActivationViewModel : InteractionAwareViewModelBase<OfflineActivationInteraction>
    {
        private readonly string _invalidLicenseKeySyntaxMessage;
        //private readonly ILicenseChecker _licenseChecker;
        private readonly LicenseKeySyntaxChecker _licenseKeySyntaxChecker = new LicenseKeySyntaxChecker();
        private readonly IProcessStarter _processStarter;
        private readonly IUserGuideHelper _userGuideHelper;
        private readonly IActivationHelper _activationHelper;

        public OfflineActivationViewModel(IProcessStarter process, IUserGuideHelper userGuideHelper, IActivationHelper activationHelper, ITranslator translator)
        {
            _processStarter = process;
            _userGuideHelper = userGuideHelper;
            _activationHelper = activationHelper;
            _invalidLicenseKeySyntaxMessage = translator.GetTranslation("OfflineActivationViewModel",
                "InvalidLicenseKeySyntax");

            OkCommand = new DelegateCommand(OkCommandExecute, OkCommandCanExecute);
            OpenOfflineActivationUrlCommand = new DelegateCommand(OpenOfflineActivationUrlCommandExecute);
            ShowHelpCommand = new DelegateCommand<KeyEventArgs>(ShowHelpCommandExecute);
        }

        public DelegateCommand OkCommand { get; }
        public DelegateCommand OpenOfflineActivationUrlCommand { get; }
        public DelegateCommand<KeyEventArgs> ShowHelpCommand { get; }

        public string LicenseServerAnswer
        {
            get { return Interaction?.LicenseServerAnswer; }
            set
            {
                Interaction.LicenseServerAnswer = value;
                RaisePropertyChanged(nameof(LicenseServerAnswer));
                OkCommand.RaiseCanExecuteChanged();
            }
        }

        public string LicenseKey
        {
            get { return Interaction?.LicenseKey; }
            set
            {
                Interaction.LicenseKey = value;
                RaisePropertyChanged(nameof(LicenseKey));
                RaisePropertyChanged(nameof(LicenseKeyIsValid));
                RaisePropertyChanged(nameof(OfflineActivationString));
            }
        }

        public bool LicenseKeyIsValid
            => _licenseKeySyntaxChecker.ValidateLicenseKey(LicenseKey) == ValidationResult.Valid;

        public string OfflineActivationString
        {
            get
            {
                if (LicenseKeyIsValid)
                    return _activationHelper.GetOfflineActivationString(LicenseKey.Trim());

                return _invalidLicenseKeySyntaxMessage;
            }
        }

        private void OkCommandExecute(object obj)
        {
            Interaction.Success = true;
            FinishInteraction();
        }

        private bool OkCommandCanExecute(object obj)
        {
            return !string.IsNullOrWhiteSpace(LicenseServerAnswer);
        }

        private void OpenOfflineActivationUrlCommandExecute(object obj)
        {
            try
            {
                _processStarter.Start(Urls.OfflineActivationUrl);
            }
            catch (Exception)
            {
            }
        }

        private void ShowHelpCommandExecute(KeyEventArgs e)
        {
            if (e.Key == Key.F1)
                _userGuideHelper.ShowHelp(HelpTopic.AppLicense);
        }

        protected override void HandleInteractionObjectChanged()
        {
            LicenseKey = Interaction.LicenseKey;
        }
    }
}