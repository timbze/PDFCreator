using NUnit.Framework;
using pdfforge.DataStorage;
using pdfforge.PDFCreator.Core.Settings;
using pdfforge.PDFCreator.Helper;

namespace PDFCreator.UnitTest.Helper
{
    [TestFixture]
    class SettingsUpgraderTest
    {
        [Test]
        public void NumberOfUpdateMethodEqualsVersionInSettingsHelper()
        {
            Data data = Data.CreateDataStorage();
            var upgrader = new SettingsUpgrader(data);

            Assert.AreEqual(SettingsHelper.SETTINGS_VERSION, upgrader.NumberOfUpgradeMethods());
        }

        [Test]
        public void VersionInDefaultPDFCreatorSettingsEqualsVersionInSettingsHelper()
        {
            Data data = Data.CreateDataStorage();
            var upgrader = new SettingsUpgrader(data);

            var pdfCreatorSettings = new PdfCreatorSettings(null);

            Assert.AreEqual(pdfCreatorSettings.ApplicationProperties.SettingsVersion, upgrader.NumberOfUpgradeMethods());
        }
        
        [Test]
        public void EmptyData_GetVersion_Returns0()
        {
            Data data = Data.CreateDataStorage();
            var upgrader = new SettingsUpgrader(data);

            Assert.AreEqual(0, upgrader.SettingsVersion);
        }

        [Test]
        public void DataWithVersion0_GetVersion_Returns0()
        {
            Data data = Data.CreateDataStorage();
            data.SetValue(SettingsUpgrader.VersionSettingPath, "0");

            var upgrader = new SettingsUpgrader(data);

            Assert.AreEqual(0, upgrader.SettingsVersion);
        }

        [Test]
        public void DataWithNonIntVersion_GetVersion_Returns0()
        {
            Data data = Data.CreateDataStorage();
            data.SetValue(SettingsUpgrader.VersionSettingPath, "xyz");

            var upgrader = new SettingsUpgrader(data);

            Assert.AreEqual(0, upgrader.SettingsVersion);
        }

        [Test]
        public void DataWithVersion1_GetVersion_Returns1()
        {
            Data data = Data.CreateDataStorage();
            data.SetValue(SettingsUpgrader.VersionSettingPath, "1");

            var upgrader = new SettingsUpgrader(data);

            Assert.AreEqual(1, upgrader.SettingsVersion);
        }

        [Test]
        public void DataWithVersion2_GetVersion_Returns2()
        {
            Data data = Data.CreateDataStorage();
            data.SetValue(SettingsUpgrader.VersionSettingPath, "2");

            var upgrader = new SettingsUpgrader(data);

            Assert.AreEqual(2, upgrader.SettingsVersion);
        }

        #region Version 0 to 1

        [Test]
        public void DataWithVersion0_UpgradeRequiredToVersion0_ReturnsFalse()
        {
            Data data = Data.CreateDataStorage();
            data.SetValue(SettingsUpgrader.VersionSettingPath, "0");

            var upgrader = new SettingsUpgrader(data);

            Assert.IsFalse(upgrader.RequiresUpgrade(0));
        }

        [Test]
        public void DataWithVersion0_UpgradeRequiredToVersion1_ReturnsTrue()
        {
            Data data = Data.CreateDataStorage();
            data.SetValue(SettingsUpgrader.VersionSettingPath, "0");

            var upgrader = new SettingsUpgrader(data);

            Assert.IsTrue(upgrader.RequiresUpgrade(1));
        }

        [Test]
        public void DataWithVersion1_UpgradeRequiredToVersion1_ReturnsFalse()
        {
            Data data = Data.CreateDataStorage();
            data.SetValue(SettingsUpgrader.VersionSettingPath, "1");

            var upgrader = new SettingsUpgrader(data);

            Assert.IsFalse(upgrader.RequiresUpgrade(1));
        }

        [Test]
        public void DataWithVersion1_UpgradeRequiredToVersion0_ReturnsFalse()
        {
            Data data = Data.CreateDataStorage();
            data.SetValue(SettingsUpgrader.VersionSettingPath, "1");

            var upgrader = new SettingsUpgrader(data);

            Assert.IsFalse(upgrader.RequiresUpgrade(0));
        }

        [Test]
        public void DataWithVersion0_UpgradeToVersion1_SetsVersionTo1()
        {
            Data data = Data.CreateDataStorage();
            data.SetValue(SettingsUpgrader.VersionSettingPath, "0");
            var upgrader = new SettingsUpgrader(data);

            upgrader.Upgrade(1);

            Assert.AreEqual("1", data.GetValue(SettingsUpgrader.VersionSettingPath));
        }

        [Test]
        public void Version0WithDefaultFormatPropertyInProfiles_UpgradeToVersion1_RenamesToOutputFormat()
        {
            Data data = Data.CreateDataStorage();
            data.SetValue(@"ConversionProfiles\numClasses", "2");
            data.SetValue(@"ConversionProfiles\0\DefaultFormat", "Pdf");
            data.SetValue(@"ConversionProfiles\1\DefaultFormat", "Png");
            var upgrader = new SettingsUpgrader(data);

            upgrader.Upgrade(1);

            Assert.AreEqual("Pdf", data.GetValue(@"ConversionProfiles\0\OutputFormat"));
            Assert.AreEqual("Png", data.GetValue(@"ConversionProfiles\1\OutputFormat"));
        }

        [Test]
        public void Version0WithDefaultFormatPropertyInProfiles_UpgradeToVersion1_DefaultFormatSettingIsRemovedAfterRename()
        {
            Data data = Data.CreateDataStorage();
            data.SetValue(@"ConversionProfiles\numClasses", "2");
            data.SetValue(@"ConversionProfiles\0\DefaultFormat", "Pdf");
            data.SetValue(@"ConversionProfiles\1\DefaultFormat", "Png");
            var upgrader = new SettingsUpgrader(data);

            upgrader.Upgrade(1);

            Assert.AreEqual("", data.GetValue(@"ConversionProfiles\0\DefaultFormat"));
        }

        [Test]
        public void Version0_UpgradeToIncrediblyHighVersion_RenamesToOutputFormat()
        {
            Data data = Data.CreateDataStorage();
            data.SetValue(@"ConversionProfiles\numClasses", "2");
            data.SetValue(@"ConversionProfiles\0\DefaultFormat", "Pdf");
            data.SetValue(@"ConversionProfiles\1\DefaultFormat", "Png");
            var upgrader = new SettingsUpgrader(data);

            upgrader.Upgrade(1);

            Assert.AreEqual("Pdf", data.GetValue(@"ConversionProfiles\0\OutputFormat"));
            Assert.AreEqual("Png", data.GetValue(@"ConversionProfiles\1\OutputFormat"));
        }

        [Test]
        public void Version0_UpgradeToIncrediblyHighVersion_RenamesEncryptionLevels()
        {
            Data data = Data.CreateDataStorage();
            data.SetValue(@"ConversionProfiles\numClasses", "3");
            data.SetValue(@"ConversionProfiles\0\PdfSettings\Security\EncryptionLevel", "Low40Bit");
            data.SetValue(@"ConversionProfiles\1\PdfSettings\Security\EncryptionLevel", "Medium128Bit");
            data.SetValue(@"ConversionProfiles\2\PdfSettings\Security\EncryptionLevel", "High128BitAes");
            var upgrader = new SettingsUpgrader(data);

            upgrader.Upgrade(1);

            Assert.AreEqual("Rc40Bit", data.GetValue(@"ConversionProfiles\0\PdfSettings\Security\EncryptionLevel"), "Did not rename Rc40Bit Encryption");
            Assert.AreEqual("Rc128Bit", data.GetValue(@"ConversionProfiles\1\PdfSettings\Security\EncryptionLevel"), "Did not rename Rc128Bit Encryption");
            Assert.AreEqual("Aes128Bit", data.GetValue(@"ConversionProfiles\2\PdfSettings\Security\EncryptionLevel"), "Did not rename Aes128Bit Encryption");
            
        }

        [Test]
        public void Version0_UpgradeVersion1_AddedTitleTemplateWithDefaultValue()
        {
            Data data = Data.CreateDataStorage();
            data.SetValue(@"ConversionProfiles\numClasses", "2");
            var upgrader = new SettingsUpgrader(data);

            upgrader.Upgrade(1);

            Assert.AreEqual("<PrintJobName>", data.GetValue(@"ConversionProfiles\0\TitleTemplate"));
            Assert.AreEqual("<PrintJobName>", data.GetValue(@"ConversionProfiles\1\TitleTemplate"));
        }

        #endregion

        #region Version 1 to 2

        [Test]
        public void DataWithVersion2_UpgradeRequiredToVersion2_ReturnsFalse()
        {
            Data data = Data.CreateDataStorage();
            data.SetValue(SettingsUpgrader.VersionSettingPath, "2");

            var upgrader = new SettingsUpgrader(data);

            Assert.IsFalse(upgrader.RequiresUpgrade(2));
        }

        [Test]
        public void DataWithVersion1_UpgradeRequiredToVersion2_ReturnsTrue()
        {
            Data data = Data.CreateDataStorage();
            data.SetValue(SettingsUpgrader.VersionSettingPath, "1");

            var upgrader = new SettingsUpgrader(data);

            Assert.IsTrue(upgrader.RequiresUpgrade(2));
        }

        [Test]
        public void DataWithVersion1_UpgradeToVersion2_SetsVersionTo2()
        {
            Data data = Data.CreateDataStorage();
            data.SetValue(SettingsUpgrader.VersionSettingPath, "2");
            var upgrader = new SettingsUpgrader(data);

            upgrader.Upgrade(2);

            Assert.AreEqual("2", data.GetValue(SettingsUpgrader.VersionSettingPath));
        }

        [Test]
        public void Version1_UpgradeVersion2_MovedBackgroundOnCover()
        {
            Data data = Data.CreateDataStorage();
            data.SetValue(SettingsUpgrader.VersionSettingPath, "1");
            data.SetValue(@"ConversionProfiles\numClasses", "2");
            data.SetValue(@"ConversionProfiles\0\CoverPage\AddBackground", "true");
            data.SetValue(@"ConversionProfiles\1\CoverPage\AddBackground", "true");

            var upgrader = new SettingsUpgrader(data);

            upgrader.Upgrade(2);

            Assert.AreEqual("true", data.GetValue(@"ConversionProfiles\0\BackgroundPage\OnCover"), "1");
            Assert.AreEqual("true", data.GetValue(@"ConversionProfiles\1\BackgroundPage\OnCover"), "2");
        }

        [Test]
        public void Version1_UpgradeVersion2_MovedBackgroundOnAttachment()
        {
            Data data = Data.CreateDataStorage();
            data.SetValue(SettingsUpgrader.VersionSettingPath, "1");
            data.SetValue(@"ConversionProfiles\numClasses", "2");
            data.SetValue(@"ConversionProfiles\0\AttachmentPage\AddBackground", "true");
            data.SetValue(@"ConversionProfiles\1\AttachmentPage\AddBackground", "true");

            var upgrader = new SettingsUpgrader(data);

            upgrader.Upgrade(2);

            Assert.AreEqual("true", data.GetValue(@"ConversionProfiles\0\BackgroundPage\OnAttachment"));
            Assert.AreEqual("true", data.GetValue(@"ConversionProfiles\1\BackgroundPage\OnAttachment"));
        }

        [Test]
        public void Version1_UpgradeVersion2_LastUsedProfilGuidRenamedToLastUsedProfilEGuid()
        {
            Data data = Data.CreateDataStorage();
            data.SetValue(SettingsUpgrader.VersionSettingPath, "1");
            data.SetValue(@"ApplicationSettings\LastUsedProfilGuid", "SomeTestGuid");

            var upgrader = new SettingsUpgrader(data);

            upgrader.Upgrade(2);

            Assert.AreEqual("SomeTestGuid", data.GetValue(@"ApplicationSettings\LastUsedProfileGuid"));
        }

        #endregion

        #region Version 2 to 3

        [Test]
        public void DataWithVersion3_UpgradeRequiredToVersion3_ReturnsFalse()
        {
            Data data = Data.CreateDataStorage();
            data.SetValue(SettingsUpgrader.VersionSettingPath, "3");

            var upgrader = new SettingsUpgrader(data);

            Assert.IsFalse(upgrader.RequiresUpgrade(3));
        }
        
        [Test]
        public void DataWithVersion2_UpgradeRequiredToVersion3_ReturnsTrue()
        {
            Data data = Data.CreateDataStorage();
            data.SetValue(SettingsUpgrader.VersionSettingPath, "2");

            var upgrader = new SettingsUpgrader(data);

            Assert.IsTrue(upgrader.RequiresUpgrade(3));
        }
        
        [Test]
        public void DataWithVersion2_UpgradeToVersion3_SetsVersionTo3()
        {
            Data data = Data.CreateDataStorage();
            data.SetValue(SettingsUpgrader.VersionSettingPath, "2");
            var upgrader = new SettingsUpgrader(data);

            upgrader.Upgrade(3);

            Assert.AreEqual("3", data.GetValue(SettingsUpgrader.VersionSettingPath));
        }

        [Test]
        public void Version2_UpgradeVersion3_PdfAisNamedPdfA2b()
        {
            Data data = Data.CreateDataStorage();
            data.SetValue(SettingsUpgrader.VersionSettingPath, "2");
            data.SetValue(@"ConversionProfiles\numClasses", "2");
            data.SetValue(@"ConversionProfiles\0\OutputFormat", "PdfA");
            data.SetValue(@"ConversionProfiles\1\OutputFormat", "PdfA");

            var upgrader = new SettingsUpgrader(data);

            upgrader.Upgrade(3);

            Assert.AreEqual("PdfA2B", data.GetValue(@"ConversionProfiles\0\OutputFormat"));
            Assert.AreEqual("PdfA2B", data.GetValue(@"ConversionProfiles\1\OutputFormat"));
        }

        [Test]
        public void Version2_UpgradeVersion3_OutputformatExceptPdfAreUnchanged()
        {
            Data data = Data.CreateDataStorage();
            data.SetValue(SettingsUpgrader.VersionSettingPath, "2");
            data.SetValue(@"ConversionProfiles\numClasses", "5");
            data.SetValue(@"ConversionProfiles\0\OutputFormat", "Pdf");
            data.SetValue(@"ConversionProfiles\1\OutputFormat", "PdfX");
            data.SetValue(@"ConversionProfiles\2\OutputFormat", "Jpeg");
            data.SetValue(@"ConversionProfiles\3\OutputFormat", "Png");
            data.SetValue(@"ConversionProfiles\4\OutputFormat", "Tif");

            var upgrader = new SettingsUpgrader(data);

            upgrader.Upgrade(3);

            Assert.AreEqual("Pdf", data.GetValue(@"ConversionProfiles\0\OutputFormat"));
            Assert.AreEqual("PdfX", data.GetValue(@"ConversionProfiles\1\OutputFormat"));
            Assert.AreEqual("Jpeg", data.GetValue(@"ConversionProfiles\2\OutputFormat"));
            Assert.AreEqual("Png", data.GetValue(@"ConversionProfiles\3\OutputFormat"));
            Assert.AreEqual("Tif", data.GetValue(@"ConversionProfiles\4\OutputFormat"));
        }

        #endregion

        #region Version 3 to 4

        [Test]
        public void DataWithVersion4_UpgradeRequiredToVersion4_ReturnsFalse()
        {
            Data data = Data.CreateDataStorage();
            data.SetValue(SettingsUpgrader.VersionSettingPath, "4");

            var upgrader = new SettingsUpgrader(data);

            Assert.IsFalse(upgrader.RequiresUpgrade(4));
        }

        [Test]
        public void DataWithVersion3_UpgradeRequiredToVersion4_ReturnsTrue()
        {
            Data data = Data.CreateDataStorage();
            data.SetValue(SettingsUpgrader.VersionSettingPath, "3");

            var upgrader = new SettingsUpgrader(data);

            Assert.IsTrue(upgrader.RequiresUpgrade(4));
        }

        [Test]
        public void DataWithVersion3_UpgradeToVersion4_SetsVersionTo4()
        {
            Data data = Data.CreateDataStorage();
            data.SetValue(SettingsUpgrader.VersionSettingPath, "3");
            var upgrader = new SettingsUpgrader(data);

            upgrader.Upgrade(4);

            Assert.AreEqual("4", data.GetValue(SettingsUpgrader.VersionSettingPath));
        }

        [Test]
        public void Version3_UpgradeVersion4_TiffColorBlackWhiteIsNamedBlackWhiteG4Fax()
        {
            Data data = Data.CreateDataStorage();
            data.SetValue(SettingsUpgrader.VersionSettingPath, "3");
            data.SetValue(@"ConversionProfiles\numClasses", "1");
            data.SetValue(@"ConversionProfiles\0\TiffSettings\Color", "BlackWhite");

            var upgrader = new SettingsUpgrader(data);

            upgrader.Upgrade(4);

            Assert.AreEqual("BlackWhiteG4Fax", data.GetValue(@"ConversionProfiles\0\TiffSettings\Color"));
        }

        #endregion

    }
}
