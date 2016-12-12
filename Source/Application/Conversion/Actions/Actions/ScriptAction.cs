using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SystemInterface.Diagnostics;
using SystemInterface.IO;
using NLog;
using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.Process;
using pdfforge.PDFCreator.Utilities.Tokens;

namespace pdfforge.PDFCreator.Conversion.Actions.Actions
{
    /// <summary>
    ///     Executes a script or executable after the conversion process.
    ///     The script receives the full paths to all created files and a string with user-configurable parameters as arguments
    /// </summary>
    public class ScriptAction : IAction, ICheckable, IScriptActionHelper
    {
        private readonly IFile _file;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IPath _path;
        private readonly IProcessStarter _processStarter;

        public ScriptAction(IPath path, IProcessStarter processStarter, IFile file)
        {
            _path = path;
            _processStarter = processStarter;
            _file = file;
        }

        /// <summary>
        ///     Calls the script
        /// </summary>
        /// <param name="job">The current job</param>
        /// <returns>An ActionResult to determine the success and a list of errors</returns>
        public ActionResult ProcessJob(Job job)
        {
            _logger.Debug("Launched Script-Action");

            var scriptFile = _path.GetFullPath(ComposeScriptPath(job.Profile.Scripting.ScriptFile, job.TokenReplacer));

            var actionResult = Check(job.Profile, job.Accounts);
            if (!actionResult)
                return actionResult;

            IProcess process = _processStarter.CreateProcess(scriptFile);
            _logger.Debug("Script-File: " + scriptFile);

            var parameters = ComposeScriptParameters(job.Profile.Scripting.ParameterString, job.OutputFiles, job.TokenReplacer);

            process.StartInfo.Arguments = parameters;
            _logger.Debug("Script-Parameters: " + parameters);

            var scriptDir = Path.GetDirectoryName(scriptFile);
            if (scriptDir != null)
                process.StartInfo.WorkingDirectory = scriptDir;

            _logger.Debug("Script-Working Directory: " + scriptDir);

            process.EnableRaisingEvents = true;
            process.Exited += (sender, args) => process.Close();

            try
            {
                _logger.Debug("Launching script...");
                process.Start();

                if (job.Profile.Scripting.WaitForScript)
                {
                    _logger.Debug("Waiting for script to end");
                    process.WaitForExit();
                    _logger.Debug("Script execution ended");
                }
                else
                {
                    _logger.Debug("The script is executed in the background");
                }

                return new ActionResult();
            }
            catch (Exception ex)
            {
                _logger.Error("Exception while running the script file \"" + scriptFile + "\"\r\n" + ex.Message);
                return new ActionResult(ErrorCode.Script_GenericError);
            }
        }

        public bool IsEnabled(ConversionProfile profile)
        {
            return profile.Scripting.Enabled;
        }

        public bool Init(Job job)
        {
            return true;
        }

        public ActionResult Check(ConversionProfile profile, Accounts accounts)
        {
            var actionResult = new ActionResult();

            if (!profile.Scripting.Enabled)
                return actionResult;

            var tokenReplacer = new TokenReplacer();
            tokenReplacer.AddToken(new EnvironmentToken());

            var validName = new ValidName();

            var scriptFile = ComposeScriptPath(profile.Scripting.ScriptFile, tokenReplacer);

            if (string.IsNullOrEmpty(scriptFile))
            {
                _logger.Error("Missing script file.");
                actionResult.Add(ErrorCode.Script_NoScriptFileSpecified);
                return actionResult;
            }

            if (!validName.IsValidFilename(scriptFile))
            {
                _logger.Error("The script file \"" + scriptFile + "\" contains illegal characters.");
                actionResult.Add(ErrorCode.Script_IllegalCharacters);
                return actionResult;
            }

            //Skip check for network path
            if (!scriptFile.StartsWith(@"\\") && !_file.Exists(scriptFile))
            {
                _logger.Error("The script file \"" + scriptFile + "\" does not exist.");
                actionResult.Add(ErrorCode.Script_FileDoesNotExist);
                return actionResult;
            }

            return actionResult;
        }

        public string ComposeScriptPath(string path, TokenReplacer tokenReplacer)
        {
            var validName = new ValidName();
            var composedPath = tokenReplacer.ReplaceTokens(path);
            composedPath = validName.MakeValidFolderName(composedPath);

            return composedPath;
        }

        public string ComposeScriptParameters(string parameterString, IList<string> outputFiles, TokenReplacer tokenReplacer)
        {
            var composedParameters = new StringBuilder();

            composedParameters.Append(tokenReplacer.ReplaceTokens(parameterString) + " ");
            composedParameters.Append(string.Join(" ", outputFiles.Select(s => "\"" + _path.GetFullPath(s) + "\"")));

            return composedParameters.ToString();
        }
    }
}