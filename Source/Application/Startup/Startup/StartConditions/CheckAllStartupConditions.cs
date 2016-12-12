using System.Collections.Generic;
using NLog;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Core.StartupInterface;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;

namespace pdfforge.PDFCreator.Core.Startup.StartConditions
{
    public class CheckAllStartupConditions : ICheckAllStartupConditions
    {
        private readonly IInteractionInvoker _interactionInvoker;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IEnumerable<IStartupCondition> _startupConditions;

        public CheckAllStartupConditions(IList<IStartupCondition> startupConditions, IInteractionInvoker interactionInvoker)
        {
            _startupConditions = startupConditions;
            _interactionInvoker = interactionInvoker;
        }

        public void CheckAll()
        {
            foreach (var startupCondition in _startupConditions)
            {
                var result = startupCondition.Check();

                if (result.IsSuccessful)
                    continue;

                if (result.ShowMessage)
                    _interactionInvoker.Invoke(new MessageInteraction(result.Message, "PDFCreator", MessageOptions.OK, MessageIcon.Error));

                if (!string.IsNullOrWhiteSpace(result.Message))
                    _logger.Error(result.Message);

                throw new StartupConditionFailedException(result.ExitCode, result.Message);
            }
        }
    }
}
