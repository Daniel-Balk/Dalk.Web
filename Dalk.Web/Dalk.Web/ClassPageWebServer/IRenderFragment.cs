namespace Dalk.Web.ClassPageWebServer
{
    public interface IRenderFragment
    {
        string BuildHtml();
        string ProtectXSS(string unsave);
    }
}