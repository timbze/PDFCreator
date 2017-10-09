using MahApps.Metro.Controls;
using pdfforge.PDFCreator.UI.Presentation.Assistants;
using pdfforge.PDFCreator.UI.Presentation.Customization;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.ServiceLocator;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Interop;

namespace pdfforge.PDFCreator.UI.Presentation
{
    public partial class MainShell : MetroWindow, IWhitelisted
    {
        private MainShellViewModel _mainShellViewModel;
        public IUpdateAssistant UpdateAssistant { get; }

        public MainShellViewModel ViewModel => (MainShellViewModel)DataContext;

        public MainShell(MainShellViewModel vm, IHightlightColorRegistration hightlightColorRegistration, IUpdateAssistant updateAssistant, ViewCustomization viewCustomization)
        {
            _mainShellViewModel = vm;
            UpdateAssistant = updateAssistant;
            DataContext = _mainShellViewModel;
            _mainShellViewModel.Init(Close);
            InitializeComponent();
            hightlightColorRegistration.RegisterHighlightColorResource(this);

            if (viewCustomization.ApplyCustomization)
            {
                Title = Title + " " + viewCustomization.MainWindowText;
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = !_mainShellViewModel.CanClose();
            base.OnClosing(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            _mainShellViewModel.OnClosed();
            base.OnClosed(e);
        }

        public override void EndInit()
        {
            base.EndInit();
            (DataContext as MainShellViewModel)?.PublishMainShellDone();
        }

        private void MainShell_OnLoaded(object sender, RoutedEventArgs e)
        {
            var desktopSize = System.Windows.Forms.Screen.FromHandle(new WindowInteropHelper(this).Handle).WorkingArea;
            if (desktopSize.Width < Width || desktopSize.Height < Height)
                WindowState = WindowState.Maximized;
        }
    }
}
