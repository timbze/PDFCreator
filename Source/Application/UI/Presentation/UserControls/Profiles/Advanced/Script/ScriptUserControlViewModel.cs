using Optional;
using pdfforge.PDFCreator.Conversion.Actions;
using pdfforge.PDFCreator.UI.Presentation.Controls;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.Utilities.Threading;
using pdfforge.PDFCreator.Utilities.Tokens;
using System;
using System.ComponentModel;
using System.IO;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Advanced.Script
{
    public class ScriptUserControlViewModel : ProfileUserControlViewModel<ScriptTranslation>
    {
        private readonly IOpenFileInteractionHelper _openFileInteractionHelper;
        private readonly IScriptActionHelper _scriptActionHelper;

        public ScriptUserControlViewModel(ITranslationUpdater translationUpdater, ISelectedProfileProvider provider, IOpenFileInteractionHelper openFileInteractionHelper, IScriptActionHelper scriptActionHelper, TokenHelper tokenHelper)
            : base(translationUpdater, provider)
        {
            _openFileInteractionHelper = openFileInteractionHelper;
            _scriptActionHelper = scriptActionHelper;

            if (tokenHelper != null)
            {
                TokenReplacer = tokenHelper.TokenReplacerWithPlaceHolders;
                var tokens = tokenHelper.GetTokenListWithFormatting();
                ParameterTokenViewModel = new TokenViewModel(x => CurrentProfile.Scripting.ParameterString = x, () => CurrentProfile?.Scripting.ParameterString, tokens, ReplaceTokens);
                ScriptFileTokenViewModel = new TokenViewModel(x => CurrentProfile.Scripting.ScriptFile = x, () => CurrentProfile?.Scripting.ScriptFile, tokens, ReplaceTokens, SelectScriptFileAction);
                ParameterTokenViewModel.OnTextChanged += TokenTextChanged;
                ScriptFileTokenViewModel.OnTextChanged += TokenTextChanged;
                TokenTextChanged(this, EventArgs.Empty);
            }
        }

        private Option<string> SelectScriptFileAction(string s1)
        {
            var title = Translation.SelectScriptTitle;
            var filter = Translation.ExecutableFiles
                         + @" (*.exe, *.bat, *.cmd)|*.exe;*.bat;*.cmd|"
                         + Translation.AllFiles
                         + @"(*.*)|*.*";

            var interactionResult = _openFileInteractionHelper.StartOpenFileInteraction(CurrentProfile.Scripting.ScriptFile, title, filter);
            interactionResult.MatchSome(s =>
            {
                ScriptFileTokenViewModel.Text = s;
                ScriptFileTokenViewModel.RaiseTextChanged();
            });

            return interactionResult;
        }

        public string PreviewScriptCall { get; set; }

        public TokenViewModel ScriptFileTokenViewModel { get; set; }

        public TokenViewModel ParameterTokenViewModel { get; set; }

        public TokenReplacer TokenReplacer { get; }

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
                scriptCall += " " + _scriptActionHelper.ComposeScriptParameters(additionalParams, new[] { @"C:\File1.pdf", @"C:\File2.pdf" }, TokenReplacer);
            }
            else
            {
                scriptCall += @" C:\File1.pdf C:\File2.pdf";
            }

            return scriptCall;
        }

        protected override void OnCurrentProfileChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            base.OnCurrentProfileChanged(sender, propertyChangedEventArgs);

            ParameterTokenViewModel.RaiseTextChanged();
            ScriptFileTokenViewModel.RaiseTextChanged();
            TokenTextChanged(this, EventArgs.Empty);
        }

        private string ReplaceTokens(string s)
        {
            if (s != null)
            {
                return TokenReplacer.ReplaceTokens(s);
            }
            return string.Empty;
        }
    }

    public class DesignTimeScriptUserControlViewModel : ScriptUserControlViewModel
    {
        public DesignTimeScriptUserControlViewModel() : base(new TranslationUpdater(new TranslationFactory(), new ThreadManager()), new DesignTimeCurrentSettingsProvider(), null, null, null)
        {
        }
    }
}
