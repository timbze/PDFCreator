using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Media;
using System.Windows;
using System.Windows.Media.Imaging;
using pdfforge.PDFCreator.Helper;
using pdfforge.PDFCreator.Shared.Helper;
using Image = System.Windows.Controls.Image;

namespace pdfforge.PDFCreator.Views
{
    internal partial class RecommendPdfArchitectWindow : Window
    {
        public bool ShowViewerWarning { get; set; }

        public RecommendPdfArchitectWindow(bool showViewerWarning)
        {
            ShowViewerWarning = showViewerWarning;

            InitializeComponent();
            var img = new Image();

            if (ShowViewerWarning)
            {
                img.Source = ConvertBitmap(SystemIcons.Warning.ToBitmap());
                SystemSounds.Hand.Play();
                WarningIconBox.Content = img;
            }
        }

        private BitmapImage ConvertBitmap(Bitmap value)
        {
            var ms = new MemoryStream();
            value.Save(ms, ImageFormat.Png);
            var image = new BitmapImage();
            image.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            image.StreamSource = ms;
            image.EndInit();

            return image;
        }

        private void InfoButton_OnClick(object sender, RoutedEventArgs e)
        {
            Process.Start(Urls.ArchitectWebsiteUrl);
            DialogResult = true;
        }


        private void RecommendPdfArchitectWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            TranslationHelper.Instance.TranslatorInstance.Translate(this);
        }

        private void DownloadButton_OnClick(object sender, RoutedEventArgs e)
        {
            Process.Start(Urls.ArchitectDownloadUrl);
            DialogResult = true;
        }

        public static bool? ShowDialogTopMost(bool showViewerWarning)
        {
            var recommendPdfArchitectWindow = new RecommendPdfArchitectWindow(showViewerWarning);
            TopMostHelper.ShowDialogTopMost(recommendPdfArchitectWindow, false);
            return recommendPdfArchitectWindow.DialogResult;
        }
    }
}
