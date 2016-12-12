using System.ComponentModel;
using System.IO;
using SystemInterface.Diagnostics;
using pdfforge.DynamicTranslator;
using pdfforge.Obsidian;
using pdfforge.Obsidian.Interaction;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.Utilities.Process;

namespace pdfforge.PDFCreator.UI.ViewModels.WindowViewModels
{
    public class PlusHintWindowViewModel : InteractionAwareViewModelBase<PlusHintInteraction>
    {
        private readonly IProcessStarter _processStarter;
        private readonly ITranslator _translator;

        public PlusHintWindowViewModel(ITranslator translator, IProcessStarter processStarter)
        {
            _translator = translator;
            _processStarter = processStarter;
            PlusButtonCommand = new DelegateCommand(PlusButtonExecute);
        }

        public DelegateCommand PlusButtonCommand { get; set; }

        public string ThankYouText { get; set; }

        private void PlusButtonExecute(object obj)
        {
            ShowUrlInBrowser(Urls.PlusHintLink);
            FinishInteraction();
        }

        private void ShowUrlInBrowser(string url)
        {
            try
            {
                _processStarter.Start(url);
            }
            catch (Win32Exception)
            {
            }
            catch (FileNotFoundException)
            {
            }
        }

        protected override void HandleInteractionObjectChanged()
        {
            ThankYouText = _translator.GetFormattedTranslation("PlusHintWindow", "ThankYou", Interaction.CurrentJobCount);
            RaisePropertyChanged(nameof(ThankYouText));
        }
    }
}