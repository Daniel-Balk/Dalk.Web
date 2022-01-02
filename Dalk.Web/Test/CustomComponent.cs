using Dalk.Web;
using Dalk.Web.ClassPageWebServer;
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
            
            return builder.ToString();
        }
    }
}
