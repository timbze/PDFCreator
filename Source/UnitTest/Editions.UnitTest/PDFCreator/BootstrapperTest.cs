using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using NUnit.Framework;
using pdfforge.Obsidian.Interaction;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.Services.Logging;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.Core.Startup.StartConditions;
using pdfforge.PDFCreator.Core.StartupInterface;
using pdfforge.PDFCreator.Core.Workflow;
using pdfforge.PDFCreator.Editions.EditionBase;
using pdfforge.PDFCreator.Editions.PDFCreator;
using pdfforge.PDFCreator.Editions.PDFCreatorBusiness;
using pdfforge.PDFCreator.Editions.PDFCreatorCustom;
using pdfforge.PDFCreator.Editions.PDFCreatorPlus;
using pdfforge.PDFCreator.Editions.PDFCreatorTerminalServer;
using pdfforge.PDFCreator.UI.ViewModels.Assistants.Update;
using SimpleInjector;
using SimpleInjector.Diagnostics;

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

        private void EnsureApplicationResources()
        {
            if (Application.Current == null)
            {
                // create the Application object
                var app = new Application();

                // merge in your application resources
                app.Resources.MergedDictionaries.Add(
                    Application.LoadComponent(
                        new Uri("PDFCreator.Views;component/Resources/AllResources.xaml",
                            UriKind.Relative)) as ResourceDictionary);
            }
        }

        [Test]
        [TestCaseSource(nameof(AllBootstrappers))]
        public void AllBootstrappers_ContainDefaultStartupConditions(Bootstrapper bootstrapper)
        {
            var container = new Container();
            bootstrapper.ConfigureContainer(container, null);

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

            var container = new Container();
            bootstrapper.ConfigureContainer(container, null);

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

            var container = new Container();
            bootstrapper.ConfigureContainer(container, null);

            var conditions = container.GetAllInstances<IStartupCondition>();
            var types = conditions.Select(c => c.GetType());

            if (isTerminalServerEdition)
                CollectionAssert.DoesNotContain(types, typeof(TerminalServerNotAllowedCondition));
            else
                CollectionAssert.Contains(types, typeof(TerminalServerNotAllowedCondition));
        }

        [Test]
        [TestCaseSource(nameof(AllBootstrappers))]
        [Ignore("Information can usually be ignored, this test can be run manually for more info about the container configuration")]
        public void ConfigurationOfContainer_CanBeValidatedWithoutInfo(Bootstrapper bootstrapper)
        {
            var container = new Container();
            bootstrapper.ConfigureContainer(container, null);
            Assert.DoesNotThrow(() => { container.Verify(VerificationOption.VerifyOnly); });

            var result = Analyzer.Analyze(container);
            var message = "";
            foreach (var diagnosticResult in result)
                message += $"{diagnosticResult.Severity} | {diagnosticResult.DiagnosticType}: {diagnosticResult.Description} {Environment.NewLine}";

            Assert.IsFalse(result.Any(), message);
        }

        [Test]
        [TestCaseSource(nameof(AllBootstrappers))]
        public void ConfigurationOfContainer_CanBeValidatedWithoutWarnings(Bootstrapper bootstrapper)
        {
            var container = new Container();
            bootstrapper.ConfigureContainer(container, null);
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
            Assert.DoesNotThrow(() => bootstrapper.ConfigureContainer(container, null));
        }

        [Test]
        [TestCaseSource(nameof(AllBootstrappers))]
        [STAThread]
        public void ConfigurationOfContainer_WithInteractions_CanBeValidatedWithoutWarnings(Bootstrapper bootstrapper)
        {
            EnsureApplicationResources();

            var container = new Container();
            var windowResolver = new TestWindowResolver(container);
            var windowRegistry = new WindowRegistry(windowResolver);

            bootstrapper.ConfigureContainer(container, windowRegistry);
            bootstrapper.RegisterInteractions(windowRegistry);

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
            var container = new Container();
            bootstrapper.ConfigureContainer(container, null);

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
    }

    internal class TestWindowResolver : IWindowResolver
    {
        private readonly Container _container;

        public TestWindowResolver(Container container)
        {
            _container = container;
        }

        public object ResolveInstance(Type type)
        {
            throw new NotImplementedException();
        }

        public void RegisterWindow(Type type)
        {
            if (typeof(Window).IsAssignableFrom(type))
                _container.Register(type);
        }
    }
}
