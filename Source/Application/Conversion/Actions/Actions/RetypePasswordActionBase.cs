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
            var actionResult = Check(job.Profile, job.Accounts);
            if (!actionResult)
                return actionResult;

            actionResult = DoActionProcessing(job);

            while (actionResult.Contains(ErrorCode.PasswordAction_Login_Error))
            {
                var aborted = false;

                job.OnErrorDuringLogin(password => SetPassword(job, password),
                                        () => aborted = true, PasswordText);

                if (aborted)
                {
                    //todo: This makes the return succesfull. Do we really want this?
                    actionResult.Remove(actionResult.First(code => code == ErrorCode.PasswordAction_Login_Error));
                    break;
                }

                actionResult = DoActionProcessing(job);
            }

            return actionResult;
        }

        public abstract bool IsEnabled(ConversionProfile profile);

        protected abstract void SetPassword(Job job, string password);

        protected abstract string PasswordText { get; }

        public abstract ActionResult Check(ConversionProfile profile, Accounts accounts);
    }
}
