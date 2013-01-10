namespace Nancy.ViewEngines.NHaml
{
    using System.Collections.Generic;
    using System.IO;
    using Responses;
    using System.Web.NHaml;
    using System.Web.NHaml.Configuration;

    public class NHamlViewEngine : IViewEngine
    {
        private readonly ITemplateEngine _templateEngine;

        public NHamlViewEngine()
            : this(XmlConfigurator.GetTemplateEngine())
        { }

        public NHamlViewEngine(ITemplateEngine templateEngine)
        {
            _templateEngine = templateEngine;
        }
        public IEnumerable<string> Extensions
        {
            get { yield return "haml"; }
        }

        public static string NHamlVersion
        {
            get
            {
                return typeof(ITemplateEngine).Assembly.GetName().Version.ToString();
            }
        }

        public void Initialize(ViewEngineStartupContext viewEngineStartupContext)
        {
            _templateEngine.TemplateContentProvider = new NancyTemplateContentProvider(viewEngineStartupContext);
        }

        public Response RenderView(ViewLocationResult viewLocationResult, dynamic model, IRenderContext renderContext)
        {
            return new HtmlResponse(
                contents: stream =>
                {
                    var templateFactory = renderContext.ViewCache.GetOrAdd(
                        viewLocationResult, x => GetTemplateFactory(viewLocationResult));
                    RenderTemplateFromTemplateFactory(templateFactory, stream, model, renderContext.Context);
                }
            );
        }

        private TemplateFactory GetTemplateFactory(ViewLocationResult viewLocationResult)
        {
            var view = new NancyNHamlView(viewLocationResult);
            return _templateEngine.GetCompiledTemplate(view, typeof(NancyNHamlTemplateBase));
        }

        private static void RenderTemplateFromTemplateFactory(TemplateFactory templateFactory, Stream stream,
            dynamic model, NancyContext context)
        {
            var streamWriter = new StreamWriter(stream);
            var template = (NancyNHamlTemplateBase)templateFactory.CreateTemplate();
            template.Model = model;
            template.Context = context;
            template.Render(streamWriter);
            streamWriter.Flush();
        }
    }
}
