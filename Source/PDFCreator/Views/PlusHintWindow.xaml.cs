using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using pdfforge.PDFCreator.Helper;
using pdfforge.PDFCreator.Shared.Helper;
using pdfforge.PDFCreator.Threading;
using pdfforge.PDFCreator.Utilities.Threading;

namespace pdfforge.PDFCreator.Views
{
    public partial class PlusHintWindow : Window
    {
        public PlusHintWindow()
        {
            InitializeComponent();
            ThankYou.Text = TranslationHelper.Instance.TranslatorInstance.GetFormattedTranslation("PlusHintWindow", "ThankYou", "You have already converted {0} files with PDFCreator. Thank you very much!", CurrentCount);
        }

        private static int CurrentCount { get; set; }

        public static void ShowTopMost(int currentCount)
        {
            CurrentCount = currentCount;
            var thread = new SynchronizedThread(ShowPlusHintWindow) {Name = "PlusHintThread"};
            thread.SetApartmentState(ApartmentState.STA);

            ThreadManager.Instance.StartSynchronizedThread(thread);
        }

        private static void ShowPlusHintWindow()
        {
            var welcomeWindow = new PlusHintWindow();
            TopMostHelper.ShowDialogTopMost(welcomeWindow, true);
        }

        private void PlusButton_OnClick(object sender, RoutedEventArgs e)
        {
            ShowUrlInBrowser(Urls.PlusHintLink);
            Close();
        }

        private void ShowUrlInBrowser(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch (Win32Exception)
            {
            }
            catch (FileNotFoundException)
            {
            }
        }

        private void PlusHintWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            TranslationHelper.Instance.TranslatorInstance.Translate(this);
        }
    }
}