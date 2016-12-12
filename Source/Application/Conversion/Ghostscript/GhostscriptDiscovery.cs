using System.Collections.Generic;
using System.Linq;
using SystemInterface.IO;
using SystemWrapper.IO;
using pdfforge.PDFCreator.Utilities;

namespace pdfforge.PDFCreator.Conversion.Ghostscript
{
    public interface IGhostscriptDiscovery
    {
        /// <summary>
        ///     Get the internal instance if it exists, otherwise the installed instance in the given version
        /// </summary>
        /// <returns>The best matching Ghostscript instance</returns>
        GhostscriptVersion GetGhostscriptInstance();
    }

    public class GhostscriptDiscovery : IGhostscriptDiscovery
    {
        private readonly IAssemblyHelper _assemblyHelper;
        private readonly IFile _file;
        private readonly IPathSafe _pathSafe = new PathWrapSafe();

        public GhostscriptDiscovery(IFile file, IAssemblyHelper assemblyHelper)
        {
            _file = file;
            _assemblyHelper = assemblyHelper;
        }

        public List<string> PossibleGhostscriptPaths { get; set; } = new List<string> {"Ghostscript", @"..\..\..\..\..\..\packages\setup\Ghostscript"};

        /// <summary>
        ///     Get the internal instance if it exists, otherwise the installed instance in the given version
        /// </summary>
        /// <returns>The best matching Ghostscript instance</returns>
        public GhostscriptVersion GetGhostscriptInstance()
        {
            var applicationPath = _assemblyHelper.GetPdfforgeAssemblyDirectory();

            var paths = PossibleGhostscriptPaths.Select(path => _pathSafe.Combine(applicationPath, path));

            foreach (var path in paths)
            {
                var exePath = _pathSafe.Combine(path, @"Bin\gswin32c.exe");
                var libPath = _pathSafe.Combine(path, @"Bin") + ';' + _pathSafe.Combine(path, @"Lib") + ';' +
                              _pathSafe.Combine(path, @"Fonts");

                if (_file.Exists(exePath))
                    return new GhostscriptVersion("<internal>", exePath, libPath);
            }

            return null;
        }
    }
}
