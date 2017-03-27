using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using pdfforge.PDFCreator.UI.ViewModels.WindowViewModels;

namespace pdfforge.PDFCreator.UI.Views.Windows
{
    public partial class AboutWindow : Window
    {
        public AboutWindow(AboutWindowViewModel aboutWindowViewModel, ViewCustomization customization)
        {
            DataContext = aboutWindowViewModel;
            InitializeComponent();
            ApplyCustomization(customization);
        }

        private void ApplyCustomization(ViewCustomization customization)
        {
            if (customization.ApplyCustomization)
            {
                CustomizationPanel.Visibility = Visibility.Visible;
                CustomText.Text = customization.AboutDialogText;
                CustomImage.Source = ConvertToBitmapSource(customization.CustomLogo);
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
                using (var memory = new MemoryStream())
                {
                    image.Save(memory, ImageFormat.Png);
                    memory.Position = 0;
                    var bitmapImage = new BitmapImage();
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
    }
}