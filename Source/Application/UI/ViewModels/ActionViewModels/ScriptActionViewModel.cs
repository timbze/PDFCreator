using System;
using System.Collections.Generic;
using System.IO;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Actions;
using pdfforge.PDFCreator.UI.ViewModels.ActionViewModels.Translations;
using pdfforge.PDFCreator.UI.ViewModels.Helper;
using pdfforge.PDFCreator.UI.ViewModels.UserControlViewModels;
using pdfforge.PDFCreator.Utilities.Tokens;

namespace pdfforge.PDFCreator.UI.ViewModels.ActionViewModels
{
    public class ScriptActionViewModel : ActionViewModel
    {
        private readonly IOpenFileInteractionHelper _openFileInteractionHelper;
        private readonly IScriptActionHelper _scriptActionHelper;

        public ScriptActionViewModel(ScriptActionSettingsAndActionTranslation translation, IOpenFileInteractionHelper openFileInteractionHelper, IScriptActionHelper scriptActionHelper, TokenHelper tokenHelper)
        {
            _openFileInteractionHelper = openFileInteractionHelper;
            _scriptActionHelper = scriptActionHelper;
            Translation = translation;

            TokenReplacer = tokenHelper.TokenReplacerWithPlaceHolders;

            DisplayName = Translation.DisplayName;
            Description = Translation.Description;

            var tokens = new List<string>(TokenReplacer.GetTokenNames(true));
            ParameterTokenViewModel = new TokenViewModel(x => CurrentProfile.Scripting.ParameterString = x, () => CurrentProfile?.Scripting.ParameterString, tokens);
            ScriptFileTokenViewModel = new TokenViewModel(x => CurrentProfile.Scripting.ScriptFile = x, () => CurrentProfile?.Scripting.ScriptFile, tokens);
            ParameterTokenViewModel.OnTextChanged += TokenTextChanged;
            ScriptFileTokenViewModel.OnTextChanged += TokenTextChanged;
        }

        public string PreviewScriptCall { get; set; }

        public TokenViewModel ScriptFileTokenViewModel { get; set; }

        public TokenViewModel ParameterTokenViewModel { get; set; }

        public ScriptActionSettingsAndActionTranslation Translation { get; }

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
            var title = Translation.SelectScriptTitle;
            var filter = Translation.ExecutableFiles
                         + @" (*.exe, *.bat, *.cmd)|*.exe;*.bat;*.cmd|"
                         + Translation.AllFiles
                         + @"(*.*)|*.*";

            var result = _openFileInteractionHelper.StartOpenFileInteraction(CurrentProfile.Scripting.ScriptFile, title, filter);
            ScriptFileTokenViewModel.Text = result;
            ScriptFileTokenViewModel.RaiseTextChanged();
        }
    }
}