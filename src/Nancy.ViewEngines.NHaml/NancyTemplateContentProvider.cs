namespace Nancy.ViewEngines.NHaml
{
    using System;
    using System.Linq;
    using System.Web.NHaml.TemplateResolution;
    using System.IO;
    using Nancy.Responses.Negotiation;

    public class NancyTemplateContentProvider : ITemplateContentProvider
    {
        private readonly ViewEngineStartupContext viewEngineStartupContext;

        public NancyTemplateContentProvider(ViewEngineStartupContext viewEngineStartupContext)
        {
            this.viewEngineStartupContext = viewEngineStartupContext;
        }

        public ViewSource GetViewSource(string templateName)
        {
            var searchPath = ConvertPath(templateName);

            var viewLocationResult = this.viewEngineStartupContext.ViewLocator.LocateView(templateName, GetFakeContext());

            if (viewLocationResult == null)
                throw new FileNotFoundException(string.Format("Template {0} not found", templateName), templateName);

            return new NancyNHamlView(viewLocationResult);
        }

        // Horrible hack, but we have no way to get a context
        private static NancyContext GetFakeContext()
        {
            var ctx = new NancyContext();
            ctx.Request = new Request("GET", "/", "http");
            ctx.NegotiationContext = new NegotiationContext();
            return ctx;
        }

        private static string ConvertPath(string path)
        {
            return path.Replace(@"\", "/");
        }

        private static bool CompareViewPaths(string storedViewPath, string requestedViewPath)
        {
            return String.Equals(storedViewPath, requestedViewPath, StringComparison.OrdinalIgnoreCase);
        }
    }
}
