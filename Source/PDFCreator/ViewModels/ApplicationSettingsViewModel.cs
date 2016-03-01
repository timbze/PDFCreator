using System;
using System.Windows;
using pdfforge.GpoReader;
using pdfforge.PDFCreator.Core.Settings;
using pdfforge.PDFCreator.Shared.Licensing;
using pdfforge.PDFCreator.Shared.ViewModels;

namespace pdfforge.PDFCreator.ViewModels
{
    internal class ApplicationSettingsViewModel : ViewModelBase
    {
        private ApplicationSettings _applicationSettings;
        private GpoSettings _gpoSettings;
        public EventHandler SettingsChanged;

        public Edition Edition { get; private set; }

        public ApplicationSettings ApplicationSettings
        {
            get { return _applicationSettings; }
            set
            {
                _applicationSettings = value;
                OnSettingsChanged(new EventArgs());
            }
        }

        public GpoSettings GpoSettings
        {
            get { return _gpoSettings; }
            set
            {
                _gpoSettings = value;
                OnSettingsChanged(new EventArgs());
            }
        }

        public ApplicationSettingsViewModel() : this(EditionFactory.Instance.Edition)
        {   }

        /// <summary>
        /// only for testing
        /// </summary>
        public ApplicationSettingsViewModel(Edition edition)
        {
            Edition = edition;
        }

        protected virtual void OnSettingsChanged(EventArgs e)
        {
            RaisePropertyChanged(nameof(ApplicationSettings));
            RaiseGpoPropertiesChanged();

            if (SettingsChanged != null)
                SettingsChanged(this, e);
        }

        private void RaiseGpoPropertiesChanged()
        {
            RaisePropertyChanged(nameof(GpoSettings));
            RaisePropertyChanged(nameof(TitleTabIsEnabled));
            RaisePropertyChanged(nameof(DebugTabIsEnabled));
            RaisePropertyChanged(nameof(PrinterTabIsEnabled));
            RaisePropertyChanged(nameof(PdfArchitectVisibilty));
            RaisePropertyChanged(nameof(LicenseTabVisibility));
        }

        public bool TitleTabIsEnabled
        {
            get
            {
                if (GpoSettings == null)
                    return true;
                return !GpoSettings.DisableTitleTab; 
            }
        }

        public bool DebugTabIsEnabled
        {
            get
            {
                if (GpoSettings == null)
                    return true;
                return !GpoSettings.DisableDebugTab;
            }
        }

        public Visibility LicenseTabVisibility
        {
            get
            {
                if(Edition == null)
                    return Visibility.Visible;

                if(Edition.HideLicensing)
                    return Visibility.Collapsed;

                if (GpoSettings == null)
                    return Visibility.Visible;

                return GpoSettings.HideLicenseTab ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        public bool PrinterTabIsEnabled
        {
            get
            {
                if (GpoSettings == null)
                    return true;
                return !GpoSettings.DisablePrinterTab; 
            }
        }

        public Visibility PdfArchitectVisibilty
        {
            get
            {
                if (GpoSettings == null)
                    return Visibility.Visible;
                return GpoSettings.HidePdfArchitectInfo ? Visibility.Collapsed : Visibility.Visible;
            }
        }
    }
}