using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using pdfforge.PDFCreator.Helper;
using pdfforge.PDFCreator.Shared.Helper;
using pdfforge.PDFCreator.Shared.Licensing;

namespace pdfforge.PDFCreator.Views
{
    internal partial class AboutWindow : Window
    {
        public Edition Edition
        {
            get {  return EditionFactory.Instance.Edition; }
        }

        public AboutWindow()
        {
            InitializeComponent();
            ApplyCustomization();
        }

        private void ApplyCustomization()
        {
            if (Properties.Customization.ApplyCustomization.Equals("true", StringComparison.OrdinalIgnoreCase))
            {
                CustomizationPanel.Visibility = Visibility.Visible;
                CustomText.Text = Properties.Customization.AboutDialog;
                CustomImage.Source = ConvertToBitmapSource(Properties.Customization.customlogo);
            }
            else
            {
                CustomizationPanel.Visibility = Visibility.Hidden;
            }
        }

        private BitmapImage ConvertToBitmapSource(Bitmap image)
        {
            try
            {
                using (MemoryStream memory = new MemoryStream())
                {
                    image.Save(memory, ImageFormat.Png);
                    memory.Position = 0;
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = memory;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();
                    return bitmapImage;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        private void ManualButton_OnClick(object sender, RoutedEventArgs e)
        {
            UserGuideHelper.ShowHelp(null, HelpTopic.General);
        }

        private void LicenseButton_OnClick(object sender, RoutedEventArgs e)
        {
            UserGuideHelper.ShowHelp(null, HelpTopic.License);
        }

        private void ShowUrlInBrowser(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch (Win32Exception)
            {
            }
            catch (FileNotFoundException)
            {
            }
        }

        private void DonateButton_OnClick(object sender, RoutedEventArgs e)
        {
            ShowUrlInBrowser(Urls.DonateUrl);
        }

        private void CompanyButton_OnClick(object sender, RoutedEventArgs e)
        {
            ShowUrlInBrowser(Urls.PdfforgeWebsiteUrl);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            TranslationHelper.Instance.TranslatorInstance.Translate(this);

            VersionText.Text = "PDFCreator " + VersionHelper.Instance.FormatWithBuildNumber();
        }

        private void FacebookButton_OnClick(object sender, RoutedEventArgs e)
        {
            ShowUrlInBrowser(Urls.Facebook);
        }

        private void GooglePlusButton_OnClick(object sender, RoutedEventArgs e)
        {
            ShowUrlInBrowser(Urls.GooglePlus);
        }
    }
}
