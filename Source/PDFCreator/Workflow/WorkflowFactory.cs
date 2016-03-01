using System;
using NLog;
using pdfforge.PDFCreator.Core.Jobs;
using pdfforge.PDFCreator.Core.Settings;
using pdfforge.PDFCreator.Helper;
using pdfforge.PDFCreator.Shared.Helper;

namespace pdfforge.PDFCreator.Workflow
{
    internal static class WorkflowFactory
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        
        /// <summary>
        /// Create a workflow based on the job and settings objects provided. This will create an AutoSave workflow if the job's printer has an AutoSave profile associated or the default profile uses AutoSave.
        /// Otherwise, an interactive workflow will be created.
        /// </summary>
        /// <param name="jobInfo">The jobinfo used for the decision</param>
        /// <param name="settings">The settings used for the decision</param>
        /// <returns>A ConversionWorkflow either for AutoSave or interactive use</returns>
        public static ConversionWorkflow CreateWorkflow(IJobInfo jobInfo, PdfCreatorSettings settings)
        {
            Logger.Trace("Creating Workflow");

            ConversionProfile preselectedProfile = PreselectedProfile(jobInfo, settings).Copy();

            Logger.Debug("Profile: {0} (GUID {1})", preselectedProfile.Name, preselectedProfile.Guid);

            var jobTranslations = new JobTranslations();
            jobTranslations.EmailSignature = MailSignatureHelper.ComposeMailSignature();

            var job = JobFactory.CreateJob(jobInfo, preselectedProfile, JobInfoQueue.Instance, jobTranslations);
            job.AutoCleanUp = true;

            if (preselectedProfile.AutoSave.Enabled)
            {
                Logger.Trace("Creating AutoSaveWorkflow");
                return new AutoSaveWorkflow(job, settings);
            }
            else
            {
                Logger.Trace("Creating InteractiveWorkflow");
                return new InteractiveWorkflow(job, settings);
            }
        }

        /// <summary>
        /// Determines the preselected profile for the printer that was used while creating the job
        /// </summary>
        /// <param name="jobInfo">The jobinfo used for the decision</param>
        /// <param name="settings">The settings used for the decision</param>
        /// <returns>The profile that is associated with the printer or the default profile</returns>
        private static ConversionProfile PreselectedProfile(IJobInfo jobInfo, PdfCreatorSettings settings)
        {
            foreach (var mapping in settings.ApplicationSettings.PrinterMappings)
            {
                if (mapping.PrinterName.Equals(jobInfo.SourceFiles[0].PrinterName, StringComparison.OrdinalIgnoreCase))
                {
                    ConversionProfile p = settings.GetProfileByGuid(mapping.ProfileGuid);
                    if (p != null)
                        return p;
                    break;
                }
            }

            ConversionProfile lastUsedProfile = settings.GetLastUsedProfile();
            if (lastUsedProfile != null)
                return lastUsedProfile;

            // try default profile
            ConversionProfile defaultProfile = SettingsHelper.GetDefaultProfile(settings.ConversionProfiles);
            if (defaultProfile != null)
                return defaultProfile;

            // last resort: first profile from the list
            return settings.ConversionProfiles[0];
        }
    }
}
