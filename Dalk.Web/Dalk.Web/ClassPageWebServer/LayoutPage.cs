namespace Dalk.Web.ClassPageWebServer
{
    public class LayoutPage : WebPage, ILayoutPage
    {
        public LayoutPage()
        {
        }

        public virtual string Get404Page()
        {
            return "404";
        }

        public virtual string GetCompletePage(WebPage page)
        {
            return page.BuildHtml();
        }

        public virtual void InitializeLayout()
        {
            
        }
    }
}