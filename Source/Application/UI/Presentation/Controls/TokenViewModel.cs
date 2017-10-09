using Optional;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ServiceLocator;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Misc;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation.Controls
{
    public class TokenViewModel : ObservableObject, ITranslatableViewModel<TokenHintPanelTranslation>
    {
        private readonly Func<string, string> _generatePreview;
        private readonly Func<string> _getProperty;

        private readonly Action<string> _setProperty;
        private string _preview;

        public TokenViewModel(Action<string> set, Func<string> get, IList<string> tokens, Func<string, string> generatePreview, Func<string, Option<string>> buttonCommandFunction = null)
        {
            if (RestrictedServiceLocator.IsLocationProviderSet)
            {
                var translationUpdater = RestrictedServiceLocator.Current.GetInstance<ITranslationUpdater>();
                translationUpdater.RegisterAndSetTranslation(this);
            }

            ButtonCommandExecute = buttonCommandFunction;
            ButtonCommand = new DelegateCommand(ExecuteButtonCommand);
            _setProperty = set;
            _getProperty = get;
            _generatePreview = generatePreview;

            Tokens = new ObservableCollection<TokenWithCommand>();
            foreach (var token in tokens)
                Tokens.Add(new TokenWithCommand(token, ExecuteMethod));
            RaiseTextChanged();
        }

        public Func<string, Option<string>> ButtonCommandExecute { get; }
        public ICommand ButtonCommand { get; }

        public bool ShowButton => ButtonCommandExecute != null;

        public string Preview
        {
            get { return _preview; }
            set
            {
                _preview = value;
                RaisePropertyChanged(nameof(Preview));
            }
        }

        public ObservableCollection<TokenWithCommand> Tokens { get; set; }

        public int CurrentCursorPos { get; set; }

        public string Text
        {
            get { return _getProperty(); }
            set
            {
                _setProperty(value);
                OnTextChanged?.Invoke(this, EventArgs.Empty);
                GeneratePreviewText();
            }
        }

        private void ExecuteButtonCommand(object o)
        {
            if (ButtonCommandExecute == null)
                return;

            var result = ButtonCommandExecute(Text);
            result.MatchSome(s => Text = s);
            CurrentCursorPos = Text.Length;
            RaiseTextChanged();
            RaisePropertyChanged(nameof(CurrentCursorPos));
        }

        private void GeneratePreviewText()
        {
            Preview = _generatePreview(Text);
        }

        private void ExecuteMethod(string text)
        {
            var newSelectionStart = CurrentCursorPos + text.Length;
            Text = Text.Insert(CurrentCursorPos, text);

            RaisePropertyChanged(nameof(Text));

            CurrentCursorPos = newSelectionStart;
            RaisePropertyChanged(nameof(CurrentCursorPos));
        }

        public event EventHandler OnTextChanged;

        public void RaiseTextChanged()
        {
            RaisePropertyChanged(nameof(Text));
            GeneratePreviewText();
        }

        public TokenHintPanelTranslation Translation { get; set; }
    }

    public class TokenWithCommand
    {
        public TokenWithCommand(string name, Action<string> execute)
        {
            Name = name;
            MyCommand = new DelegateCommand(o => execute(Name));
        }

        public string Name { get; set; }
        public DelegateCommand MyCommand { get; set; }
    }
}
