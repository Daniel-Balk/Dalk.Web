using Dalk.Web;
using Dalk.Web.ClassPageWebServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    public class Layout : LayoutPage
    {
        public override string GetCompletePage(WebPage page)
        {
            HtmlBuilder hb = new();
            hb.WriteMarkupString((MarkupString)page.BuildHtml());
            return hb.ToString();
        }
    }
}
