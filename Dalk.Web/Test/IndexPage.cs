using Dalk.Web;
using Dalk.Web.ClassPageWebServer;
using Dalk.Web.HttpServer;

namespace Test
{
    public class IndexPage : WebPage
    {
        public override string GetHtml(HtmlBuilder builder)
        {
            builder.WriteString("<b>It works</b>");
            return builder.ToString();
        }
        public override void Initialize(HttpRequest request)
        {
            base.Initialize(request);
        }
        public override bool MatchesRoute(string route)
        {
            return route == "/";
        }
    }
}