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
    public class CustomComponent : WebComponent
    {
        public override string GetHtml(HtmlBuilder builder)
        {
            builder.AddComponent(new Heading1() { Content = "Awesome Site" });
            builder.AddComponent(new Heading2() { Content = "Awesome Site 2" });
            return builder.ToString();
        }
    }
}
