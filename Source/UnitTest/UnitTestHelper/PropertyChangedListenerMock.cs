using System.ComponentModel;

namespace pdfforge.PDFCreator.UnitTest.UnitTestHelper
{
    public class PropertyChangedListenerMock
    {
        private readonly string _propertyName;

        public PropertyChangedListenerMock(INotifyPropertyChanged notifier, string propertyName)
        {
            notifier.PropertyChanged += notifier_PropertyChanged;
            _propertyName = propertyName;
        }

        public bool WasCalled { get; private set; }

        private void notifier_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == _propertyName)
                WasCalled = true;
        }
    }
}
