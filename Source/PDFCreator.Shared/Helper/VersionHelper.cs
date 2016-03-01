using System;
using pdfforge.PDFCreator.Utilities;

namespace pdfforge.PDFCreator.Shared.Helper
{
    public interface IVersionHelper
    {
        Version ApplicationVersion { get; }
        string FormatWithTwoDigits();
        string FormatWithThreeDigits();
        string FormatWithBuildNumber();
    }

    public class VersionHelper : IVersionHelper
    {
        public VersionHelper(Version applicationVersion)
        {
            ApplicationVersion = applicationVersion;
        }

        public VersionHelper()
        {
            var iVersion = new AssemblyHelper().GetCurrentAssemblyVersion();
            ApplicationVersion = new Version(iVersion.Major, iVersion.Minor, iVersion.Build, iVersion.Revision);
        }

        public Version ApplicationVersion { get; private set; }

        private static IVersionHelper _instance;

        public static IVersionHelper Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new VersionHelper();
                return _instance;
            }
        }

        /// <summary>
        /// Get current application version as string
        /// </summary>
        /// <returns>Version in format "x.x"</returns>
        public string FormatWithTwoDigits()
        {
            var v = ApplicationVersion;
            var currentVersion = string.Format("{0}.{1}", v.Major, v.Minor);

            return currentVersion;
        }

        /// <summary>
        /// Get current application version as string
        /// </summary>
        /// <returns>Version in format "x.x.x"</returns>
        public string FormatWithThreeDigits()
        {
            var v = ApplicationVersion;
            var currentVersion = string.Format("{0}.{1}.{2}", v.Major, v.Minor, v.Build);

            return currentVersion;
        }

        /// <summary>
        /// Get current application version with build number
        /// </summary>
        /// <returns>Version with Buildnumber in format "vX.X.X Build XXXX"</returns>
        public string FormatWithBuildNumber()
        {
            var v = ApplicationVersion;
            var currentVersionString = string.Format("v{0}.{1}.{2}", v.Major, v.Minor, v.Build);
            if (v.Revision == 0)
                currentVersionString += @" (Developer Preview)";
            else
                currentVersionString += string.Format(" Build {0}", v.Revision);

            return currentVersionString;
        }
    }
}