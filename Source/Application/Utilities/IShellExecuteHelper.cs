namespace pdfforge.PDFCreator.Utilities
{
    public interface IShellExecuteHelper
    {
        ShellExecuteResult RunAsAdmin(string path, string arguments);
    }
}
