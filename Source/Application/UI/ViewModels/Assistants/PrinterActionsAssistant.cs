using System.Drawing.Printing;
using System.Linq;
using pdfforge.DynamicTranslator;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Core.Printing.Printer;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;

namespace pdfforge.PDFCreator.UI.ViewModels.Assistants
{
    public class PrinterActionsAssistant : IPrinterActionsAssistant
    {
        private readonly IInteractionInvoker _invoker;
        private readonly IPrinterHelper _printerHelper;

        private readonly ITranslator _translator;
        private readonly IUacAssistant _uacAssistant;

        public PrinterActionsAssistant(ITranslator translator, IInteractionInvoker invoker, IPrinterHelper printerHelper, IUacAssistant uacAssistant)
        {
            _translator = translator;
            _invoker = invoker;
            _printerHelper = printerHelper;
            _uacAssistant = uacAssistant;
        }

        public bool AddPrinter(out string newPrinterName)
        {
            newPrinterName = CreateValidPrinterName("PDFCreator");
            var questionText = _translator.GetTranslation("InputBoxWindow", "EnterPrintername");
            newPrinterName = RequestPrinternameFromUser(questionText, newPrinterName);
            if (newPrinterName == null)
                return false;

            while (!_printerHelper.IsValidPrinterName(newPrinterName))
            {
                questionText = _translator.GetFormattedTranslation("ApplicationSettingsWindow", "PrinterAlreadyInstalled",
                    newPrinterName);
                newPrinterName = CreateValidPrinterName(newPrinterName);
                newPrinterName = RequestPrinternameFromUser(questionText, newPrinterName);
                if (newPrinterName == null)
                    return false;
            }

            if (_uacAssistant.AddPrinter(newPrinterName))
                if (CheckInstalledPrinter(newPrinterName))
                    return true;

            const string caption = "PDFCreator";
            var message = _translator.GetFormattedTranslation("ApplicationSettingsWindow", "CouldNotInstallPrinter",
                newPrinterName);
            ShowMessage(message, caption, MessageOptions.OK, MessageIcon.Error);

            return false;
        }

        public bool RenamePrinter(string oldPrinterName, out string newPrinterName)
        {
            newPrinterName = "";

            if (oldPrinterName.Length == 0)
                return false;

            var questionText = _translator.GetTranslation("InputBoxWindow", "EnterPrintername");
            newPrinterName = RequestPrinternameFromUser(questionText, oldPrinterName);

            if ((newPrinterName == null) || (newPrinterName == oldPrinterName))
                return false;

            while (!_printerHelper.IsValidPrinterName(newPrinterName))
            {
                questionText = _translator.GetFormattedTranslation("ApplicationSettingsWindow", "PrinterAlreadyInstalled",
                    newPrinterName);
                newPrinterName = CreateValidPrinterName(newPrinterName);
                newPrinterName = RequestPrinternameFromUser(questionText, newPrinterName);
                if ((newPrinterName == null) || (newPrinterName == oldPrinterName))
                    return false;
            }

            if (_uacAssistant.RenamePrinter(oldPrinterName, newPrinterName))
                if (CheckInstalledPrinter(newPrinterName))
                    return true;

            var message = _translator.GetFormattedTranslation("ApplicationSettingsWindow", "CouldNotRename",
                oldPrinterName, newPrinterName);
            const string caption = @"PDFCreator";
            ShowMessage(message, caption, MessageOptions.OK, MessageIcon.Error);

            return false;
        }

        public bool DeletePrinter(string printerName, int numPrinters)
        {
            if (numPrinters < 2)
            {
                var message = _translator.GetTranslation("ApplicationSettingsWindow", "DontDeleteLastPrinter");
                const string caption = @"PDFCreator";
                ShowMessage(message, caption, MessageOptions.OK, MessageIcon.Error);
                return false;
            }

            var msg = _translator.GetFormattedTranslation("ApplicationSettingsWindow", "AskDeletePrinter",
                printerName);
            var cpt = _translator.GetTranslation("ApplicationSettingsWindow", "DeletePrinter");

            var resonse = ShowMessage(msg, cpt, MessageOptions.YesNo, MessageIcon.Question);

            if (resonse != MessageResponse.Yes)
                return false;

            return _uacAssistant.DeletePrinter(printerName);
        }

        private MessageResponse ShowMessage(string message, string title, MessageOptions buttons, MessageIcon icon)
        {
            var interaction = new MessageInteraction(message, title, buttons, icon);
            _invoker.Invoke(interaction);
            return interaction.Response;
        }

        private bool CheckInstalledPrinter(string printerName)
        {
            var ps = new PrinterSettings();
            ps.PrinterName = printerName;
            return ps.IsValid;
        }

        private string RequestPrinternameFromUser(string questionText, string printerName)
        {
            var interaction = new InputInteraction("PDFCreator", questionText, ValidatePrinterName);
            interaction.InputText = printerName;

            _invoker.Invoke(interaction);

            if (!interaction.Success)
                return null;

            return interaction.InputText.Trim();
        }

        private InputValidation ValidatePrinterName(string arg)
        {
            var invalidChars = new[] {"!", @"\", ",", "\""}; //\" would be valid but causes problems, since it splits input strings
            if (invalidChars.Any(arg.Contains))
                return new InputValidation(false, _translator.GetTranslation("ApplicationSettingsWindow", "InvalidCharsInPrinterName"));

            if (_printerHelper.IsValidPrinterName(arg.Trim()))
                return new InputValidation(true, "");

            return new InputValidation(false, _translator.GetTranslation("ApplicationSettingsWindow", "InvalidPrinterName"));
        }

        private string CreateValidPrinterName(string baseName)
        {
            var i = 2;
            var printerName = baseName;

            while (!_printerHelper.IsValidPrinterName(printerName))
            {
                printerName = baseName + i++;
            }

            return printerName;
        }
    }
}