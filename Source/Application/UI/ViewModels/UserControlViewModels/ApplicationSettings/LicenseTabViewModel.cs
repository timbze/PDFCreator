using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using pdfforge.DynamicTranslator;
using pdfforge.LicenseValidator;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.Services.Licensing;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.ViewModels.Wrapper;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.Process;

namespace pdfforge.PDFCreator.UI.ViewModels.UserControlViewModels.ApplicationSettings
{
    public class LicenseTabViewModel : ObservableObject
    {
        private readonly IDispatcher _dispatcher;
        private readonly IInteractionInvoker _interactionInvoker;
        private readonly LicenseKeySyntaxChecker _licenseKeySyntaxChecker = new LicenseKeySyntaxChecker();
        private readonly IProcessStarter _processStarter;
        private readonly IActivationHelper _activationHelper;
        private string _lastActivationKey;
        private bool _isCheckingLicense;

        public LicenseTabViewModel(IProcessStarter processStarter, IActivationHelper activationHelper, ITranslator translator, IInteractionInvoker interactionInvoker, IDispatcher dispatcher)
        {
            _processStarter = processStarter;
            _activationHelper = activationHelper;
            _interactionInvoker = interactionInvoker;
            _dispatcher = dispatcher;
            Translator = translator;

            _lastActivationKey = LicenseKey;

            OnlineActivationCommand = new DelegateCommand(OnlineActivationCommandExecute, OnlineActivationCommandCanExecute);
            OfflineActivationCommand = new DelegateCommand(OfflineActivationCommandExecute, OfflineActivationCommandCanExecute);
            ManageLicensesCommand = new DelegateCommand(ManageLicensesCommandExecute);
        }

        public ITranslator Translator { get; }

        public Activation Activation => _activationHelper.Activation;

        public LicenseStatus LicenseStatus
        {
            get { return _activationHelper.LicenseStatus; }
        }

        public string LicenseStatusText
        {
            get { return Translator?.GetTranslation("LicenseTab", "LicenseStatus." + LicenseStatus); }
        }

        public string LicenseExpiryDate
        {
            get
            {
                if (_activationHelper.Activation == null)
                    return "";
                if (_activationHelper.Activation.LicenseExpires.Year > 2035)
                    return Translator.GetTranslation("LicenseTab", "LicenseExpiresNever");
                ;
                if (_activationHelper.Activation.LicenseExpires == DateTime.MinValue)
                    return "";
                return _activationHelper.Activation.LicenseExpires.ToShortDateString();
            }
        }

        public string ActivationValidTill
        {
            get
            {
                if (_activationHelper.Activation == null)
                    return "";
                if (_activationHelper.Activation.ActivatedTill == DateTime.MinValue)
                    return "";
                return _activationHelper.Activation.ActivatedTill.ToString(CultureInfo.InstalledUICulture);
            }
        }

        public string LastActivationTime
        {
            get
            {
                if (_activationHelper.Activation == null)
                    return "";
                if (_activationHelper.Activation?.TimeOfActivation == DateTime.MinValue)
                    return "";
                return _activationHelper.Activation.TimeOfActivation.ToString(CultureInfo.InstalledUICulture);
            }
        }

        public DelegateCommand OnlineActivationCommand { get; }

        public DelegateCommand OfflineActivationCommand { get; }

        public ICommand ManageLicensesCommand { get; }

        public bool IsCheckingLicense
        {
            get { return _isCheckingLicense; }
            private set
            {
                _isCheckingLicense = value;
                RaisePropertyChanged(nameof(IsCheckingLicense));
                RaisePropertyChanged(nameof(LicenseKey));
                RaisePropertyChanged(nameof(LicenseStatus));
                RaisePropertyChanged(nameof(LicenseStatusText));
                RaisePropertyChanged(nameof(LicenseExpiryDate));
                RaisePropertyChanged(nameof(ActivationValidTill));
                RaisePropertyChanged(nameof(LastActivationTime));
                _dispatcher.BeginInvoke(OnlineActivationCommand.RaiseCanExecuteChanged);
                _dispatcher.BeginInvoke(OfflineActivationCommand.RaiseCanExecuteChanged);
            }
        }

        public AutoResetEvent LicenseCheckFinishedEvent { get; } = new AutoResetEvent(false);

        public string LicenseKey
        {
            get
            {
                if (_activationHelper.Activation?.Key == null)
                    return "";
                return _licenseKeySyntaxChecker.NormalizeLicenseKey(_activationHelper.Activation.Key);
            }
        }

        private void OfflineActivationCommandExecute(object obj)
        {
            var interaction = new OfflineActivationInteraction(_lastActivationKey);
            _interactionInvoker.Invoke(interaction);

            if (interaction.Success)
            {
                _lastActivationKey = interaction.LicenseKey;
                Activation activation;

                try
                {
                    activation = _activationHelper.ActivateOfflineActivationStringFromLicenseServer(interaction.LicenseServerAnswer);
                }
                catch (FormatException)
                {
                    activation = new Activation();
                    activation.Key = interaction.LicenseKey;
                }
                
                UpdateActivation(activation);
                IsCheckingLicense = false;
                LicenseCheckFinishedEvent.Set();
            }
        }

        private bool OnlineActivationCommandCanExecute(object o)
        {
            if (IsCheckingLicense)
                return false;

            return _activationHelper.Activation != null;
        }

        private bool OfflineActivationCommandCanExecute(object o)
        {
            return OnlineActivationCommandCanExecute(o);
        }

        private void OnlineActivationCommandExecute(object o)
        {
            var key = QueryLicenseKey();

            if (string.IsNullOrEmpty(key))
                return;

            ActivateWithKeyAsync(key);
        }

        private string QueryLicenseKey()
        {
            var title = Translator.GetTranslation("pdfforge.PDFCreator.UI.Views.UserControls.ApplicationSettings.LicenseTab", "EnterLicenseKeyButton.Text");
            var questionText = Translator.GetTranslation("pdfforge.PDFCreator.UI.ViewModels.UserControlViewModels.ApplicationSettings", "EnterLicenseKeyText");

            var inputInteraction = new InputInteraction(title, questionText, ValidateLicenseKey);
            inputInteraction.InputText = _lastActivationKey;

            _interactionInvoker.Invoke(inputInteraction);

            if (!inputInteraction.Success)
                return null;

            _lastActivationKey = inputInteraction.InputText;

            return inputInteraction.InputText;
        }

        public InputValidation ValidateLicenseKey(string s)
        {
            var validationResult = _licenseKeySyntaxChecker.ValidateLicenseKey(s);

            string message;
            switch (validationResult)
            {
                case ValidationResult.InvalidCharacters:
                    message = Translator.GetTranslation("pdfforge.PDFCreator.UI.ViewModels.UserControlViewModels.ApplicationSettings",
                        "LicenseKeyContainsIllegalCharacters");
                    return new InputValidation(false, message);
                case ValidationResult.WrongFormat:
                    message = Translator.GetFormattedTranslation(
                        "pdfforge.PDFCreator.UI.ViewModels.UserControlViewModels.ApplicationSettings",
                        "LicenseKeyHasWrongFormat",
                        "AAAAA-BBBBB-CCCCC-DDDDD-EEEEE-12345");
                    return new InputValidation(false, message);
                case ValidationResult.Valid:
                    break;
            }

            return new InputValidation(true);
        }

        private void ActivateWithKeyAsync(string key)
        {
            Task.Factory.StartNew(() => Activate(key));
        }

        private void Activate(string key)
        {
            IsCheckingLicense = true;
            try
            {
                var activation = _activationHelper.ActivateWithoutSavingActivation(key.Replace("-", "")); 
                _dispatcher.Invoke(() => UpdateActivation(activation));
            }
            finally
            {
                IsCheckingLicense = false;
                LicenseCheckFinishedEvent.Set();
            }
        }

        private void UpdateActivation(Activation activation)
        {
            var oldActivation = _activationHelper.Activation;
            var oldActivationWasValid = _activationHelper.IsLicenseValid;
            var resetActivation = false;
            activation = activation?? new Activation();
            _activationHelper.Activation = activation;

            //Save only valid activation. Invalid activations might throw exceptions during saving.
            if (_activationHelper.IsLicenseValid)
            {
                _activationHelper.SaveActivation();
            }
            //Save activation if current key is blocked
            else if ((activation.Result == Result.BLOCKED) 
                && oldActivation.Key.Equals(activation.Key))
            {
                _activationHelper.SaveActivation();
            }
            //Reset license if the old one was valid
            else if (oldActivationWasValid)
            {
                //Do the reset after user notification
                resetActivation = true;
            }
            //set key in invalid activation to make it visible in view   
            else
            {
                _activationHelper.Activation.Key = _lastActivationKey;
            }
            
            //Notify user
            InvokeActivationResponse();

            //Reset license if the old one was valid
            if (resetActivation)
            {
                _activationHelper.Activation = oldActivation;
            }

            CloseLicenseWindowEvent?.Invoke(this, new ActivationResponseEventArgs(_activationHelper.Activation, _activationHelper.IsLicenseValid));
        }

        public delegate void CloseLicenseWindow(object sender, ActivationResponseEventArgs e);
        public event CloseLicenseWindow CloseLicenseWindowEvent;

        private void InvokeActivationResponse()
        {
            if (_activationHelper.IsLicenseValid)
            {
                StoreLicenseForAllUsersQuery();
            }
            else
            {
                var title = Translator.GetTranslation("LicenseTab", "ActivationFailed");
                var message = Translator.GetTranslation("LicenseTab", "ActivationFailedMessage");
                message += "\r\n" + Translator.GetTranslation("LicenseTab", "LicenseStatus." + _activationHelper.LicenseStatus);

                var interaction = new MessageInteraction(message, title, MessageOptions.OK, MessageIcon.Error);
                _interactionInvoker.Invoke(interaction);
            }
        }

        private void ManageLicensesCommandExecute(object o)
        {
            try
            {
                _processStarter.Start(Urls.LicenseServerUrl);
            }
            catch (Exception)
            {   }
        }

        private void StoreLicenseForAllUsersQuery()
        {
            var interaction = new StoreLicenseForAllUsersInteraction();
            _interactionInvoker.Invoke(interaction);
        }
    }
}