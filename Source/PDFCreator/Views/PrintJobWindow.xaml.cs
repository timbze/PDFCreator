using System;
using System.Windows;
using System.Windows.Input;
using pdfforge.GpoReader;
using pdfforge.PDFCreator.Assistants;
using pdfforge.PDFCreator.Core.Settings;
using pdfforge.PDFCreator.Helper;
using pdfforge.PDFCreator.Shared.Helper;
using pdfforge.PDFCreator.ViewModels;
using pdfforge.PDFCreator.WindowsApi;

namespace pdfforge.PDFCreator.Views
{
    internal partial class PrintJobWindow : Window
    {
        private PdfCreatorSettings _settings = SettingsHelper.Settings;
        private readonly GpoSettings _gpoSettings = SettingsHelper.GpoSettings;

        public PrintJobWindow()
        {
            InitializeComponent();
        }

        private void SettingsButton_OnClick(object sender, RoutedEventArgs e)
        {
            var vm = (PrintJobViewModel)DataContext;

            TopMostHelper.UndoTopMostWindow(this);
            _settings.ApplicationSettings.LastUsedProfileGuid = vm.SelectedProfile.Guid;

            var window = new ProfileSettingsWindow(_settings, _gpoSettings);
            if (window.ShowDialog() == true)
            {
                _settings = window.Settings;

                vm.Profiles = _settings.ConversionProfiles;
                vm.ApplicationSettings = _settings.ApplicationSettings;
                vm.SelectProfileByGuid(_settings.ApplicationSettings.LastUsedProfileGuid);
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            TranslationHelper.Instance.TranslatorInstance.Translate(this);

            if (Properties.Customization.ApplyCustomization.Equals("true", StringComparison.OrdinalIgnoreCase))
                Title = Properties.Customization.PrintJobWindowCaption;

            FlashWindow.Flash(this, 3);
        }

        private void CommandButtons_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void OnDragEnter(object sender, DragEventArgs e)
        {
            DragAndDropHelper.OnDragEnter(e);
        }

        private void OnDrop(object sender, DragEventArgs e)
        {
            DragAndDropHelper.OnDrop(e);
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F1)
                UserGuideHelper.ShowHelp(null, HelpTopic.CreatingPdf);
        }
    }
}
