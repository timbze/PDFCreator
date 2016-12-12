using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using pdfforge.DynamicTranslator;
using pdfforge.Obsidian;
using pdfforge.Obsidian.Interaction;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.Core.Workflow;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.ViewModels.Helper;

namespace pdfforge.PDFCreator.UI.ViewModels.WindowViewModels
{
    public class PrintJobViewModel : InteractionAwareViewModelBase<PrintJobInteraction>
    {
        private readonly Dispatcher _currentThreadDispatcher;
        private readonly DragAndDropEventHandler _dragAndDrop;
        private readonly IInteractionInvoker _interactionInvoker;
        private readonly IJobInfoQueue _jobInfoQueue;
        private readonly ISettingsManager _settingsManager;
        private readonly ISettingsProvider _settingsProvider;
        private readonly ITranslator _translator;
        private readonly IUserGuideHelper _userGuideHelper;
        private ApplicationSettings _applicationSettings;
        private JobInfo _jobInfo;
        private ConversionProfile _preselectedProfile;
        private IList<ConversionProfile> _profiles;

        public PrintJobViewModel(ISettingsManager settingsManager, IJobInfoQueue jobInfoQueue, ITranslator translator, DragAndDropEventHandler dragAndDrop, IInteractionInvoker interactionInvoker, IUserGuideHelper userGuideHelper, ApplicationNameProvider applicationNameProvider)
        {
            Title = applicationNameProvider.ApplicationName;

            _currentThreadDispatcher = Dispatcher.CurrentDispatcher;
            _translator = translator;
            _dragAndDrop = dragAndDrop;
            _interactionInvoker = interactionInvoker;
            _userGuideHelper = userGuideHelper;
            _settingsManager = settingsManager;
            _jobInfoQueue = jobInfoQueue;

            _jobInfoQueue.OnNewJobInfo += OnNewJobInfo;

            _settingsProvider = settingsManager.GetSettingsProvider();

            SaveCommand = new DelegateCommand(ExecuteSave);
            EmailCommand = new DelegateCommand(ExecuteMail);
            ManagePrintJobsCommand = new DelegateCommand(ExecuteManagePrintJobs);
            ShowSettingsCommand = new DelegateCommand(ExecuteShowSettings);
            WindowClosingCommand = new DelegateCommand(OnWindowClosing);
            DragEnterCommand = new DelegateCommand<DragEventArgs>(OnDragEnter);
            DropCommand = new DelegateCommand<DragEventArgs>(OnDrop);
            KeyDownCommand = new DelegateCommand<KeyEventArgs>(OnKeyDown);
        }

        public ICollectionView ProfilesView { get; private set; }
        public Metadata Metadata { get; set; }

        public DelegateCommand SaveCommand { get; private set; }
        public DelegateCommand EmailCommand { get; private set; }
        public DelegateCommand ManagePrintJobsCommand { get; }
        public DelegateCommand ShowSettingsCommand { get; }
        public DelegateCommand WindowClosingCommand { get; }
        public DelegateCommand<DragEventArgs> DragEnterCommand { get; }
        public DelegateCommand<DragEventArgs> DropCommand { get; }
        public DelegateCommand<KeyEventArgs> KeyDownCommand { get; }

        public JobInfo JobInfo
        {
            get { return _jobInfo; }
            private set
            {
                _jobInfo = value;
                RaisePropertyChanged(nameof(JobInfo));
                if (_jobInfo == null)
                    return;

                Metadata = _jobInfo.Metadata.Copy();
                RaisePropertyChanged(nameof(Metadata));
            }
        }

        public ConversionProfile SelectedProfile
        {
            get { return (ConversionProfile) ProfilesView.CurrentItem; }
        }

        public IList<ConversionProfile> Profiles
        {
            get { return _profiles; }
            set
            {
                _profiles = value;
                ProfilesView = CollectionViewSource.GetDefaultView(_profiles);
                ProfilesView.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
                RaisePropertyChanged(nameof(Profiles));
                RaisePropertyChanged(nameof(ProfilesView));
            }
        }

        public ApplicationSettings ApplicationSettings
        {
            get { return _applicationSettings; }
            set
            {
                _applicationSettings = value;
                RaisePropertyChanged(nameof(ApplicationSettings));
                if (_preselectedProfile != null)
                    SelectProfileByGuid(_preselectedProfile.Guid);
                else
                    SelectProfileByGuid(value.LastUsedProfileGuid);
            }
        }

        public string PendingJobsText
        {
            get
            {
                var additionalJobs = _jobInfoQueue.Count - 1;
                if (additionalJobs == 1)
                    return _translator.GetTranslation("PrintJobWindow", "OneMoreJobWaiting");

                if (additionalJobs > 1)
                    return _translator.GetFormattedTranslation("PrintJobWindow", "MoreJobsWaiting", additionalJobs);

                return _translator.GetTranslation("PrintJobWindow", "NoJobsWaiting");
            }
        }

        public string Title { get; }

        protected override void HandleInteractionObjectChanged()
        {
            Profiles = _settingsProvider.Settings.ConversionProfiles;
            _preselectedProfile = Interaction.Profile;

            //must be set before ApplicationSettings because it is evaluated in the Set method of appsettings.
            ApplicationSettings = _settingsProvider.Settings.ApplicationSettings;

            JobInfo = Interaction.JobInfo;
        }

        private void ExecuteShowSettings(object obj)
        {
            _settingsProvider.Settings.ApplicationSettings.LastUsedProfileGuid = SelectedProfile.Guid;

            var interaction = new ProfileSettingsInteraction(_settingsProvider.Settings.Copy(), _settingsProvider.GpoSettings);
            _interactionInvoker.Invoke(interaction);

            if (!interaction.ApplySettings)
                return;

            ApplicationSettings = interaction.Settings.ApplicationSettings;
            Profiles = interaction.Settings.ConversionProfiles;
            SelectProfileByGuid(ApplicationSettings.LastUsedProfileGuid);

            _settingsManager.ApplyAndSaveSettings(interaction.Settings);
        }

        public void SelectProfileByGuid(string guid)
        {
            foreach (var conversionProfile in Profiles)
            {
                if (conversionProfile.Guid == guid)
                    ProfilesView.MoveCurrentTo(conversionProfile);
            }
        }

        private void ExecuteSave(object obj)
        {
            JobInfo.Metadata = Metadata;
            ApplicationSettings.LastUsedProfileGuid = SelectedProfile.Guid;
            Interaction.PrintJobAction = PrintJobAction.Save;
            Interaction.Profile = SelectedProfile;
            FinishInteraction();
        }

        private void ExecuteMail(object obj)
        {
            JobInfo.Metadata = Metadata;

            ApplicationSettings.LastUsedProfileGuid = SelectedProfile.Guid;
            Interaction.PrintJobAction = PrintJobAction.EMail;
            FinishInteraction();
        }

        private void ExecuteManagePrintJobs(object obj)
        {
            Interaction.PrintJobAction = PrintJobAction.ManagePrintJobs;
            FinishInteraction();
        }

        private void OnNewJobInfo(object sender, NewJobInfoEventArgs e)
        {
            Action updatePendingJobs = () => RaisePropertyChanged(nameof(PendingJobsText));

            _currentThreadDispatcher.Invoke(updatePendingJobs);
        }

        private void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.F1)
                _userGuideHelper.ShowHelp(HelpTopic.CreatingPdf);
        }

        private void OnDrop(DragEventArgs e)
        {
            _dragAndDrop.HandleDropEvent(e);
        }

        private void OnDragEnter(DragEventArgs e)
        {
            _dragAndDrop.HandleDragEnter(e);
        }

        private void OnWindowClosing(object obj)
        {
            _jobInfoQueue.OnNewJobInfo -= OnNewJobInfo;
        }
    }
}