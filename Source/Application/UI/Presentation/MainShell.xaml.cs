using MahApps.Metro.Controls;
using pdfforge.PDFCreator.Core.Services.Macros;
using pdfforge.PDFCreator.UI.Presentation.Assistants;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.ServiceLocator;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace pdfforge.PDFCreator.UI.Presentation
{
    public partial class MainShell : MetroWindow, IWhitelisted
    {
        private MainShellViewModel _mainShellViewModel;
        private bool _skipSettingsCheck;
        public IUpdateAssistant UpdateAssistant { get; }

        public MainShellViewModel ViewModel => (MainShellViewModel)DataContext;

        public MainShell(MainShellViewModel vm, IHightlightColorRegistration hightlightColorRegistration, IUpdateAssistant updateAssistant)
        {
            _mainShellViewModel = vm;
            UpdateAssistant = updateAssistant;
            DataContext = _mainShellViewModel;
            _mainShellViewModel.Init(Close);
            InitializeComponent();
            hightlightColorRegistration.RegisterHighlightColorResource(this);
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

            FocusManager.SetFocusedElement(this, HomeButton);
        }

        private async void MainShell_OnClosing(object sender, CancelEventArgs e)
        {
            if (_skipSettingsCheck)
            {
                return;
            }

            e.Cancel = true;
            _skipSettingsCheck = false;
            var result = await _mainShellViewModel.CloseCommand.ExecuteAsync(null);

            if (result == ResponseStatus.Success)
            {
                _skipSettingsCheck = true;
                // Invoke required because we can't call Close during the closing event
                Dispatcher.BeginInvoke(new Action(Close));
            }
        }
    }
}
