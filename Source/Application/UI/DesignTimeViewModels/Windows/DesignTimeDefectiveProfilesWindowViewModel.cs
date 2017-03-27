using System.Collections.Generic;
using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.UI.ViewModels.WindowViewModels;
using pdfforge.PDFCreator.UI.ViewModels.WindowViewModels.Translations;
using Translatable;

namespace pdfforge.PDFCreator.UI.DesignTimeViewModels.Windows
{
    public class DesignTimeDefectiveProfilesWindowViewModel : DefectiveProfilesWindowViewModel
    {
        public DesignTimeDefectiveProfilesWindowViewModel() : base(new DefectiveProfilesWindowTranslation(), new ErrorCodeInterpreter(new TranslationFactory()))
        {
            ProfileErrors = new List<ProfileError>();
            ProfileErrors.Add(new ProfileError("Profile 1", "This is the first error message"));
            ProfileErrors.Add(new ProfileError("Profile 1", "This is the second error message"));
            ProfileErrors.Add(new ProfileError("Profile 2", "This is the first error message for the second profile"));
        }
    }
}