using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using SystemWrapper.IO;

namespace pdfforge.PDFCreator.Utilities
{
    public interface IAssemblyHelper
    {
        string GetPdfforgeAssemblyDirectory();

        Version GetPdfforgeAssemblyVersion();
    }

    public class AssemblyHelper : IAssemblyHelper
    {
        private readonly PathWrapSafe _pathWrapSafe = new PathWrapSafe();

        public string GetPdfforgeAssemblyDirectory()
        {
            var assembly = GetPdfforgeAssemblyFromStackTrace();

            return _pathWrapSafe.GetDirectoryName(GetAssemblyPath(assembly));
        }

        public Version GetPdfforgeAssemblyVersion()
        {
            var assembly = GetPdfforgeAssemblyFromStackTrace();

            return assembly?.GetName().Version;
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

        private Assembly GetPdfforgeAssemblyFromStackTrace()
        {
            var stackTrace = new StackTrace(); // get call stack
            var stackFrames = stackTrace.GetFrames(); // get method calls (frames)

            //reverse frames to start at application entry point
            var invertedFrames = stackFrames.Reverse();

            //skip stackframes that are not in pdfforge namespace and stackframes from this assembly
            var firstPdfforgeFrame = invertedFrames
                .SkipWhile(x => !IsInPdfforgeNamespace(x) || IsThisAssembly(x))
                .FirstOrDefault();

            if (firstPdfforgeFrame == null)
            {
                throw new InvalidOperationException("Could not find a pdfforge assembly in the call stack. The AssemblyHelper requires at least one class where namespace starts with pdfforge.");
            }

            var assembly = firstPdfforgeFrame.GetMethod().DeclaringType?.Assembly;
            return assembly;
        }

        private bool IsThisAssembly(StackFrame stackFrame)
        {
            return stackFrame.GetMethod().DeclaringType == GetType();
        }

        private bool IsInPdfforgeNamespace(StackFrame frame)
        {
            return frame.GetMethod().DeclaringType?.FullName.StartsWith("pdfforge") == true;
        }

    }
}