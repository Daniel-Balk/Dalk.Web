namespace Dalk.Web.ClassPageWebServer.Components
{
    public class Title : WebComponent
    {
        public string Content { get; set; }
        public override string GetHtml(HtmlBuilder builder)
        {
            builder.WriteMarkupString("<title>");
            builder.WriteString(Content);
            builder.WriteMarkupString("</title>");
            return builder.ToString();
        }
    }
}