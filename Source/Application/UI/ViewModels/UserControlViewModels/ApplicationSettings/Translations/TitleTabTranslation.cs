using pdfforge.PDFCreator.Conversion.Settings.Enums;
using Translatable;

namespace pdfforge.PDFCreator.UI.ViewModels.UserControlViewModels.ApplicationSettings.Translations
{
    public class TitleTabTranslation : ITranslatable
    {
        public string PreviewTitleText { get; private set; } = "Replaced Title:";
        public string ReplaceColumnHeader { get; private set; } = "Replace with";
        public string SampleTitleText { get; private set; } = "Sample Title:";
        public string SearchColumnHeader { get; private set; } = "Search text";
        public string TitleReplacementControlHeader { get; private set; } = "Title Replacement";
        public string TitleReplacementPreviewHeader { get; private set; } = "Preview";
        public string TypeColoumnHeader { get; private set; } = "Type";
        public EnumTranslation<ReplacementType>[] ReplacementValues { get; private set; } = EnumTranslation<ReplacementType>.CreateDefaultEnumTranslation();
    }
}
