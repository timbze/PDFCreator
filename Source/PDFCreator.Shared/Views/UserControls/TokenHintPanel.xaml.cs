using System.Windows;
using System.Windows.Controls;
using pdfforge.PDFCreator.Shared.Helper;

namespace pdfforge.PDFCreator.Shared.Views.UserControls
{    
    public partial class TokenHintPanel : UserControl
    {
        private static readonly TranslationHelper TranslationHelper = TranslationHelper.Instance;

        public static readonly DependencyProperty TextWithTokenProperty = DependencyProperty.Register(
            "TextWithToken", typeof(string), typeof(TokenHintPanel), new PropertyMetadata(""));
        
        public TokenHintPanel()
        {
            InitializeComponent();
            if (TranslationHelper.IsInitialized)
            {
                TranslationHelper.TranslatorInstance.Translate(this);
            }
        }

        public string TextWithToken
        {
            get { return (string)GetValue(TextWithTokenProperty); }
            set { SetValue(TextWithTokenProperty, value); }
        }

        private void TokenHintOnClick(object sender, RoutedEventArgs e)
        {
            UserGuideHelper.ShowHelp(HelpTopic.Tokens);
        }
    }
}
