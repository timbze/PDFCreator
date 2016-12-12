using System;
using System.IO;
using System.Linq;
using System.Reflection;
using pdfforge.PDFCreator.Utilities;

namespace pdfforge.PDFCreator.Core.SettingsManagement
{
    public interface IPDFCreatorNameProvider
    {
        string GetName();
    }

    public class PDFCreatorNameProvider : IPDFCreatorNameProvider
    {
        private readonly IAssemblyHelper _assemblyHelper;

        public PDFCreatorNameProvider(IAssemblyHelper assemblyHelper)
        {
            _assemblyHelper = assemblyHelper;
        }

        public string GetName()
        {
            var assemblyDirectory = _assemblyHelper.GetPdfforgeAssemblyDirectory();
            var directoryInfo = new DirectoryInfo(assemblyDirectory);
            var candidates = directoryInfo.GetFiles("PDFCreator*.exe").Where(file => !file.Name.Contains(".vshost.exe"));

            if (candidates.Count() != 1)
                throw new ApplicationException("The assembly directory contains more or less than one PDFCreator*.exe");

            return candidates.First().Name;
        }
    }
}
