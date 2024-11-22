using Blazor.Extensions.Canvas.Canvas2D;
using Microsoft.AspNetCore.Components;
using System.Text.Json.Serialization;

namespace GUI.Client.Models
{
    /// <summary>
    /// Class to represent power up objects in each frame update of game. Power ups can be collected by player snakes in order to grow the snake body in length.
    /// Contains attributes/properties thare sent from server JSON in each frame.
    /// <authors>
    /// Brian Keller & Wyatt Young
    /// </authors>
    /// <versions>
    /// November 17th, 2024
    /// </versions>
    /// </summary>
    public class Power
    {
        /// <summary>
        /// public property for a power up objects unique id
        /// </summary>
        public int power { get; set; }
        /// <summary>
        /// public property for a power up objects location via Point2D object
        /// </summary>
        public Point2D loc {  get; set; }
        /// <summary>
        /// public property for the died member variable denoting whether a power up was collected on the frame.
        /// </summary>
        public bool died { get; set; }
        /// <summary>
        /// defauilt constructor for Power class representing a power up object in the game that can be collected by players. ID set to 0 to denote default construction.
        /// </summary>
        public Power() 
        {
            this.power = 0;
            this.loc = new();
            this.died = false;
        }
        /// <summary>
        /// Powerup constructor used by the deserialized json messages from the server
        /// </summary>
        /// <param name="power"></param>
        /// <param name="loc"></param>
        /// <param name="died"></param>
        [JsonConstructor]
        public Power(int power, Point2D loc, bool died)
        {
            this.power = power;
            this.loc = loc;
            this.died = died;
        }

        public async Task Draw(Canvas2DContext context, ElementReference apple)
        {
            if (died)
            {
                return;
            }
            await context.DrawImageAsync(apple, loc.X - 12, loc.Y- 12, 25, 25);
        }
    }
}
