using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Input;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.TabHelper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.TitleReplacementSettings;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.DefaultViewerSettings
{
    public class DefaultViewerViewModel : TranslatableViewModelBase<DefaultViewerTranslation>, ITabViewModel
    {
        private readonly ICurrentSettingsProvider _currentSettingsProvider;
        private readonly IOpenFileInteractionHelper _fileInteractionHelper;
        public IGpoSettings GpoSettings { get; }
        
        private ObservableCollection<DefaultViewer> _defaultViewers = new ObservableCollection<DefaultViewer>();

        public DefaultViewerViewModel(ITranslationUpdater transalationUpdater, ICurrentSettingsProvider currentSettingsProvider, 
            IGpoSettings gpoSettings, IOpenFileInteractionHelper fileInteractionHelper)
            : base(transalationUpdater)
        {
            _currentSettingsProvider = currentSettingsProvider;
            _fileInteractionHelper = fileInteractionHelper;
            GpoSettings = gpoSettings;

            if (_currentSettingsProvider.Settings != null)
                UpdateDefaultViewer();

            _currentSettingsProvider.SettingsChanged += (sender, args) => UpdateDefaultViewer();
            FindPathCommand = new DelegateCommand(ExecuteFindPath);
        }

        public ICommand FindPathCommand { get; set; }

        private void UpdateDefaultViewer()
        {
            _defaultViewers = _currentSettingsProvider.Settings.ApplicationSettings.DefaultViewerList();
            RaisePropertyChanged(nameof(DefaultViewers));
        }

        public void ExecuteFindPath(object data)
        {
            var model = (DefaultViewer) data;
            var filter = Translation.ExecutableFiles
                         + @" (*.exe, *.bat, *.cmd)|*.exe;*.bat;*.cmd|"
                         + Translation.AllFiles
                         + @"(*.*)|*.*";

            var interactionResult = _fileInteractionHelper.StartOpenFileInteraction("", "", filter);
            interactionResult.MatchSome(s =>
            {
                model.Path = s;
                RaisePropertyChanged(nameof(DefaultViewers));
            });
        }
        
        public ObservableCollection<DefaultViewer> DefaultViewers
        {
            get { return _defaultViewers; }
            set
            {
                _defaultViewers = value;
                RaisePropertyChanged(nameof(_defaultViewers));
            }
        }

        public string Title { get; set; } = "Viewer";
        public IconList Icon { get; set; } = IconList.DefaultViewerSettings;
        public bool HiddenByGPO => false;
        public bool BlockedByGPO => GpoSettings.DisableApplicationSettings;

        protected override void OnTranslationChanged()
        {
            base.OnTranslationChanged();
            Title = Translation.Title; 
            RaisePropertyChanged(nameof(Title));
            RaisePropertyChanged();
        }
    }

    public class DesignTimeDefaultViewerViewModel : DefaultViewerViewModel
    {
        private static readonly ICurrentSettingsProvider CurrentSettingsProvider = new DesignTimeCurrentSettingsProvider();

        public DesignTimeDefaultViewerViewModel() : base(new DesignTimeTranslationUpdater(), CurrentSettingsProvider, null, null)
        {
        }
    }
}
