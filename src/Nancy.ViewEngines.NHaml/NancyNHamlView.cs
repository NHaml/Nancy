namespace Nancy.ViewEngines.NHaml
{
    using global::System.Web.NHaml.TemplateResolution;

    public class NancyNHamlView : StreamViewSource
    {
        public NancyNHamlView(ViewLocationResult viewLocationResult)
            : base(viewLocationResult.Contents.Invoke(),
            viewLocationResult.GetSafeViewPath().Replace("/", "\\"))
        { }
    }
}
