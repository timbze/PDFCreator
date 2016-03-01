using System.ComponentModel;
using System.Windows.Controls;
using pdfforge.PDFCreator.Core.Settings;
using pdfforge.PDFCreator.Shared.Annotations;

namespace pdfforge.PDFCreator.Shared.Views.ActionControls
{
    public class ActionControl : UserControl, INotifyPropertyChanged
    {
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public virtual bool IsActionEnabled { get; set; }

        private ConversionProfile _conversionProfile;

        public ConversionProfile CurrentProfile
        {
            get { return _conversionProfile; }
            set
            {
                _conversionProfile = value;
                OnPropertyChanged("CurrentProfile");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
