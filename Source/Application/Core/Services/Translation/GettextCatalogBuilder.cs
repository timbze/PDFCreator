using System.IO;
using NGettext;

namespace pdfforge.PDFCreator.Core.Services.Translation
{
    public interface IGettextCatalogBuilder
    {
        ICatalog GetCatalog(string messageDomain, string languageName);
    }

    public class GettextCatalogBuilder : IGettextCatalogBuilder
    {
        private readonly string _localeFolder;

        public GettextCatalogBuilder(string localeFolder)
        {
            _localeFolder = localeFolder;
        }

        public ICatalog GetCatalog(string messageDomain, string languageName)
        {
            var messageFile = $"{_localeFolder}\\{languageName}\\LC_MESSAGES\\{messageDomain}.mo";
            if (!File.Exists(messageFile))
                return new Catalog();

            using (var s = File.OpenRead(messageFile))
            {
                return new Catalog(s);
            }
        }
    }
}