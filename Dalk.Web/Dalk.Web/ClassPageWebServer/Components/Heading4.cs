using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dalk.Web.ClassPageWebServer.Components
{
    public class Heading4 : WebComponent
    {
        public string Content { get; set; }
        private static readonly int hn = 4;
        public override string GetHtml(HtmlBuilder builder)
        {
            builder.WriteMarkupString($"<h{hn}>");
            builder.WriteString(Content);
            builder.WriteMarkupString($"</h{hn}>");
            return builder.ToString();
        }
    }
}
