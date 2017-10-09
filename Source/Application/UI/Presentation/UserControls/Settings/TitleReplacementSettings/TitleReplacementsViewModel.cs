using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.UI.Presentation.Commands.TitleReplacements;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.TabHelper;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using Prism.Mvvm;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.TitleReplacementSettings
{
    public class TitleReplacementsViewModel : TranslatableViewModelBase<TitleReplacementsTranslation>, ITabViewModel
    {
        private readonly ICurrentSettingsProvider _settingsProvider;
        public IGpoSettings GpoSettings { get; }

        private string _replacedSampleText;
        //private string _replacementTypeText;
        private string _sampleText = "Microsoft Word - Sample Text.doc";
        
        private ObservableCollection<TitleReplacement> _titleReplacements;

        public TitleReplacementsViewModel(ITranslationUpdater transalationUpdater, ICurrentSettingsProvider settingsProvider, ICommandLocator commandLocator, IGpoSettings gpoSettings):base(transalationUpdater)
        {
            _settingsProvider = settingsProvider;
            GpoSettings = gpoSettings;
            TitleAddCommand = commandLocator.GetCommand<TitleReplacementAddCommand>();
            TitleDeleteCommand = commandLocator.GetCommand<TitleReplacementRemoveCommand>();
            TitleEditCommand = commandLocator.GetCommand<TitleReplacementEditCommand>();

            if (settingsProvider?.Settings != null)
            {
                TitleReplacements = settingsProvider.Settings.ApplicationSettings.TitleReplacement;
            }
            else
            {
                TitleReplacements = new ObservableCollection<TitleReplacement>();
            }

            UpdateReplacedText();

            TitleReplacements.CollectionChanged += TitleReplacementsOnCollectionChanged;
        }

        private void TitleReplacementsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            UpdateReplacedText();
        }

        public string SampleText
        {
            get { return _sampleText; }
            set
            {
                _sampleText = value;
                UpdateReplacedText();
            }
        }

        public string ReplacedSampleText
        {
            get { return _replacedSampleText; }
            set
            {
                _replacedSampleText = value;
                RaisePropertyChanged();
            }
        }

        public ICommand TitleEditCommand { get; set; }

        public ICommand TitleAddCommand { get; set; }

        public ICommand TitleDeleteCommand { get; set; }

        public ObservableCollection<TitleReplacement> TitleReplacements
        {
            get { return _titleReplacements; }
            set
            {
                _titleReplacements = value;
                var view = CollectionViewSource.GetDefaultView(_titleReplacements);
                view.SortDescriptions.Add(new SortDescription("ReplacementType", ListSortDirection.Ascending));
                view.SortDescriptions.Add(new SortDescription("Search", ListSortDirection.Descending));
                
                RaisePropertyChanged(nameof(TitleReplacements));
            }
        }

        public string Title { get; set; } = "Title Replacements";
        public IconList Icon { get; set; } = IconList.TitleReplacementSettings;
        public bool HiddenByGPO => false;
        public bool BlockedByGPO => GpoSettings.DisableTitleTab;

        protected override void OnTranslationChanged()
        {
            base.OnTranslationChanged();
            Title = Translation.Title;
            RaisePropertyChanged(nameof(Title));
            RaisePropertyChanged();
        }
        
        private void UpdateReplacedText()
        {
            var titleReplacer = new TitleReplacer();
            titleReplacer.AddReplacements(TitleReplacements);
            ReplacedSampleText = titleReplacer.Replace(SampleText);
        }

        public bool TitleIsEnabled
        {
            get
            {
                if (_settingsProvider.Settings.ApplicationSettings == null)
                    return true;

                return GpoSettings != null ? !GpoSettings.DisableTitleTab : true;
            }
        }
    }

    public class DesignTimeTitleReplacementsViewModel : TitleReplacementsViewModel
    {
        private static readonly ICurrentSettingsProvider CurrentSettingsProvider = new DesignTimeCurrentSettingsProvider();

        public DesignTimeTitleReplacementsViewModel() : base(new DesignTimeTranslationUpdater(), CurrentSettingsProvider, new DesignTimeCommandLocator(),null)
        {
        }
    }
}
