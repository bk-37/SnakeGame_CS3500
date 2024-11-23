using Blazor.Extensions.Canvas.Canvas2D;
using Microsoft.AspNetCore.Components;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace GUI.Client.Models
{
    /// <summary>
    /// Class to represent wall objects in each frame update of game. Walls will kill a snake/player when they collide.
    /// Contains attributes/properties thare sent from server JSON in each frame.
    /// <authors>
    /// Brian Keller & Wyatt Young
    /// </authors>
    /// <versions>
    /// November 17th, 2024
    /// </versions>
    /// </summary>
    public class Wall
    {
        /// <summary>
        /// public property for walls unique id
        /// </summary>
        [JsonPropertyName("wall")]
        public int wall{get; private set;}
        /// <summary>
        /// public property for first Point2D endpoint of wall
        /// </summary>
        [JsonPropertyName("p1")]
        public Point2D p1 { get; private set;}
        /// <summary>
        /// public property for second Point2D endpoint of wall
        /// </summary>
        [JsonPropertyName("p2")]
        public Point2D p2 { get; private set; }
        /// <summary>
        /// default constructor object representing wall object from server. ID set to 0 to denote default construction.
        /// </summary>
        public Wall() 
        {
            this.wall = 0;
            this.p1 = new Point2D();
            this.p2 = new Point2D();
        }
        /// <summary>
        /// Wall constructor used for creating walls from deserialized Json 
        /// </summary>
        /// <param name="wall"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        [JsonConstructor]
        public Wall(int wall, Point2D p1, Point2D p2)
        {
            this.wall = wall;
            this.p1 = p1;
            this.p2 = p2;
        }
        
        /// <summary>
        /// Method for representing walls as strings for debuggins purposes
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"Wall: P1({p1.X}, {p1.Y}) - P2({p2.X}, {p2.Y})";
        }
        /// <summary>
        /// helper method to draw walls
        /// </summary>
        /// <param name="context"> drawing context </param>
        public async Task Draw(Canvas2DContext context, ElementReference image)
        {
            //determine if wall is horizontal or vertical
            bool isHorizontal = p1.Y == p2.Y;
            int wallLength = 0;
            if(isHorizontal) 
                wallLength = Math.Abs(p1.X - p2.X);
            else
                wallLength = Math.Abs(p1.Y - p2.Y);
            //calculate how many sprites to use
            int numWalls = wallLength / 50;
            for (int i = 0; i <= numWalls; i++)
            {
                int x;
                int y;
                if (isHorizontal)
                {
                    y = p1.Y;
                    if (p1.X < p2.X)
                        x = p1.X + i * 50;
                    else 
                        x = p1.X - i * 50;
                }
                else
                {
                    x = p1.X;
                    if(p1.Y < p2.Y)
                        y = p1.Y + i * 50;
                    else
                        y = p1.Y - i * 50;
                }
                await context.DrawImageAsync(image, x - 25, y - 25, 50, 50);
            }
        }
    }
}
