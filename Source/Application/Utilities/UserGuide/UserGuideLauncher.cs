using System.IO;
using System.Windows.Forms;

namespace pdfforge.PDFCreator.Utilities.UserGuide
{
    /// <summary>
    ///     The UserGuideLauncher provides an easy mechanism of referencing and
    ///     launching sections of a user guide. Each topic is identified by an
    ///     enum value that is annotated with a <see cref="HelpTopicAttribute" />.
    /// </summary>
    public class UserGuideLauncher : IUserGuideLauncher
    {
        private string _helpFile;

        /// <summary>
        ///     Launch the user guide with the given topic.
        /// </summary>
        /// <param name="topic">An enum value that is the symbolic reference to a help topic.</param>
        public void ShowHelpTopic(object topic)
        {
            if (_helpFile == null)
                return;

            var topicText = GetTopic(topic);

            if (topicText == null)
            {
                return;
            }

            Help.ShowHelp(null, _helpFile, HelpNavigator.Topic, topicText + ".html");
        }

        public void SetUserGuide(string path)
        {
            if (!File.Exists(path))
                throw new IOException($"The file '{path}' does not exist");

            _helpFile = path;
        }

        /// <summary>
        ///     Determine the string value of a help topic, which identifies the section within the user guide.
        /// </summary>
        /// <param name="value">An enum value that is the symbolic reference to a help topic.</param>
        /// <returns>The string representation of the help topic, i.e. the path within the file.</returns>
        private string GetTopic(object value)
        {
            return StringValueAttribute.GetValue(value);
        }
    }
}
