using System.ComponentModel;
using pdfforge.PDFCreator.Shared.Annotations;

namespace pdfforge.PDFCreator.Shared.Views.ActionControls
{
    public class ActionBundle : INotifyPropertyChanged
    {
        private readonly ActionControl _actionControl;
        public bool IsEnabled
        {
            get { return _actionControl.IsActionEnabled; }
            set { _actionControl.IsActionEnabled = value; RaiseIsEnabledChanged(); }
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public ActionControl ActionControl { get; set; }

        public ActionBundle(ActionControl actionControl)
        {
            _actionControl = actionControl;
            Name = actionControl.DisplayName;
            Description = actionControl.Description;
            ActionControl = actionControl;
        }

        public void RaiseIsEnabledChanged()
        {
            OnPropertyChanged("IsEnabled");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
