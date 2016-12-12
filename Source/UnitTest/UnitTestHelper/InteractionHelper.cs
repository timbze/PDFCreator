using pdfforge.Obsidian.Interaction;

namespace pdfforge.PDFCreator.UnitTest.UnitTestHelper
{
    public class InteractionHelper<T> where T : class, IInteraction
    {
        private InteractionAwareViewModelBase<T> _viewModel;

        public InteractionHelper(InteractionAwareViewModelBase<T> viewModel, T interaction)
        {
            _viewModel = viewModel;
            viewModel.FinishInteraction = () => InteractionIsFinished = true;
            viewModel.SetInteraction(interaction);
        }

        public bool InteractionIsFinished { get; private set; }
    }
}