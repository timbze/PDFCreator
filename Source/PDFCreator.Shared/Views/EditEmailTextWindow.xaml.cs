using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using pdfforge.PDFCreator.Shared.Converter;
using pdfforge.PDFCreator.Shared.Helper;

namespace pdfforge.PDFCreator.Shared.Views
{
    public partial class EditEmailTextWindow : Window
    {
        private string _signatureText;
        
        public string Subject
        {
            get { return SubjectText.Text; }
            set { SubjectText.Text = value; }
        }

        public string Body
        {
            get { return BodyText.Text; }
            set { BodyText.Text = value; }
        }
        
        public bool AddSignature { get; set; }

        private TokenReplacerConverter BodyTokenReplacerConverter
        {
            get { return FindResource("TokenReplacerConverterWithFooter") as TokenReplacerConverter; }
        }

        private TokenReplacerConverter HeaderTokenReplacerConverter
        {
            get { return FindResource("TokenReplacerConverter") as TokenReplacerConverter; }
        }

        public EditEmailTextWindow(bool addSignature)
        {
            AddSignature = addSignature;

            InitializeComponent();

            TranslationHelper.Instance.TranslatorInstance.Translate(this);

            BodyTokenReplacerConverter.TokenReplacer = TokenHelper.TokenReplacerWithPlaceHolders;
            HeaderTokenReplacerConverter.TokenReplacer = BodyTokenReplacerConverter.TokenReplacer;

            var tokens = TokenHelper.GetTokenListForEmail();

            SubjectTokenComboBox.ItemsSource = tokens;
            BodyTokenComboBox.ItemsSource = tokens;

            _signatureText = MailSignatureHelper.ComposeMailSignature();
            AttachSignature_OnChecked(null, null);
        }

        private void OkButton_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void AttachSignature_OnChecked(object sender, RoutedEventArgs e)
        {
            BodyTokenReplacerConverter.Footer = AddSignature ? _signatureText : "";

            BindingExpression be = FinalBodyText.GetBindingExpression(TextBox.TextProperty);
            be.UpdateTarget();
        }

        private void SubjectTokenComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            InsertToken(SubjectText, (ComboBox)sender);
        }

        private void BodyTokenComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            InsertToken(BodyText, (ComboBox)sender);
        }

        private void InsertToken(TextBox txt, ComboBox cmb)
        {
            string text = cmb.SelectedItem.ToString();
            int newSelectionStart = txt.SelectionStart + text.Length;
            txt.Text = txt.Text.Insert(txt.SelectionStart, text);
            txt.SelectionStart = newSelectionStart;
        }
    }
}
