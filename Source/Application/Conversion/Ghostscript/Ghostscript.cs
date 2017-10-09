using NLog;
using pdfforge.PDFCreator.Conversion.Ghostscript.OutputDevices;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace pdfforge.PDFCreator.Conversion.Ghostscript
{
    /// <summary>
    ///     Provides access to Ghostscript, either through DLL access or by calling the Ghostscript exe
    /// </summary>
    public class GhostScript
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public GhostScript(GhostscriptVersion ghostscriptVersion)
        {
            GhostscriptVersion = ghostscriptVersion;
        }

        public GhostscriptVersion GhostscriptVersion { get; }

        public event EventHandler<OutputEventArgs> Output;

        private bool Run(IList<string> parameters, string tempOutputFolder)
        {
            _logger.Debug("Ghostscript Parameters:\r\n" + string.Join("\r\n", parameters));

            // Start the child process.
            var p = new Process();
            // Redirect the output stream of the child process.
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = GhostscriptVersion.ExePath;
            p.StartInfo.CreateNoWindow = true;

            var isFirst = true;
            var sb = new StringBuilder();

            foreach (var s in parameters)
            {
                var tmp = s;
                if (isFirst)
                {
                    isFirst = false;
                    continue;
                }

                if (tmp.Contains(" "))
                {
                    tmp = tmp.Replace("\"", "\\\"");
                    tmp = "\"" + tmp + "\"";
                }

                sb.AppendLine(tmp);
            }

            var parameterFile = Path.Combine(tempOutputFolder, "parameters.txt");
            File.WriteAllText(parameterFile, sb.ToString());

            p.StartInfo.Arguments = string.Format("@\"{0}\"", parameterFile);

            p.Start();
            // Do not wait for the child process to exit before
            // reading to the end of its redirected stream.
            // p.WaitForExit();
            // Read the output stream first and then wait.

            var sbout = new StringBuilder();

            while (!p.StandardOutput.EndOfStream)
            {
                var c = (char)p.StandardOutput.Read();
                sbout.Append(c);

                if ((c == ']') || (c == '\n'))
                {
                    RaiseOutputEvent(sbout.ToString());

                    sbout.Length = 0;
                }
            }

            if (sbout.Length > 0)
                RaiseOutputEvent(sbout.ToString());

            p.WaitForExit();

            return p.ExitCode == 0;
        }

        private void RaiseOutputEvent(string message)
        {
            if (Output != null)
            {
                Output(this, new OutputEventArgs(message));
            }
        }

        /// <summary>
        ///     Runs Ghostscript with the parameters specified by the OutputDevice
        /// </summary>
        /// <param name="output">The output device to use for conversion</param>
        /// <param name="tempOutputFolder">Full path to the folder, where temporary files can be stored</param>
        public bool Run(OutputDevice output, string tempOutputFolder)
        {
            var parameters = (List<string>)output.GetGhostScriptParameters(GhostscriptVersion);
            var success = Run(parameters.ToArray(), tempOutputFolder);

            var outputFolder = Path.GetDirectoryName(output.Job.OutputFilenameTemplate);

            if (outputFolder != null && !Directory.Exists(outputFolder))
                Directory.CreateDirectory(outputFolder);

            CollectTempOutputFiles(output.Job);

            return success;
        }

        private void CollectTempOutputFiles(Job job)
        {
            var files = Directory.GetFiles(job.JobTempOutputFolder);

            foreach (var file in files)
            {
                job.TempOutputFiles.Add(file);
            }
        }
    }

    public class OutputEventArgs : EventArgs
    {
        public OutputEventArgs(string output)
        {
            Output = output;
        }

        public string Output { get; private set; }
    }
}
