namespace pdfforge.PDFCreator.Conversion.Settings.GroupPolicies
{
    public interface IGpoSettings
    {
        bool DisableApplicationSettings { get; }

        bool DisableDebugTab { get; }

        bool DisablePrinterTab { get; }

        bool DisableProfileManagement { get; }

        bool DisableTitleTab { get; }

        bool HideLicenseTab { get; }

        bool HidePdfArchitectInfo { get; }

        string Language { get; }

        string UpdateInterval { get; }
    }
}
