using System.Collections.Generic;
using System.Linq;
using SystemInterface.IO;
using SystemInterface.Microsoft.Win32;
using SystemWrapper.IO;
using SystemWrapper.Microsoft.Win32;
using pdfforge.PDFCreator.Utilities;

namespace pdfforge.PDFCreator.Core.Ghostscript
{
    public class GhostscriptDiscovery
    {
        public const string GhostscriptVersion = "9.10";
        
        private readonly IFile _file;
        private readonly IRegistry _registry;
        private readonly IPathSafe _pathSafe = new PathWrapSafe();

        public GhostscriptDiscovery() : this(new FileWrap(), new RegistryWrap(), new AssemblyHelper(), new OsHelper())
        { }

        public GhostscriptDiscovery(IFile file, IRegistry registry, IAssemblyHelper assemblyHelper, IOsHelper osHelper)
        {
            _file = file;
            _registry = registry;

            ApplicationPath = assemblyHelper.GetCurrentAssemblyDirectory();

            if (osHelper.Is64BitOperatingSystem)
                RegistryPath = "SOFTWARE\\Wow6432Node\\GPL Ghostscript";
            else
                RegistryPath = "SOFTWARE\\GPL Ghostscript";
        }

        public string ApplicationPath { get; }

        public string RegistryPath { get; }

        /// <summary>
        ///     Search for Ghostscript instances in the application folder
        /// </summary>
        /// <returns>A GhostscriptVersion if an internal instance exists, null otherwise</returns>
        public GhostscriptVersion FindInternalInstance()
        {
            string[] paths = { _pathSafe.Combine(ApplicationPath, "Ghostscript"), _pathSafe.Combine(ApplicationPath, @"..\..\Ghostscript") };

            foreach (string path in paths)
            {
                string exePath = _pathSafe.Combine(path, @"Bin\gswin32c.exe");
                string libPath = _pathSafe.Combine(path, @"Bin") + ';' + _pathSafe.Combine(path, @"Lib") + ';' +
                                 _pathSafe.Combine(path, @"Fonts");

                if (_file.Exists(exePath))
                {
                    return new GhostscriptVersion("<internal>", exePath, libPath);
                }
            }

            return null;
        }

        /// <summary>
        ///     Get the internal instance if it exists, otherwise the installed instance in the given version
        /// </summary>
        /// <returns>The best matching Ghostscript instance</returns>
        public GhostscriptVersion GetBestGhostscriptInstance()
        {
            return GetBestGhostscriptInstance(GhostscriptVersion);
        }

        /// <summary>
        ///     Get the internal instance if it exists, otherwise the installed instance in the given version
        /// </summary>
        /// <param name="requiredGsVersion">For Tests with specific Ghostscript version</param>
        /// <returns>The best matching Ghostscript instance</returns>
        public GhostscriptVersion GetBestGhostscriptInstance(string requiredGsVersion)
        {   
            var version = FindInternalInstance();

            if (version != null)
                return version;

            IList<GhostscriptVersion> versions = FindRegistryInstances();

            if (versions.Count == 0)
                return null;

            return versions.FirstOrDefault(v => v.Version == requiredGsVersion);
        }

        private IList<GhostscriptVersion> FindRegistryInstances()
        {
            var versions = new List<GhostscriptVersion>();

            IRegistryKey gsMainKey = _registry.LocalMachine.OpenSubKey(RegistryPath);

            if (gsMainKey == null)
                return versions;

            foreach (string subkey in gsMainKey.GetSubKeyNames())
            {
                GhostscriptVersion v = IsGhostscriptInstalled(subkey);

                if (v != null)
                    versions.Add(v);
            }

            return versions;
        }

        /// <summary>
        ///     Check if Ghostscript is installed with a given version. It does a lookup in the registry and checks if the paths
        ///     exist.
        /// </summary>
        /// <param name="version">Name of the version to check, i.e. "9.05"</param>
        /// <returns>A GhostscriptVersion object if a version has been found, null otherwise</returns>
        private GhostscriptVersion IsGhostscriptInstalled(string version)
        {
            try
            {
                IRegistryKey myKey = _registry.LocalMachine.OpenSubKey(RegistryPath + "\\" + version);
                if (myKey == null)
                    return null;

                var gsDll = (string)myKey.GetValue("GS_DLL");
                var gsLib = (string)myKey.GetValue("GS_LIB");

                var gsExe = _pathSafe.Combine(_pathSafe.GetDirectoryName(gsDll), "gswin32c.exe");

                myKey.Close();
                if (_file.Exists(gsExe))
                {
                    return new GhostscriptVersion(version, gsExe, gsLib);
                }
            }
            catch
            {
                return null;
            }

            return null;
        }
    }
}