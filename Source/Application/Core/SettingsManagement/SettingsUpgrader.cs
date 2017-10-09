using pdfforge.DataStorage;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace pdfforge.PDFCreator.Core.SettingsManagement
{
    /// <summary>
    ///     The SettingsUpgrader class performs updates to the PDFCreator settings.
    ///     This is done after loading the raw data and before loading them into the DataStorage classes.
    ///     There is one update method from each version to the next (0 to 1, 1 to 2 etc.) and they are called subsequently, if
    ///     required.
    /// </summary>
    public class SettingsUpgrader : DataUpgrader
    {
        public const string VersionSettingPath = @"ApplicationProperties\SettingsVersion";
        private readonly List<Action> _upgradeMethods = new List<Action>();

        public SettingsUpgrader(Data settingsData)
        {
            Data = settingsData;

            _upgradeMethods.Add(UpgradeV0ToV1);
            _upgradeMethods.Add(UpgradeV1ToV2);
            _upgradeMethods.Add(UpgradeV2ToV3);
            _upgradeMethods.Add(UpgradeV3ToV4);
            _upgradeMethods.Add(UpgradeV4ToV5);
            _upgradeMethods.Add(UpgradeV5ToV6);
            _upgradeMethods.Add(UpgradeV6ToV7);
        }

        public int SettingsVersion
        {
            get
            {
                var versionString = Data.GetValue(VersionSettingPath);
                return GetInt(versionString) ?? 0;
            }
        }

        public int NumberOfUpgradeMethods()
        {
            return _upgradeMethods.Count;
        }

        public void Upgrade(int targetVersion)
        {
            for (var i = SettingsVersion; i < Math.Min(targetVersion, _upgradeMethods.Count); i++)
            {
                // Call upgrade methods subsequently, starting with the current version
                var upgradeMethod = _upgradeMethods[i];
                upgradeMethod();
            }
        }

        public bool RequiresUpgrade(int targetVersion)
        {
            return targetVersion > SettingsVersion;
        }

        private void UpgradeV0ToV1()
        {
            MoveSettingInAllProfiles("DefaultFormat", "OutputFormat");
            MapSettingInAllProfiles("PdfSettings\\Security\\" + "EncryptionLevel", MapEncryptionNamesV1);
            ApplyNewSettingInAllProfiles("TitleTemplate", "<PrintJobName>");
            ApplyNewSettingInAllProfiles("AuthorTemplate", "<PrintJobAuthor>");

            Data.SetValue(VersionSettingPath, "1");
        }

        private void UpgradeV1ToV2()
        {
            MoveSettingInAllProfiles(@"CoverPage\AddBackground", @"BackgroundPage\OnCover");
            MoveSettingInAllProfiles(@"AttachmentPage\AddBackground", @"BackgroundPage\OnAttachment");
            MoveValue(@"ApplicationSettings\LastUsedProfilGuid", @"ApplicationSettings\LastUsedProfileGuid");
            Data.SetValue(VersionSettingPath, "2");
        }

        private void UpgradeV2ToV3()
        {
            MapSettingInAllProfiles(@"OutputFormat", MapOutputformatPdfA_V3);
            Data.SetValue(VersionSettingPath, "3");
        }

        private void UpgradeV3ToV4()
        {
            Data.SetValue(VersionSettingPath, "4");
            MapSettingInAllProfiles(@"TiffSettings\Color", MapTiffColorBlackWhite_V4);
        }

        private void UpgradeV4ToV5()
        {
            Data.SetValue(VersionSettingPath, "5");

            string[] startReplacements =
            {
                "Microsoft Word - ",
                "Microsoft PowerPoint - ",
                "Microsoft Excel - "
            };

            string[] endReplacements =
            {
                ".doc",
                ".docx",
                ".xls",
                ".xlsx",
                ".ppt",
                ".pptx",
                ".png",
                ".jpg",
                ".jpeg",
                ".txt - Editor",
                " - Editor",
                ".txt",
                ".tif",
                ".tiff"
            };

            int replacementCount;
            if (int.TryParse(Data.GetValue(@"ApplicationSettings\TitleReplacement\numClasses"), out replacementCount))
            {
                for (var i = 0; i < replacementCount; i++)
                {
                    string section = $"ApplicationSettings\\TitleReplacement\\{i}\\";

                    var type = ReplacementType.Replace;
                    var search = Data.GetValue(section + "Search");

                    if (startReplacements.Contains(search))
                    {
                        type = ReplacementType.Start;
                    }

                    if (endReplacements.Contains(search))
                    {
                        type = ReplacementType.End;
                    }

                    Data.SetValue(section + "ReplacementType", type.ToString());
                }
            }
        }

        private void UpgradeV5ToV6()
        {
            Data.SetValue(VersionSettingPath, "6");

            MoveSettingInAllProfiles(@"EmailClient\AddSignature", @"EmailClientSettings\AddSignature");
            MoveSettingInAllProfiles(@"EmailClient\Content", @"EmailClientSettings\Content");
            MoveSettingInAllProfiles(@"EmailClient\Enabled", @"EmailClientSettings\Enabled");
            MoveSettingInAllProfiles(@"EmailClient\Recipients", @"EmailClientSettings\Recipients");
            MoveSettingInAllProfiles(@"EmailClient\Subject", @"EmailClientSettings\Subject");

            MoveSettingInAllProfiles(@"EmailSmtp\Address", @"EmailSmtpSettings\Address");
            MoveSettingInAllProfiles(@"EmailSmtp\AddSignature", @"EmailSmtpSettings\AddSignature");
            MoveSettingInAllProfiles(@"EmailSmtp\Content", @"EmailSmtpSettings\Content");
            MoveSettingInAllProfiles(@"EmailSmtp\Enabled", @"EmailSmtpSettings\Enabled");
            MoveSettingInAllProfiles(@"EmailSmtp\Password", @"EmailSmtpSettings\Password");
            MoveSettingInAllProfiles(@"EmailSmtp\Port", @"EmailSmtpSettings\Port");
            MoveSettingInAllProfiles(@"EmailSmtp\Recipients", @"EmailSmtpSettings\Recipients");
            MoveSettingInAllProfiles(@"EmailSmtp\SameTextAsClientMail", @"EmailSmtpSettings\SameTextAsClientMail");
            MoveSettingInAllProfiles(@"EmailSmtp\Server", @"EmailSmtpSettings\Server");
            MoveSettingInAllProfiles(@"EmailSmtp\Ssl", @"EmailSmtpSettings\Ssl");
            MoveSettingInAllProfiles(@"EmailSmtp\Subject", @"EmailSmtpSettings\Subject");
            MoveSettingInAllProfiles(@"EmailSmtp\UserName", @"EmailSmtpSettings\UserName");
        }

        private void UpgradeV6ToV7()
        {
            Data.SetValue(VersionSettingPath, "7");

            ApplyV7TargetFolder();
            ApplyV7TitleReplacements();

            V7ExtractTimeServerAccounts();
            V7ExtractFtpAccounts();
            V7ExtractSmtpAccounts();
        }

        private void ApplyV7TargetFolder()
        {
            ForAllProfiles((s, i) =>
            {
                if (GetBool(Data.GetValue(s + @"AutoSave\Enabled")) == true)
                    Data.SetValue(s + "TargetDirectory", Data.GetValue(s + @"AutoSave\TargetDirectory"));
                else
                    Data.SetValue(s + "TargetDirectory", Data.GetValue(s + @"SaveDialog\Folder"));
            });
        }

        private void ApplyV7TitleReplacements()
        {
            int replacementCount;
            if (int.TryParse(Data.GetValue(@"ApplicationSettings\TitleReplacement\numClasses"), out replacementCount))
            {
                for (var i = 0; i < replacementCount; i++)
                {
                    string section = $"ApplicationSettings\\TitleReplacement\\{i}\\";

                    var replacement = new TitleReplacement();
                    replacement.ReadValues(Data, section);

                    if (replacement.ReplacementType == ReplacementType.RegEx)
                        continue;

                    if (String.IsNullOrEmpty(replacement.Replace))
                        continue;

                    replacement.ReplacementType = ReplacementType.RegEx;
                    replacement.Search = Regex.Escape(replacement.Search);
                    replacement.Replace = Regex.Escape(replacement.Replace);

                    replacement.StoreValues(Data, section);
                }
            }
        }

        private void V7ExtractTimeServerAccounts()
        {
            var accounts = new Dictionary<TimeServerAccount, List<int>>();
            accounts[new TimeServerAccount { Url = "https://freetsa.org/tsr" }] = new List<int>();
            accounts[new TimeServerAccount { Url = "http://timestamp.digicert.com" }] = new List<int>();
            accounts[new TimeServerAccount { Url = "http://timestamp.globalsign.com/scripts/timstamp.dll" }] = new List<int>();

            ForAllProfiles((s, i) =>
            {
                var path = s + @"PdfSettings\Signature\";
                if (GetBool(Data.GetValue(path + "Enabled")) != true)
                    return;

                var account = new TimeServerAccount();
                account.IsSecured = GetBool(Data.GetValue(path + "TimeServerIsSecured")) == true;
                account.UserName = Data.GetValue(path + "TimeServerLoginName");
                account.Password = Data.Decrypt(Data.GetValue(path + "TimeServerPassword"));
                account.Url = Data.GetValue(path + "TimeServerUrl");

                var existingAccount = accounts.Keys.FirstOrDefault(a => a.Equals(account));
                if (existingAccount != null)
                {
                    accounts[existingAccount].Add(i);
                }
                else
                {
                    accounts[account] = new List<int>();
                    accounts[account].Add(i);
                }
            });

            for (var i = 0; i < accounts.Count; i++)
            {
                var account = accounts.Keys.ToArray()[i];
                account.AccountId = Guid.NewGuid().ToString();

                account.StoreValues(Data, $"ApplicationSettings\\Accounts\\TimeServerAccounts\\{i}\\");

                foreach (var profileId in accounts[account])
                {
                    Data.SetValue($"ConversionProfiles\\{profileId}\\PdfSettings\\Signature\\TimeServerAccountId", account.AccountId);
                }
            }
            Data.SetValue(@"ApplicationSettings\Accounts\TimeServerAccounts\numClasses", accounts.Keys.Count.ToString());
        }

        private void V7ExtractFtpAccounts()
        {
            var accounts = new Dictionary<FtpAccount, List<int>>();

            ForAllProfiles((s, i) =>
            {
                var path = s + @"Ftp\";
                if (GetBool(Data.GetValue(path + "Enabled")) != true)
                    return;

                var account = new FtpAccount();
                account.UserName = Data.GetValue(path + "UserName");
                account.Password = Data.Decrypt(Data.GetValue(path + "Password"));
                account.Server = Data.GetValue(path + "Server");

                var existingAccount = accounts.Keys.FirstOrDefault(a => a.Equals(account));
                if (existingAccount != null)
                {
                    accounts[existingAccount].Add(i);
                }
                else
                {
                    accounts[account] = new List<int>();
                    accounts[account].Add(i);
                }
            });

            for (var i = 0; i < accounts.Count; i++)
            {
                var account = accounts.Keys.ToArray()[i];
                account.AccountId = Guid.NewGuid().ToString();

                account.StoreValues(Data, $"ApplicationSettings\\Accounts\\FtpAccounts\\{i}\\");

                foreach (var profileId in accounts[account])
                {
                    Data.SetValue($"ConversionProfiles\\{profileId}\\Ftp\\AccountId", account.AccountId);
                }
            }
            Data.SetValue(@"ApplicationSettings\Accounts\FtpAccounts\numClasses", accounts.Keys.Count.ToString());
        }

        private void V7ExtractSmtpAccounts()
        {
            var accounts = new Dictionary<SmtpAccount, List<int>>();

            ForAllProfiles((s, i) =>
            {
                var path = s + @"EmailSmtpSettings\";
                if (GetBool(Data.GetValue(path + "Enabled")) != true)
                    return;

                var account = new SmtpAccount();
                account.Address = Data.GetValue(path + "Address");
                account.Password = Data.Decrypt(Data.GetValue(path + "Password"));
                account.Port = GetInt(Data.GetValue(path + "Port")) ?? 25;
                account.Server = Data.GetValue(path + "Server");
                account.Ssl = GetBool(Data.GetValue(path + "Ssl")) == true;
                account.UserName = Data.GetValue(path + "UserName");

                var existingAccount = accounts.Keys.FirstOrDefault(a => a.Equals(account));
                if (existingAccount != null)
                {
                    accounts[existingAccount].Add(i);
                }
                else
                {
                    accounts[account] = new List<int>();
                    accounts[account].Add(i);
                }
            });

            for (var i = 0; i < accounts.Count; i++)
            {
                var account = accounts.Keys.ToArray()[i];
                account.AccountId = Guid.NewGuid().ToString();

                account.StoreValues(Data, $@"ApplicationSettings\Accounts\SmtpAccounts\{i}\");

                foreach (var profileId in accounts[account])
                {
                    Data.SetValue($@"ConversionProfiles\{profileId}\EmailSmtpSettings\AccountId", account.AccountId);
                }
            }
            Data.SetValue(@"ApplicationSettings\Accounts\SmtpAccounts\numClasses", accounts.Keys.Count.ToString());
        }

        private string MapTiffColorBlackWhite_V4(string s)
        {
            if (s.Equals("BlackWhite", StringComparison.OrdinalIgnoreCase))
                return "BlackWhiteG4Fax";
            return s;
        }

        private string MapOutputformatPdfA_V3(string s)
        {
            if (s.Equals("PdfA", StringComparison.OrdinalIgnoreCase))
                return "PdfA2B";
            return s;
        }

        private string MapEncryptionNamesV1(string s)
        {
            switch (s)
            {
                case "Low40Bit":
                    return "Rc40Bit";

                case "Medium128Bit":
                    return "Rc128Bit";

                case "High128BitAes":
                    return "Aes128Bit";
            }

            return "Rc128Bit";
        }

        private void ForAllProfiles(Action<string, int> func)
        {
            var numProfiles = GetInt(Data.GetValue(@"ConversionProfiles\numClasses"));

            if (numProfiles != null)
            {
                for (var i = 0; i < numProfiles; i++)
                {
                    var path = $@"ConversionProfiles\{i}\";

                    func(path, i);
                }
            }
        }

        private void MoveSettingInAllProfiles(string oldPath, string newPath)
        {
            var numProfiles = GetInt(Data.GetValue(@"ConversionProfiles\numClasses"));

            if (numProfiles != null)
            {
                for (var i = 0; i < numProfiles; i++)
                {
                    var path = string.Format(@"ConversionProfiles\{0}\", i);
                    MoveValue(path + oldPath, path + newPath);
                }
            }
        }

        private void MapSettingInAllProfiles(string path, Func<string, string> mapFunction)
        {
            var numProfiles = GetInt(Data.GetValue(@"ConversionProfiles\numClasses"));

            if (numProfiles != null)
            {
                for (var i = 0; i < numProfiles; i++)
                {
                    var p = string.Format(@"ConversionProfiles\{0}\" + path, i);
                    MapValue(p, mapFunction);
                }
            }
        }

        private void ApplyNewSettingInAllProfiles(string path, string defaultValue)
        {
            var numProfiles = GetInt(Data.GetValue(@"ConversionProfiles\numClasses"));

            if (numProfiles != null)
            {
                for (var i = 0; i < numProfiles; i++)
                {
                    var p = string.Format(@"ConversionProfiles\{0}\" + path, i);
                    Data.SetValue(p, defaultValue);
                }
            }
        }

        private int? GetInt(string s)
        {
            int i;

            if (!int.TryParse(s, out i))
                return null;
            return i;
        }

        private bool? GetBool(string s)
        {
            bool b;

            if (!bool.TryParse(s, out b))
                return null;
            return b;
        }
    }
}
