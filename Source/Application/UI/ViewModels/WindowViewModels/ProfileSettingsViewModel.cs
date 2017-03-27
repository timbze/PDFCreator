using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using pdfforge.Obsidian;
using pdfforge.Obsidian.Interaction;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Workflow;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.ViewModels.Helper;
using pdfforge.PDFCreator.UI.ViewModels.UserControlViewModels.ProfileSettings;
using pdfforge.PDFCreator.UI.ViewModels.WindowViewModels.Translations;

namespace pdfforge.PDFCreator.UI.ViewModels.WindowViewModels
{
    public class ProfileSettingsViewModel : InteractionAwareViewModelBase<ProfileSettingsInteraction>
    {
        private readonly IInteractionInvoker _interactionInvoker;
        private readonly IProfileChecker _profileChecker;
        private IGpoSettings _gpoSettings;

        public ProfileSettingsViewModel(IInteractionInvoker interactionInvoker, ProfileSettingsWindowTranslation translation, IProfileChecker profileChecker, ProfileSettingsViewModelBundle viewModelBundle)
        {
            ViewModelBundle = viewModelBundle;
            _interactionInvoker = interactionInvoker;
            Translation = translation;
            _profileChecker = profileChecker;

            SaveButtonCommand = new DelegateCommand(SaveExcecute);
            AddProfileCommand = new DelegateCommand(AddProfileExcecute);
            RenameProfileCommand = new DelegateCommand(RenameProfileExcecute, RenameProfileCanExcecute);
            DeleteProfileCommand = new DelegateCommand(DeleteProfileExcecute, DeleteProfileCanExcecute);
            WindowClosingCommand = new DelegateCommand<CancelEventArgs>(WindowClosingExecute);

            CurrentProfileChangedCommand = new DelegateCommand(OnCurrentProfileChanged);
        }

        public ProfileSettingsWindowTranslation Translation { get; set; }

        public SynchronizedCollection<ConversionProfile> ProfileCollection { get; set; }

        public DelegateCommand CurrentProfileChangedCommand { get; set; }

        public ProfileSettingsViewModelBundle ViewModelBundle { get; }

        public DelegateCommand SaveButtonCommand { get; set; }
        public DelegateCommand AddProfileCommand { get; set; }
        public DelegateCommand RenameProfileCommand { get; set; }
        public DelegateCommand DeleteProfileCommand { get; set; }
        public DelegateCommand<CancelEventArgs> WindowClosingCommand { get; set; }

        public PdfCreatorSettings Settings { get; private set; }

        public ICollectionView ConversionProfilesView { get; private set; }

        public ConversionProfile CurrentProfile { get; set; }

        public bool ProfileManagementIsEnabled
        {
            get
            {
                var gpoVal = _gpoSettings?.DisableProfileManagement;
                return gpoVal != true;
            }
        }

        private void OnCurrentProfileChanged(object obj)
        {
            RaiseCurrentProfilePropertyChanged();
        }

        private void WindowClosingExecute(CancelEventArgs e)
        {
            if (Interaction.ApplySettings)
                return;

            if (CheckForCancelClosing())
                e.Cancel = true;
        }

        protected override void HandleInteractionObjectChanged()
        {
            Settings = Interaction.Settings;
            _gpoSettings = Interaction.GpoSettings;

            ProfileCollection = new SynchronizedCollection<ConversionProfile>(Settings.ConversionProfiles);
            RaisePropertyChanged(nameof(ProfileCollection));
            CurrentProfile = Settings.GetLastUsedOrFirstProfile();
            ConversionProfilesView = CollectionViewSource.GetDefaultView(ProfileCollection.ObservableCollection);
            ConversionProfilesView.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            ConversionProfilesView.CurrentChanged += (sender, evntArgs) => RaiseCurrentProfilePropertyChanged();
            ViewModelBundle.SetAccounts(Settings.ApplicationSettings.Accounts);

            RaiseCurrentProfilePropertyChanged();
        }

        private void RaiseCurrentProfilePropertyChanged()
        {
            ViewModelBundle.UpdateCurrentProfile(CurrentProfile);

            RaisePropertyChanged(nameof(CurrentProfile));

            RaisePropertyChanged(nameof(Settings));
            DeleteProfileCommand.RaiseCanExecuteChanged();
            RenameProfileCommand.RaiseCanExecuteChanged();
        }

        private bool CheckForCancelClosing()
        {
            if (!CheckForChangesInProfiles())
                return false;

            if (QueryDiscardUnsavedChanges())
                return false;

            return true;
        }

        private bool CheckForChangesInProfiles()
        {
            //check for added or deleted profiles
            if (!Settings.ConversionProfiles.Count.Equals(Interaction.UnchangedOriginalSettings.ConversionProfiles.Count))
                return true;

            for (var i = 0; i < Settings.ConversionProfiles.Count; i++)
            {
                //check for changes
                if (!Settings.ConversionProfiles[i].Equals(Interaction.UnchangedOriginalSettings.ConversionProfiles[i]))
                    return true;
            }
            return false;
        }

        private void SaveExcecute(object obj)
        {
            var actionResultDict = _profileChecker.ProfileCheckDict(Settings.ConversionProfiles, Settings.ApplicationSettings.Accounts);
            if (!actionResultDict)
                if (!QueryIgnoreDefectiveProfiles(actionResultDict))
                {
                    return; //Cancel if user wants to edit defective profiles
                }

            Settings.ApplicationSettings.LastUsedProfileGuid = CurrentProfile.Guid;

            Interaction.ApplySettings = true;
            FinishInteraction();
        }

        private string QueryProfileName(string proposedName)
        {
            var title = Translation.ProfileName;
            var questionText = Translation.EnterProfileName;

            var inputInteraction = new InputInteraction(title, questionText, ProfilenameIsValid);

            if (proposedName != null)
                inputInteraction.InputText = proposedName;
            else
                inputInteraction.InputText = Translation.NewProfile;

            _interactionInvoker.Invoke(inputInteraction);

            if (!inputInteraction.Success)
                return null;

            return inputInteraction.InputText;
        }

        private bool QueryDeleteProfile()
        {
            var message = Translation.GetReallyDeleteProfileFormattedTranslation(CurrentProfile.Name);
            var caption = Translation.DeleteProfile;

            var interaction = new MessageInteraction(message, caption, MessageOptions.YesNo, MessageIcon.Question);
            _interactionInvoker.Invoke(interaction);

            return interaction.Response == MessageResponse.Yes;
        }

        private bool QueryDiscardUnsavedChanges()
        {
            var caption = Translation.UnsavedChanges;
            var message = Translation.ReallyWantToCancel;

            var interaction = new MessageInteraction(message, caption, MessageOptions.YesNo, MessageIcon.Question);
            _interactionInvoker.Invoke(interaction);

            return interaction.Response == MessageResponse.Yes;
        }

        private bool QueryDeleteProfileWithPrinterMapping(string profileName, string printerName)
        {
            var message = Translation.GetDeleteProfileWithMappedPrinterFormattedTranslation(profileName, printerName);
            var caption = Translation.ProfileHasPrinterTitle;

            var interaction = new MessageInteraction(message, caption, MessageOptions.YesNo, MessageIcon.Question);
            _interactionInvoker.Invoke(interaction);

            return interaction.Response == MessageResponse.Yes;
        }

        private bool QueryIgnoreDefectiveProfiles(ActionResultDict actionResultDict)
        {
            var profileProblemsInteraction = new ProfileProblemsInteraction(actionResultDict);
            _interactionInvoker.Invoke(profileProblemsInteraction);
            return profileProblemsInteraction.IgnoreProblems;
        }

        private void RenameProfileExcecute(object obj)
        {
            var newProfileName = QueryProfileName(CurrentProfile.Name);
            if (newProfileName == null)
                return;

            CurrentProfile.Name = newProfileName;
            ForceComboboxUpdate();
            ConversionProfilesView.Refresh();
        }

        private void ForceComboboxUpdate()
        {
            var temp = CurrentProfile;
            CurrentProfile = null;
            RaisePropertyChanged(nameof(CurrentProfile));
            CurrentProfile = temp;
            RaisePropertyChanged(nameof(CurrentProfile));
        }

        private bool RenameProfileCanExcecute(object obj)
        {
            return CurrentProfile != null && CurrentProfile.Properties.Renamable;
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

            ProfileCollection.ObservableCollection.Remove(CurrentProfile);
            CurrentProfile = ProfileCollection.ObservableCollection.First();
            RaiseCurrentProfilePropertyChanged();
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
            ProfileCollection.ObservableCollection.Add(newProfile);
            CurrentProfile = newProfile;
            RaisePropertyChanged(nameof(CurrentProfile));
        }

        public InputValidation ProfilenameIsValid(string profileName)
        {
            var invalidProfileMessage = Translation.InvalidProfileName;

            if (profileName == null)
                return new InputValidation(false, invalidProfileMessage);

            if (profileName.Length <= 0)
                return new InputValidation(false, invalidProfileMessage);

            var profileNameDoesNotExist = Settings.GetProfileByName(profileName) == null;

            return new InputValidation(profileNameDoesNotExist,
                profileNameDoesNotExist ? null : invalidProfileMessage);
        }
    }
}