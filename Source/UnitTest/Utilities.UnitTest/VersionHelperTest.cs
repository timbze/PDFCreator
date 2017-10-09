using NUnit.Framework;
using System;
using System.Text.RegularExpressions;

namespace pdfforge.PDFCreator.Utilities.UnitTest
{
    [TestFixture]
    internal class VersionHelperTest
    {
        [Test]
        public void CurrentVersion_WithBuildNumber_HasValidFormat()
        {
            var version = new Version(1, 2, 3, 0);
            var versionHelper = new VersionHelper(version);

            var formattedVersion = versionHelper.FormatWithBuildNumber();

            Assert.IsTrue(Regex.IsMatch(formattedVersion, @"^v\d+.\d+.\d+ \(Developer Preview\)$"),
                "Current Version has invalid formatting.");
        }

        [Test]
        public void CurrentVersion_WithoutBuildNumber_HasValidFormat()
        {
            var version = new Version(1, 2, 3, 4);
            var versionHelper = new VersionHelper(version);

            var formattedVersion = versionHelper.FormatWithBuildNumber();

            Assert.IsTrue(Regex.IsMatch(formattedVersion, @"^v\d+.\d+.\d+ Build \d+$"),
                "Current Version has invalid formatting.");
        }

        [Test]
        public void CurrentVersionWithThreeDigits_HasValidFormat()
        {
            var version = new Version(1, 2, 3, 0);
            var versionHelper = new VersionHelper(version);

            var currentVersion = versionHelper.FormatWithThreeDigits();
            Assert.IsTrue(Regex.IsMatch(currentVersion, @"^\d+.\d+.\d+$"), "Current Version has invalid formatting.");
        }

        [Test]
        public void CurrentVersionWithTwoDigits_HasValidFormat()
        {
            var version = new Version(1, 2, 3, 0);
            var versionHelper = new VersionHelper(version);

            var currentVersion = versionHelper.FormatWithTwoDigits();
            Assert.IsTrue(Regex.IsMatch(currentVersion, @"^\d+.\d+$"), "Current Version has invalid formatting.");
        }

        [Test]
        public void VersionHelper_InitializedWithVersion_ContainsCorrectVersion()
        {
            var version = new Version();

            var versionHelper = new VersionHelper(version);

            Assert.AreEqual(version, versionHelper.ApplicationVersion);
        }
    }
}
