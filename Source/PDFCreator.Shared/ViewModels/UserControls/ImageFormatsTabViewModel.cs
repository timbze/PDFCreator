using System.Collections.Generic;
using pdfforge.DynamicTranslator;
using pdfforge.PDFCreator.Core.Settings.Enums;
using pdfforge.PDFCreator.Shared.Helper;

namespace pdfforge.PDFCreator.Shared.ViewModels.UserControls
{
    public class ImageFormatsTabViewModel : CurrentProfileViewModel
    {
        private static readonly TranslationHelper TranslationHelper = TranslationHelper.Instance;

        public static IEnumerable<EnumValue<JpegColor>> JpegColorValues
        {
            get { return TranslationHelper.TranslatorInstance.GetEnumTranslation<JpegColor>(); }
        }

        public static IEnumerable<EnumValue<PngColor>> PngColorValues
        {
            get { return TranslationHelper.TranslatorInstance.GetEnumTranslation<PngColor>(); }
        }

        public static IEnumerable<EnumValue<TiffColor>> TiffColorValues
        {
            get { return TranslationHelper.TranslatorInstance.GetEnumTranslation<TiffColor>(); }
        }

    }
}
