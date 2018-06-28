using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls
{
    public class AboutViewTranslation : ITranslatable
    {
        public string License { get; private set; } = "License";
        public string LicenseInfo { get; private set; } = "PDFCreator is free software consisting of multiple components with individual licenses.";
        public string PleaseReadLicenseSection { get; private set; } = "Please read the license section in the manual for further information on these licenses.";
        public string UserManual { get; private set; } = "User Manual";
        public string Version { get; private set; } = "Version";
        public string PrioritySupport { get; private set; } = "Priority Support";
    }
}
