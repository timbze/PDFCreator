using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles;
using System.Collections.Generic;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeDefectiveProfilesViewModel : DefectiveProfilesViewModel
    {
        public DesignTimeDefectiveProfilesViewModel() : base(new DesignTimeTranslationUpdater(), new ErrorCodeInterpreter(new TranslationFactory()))
        {
            ProfileErrors = new List<ProfileError>();
            ProfileErrors.Add(new ProfileError("Profile 1", "This is the first error message"));
            ProfileErrors.Add(new ProfileError("Profile 1", "This is the second error message"));
            ProfileErrors.Add(new ProfileError("Profile 2", "This is the first error message for the second profile"));
        }
    }
}
