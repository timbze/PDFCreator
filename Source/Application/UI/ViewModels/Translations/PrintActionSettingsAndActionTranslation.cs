using pdfforge.PDFCreator.Conversion.Settings.Enums;
using Translatable;

namespace pdfforge.PDFCreator.UI.ViewModels.Translations
{
    public class PrintActionSettingsAndActionTranslation : ITranslatable
    {
        public string Description { get; private set; } = "The print document action allows to print the document to a physical printer in addition to the conversion to PDF or any other format.";
        public string DisplayName { get; private set; } = "Print document";

        public string DuplexPrintingText { get; private set; } = "Duplex printing (if support by the printer)";
        public string SelectPrinterText { get; private set; } = "Select printer:";

        public EnumTranslation<DuplexPrint>[] DuplexPrintValues { get; set; } = EnumTranslation<DuplexPrint>.CreateDefaultEnumTranslation();
        public EnumTranslation<SelectPrinter>[] SelectPrinterValues { get; set; } = EnumTranslation<SelectPrinter>.CreateDefaultEnumTranslation();


    }
}
