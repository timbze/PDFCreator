using pdfforge.PDFCreator.UI.Presentation.Controls;
using System.ComponentModel;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob
{
    public interface IPasswordButtonViewModel : INotifyPropertyChanged
    {
        PasswordButtonController PasswordButtonController { get; }
        string Password { get; set; }

        void ClearPasswordFields();

        void FinishedHook();

        void SetPassword(string password);

        void SkipHook();

        void CancelHook();

        void OkHook();

        void RemoveHook();

        bool CanExecuteHook();
    }
}
