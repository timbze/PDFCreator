using System;
using SystemInterface.Microsoft.Win32;
using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.Utilities;
using Rhino.Mocks;

namespace pdfforge.PDFCreator.UnitTest.Core.SettingsManagement
{
    [TestFixture]
    public class WelcomeVersionTest
    {
        [SetUp]
        public void Setup()
        {
            var assemblyHelper = Substitute.For<IAssemblyHelper>();
            assemblyHelper.GetPdfforgeAssemblyVersion().Returns(new Version(1, 2, 3, 4));
            _versionHelper = new VersionHelper(assemblyHelper);
        }

        private VersionHelper _versionHelper;

        [Test]
        public void FirstRunTest_NoWelcomeVersionSet_IsFirstRun()
        {
            var registryMock = MockRepository.GenerateMock<IRegistry>();
            registryMock.Stub(x => x.GetValue(WelcomeSettingsHelper.RegistryKeyForWelcomeSettings, WelcomeSettingsHelper.RegistryValueForWelcomeVersion, null)).Return("");

            var welcomeSettingsHelper = new WelcomeSettingsHelper(registryMock, _versionHelper);

            Assert.IsTrue(welcomeSettingsHelper.IsFirstRun(), "Empty registry value for WelcomeVersion not detected as FirstRun.");

            registryMock.AssertWasCalled(
                x => x.GetValue(WelcomeSettingsHelper.RegistryKeyForWelcomeSettings,
                    WelcomeSettingsHelper.RegistryValueForWelcomeVersion, null), options => options.Repeat.Once());
        }

        [Test]
        public void FirstRunTest_WelcomeVersionIsBiggerThanCurrentVersion_IsFirstRun()
        {
            var registryMock = MockRepository.GenerateMock<IRegistry>();
            registryMock.Stub(x => x.GetValue(WelcomeSettingsHelper.RegistryKeyForWelcomeSettings, WelcomeSettingsHelper.RegistryValueForWelcomeVersion, null)).Return("999999999.9.9");

            var welcomeSettingsHelper = new WelcomeSettingsHelper(registryMock, _versionHelper);

            Assert.IsTrue(welcomeSettingsHelper.IsFirstRun(), "Bigger WelcomeVersion not detected as FirstRun.");

            registryMock.AssertWasCalled(
                x => x.GetValue(WelcomeSettingsHelper.RegistryKeyForWelcomeSettings,
                    WelcomeSettingsHelper.RegistryValueForWelcomeVersion, null), options => options.Repeat.Once());
        }

        [Test]
        public void FirstRunTest_WelcomeVersionIsSmallerThanCurrentVersion_IsFirstRun()
        {
            var registryMock = MockRepository.GenerateMock<IRegistry>();
            registryMock.Stub(x => x.GetValue(WelcomeSettingsHelper.RegistryKeyForWelcomeSettings, WelcomeSettingsHelper.RegistryValueForWelcomeVersion, null)).Return("0.0.0");

            var welcomeSettingsHelper = new WelcomeSettingsHelper(registryMock, _versionHelper);

            Assert.IsTrue(welcomeSettingsHelper.IsFirstRun(), "Smaller WelcomeVersion in registry not detected as FirstRun.");

            registryMock.AssertWasCalled(
                x => x.GetValue(WelcomeSettingsHelper.RegistryKeyForWelcomeSettings,
                    WelcomeSettingsHelper.RegistryValueForWelcomeVersion, null), options => options.Repeat.Once());
        }

        [Test]
        public void FirstRunTest_WelcomeVersionIsTheCurrentVersion_IsNotFirstRun()
        {
            var registryMock = MockRepository.GenerateMock<IRegistry>();

            var welcomeSettingsHelper = new WelcomeSettingsHelper(registryMock, _versionHelper);
            var currentVersion = _versionHelper.FormatWithBuildNumber();

            registryMock.Stub(x => x.GetValue(WelcomeSettingsHelper.RegistryKeyForWelcomeSettings, WelcomeSettingsHelper.RegistryValueForWelcomeVersion, null)).Return(currentVersion);

            Assert.IsFalse(welcomeSettingsHelper.IsFirstRun(), "Current Version as WelcomeVersion detected as FirstRun.");

            registryMock.AssertWasCalled(
                x => x.GetValue(WelcomeSettingsHelper.RegistryKeyForWelcomeSettings,
                    WelcomeSettingsHelper.RegistryValueForWelcomeVersion, null), options => options.Repeat.Once());
        }

        [Test]
        public void SetCurrentVersionAsWelcomeVersionTest_RegistrySetValueGetsCalled()
        {
            var registryMock = MockRepository.GenerateMock<IRegistry>();

            var welcomeSettingsHelper = new WelcomeSettingsHelper(registryMock, _versionHelper);
            var currentVersion = _versionHelper.FormatWithBuildNumber();

            welcomeSettingsHelper.SetCurrentApplicationVersionAsWelcomeVersionInRegistry();

            registryMock.AssertWasCalled(
                x => x.SetValue(WelcomeSettingsHelper.RegistryKeyForWelcomeSettings,
                    WelcomeSettingsHelper.RegistryValueForWelcomeVersion, currentVersion),
                options => options.Repeat.Once());
        }
    }
}