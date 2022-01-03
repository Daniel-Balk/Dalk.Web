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
            PageBody body = new();
            head.AddStylesheet("/style.css");
            body.Components.Add(page);

            html.Components.Add(head);
            html.Components.Add(body);
            hb.AddComponent(html);
            return hb.ToString();
        }
        public override string Get404Page()
        {
            return GetCompletePage(new Page404());
        }
    }
}
