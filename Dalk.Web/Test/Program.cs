﻿using Dalk.Web;
using Dalk.Web.ClassPageWebServer;
using Dalk.Web.HttpServer;
using Logging.Net;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Test
{
    internal class Program
    {
        static void Main(string[] args)
        {
            HttpListener lstn = new(5000);
            lstn.Start();
            while (true)
            {
                var request = lstn.AcceptRequest();
                if (request.IsWebsocket)
                {
                    var ws = request.AcceptWebSocket();
                    Console.WriteLine("Created ws");

                    ws.OnMessage += (s, e) =>
                    {
                        var text = Encoding.UTF8.GetString(e.Payload);
                        if (text == "Ping")
                            ws.Send("Pong!");
                    };

                    continue;
                }

                var response = request.GetResponse();
                var bytes = Encoding.UTF8.GetBytes($"<h1>IT WORKS</h1><p>Route: {request.Path}</p>");
                response.ContentLenght = bytes.Length;
                response.Write(bytes);
                response.Send();
                //Console.WriteLine($"{rq.Method} {rq.Path}");
            }
            /*WebServer ws = new();
            ws.Port = 5000;
            ws.SetLayout<Layout>();
            ws.RegisterPage<IndexPage>();
                ws.Log += new LogEventHandler((l, m) =>
            {
                switch (l)
                {
                    case LogLevel.Debug:
                        Logger.Debug(m);
                        break;
                    case LogLevel.Warn:
                        Logger.Warn(m);
                        break;
                    case LogLevel.Info:
                        Logger.Info(m);
                        break;
                    case LogLevel.Error:
                        Logger.Error(m);
                        break;
                }
            });
            ws.Run();*/
        }
    }
}
