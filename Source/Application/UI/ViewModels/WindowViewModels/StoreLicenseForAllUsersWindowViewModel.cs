using System.Windows;
using System.Windows.Input;
using pdfforge.Obsidian;
using pdfforge.Obsidian.Interaction;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.ViewModels.Assistants;
using pdfforge.PDFCreator.UI.ViewModels.WindowViewModels.Translations;
using pdfforge.PDFCreator.Utilities;

namespace pdfforge.PDFCreator.UI.ViewModels.WindowViewModels
{
    public class StoreLicenseForAllUsersWindowViewModel : InteractionAwareViewModelBase<StoreLicenseForAllUsersInteraction>
    {
        private readonly IOsHelper _osHelper;
        private readonly IUacAssistant _uacAssistant;
        private readonly IInteractionInvoker _interactionInvoker;
        public StoreLicenseForAllUsersWindowTranslation Translation { get;}
        public ICommand StoreLicenseInLmCommand { get; }

        public string ProductName { get; }

        public Visibility RequiresUacVisibility
        {
            get { return _osHelper.UserIsAdministrator() ? Visibility.Collapsed : Visibility.Visible; }
        }

        public StoreLicenseForAllUsersWindowViewModel(ApplicationNameProvider applicationNameProvider, IOsHelper osHelper, IUacAssistant uacAssistant, IInteractionInvoker interactionInvoker, StoreLicenseForAllUsersWindowTranslation translation)
        {
            _osHelper = osHelper;
            _uacAssistant = uacAssistant;
            _interactionInvoker = interactionInvoker;
            Translation = translation;
            ProductName = applicationNameProvider.ApplicationName;

            StoreLicenseInLmCommand = new DelegateCommand(StoreLicenseInLmCommandExecute);
        }

        private void StoreLicenseInLmCommandExecute(object obj)
        {
            var success = _uacAssistant.StoreLicenseForAllUsers(Interaction.LicenseServerCode, Interaction.LicenseKey);
            if (success)
            {
                var title = ProductName;
                var text = Translation.StoreForAllUsersSuccessful;
                var interaction = new MessageInteraction(text, title, MessageOptions.OK, MessageIcon.PDFCreator);
                _interactionInvoker.Invoke(interaction);
            }
            else
            {
                var title = ProductName;
                var text = Translation.StoreForAllUsersFailed;
                var interaction = new MessageInteraction(text, title, MessageOptions.OK, MessageIcon.Error);
                _interactionInvoker.Invoke(interaction);
            }
            FinishInteraction();
        }
    }
}
