using pdfforge.LicenseValidator;

namespace pdfforge.PDFCreator.Shared.ViewModels
{
    public class OfflineActivationViewModel : ViewModelBase
    {
        private readonly ILicenseChecker _licenseChecker;
        private string _licenseKey = "";

        public string LicenseKey
        {
            get { return _licenseKey; }
            set
            {
                _licenseKey = value;
                RaisePropertyChanged(nameof(LicenseKey));
                RaisePropertyChanged(nameof(OfflineActivationString));
            }
        }

        public string LicenseServerAnswer { get; set; }

        public string OfflineActivationString => _licenseChecker?.GetOfflineActivationString(_licenseKey);

        public OfflineActivationViewModel()
        {
            
        }

        public OfflineActivationViewModel(ILicenseChecker licenseChecker)
        {
            _licenseChecker = licenseChecker;
        }
    }
}
