using System;
using System.Collections.Generic;
using System.Diagnostics;
using CS3500.Networking;
using MySql.Data.MySqlClient;

namespace WebServer
{
    /// <summary>
    /// Responsible for running the web server that allows access to game and player databases
    /// <authors>
    /// Brian Keller & Wyatt Young
    /// </authors>
    /// <versions>
    /// December 5th, 2024
    /// </versions>
    /// </summary>
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
        /// <summary>
        /// String representing a good http direct
        /// </summary>
        private const string httpGoodHeader =
            "HTTP/1.1 200 OK\r\n" +
            "Connection: close\r\n" +
            "Content-Type: text/html; charset=UTF-8\r\n" +
            "\r\n";

        /// <summary>
        /// String representing a bad http direct
        /// </summary>
        private const string httpBadHeader =
            "HTTP/1.1 404 Not Found\r\n" +
            "Connection: close\r\n" +
            "Content-Type: text/html; charset=UTF-8\r\n" +
            "\r\n";

        /// <summary>
        /// Method for beginning the server, runs indefintely once started
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            Server.StartServer(HandleConnection, 80);
            Console.Read();
        }

        /// <summary>
        /// Method for handling the http connection
        /// </summary>
        /// <param name="connection"> the network connection object used to connect</param>
        private static void HandleConnection(NetworkConnection connection)
        {
            string request = connection.ReadLine();
            //home page 
            if(request.Contains("GET / "))
            {
                connection.Send(httpGoodHeader + "<html>\r\n  <h3>Welcome to the Snake Games Database!</h3>\r\n  <a href=\"/games\">View Games</a>\r\n</html>");

            }   
            //games table page
            else if(request.Contains("GET /games "))
            {
                connection.Send(httpGoodHeader + GenerateGamesTable());
            }
            //players table
            else if(request.Contains($"GET /games?gid="))
            {
                string idParam = request.Split('=')[1].Split(' ')[0];
                Debug.WriteLine(idParam);
                if (int.TryParse(idParam, out int gameID))
                {
                    connection.Send(httpGoodHeader + GeneratePlayersTable(gameID));
                }
                else
                {
                    connection.Send(httpBadHeader + "<html><h3>Invalid Game ID</h3></html>");
                }
            }
            connection.Disconnect();
        }
        /// <summary>
        /// helper method for creating the html to display the games table
        /// </summary>
        /// <returns> html representation of games table </returns>
        private static string GenerateGamesTable()
        {
            string html = "<html>\r\n<table border=\"1\">\r\n<thead>\r\n<tr>\r\n<td>ID</td><td>Start</td><td>End</td>\r\n</tr>\r\n</thead>\r\n<tbody>\r\n";
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "select gameID, startTime, endTime from Games";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int gameId = reader.GetInt32(0);
                            string startTime = reader.GetDateTime(1).ToString("yyyy-MM-dd HH:mm:ss");
                            string endTime = reader.IsDBNull(2) ? "Ongoing" : reader.GetDateTime(2).ToString("yyyy-MM-dd HH:mm:ss");
                            html += $"<tr>\r\n<td><a href=\"/games?gid={gameId}\">{gameId}</a></td>\r\n<td>{startTime}</td>\r\n<td>{endTime}</td>\r\n</tr>\r\n";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Database error: {ex.Message}");
                html += "<tr>\r\n<td colspan=\"3\">Error loading games</td>\r\n</tr>\r\n";
            }
            html += "</tbody>\r\n</table>\r\n</html>";
            return html;
        }
        /// <summary>
        /// helper method for creating the player table 
        /// </summary>
        /// <param name="gameID"> game id of the players' game </param>
        /// <returns> html string representation of players table</returns>
        private static string GeneratePlayersTable(int gameID)
        {
            string html = $"<html>\r\n<h3>Stats for Game {gameID}</h3>\r\n<table border=\"1\">\r\n<thead>\r\n<tr>\r\n" +
                  "<td>Player ID</td><td>Player Name</td><td>Max Score</td><td>Enter Time</td><td>Leave Time</td>\r\n" +
                  "</tr>\r\n</thead>\r\n<tbody>\r\n";
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT playerID, name, maxScore, enterTime, leaveTime FROM Players WHERE gameID = @gameID";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@gameID", gameID);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int playerID = reader.GetInt32(0);
                                string playerName = reader.GetString(1);
                                int maxScore = reader.GetInt32(2);
                                string enterTime = reader.GetDateTime(3).ToString("yyyy-MM-dd HH:mm:ss");
                                string leaveTime = reader.IsDBNull(4) ? "Ongoing" : reader.GetDateTime(4).ToString("yyyy-MM-dd HH:mm:ss");

                                html += $"<tr>\r\n" +
                                        $"<td>{playerID}</td>\r\n" +
                                        $"<td>{playerName}</td>\r\n" +
                                        $"<td>{maxScore}</td>\r\n" +
                                        $"<td>{enterTime}</td>\r\n" +
                                        $"<td>{leaveTime}</td>\r\n" +
                                        $"</tr>\r\n";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Database error: {ex.Message}");
                html += "<tr>\r\n<td colspan=\"5\">Error loading players</td>\r\n</tr>\r\n";
            }
            html += "</tbody>\r\n</table>\r\n</html>";
            return html;
        }
    }
}
