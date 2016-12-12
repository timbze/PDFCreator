using System;
using System.Collections.Generic;
using System.IO;
using pdfforge.DynamicTranslator;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Actions;
using pdfforge.PDFCreator.UI.ViewModels.Helper;
using pdfforge.PDFCreator.UI.ViewModels.UserControlViewModels;
using pdfforge.PDFCreator.Utilities.Tokens;

namespace pdfforge.PDFCreator.UI.ViewModels.ActionViewModels
{
    public class ScriptActionViewModel : ActionViewModel
    {
        private readonly IOpenFileInteractionHelper _openFileInteractionHelper;
        private readonly IScriptActionHelper _scriptActionHelper;

        public ScriptActionViewModel(ITranslator translator, IOpenFileInteractionHelper openFileInteractionHelper, IScriptActionHelper scriptActionHelper)
        {
            _openFileInteractionHelper = openFileInteractionHelper;
            _scriptActionHelper = scriptActionHelper;
            Translator = translator;

            var tokenHelper = new TokenHelper(Translator);
            TokenReplacer = tokenHelper.TokenReplacerWithPlaceHolders;

            DisplayName = Translator.GetTranslation("ScriptActionSettings", "DisplayName");
            Description = Translator.GetTranslation("ScriptActionSettings", "Description");

            var tokens = new List<string>(TokenReplacer.GetTokenNames(true));
            ParameterTokenViewModel = new TokenViewModel(x => CurrentProfile.Scripting.ParameterString = x, () => CurrentProfile?.Scripting.ParameterString, tokens);
            ScriptFileTokenViewModel = new TokenViewModel(x => CurrentProfile.Scripting.ScriptFile = x, () => CurrentProfile?.Scripting.ScriptFile, tokens);
            ParameterTokenViewModel.OnTextChanged += TokenTextChanged;
            ScriptFileTokenViewModel.OnTextChanged += TokenTextChanged;
        }

        public string PreviewScriptCall { get; set; }

        public TokenViewModel ScriptFileTokenViewModel { get; set; }

        public TokenViewModel ParameterTokenViewModel { get; set; }

        public ITranslator Translator { get; }

        public TokenReplacer TokenReplacer { get; }

        public override bool IsEnabled
        {
            get { return CurrentProfile != null && CurrentProfile.Scripting.Enabled; }
            set
            {
                CurrentProfile.Scripting.Enabled = value;
                RaisePropertyChanged(nameof(IsEnabled));
            }
        }

        public DelegateCommand BrowseScriptCommand => new DelegateCommand(BrowseScriptExecute);

        private void TokenTextChanged(object sender, EventArgs eventArgs)
        {
            PreviewScriptCall = ComposeSampleCommand(ScriptFileTokenViewModel.Text, ParameterTokenViewModel.Text);
            RaisePropertyChanged(nameof(PreviewScriptCall));
        }

        private string ComposeSampleCommand(string scriptPath, string additionalParams)
        {
            if (string.IsNullOrEmpty(scriptPath) || (scriptPath.Trim().Length == 0))
                return "";

            var scriptCall = Path.GetFileName(_scriptActionHelper.ComposeScriptPath(scriptPath, TokenReplacer));

            if (!string.IsNullOrEmpty(additionalParams))
            {
                scriptCall += " " + _scriptActionHelper.ComposeScriptParameters(additionalParams, new[] {@"C:\File1.pdf", @"C:\File2.pdf"}, TokenReplacer);
            }
            else
            {
                scriptCall += @" C:\File1.pdf C:\File2.pdf";
            }

            return scriptCall;
        }

        public override HelpTopic GetContextBasedHelpTopic()
        {
            return HelpTopic.RunScript;
        }

        protected override void HandleCurrentProfileChanged()
        {
            ParameterTokenViewModel.RaiseTextChanged();
            ScriptFileTokenViewModel.RaiseTextChanged();
        }

        private void BrowseScriptExecute(object obj)
        {
            var title = Translator.GetTranslation("ScriptActionSettings", "SelectScriptTitle");
            var filter = Translator.GetTranslation("ScriptActionSettings", "ExecutableFiles")
                         + @" (*.exe, *.bat, *.cmd)|*.exe;*.bat;*.cmd|"
                         + Translator.GetTranslation("ScriptActionSettings", "AllFiles")
                         + @"(*.*)|*.*";

            var result = _openFileInteractionHelper.StartOpenFileInteraction(CurrentProfile.Scripting.ScriptFile, title, filter);
            ScriptFileTokenViewModel.Text = result;
            ScriptFileTokenViewModel.RaiseTextChanged();
        }
    }
}