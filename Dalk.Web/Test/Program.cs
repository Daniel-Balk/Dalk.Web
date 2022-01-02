using Dalk.Web;
using Dalk.Web.HttpServer;
using System;
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
                var response = request.GetResponse();
                var bytes = Encoding.UTF8.GetBytes($"<h1>IT WORKS</h1><p>Route: {request.Path}</p>");
                response.ContentLenght = bytes.Length;
                response.Write(bytes);
                response.Send();
                //Console.WriteLine($"{rq.Method} {rq.Path}");
            }
        }
    }
}
