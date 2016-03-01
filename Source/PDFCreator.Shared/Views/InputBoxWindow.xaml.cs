using System;
using System.Windows;
using System.Windows.Controls;
using pdfforge.PDFCreator.Shared.Helper;

namespace pdfforge.PDFCreator.Shared.Views
{
    public partial class InputBoxWindow : Window
    {
        public Func<string, InputBoxValidation> IsValidInput
        {
            get;
            set;
        }

        public string InputText
        {
            get { return TextBox.Text; }
            set { TextBox.Text = value; }
        }

        public string QuestionText
        {
            set { Label.Content = value; }
        }
        
        public InputBoxWindow()
        {
            InitializeComponent();
        }

        private void OkButton_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            TranslationHelper.Instance.TranslatorInstance.Translate(this);
        }

        private void TextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (IsValidInput == null)
                return;

            var validation = IsValidInput(TextBox.Text);

            OkButton.IsEnabled = validation.IsValid;
            ValidationMessageText.Text = validation.Message;
        }
    }

    public class InputBoxValidation
    {
        public InputBoxValidation(bool isValid)
        {
            IsValid = isValid;
        }

        public InputBoxValidation(bool isValid, string message)
        {
            IsValid = isValid;
            Message = message;
        }

        public bool IsValid { get; set; }

        public string Message { get; set; }
    }
}
