using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using pdfforge.DynamicTranslator;
using pdfforge.PDFCreator.UI.ViewModels.ActionViewModels;

namespace pdfforge.PDFCreator.UI.Views.ActionControls
{
    /// <summary>
    /// Interaction logic for DropboxActionControl.xaml
    /// </summary>
    public partial class DropboxActionControl : ActionControl
    {
        public DropboxActionControl(DropboxActionViewModel dropboxActionViewModel, ITranslator translator) : base(dropboxActionViewModel)
        {
            InitializeComponent();
            translator.Translate(this);
        }
    }
}
