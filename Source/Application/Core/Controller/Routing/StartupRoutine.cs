using System.Collections.Generic;

namespace pdfforge.PDFCreator.Core.Controller.Routing
{
    public class StartupRoutine
    {
        private List<StartupAction> _startupActions = new List<StartupAction>();

        public StartupRoutine()
        {
        }

        public void AddAction(StartupAction action)
        {
            _startupActions.Add(action);
        }

        public void OverrideRoutine(StartupRoutine routine)
        {
            _startupActions = new List<StartupAction>();
            for (int i = 0; i < routine._startupActions.Count; i++)
            {
                _startupActions.Add(routine._startupActions[i]);
            }
        }

        public List<TType> GetActionByType<TType>() where TType : StartupAction
        {
            var actionByType = new List<TType>();
            foreach (var startupRoutine in _startupActions)
            {
                if (startupRoutine is TType)
                {
                    actionByType.Add(startupRoutine as TType);
                }
            }
            return actionByType;
        }
    }
}
