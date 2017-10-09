using System.Windows;
using System.Windows.Media;

namespace pdfforge.PDFCreator.UI.Presentation.Helper
{
    public interface IHightlightColorRegistration
    {
        void RegisterHighlightColorResource(FrameworkElement frameworkElement);
    }

    public class HightlightColorRegistration : IHightlightColorRegistration
    {
        private readonly Color _highlightColor;

        public const string HighlightColorName = "ApplicationHightlightColor";

        public HightlightColorRegistration(Color highlightColor)
        {
            _highlightColor = highlightColor;
        }

        public void RegisterHighlightColorResource(FrameworkElement frameworkElement)
        {
            frameworkElement.Resources[HighlightColorName] = new SolidColorBrush(_highlightColor);
        }
    }
}
