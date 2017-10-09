using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Optional;
using pdfforge.LicenseValidator.Interface;
using pdfforge.LicenseValidator.Interface.Data;
using pdfforge.Obsidian;
using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.Services.Licensing;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.TabHelper;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.Process;
using Prism.Mvvm;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.License
{
    public class LicenseSettingsOnlineViewModel : LicenseSettingsViewModel
    {
        public LicenseSettingsOnlineViewModel(IProcessStarter processStarter, ILicenseChecker licenseChecker,
            IInteractionRequest interactionRequest, IDispatcher dispatcher, ITranslationUpdater translationUpdater, IGpoSettings gpoSettings)
            : base(processStarter, licenseChecker, null, interactionRequest, dispatcher, translationUpdater, gpoSettings)
        //offlineActivator not required. Set to null. 
        {
        }

        public override bool ShowOfflineActivation => false;

        protected override void OfflineActivationCommandExecute(object obj)
        {
            throw new InvalidOperationException("Method should not be called when CanExecute returns False");
        }

        protected override bool OfflineActivationCommandCanExecute(object o)
        {
            return false;
        }
    }

    public class LicenseSettingsOfflineViewModel : LicenseSettingsViewModel
    {
        public LicenseSettingsOfflineViewModel(IProcessStarter processStarter, ILicenseChecker licenseChecker, IOfflineActivator offlineActivator,
            IInteractionRequest interactionRequest, IDispatcher dispatcher, ITranslationUpdater translationUpdater, IGpoSettings gpoSettings)
            : base(processStarter, licenseChecker, offlineActivator, interactionRequest, dispatcher, translationUpdater, gpoSettings)
        {
        }

        public override bool ShowOnlineActivation => false;

        protected override void OnlineActivationCommandExecute(object obj)
        {
            throw new InvalidOperationException();
        }

        protected override bool OnlineActivationCommandCanExecute(object o)
        {
            return false;
        }
    }

    public class LicenseSettingsViewModel : TranslatableViewModelBase<LicenseSettingsTranslation>, ITabViewModel
    {
        public delegate void CloseLicenseWindow(object sender, ActivationResponseEventArgs e);

        private readonly IDispatcher _dispatcher;
        private readonly IGpoSettings _gpoSettings;
        private readonly IInteractionRequest _interactionRequest;
        private readonly ILicenseChecker _licenseChecker;
        private readonly LicenseKeySyntaxChecker _licenseKeySyntaxChecker = new LicenseKeySyntaxChecker();
        private readonly IOfflineActivator _offlineActivator;
        private readonly IProcessStarter _processStarter;

        private Option<Activation, LicenseError> _activation;
        private bool _isCheckingLicense;
        
        public LicenseSettingsViewModel(IProcessStarter processStarter, ILicenseChecker licenseChecker, IOfflineActivator offlineActivator,
            IInteractionRequest interactionRequest, IDispatcher dispatcher, ITranslationUpdater translationUpdater, IGpoSettings gpoSettings):base(translationUpdater)
        {
            ShareLicenseForAllUsersEnabled = true;
            _processStarter = processStarter;
            _licenseChecker = licenseChecker;
            _offlineActivator = offlineActivator;

            _interactionRequest = interactionRequest;
            _dispatcher = dispatcher;
            _gpoSettings = gpoSettings;

            OnlineActivationCommand = new DelegateCommand(OnlineActivationCommandExecute, OnlineActivationCommandCanExecute);
            OfflineActivationCommand = new DelegateCommand(OfflineActivationCommandExecute, OfflineActivationCommandCanExecute);
            ManageLicensesCommand = new DelegateCommand(ManageLicensesCommandExecute);

            _activation = licenseChecker.GetSavedActivation();
        }

        public bool ShareLicenseForAllUsersEnabled { private get; set; }

        public virtual bool ShowOnlineActivation => true;
        public virtual bool ShowOfflineActivation => true;

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
                        a => a.ActivatedTill.ToString(CultureInfo.InstalledUICulture),
                        e => "");
            }
        }

        public string LastActivationTime
        {
            get
            {
                var activation = _activation.Filter(a => a.TimeOfActivation > DateTime.MinValue, LicenseError.NoActivation);

                return activation.Match(
                    a => a.TimeOfActivation.ToString(CultureInfo.InstalledUICulture),
                    e => "");
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
                    a => _licenseKeySyntaxChecker.NormalizeLicenseKey(a.Key),
                    e => "");
            }
        }

        public string Title { get; set; } = "License";
        public IconList Icon { get; set; } = IconList.LicenseSettings;
        public bool HiddenByGPO => _gpoSettings.HideLicenseTab;
        public bool BlockedByGPO => false;

        protected override void OnTranslationChanged()
        {
            base.OnTranslationChanged();

            Title = Translation.LicenseTabText;
            RaisePropertyChanged(nameof(Title));
            RaisePropertyChanged(nameof(Translation));
            RaisePropertyChanged(nameof(LicenseStatusText));
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
            _interactionRequest.Raise(interaction, activationInteraction =>
            {
                if (activationInteraction.Success)
                {
                    var activation = _offlineActivator.ValidateOfflineActivationString(interaction.LicenseServerAnswer);

                    activation.MatchSome(a => _offlineActivator.SaveActivation(a));
                    //Just to show in UI
                    //LicenseChecker in UpdateActivation can't save activation
                    UpdateActivation(activation);
                }

                IsCheckingLicense = false;
                LicenseCheckFinishedEvent.Set();
            });
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
            var lastActivationKey = LicenseKey;

            if (string.IsNullOrWhiteSpace(lastActivationKey))
            {
                lastActivationKey = _licenseChecker.GetSavedLicenseKey().ValueOr("");
            }

            var title = Translation.EnterLicenseKey;
            var questionText = Translation.EnterLicenseKeyColon;


            var inputInteraction = new InputInteraction(title, questionText, ValidateLicenseKey);
            inputInteraction.InputText = lastActivationKey;

            _interactionRequest.Raise(inputInteraction, interaction =>
            {
                var key = inputInteraction.InputText;
                if (interaction.Success && !string.IsNullOrEmpty(key))
                {
                    ActivateWithKeyAsync(key);
                }
            });
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
                default:
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

        private void UpdateActivation(Option<Activation, LicenseError> activation)
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

        public event CloseLicenseWindow CloseLicenseWindowEvent;

        private void InvokeActivationResponse(Option<Activation, LicenseError> activation)
        {
            if (!activation.Exists(a => a.IsActivationStillValid()))
            {
                var failedTitle = Translation.ActivationFailed;
                var failedMessage = Translation.ActivationFailedMessage + Environment.NewLine + DetermineLicenseStatusText(activation);
                var failedInteraction = new MessageInteraction(failedMessage, failedTitle, MessageOptions.OK, MessageIcon.Error);
                _interactionRequest.Raise(failedInteraction);
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
                _interactionRequest.Raise(successInteraction);
            }
        }

        private string DetermineLicenseStatusText(Option<Activation, LicenseError> activation)
        {
            return activation.Match(DetermineLicenseStatusText, e => Translation.LicenseStatusNoLicense);
        }

        private LicenseStatusState DetermineLicenseStatusState(Activation activation)
        {
            var returnValue = new LicenseStatusState {Translation = Translation.LicenseStatusError, Status = LicenseStatusForView.Invalid};

            switch (activation.Result)
            {
                case Result.OK:
                    //Activation must be checked before license
                    if (!activation.IsActivationStillValid())
                    {
                        returnValue.Translation = Translation.LicenseStatusActivationExpired;
                    }
                    else if (!activation.IsLicenseStillValid())
                    {
                        returnValue.Translation = Translation.LicenseStatusLicenseExpired;
                    }
                    else if (activation.LicenseType == LicenseType.PERPETUAL && DateTime.Now > activation.LicenseExpires)
                    {
                        returnValue.Translation = Translation.LicenseStatusValidForVersionButLicenseExpired;
                    }
                    else
                    {
                        returnValue.Translation = Translation.LicenseStatusValid;
                        returnValue.Status = LicenseStatusForView.Valid;
                    }
                    break;
                case Result.BLOCKED:
                    returnValue.Translation = Translation.LicenseStatusBlocked;
                    break;
                case Result.LICENSE_EXPIRED:
                    returnValue.Translation = Translation.LicenseStatusVersionNotCoveredByLicense;
                    break;
                case Result.LICENSE_LIMIT_REACHED:
                    returnValue.Translation = Translation.LicenseStatusNumberOfActivationsExceeded;
                    break;
                case Result.INVALID_LICENSE_KEY:
                    returnValue.Translation = Translation.LicenseStatusInvalidLicenseKey;
                    break;
                case Result.NO_LICENSE_KEY:
                    returnValue.Translation = Translation.LicenseStatusNoLicenseKey;
                    break;
                case Result.NO_SERVER_CONNECTION:
                    returnValue.Translation = Translation.LicenseStatusNoServerConnection;
                    break;
                case Result.UNKNOWN_VERSION:
                case Result.AUTH_FAILED:
                case Result.MACHINE_MISMATCH:
                case Result.VERSION_MISMATCH:
                case Result.PRODUCT_MISMATCH:
                case Result.NO_ACTIVATION:
                case Result.NO_HX_DLL:
                case Result.ERROR:
                default:
                    break;
            }
            return returnValue;
        }

        private LicenseStatusForView DetermineLicenseStatus(Activation activation) => DetermineLicenseStatusState(activation).Status;
        private string DetermineLicenseStatusText(Activation activation) => DetermineLicenseStatusState(activation).Translation;

        private LicenseStatusForView DetermineLicenseStatus(Option<Activation, LicenseError> activation)
        {
            return activation.Match(DetermineLicenseStatus, e => LicenseStatusForView.Invalid);
        }


        private void ManageLicensesCommandExecute(object o)
        {
            try
            {
                _processStarter.Start(Urls.LicenseServerUrl);
            }
            catch
            {
                // ignored
            }
        }

        private void StoreLicenseForAllUsersQuery(Activation activation)
        {
            var interaction = new StoreLicenseForAllUsersInteraction(activation.LSA, activation.Key);
            _interactionRequest.Raise(interaction);
        }

        private struct LicenseStatusState
        {
            public string Translation;
            public LicenseStatusForView Status;
        }
    }

    public enum LicenseStatusForView
    {
        Valid,
        ValidForVersionButLicenseExpired,
        Invalid
    }
}
