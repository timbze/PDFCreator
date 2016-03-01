using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using pdfforge.LicenseValidator;
using pdfforge.PDFCreator.Shared.Helper;
using pdfforge.PDFCreator.Shared.ViewModels;

namespace pdfforge.PDFCreator.Shared.Views
{
    /// <summary>
    /// Interaction logic for OfflineActivationWindow.xaml
    /// </summary>
    public partial class OfflineActivationWindow : Window
    {
        public OfflineActivationWindow()
        {
            InitializeComponent();
            TranslationHelper.Instance.TranslatorInstance.Translate(this);
        }

        public OfflineActivationWindow(OfflineActivationViewModel viewModel)
            :this()
        {
            DataContext = viewModel;
        }

        private void OkButton_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void Hyperlink_OnClick(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo(Urls.OfflineActivationUrl));
            e.Handled = true;
        }

        private void OfflineActivationWindow_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F1)
                UserGuideHelper.ShowHelp(HelpTopic.AppLicense);
        }
    }
}
