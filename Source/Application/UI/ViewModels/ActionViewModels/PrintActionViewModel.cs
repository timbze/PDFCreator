using System.Collections.Generic;
using System.Linq;
using pdfforge.DynamicTranslator;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Core.Printing.Printer;
using pdfforge.PDFCreator.UI.ViewModels.Helper;

namespace pdfforge.PDFCreator.UI.ViewModels.ActionViewModels
{
    public class PrintActionViewModel : ActionViewModel
    {
        private readonly ISystemPrinterProvider _systemPrinterProvider;
        private bool _printerDialogOptionEnabled = true; 

        public PrintActionViewModel(ITranslator translator, ISystemPrinterProvider systemPrinterProvider)
        {
            _systemPrinterProvider = systemPrinterProvider;
            Translator = translator;

            DisplayName = Translator.GetTranslation("PrintActionSettings", "DisplayName");
            Description = Translator.GetTranslation("PrintActionSettings", "Description");

            SelectPrinterValues = Translator.GetEnumTranslation<SelectPrinter>();
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public bool PrinterDialogOptionEnabled
        {
            get { return _printerDialogOptionEnabled; }
            set
            {
                _printerDialogOptionEnabled = value;
                UpdatePrinterValues();
            }
        }


        public ITranslator Translator { get; }

        public override bool IsEnabled
        {
            get { return (CurrentProfile != null) && CurrentProfile.Printing.Enabled; }
            set
            {
                CurrentProfile.Printing.Enabled = value;
                RaisePropertyChanged(nameof(IsEnabled));
            }
        }

        public IEnumerable<string> InstalledPrinters => _systemPrinterProvider.GetInstalledPrinters();

        public IEnumerable<EnumValue<SelectPrinter>> SelectPrinterValues { get; set; }

        public IEnumerable<EnumValue<DuplexPrint>> DuplexPrintValues => Translator.GetEnumTranslation<DuplexPrint>();

        protected override void HandleCurrentProfileChanged()
        {
            if (PrinterDialogOptionEnabled)
                return;

            if (CurrentProfile.Printing.SelectPrinter != SelectPrinter.ShowDialog)
                return;

            CurrentProfile.Printing.SelectPrinter = SelectPrinter.DefaultPrinter;
            RaisePropertyChanged(nameof(CurrentProfile));
        }

        private void UpdatePrinterValues()
        {
            SelectPrinterValues = Translator.GetEnumTranslation<SelectPrinter>();
            if (!PrinterDialogOptionEnabled)
                SelectPrinterValues = SelectPrinterValues.Where(x => x.Value != SelectPrinter.ShowDialog);

            RaisePropertyChanged(nameof(SelectPrinterValues));
        }

        public override HelpTopic GetContextBasedHelpTopic()
        {
            return HelpTopic.PrintDocument;
        }
    }
}