using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using pdfforge.DynamicTranslator;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Core.Printing.Printer;
using pdfforge.PDFCreator.Core.Printing.Printing;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.Utilities;

namespace pdfforge.PDFCreator.UI.ViewModels.Assistants
{
    /// <summary>
    ///     The PrintFileAssistant extends the PrintFileHelperBase with user interaction (ask to change default printer, show
    ///     responses for invalid files).
    /// </summary>
    public class PrintFileAssistant : PrintFileHelperBase
    {
        private readonly IInteractionInvoker _interactionInvoker;
        private readonly ITranslator _translator;

        public PrintFileAssistant(IInteractionInvoker interactionInvoker, IPrinterHelper printerHelper, ISettingsProvider settingsProvider, ITranslator translator, IFileAssoc fileAssoc)
            : base(printerHelper, settingsProvider, fileAssoc)
        {
            _interactionInvoker = interactionInvoker;
            _translator = translator;
        }

        protected override void DirectoriesNotSupportedHint()
        {
            const string caption = "PDFCreator";
            var message = _translator.GetTranslation("PrintFiles", "DirectoriesNotSupported");
            ShowMessage(message, caption, MessageOptions.OK, MessageIcon.Warning);
        }

        private MessageResponse ShowMessage(string message, string title, MessageOptions buttons, MessageIcon icon)
        {
            var interaction = new MessageInteraction(message, title, buttons, icon);
            _interactionInvoker.Invoke(interaction);
            return interaction.Response;
        }

        protected override bool UnprintableFilesQuery(IList<PrintCommand> unprintable)
        {
            var fileList =
                new List<string>(unprintable.Select(p => Path.GetFileName(p.Filename)).Take(Math.Min(3, unprintable.Count)));
            const string caption = "PDFCreator";
            var message =
                _translator.GetTranslation("PrintFiles", "NotPrintableFiles") +
                "\r\n";

            message += string.Join("\r\n", fileList.ToArray());

            if (fileList.Count < unprintable.Count)
                message += "\r\n" + _translator.GetFormattedTranslation("PrintFiles", "AndXMore", unprintable.Count - fileList.Count);

            message += "\r\n\r\n" + _translator.GetTranslation("PrintFiles", "ProceedAnyway");

            var result = ShowMessage(message, caption, MessageOptions.YesNo, MessageIcon.Warning);

            return result == MessageResponse.Yes;
        }

        protected override bool QuerySwitchDefaultPrinter()
        {
            var message =
                _translator.GetTranslation("PrintFileHelper", "AskSwitchDefaultPrinter");
            const string caption = "PDFCreator";

            var response = ShowMessage(message, caption, MessageOptions.YesNo, MessageIcon.Question);

            return response == MessageResponse.Yes;
        }
    }
}