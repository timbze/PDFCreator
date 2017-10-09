using MahApps.Metro.Controls;
using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.UI.Presentation.Helper;

namespace pdfforge.PDFCreator.UI.Presentation
{
    public partial class InteractionHostWindow : MetroWindow
    {
        public IInteractionRequest InteractionRequest { get; }

        public InteractionHostWindow(IInteractionRequest interactionRequest, IHightlightColorRegistration hightlightColorRegistration)
        {
            InteractionRequest = interactionRequest;
            InitializeComponent();
            hightlightColorRegistration.RegisterHighlightColorResource(this);
        }
    }
}
