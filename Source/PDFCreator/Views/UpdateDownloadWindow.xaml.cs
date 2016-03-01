using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Windows;
using pdfforge.PDFCreator.Core.Printer;
using pdfforge.PDFCreator.Shared.Helper;
using pdfforge.PDFCreator.Utilities;

namespace pdfforge.PDFCreator.Views
{
    internal partial class UpdateDownloadWindow : Window
    {
        private string _downloadUrl;
        private DownloadSpeed _downloadSpeed;
        private DateTime _lastUpdate;
        private WebClient _webClient;

        private string _downloadLocation;
        public string DownloadedFile { get; private set; }

        public UpdateDownloadWindow(string downloadUrl)
        {
            InitializeComponent();

            TranslationHelper.Instance.TranslatorInstance.Translate(this);

            _downloadUrl = downloadUrl;
        }

        private void StartDownload(string downloadUrl)
        {
            _webClient = new WebClient();
            _webClient.DownloadProgressChanged += WebClientOnDownloadProgressChanged;
            _webClient.DownloadFileCompleted += WebClientOnDownloadFileCompleted;
            var uri = new Uri(downloadUrl);
            string filename = Path.GetFileName(uri.LocalPath);
            _downloadSpeed = new DownloadSpeed(_webClient);

            _downloadLocation = GetTempPath();
            Directory.CreateDirectory(_downloadLocation);
            _downloadLocation = Path.Combine(_downloadLocation, filename);
            _webClient.DownloadFileAsync(uri, _downloadLocation);
        }

        private string GetTempPath()
        {
            var portReader = new PrinterPortReader();
            var printerPort = portReader.ReadPrinterPort("pdfcmon");
            var tempFolderName = (printerPort == null) ? "PDFCreator" : printerPort.TempFolderName;

            var path = Path.Combine(Path.GetTempPath(), tempFolderName);
            return Path.Combine(path, "Temp");
        }

        private void WebClientOnDownloadFileCompleted(object sender, AsyncCompletedEventArgs asyncCompletedEventArgs)
        {
            bool success = asyncCompletedEventArgs.Error == null;

            if (success)
            {
                DownloadedFile = _downloadLocation;
            }
            else
            {
                File.Delete(_downloadLocation);
            }

            Action<bool> downloadFinished = DownloadFinished;
            Dispatcher.BeginInvoke(downloadFinished, success);
        }

        private void DownloadFinished(bool success)
        {
            UpdateProgress(100);
            DialogResult = success;
            Close();
        }

        private string GetReadableSpeed(double speed)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            int order = 0;
            while (speed >= 1024 && order + 1 < sizes.Length)
            {
                order++;
                speed = speed / 1024;
            }

            return String.Format("{0:0.00} {1}", speed, sizes[order]);
        }

        private void WebClientOnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs downloadProgressChangedEventArgs)
        {
            if (DateTime.Now - _lastUpdate < TimeSpan.FromMilliseconds(100))
                return;

            Action<int> action = UpdateProgress;
            Dispatcher.BeginInvoke(action, downloadProgressChangedEventArgs.ProgressPercentage);

            _lastUpdate = DateTime.Now;
        }

        private void UpdateProgress(int progressPercentage)
        {
            ProgressBar.Value = progressPercentage;
            DownloadSpeedText.Text = string.Format("{0}/s - {1:0} s",
                GetReadableSpeed(_downloadSpeed.BytesPerSecond),
                _downloadSpeed.EstimatedRemainingDuration.TotalSeconds);
            
            if (progressPercentage == 100)
                DownloadSpeedText.Visibility = Visibility.Hidden;
        }

        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (_webClient == null)
                return;

            _webClient.CancelAsync();
            DialogResult = false;
        }

        private void UpdateDownloadWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            StartDownload(_downloadUrl);
        }
    }
}
