using NSubstitute;
using NUnit.Framework;
using pdfforge.DataStorage.Storage;
using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.Presentation.Assistants;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.General;
using pdfforge.PDFCreator.UnitTest.UnitTestHelper;
using pdfforge.PDFCreator.Utilities.Process;
using pdfforge.PDFCreator.Utilities.Threading;
using Prism.Events;
using System;
using System.ComponentModel;
using System.Windows;
using Translatable;

namespace Presentation.UnitTest.UserControls.GeneralSettings
{
    [TestFixture]
    public class UpdateIntervalSettingsViewModelTest
    {
        [SetUp]
        public void Setup()
        {
            _translationUpdater = new TranslationUpdater(new TranslationFactory(), new ThreadManager());

            _processStarter = Substitute.For<IProcessStarter>();
            _gpoSettings = Substitute.For<IGpoSettings>();
            _currentSettingsProvider = Substitute.For<ICurrentSettingsProvider>();
            _currentSettingsProvider.Settings.Returns(new PdfCreatorSettings(new IniStorage()));
            _updateAssistant = Substitute.For<IUpdateAssistant>();
            _interactionRequest = Substitute.For<IInteractionRequest>();
            _eventAggregator = new EventAggregator();
        }

        [TearDown]
        public void TearDown()
        {
            _translationUpdater.Clear();
        }

        private ITranslationUpdater _translationUpdater;
        private IGpoSettings _gpoSettings;
        private ICurrentSettingsProvider _currentSettingsProvider;
        private IProcessStarter _processStarter;
        private IUpdateAssistant _updateAssistant;
        private IInteractionRequest _interactionRequest;
        private IEventAggregator _eventAggregator;

        private UpdateIntervalSettingsViewModel BuildViewModel()
        {
            return new UpdateIntervalSettingsViewModel(_updateAssistant, _processStarter, _currentSettingsProvider, _gpoSettings, _translationUpdater, _eventAggregator, _interactionRequest);
        }

        [Test]
        public void UpdateCheckCommandIsSet()
        {
            var viewModel = BuildViewModel();
            Assert.NotNull(viewModel.UpdateCheckCommand);
        }

        [Test]
        public void VisitWebsiteCommandIsSet()
        {
            var viewModel = BuildViewModel();
            Assert.NotNull(viewModel.VisitWebsiteCommand);
        }

        [TestCase(true, Visibility.Visible)]
        [TestCase(false, Visibility.Collapsed)]
        public void SetAdminInOsHelper_RequireUACVisibilty_GetVisibility(bool areUpdatesEnabled, Visibility visibility)
        {
            _updateAssistant.UpdatesEnabled.Returns(x => areUpdatesEnabled);
            var viewModel = BuildViewModel();
            Assert.AreEqual(viewModel.UpdateCheckControlVisibility, visibility);
        }

        [Test]
        public void GetPDFFOrgeWebSiteUrl_FitsUrls()
        {
            var viewModel = BuildViewModel();
            Assert.AreEqual(Urls.PdfforgeWebsiteUrl, viewModel.PdfforgeWebsiteUrl);
        }

        [Test]
        public void ApplicationSettingsNotSet_RequestDisplayUpdateWarning_ReturnsFalse()
        {
            _currentSettingsProvider.Settings.ApplicationSettings = null;
            var viewModel = BuildViewModel();

            Assert.IsFalse(viewModel.DisplayUpdateWarning);
        }

        [Test]
        public void ApplicationSettingUpdateIntervalIsNotNever_RequestDisplayUpdateWarning_ReturnsFalse()
        {
            _currentSettingsProvider.Settings.ApplicationSettings.UpdateInterval = UpdateInterval.Daily;
            var viewModel = BuildViewModel();

            Assert.IsFalse(viewModel.DisplayUpdateWarning);
        }

        [Test]
        public void ApplicationSettingUpdateIntervalIsNever_RequestDisplayUpdateWarning_ReturnsTrue()
        {
            _currentSettingsProvider.Settings.ApplicationSettings.UpdateInterval = UpdateInterval.Never;
            var viewModel = BuildViewModel();

            Assert.IsTrue(viewModel.DisplayUpdateWarning);
        }

        [Test]
        public void ApplicationSettingIsNull_GetCurrentUpdateInterval_GetUpdateIntervalWeekly()
        {
            _currentSettingsProvider.Settings.ApplicationSettings = null;
            var viewModel = BuildViewModel();

            Assert.AreEqual(viewModel.CurrentUpdateInterval, UpdateInterval.Weekly);
        }

        [Test]
        public void GpoSettingsUpdateIntervalIsNull_GetCurrentUpdateInterval_GetUpdateFromApplicationSettings()
        {
            _gpoSettings.UpdateInterval.Returns(x => null);

            var viewModel = BuildViewModel();

            Assert.AreEqual(viewModel.CurrentUpdateInterval, _currentSettingsProvider.Settings.ApplicationSettings.UpdateInterval);
        }

        [Test]
        public void GpoSettingsIsNull_GetCurrentUpdateInterval_GetUpdateFromApplicationSettings()
        {
            _gpoSettings = null;

            var viewModel = BuildViewModel();

            Assert.AreEqual(viewModel.CurrentUpdateInterval, _currentSettingsProvider.Settings.ApplicationSettings.UpdateInterval);
        }

        [Test]
        public void GpoSettingsIsSetup_GetCurrentUpdateInterval_GetUpdateFromApplicationSettings()
        {
            _gpoSettings.UpdateInterval.Returns(UpdateInterval.Monthly.ToString());
            var viewModel = BuildViewModel();

            Assert.AreEqual(viewModel.CurrentUpdateInterval, UpdateIntervalHelper.ParseUpdateInterval(_gpoSettings.UpdateInterval));
        }

        [Test]
        public void SetupPropertyChangedListener_SetCurrentUPdateInterval_ValueSetAndRaisePropertyCalled()
        {
            var viewModel = BuildViewModel();

            var eventStub = Substitute.For<IEventHandler<PropertyChangedEventArgs>>();
            viewModel.PropertyChanged += eventStub.OnEventRaised;
            var CurrentUpdateIntervalListener = new PropertyChangedListenerMock(viewModel, nameof(viewModel.CurrentUpdateInterval));
            var DisplayUpdateWarningListener = new PropertyChangedListenerMock(viewModel, nameof(viewModel.DisplayUpdateWarning));

            Assert.AreNotEqual(_currentSettingsProvider.Settings.ApplicationSettings.UpdateInterval, UpdateInterval.Never);
            viewModel.CurrentUpdateInterval = UpdateInterval.Never;

            Assert.IsTrue(CurrentUpdateIntervalListener.WasCalled, "CurrentUpdateInterval");
            Assert.IsTrue(DisplayUpdateWarningListener.WasCalled, "DisplayUpdateWarning");
            Assert.AreEqual(_currentSettingsProvider.Settings.ApplicationSettings.UpdateInterval, UpdateInterval.Never);
        }

        [Test]
        public void GPOAndUpdateIntervalIsNull_RequestUpdateIsEnabled_IsTrue()
        {
            _gpoSettings.UpdateInterval.Returns(x => null);

            var viewModel = BuildViewModel();
            Assert.IsTrue(viewModel.UpdateIsEnabled);
        }

        [Test]
        public void GPOIsNull_RequestUpdateIsEnabled_IsTrue()
        {
            _gpoSettings = null;
            var viewModel = BuildViewModel();
            Assert.IsTrue(viewModel.UpdateIsEnabled);
        }

        [Test]
        public void GPOAndUpdateIntervalIsSet_RequestUpdateIsEnabled_IsFalse()
        {
            var viewModel = BuildViewModel();
            Assert.IsFalse(viewModel.UpdateIsEnabled);
        }

        [Test]
        public void VisitWebsiteExecute_ProcessStarterGetsCalled()
        {
            var wasCalled = false;
            var viewModel = BuildViewModel();
            _processStarter.When(x => x.Start(viewModel.PdfforgeWebsiteUrl)).Do(x => wasCalled = true);

            viewModel.VisitWebsiteCommand.Execute(null);
            Assert.IsTrue(wasCalled);
        }

        [Test]
        public void VisitWebsiteExecute_IgnoreErrorThrown()
        {
            var viewModel = BuildViewModel();
            _processStarter.When(x => x.Start(viewModel.PdfforgeWebsiteUrl)).Do(x =>
            {
                throw new Exception("ANY KIND OF EXPECTION");
            });

            Assert.DoesNotThrow(() => viewModel.VisitWebsiteCommand.Execute(null));
        }
    }
}
