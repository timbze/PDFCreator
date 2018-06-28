using pdfforge.Obsidian;
using pdfforge.Obsidian.Interaction.DialogInteractions;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.Printing.Printer;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.Services.JobHistory;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.Presentation.Commands;
using pdfforge.PDFCreator.UI.Presentation.Commands.QuickAction;
using pdfforge.PDFCreator.UI.Presentation.Commands.QuickAction.HistoricJob;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob.QuickActionStep;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Home
{
    public class HomeViewModel : TranslatableViewModelBase<HomeViewTranslation>
    {
        private readonly IFileConversionHandler _fileConversionHandler;
        private readonly IPrinterHelper _printerHelper;
        private readonly ISettingsProvider _settingsProvider;
        private readonly IInteractionInvoker _interactionInvoker;

        public HomeViewModel(IInteractionInvoker interactionInvoker, IFileConversionHandler fileConversionHandler, ITranslationUpdater translationUpdater,
                             IPrinterHelper printerHelper, ISettingsProvider settingsProvider, IJobHistoryManager jobHistoryManager, IDispatcher dispatcher,
                             ICommandLocator commandLocator)
            : base(translationUpdater)
        {
            _interactionInvoker = interactionInvoker;
            _fileConversionHandler = fileConversionHandler;
            _printerHelper = printerHelper;
            _settingsProvider = settingsProvider;

            JobHistory = CollectionViewSource.GetDefaultView(jobHistoryManager.History);
            jobHistoryManager.HistoryChanged += (sender, args) =>
            {
                dispatcher.BeginInvoke(JobHistory.Refresh);
                RaisePropertyChanged(nameof(NumberOfHistoricJobs));
            };
            JobHistory.MoveCurrentTo(null); //unselect first item

            ConvertFileCommand = new DelegateCommand(o => ConvertFileExecute());

            ClearHistoryCommand = new DelegateCommand(o => jobHistoryManager.Clear());
            RefreshHistoryCommand = new DelegateCommand(o => jobHistoryManager.Refresh());
            RemoveHistoricJobCommand = new DelegateCommand<HistoricJob>(jobHistoryManager.Remove);
            ToggleHistoryEnabledCommand = new DelegateCommand<HistoricJob>(hj => HistoryEnabled = !HistoryEnabled);

            DeleteHistoricFilesCommand = commandLocator.CreateMacroCommand()
                .AddCommand<DeleteHistoricFilesCommand>()
                .AddCommand(RefreshHistoryCommand)
                .Build();

            StartQuickActionCommand = new DelegateCommand(StartQuickActionCommandExecute);

            QuickActionOpenList = new List<QuickActionListItemVo>
            {
                new QuickActionListItemVo(Translation.OpenPDFArchitect, commandLocator.GetCommand<HistoricJobOpenWithPdfArchitectCommand>(), StartQuickActionCommand),
                new QuickActionListItemVo(Translation.OpenDefaultProgram, commandLocator.GetCommand<HistoricJobOpenWithDefaultCommand>(), StartQuickActionCommand),
                new QuickActionListItemVo(Translation.OpenExplorer, commandLocator.GetCommand<HistoricJobOpenExplorerLocationCommand>(), StartQuickActionCommand),
                new QuickActionListItemVo(Translation.OpenMailClient, commandLocator.GetCommand<HistoricJobOpenMailClientCommand>(), StartQuickActionCommand)
            };
        }

        public DelegateCommand StartQuickActionCommand { get; }
        public IEnumerable<QuickActionListItemVo> QuickActionOpenList { get; private set; }
        public ICollectionView JobHistory { get; }
        public ICommand ConvertFileCommand { get; set; }
        public ICommand ClearHistoryCommand { get; set; }
        public ICommand RefreshHistoryCommand { get; set; }
        public ICommand ToggleHistoryEnabledCommand { get; set; }
        public ICommand RemoveHistoricJobCommand { get; set; }
        public ICommand DeleteHistoricFilesCommand { get; set; }

        public int NumberOfHistoricJobs
        {
            get
            {
                int count = 0;
                foreach (var item in JobHistory.SourceCollection)
                {
                    count++;
                }
                return count;
            }
        }

        public bool HistoryEnabled
        {
            get
            {
                return _settingsProvider.Settings?.ApplicationSettings?.JobHistory?.Enabled == true;
            }
            set
            {
                _settingsProvider.Settings.ApplicationSettings.JobHistory.Enabled = value;
                RaisePropertyChanged(nameof(HistoryEnabled));
            }
        }

        public string CallToActionText => Translation.FormatCallToAction(_printerHelper.GetApplicablePDFCreatorPrinter(_settingsProvider.Settings?.ApplicationSettings?.PrimaryPrinter ?? ""));

        private void StartQuickActionCommandExecute(object obj)
        {
            var vo = obj as QuickActionListItemVo;
            if (vo == null)
                return;

            if (JobHistory.CurrentItem != null)
                vo.Command.Execute(JobHistory.CurrentItem);
        }

        private void ConvertFileExecute()
        {
            var interaction = new OpenFileInteraction();

            _interactionInvoker.Invoke(interaction);

            if (!interaction.Success)
                return;

            var file = interaction.FileName;
            _fileConversionHandler.HandleFileList(new List<string> { file });
        }

        protected override void OnTranslationChanged()
        {
            RaisePropertyChanged(nameof(CallToActionText));
        }
    }
}
