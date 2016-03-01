using System.Windows.Controls;
using pdfforge.PDFCreator.Shared.Helper;
using pdfforge.PDFCreator.Shared.ViewModels.UserControls;

namespace pdfforge.PDFCreator.Shared.Views.UserControls
{
    public partial class ActionsTab : UserControl
    {
        public ActionsTabViewModel ViewModel
        {
            get { return (ActionsTabViewModel)DataContext; }
        }

        public ActionsTab()
        {
            InitializeComponent();
            if (TranslationHelper.Instance.IsInitialized)
            {
                TranslationHelper.Instance.TranslatorInstance.Translate(this);
            }
        }

        
    }
}
