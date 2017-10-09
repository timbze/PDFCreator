using NSubstitute;
using NUnit.Framework;
using PDFCreator.TestUtilities;
using pdfforge.DataStorage.Storage;
using pdfforge.Obsidian;
using pdfforge.Obsidian.Interaction.DialogInteractions;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.Presentation.Assistants;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using System.IO;

namespace pdfforge.IntegrationTest.Presentation.IntegrationTest
{
    [TestFixture]
    public class IniSettingsAssistantTest
    {
        private IInteractionInvoker BuildInvoker(string fileName)
        {
            var invoker = Substitute.For<IInteractionInvoker>();
            invoker.When(x => x.Invoke(Arg.Any<SaveFileInteraction>())).Do(info =>
            {
                var interaction = info.Arg<SaveFileInteraction>();
                interaction.FileName = fileName;
                interaction.Success = true;
            });

            invoker.When(x => x.Invoke(Arg.Any<OpenFileInteraction>())).Do(info =>
            {
                var interaction = info.Arg<OpenFileInteraction>();
                interaction.FileName = fileName;
                interaction.Success = true;
            });

            invoker.When(x => x.Invoke(Arg.Any<MessageInteraction>())).Do(info => info.Arg<MessageInteraction>().Response = MessageResponse.Yes);

            return invoker;
        }

        private PdfCreatorSettings CreateSettings()
        {
            var profileBuilder = new DefaultSettingsBuilder();
            var settings = profileBuilder.CreateDefaultSettings("MySpecialPrinter", new IniStorage(), "Elbisch");
            return settings;
        }

        [Test]
        public void SaveAndLoad_ResultsInExactlyEqualSettings()
        {
            var tempFolder = TempFileHelper.CreateTempFolder("IniSettingsTest");
            var fileName = Path.Combine(tempFolder, "savedSettings.ini");

            var settings = CreateSettings();
            var invoker = BuildInvoker(fileName);

            var settingsHelper = Substitute.For<ISettingsProvider>();
            settingsHelper.Settings.Returns(settings);
            settingsHelper.CheckValidSettings(Arg.Any<PdfCreatorSettings>()).Returns(x => x.Arg<PdfCreatorSettings>().ConversionProfiles.Count > 0);

            PdfCreatorSettings loadedSettings = null;

            var settingsMananger = Substitute.For<ISettingsManager>();
            settingsMananger.GetSettingsProvider().Returns(settingsHelper);
            settingsMananger.When(x => x.ApplyAndSaveSettings(Arg.Any<PdfCreatorSettings>())).Do(x => loadedSettings = x.Arg<PdfCreatorSettings>());

            var iniSettingsLoader = new IniSettingsLoader(settingsMananger, new DataStorageFactory());
            var iniSettingsAssistant = new IniSettingsAssistant(invoker, new DesignTimeTranslationUpdater(), settingsMananger, new DataStorageFactory(), iniSettingsLoader);

            iniSettingsAssistant.SaveIniSettings(settings.ApplicationSettings);
            iniSettingsAssistant.LoadIniSettings();

            var titles = settings.ApplicationSettings.TitleReplacement;
            var loadedTitles = loadedSettings.ApplicationSettings.TitleReplacement;
            for (var i = 0; i < titles.Count; i++)
            {
                Assert.AreEqual(titles[i], loadedTitles[i]);
            }
            Assert.AreEqual(settings.ApplicationSettings.TitleReplacement, loadedSettings.ApplicationSettings.TitleReplacement);
            Assert.AreEqual(settings.ApplicationSettings, loadedSettings.ApplicationSettings);
            Assert.AreEqual(settings.ConversionProfiles, loadedSettings.ConversionProfiles);

            Assert.AreEqual(settings.ToString(), loadedSettings.ToString());

            TempFileHelper.CleanUp();
        }
    }
}
