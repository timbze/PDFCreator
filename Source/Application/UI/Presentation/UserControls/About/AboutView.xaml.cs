using pdfforge.PDFCreator.UI.Presentation.Customization;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls
{
    public partial class AboutView : UserControl
    {
        public AboutView(AboutViewModel viewModel, ViewCustomization viewCustomization)
        {
            DataContext = viewModel;
            InitializeComponent();

            if (viewCustomization.CustomizationEnabled && viewCustomization.CustomLogo != null)
            {
                using (MemoryStream memory = new MemoryStream())
                {
                    viewCustomization.CustomLogo.Save(memory, ImageFormat.Png);
                    memory.Position = 0;
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = memory;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();

                    CustomLogo.Source = bitmapImage;
                }
            }
        }
    }
}
