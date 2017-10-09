using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Tabs.ConvertTabs
{
    public class ConvertTextTranslation : ITranslatable
    {
        public string TextSettingsHeader { get; private set; } = "Text Settings";

        public string TextFormatIntro { get; private set; } = "As the text format is very limited in what can be displayed, there are many ways to create a TXT file from a printed document. You can choose between four different strategies:";
        public string XmlUnicode { get; private set; } = "XML-escaped Unicode along with information regarding the format of the text";
        public string XmlUnicodeMuPdf { get; private set; } = "Same XML output format as above, but attempt processing similar to MuPDF";
        public string TextUnicode { get; private set; } = "Unicode (UCS2) text with byte order mark (BOM) which approximates the original text layout";
        public string TextUtf8 { get; private set; } = "UTF-8 text which approximates the original text layout";
    }
}
