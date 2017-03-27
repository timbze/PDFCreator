using System.Collections.Generic;
using System.Linq;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Core.Printing.Printer;
using pdfforge.PDFCreator.UI.ViewModels.Helper;
using pdfforge.PDFCreator.UI.ViewModels.Translations;
using Translatable;

namespace pdfforge.PDFCreator.UI.ViewModels.ActionViewModels
{
    public class PrintActionViewModel : ActionViewModel
    {
        private readonly ISystemPrinterProvider _systemPrinterProvider;
        private bool _printerDialogOptionEnabled = true; 

        public PrintActionViewModel(PrintActionSettingsAndActionTranslation translation, ISystemPrinterProvider systemPrinterProvider)
        {
            _systemPrinterProvider = systemPrinterProvider;
            Translation = translation;

            DisplayName = Translation.DisplayName;
            Description = Translation.Description;
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


        public PrintActionSettingsAndActionTranslation Translation { get; }

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
            if (!PrinterDialogOptionEnabled)
                Translation.SelectPrinterValues = (EnumTranslation<SelectPrinter>[]) Translation.SelectPrinterValues.Where(x => x.Value != SelectPrinter.ShowDialog);

            RaisePropertyChanged(nameof(Translation.SelectPrinterValues));
        }

        public override HelpTopic GetContextBasedHelpTopic()
        {
            return HelpTopic.PrintDocument;
        }
    }
}