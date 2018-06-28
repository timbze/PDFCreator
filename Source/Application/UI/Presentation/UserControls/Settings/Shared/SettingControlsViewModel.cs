using System.Windows.Input;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.UI.Presentation.Commands;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using Prism.Mvvm;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.Shared
{
    public class SettingControlsViewModel: TranslatableViewModelBase<SettingControlsTranslation>
    {
        public SettingControlsViewModel(ITranslationUpdater translationUpdater, ICommandLocator commandLocator):base(translationUpdater)
        {
            SaveCommand = commandLocator.CreateMacroCommand()
                .AddCommand<CheckProfileCommand>()
                .AddCommand<SaveApplicationSettingsChangesCommand>()
                .AddCommand<NavigateMainTabCommand>()
                .Build();

            CancelCommand = commandLocator.CreateMacroCommand()
                .AddCommand<CancelApplicationSettingsChangesCommand>()
                .AddCommand<NavigateMainTabCommand>()
                .Build();
        }

        public ICommand SaveCommand { get; private set; }

        public ICommand CancelCommand { get; private set; }
    }
}
