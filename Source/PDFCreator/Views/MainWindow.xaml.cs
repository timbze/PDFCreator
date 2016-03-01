using System;
using System.Windows;
using System.Windows.Input;
using pdfforge.PDFCreator.Core.Settings;
using pdfforge.PDFCreator.Helper;
using pdfforge.PDFCreator.Shared.Helper;
using pdfforge.PDFCreator.Shared.Helper.Logging;
using pdfforge.PDFCreator.Shared.Licensing;

namespace pdfforge.PDFCreator.Views
{
    internal partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            ApplicationSettingsPanel.IsEnabled = !SettingsHelper.GpoSettings.DisableApplicationSettings;
        }

        private void AppSettingsButton_OnClick(object sender, RoutedEventArgs e)
        {
            PdfCreatorSettings settings = SettingsHelper.Settings;
            string currentLanguage = settings.ApplicationSettings.Language;

            ApplicationSettings tmpSettings = settings.ApplicationSettings.Copy();
            var tmpProperties = settings.ApplicationProperties.Copy();
            var window = new ApplicationSettingsWindow(tmpSettings, SettingsHelper.GpoSettings, tmpProperties, settings.ConversionProfiles);

            if (window.ShowDialog() == true)
            {
                settings.ApplicationSettings = tmpSettings;
                settings.ApplicationProperties = tmpProperties;
                if (!string.Equals(currentLanguage, settings.ApplicationSettings.Language, StringComparison.OrdinalIgnoreCase))
                {
                    TranslationHelper.Instance.InitTranslator(settings.ApplicationSettings.Language);
                    TranslationHelper.Instance.TranslatorInstance.Translate(this);
                }
                SettingsHelper.SaveSettings();
            }
            //Translation of profiles are stored in their name property and could have been changed in the AppSettingsWindow
            //To include the current language it must be translated here 
            TranslationHelper.Instance.TranslateProfileList(SettingsHelper.Settings.ConversionProfiles);

            LoggingHelper.ChangeLogLevel(settings.ApplicationSettings.LoggingLevel);
        }

        private void ProfileSettingsButton_OnClick(object sender, RoutedEventArgs e)
        {
            var window = new ProfileSettingsWindow(SettingsHelper.Settings, SettingsHelper.GpoSettings);
            window.ShowDialog();
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            TranslationHelper.Instance.TranslatorInstance.Translate(this);

            ApplicationNameText.Text = "PDFCreator " + VersionHelper.Instance.FormatWithTwoDigits();

            // Apply company name for customized setups
            ApplyCustomization();
           
            if (!EditionFactory.Instance.Edition.ShowWelcomeWindow)
                return;

            var welcomeSettingsHelper = new WelcomeSettingsHelper();
            if (welcomeSettingsHelper.IsFirstRun())
            {
                welcomeSettingsHelper.SetCurrentApplicationVersionAsWelcomeVersionInRegistry();
                WelcomeWindow.ShowDialogTopMost();
            }
            else
            {
                var plushelper = new PlusHintHelper();
                if (plushelper.DisplayHint())
                {
                    PlusHintWindow.ShowTopMost(plushelper.CurrentJobCounter);
                }
            }
        }

        private void ApplyCustomization()
        {
            if (Properties.Customization.ApplyCustomization.Equals("true", StringComparison.OrdinalIgnoreCase))
            {
                LicensedForText.Visibility = Visibility.Visible;
                LicensedForText.Text = Properties.Customization.MainForm;
            }
            else
            {
                LicensedForText.Visibility = Visibility.Hidden;
            }
        }

        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            var w = new AboutWindow();
            w.ShowDialog();
        }

        private void MainWindow_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F1)
            {
                UserGuideHelper.ShowHelp(HelpTopic.General);
            }
        }

        private void MainWindow_OnDragEnter(object sender, DragEventArgs e)
        {
            DragAndDropHelper.OnDragEnter(e);
        }

        private void MainWindow_OnDrop(object sender, DragEventArgs e)
        {
            DragAndDropHelper.OnDrop(e);
        }

        public static bool? ShowDialogTopMost()
        {
            var w = new MainWindow();
            return TopMostHelper.ShowDialogTopMost(w, true);
        }
    }
}
