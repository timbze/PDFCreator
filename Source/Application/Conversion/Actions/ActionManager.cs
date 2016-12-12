using System.Collections.Generic;
using NLog;
using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;

namespace pdfforge.PDFCreator.Conversion.Actions
{
    public class ActionManager : IActionManager
    {
        private readonly IEnumerable<IAction> _availableActions;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public ActionManager(IList<IAction> availableActions)
        {
            _availableActions = availableActions;
        }

        public IEnumerable<IAction> GetAllApplicableActions(Job job)
        {
            var enabledActions = new List<IAction>();

            foreach (var action in _availableActions)
            {
                if (!action.IsEnabled(job.Profile))
                    continue;

                if (!action.Init(job))
                    continue;

                enabledActions.Add(action);
                _logger.Trace($"added action '{action.GetType().FullName}'");
            }

            return enabledActions;
        }
    }
}