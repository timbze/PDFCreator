using NSubstitute;
using System.Collections.Generic;
using System.IO;
using SystemInterface.IO;
using SystemInterface.Microsoft.Win32;

namespace pdfforge.PDFCreator.Utilities.UnitTest.ArchitectCheck
{
    internal class PdfArchitectMockRegistryFactory
    {
        private readonly IList<PdfArchitectVersion> _pdfArchitectVersions = new List<PdfArchitectVersion>();

        public IRegistry BuildRegistry(bool throwException = false)
        {
            var hklm = Substitute.For<IRegistryKey>();

            var softwareKey = Substitute.For<IRegistryKey>();
            var softwareWow64Key = Substitute.For<IRegistryKey>();

            AddArchitectVersionKeys(softwareKey, softwareWow64Key);

            if (throwException)
            {
                hklm.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall").Returns(x => throw new IOException());
                hklm.OpenSubKey(@"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall").Returns(x => throw new IOException());
            }
            else
            {
                hklm.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall").Returns(softwareKey);
                hklm.OpenSubKey(@"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall").Returns(softwareWow64Key);
            }

            var registry = Substitute.For<IRegistry>();
            registry.LocalMachine.Returns(hklm);

            return registry;
        }

        public IFile BuildFile()
        {
            var file = Substitute.For<IFile>();

            foreach (var pdfArchitectVersion in _pdfArchitectVersions)
            {
                var version = pdfArchitectVersion;
                file.Exists(Path.Combine(version.InstallLocation, version.ExeName))
                    .Returns(true);
            }

            return file;
        }

        private void AddArchitectVersionKeys(IRegistryKey softwareKey, IRegistryKey softwareWow64Key)
        {
            var subkeys = new List<string>();
            var subkeysWow64 = new List<string>();

            foreach (var pdfArchitectVersion in _pdfArchitectVersions)
            {
                var versionKey = Substitute.For<IRegistryKey>();
                versionKey.GetValue("DisplayName").Returns(pdfArchitectVersion.DisplayName);
                versionKey.GetValue("InstallLocation").Returns(pdfArchitectVersion.InstallLocation);
                versionKey.GetValue("Publisher").Returns("pdfforge");

                var version = pdfArchitectVersion;

                IRegistryKey mockKey;
                IList<string> subkeyList;

                if (pdfArchitectVersion.IsWow64)
                {
                    mockKey = softwareWow64Key;
                    subkeyList = subkeysWow64;
                }
                else
                {
                    mockKey = softwareKey;
                    subkeyList = subkeys;
                }

                subkeyList.Add(pdfArchitectVersion.SubkeyName);

                if (pdfArchitectVersion.ThrowsException)
                    mockKey.OpenSubKey(version.SubkeyName).Returns(x => throw new IOException());
                else
                    mockKey.OpenSubKey(version.SubkeyName).Returns(versionKey);
            }

            softwareKey.GetSubKeyNames().Returns(subkeys.ToArray());
            softwareWow64Key.GetSubKeyNames().Returns(subkeysWow64.ToArray());
        }

        public void AddArchitectVersion(string subkeyName, string displayName, string installLocation, string exeName, bool isWow64, bool throwsException = false)
        {
            _pdfArchitectVersions.Add(new PdfArchitectVersion(subkeyName, displayName, installLocation, exeName, isWow64, throwsException));
        }
    }
}
