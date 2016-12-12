using System;
using pdfforge.Obsidian.Interaction;
using SimpleInjector;

namespace pdfforge.PDFCreator.Editions.EditionBase
{
    internal class SimpleInjectorWindowResolver : IWindowResolver
    {
        private readonly Container _container;

        public SimpleInjectorWindowResolver(Container container)
        {
            _container = container;
        }

        public object ResolveInstance(Type type)
        {
            return _container.GetInstance(type);
        }

        public void RegisterWindow(Type type)
        {
        }
    }
}
