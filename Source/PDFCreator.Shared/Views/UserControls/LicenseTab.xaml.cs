using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using pdfforge.DynamicTranslator;
using pdfforge.LicenseValidator;
using pdfforge.PDFCreator.Shared.Helper;
using pdfforge.PDFCreator.Shared.Licensing;
using pdfforge.PDFCreator.Shared.ViewModels;
using pdfforge.PDFCreator.Shared.ViewModels.UserControls;

namespace pdfforge.PDFCreator.Shared.Views.UserControls
{
    public partial class LicenseTab : UserControl
    {
        private readonly Translator _translator;

        public LicenseTabViewModel ViewModel
        {
            get { return ((LicenseTabViewModel)DataContext); }
        }

        public LicenseTab()
        {
            InitializeComponent();

            ViewModel.ActivationResponse += ViewModelOnActivationResponse;

            if (TranslationHelper.Instance.IsInitialized)
            {
                _translator = TranslationHelper.Instance.TranslatorInstance;
                _translator.Translate(this);
            }
        }

        private void LicenseTab_OnLoaded(object sender, RoutedEventArgs e)
        {
            // Display conversions from WPF bindings in current locale, i.e. Dates
            this.Language = XmlLanguage.GetLanguage(Thread.CurrentThread.CurrentCulture.Name);
        }

        private void LicenseTab_OnUnloaded(object sender, RoutedEventArgs e)
        {
            ViewModel.ActivationResponse -= ViewModelOnActivationResponse;
        }

        private void ViewModelOnActivationResponse(object sender, ActivationResponseEventArgs e)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke((Action) (() => ViewModelOnActivationResponse(sender, e)));
                return;
            }

            if (e.Edition.IsLicenseValid)
            {
                string title = _translator.GetTranslation("LicenseTab", "ActivationSuccessful", "Activation successful");
                string message = _translator.GetTranslation("LicenseTab", "ActivationSuccessfulMessage", "The license has been activated successfully");

                MessageWindow.ShowTopMost(message, title, MessageWindowButtons.OK, MessageWindowIcon.PDFCreator);
            }
            else
            {
                string title = _translator.GetTranslation("LicenseTab", "ActivationFailed", "Activation failed");
                string message = _translator.GetTranslation("LicenseTab", "ActivationFailedMessage", "The activation was not successful:");
                message += "\r\n" 
                    + _translator?.GetTranslation("LicenseTab", "LicenseStatus." + e.Edition.LicenseStatus, EnumToStringValueHelper.GetStringValue(e.Edition.LicenseStatus));

                MessageWindow.ShowTopMost(message, title, MessageWindowButtons.OK, MessageWindowIcon.Error);
            }
        }

        private void OfflineActivationButton_OnClick(object sender, RoutedEventArgs e)
        {
            var licenseServerHelper = new LicenseServerHelper();
            var product = ViewModel.Edition.Activation.Product;
            var licenseChecker = licenseServerHelper.BuildLicenseChecker(product, RegistryHive.CurrentUser);
            var viewModel = new OfflineActivationViewModel(licenseChecker);
            viewModel.LicenseKey = ViewModel.LicenseKey;

            var offlineActivationWindow = new OfflineActivationWindow(viewModel);

            if (offlineActivationWindow.ShowDialog() == true)
            {
                Activation activation;
                try
                {
                    activation = licenseChecker.ActivateOfflineActivationStringFromLicenseServer(viewModel.LicenseServerAnswer);
                }
                catch (FormatException)
                {
                    activation = new Activation();
                }

                ViewModel.UpdateActivation(licenseChecker, activation, viewModel.LicenseKey);
            }

        }
    }
}
