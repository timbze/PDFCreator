using System;

namespace pdfforge.PDFCreator.UI.ViewModels.Assistants.Update
{
    public class ApplicationVersion
    {
        public ApplicationVersion(Version version, string downloadUrl, string fileHash)
        {
            Version = version;
            DownloadUrl = downloadUrl;
            FileHash = fileHash;
        }

        public Version Version { get; }
        public string DownloadUrl { get; }
        public string FileHash { get; }
    }
}