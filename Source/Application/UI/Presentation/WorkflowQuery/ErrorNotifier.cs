using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.Core.Workflow.Queries;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.WorkflowQuery
{
    public class ErrorNotifierInteractive : IErrorNotifier
    {
        private readonly ITranslationFactory _translationFactory;
        private readonly IInteractionInvoker _interactionInvoker;
        private InteractiveWorkflowTranslation _translation;

        public ErrorNotifierInteractive(ITranslationFactory translationFactory, IInteractionInvoker interactionInvoker)
        {
            _translationFactory = translationFactory;
            _interactionInvoker = interactionInvoker;
            UpdateTranslation(translationFactory);
            translationFactory.TranslationChanged += (sender, args) => UpdateTranslation(translationFactory);
        }

        public void Notify(ActionResult actionResult)
        {
            var title = _translation.Error;

            var errorOccuredText = _translation.AnErrorOccured;
            var errorCodeInterpreter = new ErrorCodeInterpreter(_translationFactory);
            var errorText = errorCodeInterpreter.GetErrorText(actionResult[0], true);
            var message = $"{errorOccuredText}{System.Environment.NewLine}{errorText}";

            ShowMessage(message, title, MessageOptions.OK, MessageIcon.Error);
        }

        private void ShowMessage(string text, string title, MessageOptions buttons, MessageIcon icon)
        {
            var messageInteraction = new MessageInteraction(text, title, buttons, icon);
            _interactionInvoker.Invoke(messageInteraction);
        }

        private void UpdateTranslation(ITranslationFactory translationFactory)
        {
            _translation = translationFactory.CreateTranslation<InteractiveWorkflowTranslation>();
        }
    }
}
