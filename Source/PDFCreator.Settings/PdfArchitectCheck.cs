using System;
using System.IO;
using SystemInterface.IO;
using SystemInterface.Microsoft.Win32;
using SystemWrapper.IO;
using SystemWrapper.Microsoft.Win32;

namespace pdfforge.PDFCreator.Core.Settings
{
    public static class PdfArchitectCheck
    {
        // Tuple format: Item1: DisplayName in Registry, Item2: name of the exe file that has to exist in the InstallLocation
        private static readonly Tuple<string, string>[] PdfArchitectCandidates =
        {
            new Tuple<string, string>("PDF Architect 4", "architect.exe"),
            new Tuple<string, string>("PDF Architect 3", "PDF Architect 3.exe"),
            new Tuple<string, string>("PDF Architect 3", "architect.exe"),
            new Tuple<string, string>("PDF Architect 2", "PDF Architect 2.exe"),
            new Tuple<string, string>("PDF Architect", "PDF Architect.exe")
        };

        private static readonly string[] SoftwareKeys =
        {
            @"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall",
            @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall"
        };

        /// <summary>
        /// Finds the exe path of a known version of PDF Architect by checking if the Registry contains a
        /// programm with Publisher "pdfforge" and a known DisplayName
        /// </summary>
        /// <returns>Returns installation path if a known version of PDF Architect is installed, else null</returns>
        public static string InstallationPath()
        {
            return InstallationPath(new RegistryWrap(), new FileWrap());
        }

        /// <summary>
        /// Finds the exe path of a known version of PDF Architect by checking if the Registry contains a
        /// programm with Publisher "pdfforge" and a known DisplayName
        /// </summary>
        /// <returns>Returns installation path if a known version of PDF Architect is installed, else null</returns>
        public static string InstallationPath(IRegistry registry, IFile file)
        {
            foreach (var pdfArchitectCandidate in PdfArchitectCandidates)
            {
                var installationPath = TryFindInstallationPath(pdfArchitectCandidate.Item1, pdfArchitectCandidate.Item2, registry, file);
                
                if (installationPath != null)
                    return installationPath;
            }

            return null;
        }

        private static string TryFindInstallationPath(string msiDisplayName, string applicationExeName, IRegistry registry, IFile file)
        {
            foreach (string key in SoftwareKeys)
            {
                using (IRegistryKey rk = registry.LocalMachine.OpenSubKey(key))
                {
                    if (rk == null)
                        continue;

                    //Let's go through the registry keys and get the info we need:
                    foreach (string skName in rk.GetSubKeyNames())
                    {
                        using (IRegistryKey sk = rk.OpenSubKey(skName))
                        {
                            if (sk == null)
                                continue;

                            try
                            {
                                //first look for PDF Architect 3
                                //If the key has value, continue, if not, skip it:
                                string displayName = sk.GetValue("DisplayName").ToString();
                                if ((displayName.StartsWith(msiDisplayName, StringComparison.OrdinalIgnoreCase)) &&
                                    !displayName.Contains("Enterprise") &&
                                    (sk.GetValue("Publisher").ToString().Contains("pdfforge")) &&
                                    (sk.GetValue("InstallLocation") != null))
                                {
                                    var installLocation = sk.GetValue("InstallLocation").ToString();
                                    var exePath = Path.Combine(installLocation, applicationExeName);

                                    if (file.Exists(exePath))
                                        return exePath;

                                    // if the exe does not exist, this is the wrong path
                                    return null;
                                }
                            }
                            catch (Exception)
                            {
                                // ignored
                            }
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Check if a known version of PDF Architect is installed
        /// </summary>
        /// <returns>Returns true, if PDF Architect is installed</returns>
        public static bool Installed()
        {
            return Installed(new RegistryWrap(), new FileWrap());
        }

        /// <summary>
        /// Check if a known version of PDF Architect is installed
        /// </summary>
        /// <returns>Returns true, if PDF Architect is installed</returns>
        public static bool Installed(IRegistry registry, IFile file)
        {
            return InstallationPath(registry, file) != null;
        }
    }
}