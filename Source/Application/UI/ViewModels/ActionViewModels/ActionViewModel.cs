using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.UI.ViewModels.UserControlViewModels.ProfileSettings;

namespace pdfforge.PDFCreator.UI.ViewModels.ActionViewModels
{
    public abstract class ActionViewModel : CurrentProfileViewModel
    {
        private Accounts _accounts;
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public ActionControl Content { get; set; }
        public abstract bool IsEnabled { get; set; }

        public Accounts Accounts
        {
            get { return _accounts; }
            set
            {
                _accounts = value;
                SetAccount(_accounts);
            }
        }

        protected virtual void SetAccount(Accounts accounts)
        {

        }

        public void RaiseEnabledChanged()
        {
            RaisePropertyChanged(nameof(IsEnabled));
        }
    }
}