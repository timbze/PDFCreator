using System.Runtime.InteropServices;
using System.Text;

namespace pdfforge.PDFCreator.Core
{
    public class PathHelper
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern int GetShortPathName(
                 [MarshalAs(UnmanagedType.LPTStr)]
                   string path,
                 [MarshalAs(UnmanagedType.LPTStr)]
                   StringBuilder shortPath,
                 int bufferSize
                 );

        public static string GetShortPathName(string path)
        {
            StringBuilder buffer = new StringBuilder(256);
            int result = GetShortPathName(path, buffer, buffer.Capacity);

            if (result == 0)
                return path;

            return buffer.ToString();
        }
    }
}
