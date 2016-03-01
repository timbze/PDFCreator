using System.Windows;
using System.Windows.Controls;
using pdfforge.PDFCreator.Shared.Assistants;
using pdfforge.PDFCreator.Shared.Helper;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.ViewModels.UserControls;
using pdfforge.PDFCreator.ViewModels.Wrapper;

namespace pdfforge.PDFCreator.Views.UserControls
{
    internal partial class PrinterTab : UserControl
    {
        private static readonly TranslationHelper TranslationHelper = TranslationHelper.Instance;
        private Shared.Helper.PrinterHelper _printerHelper = new Shared.Helper.PrinterHelper();

        public PrinterTab()
        {
            InitializeComponent();
            if (TranslationHelper.IsInitialized)
            {
                TranslationHelper.TranslatorInstance.Translate(this);
            }
            ViewModel.AddPrinterAction = AddPrinterAction;
            ViewModel.RenamePrinterAction = RenamePrinterAction;
            ViewModel.DeletePrinterAction = DeletePrinterAction;
        }

        public PrinterTabViewModel ViewModel
        {
            get { return (PrinterTabViewModel) DataContext; }
            set { DataContext = value; }
        }

        private void PrimaryPrinterBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ViewModel == null)
                return;

            ViewModel.UpdatePrimaryPrinter(ViewModel.ApplicationSettings.PrimaryPrinter);
        }

        private void DataGrid_OnAutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            e.Cancel = true;
        }

        private string AddPrinterAction()
        {
            var helper = new PrinterActionsAssistant();

            string printerName;
            helper.AddPrinter(out printerName);

            return printerName;
        }

        private void RenamePrinterAction(PrinterMappingWrapper printerMapping)
        {
            var helper = new PrinterActionsAssistant();
            var wasPrimaryPrinter = printerMapping.IsPrimaryPrinter;
            string newPrinterName;
            var success = helper.RenamePrinter(printerMapping.PrinterName, out newPrinterName);

            if (success)
            {
                ViewModel.PdfCreatorPrinters = _printerHelper.GetPDFCreatorPrinters();
                printerMapping.PrinterName = newPrinterName;
                if (wasPrimaryPrinter)
                    ViewModel.PrimaryPrinter = newPrinterName;
            }
        }

        private void DeletePrinterAction(PrinterMappingWrapper printerMapping)
        {
            var helper = new PrinterActionsAssistant();
            var success = helper.DeletePrinter(printerMapping.PrinterName, ViewModel.PdfCreatorPrinters.Count);

            if (success)
            {
                ViewModel.PrinterMappings.Remove(printerMapping);
                ViewModel.PdfCreatorPrinters = _printerHelper.GetPDFCreatorPrinters();
                PrimaryPrinterBox.SelectedValue = ViewModel.PrimaryPrinter;
            }
        }

        public Visibility RequiresUacVisibility
        {
            get { return new OsHelper().UserIsAdministrator() ? Visibility.Collapsed : Visibility.Visible; }
        }

        public void UpdateProfilesList()
        {
            ViewModel.RefreshPrinterMappings();
        }
    }
}