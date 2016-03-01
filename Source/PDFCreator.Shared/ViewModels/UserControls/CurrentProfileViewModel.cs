using System;
using pdfforge.PDFCreator.Core.Settings;

namespace pdfforge.PDFCreator.Shared.ViewModels.UserControls
{
    public class CurrentProfileViewModel: ViewModelBase
    {
        private ConversionProfile _currentProfile;

        public event EventHandler ProfileChanged; 

        public ConversionProfile CurrentProfile
        {
            get { return _currentProfile; }
            set
            {
                _currentProfile = value;
                OnProfileChanged(new EventArgs());
            }
        }

        protected virtual void OnProfileChanged(EventArgs e)
        {
            RaisePropertyChanged("CurrentProfile");
            var handler = ProfileChanged;
            if (handler != null) handler(this, e);
        }

        public void RaiseCurrentProfileChanged()
        {
            OnProfileChanged(new EventArgs());
        }
    }
}
