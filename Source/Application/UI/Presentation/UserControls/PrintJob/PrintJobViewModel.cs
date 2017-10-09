using pdfforge.Obsidian;
using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Conversion.Jobs.FolderProvider;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.Services.Macros;
using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.Core.Workflow;
using pdfforge.PDFCreator.Core.Workflow.Exceptions;
using pdfforge.PDFCreator.Core.Workflow.Queries;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.Presentation.Commands.ProfileCommands;
using pdfforge.PDFCreator.UI.Presentation.Events;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using pdfforge.PDFCreator.UI.Presentation.Workflow;
using pdfforge.PDFCreator.Utilities;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Input;
using SystemInterface.IO;
using SystemWrapper.IO;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob
{
    public class PrintJobViewModel : TranslatableViewModelBase<PrintJobViewTranslation>, IWorkflowViewModel
    {
        public IGpoSettings GpoSettings { get; }
        private readonly ISettingsProvider _settingsProvider;
        private readonly IFileNameQuery _saveFileQuery;
        private readonly IProfileChecker _profileChecker;
        private readonly IInteractionRequest _interactionRequest;
        private readonly ErrorCodeInterpreter _errorCodeInterpreter;
        private readonly ISelectedProfileProvider _selectedProfileProvider;
        private readonly IFile _file;
        private readonly ITempFolderProvider _tempFolderProvider;
        private readonly IPathUtil _pathUtil;
        private PathWrapSafe _pathSafe = new PathWrapSafe();
        private Job _job;
        private string _outputFolder = "";
        private string _outputFilename = "";
        private string _latestDialogFilePath = "";

        public PrintJobViewModel(
            ISettingsProvider settingsProvider,
            ITranslationUpdater translationUpdater,
            IJobInfoQueue jobInfoQueue,
            IFileNameQuery saveFileQuery,
            IInteractionRequest interactionRequest,
            IProfileChecker profileChecker,
            ErrorCodeInterpreter errorCodeInterpreter,
            ICommandLocator commandsLocator,
            IEventAggregator eventAggregator,
            ISelectedProfileProvider selectedProfileProvider,
            ITempFolderProvider tempFolderProvider,
            IPathUtil pathUtil,
            IFile file,
            IGpoSettings gpoSettings)
            : base(translationUpdater)
        {
            GpoSettings = gpoSettings;
            _settingsProvider = settingsProvider;
            _saveFileQuery = saveFileQuery;
            _profileChecker = profileChecker;
            _interactionRequest = interactionRequest;
            _errorCodeInterpreter = errorCodeInterpreter;
            _selectedProfileProvider = selectedProfileProvider;
            _file = file;
            _tempFolderProvider = tempFolderProvider;
            _pathUtil = pathUtil;

            SaveCommand = new DelegateCommand(SaveExecute);
            SendByEmailCommand = new DelegateCommand(EmailExecute);
            MergeCommand = new DelegateCommand(MergeExecute);
            CancelCommand = new DelegateCommand(CancelExecute);
            SetOutputFormatCommand = new DelegateCommand<OutputFormat>(SetOutputFormatExecute);
            BrowseFileCommand = new DelegateCommand(BrowseFileExecute);

            SetupEditProfileCommand(commandsLocator, eventAggregator);

            var settings = settingsProvider.Settings?.CopyAndPreserveApplicationSettings();
            Profiles = settings?.ConversionProfiles;

            jobInfoQueue.OnNewJobInfo += (sender, args) => UpdateNumberOfPrintJobsHint(jobInfoQueue.Count);
            UpdateNumberOfPrintJobsHint(jobInfoQueue.Count);

            var profileListView = CollectionViewSource.GetDefaultView(Profiles);
            if (profileListView != null)
            {
                profileListView.CurrentChanged += (sender, args) =>
                {
                    if (Job != null)
                        OutputFormat = ((ConversionProfile)profileListView.CurrentItem).OutputFormat;
                };
            }
        }

        public ConversionProfile SelectedProfile
        {
            get { return Job?.Profile; }
            set
            {
                if (Job == null)
                    return;
                Job.Profile = value; RaisePropertyChanged();
            }
        }

        private void SetupEditProfileCommand(ICommandLocator commandsLocator, IEventAggregator eventAggregator)
        {
            if (commandsLocator != null)
            {
                EditProfileCommand = new MacroCommand(commandsLocator)
                    .AddCommand(new DelegateCommand(SetProfileForSelection))
                    .AddCommand<ShowLockLayerCommand>()
                    .AddCommand<OpenProfileCommand>()
                    .AddCommand<WaitProfileModificationCommand>()
                    .AddCommand(new DelegateCommand(x => eventAggregator.GetEvent<CloseMainWindowEvent>().Publish()))
                    .AddCommand(new DelegateCommand(UpdateJobProfileFromCurrentProperties))
                    .AddCommand<HideLockLayerCommand>();
            }
        }

        private void UpdateJobProfileFromCurrentProperties(object obj)
        {
            var settings = _settingsProvider.Settings?.Copy();
            Profiles = settings?.ConversionProfiles;
            if (Profiles != null)
            {
                SelectedProfile = Profiles.First(profile => profile.Name == _selectedProfileProvider.SelectedProfile.Name);
            }
        }

        private void SetProfileForSelection(object o)
        {
            _selectedProfileProvider.SelectedProfile = _selectedProfileProvider.Profiles.First(profile => profile.Name == Job.Profile.Name);
        }

        private void SetOutputFormatExecute(OutputFormat parameter)
        {
            OutputFormat = parameter;
        }

        public string NumberOfPrintJobsHint { get; private set; }

        public DelegateCommand<OutputFormat> SetOutputFormatCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand SendByEmailCommand { get; }
        public ICommand BrowseFileCommand { get; }
        public ICommand MergeCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand EditProfileCommand { get; private set; }

        public Job Job
        {
            get { return _job; }
            private set { _job = value; RaisePropertyChanged(); }
        }

        public ObservableCollection<ConversionProfile> Profiles { get; private set; }

        public IEnumerable<OutputFormat> OutputFormats => System.Enum.GetValues(typeof(OutputFormat)) as OutputFormat[];

        public OutputFormat OutputFormat
        {
            get { return Job?.Profile?.OutputFormat ?? OutputFormat.Pdf; }
            set
            {
                Job.Profile.OutputFormat = value;
                ChangeOutputFormat();
                RaisePropertyChanged();
            }
        }

        private void ChangeOutputFormat()
        {
            if (Job?.Profile?.OutputFormat == null)
                return;

            OutputFilename = Path.ChangeExtension(OutputFilename, GetExtension(Job.Profile.OutputFormat));
        }

        private string GetExtension(OutputFormat outputFormat)
        {
            var formatString = outputFormat.ToString().ToLowerInvariant();

            if (formatString.StartsWith("pdf"))
                return "pdf";

            return formatString;
        }

        public string OutputFolder
        {
            get { return _outputFolder; }
            set { SetProperty(ref _outputFolder, value); ComposeOutputFilename(); }
        }

        public string OutputFilename
        {
            get { return _outputFilename; }
            set { SetProperty(ref _outputFilename, value); ComposeOutputFilename(); }
        }

        private void ComposeOutputFilename()
        {
            _job.OutputFilenameTemplate = _pathSafe.Combine(OutputFolder, OutputFilename);
        }

        private void BrowseFileExecute(object parameter)
        {
            var result = _saveFileQuery.GetFileName(OutputFolder, OutputFilename, OutputFormat);

            if (result.Success)
            {
                SetOutputFilenameAndFolder(result.Data.Filepath);
                _latestDialogFilePath = result.Data.Filepath;
                OutputFormat = result.Data.OutputFormat;
            }
        }

        public void SetJob(Job job)
        {
            if (job == null)
                return;

            _job = job;

            var filenameTemplate = job.OutputFilenameTemplate;
            SetOutputFilenameAndFolder(filenameTemplate);
            OutputFormat = job.Profile.OutputFormat;
            RaisePropertyChanged(nameof(SelectedProfile));
            RaisePropertyChanged(nameof(Job));
        }

        private void SetOutputFilenameAndFolder(string filenameTemplate)
        {
            OutputFilename = Path.GetFileName(filenameTemplate);
            OutputFolder = Path.GetDirectoryName(filenameTemplate);
        }

        private void UpdateNumberOfPrintJobsHint(int numberOfPrintJobs)
        {
            if (numberOfPrintJobs <= 1)
                NumberOfPrintJobsHint = "";
            else if (numberOfPrintJobs > 99)
            {
                NumberOfPrintJobsHint = "99+";
            }
            else
            {
                NumberOfPrintJobsHint = numberOfPrintJobs.ToString();
            }

            RaisePropertyChanged(nameof(NumberOfPrintJobsHint));
        }

        public event EventHandler StepFinished;

        public void ExecuteWorkflowStep(Job job)
        {
            SetJob(job);
        }

        private void SaveExecute(object obj)
        {
            if (IsProfileValidAndNotifyUserIfNot())
            {
                if (FileDoesNotExistsElseNotifyUser() && FilePathShortEnoughElseNotifyUser())
                {
                    CallFinishInteraction();
                }
            }
        }

        private bool FilePathShortEnoughElseNotifyUser()
        {
            var filePath = _pathSafe.Combine(OutputFolder, OutputFilename);

            if (filePath.Length < _pathUtil.MAX_PATH)
                return true;

            var title = Translation.FilePathTooLongTitle.ToUpper(CultureInfo.CurrentCulture);
            var text = Translation.FormatFilePathTooLongDescription(_pathUtil.MAX_PATH);

            var interaction = new MessageInteraction(text, title, MessageOptions.OK, MessageIcon.Exclamation);

            _interactionRequest.Raise(interaction);

            return false;
        }

        private bool IsProfileValidAndNotifyUserIfNot()
        {
            var profileCheckResult = _profileChecker.ProfileCheck(_job.Profile, _job.Accounts);

            if (!profileCheckResult)
            {
                var title = Translation.DefectiveProfile;
                var message = new StringBuilder();
                message.AppendLine(Translation.GetProfileIsDefectiveMessage(_job.Profile.Name, profileCheckResult));
                message.AppendLine();
                message.AppendLine(_errorCodeInterpreter.GetErrorText(profileCheckResult, true, "\u2022"));
                message.AppendLine(Translation.EditOrSelectNewProfile);
                var interaction = new MessageInteraction(message.ToString(), title, MessageOptions.OK, MessageIcon.Exclamation);
                _interactionRequest.Raise(interaction);
                return false;
            }
            return true;
        }

        public bool IsProfileEnabled
        {
            get
            {
                return GpoSettings != null ? !GpoSettings.DisableProfileManagement : true;
            }
        }

        private bool FileDoesNotExistsElseNotifyUser()
        {
            var filePath = _pathSafe.Combine(OutputFolder, OutputFilename);

            //Do not inform user, if SaveFileDialog already did
            if (filePath == _latestDialogFilePath)
                return true;

            if (!_file.Exists(filePath))
                return true;

            var title = Translation.ConfirmSaveAs.ToUpper(CultureInfo.CurrentCulture);
            var text = Translation.GetFileAlreadyExists(filePath);

            var interaction = new MessageInteraction(text, title, MessageOptions.YesNo, MessageIcon.Exclamation);

            _interactionRequest.Raise(interaction, NotifyFileExistsCallback);

            return false;
        }

        private void CallFinishInteraction()
        {
            _job.Passwords = JobPasswordHelper.GetJobPasswords(_job.Profile, _job.Accounts); //todo: Why here? Aren't we doing that already somewhere else?
            FinishInteraction();
        }

        private void NotifyFileExistsCallback(MessageInteraction interaction)
        {
            if (interaction.Response == MessageResponse.Yes)
            {
                CallFinishInteraction();
            }
            _latestDialogFilePath = "";
        }

        private void EmailExecute(object obj)
        {
            if (IsProfileValidAndNotifyUserIfNot())
            {
                _job.Passwords = JobPasswordHelper.GetJobPasswords(_job.Profile, _job.Accounts);

                _job.OutputFilenameTemplate = Path.Combine(
                    _tempFolderProvider.TempFolder,
                    Path.GetFileNameWithoutExtension(Path.GetRandomFileName()),
                    OutputFilename);

                _job.Profile.EmailClientSettings.Enabled = true;
                _job.Profile.OpenViewer = false;
                _job.Profile.OpenWithPdfArchitect = false;

                FinishInteraction();
            }
        }

        private void CancelExecute(object obj)
        {
            // This needs to be called before the exceptions are thrown
            FinishInteraction();
            throw new AbortWorkflowException("User cancelled in the PrintJobView");
        }

        private void MergeExecute(object obj)
        {
            // This needs to be called before the exceptions are thrown
            FinishInteraction();
            throw new ManagePrintJobsException();
        }

        private void FinishInteraction()
        {
            StepFinished?.Invoke(this, EventArgs.Empty);
        }
    }
}
