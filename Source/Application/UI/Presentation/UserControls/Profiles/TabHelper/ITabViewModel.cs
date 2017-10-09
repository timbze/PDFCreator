using pdfforge.PDFCreator.UI.Presentation.ServiceLocator;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.TabHelper
{
    public interface ITabViewModel : IWhitelisted
    {
        string Title { get; }
        IconList Icon { get; }

        bool HiddenByGPO { get; }
        bool BlockedByGPO { get; }
    }
}
