using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Navigation;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.UI.ViewModels.DialogViewModels;

namespace pdfforge.PDFCreator.UI.Views.Dialogs
{
    public partial class DropboxSharedLinksWindow : Window
    {
        private readonly DropboxSharedLinksViewModel _viewModel;

        public DropboxSharedLinksWindow(DropboxSharedLinksViewModel viewModel)
        {
            _viewModel = viewModel;
            DataContext = viewModel;
            InitializeComponent();
        }
  }
}