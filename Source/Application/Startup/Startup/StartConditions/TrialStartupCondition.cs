using pdfforge.PDFCreator.Core.Startup.Translations;
using pdfforge.PDFCreator.Core.StartupInterface;
using pdfforge.PDFCreator.UI.Presentation.Customization;
using pdfforge.PDFCreator.Utilities;
using Translatable;

namespace pdfforge.PDFCreator.Core.Startup.StartConditions
{
    public class TrialStartupCondition : IStartupCondition
    {
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ViewCustomization _viewCustomization;
        private readonly ProgramTranslation _translation;

        public TrialStartupCondition(ITranslationFactory translationFactory, IDateTimeProvider dateTimeProvider, ViewCustomization viewCustomization)
        {
            _dateTimeProvider = dateTimeProvider;
            _viewCustomization = viewCustomization;
            _translation = translationFactory.CreateTranslation<ProgramTranslation>();
        }

        public bool CanRequestUserInteraction => false;

        public StartupConditionResult Check()
        {
            if (_viewCustomization.TrialEnabled)
            {
                if (_viewCustomization.TrialExpireDateTime < _dateTimeProvider.Now().Date)
                {
                    var errorMessage = _translation.GetFormattedTrialExpiredTranslation(_viewCustomization.TrialExpireDateString);

                    return StartupConditionResult.BuildErrorWithMessage((int)ExitCode.TrialExpired, errorMessage, true);
                }
            }

            return StartupConditionResult.BuildSuccess();
        }
    }
}
