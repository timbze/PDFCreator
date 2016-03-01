using System.Security;
using SystemInterface.Microsoft.Win32;
using NUnit.Framework;
using pdfforge.PDFCreator.Helper;
using pdfforge.PDFCreator.Utilities.Registry;
using Rhino.Mocks;

namespace PDFCreator.UnitTest.Helper
{
    [TestFixture]
    public class SettingsMoverTest
    {
        private IRegistry _registryWrap;
        private IRegistryKey _hkcu;
        private IRegistryUtility _registryUtility;

        private const string OldRegistryPath = @"Software\PDFCreator.Net";
        private const string NewRegistryPath = @"Software\pdfforge\PDFCreator";

        [SetUp]
        public void Setup()
        {
            _registryWrap = MockRepository.GenerateStub<IRegistry>();
            _hkcu = MockRepository.GenerateStub<IRegistryKey>();
            _registryWrap.Stub(x => x.CurrentUser).Return(_hkcu);

            _registryUtility = MockRepository.GenerateMock<IRegistryUtility>();
        }

        [Test]
        public void MoveRequired_WhenNoRegistryKeysExist_ReturnsFalse()
        {
            var settingsMover = CreateSettingsMover(_registryWrap);

            Assert.IsFalse(settingsMover.MoveRequired());
        }

        [Test]
        public void MoveRequired_WhenOldRegistryKeysExistsAndOldDoesNotExist_ReturnsTrue()
        {
            _hkcu.Stub(x => x.OpenSubKey(OldRegistryPath))
                .Return(MockRepository.GenerateStub<IRegistryKey>());
            var settingsMover = CreateSettingsMover(_registryWrap);

            Assert.IsTrue(settingsMover.MoveRequired());
        }

        [Test]
        public void MoveRequired_WhenOldRegistryKeysExists_KeyIsClosedAfterUsage()
        {
            var key = MockRepository.GenerateStub<IRegistryKey>();
            _hkcu.Stub(x => x.OpenSubKey(OldRegistryPath))
                .Return(key);
            var settingsMover = CreateSettingsMover(_registryWrap);
            
            settingsMover.MoveRequired();

            key.AssertWasCalled(x => x.Close());
        }

        [Test]
        public void MoveRequired_WhenOldRegistryKeysExistsAndOldDoesExist_ReturnsFalse()
        {
            _hkcu.Stub(x => x.OpenSubKey(OldRegistryPath))
                .Return(MockRepository.GenerateStub<IRegistryKey>());

            _hkcu.Stub(x => x.OpenSubKey(NewRegistryPath))
                .Return(MockRepository.GenerateStub<IRegistryKey>());

            var settingsMover = CreateSettingsMover(_registryWrap);

            Assert.IsFalse(settingsMover.MoveRequired());
        }

        [Test]
        public void MoveRequired_WhenOldRegistryKeysExistsAndOldDoesExist_KeyIsClosedAfterUsage()
        {
            var key = MockRepository.GenerateStub<IRegistryKey>();

            _hkcu.Stub(x => x.OpenSubKey(OldRegistryPath))
                .Return(MockRepository.GenerateStub<IRegistryKey>());

            _hkcu.Stub(x => x.OpenSubKey(NewRegistryPath))
                .Return(key);

            var settingsMover = CreateSettingsMover(_registryWrap);

            settingsMover.MoveRequired();

            key.AssertWasCalled(x => x.Close());
        }

        [Test]
        public void MoveRequired_WithExceptionWhenOpeningOldKey_ReturnsFalse()
        {
            _hkcu.Stub(x => x.OpenSubKey(OldRegistryPath))
                .Throw(new SecurityException());
            var settingsMover = CreateSettingsMover(_registryWrap);

            Assert.IsFalse(settingsMover.MoveRequired());
        }

        [Test]
        public void MoveSettings_WhenMoveIsNotRequired_ReturnsFalse()
        {
            var settingsMover = CreateSettingsMover(_registryWrap);

            Assert.IsFalse(settingsMover.MoveSettings());
        }

        [Test]
        public void MoveSettings_WhenMoveIsRequired_PerformsMove()
        {
            var settingsMover = CreateSettingsMover(_registryWrap);
            _hkcu.Stub(x => x.OpenSubKey(OldRegistryPath))
                .Return(MockRepository.GenerateStub<IRegistryKey>());
            _registryUtility.Stub(x => x.RenameSubKey(_registryWrap.CurrentUser, OldRegistryPath, NewRegistryPath)).Return(true);

            var result = settingsMover.MoveSettings();

            Assert.IsTrue(result);
            _registryUtility.AssertWasCalled(x => x.RenameSubKey(_registryWrap.CurrentUser, OldRegistryPath, NewRegistryPath));
        }

        private SettingsMover CreateSettingsMover(IRegistry registryWrap)
        {
            var settingsMover = new SettingsMover(registryWrap, _registryUtility);

            return settingsMover;
        }
    }
}
