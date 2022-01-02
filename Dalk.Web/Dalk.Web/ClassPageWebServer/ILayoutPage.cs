namespace Dalk.Web.ClassPageWebServer
{
    public interface ILayoutPage
    {
        void InitializeLayout();
        string GetCompletePage(WebPage page);
        string Get404Page();
    }
}