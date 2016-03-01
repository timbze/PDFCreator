using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SystemInterface.IO;
using SystemWrapper.IO;
using NLog;
using NLog.Config;
using NLog.Targets;
using NUnit.Framework;
using pdfforge.PDFCreator.Core.Ghostscript;
using pdfforge.PDFCreator.Core.Jobs;
using pdfforge.PDFCreator.Core.Settings;
using pdfforge.PDFCreator.Core.Settings.Enums;
using PDFCreator.TestUtilities.Properties;
using Rhino.Mocks;

namespace PDFCreator.TestUtilities
{    
    public class TestHelper
    {
        public const string SpecialCharactersString = "Täßting# Spècînál ChârÜcktérs iİıI вѣдѣ вѣди дūпт 形音碼她们它们";
        
        //default passwords
        private const string UserPassword = "User";
        private const string OwnerPassword = "Owner";
        private const string SignaturePassword = "Test1";
        
        public string TmpTestFolder { get; set; }

        public ApplicationSettings AppSettings { get; set; }

        private ConversionProfile _profile;

        public ConversionProfile Profile
        {
            get
            {
                return Job != null ? Job.Profile : _profile;
            }
            set
            {
                if (Job != null)
                    Job.Profile = value;
                else
                    _profile = value;
            }
        }

        public string TmpInfFile;
        public List<string> TmpPsFiles; 

        /// <summary>
        /// Note: You first have to generate a Job! 
        /// </summary>
        public IJob Job { get; set; }
        /// <summary>
        /// Note: You first have to generate a Job! 
        /// </summary>
        public JobInfo JobInfo { get; set; }

        public TestHelper(string testName)
        {
            TmpTestFolder = TempFileHelper.CreateTempFolder("PdfCreatorTest\\" + testName);

            AppSettings = new ApplicationSettings();
            Profile = new ConversionProfile();
            Profile.OpenViewer = false;
        }

        public void CleanUp()
        {
            try
            {
                if (Directory.Exists(TmpTestFolder))
                    Directory.Delete(TmpTestFolder, true);

                TempFileHelper.CleanUp();
            }
            catch (IOException) { }
        }

        private List<string> GeneratePSFileList(PSfiles psFiles, string tmpTestFolder)
        {
            var list = new List<string>();
            string testFilePath;

            switch (psFiles)
            {
                case PSfiles.ElevenTextPages:
                    testFilePath = Path.Combine(tmpTestFolder, psFiles + ".ps"); 
                    File.WriteAllBytes(testFilePath, Resources.ElevenTextPagesPS);
                    list.Add(testFilePath);
                    break;
                case PSfiles.EmptyPage:
                    testFilePath = Path.Combine(tmpTestFolder, psFiles + ".ps"); 
                    File.WriteAllBytes(testFilePath, Resources.EmptyPagePS);
                    list.Add(testFilePath);
                    break;
                case PSfiles.LandscapePage:
                    testFilePath = Path.Combine(tmpTestFolder, psFiles + ".ps"); 
                    File.WriteAllBytes(testFilePath, Resources.LandscapePagePS);
                    list.Add(testFilePath);
                    break;
                case PSfiles.PDFCreatorTestpage:
                    testFilePath = Path.Combine(tmpTestFolder, psFiles + ".ps"); 
                    File.WriteAllBytes(testFilePath, Resources.PDFCreatorTestpagePS);
                    list.Add(testFilePath);
                    break;
                case PSfiles.ThreePDFCreatorTestpages:
                    testFilePath = Path.Combine(tmpTestFolder, "PDFCreatorTestpage.ps");
                    File.WriteAllBytes(testFilePath, Resources.PDFCreatorTestpagePS);
                    list.Add(testFilePath);
                    list.Add(testFilePath);
                    list.Add(testFilePath);
                    break;
                case PSfiles.PortraitLandscapeLandscapeLandscapePortrait:
                    string portrait = Path.Combine(tmpTestFolder, "Portrait.ps");
                    File.WriteAllBytes(portrait, Resources.PortraitPagePS);
                    string landscape = Path.Combine(tmpTestFolder, "Landscape.ps");
                    File.WriteAllBytes(landscape, Resources.LandscapePagePS);
                    list.Add(portrait);
                    list.Add(landscape);
                    list.Add(landscape);
                    list.Add(landscape);
                    list.Add(portrait);
                    break;
                case PSfiles.PortraitPage:
                    testFilePath = Path.Combine(tmpTestFolder, psFiles + ".ps"); 
                    File.WriteAllBytes(testFilePath, Resources.PortraitPagePS);
                    list.Add(testFilePath);
                    break;
                case PSfiles.SixEmptyPages:
                    testFilePath = Path.Combine(TmpTestFolder, "EmptyPage.ps"); 
                    File.WriteAllBytes(testFilePath, Resources.EmptyPagePS);
                    list.Add(testFilePath);
                    list.Add(testFilePath);
                    list.Add(testFilePath);
                    list.Add(testFilePath);
                    list.Add(testFilePath);
                    list.Add(testFilePath);
                    break;
            }
            return list;
        }

        public void ResetProfileToDefault()
        {
            _profile = new ConversionProfile();
            _profile.OpenViewer = false;
        }

        /// <summary>
        /// Generates a job with default testpasswords (if required by the profile settings).
        /// Therefore an INF file and the required PS Files will be created and set in the jobs JobInfo.
        /// </summary>
        /// <param name="psFiles">select test content according to psFiles</param>
        public void GenerateGsJob(PSfiles psFiles, OutputFormat outputformat, string jobTempFolder = null)
        {
            GenerateGsJob(psFiles, outputformat, new FileWrap(), new DirectoryWrap(), jobTempFolder);
        }

        /// <summary>
        /// Generates a job with default testpasswords (if required by the profile settings).
        /// Therefore an INF file and the required PS Files will be created and set in the jobs JobInfo.
        /// </summary>
        /// <param name="psFiles">select test content according to psFiles</param>
        public void GenerateGsJob(PSfiles psFiles, OutputFormat outputformat, IFile fileWrap, IDirectory directoryWrap, string jobTempFolder = null)
        {
            var tempFolderProvider = MockRepository.GenerateStub<ITempFolderProvider>();
            if (string.IsNullOrEmpty(jobTempFolder))
            {
                tempFolderProvider.Stub(x => x.TempFolder)
                    .Return(Environment.ExpandEnvironmentVariables(@"%TMP%\PDFCreator\Temp"));
            }
            else
            {
                tempFolderProvider.Stub(x => x.TempFolder)
                    .Return(Environment.ExpandEnvironmentVariables(jobTempFolder));
            }
            _profile.OutputFormat = outputformat;

            GenerateInfFileWithPsFiles(psFiles);
            JobInfo = new JobInfo(TmpInfFile, AppSettings.TitleReplacement);

            var jobTranslations = new JobTranslations();
            jobTranslations.EmailSignature = "\r\n\r\nCreated with PDFCreator";

            Job = new GhostscriptJob(JobInfo, _profile, jobTranslations, tempFolderProvider, fileWrap, directoryWrap);
            
            string extension = outputformat.ToString();
            if (outputformat == OutputFormat.PdfA1B || outputformat == OutputFormat.PdfA2B || outputformat == OutputFormat.PdfX)
                extension = "pdf";
            Job.OutputFilenameTemplate = TmpInfFile.Replace(".inf", "." + extension);
            Job.Passwords.PdfUserPassword = _profile.PdfSettings.Security.RequireUserPassword ? UserPassword : null;
            Job.Passwords.PdfOwnerPassword = _profile.PdfSettings.Security.Enabled ? OwnerPassword : null;
            Job.Passwords.PdfSignaturePassword = _profile.PdfSettings.Signature.Enabled ? SignaturePassword : null;

            EnableLogging();
        }

        /// <summary>
        /// Create PathWrapStub with given folder as tempfolder. Other functions return the behavior of Path
        /// (Incomplete! Might be extended for further functionalities...)
        /// </summary>
        /// <param name="tempFolder">Folder as return value for GetTempPath()</param>
        /// <returns>PathWrapStub</returns>
        public static IPath CreatePathWrapStubWithGivenTempfolder(string tempFolder)
        {
            var pathWrapStub = MockRepository.GenerateStub<PathWrapSafe>();

            pathWrapStub.Stub(x => x.GetTempPath()).Return(tempFolder).Repeat.Any();

            return pathWrapStub;
        }

        /// <summary>
        /// Generates a job with default testpasswords (if required by the profile settings)
        /// and sets a tesfile as output, without running it.
        /// Note: The INF- and the PS file will be created with the content of the PDFCreatorTestpage  
        /// 
        /// </summary>
        /// <param name="testFileAsOutput">Testfile setted as output</param>
        public void GenerateGsJob_WithSettedOutput(TestFile testFileAsOutput)
        {
            var testFile = GenerateTestFile(testFileAsOutput);

            switch(testFileAsOutput)
            {
                case TestFile.Cover2PagesSixEmptyPagesPDF:
                case TestFile.Cover2PagesSixEmptyPagesAttachment3PagesPDF:
                case TestFile.SixEmptyPagesPDF:
                case TestFile.SixEmptyPagesAttachment3PagesPDF:
                    _profile.OutputFormat = OutputFormat.Pdf;
                    GenerateGsJob(PSfiles.SixEmptyPages, _profile.OutputFormat, new FileWrap(), new DirectoryWrap());
                    break;
                case TestFile.PDFCreatorTestpagePdfA:
                    _profile.OutputFormat = OutputFormat.PdfA2B;
                    GenerateGsJob(PSfiles.PDFCreatorTestpage, _profile.OutputFormat, new FileWrap(), new DirectoryWrap());
                    break;
                default:
                    GenerateGsJob(PSfiles.PDFCreatorTestpage, _profile.OutputFormat, new FileWrap(), new DirectoryWrap());
                    break;
            }

            Job.OutputFiles.Clear();
            Job.OutputFiles.Add(testFile);
            Job.OutputFilenameTemplate = testFile;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="psFiles"></param>
        /// <returns></returns>
        public string GenerateInfFileWithPsFiles(PSfiles psFiles)
        {
            TmpInfFile = Path.Combine(TmpTestFolder, psFiles + ".inf");

            TmpPsFiles = GeneratePSFileList(psFiles, TmpTestFolder);

            var sb = new StringBuilder();

            for (int i = 1; i <= TmpPsFiles.Count; i++)
            {
                sb.AppendLine("[" + i + "]");
                sb.AppendLine("SessionId=1");
                sb.AppendLine("WinStation=Console");
                sb.AppendLine("UserName=SampleUser1234");
                sb.AppendLine("ClientComputer=\\PC1");
                sb.AppendLine("SpoolFileName=" + TmpPsFiles[i - 1]);
                sb.AppendLine("PrinterName=PDFCreator");
                sb.AppendLine("JobId=1");
                sb.AppendLine("DocumentTitle=Title");
                AddTotalPagesAndCopies(psFiles, sb);
                sb.AppendLine("");
            }

            File.WriteAllText(TmpInfFile, sb.ToString(), Encoding.GetEncoding("Unicode"));

            return TmpInfFile;
        }

        private void AddTotalPagesAndCopies(PSfiles psFiles, StringBuilder sb)
        {
            switch (psFiles)
            {
                case PSfiles.ElevenTextPages:
                    sb.AppendLine("TotalPages=11");
                    sb.AppendLine("Copies=1");
                    break;
                case PSfiles.EmptyPage:
                case PSfiles.LandscapePage:
                case PSfiles.PortraitPage:
                case PSfiles.PDFCreatorTestpage:
                    sb.AppendLine("Copies=1");
                    sb.AppendLine("TotalPages=1");
                    break;
                case PSfiles.PortraitLandscapeLandscapeLandscapePortrait:
                    sb.AppendLine("Copies=1");
                    sb.AppendLine("TotalPages=1");
                    break;
                case PSfiles.SixEmptyPages:
                    sb.AppendLine("Copies=6");
                    sb.AppendLine("TotalPages=1");
                    break;
                case PSfiles.ThreePDFCreatorTestpages:
                    sb.AppendLine("Copies=3");
                    sb.AppendLine("TotalPages=1");
                    break;
            }
        }

        private void EnableLogging()
        {
            string outputFolder = Path.GetDirectoryName(TmpTestFolder) ?? "";

            var config = new LoggingConfiguration();

            var fileTarget = new FileTarget();
            fileTarget.FileName = Path.Combine(outputFolder, "log.txt");

            var consoleTarget = new ColoredConsoleTarget();

            config.AddTarget("logFile", fileTarget);
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Trace, fileTarget));
            
            config.AddTarget("console", consoleTarget);
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Trace, consoleTarget));
            
            LogManager.Configuration = config;
        }

        public void RunGsJob()
        {
            SetUpGhostscript();
            Job.RunJob();
        }

        private void SetUpGhostscript()
        {
            GhostscriptVersion gsVersion = new GhostscriptDiscovery().GetBestGhostscriptInstance();
            Assert.IsNotNull(gsVersion, "No Ghostscript instance found");
        }

        /// <summary>
        /// Allows to set the template for the full file template without having to respecify the path
        /// </summary>
        /// <param name="filename"></param>
        public void SetFilenameTemplate(string filename)
        {
            string outputFolder = Path.GetDirectoryName(Job.OutputFilenameTemplate) ?? "";
            Job.OutputFilenameTemplate = Path.Combine(outputFolder, filename);
        }

        /// <summary>
        /// Creates testfiles according to the TestFile enum and returns its path. 
        /// </summary>
        /// <param name="testFile">Choose a testfile from the enum</param>
        /// <returns>Path to the created testfile</returns>
        public string GenerateTestFile(TestFile testFile)
        {
            var testfilePath = Path.Combine(TmpTestFolder, testFile + ".pdf");
            
            switch (testFile)
            {   
                case TestFile.Attachment3PagesPDF:
                    File.WriteAllBytes(testfilePath, Resources.Attachment3PagesPDF);
                    break;
                case TestFile.Background3PagesPDF:
                    File.WriteAllBytes(testfilePath, Resources.Background3PagesPDF);
                    break;
                case TestFile.CertificationFileP12:
                    testfilePath = testfilePath.Replace(".pdf", ".p12");
                    File.WriteAllBytes(testfilePath, Resources.CertificationFileP12);
                    break;
                case TestFile.Cover2PagesPDF:
                    File.WriteAllBytes(testfilePath, Resources.Cover2PagesPDF);
                    break;
                case TestFile.Cover2PagesSixEmptyPagesPDF:
                    File.WriteAllBytes(testfilePath, Resources.Cover2PagesSixEmptyPagesPDF);
                    break;
                case TestFile.Cover2PagesSixEmptyPagesAttachment3PagesPDF:
                    File.WriteAllBytes(testfilePath, Resources.Cover2PagesSixEmptyPagesAttachment3PagesPDF);
                    break;
                case TestFile.FourRotatingPDFCreatorTestpagesPDF:
                    File.WriteAllBytes(testfilePath, Resources.FourRotatingPDFCreatorTestpagesPDF);
                    break;
                case TestFile.PageRotation0PDF:
                    File.WriteAllBytes(testfilePath, Resources.PageRotation0PDF);
                    break;
                case TestFile.PageRotation180PDF:
                    File.WriteAllBytes(testfilePath, Resources.PageRotation180PDF);
                    break;
                case TestFile.PageRotation270PDF:
                    File.WriteAllBytes(testfilePath, Resources.PageRotation270PDF);
                    break;
                case TestFile.PageRotation90PDF:
                    File.WriteAllBytes(testfilePath, Resources.PageRotation90PDF);
                    break;
                case TestFile.PDFCreatorTestpagePDF:
                    File.WriteAllBytes(testfilePath, Resources.PDFCreatorTestpagePDF);
                    break;
                case TestFile.PDFCreatorTestpagePdfA:
                    File.WriteAllBytes(testfilePath, Resources.PDFCreatorTestpagePDFA);
                    break;
                case TestFile.PortraitLandscapeLandscapeLandscapePortraitPDF:
                    File.WriteAllBytes(testfilePath, Resources.PortraitLandscapeLandscapeLandscapePortraitPDF);
                    break;
                case TestFile.SixEmptyPagesPDF:
                    File.WriteAllBytes(testfilePath, Resources.SixEmptyPagesPDF);
                    break;
                case TestFile.SixEmptyPagesAttachment3PagesPDF:
                    File.WriteAllBytes(testfilePath, Resources.SixEmptyPagesAttachment3PagesPDF);
                    break;
                case TestFile.ScriptCopyFilesToDirectoryCMD:
                    testfilePath = testfilePath.Replace(".pdf", ".cmd");
                    File.WriteAllText(testfilePath, Resources.ScriptCopyFilesToDirectoryCMD);
                    break;
                case TestFile.ThreePDFCreatorTestpagesPDF:
                    File.WriteAllBytes(testfilePath, Resources.ThreePDFCreatorTestpagesPDF);
                    break;
                case TestFile.PDFCreatorTestpagePs:
                    testfilePath = testfilePath.Replace(".pdf", ".ps");
                    File.WriteAllBytes(testfilePath, Resources.PDFCreatorTestpagePS);
                    break;
            }

            return testfilePath;
        }
    }

    public enum TestFile
    {
        Attachment3PagesPDF,
        Background3PagesPDF,
        Cover2PagesPDF,
        Cover2PagesSixEmptyPagesPDF,
        Cover2PagesSixEmptyPagesAttachment3PagesPDF,
        CertificationFileP12,
        FourRotatingPDFCreatorTestpagesPDF,
        PageRotation0PDF,
        PageRotation180PDF,
        PageRotation270PDF,
        PageRotation90PDF,
        PDFCreatorTestpagePDF,
        PDFCreatorTestpagePdfA,
        PortraitLandscapeLandscapeLandscapePortraitPDF,
        SixEmptyPagesPDF,
        SixEmptyPagesAttachment3PagesPDF,
        ScriptCopyFilesToDirectoryCMD,
        ThreePDFCreatorTestpagesPDF,
        PDFCreatorTestpagePs
    }
    
    public enum PSfiles
    {
        ElevenTextPages,
        LandscapePage,
        PDFCreatorTestpage,
        ThreePDFCreatorTestpages,
        PortraitPage,
        PortraitLandscapeLandscapeLandscapePortrait,
        EmptyPage,
        SixEmptyPages
    }
}
