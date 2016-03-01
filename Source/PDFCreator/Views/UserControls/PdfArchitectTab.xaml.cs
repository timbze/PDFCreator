using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using pdfforge.PDFCreator.Helper;
using pdfforge.PDFCreator.Shared.Helper;

namespace pdfforge.PDFCreator.Views.UserControls
{
    internal partial class PdfArchitectTab : UserControl
    {
        private static readonly TranslationHelper TranslationHelper = TranslationHelper.Instance;

        public PdfArchitectTab()
        {
            InitializeComponent();

            if (TranslationHelper.IsInitialized)
            {
                TranslationHelper.TranslatorInstance.Translate(this);
            }

            if (PdfArchitectHelper.IsPdfArchitectInstalled)
            {
                LaunchPdfArchitectStackPanel.Visibility = Visibility.Visible;
                GetPdfArchitectStackPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                LaunchPdfArchitectStackPanel.Visibility = Visibility.Collapsed;
                GetPdfArchitectStackPanel.Visibility = Visibility.Visible;
            }
        }

        private void LaunchPdfArchitectButton_OnClick(object sender, RoutedEventArgs e)
        {
            string applicationPath = PdfArchitectHelper.InstallPath;

            if (!string.IsNullOrWhiteSpace(applicationPath))
                Process.Start(applicationPath);
        }

        private void GetPdfArchitectButton_OnClick(object sender, RoutedEventArgs e)
        {
            Process.Start(Urls.ArchitectWebsiteUrl);
        }

        private void DownloadPdfArchitectButton_OnClickPdfArchitectButton_OnClick(object sender, RoutedEventArgs e)
        {
            Process.Start(Urls.ArchitectDownloadUrl);
        }
    }
}
