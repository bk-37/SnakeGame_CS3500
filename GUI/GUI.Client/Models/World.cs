using System.ComponentModel;

namespace GUI.Client.Models
{
    /// <summary>
    /// Class to hold current state of world for each frame. Contains lists of game objects as well as update objects based on commands by server.
    /// <authors>
    /// Brian Keller & Wyatt Young
    /// </authors>
    /// <versions>
    /// November 17th, 2024
    /// </versions>
    /// </summary>
    public class World
    {
        /// <summary>
        /// private member field to store the size of the world sent by the server.
        /// </summary>
        public int WorldSize
        {
            get;
            private set;
        }
        /// <summary>
        /// private dictionary field to store snake/player objecst and their corresponding ids.
        /// </summary>
        public Dictionary<int, Snake> snakes {  get; private set; }
        /// <summary>
        /// private field to store list of walls upon server startup
        /// </summary>
        public List<Wall> walls {  get; private set; }
        /// <summary>
        /// private field to store dictionary of powerups active in the current frame.
        /// </summary>
        public Dictionary<int, Power> powerups { get; private set; }
        /// <summary>
        /// constructor for World object to represent games state in current frame. Initialzied with worldsize
        /// </summary>
        /// <param name="worldsize">size of world sent from server</param>
        public World(int worldsize) 
        {
            this.WorldSize = worldsize;
            snakes = new Dictionary<int, Snake>();
            walls = new List<Wall>();
            powerups = new Dictionary<int, Power>();
        }

        public World(World world)
        {
            this.WorldSize = world.WorldSize;
            this.snakes = world.snakes;
            this.walls = world.walls;
            this.powerups = world.powerups;
        }
        /// <summary>
        /// method to remove all dead objects from game. Includes snakes and powerups.
        /// </summary>
        private void RemoveDeadObjects() 
        {
            for (int i = snakes.Count - 1; i >= 0; i--) 
            {
                if (snakes[i].died) { snakes.Remove(snakes[i].snake); }
            }
            for (int i = powerups.Count - 1; i >= 0; i--) 
            {
                if (powerups[i].died) {  powerups.Remove(powerups[i].power);}
            }
        }
        /// <summary>
        /// helper method to add wall object to list of walls
        /// </summary>
        /// <param name="wall">wall object to be added</param>
        public void AddWall(Wall wall) 
        {
            walls.Add(wall);
        }
        /// <summary>
        /// helper method to add snake object to dictionary of snakes using id as key.
        /// </summary>
        /// <param name="snake">snake object to be added</param>
        public void AddSnake(Snake snake)
        {
            snakes[snake.snake] = snake;
        }
        /// <summary>
        /// helper method to add power up objects to dictionary of power ups
        /// </summary>
        /// <param name="power">power up object to be added</param>
        public void AddPowerup(Power power)
        {
            powerups[power.power] = power;
        }
        /// <summary>
        /// helper method to remove wall object to list of walls
        /// </summary>
        /// <param name="wall">wall object to be removed</param>
        public void RemoveWall(Wall wall)
        {
            walls.Remove(wall);
        }
        /// <summary>
        /// helper method to remove snake object to dictionary of snakes using id as key.
        /// </summary>
        /// <param name="snake">snake object to be removed</param>
        public void RemoveSnake(Snake snake)
        {
            snakes.Remove(snake.snake);
        }
        /// <summary>
        /// helper method to remove power up objects to dictionary of power ups
        /// </summary>
        /// <param name="power">power up object to be remove</param>
        public void RemovePowerup(Power power)
        {
            powerups.Remove(power.power);
        }
    }
}
