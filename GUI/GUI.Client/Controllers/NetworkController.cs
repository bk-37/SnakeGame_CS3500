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
        private readonly NetworkConnection connection;

        /// <summary>
        /// member variable to hold the THIS player's snake object
        /// </summary>
        private Snake snake;

        /// <summary>
        /// Member variable to hold a World object that represnets current state of game from server. 
        /// </summary>
        private World world;

        /// <summary>
        /// private member boolean that denotes whether the client has recieved
        /// the ID and world size and walls from server. Thus it may start making frame commands.
        /// </summary>
        private bool clientCommands = false;

        /// <summary>
        /// private member boolean that denotes whether a command has already been sent
        /// by the client during the current frame.
        /// </summary>
        private bool commandOnFrame = false;

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
                int id = int.Parse(connection.ReadLine() ?? "0");
                int worldSize = int.Parse(connection.ReadLine() ?? "0");
                // initialize player as well as world
                snake = new Snake(id);
                world = new World(worldSize);
                //recieve walls
                string? message;
                while ((message = connection.ReadLine()) != null) 
                {
                    if (message.Contains("\"wall\""))
                    {
                        Wall? wall = JsonSerializer.Deserialize<Wall>(message);
                        world.AddWall(wall);
                    }
                    else 
                    {
                        //no more wall objects
                        break;
                    }
                }
                clientCommands = true;
                //concurrently recueve current state of game at each frame as well as walls
                await GetUpdates();
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
                }
                else 
                {
                    //update or add snake in model
                }
            }
            else if (message.Contains("\"power\""))
            {
                Power? power = JsonSerializer.Deserialize<Power>(message);
                if (power.died == true) 
                {
                    //remove used powerup
                }
            }
        }

        /// <summary>
        /// Helper method for sending commands as JSON objects
        /// </summary>
        /// <param name="direction"></param> corresponds to input direction (up, down, left, right)
        private void SendMovementDirection(string direction) 
        {
            // ensure commands are allowed on frame
            if (clientCommands && !commandOnFrame)
            {
                //create an object to hold the command with the moving keyword
                var sendable = new { moving = direction };
                //serialize into JSON 
                string json = JsonSerializer.Serialize(sendable);
                //send it
                connection.Send(json);
                // one command per frame
                commandOnFrame = true;
            }
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
                string? message = connection.ReadLine();
                if (!string.IsNullOrEmpty(message))
                {
                    ParseMessage(message);
                    commandOnFrame = false;
                }
            }

        }

    }
}
