using pdfforge.PDFCreator.Core.StartupInterface;
using pdfforge.PDFCreator.UI.COM;
using System;
using System.Collections.Generic;

namespace pdfforge.PDFCreator.IntegrationTest.UI.COM
{
    internal static class ComTestHelper
    {
        public static ComDependencies ModifyAndBuildComDependencies()
        {
            var builder = new ComDependencyBuilder();
            builder.ModifyRegistrations = container =>
            {
                container.Options.AllowOverridingRegistrations = true;
                container.RegisterCollection<IStartupCondition>(new List<Type>());
                container.Options.AllowOverridingRegistrations = false;
            };

            return builder.ComDependencies;
        }
    }
}
