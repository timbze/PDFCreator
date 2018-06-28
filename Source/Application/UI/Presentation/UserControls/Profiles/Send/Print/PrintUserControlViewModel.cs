using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Core.Printing.Printer;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Send.Print
{
    public class PrintUserControlViewModel : ProfileUserControlViewModel<PrintTabTranslation>
    {
        public PrintUserControlViewModel(ISystemPrinterProvider systemPrinterProvider, ITranslationUpdater translationUpdater, ISelectedProfileProvider profile, IDispatcher dispatcher) : base(translationUpdater, profile, dispatcher)
        {
            _systemPrinterProvider = systemPrinterProvider;
        }

        private readonly ISystemPrinterProvider _systemPrinterProvider;
        private bool _printerDialogOptionEnabled = true;

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

        public IEnumerable<string> InstalledPrinters => _systemPrinterProvider.GetInstalledPrinters();

        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);
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
                Translation.SelectPrinterValues = (EnumTranslation<SelectPrinter>[])Translation.SelectPrinterValues.Where(x => x.Value != SelectPrinter.ShowDialog);

            RaisePropertyChanged(nameof(Translation.SelectPrinterValues));
        }
    }

    public class DesignTimePrintUserControlViewModel : PrintUserControlViewModel
    {
        public DesignTimePrintUserControlViewModel() : base(null, new DesignTimeTranslationUpdater(), new DesignTimeCurrentSettingsProvider(), null)
        {
        }
    }
}
