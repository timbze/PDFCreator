using System;
using System.Collections.Generic;
using System.ComponentModel;
using SystemInterface.IO;
using NSubstitute;
using NUnit.Framework;
using pdfforge.DynamicTranslator;
using pdfforge.Obsidian;
using pdfforge.Obsidian.Interaction;
using pdfforge.Obsidian.Interaction.DialogInteractions;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Workflow;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.ViewModels.Helper;
using pdfforge.PDFCreator.UI.ViewModels.UserControlViewModels.ProfileSettings;
using pdfforge.PDFCreator.UI.ViewModels.WindowViewModels;
using pdfforge.PDFCreator.UnitTest.UnitTestHelper;

namespace pdfforge.PDFCreator.UnitTest.UI.ViewModels.WindowViewModels
{
    [TestFixture]
    internal class ProfileSettingsViewModelTest
    {
        [SetUp]
        public void SetUp()
        {
            _settings = new PdfCreatorSettings(null);
            _originalFirstProfile = new ConversionProfile();
            _originalFirstProfile.Guid = "firstProfileGuid";
            _originalFirstProfile.Name = OriginalProfileName;
            _settings.ConversionProfiles.Add(_originalFirstProfile);
            _interactionInvoker = new ProfileSettingsMockInvoker();
        }

        private const string OriginalProfileName = "Original Profile Name set for first profile during test setup";
        private const string NewProfileName = "New Profile Name from Query";
        private ConversionProfile _originalFirstProfile;
        private ProfileSettingsViewModel _profileSettingsViewModel;
        private PdfCreatorSettings _settings;
        private ProfileSettingsMockInvoker _interactionInvoker;
        private IProfileChecker _profileChecker;
        private InteractionHelper<ProfileSettingsInteraction> _interactionHelper;

        public void CompareProfiles(ConversionProfile oldProfile, ConversionProfile newProfile)
        {
            //Equalize name, guid and properties before comparing
            oldProfile.Name = newProfile.Name;
            oldProfile.Guid = newProfile.Guid;
            oldProfile.Properties = newProfile.Properties;

            Assert.AreEqual(oldProfile, oldProfile,
                "Added profile is not equal to it's original (except Name, Guid and Properties");
        }

        private ProfileSettingsViewModel CreateProfileSettingsViewModel(PdfCreatorSettings settings)
        {
            var translator = Substitute.For<ITranslator>();
            translator.GetTranslation(Arg.Any<string>(), Arg.Any<string>()).Returns(x => x[1]);

            var interactionInvoker = Substitute.For<IInteractionInvoker>();
            var viewModelBundle = BuildViewModelBundle(interactionInvoker, translator);
            _profileChecker = Substitute.For<IProfileChecker>();
            _profileChecker.ProfileCheckDict(Arg.Any<IList<ConversionProfile>>(), Arg.Any<Accounts>()).Returns(new ActionResultDict());
            var viewModel = new ProfileSettingsViewModel(_interactionInvoker, translator, _profileChecker, viewModelBundle);

            var interaction = new ProfileSettingsInteraction(settings, new GpoSettingsDefaults());
            _interactionHelper = new InteractionHelper<ProfileSettingsInteraction>(viewModel, interaction);

            return viewModel;
        }

        private ProfileSettingsViewModelBundle BuildViewModelBundle(IInteractionInvoker invoker, ITranslator translator)
        {
            var documentTabViewModel = new DocumentTabViewModel(translator, invoker, Substitute.For<IFontHelper>());
            var saveTabViewModel = new SaveTabViewModel(translator, invoker);
            var autoSaveTabViewModel = new AutoSaveTabViewModel(translator, invoker);
            var actionsTabViewModel = new ActionsTabViewModel(translator);
            var imageTabViewModel = new ImageFormatsTabViewModel(translator);
            var pdfTabViewModel = new PdfTabViewModel(translator, invoker, Substitute.For<IFile>(), Substitute.For<IOpenFileInteractionHelper>());

            return new ProfileSettingsViewModelBundle(documentTabViewModel, saveTabViewModel, autoSaveTabViewModel, actionsTabViewModel, imageTabViewModel, pdfTabViewModel);
        }

        [Test]
        public void AddProfile_QueryProfileNameReturnsNewProfileName()
        {
            _profileSettingsViewModel = CreateProfileSettingsViewModel(_settings);

            var formerCurrentProfile = _profileSettingsViewModel.CurrentProfile;

            _interactionInvoker.HandleInputInteraction = interaction =>
            {
                interaction.Success = true;
                interaction.InputText = NewProfileName;
            };

            _profileSettingsViewModel.AddProfileCommand.Execute(null);

            Assert.AreEqual(2, _profileSettingsViewModel.Settings.ConversionProfiles.Count,
                "Wrong number of profiles after adding new profile");
            Assert.AreEqual(NewProfileName, _profileSettingsViewModel.CurrentProfile.Name,
                "Wrong Profile name, for added profile");

            var properties = _profileSettingsViewModel.CurrentProfile.Properties;

            Assert.IsTrue(properties.Deletable, "Deletable poperty was not true for added profile");
            Assert.IsTrue(properties.Editable, "Editable poperty was not true for added profile");
            Assert.IsTrue(properties.Renamable, "Renamable poperty was not true for added profile");

            Assert.AreNotEqual(formerCurrentProfile, _profileSettingsViewModel.CurrentProfile.Name,
                "New profile has same name as former profile");
            Assert.AreNotEqual(formerCurrentProfile, _profileSettingsViewModel.CurrentProfile.Guid,
                "New profile has same guid as former profile");

            //Equalize name, guid and properties before comparing
            formerCurrentProfile.Name = _profileSettingsViewModel.CurrentProfile.Name;
            formerCurrentProfile.Guid = _profileSettingsViewModel.CurrentProfile.Guid;
            formerCurrentProfile.Properties = _profileSettingsViewModel.CurrentProfile.Properties;

            CompareProfiles(formerCurrentProfile, _profileSettingsViewModel.CurrentProfile);
        }

        [Test]
        public void AddProfile_QueryProfileNameReturnsNull_AddingGetsCanceled()
        {
            _profileSettingsViewModel = CreateProfileSettingsViewModel(_settings);

            var formerCurrentProfile = _profileSettingsViewModel.CurrentProfile;

            _interactionInvoker.HandleInputInteraction = interaction => { interaction.Success = false; };

            _profileSettingsViewModel.AddProfileCommand.Execute(null);

            Assert.AreEqual(1, _profileSettingsViewModel.Settings.ConversionProfiles.Count,
                "Wrong number of profiles after canceled adding");
            Assert.AreEqual(formerCurrentProfile, _profileSettingsViewModel.CurrentProfile,
                "Current profile is not the former current profile before canceled adding");
        }

        [Test]
        public void ConversionProfilesAreSetProperly()
        {
            _profileSettingsViewModel = CreateProfileSettingsViewModel(_settings);
            Assert.AreEqual(_settings.ConversionProfiles, _profileSettingsViewModel.ConversionProfilesView,
                "Profiles in ConversionProfilesView are not the profiles from the current settings");
        }

        [Test]
        public void CurrentProfileIsNotRenameable_RenameProfileCanExecuteIsDisabled()
        {
            _profileSettingsViewModel = CreateProfileSettingsViewModel(_settings);

            _profileSettingsViewModel.CurrentProfile.Properties.Renamable = false;
            Assert.IsFalse(_profileSettingsViewModel.RenameProfileCommand.CanExecute(null),
                "RenameProfileCommand is executable for disabled renamable property");
        }

        [Test]
        public void CurrentProfileIsRenameable_RenameProfileCanExecuteIsEnabled()
        {
            _profileSettingsViewModel = CreateProfileSettingsViewModel(_settings);

            _profileSettingsViewModel.CurrentProfile.Properties.Renamable = true;
            Assert.IsTrue(_profileSettingsViewModel.RenameProfileCommand.CanExecute(null),
                "RenameProfileCommand is not executable for enabled renamable property");
        }

        [Test]
        public void CurrentProfileIsSetProperly()
        {
            _profileSettingsViewModel = CreateProfileSettingsViewModel(_settings);
            Assert.AreEqual(_settings.GetLastUsedOrFirstProfile(), _profileSettingsViewModel.CurrentProfile,
                "CurrentProfile after initialization is not last used or first profile from settings");
        }

        [Test]
        public void DeleteProfile_OnlyOneProfile_ProfileIsDeletable_DeleteProfileCommandIsNotExecutable()
        {
            _profileSettingsViewModel = CreateProfileSettingsViewModel(_settings);

            _profileSettingsViewModel.CurrentProfile.Properties.Deletable = true;
            Assert.IsFalse(_profileSettingsViewModel.DeleteProfileCommand.CanExecute(null),
                "Last profile must not be deleted even if deletable property is enabled.");
        }

        [Test]
        public void DeleteProfile_OnlyOneProfile_ProfileIsNotDeletable_DeleteProfileCommandIsNotExecutable()
        {
            _profileSettingsViewModel = CreateProfileSettingsViewModel(_settings);

            _profileSettingsViewModel.CurrentProfile.Properties.Deletable = false;
            Assert.IsFalse(_profileSettingsViewModel.DeleteProfileCommand.CanExecute(null),
                "Last profile must not be deleted even if deletable property is enabled.");
        }

        [Test]
        public void
            DeleteProfile_ProfileWithoutPrinterMapping_TwoProfiles_QueryDeleteProfileReturnsFalse_DeletionGetsCanceled_()
        {
            var secondProfile = new ConversionProfile();
            _settings.ConversionProfiles.Add(secondProfile);
            _settings.ApplicationSettings.LastUsedProfileGuid = secondProfile.Guid;

            _profileSettingsViewModel = CreateProfileSettingsViewModel(_settings);

            _interactionInvoker.HandleMessageInteraction = interaction =>
            {
                // this is called twice
                interaction.Response = MessageResponse.No;
            };

            _profileSettingsViewModel.DeleteProfileCommand.Execute(null);

            Assert.IsTrue(_profileSettingsViewModel.Settings.ConversionProfiles.Contains(secondProfile),
                "Current profile was deleted although deletion was canceled.");
            Assert.AreEqual(2, _profileSettingsViewModel.Settings.ConversionProfiles.Count,
                "Wrong number of profiles after canceled deletion.");
            Assert.AreEqual(secondProfile, _profileSettingsViewModel.CurrentProfile,
                "Wrong CurrentProfile after canceled deletion.");
        }

        [Test]
        public void DeleteProfile_TwoProfiles_CurrentProfileWithDisabledDeletable_DeleteProfileCommandIsNotExecutable()
        {
            var secondProfile = new ConversionProfile();
            secondProfile.Properties.Deletable = false;
            _settings.ConversionProfiles.Add(secondProfile);
            _settings.ApplicationSettings.LastUsedProfileGuid = secondProfile.Guid;

            _profileSettingsViewModel = CreateProfileSettingsViewModel(_settings);

            Assert.IsFalse(_profileSettingsViewModel.DeleteProfileCommand.CanExecute(null),
                "Can Execute is not disabled for profile with disabled deletable property.");
        }

        [Test]
        public void DeleteProfile_TwoProfiles_CurrentProfileWithEnabledDeletable_DeleteProfileCommandIsExecutable()
        {
            var secondProfile = new ConversionProfile();
            secondProfile.Properties.Deletable = true;
            _settings.ConversionProfiles.Add(secondProfile);
            _settings.ApplicationSettings.LastUsedProfileGuid = secondProfile.Guid;

            _profileSettingsViewModel = CreateProfileSettingsViewModel(_settings);

            Assert.IsTrue(_profileSettingsViewModel.DeleteProfileCommand.CanExecute(null),
                "Can Execute is not enabled for profile with enabled deletable property.");
        }

        [Test]
        public void
            DeleteProfile_TwoProfiles_ProfileWithoutPrinterMapping_QueryDeleteProfileReturnsTrue_CurrentProfileGetsDeleted
            ()
        {
            var secondProfile = new ConversionProfile();
            _settings.ConversionProfiles.Add(secondProfile);
            _settings.ApplicationSettings.LastUsedProfileGuid = secondProfile.Guid;

            _profileSettingsViewModel = CreateProfileSettingsViewModel(_settings);

            var firstCall = true;
            _interactionInvoker.HandleMessageInteraction = interaction =>
            {
                if (firstCall)
                    interaction.Response = MessageResponse.Yes;
                else
                    interaction.Response = MessageResponse.No;

                firstCall = false;
            };

            _profileSettingsViewModel.DeleteProfileCommand.Execute(null);

            Assert.IsFalse(_profileSettingsViewModel.Settings.ConversionProfiles.Contains(secondProfile),
                "Current profile has not been deleted.");
            Assert.AreEqual(1, _profileSettingsViewModel.Settings.ConversionProfiles.Count,
                "Wrong number of profiles after current profile should be deleted.");
            Assert.AreEqual(_originalFirstProfile, _profileSettingsViewModel.CurrentProfile,
                "CurrentProfile is not the original first profile.");
        }

        [Test]
        public void
            DeleteProfile_TwoProfiles_ProfileWithPrinterMapping_QueryDeleteProfileWithPrinterMappingReturnsFalse_DeletionGetsCanceled
            ()
        {
            var secondProfile = new ConversionProfile();
            _settings.ConversionProfiles.Add(secondProfile);
            _settings.ApplicationSettings.LastUsedProfileGuid = secondProfile.Guid;
            var printerMapping = new PrinterMapping();
            printerMapping.PrinterName = "fake printer";
            printerMapping.ProfileGuid = _settings.ApplicationSettings.LastUsedProfileGuid;
            _settings.ApplicationSettings.PrinterMappings.Add(printerMapping);

            _profileSettingsViewModel = CreateProfileSettingsViewModel(_settings);

            _interactionInvoker.HandleMessageInteraction = interaction => { interaction.Response = MessageResponse.No; };

            _profileSettingsViewModel.DeleteProfileCommand.Execute(null);

            Assert.IsTrue(_profileSettingsViewModel.Settings.ConversionProfiles.Contains(secondProfile),
                "Current profile was deleted although deletion was canceled.");
            Assert.AreEqual(2, _profileSettingsViewModel.Settings.ConversionProfiles.Count,
                "Wrong number of profiles after canceled deletion.");
            Assert.AreEqual(secondProfile, _profileSettingsViewModel.CurrentProfile,
                "Wrong CurrentProfile after canceled deletion.");
        }

        [Test]
        public void
            DeleteProfile_TwoProfiles_ProfileWithPrinterMapping_QueryDeleteProfileWithPrinterMappingReturnsTrue_CurrentProfileGetsDeleted
            ()
        {
            var secondProfile = new ConversionProfile();
            _settings.ConversionProfiles.Add(secondProfile);
            _settings.ApplicationSettings.LastUsedProfileGuid = secondProfile.Guid;
            var printerMapping = new PrinterMapping();
            printerMapping.PrinterName = "fake printer";
            printerMapping.ProfileGuid = _settings.ApplicationSettings.LastUsedProfileGuid;
            _settings.ApplicationSettings.PrinterMappings.Add(printerMapping);

            _profileSettingsViewModel = CreateProfileSettingsViewModel(_settings);

            _interactionInvoker.HandleMessageInteraction = interaction => { interaction.Response = MessageResponse.Yes; };

            _profileSettingsViewModel.DeleteProfileCommand.Execute(null);

            Assert.IsFalse(_profileSettingsViewModel.Settings.ConversionProfiles.Contains(secondProfile),
                "Current profile has not been deleted.");
            Assert.AreEqual(1, _profileSettingsViewModel.Settings.ConversionProfiles.Count,
                "Wrong number of profiles after current profile should be deleted.");
            Assert.AreEqual(_originalFirstProfile, _profileSettingsViewModel.CurrentProfile,
                "CurrentProfile is not the original first profile.");
        }

        [Test]
        public void OnClosing_IfClosedWithSaveButton_DoesNotQueryOrCancelClosing()
        {
            var viewModel = CreateProfileSettingsViewModel(_settings);
            viewModel.SaveButtonCommand.Execute(null);
            var eventArgs = new CancelEventArgs();
            viewModel.WindowClosingCommand.Execute(eventArgs);
            Assert.IsFalse(eventArgs.Cancel);
            Assert.IsEmpty(_interactionInvoker.InvokedInteractions);
        }

        [Test]
        public void OnClosing_IfNoChangesWereMade_DoesNotQueryOrCancelClosing()
        {
            var viewModel = CreateProfileSettingsViewModel(_settings);
            var eventArgs = new CancelEventArgs();
            viewModel.WindowClosingCommand.Execute(eventArgs);
            Assert.IsFalse(eventArgs.Cancel);
            Assert.IsEmpty(_interactionInvoker.InvokedInteractions);
        }

        [Test]
        public void OnClosing_WithUnsavedChanges_IfUserSelectsDiscard_DoesNotCancelClosing()
        {
            var viewModel = CreateProfileSettingsViewModel(_settings);
            viewModel.Interaction.Settings.ConversionProfiles.Add(new ConversionProfile());

            var queryDiscard = false;
            _interactionInvoker.HandleMessageInteraction += delegate(MessageInteraction interaction)
            {
                queryDiscard = true;
                interaction.Response = MessageResponse.Yes;
            };

            var eventArgs = new CancelEventArgs();
            viewModel.WindowClosingCommand.Execute(eventArgs);

            Assert.IsFalse(eventArgs.Cancel);
            Assert.IsTrue(queryDiscard);
            Assert.AreEqual(1, _interactionInvoker.InvokedInteractions.Count, "Unexpected interactions were invoked");
        }

        [Test]
        public void OnClosing_WithUnsavedChanges_IfUserSelectsKeep_CancelsClosing()
        {
            var viewModel = CreateProfileSettingsViewModel(_settings);
            viewModel.Interaction.Settings.ConversionProfiles.Add(new ConversionProfile());

            var queryDiscard = false;
            _interactionInvoker.HandleMessageInteraction += delegate(MessageInteraction interaction)
            {
                queryDiscard = true;
                interaction.Response = MessageResponse.No;
            };

            var eventArgs = new CancelEventArgs();
            viewModel.WindowClosingCommand.Execute(eventArgs);

            Assert.IsTrue(eventArgs.Cancel);
            Assert.IsTrue(queryDiscard);
            Assert.AreEqual(1, _interactionInvoker.InvokedInteractions.Count, "Unexpected interactions were invoked");
        }

        [Test]
        public void ProfileNameIsValid_Test()
        {
            _profileSettingsViewModel = CreateProfileSettingsViewModel(_settings);

            Assert.IsTrue(_profileSettingsViewModel.ProfilenameIsValid(NewProfileName).IsValid,
                "Not existing profile name must be valid.");
            Assert.IsTrue(_profileSettingsViewModel.ProfilenameIsValid(OriginalProfileName + " appendix").IsValid,
                "Existing profile with appendix must be valid.");
            Assert.IsFalse(_profileSettingsViewModel.ProfilenameIsValid(OriginalProfileName).IsValid,
                "Existing profile name must be invalid.");
        }

        [Test]
        public void RenameProfile_QueryProfileNameReturnsNewProfileName()
        {
            _profileSettingsViewModel = CreateProfileSettingsViewModel(_settings);
            _profileSettingsViewModel.CurrentProfile.Properties.Renamable = true;

            _interactionInvoker.HandleInputInteraction = interaction =>
            {
                interaction.Success = true;
                interaction.InputText = NewProfileName;
            };

            _profileSettingsViewModel.RenameProfileCommand.Execute(null);

            Assert.AreEqual(NewProfileName, _profileSettingsViewModel.CurrentProfile.Name,
                "Wrong Profile name after renaming");
        }

        [Test]
        public void RenameProfile_QueryProfileNameReturnsNull_RenamingGetsCanceled()
        {
            _profileSettingsViewModel = CreateProfileSettingsViewModel(_settings);

            var formerProfileName = _profileSettingsViewModel.CurrentProfile.Name;
            _profileSettingsViewModel.CurrentProfile.Properties.Renamable = true;

            _interactionInvoker.HandleInputInteraction = interaction => { interaction.Success = false; };

            var profileChangedWasCalled = false;

            _profileSettingsViewModel.RenameProfileCommand.Execute(null);

            Assert.IsFalse(profileChangedWasCalled);

            Assert.AreEqual(formerProfileName, _profileSettingsViewModel.CurrentProfile.Name,
                "Wrong Profile name after canceled renaming");
        }

        [Test]
        public void SaveCommand_NoDefectiveProfiles_ClosesWithoutInteraction()
        {
            var viewModel = CreateProfileSettingsViewModel(_settings);
            viewModel.CurrentProfile.Guid = "MyNewProfileGuid";
            viewModel.SaveButtonCommand.Execute(null);

            Assert.IsTrue(viewModel.Interaction.ApplySettings);
            Assert.IsTrue(_interactionHelper.InteractionIsFinished);
            Assert.AreEqual(viewModel.CurrentProfile.Guid, viewModel.Interaction.Settings.ApplicationSettings.LastUsedProfileGuid);
            Assert.AreEqual(0, _interactionInvoker.InvokedInteractions.Count, "Unexpected interactions were invoked");
        }

        [Test]
        public void SaveCommand_WithDefects_OnIgnore_FinishesInteraction()
        {
            var viewModel = CreateProfileSettingsViewModel(_settings);
            var dict = new ActionResultDict();
            dict.Add("some error", new ActionResult(ErrorCode.Conversion_UnknownError));

            _profileChecker.ProfileCheckDict(viewModel.Interaction.Settings.ConversionProfiles, Arg.Any<Accounts>()).Returns(dict);

            _interactionInvoker.HandleProfileProblemsInteraction += delegate(ProfileProblemsInteraction interaction) { interaction.IgnoreProblems = true; };

            viewModel.SaveButtonCommand.Execute(null);

            Assert.IsTrue(viewModel.Interaction.ApplySettings);
            Assert.IsTrue(_interactionHelper.InteractionIsFinished);
            Assert.AreEqual(viewModel.CurrentProfile.Guid, viewModel.Interaction.Settings.ApplicationSettings.LastUsedProfileGuid);

            Assert.AreEqual(1, _interactionInvoker.InvokedInteractions.Count, "Unexpected interactions were invoked");
        }

        [Test]
        public void SaveCommand_WithDefects_WithoutIgnore_DoesNotFinishInteraction()
        {
            var viewModel = CreateProfileSettingsViewModel(_settings);
            var dict = new ActionResultDict();
            dict.Add("some error", new ActionResult(ErrorCode.Conversion_UnknownError));

            _profileChecker.ProfileCheckDict(viewModel.Interaction.Settings.ConversionProfiles, Arg.Any<Accounts>()).Returns(dict);

            _interactionInvoker.HandleProfileProblemsInteraction += delegate(ProfileProblemsInteraction interaction) { interaction.IgnoreProblems = false; };

            viewModel.SaveButtonCommand.Execute(null);

            Assert.IsFalse(viewModel.Interaction.ApplySettings);
            Assert.IsFalse(_interactionHelper.InteractionIsFinished);
            Assert.AreEqual(1, _interactionInvoker.InvokedInteractions.Count, "Unexpected interactions were invoked");
        }
    }

    internal class ProfileSettingsMockInvoker : IInteractionInvoker
    {
        public IList<IInteraction> InvokedInteractions { get; } = new List<IInteraction>();
        public Action<InputInteraction> HandleInputInteraction { get; set; }

        public Action<MessageInteraction> HandleMessageInteraction { get; set; }

        public Action<ProfileProblemsInteraction> HandleProfileProblemsInteraction { get; set; }

        public void Invoke<T>(T interaction) where T : IInteraction
        {
            InvokedInteractions.Add(interaction);

            var inputInteraction = interaction as InputInteraction;
            if (inputInteraction != null)
            {
                HandleInputInteraction(inputInteraction);
            }

            var messageInteraction = interaction as MessageInteraction;
            if (messageInteraction != null)
            {
                HandleMessageInteraction(messageInteraction);
            }

            var profileProblemsInteraction = interaction as ProfileProblemsInteraction;
            if (profileProblemsInteraction != null)
            {
                HandleProfileProblemsInteraction(profileProblemsInteraction);
            }
        }

        public void InvokeNonBlocking<T>(T interaction, Action<T> callback) where T : IInteraction
        {
            throw new NotImplementedException();
        }

        public void InvokeNonBlocking<T>(T interaction) where T : IInteraction
        {
            throw new NotImplementedException();
        }

        public void Invoke(SaveFileInteraction interaction)
        {
            throw new NotImplementedException();
        }

        public void Invoke(FolderBrowserInteraction interaction)
        {
            throw new NotImplementedException();
        }

        public void Invoke(ColorInteraction interaction)
        {
            throw new NotImplementedException();
        }

        public void Invoke(FontInteraction interaction)
        {
            throw new NotImplementedException();
        }

        public void Invoke(OpenFileInteraction interaction)
        {
            throw new NotImplementedException();
        }
    }
}