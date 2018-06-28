using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob.QuickActionStep
{
    public class QuickActionListItemVo
    {
        public string Name { get; }
        public ICommand Command { get; }
        public ICommand Trigger { get; }

        public QuickActionListItemVo(string name, ICommand command, ICommand trigger)
        {
            Name = name;
            Command = command;
            Trigger = trigger;
        }
    }
}
