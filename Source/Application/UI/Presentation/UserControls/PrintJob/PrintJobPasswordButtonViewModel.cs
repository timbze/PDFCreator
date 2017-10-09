using pdfforge.Obsidian;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob
{
    public class PrintJobPasswordButtonViewModel : TranslatableViewModelBase<PrintJobPasswordButtonTranslation>
    {
        public PrintJobPasswordButtonViewModel(ITranslationUpdater translationUpdater) : base(translationUpdater)
        {
        }

        public bool AllowSkip { get; set; }
        public bool AllowRemove { get; set; }

        public DelegateCommand OkCommand { get; set; }
        public ICommand SkipCommand { get; set; }
        public ICommand RemoveCommand { get; set; }
        public ICommand CancelCommand { get; set; }
    }

    public class DesignTimePrintJobPasswordButtonViewModel : PrintJobPasswordButtonViewModel
    {
        public DesignTimePrintJobPasswordButtonViewModel() : base(new DesignTimeTranslationUpdater())
        {
            AllowSkip = true;
            AllowRemove = true;
        }
    }
}
