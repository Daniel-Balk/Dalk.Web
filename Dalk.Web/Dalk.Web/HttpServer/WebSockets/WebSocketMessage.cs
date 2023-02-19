using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dalk.Web.HttpServer.WebSockets
{
    public class WebSocketMessage
    {
        public WebSocketMessage()
        {

        }

        public WebSocketOpCode Type { get; set; }
        public byte[] Payload { get; set; }
    }
}
