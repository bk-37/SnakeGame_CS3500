using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text.Json;
using CS3500.Networking;
using GUI.Client.Models;
using GUI.Client.Pages;
using MySql.Data.MySqlClient;
namespace GUI.Client.Controllers
{
    /// <summary>
    /// Responsible for parsing information received from the network and
    /// updating the model based on that information.
    /// <authors>
    /// Brian Keller & Wyatt Young
    /// </authors>
    /// <versions>
    /// November 17th, 2024
    /// </versions>
    /// </summary>
    public class NetworkController
    {
        /// <summary>
        /// Member variable to represent our network connection
        /// </summary>
        public NetworkConnection connection {  get; private set; } = new NetworkConnection();   

        /// <summary>
        /// property to hold the THIS player's snake object
        /// </summary>
        public Snake snake {  get; private set; }

        /// <summary>
        /// property to hold a World object that represnets current state of game from server. 
        /// </summary>
        public World world { get; private set; } = new World(1000);

        /// <summary>
        /// field representing the size of the world
        /// </summary>
        public static int worldSize;

        /// <summary>
        /// field holding the player's id
        /// </summary>
        public int id { get; private set; }

        /// <summary>
        /// The connection string.
        /// Your uID login name serves as both your database name and your uid
        /// </summary>
        public const string connectionString = "server=atr.eng.utah.edu;" +
          "database=u1364562;" +
          "uid=u1364562;" +
          "password=BME_BLAZOR42069";

        /// <summary>
        /// Dictionary for holding snakes' max scores with their player id's
        /// </summary>
        private Dictionary<int, int> maxScores = new();

        /// <summary>
        /// Method for connecting to server and sending it the player's name
        /// </summary>
        /// <param name="name">name of player connecting</param>
        /// <param name="host">host server, deafult to localhost</param>
        /// <param name="port">port for game server</param>
        /// <returns></returns>
        public void ConnectToServer(string name, string host, int port)
        {
            //if name is too long, concatenate it to the proper length
            if (name.Length > 16)
            {
                name = name.Substring(0, 16);
            }
            //attempt to connect and send name
            try
            {
                connection.Connect(host, port);
                connection.Send(name);
            }
            catch(SocketException ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Method called by the game loop to read incoming messages from the server and parse them according to their order 
        /// </summary>
        public async void ReadServerMessageAsync() 
        {
            //variables for tracking the first and second messages read status
            bool first = true;
            bool second = true;
            while (connection.IsConnected)
            {
                try
                {
                    //get message from server
                    string message = await ReadFromServerAsync();
                    if (!string.IsNullOrEmpty(message))
                    {
                        //if message is the first message sent, set it as ID
                        if (first && int.TryParse(message, out int ID))
                        {
                            id = ID;
                            first = false;
                        }
                        //if message is the second message sent, set it as world size
                        else if (second && int.TryParse(message, out int size))
                        {
                            world = new World(size);
                            second = false;
                        }
                        //otherwise, parse the message according to the Json elements
                        else
                            ParseMessage(message);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error processing message: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Helper method for parsing and deserializing incoming messages from the server
        /// </summary>
        /// <param name="message"></param>
        private void ParseMessage(string message)
        {
            //Parse snake objects
            if (message.Contains("\"snake\""))
            {
                Snake? snake = JsonSerializer.Deserialize<Snake>(message);
                if (snake.died || snake.dc)
                {
                    if (snake.dc && world.snakes.ContainsKey(snake.snake))
                    {
                        sendToDataBase($"update Players set leaveTime = '{DateTime.Now:yyyy-MM-dd HH:mm:ss}' where playerID = {snake.snake}");
                    }
                    //remove/hide snake in model
                    world.RemoveSnake(snake);
                }
                else
                {
                    if (snake.join && !world.snakes.ContainsKey(snake.snake))
                    {
                        maxScores[snake.snake] = snake.score;
                        int gameID = SnakeGUI.gameID;
                        sendToDataBase($"insert into Players (playerID, name, maxScore, enterTime, gameID) values ('{snake.snake}', '{snake.name}', '{maxScores[snake.snake]}', '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}', '{gameID}')");
                    }
                    //add snake to world
                    world.AddSnake(snake);
                }
                if(snake.score > maxScores[snake.snake])
                {
                    maxScores[snake.snake] = snake.score;
                    sendToDataBase("update Players set maxScore =" + maxScores[snake.snake] + " where playerID = " + snake.snake + " and gameID = last_insert_id();");
                }
            }
            //parse powerup objects
            else if (message.Contains("\"power\""))
            {
                Power? power = JsonSerializer.Deserialize<Power>(message);
                if (power.died == true)
                {
                    //remove/hide power up in model
                    world.RemovePowerup(power);
                }
                else
                {
                    //update or add power up in model
                    world.AddPowerup(power);
                }
            }
            //parse wall objects
            else if (message.Contains("\"wall\""))
            {
                Wall? wall = JsonSerializer.Deserialize<Wall>(message);
                if (wall != null)
                {
                    //add wall to world
                    world.AddWall(wall);
                }
            }
        }

        /// <summary>
        /// public wrapper method to handle arrow key presses from GUI
        /// </summary>
        /// <param name="key"> string representation of key pressed </param>
        public void HandleKey(string key) 
        {
            if(key != null)
                key = key.ToLowerInvariant();
            //determine which key was pressed via switch case and send the corresponding message to the server via Json
            switch (key)
            {
                case "w":
                case "arrowup":
                    connection.Send(JsonSerializer.Serialize(new { moving = "up" }));
                    break;
                case "a":
                case "arrowleft":
                    connection.Send(JsonSerializer.Serialize(new { moving = "left" }));
                    break;
                case "s":
                case "arrowdown":
                    connection.Send(JsonSerializer.Serialize(new { moving = "down" }));
                    break;
                case "d":
                case "arrowright":
                    connection.Send(JsonSerializer.Serialize(new { moving = "right" }));
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Helper method for reading lines sent from the server
        /// </summary>
        /// <returns></returns>
        private async Task<string> ReadFromServerAsync()
        {
                return await Task.Run(() => connection.ReadLine() ?? String.Empty);
        }

        /// <summary>
        /// Helper method to send queries to the database
        /// </summary>
        /// <param name="query"> the query being sent to mySQL </param>
        private static void sendToDataBase(string query)
        {
            //connect to the database and send a start time
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                MySqlCommand command = conn.CreateCommand();
                command.CommandText = query;
                command.ExecuteNonQuery();
            }
        }
    }
}
