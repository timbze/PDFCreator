using pdfforge.Obsidian.Interaction;
using pdfforge.PDFCreator.Conversion.Ghostscript;
using pdfforge.PDFCreator.Editions.EditionBase;
using pdfforge.PDFCreator.Editions.PDFCreator;
using SimpleInjector;

namespace PDFCreator.TestUtilities
{
    public class IntegrationTestBootstrapper
    {
        private readonly Bootstrapper _bootstrapper = new PDFCreatorBootstrapper();

        public Container ConfigureContainer()
        {
            var container = new Container();
            _bootstrapper.ConfigureContainer(container, new WindowRegistry(null));

            OverrideRegistrations(container);

            return container;
        }

        private void OverrideRegistrations(Container container)
        {
            container.Options.AllowOverridingRegistrations = true;

            container.Register<IGhostscriptDiscovery, PaketGhostscriptDiscovery>();

            container.Options.AllowOverridingRegistrations = false;
        }
    }
}
