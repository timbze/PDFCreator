using System;
using System.Windows.Threading;
using pdfforge.Obsidian;
using pdfforge.Obsidian.Interaction;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.UI.Interactions;

namespace pdfforge.PDFCreator.UI.ViewModels.WindowViewModels
{
    public class ConversionProgressWindowViewModel : InteractionAwareViewModelBase<ConversionProgressInteraction>
    {
        private readonly Dispatcher _currentThreadDispatcher;

        public ConversionProgressWindowViewModel()
        {
            _currentThreadDispatcher = Dispatcher.CurrentDispatcher;
            OnClosing = new DelegateCommand(OnClosingExecute);
            SetProgressText(0);
        }

        public string ProgressText { get; set; }

        public DelegateCommand OnClosing { get; set; }

        private void OnClosingExecute(object obj)
        {
            Interaction.Job.OnJobProgressChanged -= OnJobProgressChanged;
            Interaction.Job.OnJobCompleted -= OnJobCompleted;
        }

        protected override void HandleInteractionObjectChanged()
        {
            Interaction.Job.OnJobProgressChanged += OnJobProgressChanged;
            Interaction.Job.OnJobCompleted += OnJobCompleted;
        }

        private void SetProgressText(int progress)
        {
            ProgressText = $"{progress}%";
            RaisePropertyChanged(nameof(ProgressText));
        }

        private void OnJobProgressChanged(object sender, JobProgressChangedEventArgs e)
        {
            Action<int> setProgress = SetProgressText;
            _currentThreadDispatcher.BeginInvoke(setProgress, e.ProgressPercentage);
        }

        private void OnJobCompleted(object sender, JobCompletedEventArgs e)
        {
            Action<int> setProgress = SetProgressText;
            _currentThreadDispatcher.BeginInvoke(setProgress, 100);

            _currentThreadDispatcher.BeginInvoke(new Action(() => FinishInteraction()));
        }
    }
}