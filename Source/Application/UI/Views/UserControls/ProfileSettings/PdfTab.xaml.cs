using System.Windows;
using System.Windows.Controls;
using pdfforge.PDFCreator.UI.ViewModels.Helper;
using pdfforge.PDFCreator.UI.ViewModels.UserControlViewModels.ProfileSettings;

namespace pdfforge.PDFCreator.UI.Views.UserControls.ProfileSettings
{
    public partial class PdfTab : UserControl
    {
        public PdfTab()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var viewModel = DataContext as PdfTabViewModel;
            if (viewModel == null)
                return;

            viewModel.QueryHelpTopicForCurrentTab = QueryHelpForCurrentTab;
            viewModel.Translator.Translate(this);
        }

        private HelpTopic QueryHelpForCurrentTab()
        {
            var activePdfTab = PdfSettingsTabControl.SelectedItem;

            if (ReferenceEquals(activePdfTab, PdfGeneralTab))
                return HelpTopic.PdfGeneral;
            if (ReferenceEquals(activePdfTab, PdfSecurityTab))
                return HelpTopic.PdfSecurity;
            if (ReferenceEquals(activePdfTab, PdfSignatureTab))
                return HelpTopic.PdfSignature;
            if (ReferenceEquals(activePdfTab, PdfCompressionTab))
                return HelpTopic.PdfCompression;

            return HelpTopic.ProfilePdf;
        }

        // This can stay in the code behind, can't easily be done in viewmodel because the textbox is bound to a double
        private void JpegFactorTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var cursorPosition = JpegFactorTextBox.SelectionStart;
            JpegFactorTextBox.Text = JpegFactorTextBox.Text.Replace(',', '.');
            JpegFactorTextBox.SelectionStart = cursorPosition;
        }
    }
}