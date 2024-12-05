using System;
using System.Collections.Generic;
using System.Diagnostics;
using CS3500.Networking;
using MySql.Data.MySqlClient;

namespace WebServer
{
    public static class WebServer
    {
        /// <summary>
        /// The connection string.
        /// Your uID login name serves as both your database name and your uid
        /// </summary>
        private const string connectionString = "server=atr.eng.utah.edu;" +
        "database=u1364562;" +
        "uid=u1364562;" +
        "password=BME_BLAZOR42069";
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
                string gameTable = GenerateGamesTable();
                Debug.WriteLine(gameTable);
                connection.Send(httpGoodHeader + gameTable);
            }
            //else if(request.Contains($"Get /games?gid={x}"))
            //{
            //    connection.Send(httpGoodHeaders  + "")
            //}
            connection.Disconnect();
        }
        private static string GenerateGamesTable()
        {
            string html = "<html>\r\n  <table border=\"1\">\r\n    <thead>\r\n      <tr>\r\n        <td>ID</td><td>Start</td><td>End</td>\r\n      </tr>\r\n    </thead>\r\n    <tbody>\r\n";

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    // Query the Games table
                    string query = "select gameID, startTime, endTime from Games";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Debug.WriteLine("Reader is reading");
                            int gameId = reader.GetInt32(0);
                            string startTime = reader.GetDateTime(1).ToString("yyyy-MM-dd HH:mm:ss");
                            string endTime = reader.IsDBNull(2) ? "Ongoing" : reader.GetDateTime(2).ToString("yyyy-MM-dd HH:mm:ss");

                            html += $"      <tr>\r\n        <td><a href=\"/games?gid={gameId}\">{gameId}</a></td>\r\n        <td>{startTime}</td>\r\n        <td>{endTime}</td>\r\n      </tr>\r\n";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Database error: {ex.Message}");
                html += "      <tr>\r\n        <td colspan=\"3\">Error loading games</td>\r\n      </tr>\r\n";
            }

            html += "    </tbody>\r\n  </table>\r\n</html>";
            return html;
        }
    }
}
