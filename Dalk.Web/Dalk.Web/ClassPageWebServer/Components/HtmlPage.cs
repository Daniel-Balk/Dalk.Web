using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dalk.Web.ClassPageWebServer.Components
{
    public class HtmlPage : WebComponent
    {
        public override string GetHtml(HtmlBuilder builder)
        {
            builder.WriteMarkupString("<!DOCTYPE html>");
            builder.WriteMarkupString("<html>");
            builder.AddComponents(Components);
            builder.WriteMarkupString("</html>");
            return builder.ToString();
        }
    }
}
