using System;
using System.Globalization;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Optional;
using pdfforge.LicenseValidator.Interface;
using pdfforge.LicenseValidator.Interface.Data;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.Services.Licensing;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.ViewModels.UserControlViewModels.ApplicationSettings.Translations;
using pdfforge.PDFCreator.UI.ViewModels.Wrapper;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.Process;

namespace pdfforge.PDFCreator.UI.ViewModels.UserControlViewModels.ApplicationSettings
{
    public class LicenseTabOnlineViewModel : LicenseTabViewModel
    {
        public LicenseTabOnlineViewModel(IProcessStarter processStarter, ILicenseChecker licenseChecker, 
            IInteractionInvoker interactionInvoker, IDispatcher dispatcher, LicenseTabTranslation translation) 
            : base(processStarter, licenseChecker, null, interactionInvoker, dispatcher, translation) 
                                                  //offlineActivator not required. Set to null. 
        { }

        public override bool ShowOfflineActivation => false;

        protected override void OfflineActivationCommandExecute(object obj)
        { /*Do nothing */ }

        protected override bool OfflineActivationCommandCanExecute(object o)
        { return false; }
    }

    public class LicenseTabOfflineViewModel : LicenseTabViewModel
    {
        public LicenseTabOfflineViewModel(IProcessStarter processStarter, ILicenseChecker licenseChecker, IOfflineActivator offlineActivator,
            IInteractionInvoker interactionInvoker, IDispatcher dispatcher, LicenseTabTranslation translation)
            : base(processStarter, licenseChecker, offlineActivator, interactionInvoker, dispatcher, translation)
        { }

        public override bool ShowOnlineActivation => false;

        protected override void OnlineActivationCommandExecute(object obj)
        { /*Do nothing */ }

        protected override bool OnlineActivationCommandCanExecute(object o)
        { return false; }
    }


    public class LicenseTabViewModel : ObservableObject
    {
        private readonly IDispatcher _dispatcher;
        private readonly IInteractionInvoker _interactionInvoker;
        private readonly LicenseKeySyntaxChecker _licenseKeySyntaxChecker = new LicenseKeySyntaxChecker();
        private readonly IProcessStarter _processStarter;
        private readonly ILicenseChecker _licenseChecker;
        private readonly IOfflineActivator _offlineActivator;
        private bool _isCheckingLicense;

        private LicenseTabTranslation _translation;

        public bool ShareLicenseForAllUsersEnabled { private get; set; }

        public virtual bool ShowOnlineActivation => true; 
        public virtual bool ShowOfflineActivation => true; 

        public LicenseTabTranslation Translation
        {
            get { return _translation; }
            set
            {
                _translation = value;
                RaisePropertyChanged(nameof(Translation));
                RaisePropertyChanged(nameof(LicenseStatusText));
            }
        }

        public LicenseTabViewModel(IProcessStarter processStarter, ILicenseChecker licenseChecker, IOfflineActivator offlineActivator, 
            IInteractionInvoker interactionInvoker, IDispatcher dispatcher, LicenseTabTranslation translation)
        {
            ShareLicenseForAllUsersEnabled = true;
            _processStarter = processStarter;
            _licenseChecker = licenseChecker;
            _offlineActivator = offlineActivator;

            _interactionInvoker = interactionInvoker;
            _dispatcher = dispatcher;

            _translation = translation;

            Translation = translation;

            OnlineActivationCommand = new DelegateCommand(OnlineActivationCommandExecute, OnlineActivationCommandCanExecute);
            OfflineActivationCommand = new DelegateCommand(OfflineActivationCommandExecute, OfflineActivationCommandCanExecute);
            ManageLicensesCommand = new DelegateCommand(ManageLicensesCommandExecute);
            
            _activation = licenseChecker.GetSavedActivation();
        }

        private Option<Activation, LicenseError> _activation;

        public string Licensee => _activation.Match(a => a.Licensee, e => "");
        public string MachineId => _activation.Match(a => a.MachineId, e => "");
        public string LicenseStatusText => DetermineLicenseStatusText(_activation);
        public LicenseStatusForView LicenseStatusForView => DetermineLicenseStatus(_activation);

        public string LicenseExpiryDate
        {
            get
            {

                var activationDate = _activation.Map(a =>
                {
                    if (a.LicenseExpires.Year > 2035)
                        return Translation.Never;
                    
                    if (a.LicenseExpires == DateTime.MinValue)
                        return "";
                    return a.LicenseExpires.ToShortDateString();
                });

                return activationDate.ValueOr("");
            }
        }

        public string ActivationValidTill
        {
            get
            {
                return _activation
                    .Filter(a => a.ActivatedTill > DateTime.MinValue, LicenseError.NoActivation)
                    .Match(
                    some: a => a.ActivatedTill.ToString(CultureInfo.InstalledUICulture),
                    none: e => "");
            }
        }

        public string LastActivationTime
        {
            get
            {
                var activation = _activation.Filter(a => a.TimeOfActivation > DateTime.MinValue, LicenseError.NoActivation);

                return activation.Match(
                    some: a => a.TimeOfActivation.ToString(CultureInfo.InstalledUICulture),
                    none: e => "");
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
                RaisePropertyChanged(nameof(LicenseStatusText));
                RaisePropertyChanged(nameof(LicenseStatusForView));
                RaisePropertyChanged(nameof(LicenseExpiryDate));
                RaisePropertyChanged(nameof(ActivationValidTill));
                RaisePropertyChanged(nameof(LastActivationTime));
                RaisePropertyChanged(nameof(Licensee));
                RaisePropertyChanged(nameof(MachineId));
                _dispatcher.BeginInvoke(OnlineActivationCommand.RaiseCanExecuteChanged);
                _dispatcher.BeginInvoke(OfflineActivationCommand.RaiseCanExecuteChanged);
            }
        }

        public AutoResetEvent LicenseCheckFinishedEvent { get; } = new AutoResetEvent(false);

        public string LicenseKey
        {
            get
            {
                return _activation.Match(
                        some: a => _licenseKeySyntaxChecker.NormalizeLicenseKey(a.Key),
                        none: e => "");
            }
        }

        protected virtual void OfflineActivationCommandExecute(object obj)
        {
            if (_offlineActivator == null)
                return;

            var lastActivationKey = LicenseKey;

            if (string.IsNullOrWhiteSpace(lastActivationKey))
            {
                lastActivationKey = _licenseChecker.GetSavedLicenseKey().ValueOr("");
            }

            var interaction = new OfflineActivationInteraction(lastActivationKey);
            _interactionInvoker.Invoke(interaction);

            if (interaction.Success)
            {
                var activation = _offlineActivator.ValidateOfflineActivationString(interaction.LicenseServerAnswer);

                try
                {
                    activation.MatchSome(a => _offlineActivator.SaveActivation(a));
                }
                catch (SecurityException)
                {
                }

                //Just to show in UI
                //LicenseChecker in UpdateActivation can't save activation
                UpdateActivation(activation);
            }

            IsCheckingLicense = false;
            LicenseCheckFinishedEvent.Set();
        }

        protected virtual bool OnlineActivationCommandCanExecute(object o)
        {
            return (!IsCheckingLicense);
        }

        protected virtual bool OfflineActivationCommandCanExecute(object o)
        {
            return OnlineActivationCommandCanExecute(o);
        }

        protected virtual void OnlineActivationCommandExecute(object o)
        {
            var key = QueryLicenseKey();

            if (string.IsNullOrEmpty(key))
                return;

            ActivateWithKeyAsync(key);
        }

        private string QueryLicenseKey()
        {
            var lastActivationKey = LicenseKey;

            if (string.IsNullOrWhiteSpace(lastActivationKey))
            {
                lastActivationKey = _licenseChecker.GetSavedLicenseKey().ValueOr("");
            }

            var title = Translation.EnterLicenseKey;
            var questionText = Translation.EnterLicenseKeyColon;

            

            var inputInteraction = new InputInteraction(title, questionText, ValidateLicenseKey);
            inputInteraction.InputText = lastActivationKey;

            _interactionInvoker.Invoke(inputInteraction);

            if (!inputInteraction.Success)
                return null;

            return inputInteraction.InputText;
        }

        public InputValidation ValidateLicenseKey(string s)
        {
            var validationResult = _licenseKeySyntaxChecker.ValidateLicenseKey(s);

            string message;
            switch (validationResult)
            {
                case ValidationResult.InvalidCharacters:
                    message = Translation.LicenseKeyContainsIllegalCharacters;
                    return new InputValidation(false, message);
                case ValidationResult.WrongFormat:
                    message = Translation.GetLicenseKeyHasWrongFormatMessage("AAAAA-BBBBB-CCCCC-DDDDD-EEEEE-12345");
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
                var activation = _licenseChecker.ActivateWithoutSaving(key.Replace("-", "")); 
                _dispatcher.BeginInvoke(() => UpdateActivation(activation));
            }
            finally
            {
                IsCheckingLicense = false;
                LicenseCheckFinishedEvent.Set();
            }
        }

        private void UpdateActivation(Option<Activation,LicenseError> activation)
        {
            var oldKey = _activation.Match(a => a.Key, e => "");

            //Save only valid activation. Invalid activations might throw exceptions during saving.
            if (activation.Exists(a => a.IsLicenseStillValid()))
            {
                activation.MatchSome(a =>
                {
                    _activation = activation;
                    _licenseChecker.SaveActivation(a);
                });
            }
            //Save activation if current key is blocked
            else if (activation.Exists(a => (a.Result == Result.BLOCKED) && a.Key == oldKey))
            {
                activation.MatchSome(a =>
                {
                    _activation = activation;
                    _licenseChecker.SaveActivation(a);
                });
            }
            
            //Notify user
            InvokeActivationResponse(activation);

            CloseLicenseWindowEvent?.Invoke(this, new ActivationResponseEventArgs(activation));
        }

        public delegate void CloseLicenseWindow(object sender, ActivationResponseEventArgs e);
        public event CloseLicenseWindow CloseLicenseWindowEvent;

        private void InvokeActivationResponse(Option<Activation, LicenseError> activation)
        {
            if (!activation.Exists(a => a.IsActivationStillValid()))
            {
                var failedTitle = Translation.ActivationFailed;
                var failedMessage = Translation.ActivationFailedMessage  + Environment.NewLine + DetermineLicenseStatusText(activation);
                var failedInteraction = new MessageInteraction(failedMessage, failedTitle, MessageOptions.OK, MessageIcon.Error);
                _interactionInvoker.Invoke(failedInteraction);
            }
            else if (ShareLicenseForAllUsersEnabled)
            {
                //StoreLicenseForAllUsersQuery is also a Successfull Message
                activation.MatchSome(StoreLicenseForAllUsersQuery);
            }
            else
            {
                var successTitle = Translation.ActivationSuccessful;
                var successMessage = Translation.ActivationSuccessfulMessage;
                var successInteraction = new MessageInteraction(successMessage, successTitle, MessageOptions.OK, MessageIcon.PDFForge);
                _interactionInvoker.Invoke(successInteraction);
            }
        }


        private string DetermineLicenseStatusText(Option<Activation, LicenseError> activation)
        {
            return activation.Match(DetermineLicenseStatusText, e => Translation.LicenseStatusNoLicense);
        }

        private string DetermineLicenseStatusText(Activation activation)
        {
            switch (activation.Result)
            {
                case Result.OK:
                    //Activation must be checked before license
                    if (!activation.IsActivationStillValid())
                        return Translation.LicenseStatusActivationExpired;

                    if (!activation.IsLicenseStillValid())
                        return activation.LicenseType == LicenseType.PERPETUAL
                            ? Translation.LicenseStatusValidForVersionButLicenseExpired
                            : Translation.LicenseStatusLicenseExpired; 

                    return Translation.LicenseStatusValid;
                case Result.BLOCKED:
                    return Translation.LicenseStatusBlocked;
                case Result.LICENSE_EXPIRED:
                    return Translation.LicenseStatusVersionNotCoveredByLicense;
                case Result.LICENSE_LIMIT_REACHED:
                    return Translation.LicenseStatusNumberOfActivationsExceeded;
                case Result.INVALID_LICENSE_KEY:
                    return Translation.LicenseStatusInvalidLicenseKey;
                case Result.NO_LICENSE_KEY:
                    return Translation.LicenseStatusNoLicenseKey;
                case Result.NO_SERVER_CONNECTION:
                    return Translation.LicenseStatusNoServerConnection;
                case Result.PRODUCT_MISMATCH:
                    //Do not interpret. 
                    //Reactivation will cause an UnknownKey and not ProductMismatch, 
                    //which is inconsistant/irritating for the user.  
                case Result.AUTH_FAILED:
                case Result.UNKNOWN_VERSION:
                case Result.NO_HX_DLL:
                case Result.MACHINE_MISMATCH:
                case Result.VERSION_MISMATCH:
                case Result.NO_ACTIVATION:
                case Result.ERROR:
                default:
                    return Translation.LicenseStatusError;
            }
        }

        private LicenseStatusForView DetermineLicenseStatus(Option<Activation, LicenseError> activation)
        {
            return activation.Match(DetermineLicenseStatus, e => LicenseStatusForView.Invalid);
        }

        private LicenseStatusForView DetermineLicenseStatus(Activation activation)
        {
            switch (activation.Result)
            {
                case Result.OK:
                    //Activation must be checked before license
                    if (!activation.IsActivationStillValid())
                        return LicenseStatusForView.Invalid;

                    if (!activation.IsLicenseStillValid())
                        return activation.LicenseType == LicenseType.PERPETUAL
                            ? LicenseStatusForView.ValidForVersionButLicenseExpired
                            : LicenseStatusForView.Invalid;

                    return LicenseStatusForView.Valid;
                case Result.BLOCKED:
                case Result.LICENSE_EXPIRED:
                case Result.LICENSE_LIMIT_REACHED:
                case Result.INVALID_LICENSE_KEY:
                case Result.NO_LICENSE_KEY:
                case Result.NO_SERVER_CONNECTION:
                case Result.PRODUCT_MISMATCH:
                case Result.AUTH_FAILED:
                case Result.UNKNOWN_VERSION:
                case Result.NO_HX_DLL:
                case Result.MACHINE_MISMATCH:
                case Result.VERSION_MISMATCH:
                case Result.NO_ACTIVATION:
                case Result.ERROR:
                default:
                    return LicenseStatusForView.Invalid;
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

        private void StoreLicenseForAllUsersQuery(Activation activation)
        {
            var interaction = new StoreLicenseForAllUsersInteraction(activation.LSA, activation.Key);
            _interactionInvoker.Invoke(interaction);
        }
    }

    public enum LicenseStatusForView
    {
        Valid,
        ValidForVersionButLicenseExpired,
        Invalid
    }
}