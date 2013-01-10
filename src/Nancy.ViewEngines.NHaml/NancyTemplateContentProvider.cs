namespace Nancy.ViewEngines.NHaml
{
    using System;
    using System.Linq;
    using System.Web.NHaml.TemplateResolution;
    using System.IO;

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

            var viewLocationResult = this.viewEngineStartupContext.ViewLocationResults
                .FirstOrDefault(v => CompareViewPaths(v.GetSafeViewPath(), searchPath));

            if (viewLocationResult == null)
                throw new FileNotFoundException(string.Format("Template {0} not found", templateName), templateName);

            return new NancyNHamlView(viewLocationResult);
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
