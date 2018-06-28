using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Core.Workflow;
using pdfforge.PDFCreator.Core.Workflow.Output;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Overlay.Password;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using pdfforge.PDFCreator.UI.Presentation.Workflow;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob
{
    public class ProgressViewModel : TranslatableViewModelBase<ProgressViewTranslation>, IWorkflowViewModel
    {
        private readonly IJobRunner _jobRunner;
        private readonly IProfileChecker _profileChecker;
        private readonly IInteractionRequest _interactionRequest;
        private readonly IDispatcher _dispatcher;
        private readonly InteractiveOutputFileMover _outputFileMover;
        private readonly AutoResetEvent _finishedEvent = new AutoResetEvent(false);
        private PasswordOverlayTranslation _passwordOverlayTranslation;

        public int ProgressPercentage { get; set; } = 0;

        public ProgressViewModel(IJobRunner jobRunner, IProfileChecker profileChecker, IInteractionRequest interactionRequest, IDispatcher dispatcher,
            ITranslationUpdater translationUpdater, InteractiveOutputFileMover outputFileMover)
            : base(translationUpdater)
        {
            _jobRunner = jobRunner;
            _profileChecker = profileChecker;
            _interactionRequest = interactionRequest;
            _dispatcher = dispatcher;
            _outputFileMover = outputFileMover;
            translationUpdater.RegisterAndSetTranslation(tf => _passwordOverlayTranslation = tf.UpdateOrCreateTranslation(_passwordOverlayTranslation));
        }

        public async void ExecuteWorkflowStep(Job job)
        {
            ProgressPercentage = 0;

            await Task.Run(() =>
            {
                try
                {
                    job.OnJobCompleted += OnJobCompleted;
                    job.OnJobProgressChanged += OnJobProgressChanged;

                    job.OnJobHasError += OnAnErrorOccurredInJob;

                    _jobRunner.RunJob(job, _outputFileMover);

                    _finishedEvent.WaitOne();

                    StepFinished?.Invoke(this, EventArgs.Empty);
                }
                finally
                {
                    job.OnJobCompleted -= OnJobCompleted;
                    job.OnJobProgressChanged -= OnJobProgressChanged;
                    job.OnJobHasError -= OnAnErrorOccurredInJob;
                }
            });
        }

        private void OnAnErrorOccurredInJob(object sender, JobLoginFailedEventArgs args)
        {
            var interactionFinishedEvent = new ManualResetEventSlim(false);

            _dispatcher.InvokeAsync(() => RaisePasswordOverlayInteraction(args, interactionFinishedEvent));

            interactionFinishedEvent.Wait();
        }

        private void RaisePasswordOverlayInteraction(JobLoginFailedEventArgs args, ManualResetEventSlim interactionFinishedEvent)
        {
            var invalidPasswordMessage = _passwordOverlayTranslation.FormatInvalidPasswordMessage(args.ActionDisplayName);
            var interaction = new PasswordOverlayInteraction(PasswordMiddleButton.None, _passwordOverlayTranslation.ReenterPassword, invalidPasswordMessage, false);

            _interactionRequest.Raise(interaction, delegate (PasswordOverlayInteraction overlayInteraction)
            {
                if (overlayInteraction.Result == PasswordResult.StorePassword)
                {
                    args.ContinueAction(interaction.Password);
                }
                else
                {
                    args.AbortAction(LoginQueryResult.AbortedByUser);
                }
                interactionFinishedEvent.Set();
            });
        }

        public event EventHandler StepFinished;

        private void OnJobCompleted(object sender, JobCompletedEventArgs args)
        {
            _finishedEvent.Set();
        }

        private void OnJobProgressChanged(object sender, JobProgressChangedEventArgs args)
        {
            ProgressPercentage = args.ProgressPercentage;
            RaisePropertyChanged(nameof(ProgressPercentage));
        }
    }
}
