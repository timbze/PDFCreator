using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using NUnit.Framework;
using pdfforge.DynamicTranslator;
using pdfforge.GpoReader;
using pdfforge.PDFCreator.Core.Settings;
using pdfforge.PDFCreator.Core.Settings.Enums;
using pdfforge.PDFCreator.Shared.Licensing;
using pdfforge.PDFCreator.ViewModels.UserControls;
using PDFCreator.UnitTest.ViewModels.Helper;
using Rhino.Mocks;

namespace PDFCreator.UnitTest.ViewModels.UserControls
{
    [TestFixture]
    class GeneralTabViewModelTest
    {
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
        public void EmptyViewModel_SettingApplicationProperties_RaisesPropertyChanged()
        {
            var eventStub = MockRepository.GenerateStub<IEventHandler<PropertyChangedEventArgs>>();
            var generalTabViewModel = BuildGeneralTabViewModel();
            generalTabViewModel.PropertyChanged += eventStub.OnEventRaised;
            var propertyListener = new PropertyChangedListenerMock(generalTabViewModel, "ApplicationProperties");

            generalTabViewModel.ApplicationProperties = new ApplicationProperties();
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

            generalTabViewModel.GpoSettings = new GpoSettings();

            Assert.IsTrue(languageIsEnabledPropertyListener.WasCalled);
            Assert.IsTrue(currentLanguagePropertyListener.WasCalled);
            Assert.IsTrue(updateIsEnabledPropertyListener.WasCalled);
            Assert.IsTrue(currentUpdateIntervalPropertyListener.WasCalled);
        }

        [Test]
        public void GpoProperties_CheckInitializations()
        {
            var generalTabViewModel = BuildGeneralTabViewModel();
            generalTabViewModel.GpoSettings = null;

            Assert.IsTrue(generalTabViewModel.LanguageIsEnabled);
            Assert.IsTrue(generalTabViewModel.UpdateIsEnabled);
        }

        [Test]
        public void GpoProperties_GpoLanguageIsNull_LanguageIsEnabledIsTrue()
        {
            var generalTabViewModel = BuildGeneralTabViewModel();
            var gpoSettings = new GpoSettings();
            gpoSettings.Language = null;

            generalTabViewModel.GpoSettings = gpoSettings;

            Assert.IsTrue(generalTabViewModel.LanguageIsEnabled);
        }

        [Test]
        public void GpoProperties_GpoLanguageIsNotNull_LanguageIsEnabledIsFalse()
        {
            var generalTabViewModel = BuildGeneralTabViewModel();
            var gpoSettings = new GpoSettings();
            gpoSettings.Language = "Some Language";

            generalTabViewModel.GpoSettings = gpoSettings;

            Assert.IsFalse(generalTabViewModel.LanguageIsEnabled);
        }

        [Test]
        public void GpoProperties_GpoLanguageIsNull_CurrentLanguageIsApplicationSettingsLanguage()
        {
            var generalTabViewModel = BuildGeneralTabViewModel();
            var gpoSettings = new GpoSettings();
            gpoSettings.Language = null;
            generalTabViewModel.ApplicationSettings.Language = "application settings language";

            generalTabViewModel.GpoSettings = gpoSettings;

            Assert.AreEqual(generalTabViewModel.ApplicationSettings.Language, generalTabViewModel.CurrentLanguage);
        }

        [Test]
        public void GpoProperties_GpoLanguageIsNotNull_CurrentLanguageIsGpoLanguage()
        {
            var generalTabViewModel = BuildGeneralTabViewModel();
            var gpoSettings = new GpoSettings();
            gpoSettings.Language = "gpo language";

            generalTabViewModel.ApplicationSettings.Language = "application settings language";
            generalTabViewModel.GpoSettings = gpoSettings;

            Assert.AreEqual(gpoSettings.Language, generalTabViewModel.CurrentLanguage);
        }

        [Test]
        public void GpoProperties_CurrentLanguage_ValueGetsStoredInApplicationSettingsLanguage()
        {
            var generalTabViewModel = BuildGeneralTabViewModel();
            generalTabViewModel.CurrentLanguage = "test language";

            Assert.AreEqual("test language", generalTabViewModel.ApplicationSettings.Language);
        }

        [Test]
        public void GpoProperties_GpoUpdateIntervallIsNull_UpdateIsEnabledIsTrue()
        {
            var generalTabViewModel = BuildGeneralTabViewModel();
            var gpoSettings = new GpoSettings();
            gpoSettings.UpdateInterval = null;

            generalTabViewModel.GpoSettings = gpoSettings;

            Assert.IsTrue(generalTabViewModel.UpdateIsEnabled);
        }

        [Test]
        public void GpoProperties_GpoUpdateIntervallIsNotNull_UpdateIsEnabledIsFalse()
        {
            var generalTabViewModel = BuildGeneralTabViewModel();
            var gpoSettings = new GpoSettings();
            gpoSettings.UpdateInterval = "Some Interval";

            generalTabViewModel.GpoSettings = gpoSettings;

            Assert.IsFalse(generalTabViewModel.UpdateIsEnabled);
        }

        [Test]
        public void GpoProperties_GpoUpdateIntervalIsNull_CurrentUpdateIntervalIsApplicationSettingsUpdateInterval()
        {
            var generalTabViewModel = BuildGeneralTabViewModel();
            var gpoSettings = new GpoSettings();
            gpoSettings.UpdateInterval = null;
            generalTabViewModel.ApplicationSettings.UpdateInterval = UpdateInterval.Weekly;

            generalTabViewModel.GpoSettings = gpoSettings;

            Assert.AreEqual(generalTabViewModel.ApplicationSettings.UpdateInterval, generalTabViewModel.CurrentUpdateInterval);
        }

        [Test]
        public void GpoProperties_GpoUpdateIntervalIsNotNull_CurrentUpdateIsGpoUpdateInterval()
        {
            var generalTabViewModel = BuildGeneralTabViewModel();
            var gpoSettings = new GpoSettings();
            gpoSettings.UpdateInterval = "Daily";
            generalTabViewModel.ApplicationSettings.UpdateInterval = UpdateInterval.Weekly;

            generalTabViewModel.GpoSettings = gpoSettings;

            Assert.AreEqual(UpdateInterval.Daily, generalTabViewModel.CurrentUpdateInterval);
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
        public void UpdateCheckControlVisibility_EditionIsNull_ReturnsVisibile()
        {
            var generalTabViewModel = new GeneralTabViewModel(null);

            Assert.AreEqual(Visibility.Visible, generalTabViewModel.UpdateCheckControlVisibility);
        }

        [Test]
        public void UpdateCheckControlVisibility_EditionHideAndDisableUpdatesIsDisabled_ReturnsVisibile()
        {
            var edition = new Edition();
            edition.HideAndDisableUpdates = false;
            var generalTabViewModel = new GeneralTabViewModel(edition);

            Assert.AreEqual(Visibility.Visible, generalTabViewModel.UpdateCheckControlVisibility);
        }

        [Test]
        public void UpdateCheckControlVisibility_EditionHideAndDisableUpdatesIsEnabled_ReturnsCollapsed()
        {
            var edition = new Edition();
            edition.HideAndDisableUpdates = true;
            var generalTabViewModel = new GeneralTabViewModel(edition);

            Assert.AreEqual(Visibility.Collapsed, generalTabViewModel.UpdateCheckControlVisibility);
        }

        private GeneralTabViewModel BuildGeneralTabViewModel()
        {
            var generalTabViewModel = new GeneralTabViewModel(null);
            generalTabViewModel.ApplicationSettings = new ApplicationSettings();

            return generalTabViewModel;
        }
    }
}
