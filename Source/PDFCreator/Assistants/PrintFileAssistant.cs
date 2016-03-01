using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using pdfforge.PDFCreator.Helper;
using pdfforge.PDFCreator.PrintFile;
using pdfforge.PDFCreator.Shared.Helper;
using pdfforge.PDFCreator.Shared.ViewModels;
using pdfforge.PDFCreator.Shared.Views;

namespace pdfforge.PDFCreator.Assistants
{
    /// <summary>
    /// The PrintFileAssistant extends the PrintFileHelperBase with user interaction (ask to change default printer, show responses for invalid files).
    /// </summary>
    internal class PrintFileAssistant : PrintFileHelperBase
    {
        private readonly TranslationHelper _translationHelper = TranslationHelper.Instance;

        public PrintFileAssistant()
        {

        }

        public PrintFileAssistant(string printerName)
            : base(printerName)
        {

        }

        protected override void DirectoriesNotSupportedHint()
        {
            const string caption = "PDFCreator";
            string message = _translationHelper.TranslatorInstance.GetTranslation("PrintFiles", "DirectoriesNotSupported", "You have tried to convert directories here. This is currently not supported.");
            MessageWindow.ShowTopMost(message, caption, MessageWindowButtons.OK, MessageWindowIcon.Warning);
        }

        protected override void UnprintableFilesHint(IList<PrintCommand> unprintable)
        {
            var fileList =
                new List<string>(unprintable.Select(p => Path.GetFileName(p.Filename)).Take(Math.Min(3, unprintable.Count)));
            const string caption = "PDFCreator";
            string message =
                _translationHelper.TranslatorInstance.GetTranslation("PrintFiles", "NotPrintableFiles", "The following files can't be converted:") +
                "\r\n";

            message += string.Join("\r\n", fileList.ToArray());

            if (fileList.Count < unprintable.Count)
                message += "\r\n" + _translationHelper.TranslatorInstance.GetFormattedTranslation("PrintFiles", "AndXMore", "and {0} more.",
                    unprintable.Count - fileList.Count);

            MessageWindow.ShowTopMost(message, caption, MessageWindowButtons.OK, MessageWindowIcon.Warning);
        }

        protected override bool QuerySwitchDefaultPrinter()
        {
            var message = 
               _translationHelper.TranslatorInstance.GetTranslation("PrintFileHelper", "AskSwitchDefaultPrinter",
                            "PDFCreator needs to temporarily change the default printer to be able to convert the file. Do you want to proceed?");
            const string caption = "PDFCreator";
            return (MessageWindowResponse.Yes ==
                    MessageWindow.ShowTopMost(message, caption, MessageWindowButtons.YesNo, MessageWindowIcon.Question));
        }
    }
}
