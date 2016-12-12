using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using pdfforge.DynamicTranslator;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.UI.ViewModels.Helper;

namespace pdfforge.PDFCreator.UI.ViewModels.UserControlViewModels.ApplicationSettings
{
    public class TitleTabViewModel : ObservableObject
    {
        private SynchronizedCollection<TitleReplacement> _titleReplacements;

        public TitleTabViewModel(ITranslator translator)
        {
            Translator = translator;
            TitleAddCommand = new DelegateCommand(TitleAddCommandExecute);
            TitleDeleteCommand = new DelegateCommand(TitleDeleteCommandExecute, TitleDeleteCommandCanExecute);
            ReplacementValues = translator.GetEnumTranslation<ReplacementType>().ToList();
        }

        public ITranslator Translator { get; }

        public ICollectionView TitleReplacementView { get; private set; }

        public DelegateCommand TitleAddCommand { get; set; }
        public DelegateCommand TitleDeleteCommand { get; set; }

        public IList<EnumValue<ReplacementType>> ReplacementValues { get; }

        public ObservableCollection<TitleReplacement> TitleReplacements
        {
            get { return _titleReplacements?.ObservableCollection; }
        }

        public void ApplyTitleReplacements(IList<TitleReplacement> titleReplacements)
        {
            if (TitleReplacements != null)
            {
                TitleReplacements.CollectionChanged -= TitleReplacementsOnCollectionChanged;
            }

            if (titleReplacements != null)
            {
                var replacements = titleReplacements;

                _titleReplacements = new SynchronizedCollection<TitleReplacement>(replacements);
                _titleReplacements.ObservableCollection.CollectionChanged += TitleReplacementsOnCollectionChanged;
                TitleReplacementView = CollectionViewSource.GetDefaultView(_titleReplacements.ObservableCollection);
                TitleReplacementView.SortDescriptions.Add(new SortDescription(nameof(TitleReplacement.ReplacementType), ListSortDirection.Descending));
                TitleReplacementView.SortDescriptions.Add(new SortDescription(nameof(TitleReplacement.Search), ListSortDirection.Descending));
                TitleReplacementView.CurrentChanged += TitleReplacement_OnCurrentChanged;
            }

            RaisePropertyChanged(nameof(TitleReplacements));
            RaiseTitleCommandsCanExecuteChanged();
        }

        private void RaiseTitleCommandsCanExecuteChanged()
        {
            TitleDeleteCommand.RaiseCanExecuteChanged();
        }

        private void TitleReplacement_OnCurrentChanged(object sender, EventArgs eventArgs)
        {
            RaiseTitleCommandsCanExecuteChanged();
        }

        private bool TitleDeleteCommandCanExecute(object obj)
        {
            if (TitleReplacementView?.CurrentItem == null || TitleReplacementView.CurrentItem == CollectionView.NewItemPlaceholder)
                return false;

            return true;
        }

        private void TitleDeleteCommandExecute(object obj)
        {
            TitleReplacements.Remove(TitleReplacementView.CurrentItem as TitleReplacement);
        }

        private void TitleAddCommandExecute(object obj)
        {
            var newItem = new TitleReplacement();
            TitleReplacements.Add(newItem);
            TitleReplacementView.MoveCurrentTo(newItem);
        }

        private void TitleReplacementsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RaiseTitleCommandsCanExecuteChanged();
        }
    }
}