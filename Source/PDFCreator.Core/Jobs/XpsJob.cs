using System;
using System.IO;
using System.Threading;
using pdfforge.PDFCreator.Core.Settings;
using pdfforge.PDFCreator.Core.Xps;
using pdfforge.PDFCreator.Core.Xps.OutputFileMover;

namespace pdfforge.PDFCreator.Core.Jobs
{
    internal class XpsJob : AbstractJob
    {
        public XpsJob(IJobInfo jobInfo, ConversionProfile profile, JobTranslations jobTranslations) 
            : base(jobInfo, profile, jobTranslations)
        {
            JobTempFolder = Path.Combine(Path.Combine(Path.GetTempPath(), "PDFCreator\\Temp"),
                "Job_" + Path.GetFileNameWithoutExtension(Path.GetRandomFileName()));
            JobTempOutputFolder = Path.Combine(JobTempFolder, "tempoutput");
            Directory.CreateDirectory(JobTempFolder);
            Directory.CreateDirectory(JobTempOutputFolder);
        }

        public override event EventHandler OnRecommendPdfArchitect;
        public override event EventHandler<QueryPasswordEventArgs> OnRetypeSmtpPassword;

        protected override JobState RunJobWork()
        {
            SetThreadName();

            OutputFiles.Clear();
            SetUpActions();

            var converter = new XpsConverter(JobInfo);
            var path = Path.Combine(JobTempOutputFolder, JobTempFileName + Path.GetExtension(OutputFilenameTemplate));
            converter.Convert(path);


            //PDFProcessor.process

            MoveOutputFile(path);

            JobState = JobState.Succeeded;
            return JobState;
        }

        private void DummyEventUsage()
        {
            OnRecommendPdfArchitect?.Invoke(null, null);
            OnRetypeSmtpPassword?.Invoke(null, null);
        }

        private void MoveOutputFile(string file)
        {
            var fileMover = new SingleFileMover();

            if (Profile.AutoSave.Enabled && Profile.AutoSave.EnsureUniqueFilenames)
                fileMover.UniqueFileNameEnabled = true;

            var movedFile = fileMover.MoveSingleOutputFile(file, OutputFilenameTemplate);
            OutputFiles.Add(movedFile);

        }

        private void SetUpActions()
        {
            // TODO 
        }

        private void SetThreadName()
        {
            try
            {
                if (String.IsNullOrEmpty(Thread.CurrentThread.Name))
                    Thread.CurrentThread.Name = "JobWorker";
            }
            catch (InvalidOperationException)
            {
            }
        }
    }
}