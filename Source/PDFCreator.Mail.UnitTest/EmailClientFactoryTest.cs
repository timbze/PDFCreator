using SystemInterface.Microsoft.Win32;
using NUnit.Framework;
using pdfforge.PDFCreator.Mail;
using Rhino.Mocks;

namespace PDFCreator.Mail.UnitTest
{
    [TestFixture]
    public class EmailClientFactoryTest
    {        
        [Test]
        public void UserRegistryKeyContainsOutlook_EmailClientFactoryReturnsOutlookClient()
        {
            var registryStub = MockRepository.GenerateStub<IRegistry>();
            registryStub.Stub(x => x.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\Clients\Mail", "", null)).Return("Something with Outlook");

            var eMailClientFactory = CreateFactoryWithInstalledOutlookMock(registryStub);
            var client = eMailClientFactory.CreateEmailClient();

            registryStub.AssertWasCalled(x => x.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\Clients\Mail", "", null));
            Assert.IsInstanceOf<OutlookClient>(client, "Wrong Type of EmailClient");
        }

        [Test]
        public void UserRegistryKeyContainsIsEmptyAndMachineKeyContainsOutlook_EmailClientFactoryReturnsOutlookClient()
        {
            var registryStub = MockRepository.GenerateStub<IRegistry>();
            registryStub.Stub(x => x.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\Clients\Mail", "", null)).Return(null);
            registryStub.Stub(x => x.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Clients\Mail", "", null)).Return("Something with Outlook");

            var eMailClientFactory = CreateFactoryWithInstalledOutlookMock(registryStub);
            var client = eMailClientFactory.CreateEmailClient();

            registryStub.AssertWasCalled(x => x.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\Clients\Mail", "", null));
            registryStub.AssertWasCalled(x => x.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Clients\Mail", "", null));
            Assert.IsInstanceOf<OutlookClient>(client, "Wrong Type of EmailClient");
        }

        [Test]
        public void UserRegistryKeyContainsIsEmptyAndMachineKeyContainsOutlookAndOutlookNotInstalled_EmailClientFactoryReturnsMapiClient()
        {
            var registryStub = MockRepository.GenerateStub<IRegistry>();
            registryStub.Stub(x => x.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\Clients\Mail", "", null)).Return(null);
            registryStub.Stub(x => x.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Clients\Mail", "", null)).Return("Something with Outlook");

            var eMailClientFactory = CreateFactoryWithNotInstalledOutlookMock(registryStub);
            var client = eMailClientFactory.CreateEmailClient();

            registryStub.AssertWasCalled(x => x.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\Clients\Mail", "", null));
            registryStub.AssertWasCalled(x => x.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Clients\Mail", "", null));
            Assert.IsInstanceOf<MapiClient>(client, "Wrong Type of EmailClient");
        }

        [Test]
        public void RegistryKeyDoesNotContainOutlook_EmailClientFactoryReturnsMapiClient()
        {
            var registryStub = MockRepository.GenerateStub<IRegistry>();
            registryStub.Stub(x => x.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\Clients\Mail", "", null)).Return("Something like Thunderbird");

            var eMailClientFactory = CreateFactoryWithInstalledOutlookMock(registryStub);
            var client = eMailClientFactory.CreateEmailClient();

            registryStub.AssertWasCalled(x => x.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\Clients\Mail", "", null));
            Assert.IsInstanceOf<MapiClient>(client, "Wrong Type of EmailClient");
        }

        private EmailClientFactory CreateFactoryWithInstalledOutlookMock(IRegistry registry)
        {
            var outlookClient = MockRepository.GenerateStub<OutlookClient>();

            outlookClient.Stub(x => x.IsOutlookInstalled).Return(true);

            return new MockedOutlookEmailClientFactory(registry, outlookClient);
        }

        private EmailClientFactory CreateFactoryWithNotInstalledOutlookMock(IRegistry registry)
        {
            var outlookClient = MockRepository.GenerateStub<OutlookClient>();

            outlookClient.Stub(x => x.IsOutlookInstalled).Return(false);

            return new MockedOutlookEmailClientFactory(registry, outlookClient);
        }
    }

    class MockedOutlookEmailClientFactory : EmailClientFactory
    {
        private readonly OutlookClient _outlookClient;

        public MockedOutlookEmailClientFactory(IRegistry registry, OutlookClient outlookClient)
            : base(registry)
        {
            _outlookClient = outlookClient;
        }

        protected override OutlookClient CreateOutlookClient()
        {
            return _outlookClient;
        }
    }
}
