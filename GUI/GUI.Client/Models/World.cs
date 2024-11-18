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
        int WorldSize;
        /// <summary>
        /// private member field to store snake/player objecst and their corresponding ids.
        /// </summary>
        private List<Snake> snakes;
        /// <summary>
        /// private field to store list of walls upon server startup
        /// </summary>
        private List<Wall> walls;
        /// <summary>
        /// private field to store list of powerups active in the current frame.
        /// </summary>
        private List<Power> powerups;
        /// <summary>
        /// constructor for World object to represent games state in current frame. Initialzied with worldsize
        /// </summary>
        /// <param name="worldsize">size of world sent from server</param>
        public World(int worldsize) 
        {
            this.WorldSize = worldsize;
        }
        /// <summary>
        /// method to remove all dead objects from game. Includes snakes and powerups.
        /// </summary>
        private void RemoveDeadObjects() 
        {
            for (int i = snakes.Count - 1; i >= 0; i--) 
            {
                if (snakes[i].died) { snakes.Remove(snakes[i]); }
            }
            for (int i = powerups.Count - 1; i >= 0; i--) 
            {
                if (powerups[i].died) {  powerups.Remove(powerups[i]);}
            }
        }
        /// <summary>
        /// private helper method to add wall object to list of walls
        /// </summary>
        /// <param name="wall">wall object to be added</param>
        private void AddWall(Wall wall) 
        {
            walls.Add(wall);
        }
    }
}
