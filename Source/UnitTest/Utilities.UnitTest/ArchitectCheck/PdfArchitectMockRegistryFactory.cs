using Rhino.Mocks;
using System.Collections.Generic;
using System.IO;
using SystemInterface.IO;
using SystemInterface.Microsoft.Win32;

namespace pdfforge.PDFCreator.Utilities.UnitTest.ArchitectCheck
{
    internal class PdfArchitectMockRegistryFactory
    {
        private readonly IList<PdfArchitectVersion> _pdfArchitectVersions = new List<PdfArchitectVersion>();

        public IRegistry BuildRegistry()
        {
            var hklm = MockRepository.GenerateStub<IRegistryKey>();

            var softwareKey = MockRepository.GenerateStub<IRegistryKey>();
            var softwareWow64Key = MockRepository.GenerateStub<IRegistryKey>();

            AddArchitectVersionKeys(softwareKey, softwareWow64Key);

            hklm.Stub(x => x.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall")).Return(softwareKey);
            hklm.Stub(x => x.OpenSubKey(@"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall")).Return(softwareWow64Key);

            var registry = MockRepository.GenerateStub<IRegistry>();
            registry.Stub(x => x.LocalMachine).Return(hklm);

            return registry;
        }

        public IFile BuildFile()
        {
            var file = MockRepository.GenerateStub<IFile>();

            foreach (var pdfArchitectVersion in _pdfArchitectVersions)
            {
                var version = pdfArchitectVersion;
                file.Stub(x => x.Exists(Path.Combine(version.InstallLocation, version.ExeName)))
                    .Return(true);
            }

            return file;
        }

        private void AddArchitectVersionKeys(IRegistryKey softwareKey, IRegistryKey softwareWow64Key)
        {
            var subkeys = new List<string>();
            var subkeysWow64 = new List<string>();

            foreach (var pdfArchitectVersion in _pdfArchitectVersions)
            {
                var versionKey = MockRepository.GenerateStub<IRegistryKey>();
                versionKey.Stub(x => x.GetValue("DisplayName")).Return(pdfArchitectVersion.DisplayName);
                versionKey.Stub(x => x.GetValue("InstallLocation")).Return(pdfArchitectVersion.InstallLocation);
                versionKey.Stub(x => x.GetValue("Publisher")).Return("pdfforge");

                var version = pdfArchitectVersion;

                if (pdfArchitectVersion.IsWow64)
                {
                    softwareWow64Key.Stub(x => x.OpenSubKey(version.SubkeyName)).Return(versionKey);
                    subkeysWow64.Add(pdfArchitectVersion.SubkeyName);
                }
                else
                {
                    softwareKey.Stub(x => x.OpenSubKey(version.SubkeyName)).Return(versionKey);
                    subkeys.Add(pdfArchitectVersion.SubkeyName);
                }
            }

            softwareKey.Stub(x => x.GetSubKeyNames()).Return(subkeys.ToArray());
            softwareWow64Key.Stub(x => x.GetSubKeyNames()).Return(subkeysWow64.ToArray());
        }

        public void AddArchitectVersion(string subkeyName, string displayName, string installLocation, string exeName, bool isWow64)
        {
            _pdfArchitectVersions.Add(new PdfArchitectVersion(subkeyName, displayName, installLocation, exeName, isWow64));
        }
    }
}
