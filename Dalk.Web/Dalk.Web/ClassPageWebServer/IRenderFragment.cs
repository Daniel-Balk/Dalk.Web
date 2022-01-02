namespace Dalk.Web
{
    public interface IRenderFragment
    {
        string BuildHtml();
        string ProtectXSS(string unsave);
    }
}