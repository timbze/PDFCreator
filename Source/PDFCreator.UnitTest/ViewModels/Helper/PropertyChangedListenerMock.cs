using System.ComponentModel;

namespace PDFCreator.UnitTest.ViewModels.Helper
{
    public class PropertyChangedListenerMock
    {
        public bool WasCalled { get; private set; }
        private readonly string _propertyName;

        public PropertyChangedListenerMock(INotifyPropertyChanged notifier, string propertyName)
        {
            notifier.PropertyChanged += notifier_PropertyChanged;
            _propertyName = propertyName;
        }

        void notifier_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == _propertyName)
                WasCalled = true;
        }
    }
}