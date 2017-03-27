using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using NSubstitute;
using NUnit.Framework;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.UI.ViewModels.Assistants;
using pdfforge.PDFCreator.UI.ViewModels.Assistants.Update;
using pdfforge.PDFCreator.UI.ViewModels.UserControlViewModels.ApplicationSettings;
using pdfforge.PDFCreator.UI.ViewModels.UserControlViewModels.ApplicationSettings.Translations;
using pdfforge.PDFCreator.UnitTest.UI.ViewModels.WindowViewModels;
using pdfforge.PDFCreator.UnitTest.UnitTestHelper;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.Process;
using Rhino.Mocks;

namespace pdfforge.PDFCreator.UnitTest.UI.ViewModels.UserControlViewModels.ApplicationSettings
{
    [TestFixture]
    internal class GeneralTabViewModelTest
    {
        private GeneralTabViewModel BuildGeneralTabViewModel()
        {
            var updateAssistant = Substitute.For<IUpdateAssistant>();
            updateAssistant.UpdatesEnabled.Returns(true);

            var generalTabViewModel = new GeneralTabViewModel(Substitute.For<ILanguageProvider>(), null, updateAssistant, Substitute.For<IUacAssistant>(), Substitute.For<IInteractionInvoker>(), Substitute.For<IOsHelper>(), Substitute.For<IProcessStarter>(), new GeneralTabTranslation());
            generalTabViewModel.SetSettingsAndRaiseNotifications(new PdfCreatorSettings(null), null);

            return generalTabViewModel;
        }

        private GeneralTabViewModel BuildGeneralTabViewModelWithoutUpdate()
        {
            var updateAssistant = Substitute.For<IUpdateAssistant>();
            updateAssistant.UpdatesEnabled.Returns(false);

            var generalTabViewModel = new GeneralTabViewModel(Substitute.For<ILanguageProvider>(), null, updateAssistant, Substitute.For<IUacAssistant>(), Substitute.For<IInteractionInvoker>(), Substitute.For<IOsHelper>(), Substitute.For<IProcessStarter>(), new GeneralTabTranslation());
            generalTabViewModel.SetSettingsAndRaiseNotifications(new PdfCreatorSettings(null), null);

            return generalTabViewModel;
        }

        [Test]
        public void EmptyViewModel_SettingApplicationProperties_RaisesPropertyChanged()
        {
            var eventStub = MockRepository.GenerateStub<IEventHandler<PropertyChangedEventArgs>>();
            var generalTabViewModel = BuildGeneralTabViewModel();
            generalTabViewModel.PropertyChanged += eventStub.OnEventRaised;
            var propertyListener = new PropertyChangedListenerMock(generalTabViewModel, "ApplicationProperties");

            generalTabViewModel.SetSettingsAndRaiseNotifications(new PdfCreatorSettings(null), null);
            Assert.IsTrue(propertyListener.WasCalled);
        }

        [Test]
        public void EmptyViewModel_SettingGpoSettings_RaisesPropertyChanged()
        {
            var eventStub = MockRepository.GenerateStub<IEventHandler<PropertyChangedEventArgs>>();
            var generalTabViewModel = BuildGeneralTabViewModel();
            generalTabViewModel.PropertyChanged += eventStub.OnEventRaised;
            var languageIsEnabledPropertyListener = new PropertyChangedListenerMock(generalTabViewModel, "LanguageIsEnabled");
            var currentLanguagePropertyListener = new PropertyChangedListenerMock(generalTabViewModel, "CurrentLanguage");
            var updateIsEnabledPropertyListener = new PropertyChangedListenerMock(generalTabViewModel, "UpdateIsEnabled");
            var currentUpdateIntervalPropertyListener = new PropertyChangedListenerMock(generalTabViewModel, "CurrentUpdateInterval");

            generalTabViewModel.SetSettingsAndRaiseNotifications(new PdfCreatorSettings(null), new GpoSettingsDefaults());

            Assert.IsTrue(languageIsEnabledPropertyListener.WasCalled);
            Assert.IsTrue(currentLanguagePropertyListener.WasCalled);
            Assert.IsTrue(updateIsEnabledPropertyListener.WasCalled);
            Assert.IsTrue(currentUpdateIntervalPropertyListener.WasCalled);
        }

        [Test]
        public void EmptyViewModel_SettingLanguages_RaisesPropertyChanged()
        {
            var eventStub = MockRepository.GenerateStub<IEventHandler<PropertyChangedEventArgs>>();
            var generalTabViewModel = BuildGeneralTabViewModel();
            generalTabViewModel.PropertyChanged += eventStub.OnEventRaised;
            var propertyListener = new PropertyChangedListenerMock(generalTabViewModel, "Languages");

            generalTabViewModel.Languages = new List<Language>();
            Assert.IsTrue(propertyListener.WasCalled);
        }

        [Test]
        public void GpoProperties_CheckInitializations()
        {
            var generalTabViewModel = BuildGeneralTabViewModel();

            generalTabViewModel.SetSettingsAndRaiseNotifications(new PdfCreatorSettings(null), null);

            Assert.IsTrue(generalTabViewModel.LanguageIsEnabled);
            Assert.IsTrue(generalTabViewModel.UpdateIsEnabled);
        }

        [Test]
        public void GpoProperties_CurrentLanguage_ValueGetsStoredInApplicationSettingsLanguage()
        {
            var generalTabViewModel = BuildGeneralTabViewModel();
            generalTabViewModel.SetSettingsAndRaiseNotifications(new PdfCreatorSettings(null), null);

            generalTabViewModel.CurrentLanguage = "test language";

            Assert.AreEqual("test language", generalTabViewModel.ApplicationSettings.Language);
        }

        [Test]
        public void GpoProperties_CurrentUpdateIntervall_ValueGetsStoredInApplicationSettingsUpdateInterval()
        {
            var generalTabViewModel = BuildGeneralTabViewModel();
            generalTabViewModel.ApplicationSettings.UpdateInterval = UpdateInterval.Daily;
            generalTabViewModel.CurrentUpdateInterval = UpdateInterval.Never;

            Assert.AreEqual(UpdateInterval.Never, generalTabViewModel.ApplicationSettings.UpdateInterval);
        }

        [Test]
        public void GpoProperties_GpoLanguageIsNotNull_CurrentLanguageIsGpoLanguage()
        {
            var generalTabViewModel = BuildGeneralTabViewModel();
            var gpoSettings = new TestGpoSettings
            {
                Language = "gpo language"
            };

            generalTabViewModel.ApplicationSettings.Language = "application settings language";
            generalTabViewModel.SetSettingsAndRaiseNotifications(new PdfCreatorSettings(null), gpoSettings);

            Assert.AreEqual(gpoSettings.Language, generalTabViewModel.CurrentLanguage);
        }

        [Test]
        public void GpoProperties_GpoLanguageIsNotNull_LanguageIsEnabledIsFalse()
        {
            var generalTabViewModel = BuildGeneralTabViewModel();
            var gpoSettings = new TestGpoSettings
            {
                Language = "Some Language"
            };

            generalTabViewModel.SetSettingsAndRaiseNotifications(new PdfCreatorSettings(null), gpoSettings);

            Assert.IsFalse(generalTabViewModel.LanguageIsEnabled);
        }

        [Test]
        public void GpoProperties_GpoLanguageIsNull_CurrentLanguageIsApplicationSettingsLanguage()
        {
            var generalTabViewModel = BuildGeneralTabViewModel();
            var gpoSettings = new TestGpoSettings
            {
                Language = null
            };
            generalTabViewModel.ApplicationSettings.Language = "application settings language";

            generalTabViewModel.SetSettingsAndRaiseNotifications(new PdfCreatorSettings(null), gpoSettings);

            Assert.AreEqual(generalTabViewModel.ApplicationSettings.Language, generalTabViewModel.CurrentLanguage);
        }

        [Test]
        public void GpoProperties_GpoLanguageIsNull_LanguageIsEnabledIsTrue()
        {
            var generalTabViewModel = BuildGeneralTabViewModel();
            var gpoSettings = new TestGpoSettings
            {
                Language = null
            };

            generalTabViewModel.SetSettingsAndRaiseNotifications(new PdfCreatorSettings(null), gpoSettings);

            Assert.IsTrue(generalTabViewModel.LanguageIsEnabled);
        }

        [Test]
        public void GpoProperties_GpoUpdateIntervalIsNotNull_CurrentUpdateIsGpoUpdateInterval()
        {
            var generalTabViewModel = BuildGeneralTabViewModel();
            var gpoSettings = new TestGpoSettings
            {
                UpdateInterval = "Daily"
            };
            generalTabViewModel.ApplicationSettings.UpdateInterval = UpdateInterval.Weekly;

            generalTabViewModel.SetSettingsAndRaiseNotifications(new PdfCreatorSettings(null), gpoSettings);

            Assert.AreEqual(UpdateInterval.Daily, generalTabViewModel.CurrentUpdateInterval);
        }

        [Test]
        public void GpoProperties_GpoUpdateIntervalIsNull_CurrentUpdateIntervalIsApplicationSettingsUpdateInterval()
        {
            var generalTabViewModel = BuildGeneralTabViewModel();
            var gpoSettings = new TestGpoSettings
            {
                UpdateInterval = null
            };
            generalTabViewModel.ApplicationSettings.UpdateInterval = UpdateInterval.Weekly;

            generalTabViewModel.SetSettingsAndRaiseNotifications(new PdfCreatorSettings(null), gpoSettings);

            Assert.AreEqual(generalTabViewModel.ApplicationSettings.UpdateInterval, generalTabViewModel.CurrentUpdateInterval);
        }

        [Test]
        public void GpoProperties_GpoUpdateIntervallIsNotNull_UpdateIsEnabledIsFalse()
        {
            var generalTabViewModel = BuildGeneralTabViewModel();
            var gpoSettings = new TestGpoSettings
            {
                UpdateInterval = "Some Interval"
            };

            generalTabViewModel.SetSettingsAndRaiseNotifications(new PdfCreatorSettings(null), gpoSettings);

            Assert.IsFalse(generalTabViewModel.UpdateIsEnabled);
        }

        [Test]
        public void GpoProperties_GpoUpdateIntervallIsNull_UpdateIsEnabledIsTrue()
        {
            var generalTabViewModel = BuildGeneralTabViewModel();
            var gpoSettings = new TestGpoSettings
            {
                UpdateInterval = null
            };

            generalTabViewModel.SetSettingsAndRaiseNotifications(new PdfCreatorSettings(null), gpoSettings);

            Assert.IsTrue(generalTabViewModel.UpdateIsEnabled);
        }

        [Test]
        public void UpdateCheckControlVisibility_EditionHideAndDisableUpdatesIsDisabled_ReturnsVisibile()
        {
            var generalTabViewModel = BuildGeneralTabViewModel();

            Assert.AreEqual(Visibility.Visible, generalTabViewModel.UpdateCheckControlVisibility);
        }

        [Test]
        public void UpdateCheckControlVisibility_EditionHideAndDisableUpdatesIsEnabled_ReturnsCollapsed()
        {
            var generalTabViewModel = BuildGeneralTabViewModelWithoutUpdate();

            Assert.AreEqual(Visibility.Collapsed, generalTabViewModel.UpdateCheckControlVisibility);
        }

        [Test]
        public void UpdateCheckControlVisibility_EditionIsNull_ReturnsVisibile()
        {
            var generalTabViewModel = BuildGeneralTabViewModel();

            Assert.AreEqual(Visibility.Visible, generalTabViewModel.UpdateCheckControlVisibility);
        }
    }
}