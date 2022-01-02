namespace Dalk.Web.ClassPageWebServer
{
    public interface IPage
    {
        bool MatchesRoute(string route);
    }
}