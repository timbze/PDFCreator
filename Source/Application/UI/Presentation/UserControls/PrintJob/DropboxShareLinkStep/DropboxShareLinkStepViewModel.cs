using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.UI.Presentation.Commands;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using pdfforge.PDFCreator.UI.Presentation.Workflow;
using System;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob
{
    public class DropboxShareLinkStepViewModel : TranslatableViewModelBase<DropboxTranslation>, IWorkflowViewModel
    {
        public ICommand OkCommand { get; set; }
        public ICommand CopyToClipboardCommand { get; set; }
        public ICommand UrlOpenCommand { get; }
        public string ShareUrl { get; set; }

        public DropboxShareLinkStepViewModel(ITranslationUpdater translationUpdater, ICommandLocator commandLocator) : base(translationUpdater)
        {
            CopyToClipboardCommand = commandLocator.GetCommand<CopyToClipboardCommand>();
            UrlOpenCommand = commandLocator.GetCommand<UrlOpenCommand>();
            OkCommand = new DelegateCommand(o => FinishInteraction());
        }

        public void ExecuteWorkflowStep(Job job)
        {
            ShareUrl = job.ShareLinks.DropboxShareUrl;
            RaisePropertyChanged(nameof(ShareUrl));
        }

        private void FinishInteraction()
        {
            StepFinished?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler StepFinished;
    }
}
