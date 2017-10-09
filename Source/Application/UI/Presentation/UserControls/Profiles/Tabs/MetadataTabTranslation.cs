using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Tabs
{
    public class MetadataTabTranslation : ITranslatable
    {
        public string Title { get; private set; } = "Metadata";
        public string TitleTemplateText { get; private set; } = "Title:";
        public string AuthorTemplateText { get; private set; } = "Author:";
        public string SubjectTemplateTextle { get; private set; } = "Subject:";
        public string KeywordsTemplateText { get; private set; } = "Keywords:";
    }
}
