namespace pdfforge.PDFCreator.Utilities.UnitTest.ArchitectCheck
{
    internal class PdfArchitectVersion
    {
        public PdfArchitectVersion(string subkeyName, string displayName, string installLocation, string exeName, bool isWow64)
        {
            SubkeyName = subkeyName;
            DisplayName = displayName;
            InstallLocation = installLocation;
            ExeName = exeName;
            IsWow64 = isWow64;
        }

        public string SubkeyName { get; set; }
        public string DisplayName { get; set; }
        public string InstallLocation { get; set; }
        public string ExeName { get; set; }
        public bool IsWow64 { get; set; }
    }
}