using System;
using System.Collections.Generic;
using System.Diagnostics;
using CS3500.Networking;

namespace WebServer
{
    public static class WebServer
    {
        private const string httpGoodHeader =
            "HTTP/1.1 200 OK\r\n" +
            "Connection: close\r\n" +
            "Content-Type: text/html; charset=UTF-8\r\n" +
            "\r\n";
        private const string httpBadHeader =
            "HTTP/1.1 404 Not Found\r\n" +
            "Connection: close\r\n" +
            "Content-Type: text/html; charset=UTF-8\r\n" +
            "\r\n";
        public static void Main(string[] args)
        {
            Server.StartServer(HandleConnection, 80);
            Console.Read();
        }

        private static void HandleConnection(NetworkConnection connection)
        {
            string request = connection.ReadLine();
            Debug.WriteLine(request);
            if(request.Contains("GET / "))
            {
                connection.Send(httpGoodHeader + "<html>\r\n  <h3>Welcome to the Snake Games Database!</h3>\r\n  <a href=\"/games\">View Games</a>\r\n</html>");

            }   
            else if(request.Contains("GET /games"))
            {
                connection.Send(httpGoodHeader + "...");
            }
            //else if(request.Contains($"Get /games?gid={x}"))
            //{
            //    connection.Send(httpGoodHeaders  + "")
            //}
            connection.Disconnect();
        }
    }
}
