using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dalk.Web.ClassPageWebServer.Components
{
    public class PageHead : WebComponent
    {
        public override string GetHtml(HtmlBuilder builder)
        {
            builder.WriteMarkupString("<head>");
            builder.AddComponents(Components);
            if (!string.IsNullOrWhiteSpace(Title))
                builder.AddComponent(new Title() { Content = Title });
            builder.WriteMarkupString("</head>");
            return builder.ToString();
        }

        public string Title { get; set; } = null;

        public void AddStylesheet(string path)
        {
            Components.Add(new Stylesheet() { Path = path });
        }
    }
}
