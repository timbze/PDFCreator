using NLog;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using System;
using System.Collections.Generic;
using System.Linq;

namespace pdfforge.PDFCreator.Core.Workflow
{
    public interface IJobBuilder
    {
        Job BuildJobFromJobInfo(JobInfo jobInfo, PdfCreatorSettings settings);
    }

    public abstract class JobBuilder : IJobBuilder
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IMailSignatureHelper _mailSignatureHelper;

        public JobBuilder(IMailSignatureHelper mailSignatureHelper)
        {
            _mailSignatureHelper = mailSignatureHelper;
        }

        public abstract Job SkipPrintDialog(Job job);

        public Job BuildJobFromJobInfo(JobInfo jobInfo, PdfCreatorSettings settings)
        {
            _logger.Trace("Building Job from JobInfo");

            var preselectedProfile = PreselectedProfile(jobInfo, settings).Copy();

            _logger.Debug("Profile: {0} (GUID {1})", preselectedProfile.Name, preselectedProfile.Guid);

            var jobTranslations = new JobTranslations { EmailSignature = _mailSignatureHelper.ComposeMailSignature() };

            var job = new Job(jobInfo, preselectedProfile, jobTranslations, settings.ApplicationSettings.Accounts);

            SkipPrintDialog(job);

            return job;
        }

        /// <summary>
        ///     Determines the preselected profile for the printer that was used while creating the job
        /// </summary>
        /// <param name="jobInfo">The jobinfo used for the decision</param>
        /// <param name="settings">The settings used for the decision</param>
        /// <returns>The profile that is associated with the printer or the default profile</returns>
        private ConversionProfile PreselectedProfile(JobInfo jobInfo, PdfCreatorSettings settings)
        {
            foreach (var mapping in settings.ApplicationSettings.PrinterMappings)
            {
                if (mapping.PrinterName.Equals(jobInfo.SourceFiles[0].PrinterName, StringComparison.OrdinalIgnoreCase))
                {
                    var p = settings.GetProfileByGuid(mapping.ProfileGuid);
                    if (p != null)
                        return p;
                    break;
                }
            }

            var lastUsedProfile = settings.GetLastUsedProfile();
            if (lastUsedProfile != null)
                return lastUsedProfile;

            // try default profile
            var defaultProfile = GetDefaultProfile(settings.ConversionProfiles);
            if (defaultProfile != null)
                return defaultProfile;

            // last resort: first profile from the list
            return settings.ConversionProfiles[0];
        }

        private ConversionProfile GetDefaultProfile(IList<ConversionProfile> conversionProfiles)
        {
            return conversionProfiles.FirstOrDefault(p => p.IsDefault);
        }
    }

    public class JobBuilderFree : JobBuilder
    {
        public JobBuilderFree(IMailSignatureHelper mailSignatureHelper) : base(mailSignatureHelper)
        {
        }

        public override Job SkipPrintDialog(Job job)
        {
            job.Profile.SkipPrintDialog = false;
            return job;
        }
    }

    public class JobBuilderPlus : JobBuilder
    {
        public JobBuilderPlus(IMailSignatureHelper mailSignatureHelper) : base(mailSignatureHelper)
        {
        }

        public override Job SkipPrintDialog(Job job)
        {
            return job;
        }
    }
}
