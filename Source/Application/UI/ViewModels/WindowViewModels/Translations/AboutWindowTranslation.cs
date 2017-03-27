using Translatable;

namespace pdfforge.PDFCreator.UI.ViewModels.WindowViewModels.Translations
{
     public class AboutWindowTranslation : ITranslatable
     {
          public string LicenseButtonContent { get; private set; } = "License";
          public string LicenseInfoText { get; private set; } = "PDFCreator is free software consisting of multiple components with individual licenses. Please read the license section in the manual for further information on these licenses.";
          public string Title { get; private set; } = "About PDFCreator";
          public string UserManualButtonContent { get; private set; } = "User Manual";
     }
}
