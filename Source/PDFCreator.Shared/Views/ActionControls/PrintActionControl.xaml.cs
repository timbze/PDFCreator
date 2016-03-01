using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using pdfforge.DynamicTranslator;
using pdfforge.PDFCreator.Core.Settings.Enums;
using pdfforge.PDFCreator.Shared.Helper;

namespace pdfforge.PDFCreator.Shared.Views.ActionControls
{
    public partial class PrintActionControl : ActionControl
    {
        public PrintActionControl()
        {
            InitializeComponent();

            DisplayName = TranslationHelper.Instance.TranslatorInstance.GetTranslation("PrintActionSettings", "DisplayName", "Print document");
            Description = TranslationHelper.Instance.TranslatorInstance.GetTranslation("PrintActionSettings", "Description", "The print document action allows to print the document to a physical printer in addition to the conversion to PDF or any other format.");

            TranslationHelper.Instance.TranslatorInstance.Translate(this);
        }

        public static IEnumerable<EnumValue<SelectPrinter>> SelectPrinterValues
        {
            get { return TranslationHelper.Instance.TranslatorInstance.GetEnumTranslation<SelectPrinter>(); }
        }

        public static IEnumerable<EnumValue<DuplexPrint>> DuplexPrintValues
        {
            get { return TranslationHelper.Instance.TranslatorInstance.GetEnumTranslation<DuplexPrint>(); }
        } 

        public override bool IsActionEnabled
        {
            get
            {
                if (CurrentProfile == null)
                    return false;
                return CurrentProfile.Printing.Enabled;
            }
            set { CurrentProfile.Printing.Enabled = value; }
        }

        public IEnumerable<string> InstalledPrinters
        {
            get
            {
                var printers = PrinterSettings.InstalledPrinters.Cast<string>().ToList();
                printers.Sort();
                return printers;
            }
        }
    }
}
