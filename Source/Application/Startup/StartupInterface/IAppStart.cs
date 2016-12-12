namespace pdfforge.PDFCreator.Core.StartupInterface
{
    public interface IAppStart
    {
        /// <summary>
        /// Check all applicable startup conditions. If a condition fails, a StartupConditionFailedException will be thrown
        /// </summary>
        void CheckApplicationConditions();

        ExitCode Run();
    }
}