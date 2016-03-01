using System;
using System.ComponentModel;

namespace pdfforge.PDFCreator.Shared.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public Action<bool?> CloseViewAction { get; set; }

        public void RaiseCloseView(bool? result)
        {
            if (CloseViewAction != null)
                CloseViewAction(result);
        }

        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
