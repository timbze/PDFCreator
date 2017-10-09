using pdfforge.DataStorage;
using pdfforge.DataStorage.Storage;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Text;

namespace pdfforge.PDFCreator.Conversion.Jobs.JobInfo
{
    public interface IJobInfoManager
    {
        /// <summary>
        ///     Creates a JobInfo based on the given Inf file
        /// </summary>
        /// <param name="infFile">full path to the Inf file to use</param>
        JobInfo ReadFromInfFile(string infFile);

        /// <summary>
        ///     Save the inf file to the path from the InfFile property
        /// </summary>
        void SaveToInfFile(JobInfo jobInfo);

        /// <summary>
        ///     Save the inf file to the given path
        ///     <param name="filename">filename where the inf will be stored</param>
        /// </summary>
        void SaveToInfFile(JobInfo jobInfo, string filename);

        /// <summary>
        ///     Deletes the inf file and also the source files
        /// </summary>
        void DeleteInfAndSourceFiles(JobInfo jobInfo);

        /// <summary>
        ///     Merge the second JobInfo into the first one
        /// </summary>
        void Merge(JobInfo jobInfo, JobInfo jobinfoToMerge);
    }

    public class JobInfoManager : IJobInfoManager
    {
        private readonly ITitleReplacerProvider _titleReplacerProvider;

        public JobInfoManager(ITitleReplacerProvider titleReplacerProvider)
        {
            _titleReplacerProvider = titleReplacerProvider;
        }

        public JobInfo ReadFromInfFile(string infFile)
        {
            var titleReplacer = _titleReplacerProvider.BuildTitleReplacer();
            return ReadFromInfFile(infFile, titleReplacer);
        }

        public void SaveToInfFile(JobInfo jobInfo)
        {
            if (string.IsNullOrEmpty(jobInfo.InfFile))
            {
                throw new InvalidOperationException("The inf file must not be empty");
            }

            var infData = Data.CreateDataStorage();
            var ini = new IniStorage(Encoding.GetEncoding("Unicode"));
            ini.SetData(infData);

            var sourceFileReader = new SourceFileInfoDataReader();

            var sectionId = 0;
            foreach (var sourceFileInfo in jobInfo.SourceFiles)
            {
                var section = sectionId.ToString(CultureInfo.InvariantCulture) + "\\";
                sourceFileReader.WriteSourceFileInfoToData(infData, section, sourceFileInfo);
                sectionId++;
            }

            ini.WriteData(jobInfo.InfFile);
        }

        public void SaveToInfFile(JobInfo jobInfo, string filename)
        {
            jobInfo.InfFile = filename;
            SaveToInfFile(jobInfo);
        }

        public void DeleteInfAndSourceFiles(JobInfo jobInfo)
        {
            foreach (var sourceFileInfo in jobInfo.SourceFiles)
            {
                try
                {
                    File.Delete(sourceFileInfo.Filename);
                }
                catch (IOException)
                {
                }
            }
            jobInfo.SourceFiles.Clear();
            DeleteInf(jobInfo);
        }

        public void Merge(JobInfo jobInfo, JobInfo jobinfoToMerge)
        {
            if (jobInfo.JobType != jobinfoToMerge.JobType)
                return;

            foreach (var sourceFile in jobinfoToMerge.SourceFiles)
            {
                jobInfo.SourceFiles.Add(sourceFile);
            }

            DeleteInf(jobinfoToMerge);
        }

        private void DeleteInf(JobInfo jobInfo)
        {
            try
            {
                if (!string.IsNullOrEmpty(jobInfo.InfFile))
                    File.Delete(jobInfo.InfFile);
            }
            catch (IOException)
            {
            }
            jobInfo.InfFile = null;
        }

        private JobInfo ReadFromInfFile(string infFile, TitleReplacer titleReplacer)
        {
            var jobInfo = new JobInfo();

            jobInfo.InfFile = infFile;
            var infData = Data.CreateDataStorage();
            var ini = new IniStorage(Encoding.GetEncoding("Unicode"));
            ini.SetData(infData);
            ini.ReadData(infFile);

            var sourceFiles = new ObservableCollection<SourceFileInfo>();
            var sourceFileReader = new SourceFileInfoDataReader();
            foreach (var section in infData.GetSections())
            {
                var sfi = sourceFileReader.ReadSourceFileInfoFromData(infFile, infData, section);
                if (sfi != null)
                    sourceFiles.Add(sfi);
            }
            jobInfo.SourceFiles = sourceFiles;

            var metadata = new Metadata();
            if (sourceFiles.Count > 0)
            {
                metadata.PrintJobAuthor = sourceFiles[0].Author;
                metadata.PrintJobName = titleReplacer.Replace(sourceFiles[0].DocumentTitle);

                jobInfo.JobType = sourceFiles[0].Type;
            }
            jobInfo.Metadata = metadata;

            jobInfo.PrintDateTime = File.GetCreationTime(infFile);

            return jobInfo;
        }
    }
}
