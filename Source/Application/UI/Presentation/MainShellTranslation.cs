using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation
{
    public class MainShellTranslation : ITranslatable
    {
        public string Home { get; private set; } = "Home";
        public string Profiles { get; private set; } = "Profiles";
        public string Printer { get; private set; } = "Printer";
        public string Accounts { get; private set; } = "Accounts";
        public string UpdateToolTip { get; private set; } = "A new Update is available";
    }
}
