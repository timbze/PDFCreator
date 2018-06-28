using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.UI.Presentation.Assistants;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using pdfforge.PDFCreator.UI.Presentation.Windows;
using pdfforge.PDFCreator.UI.Presentation.Workflow;
using pdfforge.PDFCreator.Utilities.Process;
using Prism.Events;
using System;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob.UpdateHint
{
    public class UpdateHintViewModel : TranslatableViewModelBase<UpdateManagerTranslation>, IWorkflowViewModel
    {
        private readonly IUpdateAssistant _updateAssistant;
        private readonly IProcessStarter _processStarter;
        private readonly IEventAggregator _eventAggregator;
        public string WhatsNewUrl { get; }
        public string AvailabeVersionText { get; }

        public UpdateHintViewModel(IUpdateAssistant updateAssistant, IProcessStarter processStarter,
            ITranslationUpdater translationUpdater, IEventAggregator eventAggregator) : base(translationUpdater)
        {
            _updateAssistant = updateAssistant;
            _processStarter = processStarter;
            _eventAggregator = eventAggregator;
            WhatsNewUrl = Urls.PDFCreatorWhatsNewUrl;
            AvailabeVersionText = Translation.GetNewUpdateMessage(updateAssistant.OnlineVersion.Version.ToString(3));
        }

        public ICommand InstallUpdateCommand => new DelegateCommand(o => InstallUpdate());
        public ICommand SkipVersionCommand => new DelegateCommand(o => SkipVersion());
        public ICommand AskLaterCommand => new DelegateCommand(o => UpdateLater());

        public ICommand WhatsNewCommand => new DelegateCommand(o => _processStarter.Start(WhatsNewUrl));

        private void InstallUpdate()
        {
            _updateAssistant.InstallNewUpdate();
            _eventAggregator.GetEvent<SetShowUpdateEvent>().Publish(false);
            FinishStep();
        }

        private void SkipVersion()
        {
            _updateAssistant.SkipVersion();
            _eventAggregator.GetEvent<SetShowUpdateEvent>().Publish(false);
            FinishStep();
        }

        private void UpdateLater()
        {
            _updateAssistant.SetNewUpdateTime();
            _eventAggregator.GetEvent<SetShowUpdateEvent>().Publish(false);
            FinishStep();
        }

        private void FinishStep()
        {
            StepFinished?.Invoke(this, EventArgs.Empty);
        }

        public void ExecuteWorkflowStep(Job job)
        {
        }

        public event EventHandler StepFinished;
    }
}
