using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.Json;
using CS3500.Networking;
using GUI.Client.Models;
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
            //attempt to connect
            connection.Connect(host, port);
            //send name across connection
            connection.Send(name);
        }


        public async void ReadServerMessageAsync() 
        {
            bool first = true;
            bool second = true;
            while (connection.IsConnected)
            {
                try
                {
                    string message = await ReadFromServerAsync();
                    if (!string.IsNullOrEmpty(message))
                    {
                        if (first && int.TryParse(message, out int ID))
                        {
                            id = ID;
                            first = false;
                        }
                        else if (second && int.TryParse(message, out int size))
                        {
                            world = new World(size);
                            second = false;
                        }
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
            if (message.Contains("\"snake\""))
            {
                Snake? snake = JsonSerializer.Deserialize<Snake>(message);
                if (snake.died == true)
                {
                    //remove/hide snake in model
                    world.RemoveSnake(snake);
                }
                else
                {
                    //update or add snake in model
                    world.AddSnake(snake);
                }
            }
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
            else if (message.Contains("\"wall\""))
            {
                Wall? wall = JsonSerializer.Deserialize<Wall>(message);
                if (wall != null)
                {
                    world.AddWall(wall);
                }
            }
        }

        /// <summary>
        /// public wrapper method to handle arrow key presses from GUI
        /// </summary>
        /// <param name="key"></param>
        public void HandleKey(string key) 
        {
            if(key != null)
                key = key.ToLowerInvariant();
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

        private async Task<string> ReadFromServerAsync()
        {
                return await Task.Run(() => connection.ReadLine() ?? String.Empty);
        }

    }
}
