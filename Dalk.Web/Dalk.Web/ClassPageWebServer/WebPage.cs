namespace Dalk.Web.ClassPageWebServer
{
    public class WebPage : WebComponent, IPage
    {
        public virtual bool MatchesRoute(string route)
        {
            return false;
        }
    }
}