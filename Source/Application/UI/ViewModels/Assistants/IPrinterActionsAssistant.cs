namespace pdfforge.PDFCreator.UI.ViewModels.Assistants
{
    public interface IPrinterActionsAssistant
    {
        bool AddPrinter(out string newPrinterName);
        bool DeletePrinter(string printerName, int numPrinters);
        bool RenamePrinter(string oldPrinterName, out string newPrinterName);
    }
}