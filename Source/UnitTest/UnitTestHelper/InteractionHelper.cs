using pdfforge.Obsidian.Interaction;

namespace pdfforge.PDFCreator.UnitTest.UnitTestHelper
{
    public class InteractionHelper<T> where T : class, IInteraction
    {
        private IInteractionAware _viewModel;

        public InteractionHelper(IInteractionAware viewModel, T interaction)
        {
            _viewModel = viewModel;
            viewModel.FinishInteraction = () => InteractionIsFinished = true;
            viewModel.SetInteraction(interaction);
        }

        public bool InteractionIsFinished { get; private set; }
    }
}
