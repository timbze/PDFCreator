using SystemInterface.Diagnostics;
using SystemWrapper.Diagnostics;

namespace pdfforge.PDFCreator.Utilities.Process
{
    public interface IProcessStarter
    {
        IProcess Start(string fileName);
        IProcess Start(string fileName, string arguments);
        bool Start(IProcessStartInfo startInfo);
        IProcess CreateProcess(string fileName);
    }

    public class ProcessStarter : IProcessStarter
    {
        public IProcess Start(string fileName)
        {
            var process = new ProcessWrap();
            process.Start(fileName);

            return process;
        }

        public IProcess Start(string fileName, string arguments)
        {
            var process = new ProcessWrap();
            process.Start(fileName, arguments);

            return process;
        }

        public bool Start(IProcessStartInfo startInfo)
        {
            var process = new ProcessWrap();
            process.StartInfo = startInfo;
            return process.Start();
        }

        public IProcess CreateProcess(string fileName)
        {
            var process = new ProcessWrap();
            process.StartInfo.FileName = fileName;

            return process;
        }
    }
}
