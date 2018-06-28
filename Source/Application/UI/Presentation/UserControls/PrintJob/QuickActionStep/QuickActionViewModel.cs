using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.UI.Presentation.Commands;
using pdfforge.PDFCreator.UI.Presentation.Commands.QuickAction;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using pdfforge.PDFCreator.UI.Presentation.Workflow;
using System;
using System.Collections.Generic;
using System.Linq;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob.QuickActionStep
{
    public class QuickActionViewModel : TranslatableViewModelBase<QuickActionTranslation>, IWorkflowViewModel
    {
        private readonly ICommandLocator _commandLocator;
        private readonly IReadableFileSizeFormatter _readableFileSizeHelper;
        private readonly ICurrentSettingsProvider _currentSettingsProvider;
        private OutputFormat _outputFormat = OutputFormat.Pdf;
        private Job _job;

        private string _fileDirectory;
        private string _fileName;
        private string _fileSize;

        public QuickActionViewModel(ITranslationUpdater translationUpdater, ICommandLocator commandLocator, IReadableFileSizeFormatter readableFileSizeHelper, ICurrentSettingsProvider currentSettingsProvider) : base(translationUpdater)
        {
            _commandLocator = commandLocator;
            _readableFileSizeHelper = readableFileSizeHelper;
            _currentSettingsProvider = currentSettingsProvider;
            StartQuickActionCommand = new DelegateCommand(StartQuickActionCommandExecute);
            FinishCommand = new DelegateCommand(OnFinish);
            InitList();
        }

        private void OnFinish(object obj)
        {
            _commandLocator.GetCommand<SaveApplicationSettingsChangesCommand>().Execute(null);
            StepFinished?.Invoke(this, EventArgs.Empty);
        }

        private void InitList()
        {
            QuickActionOpenList = new List<QuickActionListItemVo>
            {
                new QuickActionListItemVo(Translation.OpenPDFArchitect, _commandLocator.GetCommand<JobQuickActionOpenWithPdfArchitectCommand>(), StartQuickActionCommand),
                new QuickActionListItemVo(Translation.OpenDefaultProgram, _commandLocator.GetCommand<JobQuickActionOpenWithDefaultCommand>(),StartQuickActionCommand),
                new QuickActionListItemVo(Translation.OpenExplorer, _commandLocator.GetCommand<JobQuickActionOpenExplorerLocationCommand>(),StartQuickActionCommand)
            };

            QuickActionSendList = new List<QuickActionListItemVo>
            {
                new QuickActionListItemVo(Translation.SendEmail, _commandLocator.GetCommand<JobQuickActionSendEmailCommand>(), StartQuickActionCommand)
            };
        }

        public void StartQuickActionCommandExecute(object obj)
        {
            var vo = obj as QuickActionListItemVo;
            if (vo != null)
            {
                vo.Command.Execute(_job);
            }
        }

        public void ExecuteWorkflowStep(Job job)
        {
            _job = job;
            OutputFormat = job.Profile.OutputFormat;
            var firstFile = job.OutputFiles.First();

            FileName = System.IO.Path.GetFileName(firstFile);
            FileDirectory = System.IO.Path.GetDirectoryName(firstFile);
            FileSize = _readableFileSizeHelper.GetFileSizeString(firstFile);
            RaisePropertyChanged(nameof(IsActive));
        }

        public event EventHandler StepFinished;

        public OutputFormat OutputFormat
        {
            get { return _outputFormat; }
            set
            {
                _outputFormat = value;
                RaisePropertyChanged(nameof(OutputFormat));
            }
        }

        public string FileDirectory
        {
            get { return _fileDirectory; }
            private set
            {
                _fileDirectory = value;
                RaisePropertyChanged(nameof(FileDirectory));
            }
        }

        public string FileSize
        {
            get { return _fileSize; }
            set
            {
                _fileSize = value;
                RaisePropertyChanged(nameof(FileSize));
            }
        }

        public string FileName
        {
            get { return _fileName; }
            set
            {
                _fileName = value;
                RaisePropertyChanged(nameof(FileName));
            }
        }

        public bool IsActive
        {
            set
            {
                if (_job != null && _currentSettingsProvider != null)
                {
                    var conversionProfile = _currentSettingsProvider.Profiles.First(x => x.Equals(_job.Profile));
                    conversionProfile.ShowQuickActions = !value;
                    _job.Profile.ShowQuickActions = !value;
                    RaisePropertyChanged(nameof(IsActive));
                }
            }
            get
            {
                return _job?.Profile?.ShowQuickActions == false;
            }
        }

        public DelegateCommand StartQuickActionCommand { get; }

        public DelegateCommand FinishCommand { get; }

        public IEnumerable<QuickActionListItemVo> QuickActionOpenList { get; private set; }
        public IEnumerable<QuickActionListItemVo> QuickActionSendList { get; private set; }
    }
}
