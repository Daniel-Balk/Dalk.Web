using Dalk.Web.HttpServer;

namespace Dalk.Web.ClassPageWebServer
{
    public class WebPage : WebComponent, IPage
    {
        public virtual void Initialize(HttpRequest request)
        {
        }

        public virtual bool MatchesRoute(string route)
        {
            return false;
        }
    }
}