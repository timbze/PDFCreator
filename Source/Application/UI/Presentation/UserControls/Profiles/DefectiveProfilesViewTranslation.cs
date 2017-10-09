using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles
{
    public class DefectiveProfilesViewTranslation : ITranslatable
    {
        public string DefectiveProfiles { get; private set; } = "Defective Profiles";
        public string Profile { get; private set; } = "Profile:";
        public string DefectiveProfileWarningText { get; private set; } = "The settings contain the following problems:";
        public string AskSaveSettings { get; private set; } = "Do you want to save anyway?";
        public string Cancel { get; private set; } = "_Cancel";
        public string Save { get; private set; } = "_Save";
    }
}
