using Translatable;

namespace pdfforge.PDFCreator.UI.ViewModels.WindowViewModels.Translations
{
     public class DefectiveProfilesWindowTranslation : ITranslatable
     {
          public string DefectiveProfile { get; private set; } = "Defective profile:";
          public string DefectiveProfiles { get; private set; } = "Defective Profiles";
          public string Profile { get; private set; } = "Profile:";
          public string DefectiveProfileWarningText { get; private set; } = "Do you want to return and edit the settings or\r\nignore the error and save anyway?";
          public string Edit { get; private set; } = "_Edit";
          public string Ignore { get; private set; } = "_Ignore";
     }
}
