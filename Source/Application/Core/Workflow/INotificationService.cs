namespace pdfforge.PDFCreator.Core.Workflow
{
    public interface INotificationService
    {
        void ShowInfoNotification(string documentName);

        void ShowErrorNotification(string documentName);
    }

    public class DisabledNotificationService : INotificationService
    {
        public void ShowInfoNotification(string documentName)
        {
        }

        public void ShowErrorNotification(string documentName)
        {
        }
    }
}
