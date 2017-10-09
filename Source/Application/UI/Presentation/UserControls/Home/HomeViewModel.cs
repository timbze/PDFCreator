using pdfforge.Obsidian;
using pdfforge.Obsidian.Interaction.DialogInteractions;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.Printing.Printer;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using System.Collections.Generic;
using System.Windows.Input;
using DelegateCommand = Prism.Commands.DelegateCommand;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Home
{
    public class HomeViewModel : TranslatableViewModelBase<HomeViewTranslation>
    {
        private readonly IFileConversionHandler _fileConversionHandler;
        private readonly IPrinterHelper _printerHelper;
        private readonly ISettingsProvider _settingsProvider;
        private readonly IInteractionInvoker _interactionInvoker;

        public HomeViewModel(IInteractionInvoker interactionInvoker, IFileConversionHandler fileConversionHandler, ITranslationUpdater translationUpdater, IPrinterHelper printerHelper, ISettingsProvider settingsProvider)
            : base(translationUpdater)
        {
            _interactionInvoker = interactionInvoker;
            _fileConversionHandler = fileConversionHandler;
            _printerHelper = printerHelper;
            _settingsProvider = settingsProvider;
            ConvertFileCommand = new DelegateCommand(ConvertFileExecute);
        }

        public ICommand ConvertFileCommand { get; set; }

        public string CallToActionText => Translation.FormatCallToAction(_printerHelper.GetApplicablePDFCreatorPrinter(_settingsProvider.Settings?.ApplicationSettings?.PrimaryPrinter ?? ""));

        private void ConvertFileExecute()
        {
            var interaction = new OpenFileInteraction();

            _interactionInvoker.Invoke(interaction);

            if (!interaction.Success)
                return;

            var file = interaction.FileName;
            _fileConversionHandler.HandleFileList(new List<string> { file });
        }

        protected override void OnTranslationChanged()
        {
            RaisePropertyChanged(nameof(CallToActionText));
        }
    }
}
