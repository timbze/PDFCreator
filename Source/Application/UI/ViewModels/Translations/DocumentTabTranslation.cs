using Translatable;

namespace pdfforge.PDFCreator.UI.ViewModels.Translations
{
    public class DocumentTabTranslation : ITranslatable
    {
        public string AuthorLabelContent { get; private set; } = "Author:";
        public string AuthorPreviewLabelContent { get; private set; } = "Preview:";
        public string AuthorTokenLabelContent { get; private set; } = "Add Token:";
        public string KeywordLabelContent { get; private set; } = "Keywords:";
        public string KeywordPreviewLabelContent { get; private set; } = "Preview:";
        public string KeywordTokenLabelContent { get; private set; } = "Add Token:";
        public string MetadataControlHeader { get; private set; } = "Metadata";
        public string StampCheckBoxContent { get; private set; } = "Add a stamp text on top of all pages";
        public string StampControlHeader { get; private set; } = "Stamp";
        public string StampFontAsOutlineCheckBoxContent { get; private set; } = "Show Font as outline";
        public string StampFontColorLabelContent { get; private set; } = "Font Color:";
        public string StampFontLabelContent { get; private set; } = "Font:";
        public string StampOutlineWidthLabelContent { get; private set; } = "Outline Width:";
        public string StampTextLabelContent { get; private set; } = "Stamp Text:";
        public string SubjectLabelContent { get; private set; } = "Subject:";
        public string SubjectPreviewLabelContent { get; private set; } = "Preview:";
        public string SubjectTokenLabelContent { get; private set; } = "Add Token:";
        public string TitleLabelContent { get; private set; } = "Title:";
        public string TitlePreviewLabelContent { get; private set; } = "Preview:";
        public string TitleTokenLabelContent { get; private set; } = "Add Token:";
        public string FontFileNotSupported { get; private set; } = "The selected font is not supported. Please select a different font.";

    }
}
