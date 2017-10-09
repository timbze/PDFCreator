using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.UI.Presentation.Help;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using System;

namespace pdfforge.PDFCreator.UI.Presentation.Commands.UserGuide
{
    public class ShowUserGuideCommand : IInitializedCommand<HelpTopic>
    {
        private readonly IUserGuideHelper _userGuildeHelper;
        private HelpTopic _helpTopic = HelpTopic.General;

        public ShowUserGuideCommand(IUserGuideHelper userGuildeHelper)
        {
            _userGuildeHelper = userGuildeHelper;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if (parameter is HelpTopic)
            {
                _userGuildeHelper.ShowHelp((HelpTopic)parameter);
            }
            else
            {
                _userGuildeHelper.ShowHelp(_helpTopic);
            }
        }

        public void Init(HelpTopic parameter)
        {
            _helpTopic = parameter;
        }

#pragma warning disable CS0067

        public event EventHandler CanExecuteChanged;

#pragma warning restore CS0067
    }
}
