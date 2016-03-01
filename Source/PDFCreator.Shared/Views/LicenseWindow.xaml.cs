using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using pdfforge.PDFCreator.Shared.Helper;
using pdfforge.PDFCreator.Shared.Licensing;
using pdfforge.PDFCreator.Shared.ViewModels.UserControls;

namespace pdfforge.PDFCreator.Shared.Views
{
    /// <summary>
    /// Interaction logic for LicenseWindow.xaml
    /// </summary>
    public partial class LicenseWindow : Window
    {
        public LicenseWindow()
        {
            InitializeComponent();
            Title = EditionFactory.Instance.Edition.Name;

            LicenseControl.ViewModel.ActivationResponse += CloseWhenLicenseValid;
        }

        private void CloseWhenLicenseValid(object sender, ActivationResponseEventArgs e)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke((Action)(() => CloseWhenLicenseValid(sender, e)));
                return;
            }

            if (e.Edition.IsLicenseValid)
                Close();
        }

        public static void ShowDialogTopMost()
        {
            var licenseWindow = new LicenseWindow();
            TopMostHelper.ShowDialogTopMost(licenseWindow, true);
        }

        private void LicenseWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            TranslationHelper.Instance.TranslatorInstance.Translate(this);
        }


        private void LicenseWindow_OnClosing(object sender, CancelEventArgs e)
        {
            LicenseControl.ViewModel.ActivationResponse -= CloseWhenLicenseValid;
        }

        private void LicenseWindow_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F1)
                UserGuideHelper.ShowHelp(HelpTopic.AppLicense);
        }
    }
}
