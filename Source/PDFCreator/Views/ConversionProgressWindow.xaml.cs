using System;
using System.Timers;
using System.Windows;
using pdfforge.PDFCreator.Core.Jobs;

namespace pdfforge.PDFCreator.Views
{
    internal partial class ConversionProgressWindow : Window
    {
        private Timer _timer = new Timer(500);
        public ConversionProgressWindow()
        {
            InitializeComponent();
            SetProgress(0);
        }

        public void ApplyJob(IJob job)
        {
            job.OnJobProgressChanged += job_OnJobProgressChanged;
            job.OnJobCompleted += job_OnJobCompleted;

            // Backup timer to close window if the event should not be fired for some reason
            _timer.Elapsed += (sender, args) => { if (job.Completed) job_OnJobCompleted(this, null); };
            _timer.Start();
        }

        void job_OnJobCompleted(object sender, JobCompletedEventArgs e)
        {
            Action<int> setProgress = SetProgress;
            Dispatcher.BeginInvoke(setProgress, 100);

            Action closeAction = Close;
            Dispatcher.BeginInvoke(closeAction);

            if (_timer != null)
                _timer.Enabled = false;
        }

        void job_OnJobProgressChanged(object sender, JobProgressChangedEventArgs e)
        {
            Action<int> setProgress = SetProgress;
            Dispatcher.BeginInvoke(setProgress, e.ProgressPercentage);
        }

        private void SetProgress(int progressPercentage)
        {
            ProgressPercentageText.Text = string.Format("{0}%", progressPercentage);
        }
    }
}
