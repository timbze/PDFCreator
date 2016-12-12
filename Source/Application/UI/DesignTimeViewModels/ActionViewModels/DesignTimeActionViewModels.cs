using pdfforge.Obsidian;
using pdfforge.PDFCreator.Core.Printing.Printer;
using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.UI.DesignTimeViewModels.Helper;
using pdfforge.PDFCreator.UI.ViewModels.ActionViewModels;
using pdfforge.PDFCreator.UI.ViewModels.Helper;
using pdfforge.PDFCreator.Utilities.Process;

namespace pdfforge.PDFCreator.UI.DesignTimeViewModels.ActionViewModels
{
    public class DesignTimeAttatchmentViewModel : AttatchmentActionViewModel
    {
        public DesignTimeAttatchmentViewModel() : base(new TranslationProxy(), new OpenFileInteractionHelper(new DesignTimeInteractionInvoker()))
        {
        }
    }

    public class DesignTimeBackgroundViewModel : BackgroundActionViewModel
    {
        public DesignTimeBackgroundViewModel() : base(new TranslationProxy(), new OpenFileInteractionHelper(new DesignTimeInteractionInvoker()))
        {
        }
    }

    public class DesignTimeCoverViewModel : CoverActionViewModel
    {
        public DesignTimeCoverViewModel() : base(new TranslationProxy(), new OpenFileInteractionHelper(new DesignTimeInteractionInvoker()))
        {
        }
    }

    public class DesignTimeEmailClientViewModel : EmailClientActionViewModel
    {
        public DesignTimeEmailClientViewModel() : base(new TranslationProxy(), new DesignTimeInteractionInvoker(), new DesignTimeClientTestEmail())
        {
        }
    }

    public class DesignTimeEmailSmtpViewModel : EmailSmtpActionViewModel
    {
        public DesignTimeEmailSmtpViewModel() : base(new TranslationProxy(), new DesignTimeInteractionInvoker(), new DesignTimeSmtpTest())
        {
        }
    }

    public class DesignTimeFtpViewModel : FtpActionViewModel
    {
        public DesignTimeFtpViewModel() : base(new TranslationProxy(), new DesignTimeInteractionInvoker())
        {
        }
    }

    public class DesignTimeOpenViewerViewModel : OpenViewerActionViewModel
    {
        public DesignTimeOpenViewerViewModel() : base(new TranslationProxy(), new DesignTimeInteractionInvoker(), new DesignTimeArchitectCheck(), new DesignTimeSettingsProvider(), new ProcessStarter())
        {
        }
    }

    public class DesignTimePrintViewModel : PrintActionViewModel
    {
        public DesignTimePrintViewModel() : base(new TranslationProxy(), new SystemPrinterProvider())
        {
        }
    }

    public class DesignTimeScriptViewModel : ScriptActionViewModel
    {
        public DesignTimeScriptViewModel() : base(new TranslationProxy(), new OpenFileInteractionHelper(new DesignTimeInteractionInvoker()), new DesignTimeScriptActionHelper())
        {
        }
    }

    public class DesignTimeUserTokenViewModel : UserTokenActionViewModel
    {
        public DesignTimeUserTokenViewModel() : base(new TranslationProxy(), null)
        {
        }
    }
}