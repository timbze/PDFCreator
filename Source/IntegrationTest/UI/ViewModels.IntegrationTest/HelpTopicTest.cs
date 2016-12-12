using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using pdfforge.PDFCreator.UI.ViewModels.Helper;
using pdfforge.PDFCreator.Utilities;

namespace pdfforge.IntegrationTest.ViewModels.IntegrationTest
{
    [TestFixture]
    internal class HelpTopicTest
    {
        [TearDown]
        public void TearDown()
        {
            foreach (var folder in _tempFoldersList)
            {
                if (Directory.Exists(folder))
                {
                    try
                    {
                        Directory.Delete(folder, true);
                    }
                    catch (IOException)
                    {
                    }
                }
            }
        }

        private readonly List<string> _tempFoldersList = new List<string>();

        private void TestSingleHelpFile(string filename, string folder)
        {
            var helpPath = folder;

            foreach (HelpTopic topic in Enum.GetValues(typeof (HelpTopic)))
            {
                var sourceFile = StringValueAttribute.GetValue(topic);
                sourceFile = Path.Combine(helpPath, sourceFile + ".html");
                Assert.IsTrue(File.Exists(sourceFile), $"Help file '{sourceFile}' does not exist in {filename}!");
            }
        }

        private string DecompileChmFile(string chmFile)
        {
            Assert.NotNull(chmFile);

            var tmpFolder = GetTemporaryDirectory();

            var tmpFile = Path.Combine(tmpFolder, Path.GetFileName(chmFile));
            File.Copy(chmFile, tmpFile);
            var args = string.Format("-decompile {0} {1}", ".", Path.GetFileName(chmFile));
            args = args.Replace("\\", "/");

            var psi = new ProcessStartInfo("hh", args);
            psi.WorkingDirectory = tmpFolder;
            psi.CreateNoWindow = true;
            psi.WindowStyle = ProcessWindowStyle.Hidden;

            var p = Process.Start(psi);
            p.WaitForExit();

            return tmpFolder;
        }

        private static string FindManualFolder()
        {
            var a = Assembly.GetExecutingAssembly();
            var appDir = Path.GetDirectoryName(a.CodeBase.Replace(@"file:///", ""));
            Assert.IsNotNull(appDir);

            var candidates = new[]
            {
                appDir,
                Path.GetFullPath(Path.Combine(appDir, @"..")),
                Path.GetFullPath(Path.Combine(appDir, @"..\..")),
                Path.GetFullPath(Path.Combine(appDir, @"..\..\..")),
                Path.GetFullPath(Path.Combine(appDir, @"..\..\..\..")),
                Path.GetFullPath(Path.Combine(appDir, @"..\..\..\..\..")),
                Path.GetFullPath(Path.Combine(appDir, @"..\..\..\..\..\..")),
                ""
            };

            foreach (var dir in candidates)
            {
                var dirCandidate = Path.Combine(dir, @"build\chm");
                if (Directory.Exists(dirCandidate) && Directory.GetFiles(dirCandidate, "*.chm").Length > 0)
                    return dirCandidate;
            }

            throw new IOException("Could not find user guide folder");
        }

        public string GetTemporaryDirectory()
        {
            var tempDirectory = Path.Combine(Path.GetTempPath(), "PDFCreatorTest-" + Path.GetRandomFileName());
            Directory.CreateDirectory(tempDirectory);
            return tempDirectory;
        }

        [Test]
        public void TestEnglishManualExists()
        {
            var manualFolder = FindManualFolder();

            Assert.IsTrue(File.Exists(Path.Combine(manualFolder, "PDFCreator_english.chm")));
        }

        [Test]
        public void TestHelpTopicFilesExist()
        {
            var manualFolder = FindManualFolder();

            foreach (var manual in Directory.GetFiles(manualFolder, "*.chm"))
            {
                var folder = DecompileChmFile(manual);
                //Assert.IsTrue(Directory.Exists(Path.Combine(folder, "html")), "Decompiling failed, the html folder does not exist");

                _tempFoldersList.Add(folder);
                TestSingleHelpFile(Path.GetFileName(manual), folder);
            }
        }

        [Test]
        public void TestHelpTopicsAssigned()
        {
            foreach (HelpTopic topic in Enum.GetValues(typeof (HelpTopic)))
            {
                Assert.IsNotNullOrEmpty(StringValueAttribute.GetValue(topic), $"Topic {topic} does not have a html reference attached");
            }
        }
    }
}