using System.Collections.Generic;
using System.IO;
using SystemInterface.IO;
using SystemWrapper.IO;
using pdfforge.PDFCreator.Conversion.Ghostscript;
using pdfforge.PDFCreator.Utilities;

namespace PDFCreator.TestUtilities
{
    public class PaketGhostscriptDiscovery : IGhostscriptDiscovery
    {
        private readonly IPathSafe _pathSafe = new PathWrapSafe();

        private IEnumerable<string> GetPathCandidates()
        {
            var assemblyHelper = new AssemblyHelper();

            var path = assemblyHelper.GetPdfforgeAssemblyDirectory();

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
