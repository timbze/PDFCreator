using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.ViewModels.ActionViewModels
{
    public class ActionControl : UserControl
    {
        protected ActionControl(ActionViewModel vm)
        {
            if (vm != null) // this check is necessary because the designer passes null into the contructor
                vm.Content = this; // this set here so this control can be displayed by binding to the Content property

            DataContext = vm;
            ActionViewModel = vm;
        }

        public ActionViewModel ActionViewModel { get; private set; }
    }
}