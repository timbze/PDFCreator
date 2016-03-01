using SystemInterface.Microsoft.Win32;
using NUnit.Framework;
using pdfforge.PDFCreator.Mail;
using Rhino.Mocks;

namespace PDFCreator.Mail.UnitTest
{
    [TestFixture]
    public class MapiClientTest
    {
        [Test]
        public void IsMapiClientInstalled_WhenRegistryKeysAreEmpty_IsFalse()
        {
            var registryStub = MockRepository.GenerateStub<IRegistry>();
            registryStub.Stub(x => x.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\Clients\Mail", "", null)).Return(null);
            registryStub.Stub(x => x.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Clients\Mail", "", null)).Return(null);

            var mapiClient = new MapiClient(registryStub);

            Assert.IsFalse(mapiClient.IsMapiClientInstalled);
        }

        [Test]
        public void IsMapiClientInstalled_WhenCurrentUserKeyIsNotEmpty_IsTrue()
        {
            var registryStub = MockRepository.GenerateStub<IRegistry>();
            registryStub.Stub(x => x.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\Clients\Mail", "", null)).Return("Some mail client");
            registryStub.Stub(x => x.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Clients\Mail", "", null)).Return(null);

            var mapiClient = new MapiClient(registryStub);

            Assert.IsTrue(mapiClient.IsMapiClientInstalled);
        }

        [Test]
        public void IsMapiClientInstalled_WhenLocalMachineKeyIsNotEmpty_IsTrue()
        {
            var registryStub = MockRepository.GenerateStub<IRegistry>();
            registryStub.Stub(x => x.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\Clients\Mail", "", null)).Return(null);
            registryStub.Stub(x => x.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Clients\Mail", "", null)).Return("Some mail client");

            var mapiClient = new MapiClient(registryStub);

            Assert.IsTrue(mapiClient.IsMapiClientInstalled);
        }
    }
}
