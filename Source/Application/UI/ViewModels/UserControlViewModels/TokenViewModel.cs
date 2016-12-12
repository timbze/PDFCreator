using System;
using System.Collections.Generic;
using pdfforge.Obsidian;

namespace pdfforge.PDFCreator.UI.ViewModels.UserControlViewModels
{
    public class TokenViewModel : ObservableObject
    {
        private readonly Func<string> _getProperty;

        private readonly Action<string> _setProperty;

        public TokenViewModel(Action<string> set, Func<string> get, IList<string> tokens)
        {
            _setProperty = set;
            _getProperty = get;
            TokenList = tokens;
        }

        public IList<string> TokenList { get; set; }

        public string SelectedToken { get; set; }

        public int CurrentCursorPos { get; set; }

        public string Text
        {
            get { return _getProperty(); }
            set
            {
                _setProperty(value);
                OnTextChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public DelegateCommand OnSelectionChangedCommand => new DelegateCommand(OnSelectionChangedExecute);

        public event EventHandler OnTextChanged;

        private void OnSelectionChangedExecute(object obj)
        {
            var newSelectionStart = CurrentCursorPos + SelectedToken.Length;
            Text = Text.Insert(CurrentCursorPos, SelectedToken);

            RaisePropertyChanged(nameof(Text));

            CurrentCursorPos = newSelectionStart;
            RaisePropertyChanged(nameof(CurrentCursorPos));
        }

        public void RaiseTextChanged()
        {
            RaisePropertyChanged(nameof(Text));
        }
    }
}