namespace pdfforge.PDFCreator.Core.StartupInterface
{
    public interface IStartupCondition
    {
        StartupConditionResult Check();
    }
}