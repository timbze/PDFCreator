using pdfforge.DynamicTranslator;
using pdfforge.PDFCreator.Shared.Helper;
using pdfforge.PDFCreator.Shared.ViewModels;
using pdfforge.PDFCreator.Shared.Views;

namespace pdfforge.PDFCreator.Shared.Assistants
{
    public class PrinterActionsAssistant
    {
        private Translator _translator = TranslationHelper.Instance.TranslatorInstance;

        public bool AddPrinter(out string newPrinterName)
        {
            newPrinterName = CreateValidPrinterName("PDFCreator");
            string questionText = _translator.GetTranslation("InputBoxWindow", "EnterPrintername",
                "Please enter printer name:");
            newPrinterName = RequestPrinternameFromUser(questionText, newPrinterName);
            if (newPrinterName == null)
                return false;

            var printerHelper = new PrinterHelper();
            while (!printerHelper.IsValidPrinterName(newPrinterName))
            {
                questionText = _translator.GetFormattedTranslation("ApplicationSettingsWindow", "PrinterAlreadyInstalled",
                    "A printer with the name '{0}' is already installed on your system. Please enter a new printer name:",
                    newPrinterName);
                newPrinterName = CreateValidPrinterName(newPrinterName);
                newPrinterName = RequestPrinternameFromUser(questionText, newPrinterName);
                if (newPrinterName == null)
                    return false;
            }

            var uac = new UacAssistant();
            return uac.AddPrinter(newPrinterName);
        }

        public bool RenamePrinter(string oldPrinterName, out string newPrinterName)
        {
            newPrinterName = "";

            if (oldPrinterName.Length == 0)
                return false;

            string questionText = _translator.GetTranslation("InputBoxWindow", "EnterPrintername",
                "Please enter printer name:");
            newPrinterName = RequestPrinternameFromUser(questionText, oldPrinterName);

            if ((newPrinterName == null) || (newPrinterName == oldPrinterName))
                return false;

            var printerHelper = new PrinterHelper();
            while (!printerHelper.IsValidPrinterName(newPrinterName))
            {
                questionText = _translator.GetFormattedTranslation("ApplicationSettingsWindow", "PrinterAlreadyInstalled",
                    "A printer with the name '{0}' is already installed on your system. Please enter a new printer name:",
                    newPrinterName);
                newPrinterName = CreateValidPrinterName(newPrinterName);
                newPrinterName = RequestPrinternameFromUser(questionText, newPrinterName);
                if ((newPrinterName == null) || (newPrinterName == oldPrinterName))
                    return false;
            }

            var uac = new UacAssistant();
            return uac.RenamePrinter(oldPrinterName, newPrinterName);
        }

        public bool DeletePrinter(string printerName, int numPrinters)
        {
            if (numPrinters < 2)
            {
                var message = _translator.GetTranslation("ApplicationSettingsWindow", "DontDeleteLastPrinter",
                    "You may not delete the last printer. Uninstall PDFCreator if you really want to remove all related printers.");
                const string caption = @"PDFCreator";
                MessageWindow.ShowTopMost(message, caption, MessageWindowButtons.OK, MessageWindowIcon.Error);
                return false;
            }

            var msg = _translator.GetFormattedTranslation("ApplicationSettingsWindow", "AskDeletePrinter",
                "Are you sure that you want to delete the printer '{0}'?", printerName);
            var cpt = _translator.GetTranslation("ApplicationSettingsWindow", "DeletePrinter", "Delete Printer");
            if (MessageWindow.ShowTopMost(msg, cpt, MessageWindowButtons.YesNo, MessageWindowIcon.Question) != MessageWindowResponse.Yes)
                return false;

            var uac = new UacAssistant();
            return uac.DeletePrinter(printerName);
        }

        private string RequestPrinternameFromUser(string questionText, string printerName)
        {
            var w = new InputBoxWindow();
            w.IsValidInput = ValidatePrinterName;
            w.QuestionText = questionText;
            w.InputText = printerName;

            if (w.ShowDialog() != true)
                return null;

            return w.InputText;
        }

        private InputBoxValidation ValidatePrinterName(string arg)
        {
            var printerHelper = new PrinterHelper();
            if (printerHelper.IsValidPrinterName(arg))
                return new InputBoxValidation(true, "");

            return new InputBoxValidation(false, _translator.GetTranslation("ApplicationSettingsWindow", "InvalidPrinterName",
                    "The name is invalid or a printer with this name already exists"));
        }

        private string CreateValidPrinterName(string baseName)
        {
            int i = 2;
            string printerName = baseName;

            var printerHelper = new PrinterHelper();
            while (!printerHelper.IsValidPrinterName(printerName))
            {
                printerName = baseName + i++;
            }

            return printerName;
        }
    }
}
