using pdfforge.Obsidian;
using pdfforge.Obsidian.Interaction.DialogInteractions;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using System.ComponentModel;
using System.Drawing;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyTab.Stamp
{
    public class StampUserControlViewModel : ProfileUserControlViewModel<DocumentTabTranslation>
    {
        private readonly IFontHelper _fontHelper;
        private readonly IInteractionInvoker _interactionInvoker;

        public StampUserControlViewModel(IInteractionInvoker interactionInvoker, IFontHelper fontHelper, TokenHelper tokenHelper, ITranslationUpdater translationUpdater, ISelectedProfileProvider profile) : base(translationUpdater, profile)
        {
            _interactionInvoker = interactionInvoker;
            _fontHelper = fontHelper;

            if (CurrentProfile != null)
            {
                UpdateFontButtonText(CurrentProfile.Stamping);
            }
        }

        public string FontButtonText { get; set; }

        public DelegateCommand ChooseStampFont => new DelegateCommand(ChooseStampFontExecute);
        public DelegateCommand ChooseStampColor => new DelegateCommand(ChooseStampColorExecute);

        private void ChooseStampColorExecute(object obj)
        {
            var interaction = new ColorInteraction();
            interaction.Color = CurrentProfile.Stamping.Color;

            _interactionInvoker.Invoke(interaction);

            if (!interaction.Success)
                return;

            CurrentProfile.Stamping.Color = interaction.Color;
            RaisePropertyChanged(nameof(CurrentProfile));
        }

        private void ChooseStampFontExecute(object obj)
        {
            var interaction = new FontInteraction();
            interaction.Font = new Font(CurrentProfile.Stamping.FontName, CurrentProfile.Stamping.FontSize);

            _interactionInvoker.Invoke(interaction);
            if (!interaction.Success)
                return;

            var postScriptName = _fontHelper.FindPostScriptName(interaction.Font.Name);

            if (postScriptName == null)
            {
                DisplayFontError();
                return;
            }

            CurrentProfile.Stamping.FontName = interaction.Font.Name;
            CurrentProfile.Stamping.PostScriptFontName = postScriptName;
            CurrentProfile.Stamping.FontSize = interaction.Font.Size;

            UpdateFontButtonText(CurrentProfile.Stamping);
        }

        protected override void OnCurrentProfileChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            base.OnCurrentProfileChanged(sender, propertyChangedEventArgs);
            UpdateFontButtonText(CurrentProfile.Stamping);
        }

        private void UpdateFontButtonText(Stamping stamping)
        {
            var fontSize = stamping.FontSize.ToString();
            var fontstring = $"{stamping.FontName} {fontSize}pt";
            if (fontstring.Length > 25)
            {
                fontstring = stamping.FontName.Substring(0, 25 - 4 - fontSize.Length).TrimEnd();
                fontstring = $"{fontstring}. {fontSize}pt";
            }

            FontButtonText = fontstring;
            RaisePropertyChanged(nameof(FontButtonText));
        }

        private void DisplayFontError()
        {
            var message = Translation.FontFileNotSupported;

            var interaction = new MessageInteraction(message, "PDFCreator", MessageOptions.OK, MessageIcon.Warning);
            _interactionInvoker.Invoke(interaction);
        }
    }

    public class DesignTimeStampUserControlViewModel : StampUserControlViewModel
    {
        public DesignTimeStampUserControlViewModel() : base(null, null, null, new DesignTimeTranslationUpdater(), new DesignTimeCurrentSettingsProvider())
        {
        }
    }
}
