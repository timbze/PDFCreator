namespace pdfforge.PDFCreator.UnitTest.UnitTestHelper
{
    public interface IEventHandler<T>
    {
        void OnEventRaised(object sender, T eventargs);
    }
}
