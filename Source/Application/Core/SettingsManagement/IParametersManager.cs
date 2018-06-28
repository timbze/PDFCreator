using pdfforge.PDFCreator.Conversion.Settings;

namespace pdfforge.PDFCreator.Core.SettingsManagement
{
    public interface IParametersManager
    {
        bool HasPredefinedParameters();

        Parameters GetAndResetParameters();

        void SaveParameterSettings(string outputFile, string profileParameter);
    }
}
