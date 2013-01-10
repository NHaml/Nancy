namespace Nancy.ViewEngines.NHaml.Tests
{
    public class DummyNHamlTemplate : NancyNHamlTemplateBase
    {
        protected override void CoreRender(System.IO.TextWriter textWriter)
        {
            base.CoreRender(textWriter);
            textWriter.Write("Hello World");
        }
    }
}
