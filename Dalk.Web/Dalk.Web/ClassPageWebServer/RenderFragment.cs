using System.Collections.Generic;

namespace Dalk.Web
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
            builder.WriteString("<b>NoXSS, No Content</b>");
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
    }
}