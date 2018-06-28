namespace pdfforge.PDFCreator.Utilities.UnitTest.ArchitectCheck
{
    internal class PdfArchitectVersion
    {
        public PdfArchitectVersion(string subkeyName, string displayName, string installLocation, string exeName, bool isWow64, bool throwsException)
        {
            SubkeyName = subkeyName;
            DisplayName = displayName;
            InstallLocation = installLocation;
            ExeName = exeName;
            IsWow64 = isWow64;
            ThrowsException = throwsException;
        }

        public string SubkeyName { get; }
        public string DisplayName { get; }
        public string InstallLocation { get; }
        public string ExeName { get; }
        public bool IsWow64 { get; }
        public bool ThrowsException { get; }
    }
}
