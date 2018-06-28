using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using System.Linq;

namespace pdfforge.PDFCreator.Conversion.Actions.Actions
{
    public abstract class RetypePasswordActionBase : IAction, ICheckable
    {
        protected abstract ActionResult DoActionProcessing(Job job);

        //todo: Test this
        public ActionResult ProcessJob(Job job)
        {
            ApplyPreSpecifiedTokens(job);
            var actionResult = Check(job.Profile, job.Accounts, CheckLevel.Job);
            if (!actionResult)
                return actionResult;

            actionResult = DoActionProcessing(job);

            while (actionResult.Contains(ErrorCode.PasswordAction_Login_Error))
            {
                LoginQueryResult? abortReason = null;

                job.OnErrorDuringLogin(password => SetPassword(job, password),
                                        reason => abortReason = reason, PasswordText);

                if (abortReason != null)
                {
                    if (abortReason == LoginQueryResult.AbortedByUser)
                    {
                        // if the user decides to abort, we don't want to see this as error. If there is no way to requery (i.e. during autosave), we want to keep this error!
                        actionResult.Remove(actionResult.First(code => code == ErrorCode.PasswordAction_Login_Error));
                    }

                    break;
                }

                actionResult = DoActionProcessing(job);
            }

            return actionResult;
        }

        public abstract bool IsEnabled(ConversionProfile profile);

        protected abstract void SetPassword(Job job, string password);

        protected abstract string PasswordText { get; }

        public abstract void ApplyPreSpecifiedTokens(Job job);

        public abstract ActionResult Check(ConversionProfile profile, Accounts accounts, CheckLevel checkLevel);
    }
}
