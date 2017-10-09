using pdfforge.PDFCreator.UI.Presentation.ServiceLocator;

namespace pdfforge.PDFCreator.Core.Controller
{
    public class ApplicationNameProvider : IWhitelisted
    {
        public ApplicationNameProvider(string editionName)
        {
            EditionName = editionName;
        }

        public string ApplicationName => "PDFCreator";
        public string EditionName { get; }
        public string ApplicationNameWithEdition => ApplicationName + " " + EditionName;
    }
}
