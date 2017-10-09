using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.ServiceLocator;
using System;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.Controls.Tab
{
    public interface ISubTabViewModel : IWhitelisted

    {
        string Title { get; }
        bool IsChecked { get; set; }

        void Init<TTranslation>(Func<TTranslation, string> titleId, Func<ConversionProfile, IProfileSetting> setting, PrismNavigationValueObject navigationObject) where TTranslation : ITranslatable, new();
    }
}
