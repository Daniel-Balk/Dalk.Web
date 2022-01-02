using System.Collections.Generic;

namespace Dalk.Web
{
    public class HtmlBuilder
    {
        private string blStr = "";
        public void AddComponent(RenderFragment component)
        {
            blStr += component.BuildHtml();
        }
        public void WriteMarkupString(MarkupString markupString)
        {
            blStr += markupString.ToString();
        }
        public void WriteString(string str)
        {
            blStr += ProtectXSS(str);
        }
        public void AddComponents(IEnumerable<RenderFragment> components)
        {
            foreach (var component in components)
            {
                AddComponent(component);
            }
        }
        public string GetString()
        {
            return blStr;
        }

        private static string ProtectXSS(string unsave)
        {
            return unsave
                .Replace("<", "&lt;")
                .Replace("≤", "&le;")
                .Replace("≥", "&ge;")
                .Replace(">", "&gt;");
        }
        public override string ToString()
        {
            return GetString();
        }
    }
}