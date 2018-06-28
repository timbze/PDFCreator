using NUnit.Framework;
using pdfforge.DataStorage.Storage;
using pdfforge.Obsidian.Interaction;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.Services.Logging;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.Core.Startup.StartConditions;
using pdfforge.PDFCreator.Core.StartupInterface;
using pdfforge.PDFCreator.Core.Workflow;
using pdfforge.PDFCreator.Editions.EditionBase;
using pdfforge.PDFCreator.Editions.EditionBase.Prism.SimpleInjector;
using pdfforge.PDFCreator.Editions.PDFCreator;
using pdfforge.PDFCreator.Editions.PDFCreatorBusiness;
using pdfforge.PDFCreator.Editions.PDFCreatorCustom;
using pdfforge.PDFCreator.Editions.PDFCreatorPlus;
using pdfforge.PDFCreator.Editions.PDFCreatorTerminalServer;
using pdfforge.PDFCreator.UI.Presentation.Assistants;
using pdfforge.PDFCreator.UI.Presentation.ServiceLocator;
using SimpleInjector;
using SimpleInjector.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Translatable;
using MainShell = pdfforge.PDFCreator.UI.Presentation.MainShell;
using PrintJobShell = pdfforge.PDFCreator.UI.Presentation.PrintJobShell;

namespace pdfforge.PDFCreator.UnitTest.Editions.PDFCreator
{
    [TestFixture]
    public class BootstrapperTest
    {
        // For some classes, we accept that they are registered as singleton but have transient dependencies.
        // There must be a reason for this, i.e. that we want a single instance of this class, but don't want it to share its state with other usages
        private readonly Type[] _lifestyleMismatchAcceptableClasses =
        {
            typeof(IJobInfoQueueManager),
            typeof(IFileConversionHandler),
            typeof(IUpdateAssistant)
        };

        private static IList<Bootstrapper> AllBootstrappers()
        {
            return new Bootstrapper[]
            {
                new PDFCreatorBootstrapper(),
                new PDFCreatorPlusBootstrapper(),
                new PDFCreatorBusinessBootstrapper(),
                new PDFCreatorTerminalServerBootstrapper(),
                new PDFCreatorCustomBootstrapper()
            }.ToList();
        }

        private Container BuildContainer(Bootstrapper bootstrapper)
        {
            var container = new Container();
            bootstrapper.ConfigureContainer(container);

            return container;
        }

        private Container BuildPrismContainer(Bootstrapper bootstrapper)
        {
            var container = BuildContainer(bootstrapper);

            var prismBootstrapper = new PrismBootstrapper(bootstrapper.DefineProfileSettingsTabs(), bootstrapper.DefineApplicationSettingsTabs());
            prismBootstrapper.ConfigurePrismDependecies(container);
            prismBootstrapper.RegisterNavigationViews(container);

            return container;
        }

        [Test]
        [TestCaseSource(nameof(AllBootstrappers))]
        public void AllBootstrappers_ContainDefaultStartupConditions(Bootstrapper bootstrapper)
        {
            var container = BuildContainer(bootstrapper);

            var conditions = container.GetAllInstances<IStartupCondition>();
            var types = conditions.Select(c => c.GetType());

            var defaultConditions = new[]
            {
                typeof(SpoolerRunningCondition),
                typeof(CheckSpoolFolderCondition),
                typeof(GhostscriptCondition),
                typeof(PrinterInstalledCondition)
            };

            CollectionAssert.IsSubsetOf(defaultConditions, types);
        }

        [Test]
        [TestCaseSource(nameof(AllBootstrappers))]
        public void AllBootstrappers_ProperlyRegisterLicenseCondition(Bootstrapper bootstrapper)
        {
            var isLicensedEdition = !(bootstrapper is PDFCreatorBootstrapper) && !(bootstrapper is PDFCreatorCustomBootstrapper);

            var container = BuildContainer(bootstrapper);

            var conditions = container.GetAllInstances<IStartupCondition>();
            var types = conditions.Select(c => c.GetType());

            if (isLicensedEdition)
                CollectionAssert.Contains(types, typeof(LicenseCondition));
            else
                CollectionAssert.DoesNotContain(types, typeof(LicenseCondition));
        }

        [Test]
        [TestCaseSource(nameof(AllBootstrappers))]
        public void AllBootstrappers_ProperlyRegisterTerminalServerCondition(Bootstrapper bootstrapper)
        {
            var isTerminalServerEdition = bootstrapper is PDFCreatorTerminalServerBootstrapper || ((bootstrapper as PDFCreatorCustomBootstrapper)?.ValidOnTerminalServer == true);

            var container = BuildContainer(bootstrapper);

            var conditions = container.GetAllInstances<IStartupCondition>();
            var types = conditions.Select(c => c.GetType());

            if (isTerminalServerEdition)
                CollectionAssert.DoesNotContain(types, typeof(TerminalServerNotAllowedCondition));
            else
                CollectionAssert.Contains(types, typeof(TerminalServerNotAllowedCondition));
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        [TestCaseSource(nameof(AllBootstrappers))]
        public void ConfigurationOfContainer_CanBeValidatedWithoutWarnings(Bootstrapper bootstrapper)
        {
            var container = BuildPrismContainer(bootstrapper);

            Assert.DoesNotThrow(() => { container.Verify(VerificationOption.VerifyOnly); });

            var result = Analyzer.Analyze(container)
                .Where(x => x.Severity > DiagnosticSeverity.Information)
                .Where(x => (x.DiagnosticType != DiagnosticType.LifestyleMismatch) || !_lifestyleMismatchAcceptableClasses.Contains(x.ServiceType))
                .ToList();

            var message = "";
            foreach (var diagnosticResult in result)
                message += $"{diagnosticResult.Severity} | {diagnosticResult.DiagnosticType}: {diagnosticResult.Description} {Environment.NewLine}";

            Assert.IsFalse(result.Any(), message);
        }

        [Test]
        [TestCaseSource(nameof(AllBootstrappers))]
        public void ConfigurationOfContainer_DoesNotThrowExceptions(Bootstrapper bootstrapper)
        {
            var container = new Container();
            Assert.DoesNotThrow(() => bootstrapper.ConfigureContainer(container));
        }

        [Test]
        [TestCaseSource(nameof(AllBootstrappers))]
        [Apartment(ApartmentState.STA)]
        public void ConfigurationOfContainer_WithInteractions_CanBeValidatedWithoutWarnings(Bootstrapper bootstrapper)
        {
            ViewRegistry.ClearInteractionMappings();

            // TODO this needs to be tested differently, maybe retrieve all registrations from ViewRegistry?
            //var windowResolver = new TestWindowResolver(container);
            //var windowRegistry = new WindowRegistry(windowResolver);

            var container = BuildPrismContainer(bootstrapper);

            LoggingHelper.InitConsoleLogger("PDFCreator-Test", LoggingLevel.Off);
            var settingsHelper = container.GetInstance<ISettingsManager>();
            settingsHelper.LoadAllSettings();

            container.Verify(VerificationOption.VerifyOnly);

            var result = Analyzer.Analyze(container)
                .Where(x => x.Severity > DiagnosticSeverity.Information)
                .Where(x => (x.DiagnosticType != DiagnosticType.LifestyleMismatch) || !_lifestyleMismatchAcceptableClasses.Contains(x.ServiceType))
                .ToList();

            var message = "";
            foreach (var diagnosticResult in result)
                message += $"{diagnosticResult.Severity} | {diagnosticResult.DiagnosticType}: {diagnosticResult.Description} {Environment.NewLine}";

            Assert.IsFalse(result.Any(), message);
        }

        [Test]
        public void AllAppStarts_AreRegistered()
        {
            var bootstrapper = new PDFCreatorPlusBootstrapper();
            var container = BuildPrismContainer(bootstrapper);

            var appStarts = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                             from type in assembly.GetTypes()
                             where !type.IsAbstract && typeof(IAppStart).IsAssignableFrom(type)
                             select type).ToList();

            foreach (var appStart in appStarts)
            {
                Assert.DoesNotThrow(() => container.GetInstance(appStart));
            }

            CollectionAssert.IsNotEmpty(appStarts);
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void AllWhitelistedClasses_AreRegistered()
        {
            var bootstrapper = new PDFCreatorPlusBootstrapper();
            var container = BuildPrismContainer(bootstrapper);

            var settingsProvider = container.GetInstance<ISettingsProvider>();
            var builder = new DefaultSettingsBuilder();
            var settings = builder.CreateDefaultSettings("PDFCreator", new IniStorage(), "en");
            settingsProvider.UpdateSettings(settings);

            var whitelisted = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                               from type in assembly.GetTypes()
                               where !type.IsAbstract && typeof(IWhitelisted).IsAssignableFrom(type)
                               select type).ToList();

            foreach (var type in whitelisted)
            {
                Assert.DoesNotThrow(() => container.GetInstance(type), $"Could not create type '{type}'");
            }

            CollectionAssert.IsNotEmpty(whitelisted);
        }

        [TestCase(typeof(MainShell))]
        [TestCase(typeof(PrintJobShell))]
        [Apartment(ApartmentState.STA)]
        public void Shells_CanBeResolved(Type type)
        {
            var bootstrapper = new PDFCreatorPlusBootstrapper();
            var container = BuildPrismContainer(bootstrapper);

            Assert.DoesNotThrow(() => container.GetInstance(type));
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void AllTypes_DoNotRequestITranslatableInConstructor()
        {
            var bootstrapper = new PDFCreatorBootstrapper();
            BuildPrismContainer(bootstrapper);

            var typesWithTranslatableConstructor = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(TypeHasTranslatableConstructor);

            CollectionAssert.IsEmpty(typesWithTranslatableConstructor);
        }

        private bool TypeHasTranslatableConstructor(Type type)
        {
            return type.GetConstructors()
                .Any(constructorInfo =>
                    constructorInfo.GetParameters().Any(a => typeof(ITranslatable).IsAssignableFrom(a.ParameterType)));
        }
    }
}
