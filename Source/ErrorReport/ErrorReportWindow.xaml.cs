using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;

namespace pdfforge.PDFCreator.ErrorReport
{
    public partial class ErrorReportWindow : Window
    {
        private Report _report = null;

        public ErrorReportWindow()
        {
            InitializeComponent();
        }

        public ErrorReportWindow(Report errorReport, bool allowTerminateApplication)
            : this()
        {
            _report = errorReport;
            ErrorDescriptionText.Text = errorReport.ComposeErrorText();

            if (!allowTerminateApplication)
                QuitButton.Visibility = Visibility.Collapsed;
        }

        private void QuitButton_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void ReportButton_OnClick(object sender, RoutedEventArgs e)
        {
            var tartaros = new TartarosClient();
            var success = tartaros.SendErrorReport(_report);

            if (success)
            {
                MessageBox.Show("Thank you, the error has been reported successfully.", "Error reported");
                Close();
            }
            else
            {
                MessageBox.Show("We are sorry, we have not been able to report the error. Please check your internet connection and retry to send the report.", "Error not reported", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
