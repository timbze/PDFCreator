using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.Presentation.Commands.ProfileCommands;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles
{
    public class ProfilesViewModel : TranslatableViewModelBase<ProfileMangementTranslation>
    {
        private readonly ISettingsProvider _settingsProvider;
        public IGpoSettings GpoSettings { get; }

        public ProfilesViewModel(ISelectedProfileProvider selectedProfileProvider, ITranslationUpdater translationUpdater, ICommandLocator commandLocator, ISettingsProvider settingsProvider, IGpoSettings gpoSettings)
            : base(translationUpdater)
        {
            _settingsProvider = settingsProvider;
            GpoSettings = gpoSettings;
            SelectedProfileProvider = selectedProfileProvider;

            ProfileAddCommand = commandLocator.GetCommand<ProfileAddCommand>();
            ProfileRenameCommand = commandLocator.GetCommand<ProfileRenameCommand>();
            ProfileRemoveCommand = commandLocator.GetCommand<ProfileRemoveCommand>();

            if (selectedProfileProvider != null)
            {
                selectedProfileProvider.SettingsChanged += OnProfilesChanged;
                selectedProfileProvider.SelectedProfileChanged += (s, args) => RaisePropertyChanged(nameof(SelectedProfile));
            }
        }

        private void OnProfilesChanged(object sender, EventArgs e)
        {
            var profileGuid = SelectedProfile.Guid;
            RaisePropertyChanged(nameof(Profiles));

            var selectedProfile = Profiles.FirstOrDefault(x => x.Guid == profileGuid)
                ?? Profiles.FirstOrDefault();

            SelectedProfileProvider.SelectedProfile = selectedProfile;
            RaisePropertyChanged(nameof(SelectedProfile));
        }

        public ConversionProfile SelectedProfile
        {
            get { return SelectedProfileProvider?.SelectedProfile; }
            set { SelectedProfileProvider.SelectedProfile = value; }
        }

        public ObservableCollection<ConversionProfile> Profiles
        {
            get { return SelectedProfileProvider?.Profiles; }
            set { }
        }

        public bool ProfileIsDisabled
        {
            get
            {
                if (_settingsProvider.Settings?.ApplicationSettings == null)
                    return false;

                return GpoSettings != null ? GpoSettings.DisableProfileManagement : false;
            }
        }

        public ISelectedProfileProvider SelectedProfileProvider { get; }

        public ICommand ProfileAddCommand { get; set; }
        public ICommand ProfileRenameCommand { get; set; }
        public ICommand ProfileRemoveCommand { get; set; }
    }
}
