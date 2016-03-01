using System;
using System.Windows;
using System.Windows.Controls;
using pdfforge.PDFCreator.Shared.Helper;
using pdfforge.PDFCreator.Shared.ViewModels.UserControls;

namespace pdfforge.PDFCreator.Shared.Views.UserControls
{
    public partial class ImageFormatsTab : UserControl
    {
        public ImageFormatsTab()
        {
            InitializeComponent();
            if (TranslationHelper.Instance.IsInitialized)
            {
                TranslationHelper.Instance.TranslatorInstance.Translate(this);
            }
        }

        public ImageFormatsTabViewModel ViewModel
        {
            get { return (ImageFormatsTabViewModel)DataContext; }
        }

        private void UIElement_OnLostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                int quality = Convert.ToInt32(JpegQualityTextBox.Text);
                if (quality < 1)
                    JpegQualityTextBox.Text = "1";
                if (quality > 100)
                    JpegQualityTextBox.Text = "100";
            }
            catch (Exception)
            {
                JpegQualityTextBox.Text = "75";
            }
        }
    }
}
