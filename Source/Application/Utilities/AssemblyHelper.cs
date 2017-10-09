using System;
using System.Reflection;
using SystemWrapper.IO;

namespace pdfforge.PDFCreator.Utilities
{
    public interface IAssemblyHelper
    {
        string GetAssemblyDirectory();
    }

    public class AssemblyHelper : IAssemblyHelper
    {
        private readonly PathWrapSafe _pathWrapSafe = new PathWrapSafe();
        private readonly Assembly _assembly;

        public AssemblyHelper(Assembly assembly)
        {
            _assembly = assembly;
        }

        public string GetAssemblyDirectory()
        {
            return _pathWrapSafe.GetDirectoryName(GetAssemblyPath(_assembly));
        }

        private string GetAssemblyPath(Assembly assembly)
        {
            var assemblyPath = assembly.CodeBase;

            if (string.IsNullOrEmpty(assemblyPath))
                assemblyPath = assembly.Location;

            if (assemblyPath.StartsWith(@"file:///", StringComparison.OrdinalIgnoreCase))
                assemblyPath = assemblyPath.Substring(8);

            return assemblyPath;
        }
    }
}
