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

            // Get files that start with PDFCreator, end with exe and have only one dot (to exclude .vshost.exe and PDFCreator.LicenseService.exe)
            var candidates = directoryInfo.GetFiles("PDFCreator*.exe")
                .Where(file => file.Name.Count(c => c == '.') == 1);

            if (candidates.Count() != 1)
                throw new ApplicationException("The assembly directory contains more or less than one PDFCreator*.exe");

            return candidates.First().Name;
        }
    }
}
