using pdfforge.PDFCreator.Conversion.Ghostscript;
using pdfforge.PDFCreator.Utilities;
using System.Collections.Generic;
using System.IO;
using SystemInterface.IO;
using SystemWrapper.IO;

namespace PDFCreator.TestUtilities
{
    public class PaketGhostscriptDiscovery : IGhostscriptDiscovery
    {
        private readonly IAssemblyHelper _assemblyHelper;
        private readonly IPathSafe _pathSafe = new PathWrapSafe();

        public PaketGhostscriptDiscovery(IAssemblyHelper assemblyHelper)
        {
            _assemblyHelper = assemblyHelper;
        }

        private IEnumerable<string> GetPathCandidates()
        {
            var path = _assemblyHelper.GetAssemblyDirectory();

            while (path.Length > 3)
            {
                yield return path;

                path = Path.GetDirectoryName(path) ?? "";
            }
        }

        public GhostscriptVersion GetGhostscriptInstance()
        {
            foreach (var path in GetPathCandidates())
            {
                var testPath = _pathSafe.Combine(path, @"packages\setup\Ghostscript");

                var exePath = _pathSafe.Combine(testPath, @"Bin\gswin32c.exe");
                var libPath = _pathSafe.Combine(testPath, @"Bin") + ';' + _pathSafe.Combine(testPath, @"Lib") + ';' +
                              _pathSafe.Combine(testPath, @"Fonts");

                if (File.Exists(exePath))
                {
                    return new GhostscriptVersion("<internal>", exePath, libPath);
                }
            }

            return null;
        }
    }
}
