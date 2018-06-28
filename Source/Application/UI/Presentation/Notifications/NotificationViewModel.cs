using pdfforge.PDFCreator.Conversion.Jobs.Annotations;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ToastNotifications.Core;

namespace pdfforge.PDFCreator.UI.Presentation.Notifications
{
    public class NotificationViewModel : NotificationBase, INotifyPropertyChanged
    {
        private NotificationView _notificationView;

        public NotificationType NotificationType { get; set; }

        public override NotificationDisplayPart DisplayPart => _notificationView ?? (_notificationView = new NotificationView(this));

        private string _title;

        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        public bool IsSuccess => NotificationType == NotificationType.Success;
        public bool IsError => NotificationType == NotificationType.Error;

        private string _message;

        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                _message = value;
                OnPropertyChanged();
            }
        }

        public NotificationViewModel(string title, string message, NotificationType type)
        {
            Title = title;
            Message = message;
            NotificationType = type;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public enum NotificationType
    {
        Success,
        Error
    }
}
