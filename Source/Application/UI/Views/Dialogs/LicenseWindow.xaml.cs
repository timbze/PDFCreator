using System;
using System.Windows;
using pdfforge.PDFCreator.Core.Services.Licensing;
using pdfforge.PDFCreator.UI.ViewModels.DialogViewModels;
using pdfforge.PDFCreator.UI.ViewModels.UserControlViewModels.ApplicationSettings;

namespace pdfforge.PDFCreator.UI.Views.Dialogs
{
    /// <summary>
    ///     Interaction logic for LicenseWindow.xaml
    /// </summary>
    public partial class LicenseWindow : Window
    {
        public LicenseWindow(LicenseWindowViewModel licenseWindowViewModel)
        {
            DataContext = licenseWindowViewModel;
            InitializeComponent();
        }
    }
}