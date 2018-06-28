using Microsoft.Win32;
using NSubstitute;
using NUnit.Framework;
using pdfforge.DataStorage;
using pdfforge.DataStorage.Storage;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.Core.Startup;
using System;

namespace pdfforge.PDFCreator.UnitTest.Startup
{
    [TestFixture]
    public class ParametersManagerTest
    {
        private IDataStorageFactory _dataStorageFactory;
        private IInstallationPathProvider _installationPathProvider;
        private ParametersManager _parametersManager;

        [SetUp]
        public void SetUp()
        {
            _dataStorageFactory = Substitute.For<IDataStorageFactory>();
            _installationPathProvider = Substitute.For<IInstallationPathProvider>();
            _parametersManager = new ParametersManager(_installationPathProvider, _dataStorageFactory);
        }

        [Test]
        public void SaveParameterSettings_CallsSetDataAndWriteData()
        {
            _parametersManager.SaveParameterSettings("outputFileParameter", "profileParameter");
            var regStorage = _dataStorageFactory.BuildRegistryStorage(RegistryHive.CurrentUser, _installationPathProvider.ApplicationRegistryPath);

            regStorage.Received(1).Data = Arg.Any<Data>();
            regStorage.Received(1).WriteData(Arg.Any<string>());
        }

        [Test]
        public void NoParametersAreFoundInRegistry_GetAndResetParameters_ThrowsInvalidOperationException()
        {
            _dataStorageFactory.BuildRegistryStorage(RegistryHive.CurrentUser, _installationPathProvider.ApplicationRegistryPath);

            Assert.Throws<InvalidOperationException>(() => _parametersManager.GetAndResetParameters());
        }

        [Test]
        public void FoundAParameterInRegistry_GetAndResetParameters_ParametersAreResetInRegistry()
        {
            var regStorage = Substitute.For<IStorage>();
            Data data = null;
            regStorage
                .When(x => x.Data = Arg.Any<Data>())
                .Do(x => data = x.Arg<Data>());

            regStorage
                .When(x => x.ReadData(Arg.Any<string>()))
                .Do(x => data.SetValue(@"Parameters\Outputfile", "X:\\test.pdf"));

            _dataStorageFactory.BuildRegistryStorage(Arg.Any<RegistryHive>(), Arg.Any<string>()).Returns(regStorage);

            _parametersManager.GetAndResetParameters();

            regStorage.Received().WriteData("");
            Assert.AreEqual("", data.GetValue(@"Parameters\Outputfile"));
        }

        [Test]
        public void OutputFileParameterIsSetInRegistry_HasPredefinedParameters_ReturnsTrue()
        {
            var regStorage = Substitute.For<IStorage>();
            Data data = null;
            regStorage
                .When(x => x.Data = Arg.Any<Data>())
                .Do(x => data = x.Arg<Data>());

            regStorage
                .When(x => x.ReadData(Arg.Any<string>()))
                .Do(x => data.SetValue(@"Parameters\Outputfile", "X:\\test.pdf"));

            _dataStorageFactory.BuildRegistryStorage(Arg.Any<RegistryHive>(), Arg.Any<string>()).Returns(regStorage);

            var parameters = _parametersManager.HasPredefinedParameters();

            Assert.IsTrue(parameters);
        }

        [Test]
        public void ProfileParameterIsSetInRegistry_HasPredefinedParameters_ReturnsTrue()
        {
            var regStorage = Substitute.For<IStorage>();
            Data data = null;
            regStorage
                .When(x => x.Data = Arg.Any<Data>())
                .Do(x => data = x.Arg<Data>());

            regStorage
                .When(x => x.ReadData(Arg.Any<string>()))
                .Do(x => data.SetValue(@"Parameters\Profile", "MyProfile"));

            _dataStorageFactory.BuildRegistryStorage(Arg.Any<RegistryHive>(), Arg.Any<string>()).Returns(regStorage);

            var parameters = _parametersManager.HasPredefinedParameters();

            Assert.IsTrue(parameters);
        }

        [Test]
        public void NoParameterIsSetInRegistry_HasPredefinedParameters_ReturnsFalse()
        {
            var regStorage = Substitute.For<IStorage>();
            Data data = null;
            regStorage
                .When(x => x.Data = Arg.Any<Data>())
                .Do(x => data = x.Arg<Data>());

            regStorage
                .When(x => x.ReadData(Arg.Any<string>()))
                .Do(x =>
                {
                    data.SetValue(@"Parameters\Outputfile", "");
                    data.SetValue(@"Parameters\Profile", "");
                });

            _dataStorageFactory.BuildRegistryStorage(Arg.Any<RegistryHive>(), Arg.Any<string>()).Returns(regStorage);

            var parameters = _parametersManager.HasPredefinedParameters();

            Assert.IsFalse(parameters);
        }
    }
}
