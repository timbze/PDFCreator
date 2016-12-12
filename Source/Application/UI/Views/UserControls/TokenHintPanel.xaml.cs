using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using pdfforge.DynamicTranslator;
using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.UI.ViewModels.Helper;

namespace pdfforge.PDFCreator.UI.Views.UserControls
{
    public partial class TokenHintPanel : UserControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty TextWithTokenProperty = DependencyProperty.Register(
            "TextWithToken", typeof (string), typeof (TokenHintPanel), new PropertyMetadata("", TextWithTokenChanged));

        public static readonly DependencyProperty TranslatorProperty = DependencyProperty.Register(
            "Translator", typeof (ITranslator), typeof (TokenHintPanel), new PropertyMetadata(TranslatorChanged));

        public TokenHintPanel()
        {
            // Initialize TokenHelper with empty Translator to avoid exception in converter
            TokenHelper = new TokenHelper(new TranslationProxy());
            InitializeComponent();
        }

        public static IUserGuideHelper UserGuideHelper { private get; set; }

        public TokenHelper TokenHelper { get; private set; }

        public string TextWithToken
        {
            get { return GetValue(TextWithTokenProperty) as string; }
            set { SetValue(TextWithTokenProperty, value); }
        }

        public string TextWrapper => TextWithToken ?? "";

        public ITranslator Translator
        {
            get { return (ITranslator) GetValue(TranslatorProperty); }
            set { SetValue(TranslatorProperty, value); }
        }

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

        private static void TranslatorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var panel = d as TokenHintPanel;
            panel?.DoTranslate();
        }

        private void DoTranslate()
        {
            Translator?.Translate(this);
            TokenHelper = new TokenHelper(Translator);
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