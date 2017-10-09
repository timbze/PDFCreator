using Microsoft.Win32;
using pdfforge.DataStorage.Storage;
using System.Text;

namespace pdfforge.PDFCreator.Core.SettingsManagement
{
    public interface IDataStorageFactory
    {
        IStorage BuildIniStorage();

        IStorage BuildRegistryStorage(RegistryHive registryHive, string baseKey, bool clearOnWrite = false);
    }

    public class DataStorageFactory : IDataStorageFactory
    {
        public IStorage BuildIniStorage()
        {
            return new IniStorage(Encoding.UTF8);
        }

        public IStorage BuildRegistryStorage(RegistryHive registryHive, string baseKey, bool clearOnWrite = false)
        {
            return new RegistryStorage(registryHive, baseKey, clearOnWrite);
        }
    }
}
