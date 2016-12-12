using System;
using System.IO;
using System.Linq;
using System.Reflection;
using pdfforge.Obsidian.Interaction;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Core.ComImplementation;
using pdfforge.PDFCreator.Core.Services.Logging;
using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.Editions.EditionBase;
using pdfforge.PDFCreator.Utilities;
using SimpleInjector;

namespace pdfforge.PDFCreator.UI.COM
{
    internal class ComDependencyBuilder
    {
        private static ComDependencies _comDependencies;

        public ComDependencies ComDependencies
        {
            get
            {
                if (_comDependencies != null)
                    return _comDependencies;
                return _comDependencies = BuildComDependencies();
            }
        }

        private Bootstrapper CreateBootstrapper()
        {
            var assemblyHelper = new AssemblyHelper();
            var applicationDir = assemblyHelper.GetPdfforgeAssemblyDirectory();
            var assemblyPath = Path.Combine(applicationDir, "PDFCreator.exe");

            var assembly = Assembly.LoadFrom(assemblyPath);
            var bootstrapperType = assembly.GetTypes().First(t => t.IsSubclassOf(typeof(Bootstrapper)) && !t.IsAbstract);
            return (Bootstrapper)Activator.CreateInstance(bootstrapperType);
        }

        private ComDependencies BuildComDependencies()
        {
            var bootstrapper = CreateBootstrapper();
            var container = new Container();
            bootstrapper.ConfigureContainer(container, new WindowRegistry(null));
            container.Register<PrintFileHelperComFactory>();
            container.Register<IComWorkflowFactory>(() => new ComWorkflowFactory(container));
            container.RegisterSingleton(() => new ThreadPool());
            container.Register<IPrintJobAdapterFactory, PrintJobAdapterFactory>();

            var dependencies = container.GetInstance<ComDependencies>();

            LoggingHelper.InitFileLogger("PDFCreator", LoggingLevel.Error);

            var settingsManager = container.GetInstance<ISettingsManager>();
            settingsManager.LoadAllSettings();

            var translator = container.GetInstance<TranslationHelper>();
            translator.InitTranslator("english");

            return dependencies;
        }
    }
}
