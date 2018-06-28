using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Services.Macros;
using pdfforge.PDFCreator.Core.Workflow;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.Presentation;
using pdfforge.PDFCreator.UI.Presentation.Commands;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles;
using pdfforge.PDFCreator.UnitTest.UnitTestHelper;
using System.Linq;

namespace Presentation.UnitTest.Commands
{
    [TestFixture]
    public class EvaluateSettingsAndNotifyUserCommandTest
    {
        private EvaluateSettingsAndNotifyUserCommand _command;
        private UnitTestInteractionRequest _interactionRequest;
        private ICurrentSettingsProvider _currentSettingsProvider;
        private PdfCreatorSettings _settings;
        private IRegionHelper _regionHelper;
        private IProfileChecker _profileChecker;
        private ISettingsChanged _settingsChanged;
        private IAppSettingsChecker _appSettingsChecker;
        private WaitableCommandTester<EvaluateSettingsAndNotifyUserCommand> _commandTester;
        private EvaluateSettingsAndNotifyUserTranslation _translation;

        private ActionResult _actionResultWithError;
        private ActionResultDict _errorsInProfile;

        [SetUp]
        public void Setup()
        {
            _interactionRequest = new UnitTestInteractionRequest();

            _settings = new PdfCreatorSettings(null);
            _currentSettingsProvider = Substitute.For<ICurrentSettingsProvider>();
            _currentSettingsProvider.Settings.Returns(_settings);
            _currentSettingsProvider.Profiles.Returns(_settings.ConversionProfiles);
            _currentSettingsProvider.SelectedProfile.Returns(_settings.ConversionProfiles.FirstOrDefault());

            var translationUpdater = new DesignTimeTranslationUpdater();
            _regionHelper = Substitute.For<IRegionHelper>();

            _profileChecker = Substitute.For<IProfileChecker>();
            _profileChecker.CheckProfileList(_settings.ConversionProfiles, _settings.ApplicationSettings.Accounts)
                           .Returns(new ActionResultDict());

            _appSettingsChecker = Substitute.For<IAppSettingsChecker>();
            _appSettingsChecker.CheckDefaultViewers(Arg.Any<ApplicationSettings>())
                                 .Returns(new ActionResult());

            _settingsChanged = Substitute.For<ISettingsChanged>();

            _command = new EvaluateSettingsAndNotifyUserCommand(_interactionRequest, _currentSettingsProvider,
                translationUpdater, _regionHelper, _profileChecker,
                _appSettingsChecker, _settingsChanged);

            _commandTester = new WaitableCommandTester<EvaluateSettingsAndNotifyUserCommand>(_command);

            _translation = new EvaluateSettingsAndNotifyUserTranslation();

            _actionResultWithError = new ActionResult((ErrorCode)123456789);
            _errorsInProfile = new ActionResultDict();
            _errorsInProfile.Add("Some Profile", _actionResultWithError);
        }

        private void SetCurrentRegion(string region)
        {
            _regionHelper.CurrentRegionName.Returns(region);
        }

        private void SetErrorsInProfiles()
        {
            _profileChecker.CheckProfileList(_settings.ConversionProfiles, _settings.ApplicationSettings.Accounts)
                           .Returns(_errorsInProfile);
        }

        private void SetErrorsInAppSettings()
        {
            _appSettingsChecker.CheckDefaultViewers(_settings.ApplicationSettings)
                               .Returns(_actionResultWithError);
        }

        [Test]
        public void CanExecute_AlwaysTrue()
        {
            Assert.IsTrue(_command.CanExecute(null));
        }

        [Test]
        public void NoSpecificRegion_ChangesInSettings_ErrorsInProfiles_ErrorsInAppSettings__NoUserInteraction_CallsIsDoneWithSuccess()
        {
            SetCurrentRegion("No Specific Region");
            _settingsChanged.HaveChanged().Returns(true);
            SetErrorsInAppSettings(); //Should be ignored
            SetErrorsInProfiles(); //Should be ignored

            _command.Execute(null);

            Assert.IsTrue(_commandTester.IsDoneWasRaised);
            Assert.AreEqual(ResponseStatus.Success, _commandTester.LastResponseStatus);
            _interactionRequest.AssertWasNotRaised<MessageInteraction>();
        }

        [Test]
        public void NoSpecificRegion_NoChangesInSettings_ErrorsInProfiles_ErrorsInAppSettings__NoUserInteraction_CallsIsDoneWithSuccess()
        {
            SetCurrentRegion("No Specific Region");
            _settingsChanged.HaveChanged().Returns(false);
            SetErrorsInAppSettings(); //Should be ignored
            SetErrorsInProfiles(); //Should be ignored

            _command.Execute(null);

            Assert.IsTrue(_commandTester.IsDoneWasRaised);
            Assert.AreEqual(ResponseStatus.Success, _commandTester.LastResponseStatus);
            _interactionRequest.AssertWasNotRaised<MessageInteraction>();
        }

        [Test]
        public void SettingsViewRegion_NoChangesInSettings_NoErrorsInProfiles_NoErrorsInAppSettings__NoUserInteraction_CallsIsDoneWithSuccess()
        {
            SetCurrentRegion(MainRegionViewNames.SettingsView);
            _settingsChanged.HaveChanged().Returns(false);

            _command.Execute(null);

            Assert.IsTrue(_commandTester.IsDoneWasRaised);
            Assert.AreEqual(ResponseStatus.Success, _commandTester.LastResponseStatus);
            _interactionRequest.AssertWasNotRaised<MessageInteraction>();
        }

        [Test]
        public void SettingsViewRegion_ChangesInSettings_ErrorsInProfiles_NoErrorsInAppSettings__NoUserInteraction_CallsIsDoneWithSuccess()
        {
            SetCurrentRegion(MainRegionViewNames.SettingsView);
            _settingsChanged.HaveChanged().Returns(true);
            SetErrorsInProfiles(); //Should be ignored

            _command.Execute(null);

            Assert.IsTrue(_commandTester.IsDoneWasRaised);
            Assert.AreEqual(ResponseStatus.Success, _commandTester.LastResponseStatus);
            _interactionRequest.AssertWasNotRaised<MessageInteraction>();
        }

        [Test]
        public void SettingsViewRegion_NoChangesInSettings_ErrorsInProfiles_ErrorsInAppSettings__UserGetsNotifiedAboutAppSettingErrorsWithoutChanges()
        {
            SetCurrentRegion(MainRegionViewNames.SettingsView);
            _settingsChanged.HaveChanged().Returns(false);
            SetErrorsInProfiles(); //Should be ignored
            SetErrorsInAppSettings();

            _command.Execute(null);

            Assert.IsTrue(_commandTester.IsDoneWasRaised);

            var interaction = _interactionRequest.AssertWasRaised<MessageInteraction>();
            Assert.AreEqual(_translation.InvalidSettings, interaction.Text, "Interaction Text");
            Assert.AreEqual(_translation.PDFCreatorSettings, interaction.Title, "Interaction Title");
            Assert.AreEqual(MessageOptions.YesCancel, interaction.Buttons, "Interaction Buttons");
            Assert.AreEqual(MessageIcon.Warning, interaction.Icon, "Interaction Icon");
            Assert.AreEqual(_translation.DefaultViewer, interaction.ActionResultDict.Keys.First(), "Default Viewer Translation must be set as DictKey in ActionResultDict");
            Assert.AreEqual(_actionResultWithError, interaction.ActionResultDict[_translation.DefaultViewer], "ActionResult for DefaultViewers");
            Assert.AreEqual(_translation.ProceedAnyway, interaction.SecondText, "Interaction SecondText");
        }

        [Test]
        public void SettingsViewRegion_ChangesInSettings_ErrorsInProfiles_ErrorsInAppSettings__UserGetsNotifiedAboutAppSettingErrors()
        {
            SetCurrentRegion(MainRegionViewNames.SettingsView);
            _settingsChanged.HaveChanged().Returns(true);
            SetErrorsInProfiles(); //Should be ignored
            SetErrorsInAppSettings();

            _command.Execute(null);

            Assert.IsTrue(_commandTester.IsDoneWasRaised);

            var interaction = _interactionRequest.AssertWasRaised<MessageInteraction>();
            Assert.AreEqual(_translation.InvalidSettings, interaction.Text, "Interaction Text");
            Assert.AreEqual(_translation.PDFCreatorSettings, interaction.Title, "Interaction Title");
            Assert.AreEqual(MessageOptions.YesCancel, interaction.Buttons, "Interaction Buttons");
            Assert.AreEqual(MessageIcon.Warning, interaction.Icon, "Interaction Icon");
            Assert.AreEqual(_translation.DefaultViewer, interaction.ActionResultDict.Keys.First(), "Default Viewer Translation must be set as DictKey in ActionResultDict");
            Assert.AreEqual(_actionResultWithError, interaction.ActionResultDict[_translation.DefaultViewer], "ActionResult for DefaultViewers");
            Assert.AreEqual(_translation.ProceedAnyway, interaction.SecondText, "Interaction SecondText");
        }

        [Test]
        public void ProfilesViewRegion_NoChangesInSettings_NoErrorsInProfiles_NoErrorsInAppSettings__NoUserInteraction_CallsIsDoneWithSuccess()
        {
            SetCurrentRegion(MainRegionViewNames.ProfilesView);
            _settingsChanged.HaveChanged().Returns(false);

            _command.Execute(null);

            Assert.IsTrue(_commandTester.IsDoneWasRaised);
            Assert.AreEqual(ResponseStatus.Success, _commandTester.LastResponseStatus);
            _interactionRequest.AssertWasNotRaised<MessageInteraction>();
        }

        [Test]
        public void ProfilesView_ChangesInSettings_NoErrorsInProfiles_ErrorsInAppSettings__UserGetsNotifiedAboutChanges()
        {
            SetCurrentRegion(MainRegionViewNames.ProfilesView);
            _settingsChanged.HaveChanged().Returns(true);
            SetErrorsInAppSettings(); //Should be ignored

            _command.Execute(null);

            Assert.IsTrue(_commandTester.IsDoneWasRaised);

            var interaction = _interactionRequest.AssertWasRaised<MessageInteraction>();
            Assert.AreEqual(_translation.UnsavedChanges, interaction.Text, "Interaction Text");
            Assert.AreEqual(_translation.PDFCreatorSettings, interaction.Title, "Interaction Title");
            Assert.AreEqual(MessageOptions.YesNoCancel, interaction.Buttons, "Interaction Buttons");
            Assert.AreEqual(MessageIcon.Question, interaction.Icon, "Interaction Icon");
            Assert.IsNull(interaction.ActionResultDict, "Interaction ActionResultDict");
            Assert.IsNull(interaction.SecondText, "Interaction SecondText");
        }

        [Test]
        public void ProfilesViewRegion_NoChangesInSettings_ErrorsInProfiles_ErrorsInAppSettings__UserGetsNotifiedAboutProfileErrorsWithoutChanges()
        {
            SetCurrentRegion(MainRegionViewNames.ProfilesView);
            _settingsChanged.HaveChanged().Returns(false);
            SetErrorsInProfiles();
            SetErrorsInAppSettings(); //Should be ignored

            _command.Execute(null);

            Assert.IsTrue(_commandTester.IsDoneWasRaised);

            var interaction = _interactionRequest.AssertWasRaised<MessageInteraction>();
            Assert.AreEqual(_translation.InvalidSettings, interaction.Text, "Interaction Text");
            Assert.AreEqual(_translation.PDFCreatorSettings, interaction.Title, "Interaction Title");
            Assert.AreEqual(MessageOptions.YesCancel, interaction.Buttons, "Interaction Buttons");
            Assert.AreEqual(MessageIcon.Warning, interaction.Icon, "Interaction Icon");
            Assert.AreEqual(_errorsInProfile, interaction.ActionResultDict, "Interaction ActionResultDict");
            Assert.AreEqual(_translation.ProceedAnyway, interaction.SecondText, "Interaction SecondText");
        }

        [Test]
        public void ProfilesViewRegion_ChangesInSettings_ErrorsInProfiles_ErrorsInAppSettings__UserGetsNotifiedAboutProfileErrorsAndChanges()
        {
            SetCurrentRegion(MainRegionViewNames.ProfilesView);
            _settingsChanged.HaveChanged().Returns(true);
            SetErrorsInProfiles();
            SetErrorsInAppSettings(); //Should be ignored

            _command.Execute(null);

            Assert.IsTrue(_commandTester.IsDoneWasRaised);

            var interaction = _interactionRequest.AssertWasRaised<MessageInteraction>();
            Assert.AreEqual(_translation.InvalidSettings, interaction.Text, "Interaction Text");
            Assert.AreEqual(_translation.PDFCreatorSettings, interaction.Title, "Interaction Title");
            Assert.AreEqual(MessageOptions.YesNoCancel, interaction.Buttons, "Interaction Buttons");
            Assert.AreEqual(MessageIcon.Warning, interaction.Icon, "Interaction Icon");
            Assert.AreEqual(_errorsInProfile, interaction.ActionResultDict, "Interaction ActionResultDict");
            Assert.AreEqual(_translation.SaveAnyway, interaction.SecondText, "Interaction SecondText");
        }

        [Test]
        public void VerifyUserResponse_Yes__CallsIsDoneWithSuccess()
        {
            //force user interaction
            SetCurrentRegion(MainRegionViewNames.ProfilesView);
            SetErrorsInProfiles();
            //

            _interactionRequest.RegisterInteractionHandler<MessageInteraction>(i =>
            {
                i.Response = MessageResponse.Yes;
            });

            _command.Execute(null);

            _currentSettingsProvider.DidNotReceive().Reset();
            Assert.AreEqual(ResponseStatus.Success, _commandTester.LastResponseStatus);
        }

        [Test]
        public void VerifyUserResonse_No__ResetSettings_CallsIsDoneWithSkip()
        {
            //force user interaction
            SetCurrentRegion(MainRegionViewNames.ProfilesView);
            SetErrorsInProfiles();
            //

            _interactionRequest.RegisterInteractionHandler<MessageInteraction>(i =>
            {
                i.Response = MessageResponse.No;
            });

            _command.Execute(null);

            _currentSettingsProvider.Received().Reset();
            Assert.AreEqual(ResponseStatus.Skip, _commandTester.LastResponseStatus);
        }

        [Test]
        public void VerifyUserResponse_Cancel__CallsIsDoneWithCancel()
        {
            //force user interaction
            SetCurrentRegion(MainRegionViewNames.ProfilesView);
            SetErrorsInProfiles();
            //

            _interactionRequest.RegisterInteractionHandler<MessageInteraction>(i =>
            {
                i.Response = MessageResponse.Cancel;
            });

            _command.Execute(null);

            _currentSettingsProvider.DidNotReceive().Reset();
            Assert.AreEqual(ResponseStatus.Cancel, _commandTester.LastResponseStatus);
        }
    }
}
