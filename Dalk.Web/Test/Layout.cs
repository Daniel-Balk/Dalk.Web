using Dalk.Web;
using Dalk.Web.ClassPageWebServer;
using Dalk.Web.ClassPageWebServer.Components;
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
            HtmlPage html = new();
            PageHead head = new();
            //head.AddStylesheet("/1style.css");
            html.Components.Add(head);
            hb.AddComponent(html);
            hb.WriteMarkupString(page.BuildHtml());
            return hb.ToString();
        }
        public override string Get404Page()
        {
            return GetCompletePage(new Page404());
        }
    }
}
