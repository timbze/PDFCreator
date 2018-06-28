using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Jobs.FolderProvider;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using pdfforge.PDFCreator.Utilities;
using System;
using System.ComponentModel;
using System.Net;
using System.Windows.Input;
using System.Windows.Threading;
using SystemInterface.IO;
using SystemWrapper.IO;

namespace pdfforge.PDFCreator.UI.Presentation.Windows
{
    public class UpdateDownloadWindowViewModel : OverlayViewModelBase<UpdateDownloadInteraction, UpdateDownloadWindowTranslation>
    {
        private readonly IDirectory _directory;
        private readonly Dispatcher _dispatcher;
        private readonly IFile _file;
        private readonly IPathSafe _pathSafe = new PathWrapSafe();
        private readonly ITempFolderProvider _tempFolderProvider;
        private readonly IReadableFileSizeFormatter _readableFileSizeFormatter;
        private string _downloadLocation;
        private DownloadSpeed _downloadSpeed;
        private DateTime _lastUpdate;
        private WebClient _webClient;

        public UpdateDownloadWindowViewModel(IDirectory directory, IFile file, ITempFolderProvider tempFolderProvider, ITranslationUpdater translationUpdater, IReadableFileSizeFormatter readableFileSizeFormatter)
            : base(translationUpdater)
        {
            _directory = directory;
            _file = file;
            _tempFolderProvider = tempFolderProvider;
            _readableFileSizeFormatter = readableFileSizeFormatter;
            _dispatcher = Dispatcher.CurrentDispatcher;

            CancelCommand = new DelegateCommand(ExecuteCancel);
        }

        public int ProgressPercentage { get; protected set; }

        public string DownloadSpeedText { get; protected set; }

        public ICommand CancelCommand { get; }

        protected override void HandleInteractionObjectChanged()
        {
            StartDownload(Interaction.DownloadUrl);
        }

        public override string Title => Translation.Title;

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
                _readableFileSizeFormatter.GetFileSizeString(_downloadSpeed.BytesPerSecond),
                _downloadSpeed.EstimatedRemainingDuration.TotalSeconds);

            if (progressPercentage == 100)
                DownloadSpeedText = "";

            RaisePropertyChanged(nameof(DownloadSpeedText));
        }
    }
}
