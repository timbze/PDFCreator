using System.ComponentModel;
using System.IO;
using pdfforge.Obsidian;
using pdfforge.Obsidian.Interaction;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.ViewModels.WindowViewModels.Translations;
using pdfforge.PDFCreator.Utilities.Process;

namespace pdfforge.PDFCreator.UI.ViewModels.WindowViewModels
{
    public class PlusHintWindowViewModel : InteractionAwareViewModelBase<PlusHintInteraction>
    {
        public PlusHintWindowTranslation Translation { get; private set; }
        private readonly IProcessStarter _processStarter;

        public PlusHintWindowViewModel(PlusHintWindowTranslation translation, IProcessStarter processStarter)
        {
            Translation = translation;
            _processStarter = processStarter;
            PlusButtonCommand = new DelegateCommand(PlusButtonExecute);
        }

        public DelegateCommand PlusButtonCommand { get; set; }
        public DelegateCommand NoThanksCommand => new DelegateCommand(o => FinishInteraction());

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
            ThankYouText = Translation.GetThankYouMessage(Interaction.CurrentJobCount);
            RaisePropertyChanged(nameof(ThankYouText));
        }
    }
}