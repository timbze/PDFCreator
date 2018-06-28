using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.Controller.Routing;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.Presentation.Assistants;
using pdfforge.PDFCreator.UI.Presentation.Commands;
using pdfforge.PDFCreator.UI.Presentation.Events;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.Routing;
using pdfforge.PDFCreator.UI.Presentation.UserControls;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Welcome;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using System;
using System.Windows;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation
{
    public class MainShellViewModel : TranslatableViewModelBase<MainShellTranslation>
    {
        public ApplicationNameProvider ApplicationName { get; }
        public IInteractionRequest InteractionRequest { get; }

        private readonly IEventAggregator _aggregator;
        private readonly ICommandLocator _commandLocator;
        private string _activePath = MainRegionViewNames.HomeView;
        private Action _closeViewAction;
        private readonly IDispatcher _dispatcher;
        private readonly IRegionManager _regionManager;
        private readonly IWelcomeSettingsHelper _welcomeSettingsHelper;
        public IGpoSettings GpoSettings { get; }

        private bool _showUpdate;

        public bool ShowUpdate
        {
            get { return _showUpdate; }
            set
            {
                _showUpdate = value;
                RaisePropertyChanged();
            }
        }

        public MainShellViewModel(DragAndDropEventHandler dragAndDrop, ITranslationUpdater translation,
            ApplicationNameProvider applicationName, IInteractionRequest interactionRequest,
            IEventAggregator aggregator, ICommandLocator commandLocator, IDispatcher dispatcher, IRegionManager regionManager,
            IWelcomeSettingsHelper welcomeSettingsHelper, IGpoSettings gpoSettings, IUpdateAssistant updateAssistant, IEventAggregator eventAggregator)
            : base(translation)
        {
            _aggregator = aggregator;
            _commandLocator = commandLocator;
            ApplicationName = applicationName;
            InteractionRequest = interactionRequest;

            _dispatcher = dispatcher;
            _regionManager = regionManager;
            _welcomeSettingsHelper = welcomeSettingsHelper;
            ShowUpdate = updateAssistant.IsUpdateAvailable();
            GpoSettings = gpoSettings;

            NavigateCommand = commandLocator?.CreateMacroCommand()
            .AddCommand<SaveAndContinueEvaluationCommand>()
            .AddCommand<SaveApplicationSettingsChangesCommand>()
            .AddCommand<NavigateMainTabCommand>()
            .Build();

            DragEnterCommand = new DelegateCommand<DragEventArgs>(dragAndDrop.HandleDragEnter);
            DragDropCommand = new DelegateCommand<DragEventArgs>(dragAndDrop.HandleDropEvent);

            aggregator.GetEvent<NavigateToHomeEvent>().Subscribe(OnSettingsChanged);
            aggregator.GetEvent<NavigateMainTabEvent>().Subscribe(OnMainShellNavigated);
            aggregator.GetEvent<ForceMainShellNavigation>().Subscribe(OnForcedNavigation);
            aggregator.GetEvent<CloseMainWindowEvent>().Subscribe(OnCloseMainWindow);

            ShowUpdate = updateAssistant.ShowUpdate;
            eventAggregator.GetEvent<SetShowUpdateEvent>().Subscribe(value =>
            {
                ShowUpdate = updateAssistant.ShowUpdate;
            }, true);
        }

        private void SetupActivePathInMainShell(StartupRoutine startupRoutine)
        {
            var startupNavigationActions = startupRoutine.GetActionByType<StartupNavigationAction>();
            foreach (var startupNavigationAction in startupNavigationActions)
            {
                if (startupNavigationAction.Region == RegionNames.MainRegion)
                {
                    ActivePath = startupNavigationAction.Target;
                }
            }
        }

        public void MainShellStartupAction(StartupRoutine startupRoutine)
        {
            SetupActivePathInMainShell(startupRoutine);
            foreach (var startupNavigationAction in startupRoutine.GetActionByType<StartupNavigationAction>())
            {
                _regionManager.RequestNavigate(startupNavigationAction.Region, startupNavigationAction.Target);
            }

            if (_welcomeSettingsHelper.IsFirstRun())
                _dispatcher.InvokeAsync(ShowWelcomeWindow);
        }

        public void ShowWelcomeWindow()
        {
            InteractionRequest.Raise(new WelcomeView());
            _welcomeSettingsHelper.SetCurrentApplicationVersionAsWelcomeVersionInRegistry();
        }

        private void OnCloseMainWindow()
        {
            _dispatcher.BeginInvoke(() => _closeViewAction?.Invoke());
        }

        private void OnForcedNavigation(string obj)
        {
            NavigateCommand.Execute(obj);
        }

        private void OnMainShellNavigated(string targetView)
        {
            _activePath = targetView;
            RaisePropertyChanged(nameof(ActivePath));
        }

        private void OnSettingsChanged()
        {
            NavigateCommand.Execute(MainRegionViewNames.HomeView);
        }

        public ICommand DragEnterCommand { get; }

        public ICommand DragDropCommand { get; }

        public ICommand NavigateCommand { get; set; }

        public string ActivePath
        {
            set
            {
                _activePath = value;
                RaisePropertyChanged();
            }
            get
            {
                return _activePath;
            }
        }

        public void PublishMainShellDone()
        {
            _aggregator.GetEvent<MainWindowOpenedEvent>().Publish();
        }

        public void Init(Action close)
        {
            _closeViewAction = close;
        }

        public bool CanClose()
        {
            var macroCommand = _commandLocator.CreateMacroCommand()
                .AddCommand<SaveAndContinueEvaluationCommand>()
                .AddCommand<SaveApplicationSettingsChangesCommand>()
                .AddCommand(new DelegateCommand(OnCloseMainWindow))
                .Build();

            if (macroCommand.CanExecute(null))
            {
                var booleanMacroResult = macroCommand.ExecuteWithResult(null);
                return booleanMacroResult?.Result ?? false;
            }

            return true;
        }

        public void OnClosed()
        {
            _aggregator.GetEvent<MainWindowClosedEvent>().Publish();
        }

        #region nothing to see here, move along!

        //      \
        //       \ji
        //       /.(((
        //      (,/"(((__,--.
        //          \  ) _( /{
        //          !||   :||
        //          !||   :||
        //          '''   '''

        #endregion nothing to see here, move along!
    }

    public class SetShowUpdateEvent : PubSubEvent<bool>
    {
    }
}
