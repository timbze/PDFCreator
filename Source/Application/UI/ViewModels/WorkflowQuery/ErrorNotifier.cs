using NLog;
using pdfforge.DynamicTranslator;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.Core.Workflow.Queries;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;

namespace pdfforge.PDFCreator.UI.ViewModels.WorkflowQuery
{
    public class ErrorNotifierInteractive : IErrorNotifier
    {
        private readonly IInteractionInvoker _interactionInvoker;
        private readonly ITranslator _translator;

        public ErrorNotifierInteractive(ITranslator translator, IInteractionInvoker interactionInvoker)
        {
            _translator = translator;
            _interactionInvoker = interactionInvoker;
        }

        public void Notify(ActionResult actionResult)
        {
            var title = _translator.GetTranslation("InteractiveWorkflow", "Error");

            var errorOccuredText = _translator.GetTranslation("InteractiveWorkflow", "AnErrorOccured");
            var errorCodeInterpreter = new ErrorCodeInterpreter(_translator);
            var errorText = errorCodeInterpreter.GetErrorText(actionResult[0], true);
            var message = $"{errorOccuredText}\r\n{errorText}";

            ShowMessage(message, title, MessageOptions.OK, MessageIcon.Error);
        }

        private void ShowMessage(string text, string title, MessageOptions buttons, MessageIcon icon)
        {
            var messageInteraction = new MessageInteraction(text, title, buttons, icon);
            _interactionInvoker.Invoke(messageInteraction);
        }
    }

    public class ErrorNotifierAutosave : IErrorNotifier
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public void Notify(ActionResult actionResult)
        {
            _logger.Error("An error occured during the  {0}", actionResult[0]);
        }
    }
}