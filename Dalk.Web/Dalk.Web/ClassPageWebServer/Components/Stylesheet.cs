using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dalk.Web.ClassPageWebServer.Components
{
    public class Stylesheet : WebComponent
    {
        public string Path { get; set; }
        public override string GetHtml(HtmlBuilder builder)
        {
            builder.WriteMarkupString($@"<link rel=""stylesheet"" href=""{ProtectXSS(Path)}"" />");
            return builder.ToString();
        }
    }
}
