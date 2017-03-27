using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using pdfforge.PDFCreator.UI.ViewModels.Helper;
using pdfforge.PDFCreator.UI.ViewModels.Translations;
using pdfforge.PDFCreator.UI.Views.Translations;
using Translatable;

namespace pdfforge.PDFCreator.UI.Views.UserControls
{
    public partial class TokenHintPanel : UserControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty TextWithTokenProperty = DependencyProperty.Register(
            "TextWithToken", typeof (string), typeof (TokenHintPanel), new PropertyMetadata("", TextWithTokenChanged));

        public TokenHintPanel()
        {
            // Initialize TokenHelper with empty Translator to avoid exception in converter
            TokenHelper = new TokenHelper(new TokenPlaceHoldersTranslation());

            InitializeComponent();

            var translation = TranslationFactory.CreateTranslation<TokenHintPanelTranslation>();
            InsecureTokenTextBlock.Text = translation.InsecureTokenText;
        }

        public static IUserGuideHelper UserGuideHelper { private get; set; }
        public static ITranslationFactory TranslationFactory { private get; set; }

        public TokenHelper TokenHelper { get; private set; }

        public string TextWithToken
        {
            get { return GetValue(TextWithTokenProperty) as string; }
            set { SetValue(TextWithTokenProperty, value); }
        }

        public string TextWrapper => TextWithToken ?? "";

        public event PropertyChangedEventHandler PropertyChanged;

        private static void TextWithTokenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var panel = d as TokenHintPanel;
            panel?.RaiseTextChanged();
        }

        private void RaiseTextChanged()
        {
            OnPropertyChanged(nameof(TextWrapper));
        }

        private void TokenHintOnClick(object sender, RoutedEventArgs e)
        {
            UserGuideHelper.ShowHelp(HelpTopic.Tokens);
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}