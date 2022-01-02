namespace Dalk.Web
{
    public struct MarkupString
    {
        public MarkupString(string s)
        {
            String = s;
        }

        private string String { get; }

        public static implicit operator MarkupString(string s)
        {
            return new MarkupString(s);
        }

        public override string ToString()
        {
            return String;
        }
    }
}