using Dalk.Web.HttpServer;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Dalk.Web.ClassPageWebServer
{
    public class WebServer
    {
        public List<Thread> RenderThreads { get; set; }
        public Thread ListenerThread { get; set; }
        public int Threads { get; set; } = 32;
        public int Port { get; set; } = 5000;
        private HttpListener HttpListener { get; set; }
        public List<WebPage> Pages { get; set; }
        public LayoutPage Layout { get; set; }

        public WebServer()
        {
            RenderThreads = new List<Thread>();
            Pages = new List<WebPage>();
            Layout = new LayoutPage();
        }

        public void Run()
        {
            ListenerThread = new Thread(ListenWebPage);
            ListenerThread.Name = "Listener";
            ListenerThread.Start();
        }

        private void ListenWebPage()
        {
            HttpListener = new HttpListener(Port);
            HttpListener.Start();
            while (true)
            {
                try
                {
                    var request = HttpListener.AcceptRequest();
                    Thread handleThread = new Thread(HandleRequest);
                    ThreadAdd(handleThread);
                    handleThread.Start(request);
                }
                catch (Exception ex)
                {
                    Error(ex.ToString());
                }
            }
        }

        private void ThreadAdd(Thread thread)
        {
            if(RenderThreads.Count < Threads)
            {
            }
            else
            {
                RenderThreads[0].Interrupt();
            }
            RenderThreads.Add(thread);
        }

        private void HandleRequest(object obj)
        {
            try
            {
                var request = obj as HttpRequest;
                var tcpSender = request.GetSender();
                Info($"Accesslog: Access from {tcpSender.Client.RemoteEndPoint}");
                var response = request.GetResponse();
                string html = null;
                Pages.ForEach(p =>
                {
                    if (p.MatchesRoute(request.Path))
                    {
                        html = Layout.GetCompletePage(p);
                    }
                });
                if (html == null)
                    html = Layout.Get404Page();
                var bytes = Encoding.UTF8.GetBytes(html);
                response.ContentLenght = bytes.Length;
                response.Write(bytes);
                response.Send();
                Thread.CurrentThread.Interrupt();
            }
            catch (Exception ex)
            {
                Error(ex.ToString());
            }
        }

        public event LogEventHandler Log;

        private void Debug(string message)
        {
            Log?.Invoke(LogLevel.Debug, message);
        }
        private void Info(string message)
        {
            Log?.Invoke(LogLevel.Info, message);
        }
        private void Warn(string message)
        {
            Log?.Invoke(LogLevel.Warn, message);
        }
        private void Error(string message)
        {
            Log?.Invoke(LogLevel.Error, message);
        }
    }
}
