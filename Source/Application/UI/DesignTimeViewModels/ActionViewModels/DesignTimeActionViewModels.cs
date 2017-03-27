using pdfforge.PDFCreator.Core.Printing.Printer;
using pdfforge.PDFCreator.UI.DesignTimeViewModels.Helper;
using pdfforge.PDFCreator.UI.ViewModels.ActionViewModels;
using pdfforge.PDFCreator.UI.ViewModels.ActionViewModels.Translations;
using pdfforge.PDFCreator.UI.ViewModels.Helper;
using pdfforge.PDFCreator.UI.ViewModels.Translations;
using pdfforge.PDFCreator.Utilities.Process;

namespace pdfforge.PDFCreator.UI.DesignTimeViewModels.ActionViewModels
{
    public class DesignTimeAttatchmentViewModel : AttatchmentActionViewModel
    {
        public DesignTimeAttatchmentViewModel() : base(new OpenFileInteractionHelper(new DesignTimeInteractionInvoker()), new AttachmentSettingsAndActionTranslation())
        {
        }
    }

    public class DesignTimeBackgroundViewModel : BackgroundActionViewModel
    {
        public DesignTimeBackgroundViewModel() : base(new BackgroundSettingsAndActionTranslation(), new OpenFileInteractionHelper(new DesignTimeInteractionInvoker()))
        {
        }
    }

    public class DesignTimeCoverViewModel : CoverActionViewModel
    {
        public DesignTimeCoverViewModel() : base(new CoverSettingsTranslation(), new OpenFileInteractionHelper(new DesignTimeInteractionInvoker()))
        {
        }
    }

    public class DesignTimeEmailClientViewModel : EmailClientActionViewModel
    {
        public DesignTimeEmailClientViewModel() : base(new EmailClientActionSettingsAndActionTranslation(), new DesignTimeInteractionInvoker(), new DesignTimeClientTestEmail())
        {
        }
    }

    public class DesignTimeEmailSmtpViewModel : EmailSmtpActionViewModel
    {
        public DesignTimeEmailSmtpViewModel() : base(new SmtpSettingsAndActionControlTranslation(), new DesignTimeInteractionInvoker(), new DesignTimeSmtpTest())
        {
        }
    }

    public class DesignTimeFtpViewModel : FtpActionViewModel
    {
        public DesignTimeFtpViewModel() : base(new FtpActionSettingsAndActionTranslation(), new DesignTimeInteractionInvoker(), new TokenHelper(new TokenPlaceHoldersTranslation()))
        {
        }
    }

    public class DesignTimeOpenViewerViewModel : OpenViewerActionViewModel
    {
        public DesignTimeOpenViewerViewModel() : base(new OpenViewerSettingsAndActionTranslation(), new DesignTimeInteractionInvoker(), new DesignTimeArchitectCheck(), new DesignTimeSettingsProvider(), new ProcessStarter())
        {
        }
    }

    public class DesignTimePrintViewModel : PrintActionViewModel
    {
        public DesignTimePrintViewModel() : base(new PrintActionSettingsAndActionTranslation(), new SystemPrinterProvider())
        {
        }
    }

    public class DesignTimeScriptViewModel : ScriptActionViewModel
    {
        public DesignTimeScriptViewModel() : base(new ScriptActionSettingsAndActionTranslation(), new OpenFileInteractionHelper(new DesignTimeInteractionInvoker()), new DesignTimeScriptActionHelper(), new TokenHelper(new TokenPlaceHoldersTranslation()))
        {
        }
    }

    public class DesignTimeUserTokenViewModel : UserTokenActionViewModel
    {
        public DesignTimeUserTokenViewModel() : base(new  UserTokenActionViewModelTranslation(), null)
        {
        }
    }
}