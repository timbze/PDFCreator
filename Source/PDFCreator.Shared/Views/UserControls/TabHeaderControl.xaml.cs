using System.Windows;
using System.Windows.Controls;

namespace pdfforge.PDFCreator.Shared.Views.UserControls
{
    public partial class TabHeaderControl : UserControl
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text", typeof (string), typeof (TabHeaderControl), new PropertyMetadata(""));

        public TabHeaderControl()
        {
            InitializeComponent();
        }

        public string Text
        {
            get { return (string) GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
    }
}