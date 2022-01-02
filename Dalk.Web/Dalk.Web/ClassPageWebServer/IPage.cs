using Dalk.Web.HttpServer;

namespace Dalk.Web.ClassPageWebServer
{
    public interface IPage
    {
        bool MatchesRoute(string route);
        void Initialize(HttpRequest request);
    }
}