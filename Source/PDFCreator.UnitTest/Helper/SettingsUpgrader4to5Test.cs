using NUnit.Framework;
using pdfforge.DataStorage;
using pdfforge.PDFCreator.Helper;

namespace PDFCreator.UnitTest.Helper
{
    [TestFixture]
    class SettingsUpgrader4To5Test
    {
        private Data _dataV3;
        private int replacementCount;

        [SetUp]
        public void SetUp()
        {
            _dataV3 = Data.CreateDataStorage();
            replacementCount = 0;

            _dataV3.SetValue(SettingsUpgrader.VersionSettingPath, "4");
        }

        private void AddOldReplacement(string search, string replace)
        {
            string section = $"ApplicationSettings\\TitleReplacement\\{replacementCount}\\";
            _dataV3.SetValue($"{section}Search", search);
            _dataV3.SetValue($"{section}Replace", replace);
            replacementCount++;
            _dataV3.SetValue(@"ApplicationSettings\TitleReplacement\numClasses", replacementCount.ToString());
        }

        [Test]
        public void DataWithVersion4_UpgradeRequiredToVersion5_ReturnsTrue()
        {
            var upgrader = new SettingsUpgrader(_dataV3);

            Assert.IsTrue(upgrader.RequiresUpgrade(5));
        }

        [Test]
        public void DataWithVersion5_UpgradeRequiredToVersion5_ReturnsFalse()
        {
            _dataV3.SetValue(SettingsUpgrader.VersionSettingPath, "5");
            var upgrader = new SettingsUpgrader(_dataV3);

            Assert.IsFalse(upgrader.RequiresUpgrade(5));
        }

        [Test]
        public void AfterUpgrade_SettingsVersionIs5()
        {
            var upgrader = new SettingsUpgrader(_dataV3);

            upgrader.Upgrade(5);

            Assert.AreEqual("5", upgrader.Data.GetValue(SettingsUpgrader.VersionSettingPath));
        }

        [TestCase("Microsoft Word - ")]
        [TestCase("Microsoft PowerPoint - ")]
        [TestCase("Microsoft Excel - ")]
        public void OldStartReplacements_AreConvertedToStartType(string expectedStartType)
        {
            AddOldReplacement(expectedStartType, "");
            var upgrader = new SettingsUpgrader(_dataV3.Clone());

            upgrader.Upgrade(5);
            var data = upgrader.Data;

            Assert.AreEqual("Start", data.GetValue("ApplicationSettings\\TitleReplacement\\0\\ReplacementType"));
        }

        [TestCase(".doc")]
        [TestCase(".docx")]
        [TestCase(".xls")]
        [TestCase(".xlsx")]
        [TestCase(".ppt")]
        [TestCase(".pptx")]
        [TestCase(".png")]
        [TestCase(".jpg")]
        [TestCase(".jpeg")]
        [TestCase(".txt - Editor")]
        [TestCase(" - Editor")]
        [TestCase(".txt")]
        [TestCase(".tif")]
        [TestCase(".tiff")]
        public void OldEndReplacements_AreConvertedToEndType(string expectedEndType)
        {
            AddOldReplacement(expectedEndType, "");
            var upgrader = new SettingsUpgrader(_dataV3.Clone());

            upgrader.Upgrade(5);
            var data = upgrader.Data;

            Assert.AreEqual("End", data.GetValue("ApplicationSettings\\TitleReplacement\\0\\ReplacementType"));
        }
    }
}
