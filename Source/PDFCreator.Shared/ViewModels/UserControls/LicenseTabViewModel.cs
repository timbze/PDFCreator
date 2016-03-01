using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using NLog;
using pdfforge.DataStorage;
using pdfforge.DynamicTranslator;
using pdfforge.LicenseValidator;
using pdfforge.PDFCreator.Shared.Helper;
using pdfforge.PDFCreator.Shared.Licensing;
using pdfforge.PDFCreator.Shared.Views;

namespace pdfforge.PDFCreator.Shared.ViewModels.UserControls
{
    public class LicenseTabViewModel : ViewModelBase
    {
        private readonly ILicenseServerHelper _licenseServerHelper;
        private readonly IEditionFactory _editionFactory;
        private readonly Translator _translator;
        private readonly Dispatcher _dispatcher = Dispatcher.CurrentDispatcher;
        private readonly Func<string> _queryLicenseKey;

        public Edition Edition { get; private set; }

        public event EventHandler<ActivationResponseEventArgs> ActivationResponse;

        public LicenseStatus LicenseStatus
        {
            get
            {
                return Edition.LicenseStatus;
            }
        }

        public string LicenseStatusText
        {
            get
            {
                return _translator?.GetTranslation("LicenseTab", "LicenseStatus." + LicenseStatus, EnumToStringValueHelper.GetStringValue(LicenseStatus));
            }
        }

        public string LicenseExpiryDate

        {
            get
            {
                if (Edition.Activation == null)
                    return "";
                if (Edition.Activation.LicenseExpires.Year > 2035)
                    return _translator.GetTranslation("LicenseTab", "LicenseExpiresNever", "Never"); ;
                if (Edition.Activation.LicenseExpires == DateTime.MinValue)
                    return "";
                return Edition.Activation.LicenseExpires.ToShortDateString();
            }
        }

        public string ActivationValidTill
        {
            get
            {
                if (Edition.Activation == null)
                    return "";
                if (Edition.Activation.ActivatedTill == DateTime.MinValue)
                    return "";
                return Edition.Activation.ActivatedTill.ToString(CultureInfo.InstalledUICulture);
            }
        }

        public string LastActivationTime
        {
            get
            {
                if (Edition.Activation == null)
                    return "";
                if (Edition.Activation?.TimeOfActivation == DateTime.MinValue)
                    return "";
                return Edition.Activation.TimeOfActivation.ToString(CultureInfo.InstalledUICulture);
            }
        }

        public DelegateCommand RenewActivationCommand { get; }

        public DelegateCommand EnterLicenseKeyCommand { get; }

        public ICommand ManageLicensesCommand { get; }

        private bool _isCheckingLicense;
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
                RaisePropertyChanged(nameof(Edition));
                _dispatcher.BeginInvoke((Action)RenewActivationCommand.RaiseCanExecuteChanged);
                _dispatcher.BeginInvoke((Action)EnterLicenseKeyCommand.RaiseCanExecuteChanged);
            }
        }

        public AutoResetEvent LicenseCheckFinishedEvent { get; } = new AutoResetEvent(false);

        public LicenseTabViewModel()
            : this(EditionFactory.Instance, new LicenseServerHelper(), null, null)
        {
            _translator = TranslationHelper.Instance.IsInitialized ? TranslationHelper.Instance.TranslatorInstance : new BasicTranslator("None", Data.CreateDataStorage());
        }

        public LicenseTabViewModel(IEditionFactory editionFactory, ILicenseServerHelper licenseServerHelper, Func<string> queryLicenseKeyFunction, Translator translator)
        {
            _licenseServerHelper = licenseServerHelper;
            _translator = translator;
            _queryLicenseKey = queryLicenseKeyFunction ?? QueryLicenseKey;
            _editionFactory = editionFactory;
            Edition = editionFactory.Edition;

            RenewActivationCommand = new DelegateCommand(RenewActivationCommandExecute, RenewActivationCommandCanExecute);
            EnterLicenseKeyCommand = new DelegateCommand(EnterLicenseKeyCommandExecute, EnterLicenseKeyCommandCanExecute);
            ManageLicensesCommand = new DelegateCommand(ManageLicensesCommandExecute);
        }

        public string LicenseKey
        {
            get
            {
                if (Edition.Activation?.Key == null)
                    return "";
                return FormatLicenseKey(Edition.Activation.Key);
            }
        }

        private string FormatLicenseKey(string key)
        {
            var normalizedKey = key.Replace("-", "").ToUpper().Trim();
            return string.Join("-", Split(normalizedKey, 5));
        }

        private IEnumerable<string> Split(string str, int chunkSize)
        {
            int chunks = (int)Math.Ceiling(str.Length / (double)chunkSize);

            return Enumerable.Range(0, chunks)
                .Select(i => GetSafeSubstring(str, i * chunkSize, chunkSize));
        }

        private string GetSafeSubstring(string str, int position, int length)
        {
            if (position + length > str.Length)
                length = str.Length - position;

            return str.Substring(position, length);
        }

        private bool RenewActivationCommandCanExecute(object o)
        {
            if (string.IsNullOrEmpty(Edition?.Activation?.Key))
                return false;

            if (IsCheckingLicense)
                return false;

            return Edition.Activation.IsLicenseStillValid();
        }

        private void RenewActivationCommandExecute(object o)
        {
            ActivateWithKeyAsync(LicenseKey);
        }

        private bool EnterLicenseKeyCommandCanExecute(object o)
        {
            if (IsCheckingLicense)
                return false;

            return Edition?.Activation != null;
        }

        private void EnterLicenseKeyCommandExecute(object o)
        {
            var key = _queryLicenseKey();

            if (string.IsNullOrEmpty(key))
                return;

            ActivateWithKeyAsync(key);
        }

        private void ActivateWithKeyAsync(string key)
        {
            Task.Factory.StartNew(() =>
            {
                IsCheckingLicense = true;
                try
                {
                    var licenseChecker = _licenseServerHelper.BuildLicenseChecker(Edition.Activation.Product,
                        RegistryHive.CurrentUser);
                    var activation = licenseChecker.ActivateWithoutSavingActivation(key.Replace("-", ""));
                    UpdateActivation(licenseChecker, activation, key);
                }
                catch
                {   }
                finally
                {
                    IsCheckingLicense = false;
                    LicenseCheckFinishedEvent.Set();
                }
            });
        }

        public void UpdateActivation(ILicenseChecker licenseChecker, Activation activation, string key)
        {
            var oldLicenseWasValid = Edition.IsLicenseValid;

            var newEdition = _editionFactory.DetermineEdition(activation);

            //Save only valid activation. Invalid activations might throw exceptions durin saving.
            if (newEdition.IsLicenseValid)
            {
                licenseChecker.SaveActivation(newEdition.Activation);
                Edition = _editionFactory.ReloadEdition(); //Set Edition by (re)loading it from registry
            }
            else if (!oldLicenseWasValid)
            {
                //set key to make it visible in view and as preset for activation with "new" key   
                newEdition.Activation.Key = key;
                Edition = newEdition;
            }
            RaisePropertyChanged(nameof(Edition));
            //Notify user
            InvokeActivationResponse(newEdition);
        }

        private void InvokeActivationResponse(Edition edition)
        {
            ActivationResponse?.Invoke(this, new ActivationResponseEventArgs(edition));
        }

        private void ManageLicensesCommandExecute(object o)
        {
            Process.Start(new ProcessStartInfo("https://license.pdfforge.org"));
        }

        private string QueryLicenseKey()
        {
            var inputBoxWindow = new InputBoxWindow();

            inputBoxWindow.Title = _translator.GetTranslation("pdfforge.PDFCreator.Shared.Views.UserControls.LicenseTab", "EnterLicenseKeyButton.Text", "Enter new License Key");
            inputBoxWindow.QuestionText = _translator.GetTranslation("pdfforge.PDFCreator.Shared.ViewModels.UserControls.LicenseTabViewModel", "EnterLicenseKeyText", "Please enter your License key:");
            inputBoxWindow.InputText = LicenseKey;
            inputBoxWindow.IsValidInput = ValidateLicenseKey;

            if (inputBoxWindow.ShowDialog() != true)
                return null;

            return inputBoxWindow.InputText;
        }

        public InputBoxValidation ValidateLicenseKey(string s)
        {
            var normalizedKey = FormatLicenseKey(s);

            Regex r = new Regex("^[A-Z0-9]*$");
            if (!r.IsMatch(normalizedKey.Replace("-", "")))
            {
                var message = _translator.GetTranslation("pdfforge.PDFCreator.Shared.ViewModels.UserControls.LicenseTabViewModel",

"LicenseKeyContainsIllegalCharacters",
                     "The license key contains illegal characters. Valid characters are: A-Z, 0-9 and the dash.");
                return new InputBoxValidation(false, message);
            }

            if (normalizedKey.Length != 35)
            {
                var message = _translator.GetFormattedTranslation("pdfforge.PDFCreator.Shared.ViewModels.UserControls.LicenseTabViewModel", "LicenseKeyHasWrongFormat",
                     "The license key consists of 30 characters or numbers, i.e. {0}", "AAAAA-BBBBB-CCCCC-DDDDD-EEEEE-12345");
                return new InputBoxValidation(false, message);
            }

            return new InputBoxValidation(true);
        }
    }
}