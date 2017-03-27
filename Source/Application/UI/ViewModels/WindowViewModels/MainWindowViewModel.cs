using System;
using System.Windows;
using System.Windows.Input;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.Services.Logging;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.ViewModels.Helper;
using pdfforge.PDFCreator.UI.ViewModels.WindowViewModels.Translations;
using pdfforge.PDFCreator.Utilities;
using Translatable;

namespace pdfforge.PDFCreator.UI.ViewModels.WindowViewModels
{
    public class MainWindowViewModel : ObservableObject
    {
        private readonly IInteractionInvoker _interactionInvoker;
        private readonly ISettingsManager _settingsManager;
        private readonly ISettingsProvider _settingsProvider;
        private readonly IUserGuideHelper _userGuideHelper;
        private readonly IVersionHelper _versionHelper;
        private readonly ApplicationNameProvider _applicationNameProvider;
        private readonly ITranslationFactory _translationFactory;
        private bool _applicationSettingsEnabled;

        public MainWindowViewModel(ISettingsManager settingsManager, IInteractionInvoker interactionInvoker, IUserGuideHelper userGuideHelper, 
             IVersionHelper versionHelper, DragAndDropEventHandler dragAndDrop, WelcomeCommand welcomeCommand, ApplicationNameProvider applicationNameProvider, MainWindowTranslation mainWindowTranslation, ITranslationFactory translationFactory)
        {
            _settingsManager = settingsManager;
            _interactionInvoker = interactionInvoker;
            _userGuideHelper = userGuideHelper;
            _versionHelper = versionHelper;
            _applicationNameProvider = applicationNameProvider;
            _translationFactory = translationFactory;

            ApplicationSettingsCommand = new DelegateCommand(ExecuteApplicationSettingsCommand);
            ProfileSettingsCommand = new DelegateCommand(ExecuteProfileSettings);
            WelcomeCommand = welcomeCommand;
            Translation = mainWindowTranslation;
            AboutWindowCommand = new DelegateCommand(ExecuteAboutWindow);
            HelpCommand = new DelegateCommand<KeyEventArgs>(ExecuteHelpCommand);

            DragEnterCommand = new DelegateCommand<DragEventArgs>(dragAndDrop.HandleDragEnter);
            DragDropCommand = new DelegateCommand<DragEventArgs>(dragAndDrop.HandleDropEvent);

            _settingsProvider = _settingsManager.GetSettingsProvider();
            ApplicationSettingsEnabled = _settingsProvider.GpoSettings?.DisableApplicationSettings == false;
        }

        public bool ApplicationSettingsEnabled
        {
            get { return _applicationSettingsEnabled; }
            set
            {
                _applicationSettingsEnabled = value;
                RaisePropertyChanged(nameof(ApplicationSettingsEnabled));
            }
        }

        public ICommand ApplicationSettingsCommand { get; private set; }

        public ICommand ProfileSettingsCommand { get; private set; }

        public ICommand AboutWindowCommand { get; private set; }

        public ICommand WelcomeCommand { get; private set; }
        public MainWindowTranslation Translation { get; private set; }

        public ICommand HelpCommand { get; private set; }

        public ICommand DragEnterCommand { get; }

        public ICommand DragDropCommand { get; }

        public string ApplicationNameText => _applicationNameProvider.ApplicationName + " " + _versionHelper.FormatWithTwoDigits();

        private void ExecuteHelpCommand(KeyEventArgs e)
        {
            if (e.Key == Key.F1)
            {
                _userGuideHelper.ShowHelp(HelpTopic.General);
            }
        }

        private void ExecuteAboutWindow(object o)
        {
            _interactionInvoker.Invoke(new AboutWindowInteraction());
        }

        private void ExecuteProfileSettings(object o)
        {
            var interaction = new ProfileSettingsInteraction(_settingsProvider.Settings, _settingsProvider.GpoSettings);

            _interactionInvoker.Invoke(interaction);

            if (interaction.ApplySettings)
            {
                _settingsManager.ApplyAndSaveSettings(interaction.Settings);
            }
        }

        private void ExecuteApplicationSettingsCommand(object o)
        {
            var settings = _settingsProvider.Settings;

            var interaction = new ApplicationSettingsInteraction(settings.Copy(), _settingsProvider.GpoSettings);

            _interactionInvoker.Invoke(interaction);

            if (interaction.Success)
            {
                _settingsManager.ApplyAndSaveSettings(interaction.Settings);
            }

            Translation = _translationFactory.CreateTranslation<MainWindowTranslation>();
            RaisePropertyChanged(nameof(Translation));

            LoggingHelper.ChangeLogLevel(interaction.Settings.ApplicationSettings.LoggingLevel);
        }
    }

    public abstract class WelcomeCommand : ICommand
    {
        public void Execute(object parameter)
        {
            ExecuteWelcomeAction();
        }

        protected abstract void ExecuteWelcomeAction();

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;
    }

    public class ShowWelcomeWindowCommand : WelcomeCommand
    {
        private readonly IWelcomeSettingsHelper _welcomeSettingsHelper;
        private readonly IPlusHintHelper _plusHintHelper;
        private readonly IInteractionInvoker _interactionInvoker;

        public ShowWelcomeWindowCommand(IWelcomeSettingsHelper welcomeSettingsHelper, IPlusHintHelper plusHintHelper, IInteractionInvoker interactionInvoker)
        {
            _welcomeSettingsHelper = welcomeSettingsHelper;
            _plusHintHelper = plusHintHelper;
            _interactionInvoker = interactionInvoker;
        }

        protected override void ExecuteWelcomeAction()
        {
            if (_welcomeSettingsHelper.IsFirstRun())
            {
                _welcomeSettingsHelper.SetCurrentApplicationVersionAsWelcomeVersionInRegistry();
                _interactionInvoker.Invoke(new WelcomeInteraction());
            }
            else
            {
                if (!_plusHintHelper.QueryDisplayHint())
                    return;

                _interactionInvoker.InvokeNonBlocking(new PlusHintInteraction(_plusHintHelper.CurrentJobCounter));
            }
        }
    }

    public class DisabledWelcomeWindowCommand : WelcomeCommand
    {
        protected override void ExecuteWelcomeAction()
        {
            
        }
    }
}