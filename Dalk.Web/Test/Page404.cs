using Dalk.Web.ClassPageWebServer;
using Dalk.Web.ClassPageWebServer.Components;
using Dalk.Web.HttpServer;

namespace Test
{
    public class Page404 : WebPage
    {
        public override void Initialize(HttpRequest request)
        {
            base.Initialize(request);
        }

        public override string GetHtml(HtmlBuilder builder)
        {
            builder.AddComponent(new Heading1() { Content = "404" });
            return builder.ToString();
        }
    }
}