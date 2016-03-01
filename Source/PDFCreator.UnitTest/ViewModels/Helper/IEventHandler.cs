namespace PDFCreator.UnitTest.ViewModels.Helper
{
    public interface IEventHandler<T>
    {
        void OnEventRaised(object sender, T eventargs);
    }
}