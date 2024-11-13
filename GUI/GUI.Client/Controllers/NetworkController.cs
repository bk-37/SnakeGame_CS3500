using System.Text.Json;
using CS3500.Networking;
using GUI.Client.Models;
namespace GUI.Client.Controllers
{
    /// <summary>
    /// Responsible for parsing information received from the network and
    /// updating the model based on that information
    /// </summary>
    public class NetworkController
    {
        /// <summary>
        /// Member variable to represent our network connection
        /// </summary>
        private readonly NetworkConnection connection;

        /// <summary>
        /// member variable to hold the player's integer ID number
        /// </summary>
        private int ID;

        /// <summary>
        /// Member variable to hold the world size
        /// </summary>
        private int worldSize;

        /// <summary>
        /// Constructor method for network controller 
        /// </summary>
        /// <param name="connection"></param>
        public NetworkController(NetworkConnection connection)
        {
            this.connection = connection;
        }

        /// <summary>
        /// Method for connecting to server and sending it the player's name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public async Task ConnectToServer(string name, string host, int port)
        {
            //if name is too long, concatenate it to the proper length
            if (name.Length > 16)
            {
                name = name.Substring(0, 16);
            }          
            connection.Connect(host, port);
            if (connection.IsConnected)
            {
                //send player's name to server once connected
                connection.Send(name);
                //receive player id and worldsize from server
                ID = int.Parse(connection.ReadLine() ?? "0");
                worldSize = int.Parse(connection.ReadLine() ?? "0");
                await GetUpdates();
            }
        }

        /// <summary>
        /// Helper method for parsing and deserializing incoming messages from the server
        /// </summary>
        /// <param name="message"></param>
        private void ParseMessage(string message)
        {
            if (message.Contains("\"wall\""))
            {
                Wall? wall = JsonSerializer.Deserialize<Wall>(message);
            }
            else if (message.Contains("\"snake\""))
            {
                Snake? snake = JsonSerializer.Deserialize<Snake>(message);
            }
            else if (message.Contains("\"power\""))
            {
                Power? power = JsonSerializer.Deserialize<Power>(message);
            }
        }

        /// <summary>
        /// Helper method for sending commands as JSON objects
        /// </summary>
        /// <param name="direction"></param> corresponds to input direction (up, down, left, right)
        private void SendMovementDirection(string direction) 
        {
            //create an object to hold the command with the moving keyword
            var sendable = new {moving = direction};
            //serialize into JSON 
            string json = JsonSerializer.Serialize(sendable);
            //send it
            connection.Send(json);
        }

        /// <summary>
        /// Helper method for handling up key press
        /// </summary>
        public void HandleUp()
        {
            SendMovementDirection("up");
        }


        /// <summary>
        /// Helper method for handling down key press
        /// </summary>
        public void HandleDown()
        {
            SendMovementDirection("down");
        }


        /// <summary>
        /// Helper method for handling left key press
        /// </summary>
        public void HandleLeft()
        {
            SendMovementDirection("left");
        }


        /// <summary>
        /// Helper method for handling right key press
        /// </summary>
        public void HandleRight()
        {
            SendMovementDirection("right");
        }

        /// <summary>
        /// Method to continuously update from the server
        /// </summary>
        /// <returns></returns>
        private async Task GetUpdates()
        {
            while (connection.IsConnected)
            {
                string message = connection.ReadLine();
                if (!string.IsNullOrEmpty(message))
                {
                    ParseMessage(message);
                }
            }

        }

    }
}
