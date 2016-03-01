using System.IO;
using NUnit.Framework;
using pdfforge.GpoReader;
using pdfforge.PDFCreator.Core.Settings;
using pdfforge.PDFCreator.Shared.Helper;
using pdfforge.PDFCreator.ViewModels;
using PDFCreator.UnitTest.ViewModels.Helper;
using Rhino.Mocks;

namespace PDFCreator.UnitTest.ViewModels
{
    [TestFixture]
    internal class ProfileSettingsViewModelTest
    {
        private const string OriginalProfileName = "Original Profile Name set for first profile during test setup";
        private const string NewProfileName = "New Profile Name from Query";
        private ConversionProfile _originalFirstProfile;
        private ProfileSettingsViewModel _profileSettingsViewModel;
        private PdfCreatorSettings _settings;
        private TranslationHelper _translationHelper;

        [SetUp]
        public void SetUp()
        {
            _settings = new PdfCreatorSettings(null);
            _originalFirstProfile = new ConversionProfile();
            _originalFirstProfile.Guid = "firstProfileGuid";
            _originalFirstProfile.Name = OriginalProfileName;
            _settings.ConversionProfiles.Add(_originalFirstProfile);

            _translationHelper = new TranslationHelper();
            _translationHelper.InitEmptyTranslator();
        }

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
            return new ProfileSettingsViewModel(settings, new GpoSettings(), _translationHelper);
        }

        #region General Tests

        [Test]
        public void ConversionProfilesAreSetProperly()
        {
            _profileSettingsViewModel = CreateProfileSettingsViewModel(_settings);
            Assert.AreEqual(_settings.ConversionProfiles, _profileSettingsViewModel.ConversionProfilesView,
                "Profiles in ConversionProfilesView are not the profiles from the current settings");
        }

        [Test]
        public void CurrentProfileIsSetProperly()
        {
            _profileSettingsViewModel = CreateProfileSettingsViewModel(_settings);
            Assert.AreEqual(_settings.GetLastUsedOrFirstProfile(), _profileSettingsViewModel.CurrentProfile,
                "CurrentProfile after initialization is not last used or first profile from settings");
        }

        [TestCase("Settings")]
        [TestCase("CurrentProfile")]
        [TestCase("ConversionProfilesView")]
        [TestCase("LowEncryptionEnabled")]
        [TestCase("MediumEncryptionEnabled")]
        [TestCase("HighEncryptionEnabled")]
        [TestCase("ExtendedPermissonsEnabled")]
        [TestCase("RestrictLowQualityPrintingEnabled")]
        [TestCase("AllowFillFormsEnabled")]
        [TestCase("AllowScreenReadersEnabled")]
        [TestCase("AllowEditingAssemblyEnabled")]
        public void EmptyViewModel_SetProfileSettings_RaisesPropertyChanged(string propertyName)
        {
            var profileSettingsViewModel = CreateProfileSettingsViewModel(_settings);
            var propertyListener = new PropertyChangedListenerMock(profileSettingsViewModel, propertyName);

            profileSettingsViewModel.Settings = _settings.Copy();

            Assert.IsTrue(propertyListener.WasCalled, "RaisePropertyChanged was not called for {0}", propertyName);
        }

        #endregion

        #region Rename Profile Tests

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
        public void RenameProfile_QueryProfileNameReturnsNewProfileName()
        {
            _profileSettingsViewModel = CreateProfileSettingsViewModel(_settings);

            var formerProfileName = _profileSettingsViewModel.CurrentProfile.Name;
            _profileSettingsViewModel.CurrentProfile.Properties.Renamable = true;

            var functionsStub = MockRepository.GenerateStub<ITestFunctions>();
            functionsStub.Stub(x => x.ReturnsStringWithStringParameter(formerProfileName)).Return(NewProfileName);
            _profileSettingsViewModel.QueryProfileName = functionsStub.ReturnsStringWithStringParameter;
            _profileSettingsViewModel.UpdateLayoutProfilesBoxAction = functionsStub.VoidFunctionWithoutParameters;

            _profileSettingsViewModel.RenameProfileCommand.Execute(null);

            functionsStub.AssertWasCalled(x => x.ReturnsStringWithStringParameter(formerProfileName),
                options => options.Repeat.Once()); //QueryProfileName
            functionsStub.AssertWasCalled(x => x.VoidFunctionWithoutParameters(), options => options.Repeat.Once());
            //UpdateLayoutProfilesBoxAction

            Assert.AreEqual(NewProfileName, _profileSettingsViewModel.CurrentProfile.Name,
                "Wrong Profile name after renaming");
        }

        [Test]
        public void RenameProfile_QueryProfileNameReturnsNull_RenamingGetsCanceled()
        {
            _profileSettingsViewModel = CreateProfileSettingsViewModel(_settings);

            var formerProfileName = _profileSettingsViewModel.CurrentProfile.Name;
            _profileSettingsViewModel.CurrentProfile.Properties.Renamable = true;

            var functionsStub = MockRepository.GenerateStub<ITestFunctions>();
            functionsStub.Stub(x => x.ReturnsStringWithStringParameter(formerProfileName)).Return(null);
            _profileSettingsViewModel.QueryProfileName = functionsStub.ReturnsStringWithStringParameter;
            _profileSettingsViewModel.UpdateLayoutProfilesBoxAction = functionsStub.VoidFunctionWithoutParameters;

            _profileSettingsViewModel.RenameProfileCommand.Execute(null);

            functionsStub.AssertWasCalled(x => x.ReturnsStringWithStringParameter(formerProfileName),
                options => options.Repeat.Once()); //QueryProfileName
            functionsStub.AssertWasNotCalled(x => x.VoidFunctionWithoutParameters()); //UpdateLayoutProfilesBoxAction

            Assert.AreEqual(formerProfileName, _profileSettingsViewModel.CurrentProfile.Name,
                "Wrong Profile name after canceled renaming");
        }

        #endregion

        #region Add Profile Tests

        [Test]
        public void AddProfile_QueryProfileNameReturnsNewProfileName()
        {
            _profileSettingsViewModel = CreateProfileSettingsViewModel(_settings);

            var formerCurrentProfile = _profileSettingsViewModel.CurrentProfile;

            var functionsStub = MockRepository.GenerateStub<ITestFunctions>();
            functionsStub.Stub(x => x.ReturnsStringWithStringParameter(null)).Return(NewProfileName);
            _profileSettingsViewModel.QueryProfileName = functionsStub.ReturnsStringWithStringParameter;

            _profileSettingsViewModel.AddProfileCommand.Execute(null);

            functionsStub.AssertWasCalled(x => x.ReturnsStringWithStringParameter(null),
                options => options.Repeat.Once()); //QueryProfileName

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

            var functionsStub = MockRepository.GenerateStub<ITestFunctions>();
            functionsStub.Stub(x => x.ReturnsStringWithStringParameter(null)).Return(null);
            _profileSettingsViewModel.QueryProfileName = functionsStub.ReturnsStringWithStringParameter;

            _profileSettingsViewModel.AddProfileCommand.Execute(null);

            functionsStub.AssertWasCalled(x => x.ReturnsStringWithStringParameter(null),
                options => options.Repeat.Once()); //QueryProfileName

            Assert.AreEqual(1, _profileSettingsViewModel.Settings.ConversionProfiles.Count,
                "Wrong number of profiles after canceled adding");
            Assert.AreEqual(formerCurrentProfile, _profileSettingsViewModel.CurrentProfile,
                "Current profile is not the former current profile before canceled adding");
        }

        #endregion

        #region Delete Profile Tests

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

            var functionsStub = MockRepository.GenerateStub<ITestFunctions>();
            functionsStub.Stub(x => x.ReturnsBoolWithoutParameters()).Return(true);
            _profileSettingsViewModel.QueryDeleteProfile = functionsStub.ReturnsBoolWithoutParameters;
            functionsStub.Stub(x => x.ReturnsBoolWithTwoStringParameters(null, null)).IgnoreArguments().Return(false);
            _profileSettingsViewModel.QueryDeleteProfileWithPrinterMapping =
                functionsStub.ReturnsBoolWithTwoStringParameters;

            _profileSettingsViewModel.DeleteProfileCommand.Execute(null);

            functionsStub.AssertWasCalled(x => x.ReturnsBoolWithoutParameters(), options => options.Repeat.Once());
            //QueryDeleteProfile
            functionsStub.AssertWasNotCalled(x => x.ReturnsBoolWithTwoStringParameters(null, null),
                options => options.IgnoreArguments()); //QueryDeleteProfileWithPrinterMapping

            Assert.IsFalse(_profileSettingsViewModel.Settings.ConversionProfiles.Contains(secondProfile),
                "Current profile has not been deleted.");
            Assert.AreEqual(1, _profileSettingsViewModel.Settings.ConversionProfiles.Count,
                "Wrong number of profiles after current profile should be deleted.");
            Assert.AreEqual(_originalFirstProfile, _profileSettingsViewModel.CurrentProfile,
                "CurrentProfile is not the original first profile.");
        }

        [Test]
        public void
            DeleteProfile_ProfileWithoutPrinterMapping_TwoProfiles_QueryDeleteProfileReturnsFalse_DeletionGetsCanceled_()
        {
            var secondProfile = new ConversionProfile();
            _settings.ConversionProfiles.Add(secondProfile);
            _settings.ApplicationSettings.LastUsedProfileGuid = secondProfile.Guid;

            _profileSettingsViewModel = CreateProfileSettingsViewModel(_settings);

            var functionsStub = MockRepository.GenerateStub<ITestFunctions>();
            functionsStub.Stub(x => x.ReturnsBoolWithoutParameters()).Return(false);
            _profileSettingsViewModel.QueryDeleteProfile = functionsStub.ReturnsBoolWithoutParameters;
            functionsStub.Stub(x => x.ReturnsBoolWithTwoStringParameters(null, null)).IgnoreArguments().Return(false);
            _profileSettingsViewModel.QueryDeleteProfileWithPrinterMapping =
                functionsStub.ReturnsBoolWithTwoStringParameters;

            _profileSettingsViewModel.DeleteProfileCommand.Execute(null);

            functionsStub.AssertWasCalled(x => x.ReturnsBoolWithoutParameters(), options => options.Repeat.Once());
            //QueryDeleteProfile
            functionsStub.AssertWasNotCalled(x => x.ReturnsBoolWithTwoStringParameters(null, null),
                options => options.IgnoreArguments()); //QueryDeleteProfileWithPrinterMapping

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

            var functionsStub = MockRepository.GenerateStub<ITestFunctions>();
            functionsStub.Stub(x => x.ReturnsBoolWithoutParameters()).Return(false);
            _profileSettingsViewModel.QueryDeleteProfile = functionsStub.ReturnsBoolWithoutParameters;
            functionsStub.Stub(x => x.ReturnsBoolWithTwoStringParameters(secondProfile.Name, printerMapping.PrinterName))
                .Return(true);
            _profileSettingsViewModel.QueryDeleteProfileWithPrinterMapping =
                functionsStub.ReturnsBoolWithTwoStringParameters;

            _profileSettingsViewModel.DeleteProfileCommand.Execute(null);

            functionsStub.AssertWasNotCalled(x => x.ReturnsBoolWithoutParameters()); //QueryDeleteProfile
            functionsStub.AssertWasCalled(
                x => x.ReturnsBoolWithTwoStringParameters(secondProfile.Name, printerMapping.PrinterName),
                options => options.Repeat.Once()); //QueryDeleteProfileWithPrinterMapping

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

            var functionsStub = MockRepository.GenerateStub<ITestFunctions>();
            functionsStub.Stub(x => x.ReturnsBoolWithoutParameters()).Return(false);
            _profileSettingsViewModel.QueryDeleteProfile = functionsStub.ReturnsBoolWithoutParameters;
            functionsStub.Stub(x => x.ReturnsBoolWithTwoStringParameters(secondProfile.Name, printerMapping.PrinterName))
                .Return(false);
            _profileSettingsViewModel.QueryDeleteProfileWithPrinterMapping =
                functionsStub.ReturnsBoolWithTwoStringParameters;

            _profileSettingsViewModel.DeleteProfileCommand.Execute(null);

            functionsStub.AssertWasNotCalled(x => x.ReturnsBoolWithoutParameters()); //QueryDeleteProfile
            functionsStub.AssertWasCalled(
                x => x.ReturnsBoolWithTwoStringParameters(secondProfile.Name, printerMapping.PrinterName),
                options => options.Repeat.Once()); //QueryDeleteProfileWithPrinterMapping

            Assert.IsTrue(_profileSettingsViewModel.Settings.ConversionProfiles.Contains(secondProfile),
                "Current profile was deleted although deletion was canceled.");
            Assert.AreEqual(2, _profileSettingsViewModel.Settings.ConversionProfiles.Count,
                "Wrong number of profiles after canceled deletion.");
            Assert.AreEqual(secondProfile, _profileSettingsViewModel.CurrentProfile,
                "Wrong CurrentProfile after canceled deletion.");
        }

        #endregion

        #region Close with Save Tests

        [Test]
        public void CloseWithSave_NoChangesInProfiles_NoQueries_CloseViewActionGetsCalled()
        {
            var secondProfile = new ConversionProfile();
            _settings.ConversionProfiles.Add(secondProfile);
            _settings.ApplicationSettings.LastUsedProfileGuid = secondProfile.Guid;

            _profileSettingsViewModel = CreateProfileSettingsViewModel(_settings.Copy());

            var functionsStub = MockRepository.GenerateStub<ITestFunctions>();
            functionsStub.Stub(x => x.ReturnsBoolWithActionResultDictParameter(null)).IgnoreArguments().Return(false);
            _profileSettingsViewModel.QueryIgnoreDefectiveProfiles =
                functionsStub.ReturnsBoolWithActionResultDictParameter;
            functionsStub.Stub(x => x.VoidFunctionWithBoolParameters(true));
            _profileSettingsViewModel.CloseViewAction = functionsStub.VoidFunctionWithBoolParameters;

            _profileSettingsViewModel.SaveButtonCommand.Execute(null);

            functionsStub.AssertWasNotCalled(x => x.ReturnsBoolWithActionResultDictParameter(null));
            //QueryIgnoreDefectiveProfiles
            functionsStub.AssertWasCalled(x => x.VoidFunctionWithBoolParameters(true), options => options.Repeat.Once());
            //CloseViewAction

            Assert.AreEqual(_profileSettingsViewModel.Settings, _settings, "Settings changed while closing with save");
        }

        [Test]
        public void
            CloseWithSave_NoChangesInProfiles_DefectiveProfile_QueryIgnoreDefectiveProfilesReturnsTrue_CloseViewActionGetsCalled
            ()
        {
            var secondProfile = new ConversionProfile();
            _settings.ConversionProfiles.Add(secondProfile);
            _settings.ApplicationSettings.LastUsedProfileGuid = secondProfile.Guid;
            _settings.ConversionProfiles[0].CoverPage.Enabled = true;
            _settings.ConversionProfiles[0].CoverPage.File = "String to a not existing path to get defective profile";

            _profileSettingsViewModel = CreateProfileSettingsViewModel(_settings.Copy());

            var functionsStub = MockRepository.GenerateStub<ITestFunctions>();
            functionsStub.Stub(x => x.ReturnsBoolWithActionResultDictParameter(null)).IgnoreArguments().Return(true);
            _profileSettingsViewModel.QueryIgnoreDefectiveProfiles =
                functionsStub.ReturnsBoolWithActionResultDictParameter;
            functionsStub.Stub(x => x.VoidFunctionWithBoolParameters(true));
            _profileSettingsViewModel.CloseViewAction = functionsStub.VoidFunctionWithBoolParameters;

            _profileSettingsViewModel.SaveButtonCommand.Execute(null);

            functionsStub.AssertWasCalled(x => x.ReturnsBoolWithActionResultDictParameter(null),
                options => options.IgnoreArguments().Repeat.Once()); //QueryIgnoreDefectiveProfiles
            functionsStub.AssertWasCalled(x => x.VoidFunctionWithBoolParameters(true), options => options.Repeat.Once());
            //CloseViewAction

            Assert.AreEqual(_profileSettingsViewModel.Settings, _settings,
                "Unchanged settings after saving are not the original settings.");
        }

        [Test]
        public void
            CloseWithSave_NoChangesInProfiles_DefectiveProfile_QueryIgnoreDefectiveProfilesReturnsFalse_CloseViewActionGetsBlocked_CheckForClosingDoesNotCallQueryDiscardChanges
            ()
        {
            var secondProfile = new ConversionProfile();
            _settings.ConversionProfiles.Add(secondProfile);
            _settings.ApplicationSettings.LastUsedProfileGuid = secondProfile.Guid;
            _settings.ConversionProfiles[0].CoverPage.Enabled = true;
            _settings.ConversionProfiles[0].CoverPage.File = "String to a not existing path to get defective profile";

            _profileSettingsViewModel = CreateProfileSettingsViewModel(_settings.Copy());

            var functionsStub = MockRepository.GenerateStub<ITestFunctions>();
            functionsStub.Stub(x => x.ReturnsBoolWithActionResultDictParameter(null)).IgnoreArguments().Return(false);
            _profileSettingsViewModel.QueryIgnoreDefectiveProfiles =
                functionsStub.ReturnsBoolWithActionResultDictParameter;
            functionsStub.Stub(x => x.VoidFunctionWithBoolParameters(true));
            _profileSettingsViewModel.CloseViewAction = functionsStub.VoidFunctionWithBoolParameters;
            _profileSettingsViewModel.QueryDiscardChanges = functionsStub.ReturnsBoolWithoutParameters;

            _profileSettingsViewModel.SaveButtonCommand.Execute(null);

            functionsStub.AssertWasCalled(x => x.ReturnsBoolWithActionResultDictParameter(null),
                options => options.IgnoreArguments().Repeat.Once()); //QueryIgnoreDefectiveProfiles
            functionsStub.AssertWasNotCalled(x => x.VoidFunctionWithBoolParameters(true)); //CloseViewAction

            Assert.AreEqual(_profileSettingsViewModel.Settings, _settings,
                "Unchanged settings after saving are not the original settings.");

            _profileSettingsViewModel.CheckForClosingWindowAndRestoreTheSettings();
            functionsStub.AssertWasNotCalled(x => x.ReturnsBoolWithoutParameters()); //QueryDiscardChanges
        }

        [Test]
        public void CloseWithSave_ChangesInProfiles_NoDefectiveProfile_CloseViewActionGetsCalled()
        {
            var secondProfile = new ConversionProfile();
            _settings.ConversionProfiles.Add(secondProfile);
            _settings.ApplicationSettings.LastUsedProfileGuid = secondProfile.Guid;

            _profileSettingsViewModel = CreateProfileSettingsViewModel(_settings.Copy());

            var functionsStub = MockRepository.GenerateStub<ITestFunctions>();
            functionsStub.Stub(x => x.ReturnsBoolWithActionResultDictParameter(null)).IgnoreArguments().Return(false);
            _profileSettingsViewModel.QueryIgnoreDefectiveProfiles =
                functionsStub.ReturnsBoolWithActionResultDictParameter;
            functionsStub.Stub(x => x.VoidFunctionWithBoolParameters(true));
            _profileSettingsViewModel.CloseViewAction = functionsStub.VoidFunctionWithBoolParameters;

            _profileSettingsViewModel.Settings.ConversionProfiles[0].AuthorTemplate +=
                "Change the Author to change the Settings";
            var changedSettings = _profileSettingsViewModel.Settings.Copy();

            _profileSettingsViewModel.SaveButtonCommand.Execute(null);

            functionsStub.AssertWasNotCalled(x => x.ReturnsBoolWithActionResultDictParameter(null));
            //QueryIgnoreDefectiveProfiles
            functionsStub.AssertWasCalled(x => x.VoidFunctionWithBoolParameters(true), options => options.Repeat.Once());
            //CloseViewAction

            Assert.AreEqual(_profileSettingsViewModel.Settings, changedSettings,
                "Settings after saving are not the changed settings.");
        }

        [Test]
        public void
            CloseWithSave_ChangesInProfiles_DefectiveProfile_QueryIgnoreDefectiveProfilesReturnsTrue_CloseViewActionGetsCalled
            ()
        {
            var secondProfile = new ConversionProfile();
            _settings.ConversionProfiles.Add(secondProfile);
            _settings.ApplicationSettings.LastUsedProfileGuid = secondProfile.Guid;

            _profileSettingsViewModel = CreateProfileSettingsViewModel(_settings.Copy());

            var functionsStub = MockRepository.GenerateStub<ITestFunctions>();
            functionsStub.Stub(x => x.ReturnsBoolWithActionResultDictParameter(null)).IgnoreArguments().Return(true);
            _profileSettingsViewModel.QueryIgnoreDefectiveProfiles =
                functionsStub.ReturnsBoolWithActionResultDictParameter;
            functionsStub.Stub(x => x.VoidFunctionWithBoolParameters(true));
            _profileSettingsViewModel.CloseViewAction = functionsStub.VoidFunctionWithBoolParameters;

            _profileSettingsViewModel.Settings.ConversionProfiles[0].AuthorTemplate +=
                "Change the Author to change the Settings";
            _profileSettingsViewModel.Settings.ConversionProfiles[0].CoverPage.Enabled = true;
            _profileSettingsViewModel.Settings.ConversionProfiles[0].CoverPage.File =
                "String to a not existing path to get defective profile";

            var changedSettings = _profileSettingsViewModel.Settings.Copy();

            _profileSettingsViewModel.SaveButtonCommand.Execute(null);

            functionsStub.AssertWasCalled(x => x.ReturnsBoolWithActionResultDictParameter(null),
                options => options.IgnoreArguments().Repeat.Once()); //QueryIgnoreDefectiveProfiles
            functionsStub.AssertWasCalled(x => x.VoidFunctionWithBoolParameters(true), options => options.Repeat.Once());
            //CloseViewAction

            Assert.AreEqual(_profileSettingsViewModel.Settings, changedSettings,
                "Settings after saving are not the changed settings.");
        }

        [Test]
        public void
            CloseWithSave_ChangesInProfiles_DefectiveProfile_QueryIgnoreDefectiveProfilesReturnsFalse_CloseViewActionGetsBlocked_CheckForClosingCallsQueryDiscardChanges
            ()
        {
            var secondProfile = new ConversionProfile();
            _settings.ConversionProfiles.Add(secondProfile);
            _settings.ApplicationSettings.LastUsedProfileGuid = secondProfile.Guid;

            _profileSettingsViewModel = CreateProfileSettingsViewModel(_settings.Copy());

            var functionsStub = MockRepository.GenerateStub<ITestFunctions>();
            functionsStub.Stub(x => x.ReturnsBoolWithActionResultDictParameter(null)).IgnoreArguments().Return(false);
            _profileSettingsViewModel.QueryIgnoreDefectiveProfiles =
                functionsStub.ReturnsBoolWithActionResultDictParameter;
            functionsStub.Stub(x => x.VoidFunctionWithBoolParameters(true));
            _profileSettingsViewModel.CloseViewAction = functionsStub.VoidFunctionWithBoolParameters;
            _profileSettingsViewModel.QueryDiscardChanges = functionsStub.ReturnsBoolWithoutParameters;

            _profileSettingsViewModel.Settings.ConversionProfiles[0].AuthorTemplate +=
                "Change the Author to change the Settings";
            _profileSettingsViewModel.Settings.ConversionProfiles[0].CoverPage.Enabled = true;
            _profileSettingsViewModel.Settings.ConversionProfiles[0].CoverPage.File =
                "String to a not existing path to get defective profile";

            var changedSettings = _profileSettingsViewModel.Settings.Copy();

            _profileSettingsViewModel.SaveButtonCommand.Execute(null);

            functionsStub.AssertWasCalled(x => x.ReturnsBoolWithActionResultDictParameter(null),
                options => options.IgnoreArguments().Repeat.Once()); //QueryIgnoreDefectiveProfiles
            functionsStub.AssertWasNotCalled(x => x.VoidFunctionWithBoolParameters(true),
                options => options.IgnoreArguments()); //CloseViewAction

            Assert.AreEqual(_profileSettingsViewModel.Settings, changedSettings,
                "Settings after canceled saving (edit defective profiles) are not the changed settings.");

            _profileSettingsViewModel.CheckForClosingWindowAndRestoreTheSettings();
            functionsStub.AssertWasCalled(x => x.ReturnsBoolWithoutParameters(), options => options.Repeat.Once());
            //QueryDiscardChanges    
        }

        #endregion

        #region Check For Closing Tests

        [Test]
        public void CheckForClosing_NoChangesInProfiles_NoQueries()
        {
            var secondProfile = new ConversionProfile();
            _settings.ConversionProfiles.Add(secondProfile);
            _settings.ApplicationSettings.LastUsedProfileGuid = secondProfile.Guid;

            _profileSettingsViewModel = CreateProfileSettingsViewModel(_settings.Copy());

            var functionsStub = MockRepository.GenerateStub<ITestFunctions>();
            functionsStub.Stub(x => x.ReturnsBoolWithoutParameters()).Return(false);
            _profileSettingsViewModel.QueryDiscardChanges = functionsStub.ReturnsBoolWithoutParameters;

            Assert.IsTrue(_profileSettingsViewModel.CheckForClosingWindowAndRestoreTheSettings(),
                "Closing the window should not be blocked.");

            functionsStub.AssertWasNotCalled(x => x.ReturnsBoolWithoutParameters()); //QueryDiscardChanges
            Assert.AreEqual(_profileSettingsViewModel.Settings, _settings,
                "Settings after closing are not the original Settings.");
        }

        [Test]
        public void CheckForClosing_ChangesInProfiles_QueryDiscardChangesReturnsTrue()
        {
            var secondProfile = new ConversionProfile();
            _settings.ConversionProfiles.Add(secondProfile);
            _settings.ApplicationSettings.LastUsedProfileGuid = secondProfile.Guid;

            _profileSettingsViewModel = CreateProfileSettingsViewModel(_settings.Copy());

            var functionsStub = MockRepository.GenerateStub<ITestFunctions>();
            functionsStub.Stub(x => x.ReturnsBoolWithoutParameters()).Return(true);
            _profileSettingsViewModel.QueryDiscardChanges = functionsStub.ReturnsBoolWithoutParameters;

            _profileSettingsViewModel.Settings.ConversionProfiles[0].AuthorTemplate +=
                "Change the Author to change the Settings";

            Assert.IsTrue(_profileSettingsViewModel.CheckForClosingWindowAndRestoreTheSettings(),
                "Closing the window should not be blocked.");
            functionsStub.AssertWasCalled(x => x.ReturnsBoolWithoutParameters(), options => options.Repeat.Once());
            //QueryDiscardChanges
            Assert.AreEqual(_profileSettingsViewModel.Settings, _settings,
                "Settings after closing are not the original Settings.");
        }

        [Test]
        public void CheckForClosing_ChangesInProfiles_QueryDiscardChangesReturnsFalse_ClosingGetsBlocked()
        {
            var secondProfile = new ConversionProfile();
            _settings.ConversionProfiles.Add(secondProfile);
            _settings.ApplicationSettings.LastUsedProfileGuid = secondProfile.Guid;

            _profileSettingsViewModel = CreateProfileSettingsViewModel(_settings.Copy());

            var functionsStub = MockRepository.GenerateStub<ITestFunctions>();
            functionsStub.Stub(x => x.ReturnsBoolWithoutParameters()).Return(false);
            _profileSettingsViewModel.QueryDiscardChanges = functionsStub.ReturnsBoolWithoutParameters;

            _profileSettingsViewModel.Settings.ConversionProfiles[0].AuthorTemplate +=
                "Change the Author to change the Settings";
            var changedSettings = _profileSettingsViewModel.Settings.Copy();

            Assert.IsFalse(_profileSettingsViewModel.CheckForClosingWindowAndRestoreTheSettings(),
                "Closing the window should be blocked.");
            functionsStub.AssertWasCalled(x => x.ReturnsBoolWithoutParameters(), options => options.Repeat.Once());
            //QueryDiscardChanges
            Assert.AreEqual(_profileSettingsViewModel.Settings, changedSettings,
                "Settings after blocked closing are not the changed settings.");
        }

        #endregion
    }
}