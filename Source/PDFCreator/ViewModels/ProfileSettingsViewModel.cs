using System;
using System.ComponentModel;
using System.Windows.Data;
using pdfforge.GpoReader;
using pdfforge.PDFCreator.Core;
using pdfforge.PDFCreator.Core.Actions;
using pdfforge.PDFCreator.Core.Settings;
using pdfforge.PDFCreator.Shared.Helper;
using pdfforge.PDFCreator.Shared.ViewModels;
using pdfforge.PDFCreator.Shared.ViewModels.Wrapper;
using pdfforge.PDFCreator.Shared.Views;

namespace pdfforge.PDFCreator.ViewModels
{
    internal class ProfileSettingsViewModel : ViewModelBase
    {
        private ICollectionView _conversionProfilesView;
        private SynchronizedCollection<ConversionProfile> _profileCollection;
        private PdfCreatorSettings _settings;
        private PdfCreatorSettings _unchangedOriginalSettings;
        private readonly GpoSettings _gpoSettings;
        public ButtonClickedToClose ButtonClickedToClose;
        public string InvalidProfileMessage;

        public ProfileSettingsViewModel()
        {
            ButtonClickedToClose = ButtonClickedToClose.None;

            AddProfileCommand = new DelegateCommand(AddProfileExcecute);
            DeleteProfileCommand = new DelegateCommand(DeleteProfileExcecute, DeleteProfileCanExcecute);
            RenameProfileCommand = new DelegateCommand(RenameProfileExcecute, RenameProfileCanExcecute);

            SaveButtonCommand = new DelegateCommand(SaveExcecute);
        }

        public ProfileSettingsViewModel(PdfCreatorSettings settings, GpoSettings gpoSettings, TranslationHelper translationHelper)
            : this()
        {
            Settings = settings;
            _unchangedOriginalSettings = _settings.Copy();

            _gpoSettings = gpoSettings;
        }

        public ProfileSettingsViewModel(PdfCreatorSettings settings, GpoSettings gpoSettings)
            : this(settings, gpoSettings, TranslationHelper.Instance)
        {   }

        public DelegateCommand SaveButtonCommand { get; private set; }
        public Func<Boolean> QueryDeleteProfile { private get; set; }
        public Func<string, string, Boolean> QueryDeleteProfileWithPrinterMapping { private get; set; }
        public Func<Boolean> QueryDiscardChanges { private get; set; }
        public Func<ActionResultDict, Boolean> QueryIgnoreDefectiveProfiles { private get; set; }
        public Func<String, String> QueryProfileName { private get; set; }
        public Action UpdateLayoutProfilesBoxAction { private get; set; }
        public DelegateCommand AddProfileCommand { get; private set; }
        public DelegateCommand RenameProfileCommand { get; private set; }
        public DelegateCommand DeleteProfileCommand { get; private set; }

        public ICollectionView ConversionProfilesView
        {
            get { return _conversionProfilesView; }
            set
            {
                _conversionProfilesView = value;
                RaisePropertyChanged("ConversionProfilesView");
            }
        }

        public PdfCreatorSettings Settings
        {
            get { return _settings; }
            set
            {
                _settings = value;
                if (_unchangedOriginalSettings == null)
                    _unchangedOriginalSettings = _settings.Copy();

                _profileCollection = new SynchronizedCollection<ConversionProfile>(_settings.ConversionProfiles);

                ConversionProfilesView = CollectionViewSource.GetDefaultView(_profileCollection.ObservableCollection);
                ConversionProfilesView.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
                ConversionProfilesView.CurrentChanged += (sender, args) => CurrentProfilePropertyChanged();

                ConversionProfilesView.MoveCurrentTo(_settings.GetLastUsedOrFirstProfile());
                CurrentProfilePropertyChanged();
            }
        }

        public ConversionProfile CurrentProfile
        {
            get
            {
                if (_conversionProfilesView == null)
                    return null;
                return _conversionProfilesView.CurrentItem as ConversionProfile;
            }
        }

        public void CurrentProfilePropertyChanged()
        {
            RaisePropertyChanged("CurrentProfile");
            RaisePropertyChanged("Settings");
            DeleteProfileCommand.RaiseCanExecuteChanged();
            RenameProfileCommand.RaiseCanExecuteChanged();
            RaisePropertyChanged("LowEncryptionEnabled");
            RaisePropertyChanged("MediumEncryptionEnabled");
            RaisePropertyChanged("HighEncryptionEnabled");
            RaisePropertyChanged("ExtendedPermissonsEnabled");
            RaisePropertyChanged("RestrictLowQualityPrintingEnabled");
            RaisePropertyChanged("AllowFillFormsEnabled");
            RaisePropertyChanged("AllowScreenReadersEnabled");
            RaisePropertyChanged("AllowEditingAssemblyEnabled");
        }

        public bool ProfileManagementIsEnabled
        {
            get { return !_gpoSettings.DisableProfileManagement; }
        }

        public bool CheckForClosingWindowAndRestoreTheSettings()
        {
            if (CheckForChangesInProfiles())
                if (!QueryDiscardChanges())
                    return false;

            Settings = _unchangedOriginalSettings;
            return true;
        }

        private bool CheckForChangesInProfiles()
        {
            //check for added or deleted profiles
            if (!Settings.ConversionProfiles.Count.Equals(_unchangedOriginalSettings.ConversionProfiles.Count))
                return true;

            for (var i = 0; i < Settings.ConversionProfiles.Count; i++)
            {
                //check for changes
                if (!Settings.ConversionProfiles[i].Equals(_unchangedOriginalSettings.ConversionProfiles[i]))
                    return true;
            }
            return false;
        }

        private void SaveExcecute(object obj)
        {
            ButtonClickedToClose = ButtonClickedToClose.Save;

            var actionResultDict = ProfileChecker.ProfileCheckDict(Settings.ConversionProfiles);
            if (!actionResultDict)
                if (!QueryIgnoreDefectiveProfiles(actionResultDict))
                {
                    ButtonClickedToClose = ButtonClickedToClose.None;
                    return; //Cancel if user wants to edit defective profiles
                }

            Settings.ApplicationSettings.LastUsedProfileGuid = CurrentProfile.Guid;

            RaiseCloseView(true);
        }

        private void RenameProfileExcecute(object obj)
        {
            var newProfileName = QueryProfileName(CurrentProfile.Name);
            if (newProfileName == null)
                return;

            CurrentProfile.Name = newProfileName;
            ConversionProfilesView.Refresh();
            UpdateLayoutProfilesBoxAction();
        }

        private bool RenameProfileCanExcecute(object obj)
        {
            if (CurrentProfile == null)
                return false;

            return CurrentProfile.Properties.Renamable;
        }

        private void DeleteProfileExcecute(object obj)
        {
            var mapping = Settings.GetPrinterByProfile(CurrentProfile);
            if (mapping != null)
            {
                if (!QueryDeleteProfileWithPrinterMapping(CurrentProfile.Name, mapping.PrinterName))
                    return;
            }
            else
            {
                if (!QueryDeleteProfile())
                    return;
            }

            _profileCollection.ObservableCollection.Remove(CurrentProfile);
        }

        private bool DeleteProfileCanExcecute(object obj)
        {
            if (CurrentProfile == null)
                return false;
            if (!CurrentProfile.Properties.Deletable)
                return false;
            if (Settings.ConversionProfiles.Count < 2)
                return false;
            return true;
        }

        private void AddProfileExcecute(object obj)
        {
            var name = QueryProfileName(null);
            if (name == null)
                return;

            var newProfile = CurrentProfile.Copy();
            newProfile.Guid = Guid.NewGuid().ToString();
            newProfile.Name = name;
            newProfile.Properties.Deletable = true;
            newProfile.Properties.Editable = true;
            newProfile.Properties.Renamable = true;
            _profileCollection.ObservableCollection.Add(newProfile);
            ConversionProfilesView.MoveCurrentTo(newProfile);
        }

        public InputBoxValidation ProfilenameIsValid(string profileName)
        {
            if (profileName == null)
                return new InputBoxValidation(false, InvalidProfileMessage);

            if (profileName.Length <= 0)
                return new InputBoxValidation(false, InvalidProfileMessage);

            var profileNameDoesNotExist = (Settings.GetProfileByName(profileName) == null);

            return new InputBoxValidation(profileNameDoesNotExist,
                profileNameDoesNotExist ? null : InvalidProfileMessage);
        }
    }

    internal enum ButtonClickedToClose
    {
        None,
        Save,
        Ok,
        Close
    }
}