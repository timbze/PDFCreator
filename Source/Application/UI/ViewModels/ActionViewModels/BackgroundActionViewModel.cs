using System.Collections.Generic;
using pdfforge.DynamicTranslator;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.UI.ViewModels.Helper;

namespace pdfforge.PDFCreator.UI.ViewModels.ActionViewModels
{
    public class BackgroundActionViewModel : ActionViewModel
    {
        private readonly IOpenFileInteractionHelper _openFileInteractionHelper;

        public BackgroundActionViewModel(ITranslator translator, IOpenFileInteractionHelper openFileInteractionHelper)
        {
            _openFileInteractionHelper = openFileInteractionHelper;
            Translator = translator;

            SelectBackgroundCommand = new DelegateCommand(SelectBackgroundExecute);

            DisplayName = translator.GetTranslation("BackgroundSettings", "DisplayName");
            Description = translator.GetTranslation("BackgroundSettings", "Description");
        }

        public ITranslator Translator { get; }

        public DelegateCommand SelectBackgroundCommand { get; set; }

        public IEnumerable<EnumValue<BackgroundRepetition>> BackgroundRepetitionValues => Translator.GetEnumTranslation<BackgroundRepetition>();

        public override bool IsEnabled
        {
            get { return CurrentProfile != null && CurrentProfile.BackgroundPage.Enabled; }
            set
            {
                CurrentProfile.BackgroundPage.Enabled = value;
                RaisePropertyChanged(nameof(IsEnabled));
            }
        }

        private void SelectBackgroundExecute(object obj)
        {
            var titel = Translator.GetTranslation("BackgroundSettings", "SelectBackgroundFile");
            var filter = Translator.GetTranslation("BackgroundSettings", "PDFFiles")
                         + @" (*.pdf)|*.pdf|"
                         + Translator.GetTranslation("BackgroundSettings", "AllFiles")
                         + @" (*.*)|*.*";

            CurrentProfile.BackgroundPage.File = _openFileInteractionHelper.StartOpenFileInteraction(CurrentProfile.BackgroundPage.File, titel, filter);
            RaisePropertyChanged(nameof(CurrentProfile));
        }

        public override HelpTopic GetContextBasedHelpTopic()
        {
            return HelpTopic.Background;
        }
    }
}