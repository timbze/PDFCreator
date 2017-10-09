using NUnit.Framework;
using pdfforge.DataStorage;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.SettingsManagement;

namespace pdfforge.PDFCreator.UnitTest.Core.SettingsManagement
{
    [TestFixture]
    internal class SettingsUpgraderBasicTest
    {
        [Test]
        public void NumberOfUpdateMethodEqualsVersionInSettingsHelper()
        {
            var data = Data.CreateDataStorage();
            var upgrader = new SettingsUpgrader(data);

            var settingsVersion = new ApplicationProperties().SettingsVersion;

            Assert.AreEqual(settingsVersion, upgrader.NumberOfUpgradeMethods());
        }

        [Test]
        public void VersionInDefaultPdfCreatorSettingsEqualsVersionInSettingsHelper()
        {
            var data = Data.CreateDataStorage();
            var upgrader = new SettingsUpgrader(data);

            var pdfCreatorSettings = new PdfCreatorSettings(null);

            Assert.AreEqual(pdfCreatorSettings.ApplicationProperties.SettingsVersion, upgrader.NumberOfUpgradeMethods());
        }

        [Test]
        public void EmptyData_GetVersion_Returns0()
        {
            var data = Data.CreateDataStorage();
            var upgrader = new SettingsUpgrader(data);

            Assert.AreEqual(0, upgrader.SettingsVersion);
        }

        [Test]
        public void DataWithNonIntVersion_GetVersion_Returns0()
        {
            var data = Data.CreateDataStorage();
            data.SetValue(SettingsUpgrader.VersionSettingPath, "xyz");

            var upgrader = new SettingsUpgrader(data);

            Assert.AreEqual(0, upgrader.SettingsVersion);
        }

        [Test]
        public void DataWithVersion0_GetVersion_Returns0()
        {
            var data = Data.CreateDataStorage();
            data.SetValue(SettingsUpgrader.VersionSettingPath, "0");

            var upgrader = new SettingsUpgrader(data);

            Assert.AreEqual(0, upgrader.SettingsVersion);
        }

        [Test]
        public void DataWithVersion1_GetVersion_Returns1()
        {
            var data = Data.CreateDataStorage();
            data.SetValue(SettingsUpgrader.VersionSettingPath, "1");

            var upgrader = new SettingsUpgrader(data);

            Assert.AreEqual(1, upgrader.SettingsVersion);
        }

        [Test]
        public void DataWithVersion0_UpgradeRequiredToVersion0_ReturnsFalse()
        {
            var data = Data.CreateDataStorage();
            data.SetValue(SettingsUpgrader.VersionSettingPath, "0");

            var upgrader = new SettingsUpgrader(data);

            Assert.IsFalse(upgrader.RequiresUpgrade(0));
        }

        [Test]
        public void DataWithVersion1_UpgradeRequiredToVersion0_ReturnsFalse()
        {
            var data = Data.CreateDataStorage();
            data.SetValue(SettingsUpgrader.VersionSettingPath, "1");

            var upgrader = new SettingsUpgrader(data);

            Assert.IsFalse(upgrader.RequiresUpgrade(0));
        }

        [Test]
        public void DataWithVersion0_UpgradeRequiredToVersion1_ReturnsTrue()
        {
            var data = Data.CreateDataStorage();
            data.SetValue(SettingsUpgrader.VersionSettingPath, "0");

            var upgrader = new SettingsUpgrader(data);

            Assert.IsTrue(upgrader.RequiresUpgrade(1));
        }
    }
}
