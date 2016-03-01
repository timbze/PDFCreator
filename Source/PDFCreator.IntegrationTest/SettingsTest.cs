using System.ComponentModel;
using System.IO;
using System.Text;
using NUnit.Framework;
using pdfforge.DataStorage.Storage;
using pdfforge.PDFCreator.Core.Settings.Enums;
using pdfforge.PDFCreator.Helper;
using PDFCreator.TestUtilities;

namespace PDFCreator.IntegrationTest
{
    [TestFixture]
    class SettingsTest
    {
        private TestHelper _th;

        [SetUp]
        public void Setup()
        {
            _th = new TestHelper("SettingsTest");
        }

        [TearDown]
        public void CleanUp()
        {
            _th.CleanUp();
        }
        
        [Test]
        public void SettingsUpdateV3To4_VersionBecomes4_TiffColorBlackWihteGetsBlackWhiteG4Fax()
        {
            SettingsHelper.Settings.ConversionProfiles[0].TiffSettings.Color = TiffColor.BlackWhiteG4Fax;
            var currentSettingsVersion = SettingsHelper.Settings.ApplicationProperties.SettingsVersion;
            SettingsHelper.Settings.ApplicationProperties.SettingsVersion = 3;
            var iniFile = Path.Combine(_th.TmpTestFolder, "TestSettings.ini");

            var ini = new IniStorage(Encoding.UTF8);

            SettingsHelper.Settings.SaveData(ini, iniFile);

            var settingsFromIni = File.ReadAllText(iniFile);
            settingsFromIni = settingsFromIni.Replace("BlackWhiteG4Fax", "BlackWhite");
            File.WriteAllText(iniFile, settingsFromIni);

            var settings = SettingsHelper.CreateEmptySettings();
            settings.LoadData(ini, iniFile, SettingsHelper.UpgradeSettings);

            Assert.AreEqual(currentSettingsVersion, settings.ApplicationProperties.SettingsVersion, "Did not update SettingsVersion.");
            Assert.AreEqual(TiffColor.BlackWhiteG4Fax, settings.ConversionProfiles[0].TiffSettings.Color, "Did not update TiffColor BlackWhite to BlackWhiteG4Fax");
        }
    }
}
