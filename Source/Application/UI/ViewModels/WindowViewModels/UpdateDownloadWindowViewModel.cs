using System;
using System.ComponentModel;
using System.Net;
using System.Windows.Input;
using System.Windows.Threading;
using SystemInterface.IO;
using SystemWrapper.IO;
using pdfforge.Obsidian;
using pdfforge.Obsidian.Interaction;
using pdfforge.PDFCreator.Conversion.Jobs.FolderProvider;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.ViewModels.WindowViewModels.Translations;
using pdfforge.PDFCreator.Utilities;

namespace pdfforge.PDFCreator.UI.ViewModels.WindowViewModels
{
    public class UpdateDownloadWindowViewModel : InteractionAwareViewModelBase<UpdateDownloadInteraction>
    {
        private readonly IDirectory _directory;
        private readonly Dispatcher _dispatcher;
        private readonly IFile _file;
        private readonly IPathSafe _pathSafe = new PathWrapSafe();
        private readonly ITempFolderProvider _tempFolderProvider;
        private string _downloadLocation;
        private DownloadSpeed _downloadSpeed;
        private DateTime _lastUpdate;
        private WebClient _webClient;

        public UpdateDownloadWindowViewModel(IDirectory directory, IFile file, ITempFolderProvider tempFolderProvider, UpdateDownloadWindowTranslation translation)
        {
            Translation = translation;
            _directory = directory;
            _file = file;
            _tempFolderProvider = tempFolderProvider;
            _dispatcher = Dispatcher.CurrentDispatcher;

            CancelCommand = new DelegateCommand(ExecuteCancel);
        }

        public int ProgressPercentage { get; protected set; }

        public string DownloadSpeedText { get; protected set; }

        public ICommand CancelCommand { get; }
        public UpdateDownloadWindowTranslation Translation { get; }

        protected override void HandleInteractionObjectChanged()
        {
            StartDownload(Interaction.DownloadUrl);
        }

        private void StartDownload(string downloadUrl)
        {
            _webClient = new WebClient();
            _webClient.DownloadProgressChanged += WebClientOnDownloadProgressChanged;
            _webClient.DownloadFileCompleted += WebClientOnDownloadFileCompleted;
            var uri = new Uri(downloadUrl);
            var filename = _pathSafe.GetFileName(uri.LocalPath);
            _downloadSpeed = new DownloadSpeed(_webClient);

            _downloadLocation = _tempFolderProvider.TempFolder;
            _directory.CreateDirectory(_downloadLocation);
            _downloadLocation = _pathSafe.Combine(_downloadLocation, filename);
            _webClient.DownloadFileAsync(uri, _downloadLocation);
        }

        private void WebClientOnDownloadFileCompleted(object sender, AsyncCompletedEventArgs asyncCompletedEventArgs)
        {
            var success = asyncCompletedEventArgs.Error == null;

            if (success)
            {
                Interaction.DownloadedFile = _downloadLocation;
            }
            else
            {
                _file.Delete(_downloadLocation);
            }

            Action<bool> downloadFinished = DownloadFinished;
            _dispatcher.BeginInvoke(downloadFinished, success);
        }

        private void ExecuteCancel(object o)
        {
            _webClient?.CancelAsync();
            Interaction.Success = false;
            FinishInteraction();
        }

        private void DownloadFinished(bool success)
        {
            UpdateProgress(100);
            Interaction.Success = true;
            FinishInteraction();
        }

        private string GetReadableSpeed(double speed)
        {
            string[] sizes = {"B", "KB", "MB", "GB"};
            var order = 0;
            while (speed >= 1024 && order + 1 < sizes.Length)
            {
                order++;
                speed = speed/1024;
            }

            return $"{speed:0.00} {sizes[order]}";
        }

        private void WebClientOnDownloadProgressChanged(object sender,
            DownloadProgressChangedEventArgs downloadProgressChangedEventArgs)
        {
            if (DateTime.Now - _lastUpdate < TimeSpan.FromMilliseconds(100))
                return;

            Action<int> action = UpdateProgress;
            _dispatcher.BeginInvoke(action, downloadProgressChangedEventArgs.ProgressPercentage);

            _lastUpdate = DateTime.Now;
        }

        private void UpdateProgress(int progressPercentage)
        {
            ProgressPercentage = progressPercentage;
            RaisePropertyChanged(nameof(ProgressPercentage));

            DownloadSpeedText = string.Format("{0}/s - {1:0} s",
                GetReadableSpeed(_downloadSpeed.BytesPerSecond),
                _downloadSpeed.EstimatedRemainingDuration.TotalSeconds);

            if (progressPercentage == 100)
                DownloadSpeedText = "";

            RaisePropertyChanged(nameof(DownloadSpeedText));
        }
    }
}