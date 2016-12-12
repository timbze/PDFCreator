using NLog;
using pdfforge.DynamicTranslator;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.Services.Licensing;
using pdfforge.PDFCreator.Core.StartupInterface;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.Utilities.Process;

namespace pdfforge.PDFCreator.Core.Startup.StartConditions
{
    public class TerminalServerNotAllowedCondition : IStartupCondition
    {
        private readonly IInteractionInvoker _interactionInvoker;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IProcessStarter _processStarter;
        private readonly ApplicationNameProvider _applicationNameProvider;
        private readonly ITerminalServerDetection _terminalServerDetection;
        private readonly ITranslator _translator;

        public TerminalServerNotAllowedCondition(ITerminalServerDetection terminalServerDetection, ITranslator translator,
            IInteractionInvoker interactionInvoker, IProcessStarter processStarter, ApplicationNameProvider applicationNameProvider)
        {
            _terminalServerDetection = terminalServerDetection;
            _translator = translator;
            _interactionInvoker = interactionInvoker;
            _processStarter = processStarter;
            _applicationNameProvider = applicationNameProvider;
        }

        public StartupConditionResult Check()
        {
            if (!_terminalServerDetection.IsTerminalServer())
                return StartupConditionResult.BuildSuccess();

            var errorMessage = $"It is not possible to run {_applicationNameProvider.ApplicationName} with installed Terminal Services. Please use the Terminal Server Edition instead.";
            _logger.Error(errorMessage);
            var caption = _applicationNameProvider.ApplicationName;
            var message = _translator.GetTranslation("Program", "UsePDFCreatorTerminalServer");
            var result = ShowMessage(message, caption, MessageOptions.MoreInfoCancel, MessageIcon.Exclamation);
            if (result == MessageResponse.MoreInfo)
                _processStarter.Start(Urls.PdfCreatorTerminalServerUrl);

            return StartupConditionResult.BuildErrorWithMessage((int)ExitCode.NotValidOnTerminalServer, errorMessage, showMessage:false);
        }

        private MessageResponse ShowMessage(string message, string title, MessageOptions options, MessageIcon icon)
        {
            var interaction = new MessageInteraction(message, title, options, icon);
            _interactionInvoker.Invoke(interaction);
            return interaction.Response;
        }
    }
}
