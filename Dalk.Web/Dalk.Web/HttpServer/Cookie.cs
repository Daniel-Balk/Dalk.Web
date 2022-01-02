namespace Dalk.Web.HttpServer
{
    public class Cookie
    {
        public string Name { get; }
        public string Value { get; }
        public Cookie(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
}