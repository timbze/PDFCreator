using NLog;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.Core.Workflow;
using pdfforge.PDFCreator.Core.Workflow.Queries;
using pdfforge.PDFCreator.UI.Presentation.Commands;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.IO;
using System;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.UI.Presentation.Workflow
{
    public sealed class InteractiveWorkflow : ConversionWorkflow
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IShellManager _shellManager;
        private readonly IPathSafe _pathSafe;
        private readonly IErrorNotifier _errorNotifier;
        private readonly ISettingsProvider _settingsProvider;
        private readonly ICommandLocator _commandLocator;
        private readonly IPathUtil _pathUtil;
        private readonly ILastSaveDirectoryHelper _lastSaveDirectoryHelper;
        private readonly IDirectoryHelper _directoryHelper;
        private readonly InteractiveProfileChecker _interactiveProfileChecker;
        private readonly ITargetFileNameComposer _targetFileNameComposer;

        public InteractiveWorkflow(IShellManager shellManager, ITargetFileNameComposer targetFileNameComposer, IJobDataUpdater jobDataUpdater,
                                   IPathSafe pathSafe, IErrorNotifier errorNotifier, ISettingsProvider settingsProvider,
                                   ICommandLocator commandLocator, IPathUtil pathUtil, ILastSaveDirectoryHelper lastSaveDirectoryHelper,
                                   IDirectoryHelper directoryHelper, InteractiveProfileChecker interactiveProfileChecker
            )
        {
            _shellManager = shellManager;
            _pathSafe = pathSafe;
            _errorNotifier = errorNotifier;
            _settingsProvider = settingsProvider;
            _commandLocator = commandLocator;
            _pathUtil = pathUtil;
            _lastSaveDirectoryHelper = lastSaveDirectoryHelper;
            _directoryHelper = directoryHelper;
            _interactiveProfileChecker = interactiveProfileChecker;
            _targetFileNameComposer = targetFileNameComposer;

            JobDataUpdater = jobDataUpdater;
            _targetFileNameComposer = targetFileNameComposer;
        }

        protected override IJobDataUpdater JobDataUpdater { get; }

        protected override void DoWorkflowWork(Job job)
        {
            job.OutputFilenameTemplate = ComposeFilename(job);

            _lastSaveDirectoryHelper.Apply(job);

            if (job.Profile.SkipPrintDialog)
            {
                if (_interactiveProfileChecker.CheckWithErrorResultInWindow(job))
                {
                    _commandLocator.GetCommand<SkipPrintDialogCommand>().Execute(job);
                }
                else
                {
                    //Enable PrintJobView for invalid profiles
                    job.Profile.SkipPrintDialog = false;
                }
            }

            _logger.Debug("Starting PrintJobWindow");

            try
            {
                _shellManager.ShowPrintJobShell(job);
                _settingsProvider.Settings.ApplicationSettings.LastUsedProfileGuid = job.Profile.Guid;

                if (job.IsSuccessful)
                {
                    _lastSaveDirectoryHelper.Save(job);
                }
            }
            finally
            {
                _directoryHelper.DeleteCreatedDirectories();
            }
        }

        private string ComposeFilename(Job job)
        {
            var filePath = _targetFileNameComposer.ComposeTargetFileName(job);

            var folderName = _pathUtil.GetLongDirectoryName(filePath);

            if (string.IsNullOrEmpty(folderName))
                folderName = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            var filename = _pathSafe.GetFileName(filePath);

            if (string.IsNullOrEmpty(filename))
                filename = "document.pdf";

            return _pathSafe.Combine(folderName, filename);
        }

        protected override void HandleError(ErrorCode errorCode)
        {
            _errorNotifier.Notify(new ActionResult(errorCode));
        }
    }
}
