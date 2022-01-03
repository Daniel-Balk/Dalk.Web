using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dalk.Web.ClassPageWebServer.Components
{
    public class PageBody : WebComponent
    {
        public override string GetHtml(HtmlBuilder builder)
        {
            builder.WriteMarkupString("<body>");
            builder.AddComponents(Components);
            builder.WriteMarkupString("</body>");
            return builder.ToString();
        }
    }
}
