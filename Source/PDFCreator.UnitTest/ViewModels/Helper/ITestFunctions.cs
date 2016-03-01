using pdfforge.PDFCreator.Core.Actions;

namespace PDFCreator.UnitTest.ViewModels.Helper
{
    public interface ITestFunctions
    {
        void VoidFunctionWithoutParameters();
        void VoidFunctionWithBoolParameters(bool? boolParameter);
        bool ReturnsBoolWithoutParameters();
        bool ReturnsBoolWithTwoStringParameters(string firstStringParameter, string secondStringParameter);
        bool ReturnsBoolWithActionResultDictParameter(ActionResultDict actionResultDictParameter);
        string ReturnsStringWithStringParameter(string stringParameter);
    }
}
