using NLog;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.SettingsManagement;
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
        private readonly IParametersManager _parametersManager;

        public JobBuilder(IMailSignatureHelper mailSignatureHelper, IParametersManager parametersManager)
        {
            _mailSignatureHelper = mailSignatureHelper;
            _parametersManager = parametersManager;
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
            ConversionProfile profile = null;
            foreach (var mapping in settings.ApplicationSettings.PrinterMappings)
            {
                if (mapping.PrinterName.Equals(jobInfo.SourceFiles[0].PrinterName, StringComparison.OrdinalIgnoreCase))
                {
                    profile = settings.GetProfileByGuid(mapping.ProfileGuid);
                    /* TODO (see below):
                    if(mapping.ProfileGuid == LASTUSEDPROFILEGUID)
                        profile = settings.GetLastUsedProfile();
                    */
                    if (profile != null)
                        break;
                }
            }

            //consider LastUsedProfile
            //todo: Do not relate to empty string and use own GUID to request LastUsedProfile (see above)
            if (jobInfo.SourceFiles.Count > 0 && string.IsNullOrEmpty(jobInfo.SourceFiles[0].PrinterName))
            {
                var lastUsedProfile = settings.GetLastUsedProfile();
                if (lastUsedProfile != null)
                    profile = lastUsedProfile;
            }

            //Consider commandline paramaters
            if (_parametersManager.HasPredefinedParameters())
            {
                var parameters = _parametersManager.GetAndResetParameters();
                var profileParameter = parameters.Profile;
                if (!string.IsNullOrEmpty(profileParameter))
                {
                    profile = settings.GetProfileByName(profileParameter);
                    if (profile == null)
                        profile = settings.GetProfileByGuid(profileParameter);
                }

                var outputFile = parameters.Outputfile;
                if (outputFile != null)
                    jobInfo.OutputFileParameter = outputFile;
            }

            if (profile != null)
                return profile;

            // try default profile
            var defaultProfile = GetDefaultProfile(settings.ConversionProfiles);
            if (defaultProfile != null)
                return defaultProfile;

            // last resort: first profile from the list
            return settings.ConversionProfiles[0];
        }

        private ConversionProfile GetDefaultProfile(IList<ConversionProfile> conversionProfiles)
        {
            return conversionProfiles.FirstOrDefault(p => p.Guid == ProfileGuids.DEFAULT_PROFILE_GUID);
        }
    }

    public class JobBuilderFree : JobBuilder
    {
        public JobBuilderFree(IMailSignatureHelper mailSignatureHelper, IParametersManager parametersManager) : base(mailSignatureHelper, parametersManager)
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
        public JobBuilderPlus(IMailSignatureHelper mailSignatureHelper, IParametersManager parametersManager) : base(mailSignatureHelper, parametersManager)
        {
        }

        public override Job SkipPrintDialog(Job job)
        {
            return job;
        }
    }
}
