using System;
using SystemInterface.Microsoft.Win32;
using NUnit.Framework;
using pdfforge.PDFCreator.Utilities.Licensing;
using Rhino.Mocks;

namespace PDFCreator.Utilities.UnitTest.Licensing
{
    [TestFixture]
    public class MachineIdTest
    {
        [Test]
        public void BuildMachineId_WithNonexistingRegistryKey_CreatesIdWithEmptyString()
        {
            var registry = MockRepository.GenerateStub<IRegistry>();
            registry.Stub(x => x.GetValue("", "", "")).IgnoreArguments().Return("");
            Func<long> getSerialFunc = () => 0;

            var machineId = MachineId.BuildCurrentMachineId(getSerialFunc, registry);

            Assert.AreEqual("", machineId.WindowsProductId);
        }

        [Test]
        public void BuildMachineId_WithValue_CreatesIdWithProductId()
        {
            var registry = MockRepository.GenerateStub<IRegistry>();
            registry.Stub(x => x.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ProductId", null)).Return("FAKE_ID");
            Func<long> getSerialFunc = () => 0;

            var machineId = MachineId.BuildCurrentMachineId(getSerialFunc, registry);

            Assert.AreEqual("FAKE_ID", machineId.WindowsProductId);
        }

        [Test]
        public void BuildMachineId_WithVolumeSerialFuncion_AppliesCorrectSerial()
        {
            var registry = MockRepository.GenerateStub<IRegistry>();
            registry.Stub(x => x.GetValue("", "", "")).IgnoreArguments().Return("");
            Func<long> getSerialFunc = () => 123456789;

            var machineId = MachineId.BuildCurrentMachineId(getSerialFunc, registry);

            Assert.AreEqual(123456789, machineId.SystemVolumeSerial);
        }

        [Test]
        public void CalculateHash_WithEmptySalt_ThrowsArgumentException()
        {
            var machineId = new MachineId(12345, "abcdef");

            Assert.Throws<ArgumentException>(() => machineId.CaclculateMachineHash(""));
        }

        [Test]
        public void CalculateHash_WithNullSalt_ThrowsArgumentException()
        {
            var machineId = new MachineId(12345, "abcdef");

            Assert.Throws<ArgumentException>(() => machineId.CaclculateMachineHash(null));
        }

        [Test]
        public void MachineId_Contstructor_InitializesProperties()
        {
            var machineId = new MachineId(12345, "abcdef");

            Assert.AreEqual(12345, machineId.SystemVolumeSerial);
            Assert.AreEqual("abcdef", machineId.WindowsProductId);
        }

        [Test]
        public void MachineId_Contstructor_StripsSlashesForProductId()
        {
            var machineId = new MachineId(12345, "1234-5678");

            Assert.AreEqual("12345678", machineId.WindowsProductId);
        }

        [Test]
        public void MachineId_WithSerialAndProductId_GeneratesCorrectHashForDefaultSalt()
        {
            // 53929 => hex D2A9
            var machineId = new MachineId(53929, "12345-99-1234");

            // hash of 0000D2A912345991234GQ461qpa6s0SeD4qabZce6JVP7sTywtN
            Assert.AreEqual("47154b69a68069bdb601e048b74ba63e0595b22a", machineId.CaclculateMachineHash());
        }

        [Test]
        public void MachineId_WithSerialAndProductId_GeneratesCorrectHashForGivenSalt()
        {
            // 53929 => hex D2A9
            var machineId = new MachineId(53929, "12345-99-1234");

            // hash of 0000D2A912345991234salt
            Assert.AreEqual("db5bc63c9dda4552c3cd8e791213918b944d1718", machineId.CaclculateMachineHash("salt"));
        }
    }
}