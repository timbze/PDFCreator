using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using pdfforge.PDFCreator.Helper;
using pdfforge.PDFCreator.Shared.Helper;
using pdfforge.PDFCreator.ViewModels;

namespace pdfforge.PDFCreator.Views
{
    internal partial class ManagePrintJobsWindow : Window
    {
        public ManagePrintJobsWindow()
        {
            InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            TranslationHelper.Instance.TranslatorInstance.Translate(this);
            var view = (GridView) JobList.View;
            view.Columns[0].Header = TranslationHelper.Instance.TranslatorInstance.GetTranslation("ManagePrintJobsWindow", "TitleColoumn", "Title");
            view.Columns[1].Header = TranslationHelper.Instance.TranslatorInstance.GetTranslation("ManagePrintJobsWindow", "FilesColoumn", "Files");
            view.Columns[2].Header = TranslationHelper.Instance.TranslatorInstance.GetTranslation("ManagePrintJobsWindow", "PagesColoumn", "Pages");
        }

        private void OnDragEnter(object sender, DragEventArgs e)
        {
            DragAndDropHelper.OnDragEnter(e);
        }

        private void OnDrop(object sender, DragEventArgs e)
        {
            DragAndDropHelper.OnDrop(e);
        }

        private void JobList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var vm = (ManagePrintJobsViewModel)DataContext;
            vm.DeleteJobCommand.RaiseCanExecuteChanged();
            vm.MergeJobsCommand.RaiseCanExecuteChanged();
        }

        private void OnActivated(object sender, EventArgs e)
        {
            ((ManagePrintJobsViewModel)DataContext).RaiseRefreshView();
        }

        private void ManagePrintJobsWindow_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }
    }
}
