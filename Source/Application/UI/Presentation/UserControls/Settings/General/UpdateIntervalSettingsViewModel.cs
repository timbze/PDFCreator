using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.Presentation.Assistants;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles;
using pdfforge.PDFCreator.Utilities.Process;
using Prism.Events;
using System.Windows;
using System.Windows.Input;
using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.General
{
    public class UpdateIntervalSettingsViewModel : AGeneralSettingsItemControlModel
    {
        private readonly IProcessStarter _processStarter;
        private readonly IUpdateAssistant _updateAssistant;
        private readonly IInteractionRequest _interactionRequest;
        private readonly SetShowUpdateEvent _showUpdateEvent;

        public UpdateIntervalSettingsViewModel(IUpdateAssistant updateAssistant, IProcessStarter processStarter, 
            ICurrentSettingsProvider settingsProvider, IGpoSettings gpoSettings, ITranslationUpdater translationUpdater, IEventAggregator eventAggregator, IInteractionRequest interactionRequest) :
            base(translationUpdater, settingsProvider, gpoSettings)
        {
            _processStarter = processStarter;
            _updateAssistant = updateAssistant;
            _interactionRequest = interactionRequest;

            ShowUpdate = updateAssistant.ShowUpdate;
            _showUpdateEvent = eventAggregator.GetEvent<SetShowUpdateEvent>();
            _showUpdateEvent.Subscribe(SetShowDialog);
            settingsProvider.SettingsChanged += (sender, args) =>
            {
                RaisePropertyChanged(nameof(CurrentUpdateInterval));
                RaisePropertyChanged(nameof(DisplayUpdateWarning));
            };
        }

        private void SetShowDialog(bool value)
        {
            ShowUpdate = _updateAssistant.ShowUpdate;
            RaisePropertyChanged(nameof(ShowUpdate));
            RaisePropertyChanged(nameof(NewUpdateMessage));
        }

        public ICommand InstallUpdateCommand => new DelegateCommand(o => InstallUpdate());
        public ICommand SkipVersionCommand => new DelegateCommand(o => SkipVersion());
        public ICommand AskLaterCommand => new DelegateCommand(o => UpdateLater());
        public ICommand UpdateCheckCommand => new DelegateCommand(o =>
        {
            try
            {
                if (_updateAssistant.IsOnlineUpdateAvailable())
                {
                    _showUpdateEvent.Publish(true);
                }
                else
                {
                    var interaction = new MessageInteraction(Translation.NoUpdateMessage, Translation.UpdateCheckTitle, MessageOptions.OK, MessageIcon.PDFCreator);
                    _interactionRequest.Raise(interaction);
                }
            }
            catch
            {
                var interaction = new MessageInteraction(Translation.ErrorMessage, Translation.UpdateCheckTitle, MessageOptions.OK, MessageIcon.Exclamation);
                _interactionRequest.Raise(interaction);
            }
        });

        public string NewUpdateMessage => Translation.GetNewUpdateMessage(_updateAssistant.OnlineVersion.Version.ToString(3));

        private void UpdateLater()
        {
            _updateAssistant.SetNewUpdateTime();
        }

        private void InstallUpdate()
        {
            _updateAssistant.InstallNewUpdate();
        }

        private void SkipVersion()
        {
            _updateAssistant.SkipVersion();
        }

        public bool UpdateIsEnabled => GpoSettings?.UpdateInterval == null;

        public Visibility UpdateCheckControlVisibility
            => _updateAssistant.UpdatesEnabled ? Visibility.Visible : Visibility.Collapsed;

        public bool ShowUpdate { get; set; }


        public ICommand VisitWebsiteCommand => new DelegateCommand(VisitWebsiteExecute);
        public string PdfforgeWebsiteUrl => Urls.PdfforgeWebsiteUrl;

        public bool DisplayUpdateWarning => SettingsProvider.Settings?.ApplicationSettings?.UpdateInterval == UpdateInterval.Never;

        public UpdateInterval CurrentUpdateInterval
        {
            get
            {
                if (SettingsProvider.Settings?.ApplicationSettings == null)
                    return UpdateInterval.Weekly;

                if (GpoSettings?.UpdateInterval == null)
                    return SettingsProvider.Settings.ApplicationSettings.UpdateInterval;
                return UpdateIntervalHelper.ParseUpdateInterval(GpoSettings.UpdateInterval);
            }
            set
            {
                SettingsProvider.Settings.ApplicationSettings.UpdateInterval = value;
                RaisePropertyChanged(nameof(CurrentUpdateInterval));
                RaisePropertyChanged(nameof(DisplayUpdateWarning));
            }
        }

        private void VisitWebsiteExecute(object o)
        {
            try
            {
                _processStarter.Start(PdfforgeWebsiteUrl);
            }
            catch
            {
                // ignored
            }
        }
    }
}
