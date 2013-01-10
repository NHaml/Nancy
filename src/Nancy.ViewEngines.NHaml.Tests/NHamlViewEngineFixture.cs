namespace Nancy.ViewEngines.NHaml.Tests
{
    using Xunit;
    using Nancy.Tests;
    using FakeItEasy;
    using System;
    using System.IO;
    using NHaml;
    using global::System.Web.NHaml;
    using global::System.Web.NHaml.TemplateResolution;
    
    public class NHamlViewEngineFixture
    {
        private readonly NHamlViewEngine engine;
        private readonly ITemplateEngine nHamlEngine;
        private readonly IRenderContext renderContext;

        public NHamlViewEngineFixture()
        {
            nHamlEngine = A.Fake<ITemplateEngine>();
            engine = new NHamlViewEngine(nHamlEngine);

            A.CallTo(() => nHamlEngine.GetCompiledTemplate(A<ViewSource>.Ignored, A<Type>.Ignored))
                .Returns(new TemplateFactory(typeof(DummyNHamlTemplate)));

            var cache = A.Fake<IViewCache>();
            A.CallTo(() => cache.GetOrAdd(A<ViewLocationResult>.Ignored, A<Func<ViewLocationResult, TemplateFactory>>.Ignored))
                .ReturnsLazily(x =>
                {
                    var result = x.GetArgument<ViewLocationResult>(0);
                    return x.GetArgument<Func<ViewLocationResult, TemplateFactory>>(1).Invoke(result);
                });

            this.renderContext = A.Fake<IRenderContext>();
            A.CallTo(() => this.renderContext.ViewCache).Returns(cache);
        }

        [Fact]
        public void Engine_Should_Implement_IViewEngine()
        {
            engine.ShouldBeOfType<IViewEngine>();
        }

        [Fact]
        public void Extensions_should_return_nhaml()
        {
            var extensions = this.engine.Extensions;
            extensions.ShouldEqualSequence(new[] { "haml" });
        }

        [Fact]
        public void Render_should_return_response()
        {
            var result = engine.RenderView(GetDummyViewLocationResult(), null, this.renderContext);
            result.ShouldBeOfType<Response>();
        }

        [Fact]
        public void Render_should_invoke_NHaml()
        {            
            // Given, When
            var template = engine.RenderView(GetDummyViewLocationResult(), null, this.renderContext);
            template.Contents.Invoke(new MemoryStream());

            //Then
            A.CallTo(() => nHamlEngine.GetCompiledTemplate(A<ViewSource>.Ignored, A<Type>.Ignored))
                .MustHaveHappened();
        }

        [Fact]
        public void Render_should_create_valid_nhaml_IViewSource()
        {
            // Given
            var textReader = new StringReader("Hello world");
            var viewLocationResult = new ViewLocationResult("folder", "file", "haml", () => textReader);

            // When
            var template = engine.RenderView(viewLocationResult, null, this.renderContext);
            template.Contents.Invoke(new MemoryStream());

            // Then
            A.CallTo(() => nHamlEngine.GetCompiledTemplate(
                A<NancyNHamlView>.That.Matches(x => x.FilePath == @"folder\file.haml" && x.GetTextReader() == textReader),
                A<Type>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public void Render_should_return_nhaml_results()
        {
            // Given, When
            var result = engine.RenderView(GetDummyViewLocationResult(), null, this.renderContext);

            //Then
            var actualResult = new MemoryStream();
            const string expectedResult = "Hello World";
            result.Contents.Invoke(actualResult);
            actualResult.ShouldEqual(expectedResult);
        }

        [Fact]
        public void Render_should_use_cache()
        {
            // Given
            var viewLocationResult = GetDummyViewLocationResult();
            
            // When
            var template = engine.RenderView(viewLocationResult, null, this.renderContext);
            template.Contents.Invoke(new MemoryStream());
            
            //Then
            A.CallTo(() => this.renderContext.ViewCache.GetOrAdd(
                viewLocationResult,
                A<Func<ViewLocationResult,TemplateFactory>>.Ignored))
                .MustHaveHappened();
        }

        [Fact]
        public void Render_should_create_use_NancyNHamlTemplate()
        {
            // Given
            var textReader = new StringReader("Hello world");
            var viewLocationResult = new ViewLocationResult("folder", "file", "haml", () => textReader);

            // When
            var template = engine.RenderView(viewLocationResult, null, this.renderContext);
            template.Contents.Invoke(new MemoryStream());

            // Then
            A.CallTo(() => nHamlEngine.GetCompiledTemplate(
                A<NancyNHamlView>.Ignored,
                typeof(NancyNHamlTemplateBase))).MustHaveHappened();
        }

        private ViewLocationResult GetDummyViewLocationResult()
        {
            return new ViewLocationResult("folder", "file", "haml", () => new StringReader(""));
        }
    }
}
