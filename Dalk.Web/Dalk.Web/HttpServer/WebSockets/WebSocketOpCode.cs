namespace Dalk.Web
{
    public enum WebSocketOpCode : byte
    {
        Text = 1,
        Binary = 2,
        Close = 8,
        Ping = 9,
        Pong = 10
    }
}