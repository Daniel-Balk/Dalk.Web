using System.Collections.Generic;

namespace Dalk.Web.ClassPageWebServer
{
    public class RenderFragment : IRenderFragment
    {
        public string BuildHtml()
        {
            HtmlBuilder builder = new HtmlBuilder();
            return GetHtml(builder);
        }

        public virtual string GetHtml(HtmlBuilder builder)
        {
            builder.AddComponents(Components);
            return builder.ToString();
        }

        public List<RenderFragment> Components { get; set; }

        public string ProtectXSS(string unsave)
        {
            return unsave
                .Replace("<", "&lt;")
                .Replace("≤", "&le;")
                .Replace("≥", "&ge;")
                .Replace(">", "&gt;");
        }

        public RenderFragment()
        {
            Components = new List<RenderFragment>();
        }
    }
}