using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using NLog;
using pdfforge.PDFCreator.Utilities;

namespace pdfforge.PDFCreator.Shared.Helper
{
    public static class UserGuideHelper
    {
        public static string HelpFile { get; set; }

        private static readonly Dictionary<HelpTopic, StringValueAttribute> StringValues = new Dictionary<HelpTopic, StringValueAttribute>();
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static readonly IAssemblyHelper _assemblyHelper = new AssemblyHelper();

        public static void SetLanguage(string languageName)
        {
            var applicationDir = _assemblyHelper.GetCurrentAssemblyDirectory();

            HelpFile = Path.Combine(applicationDir, "PDFCreator_english.chm");

            string helpFileCandidate = Path.Combine(applicationDir, $"PDFCreator_{languageName}.chm");

            if (File.Exists(helpFileCandidate))
                HelpFile = helpFileCandidate;
        }

        public static void ShowHelp(Control parent, HelpTopic topic)
        {
            string topicText = GetTopic(topic);

            if (topicText == null)
            {
                Logger.Warn("There is no help topic for {0}", topic);
                return;
            }

            ShowHelp(parent, topicText + ".html");
        }

        public static void ShowHelp(HelpTopic topic)
        {
            ShowHelp(null, topic);
        }

        private static void ShowHelp(Control parent, string topicId)
        {
            if (!File.Exists(HelpFile))
                return;

            Help.ShowHelp(parent, HelpFile, HelpNavigator.Topic, topicId);
        }

        public static string GetTopic(HelpTopic value)
        {
            string output = null;
            Type type = value.GetType();

            //Check first in our cached results...
            if (StringValues.ContainsKey(value))
                return StringValues[value].Value;
            
            //Look for our 'StringValueAttribute' 
            //in the field's custom attributes
            FieldInfo fi = type.GetField(value.ToString());
            var attrs = fi.GetCustomAttributes(typeof(StringValueAttribute), false) as StringValueAttribute[];

            if (attrs != null && attrs.Length > 0)
            {
                StringValues.Add(value, attrs[0]);
                output = attrs[0].Value;
            }

            return output;
        }
    }

    public enum HelpTopic
    {
        [StringValue("pdfcreator/index")]
        General,

        [StringValue("pdfcreator/introduction/whats-new")]
        WhatsNew,

        [StringValue("pdfcreator/using-pdfcreator/create-a-pdf")]
        CreatingPdf,

        [StringValue("pdfcreator/pdfcreator-settings/application-settings/index")]
        AppSettings,

        // Application settings
        [StringValue("pdfcreator/pdfcreator-settings/application-settings/general")]
        AppGeneral,

        [StringValue("pdfcreator/pdfcreator-settings/application-settings/printers")]
        AppPrinters,

        [StringValue("pdfcreator/pdfcreator-settings/application-settings/title")]
        AppTitle,

        [StringValue("pdfcreator/pdfcreator-settings/application-settings/debug")]
        AppDebug,

        [StringValue("pdfcreator/pdfcreator-settings/application-settings/license")]
        AppLicense,

        // Profile settings
        [StringValue("pdfcreator/pdfcreator-settings/profile-settings/index")]
        ProfileSettings,

        [StringValue("pdfcreator/pdfcreator-settings/profile-settings/document")]
        ProfileDocument,

        [StringValue("pdfcreator/pdfcreator-settings/profile-settings/save")]
        ProfileSave,

        [StringValue("pdfcreator/pdfcreator-settings/profile-settings/actions/index")]
        ProfileActions,

        [StringValue("pdfcreator/pdfcreator-settings/profile-settings/actions/add-cover")]
        Cover,

        [StringValue("pdfcreator/pdfcreator-settings/profile-settings/actions/add-attachment")]
        Attachment,

        [StringValue("pdfcreator/pdfcreator-settings/profile-settings/actions/print-document")]
        PrintDocument,

        [StringValue("pdfcreator/pdfcreator-settings/profile-settings/actions/open-email-client")]
        OpenEmailClient,

        [StringValue("pdfcreator/pdfcreator-settings/profile-settings/actions/send-email-over-smtp")]
        SendEmailOverSmtp,

        [StringValue("pdfcreator/pdfcreator-settings/profile-settings/actions/run-script")]
        RunScript,

        [StringValue("pdfcreator/pdfcreator-settings/profile-settings/actions/upload-with-ftp")]
        UploadWithFtp,

        [StringValue("pdfcreator/pdfcreator-settings/profile-settings/image-formats")]
        ProfileImageFormats,

        [StringValue("pdfcreator/pdfcreator-settings/profile-settings/pdf/index")]
        ProfilePdf,

        [StringValue("pdfcreator/pdfcreator-settings/profile-settings/pdf/general")]
        PdfGeneral,

        [StringValue("pdfcreator/pdfcreator-settings/profile-settings/pdf/compression")]
        PdfCompression,

        [StringValue("pdfcreator/pdfcreator-settings/profile-settings/pdf/security")]
        PdfSecurity,

        [StringValue("pdfcreator/pdfcreator-settings/profile-settings/pdf/signature")]
        PdfSignature,

        //Using-PdfCreator
        [StringValue("pdfcreator/using-pdfcreator/tokens")]
        Tokens,

        //Server
        [StringValue("pdfcreator-server/index")]
        Server,

        [StringValue("pdfcreator-server/pdfcreator-server-settings/queue-settings")]
        ServerQueueSettings,

        [StringValue("pdfcreator-server/pdfcreator-server-settings/general-settings/index")]
        ServerGeneralSettings,

        [StringValue("pdfcreator-server/performance-test")]
        ServerPerformanceTest,

        [StringValue("pdfcreator-server/license/index")]
        ServerLicense,

        [StringValue("pdfcreator/license/index")]
        License,
    }
}
