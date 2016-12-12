using System.IO;
using System.Text;
using NUnit.Framework;
using pdfforge.DataStorage.Storage;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Core.SettingsManagement;
using PDFCreator.TestUtilities;

namespace pdfforge.IntegrationTest.ViewModels.IntegrationTest
{
    [TestFixture]
    internal class SettingsUpgraderTest
    {
        [SetUp]
        public void Setup()
        {
            var bootstrapper = new IntegrationTestBootstrapper();
            var container = bootstrapper.ConfigureContainer();
            _th = container.GetInstance<TestHelper>();
            _th.InitTempFolder("SettingsTest");
        }

        [TearDown]
        public void CleanUp()
        {
            _th.CleanUp();
        }

        private TestHelper _th;

        [Test]
        public void SettingsUpdateV3To4_VersionBecomes4_TiffColorBlackWihteGetsBlackWhiteG4Fax()
        {
            const int oldSettingsVersion = 3;
            const int newSettingsVersion = 4;

            var ini = new IniStorage(Encoding.UTF8);
            var builder = new DefaultProfileBuilder();
            var oldSettings = builder.CreateDefaultSettings("PDFCreator", ini, "English");

            oldSettings.ConversionProfiles[0].TiffSettings.Color = TiffColor.BlackWhiteG4Fax;
            var currentSettingsVersion = oldSettings.ApplicationProperties.SettingsVersion;
            oldSettings.ApplicationProperties.SettingsVersion = oldSettingsVersion;
            var iniFile = Path.Combine(_th.TmpTestFolder, "TestSettings.ini");

            oldSettings.SaveData(ini, iniFile);

            var settingsFromIni = File.ReadAllText(iniFile);
            settingsFromIni = settingsFromIni.Replace("BlackWhiteG4Fax", "BlackWhite");
            File.WriteAllText(iniFile, settingsFromIni);

            var upgrader = new SettingsUpgradeHelper(newSettingsVersion);
            var settings = new DefaultProfileBuilder().CreateEmptySettings(null);
            settings.LoadData(ini, iniFile, upgrader.UpgradeSettings);

            Assert.AreEqual(newSettingsVersion, settings.ApplicationProperties.SettingsVersion, "Did not update SettingsVersion.");
            Assert.AreEqual(TiffColor.BlackWhiteG4Fax, settings.ConversionProfiles[0].TiffSettings.Color, "Did not update TiffColor BlackWhite to BlackWhiteG4Fax");
        }
    }
}