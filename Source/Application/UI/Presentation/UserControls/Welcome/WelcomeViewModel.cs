using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Presentation.Commands;
using pdfforge.PDFCreator.UI.Presentation.Commands.UserGuide;
using pdfforge.PDFCreator.UI.Presentation.Help;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ServiceLocator;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using pdfforge.PDFCreator.Utilities.Process;
using System;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Welcome
{
    public class WelcomeViewModel : OverlayViewModelBase<WelcomeInteraction, WelcomeWindowTranslation>, IWhitelisted
    {
        private readonly IProcessStarter _processStarter;

        public WelcomeViewModel(ICommandLocator commandLocator, ITranslationUpdater translationUpdater, EditionHintOptionProvider editionHintOptionProvider)
            : base(translationUpdater)
        {
            AllowPrioritySupport = !editionHintOptionProvider?.ShowOnlyForPlusAndBusinessHint ?? true;

            WhatsNewCommand = commandLocator.GetInitializedCommand<ShowUserGuideCommand, HelpTopic>(HelpTopic.WhatsNew);
            BlogCommand = commandLocator.GetInitializedCommand<UrlOpenCommand, string>(Urls.Blog);
            PrioritySupportCommand = commandLocator.GetCommand<PrioritySupportUrlOpenCommand>();
        }

        public Boolean AllowPrioritySupport { get; }

        public ICommand WhatsNewCommand { get; }
        public ICommand BlogCommand { get; }
        public ICommand PrioritySupportCommand { get; }

        public override string Title => Translation.Title;
    }
}
