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

        [JsonConstructor]
        public Wall(int wall, Point2D p1, Point2D p2)
        {
            this.wall = wall;
            this.p1 = p1;
            this.p2 = p2;
        }

        public override string ToString()
        {
            return $"Wall: P1({p1.X}, {p1.Y}) - P2({p2.X}, {p2.Y})";
        }
        /// <summary>
        /// helper method to draw walls
        /// </summary>
        /// <param name="context"></param>
        public async Task Draw(Canvas2DContext context, ElementReference image)
        {
            //determine if wall is horizontal or vertical
            bool isHorizontal = p1.Y == p2.Y;
            int wallLength = isHorizontal ? Math.Abs(p2.X - p1.X) : Math.Abs(p2.Y - p1.Y);
            //calculate starting position
            int startX = Math.Min(p1.X, p2.X);
            int startY = Math.Min(p1.Y, p2.Y);
            //calculate how many sprites to use
            int numWalls = wallLength / 50;
            for (int i = 0; i < numWalls; i++)
            {
             
                int drawX = isHorizontal ? startX + i * 50 : startX;
                int drawY = isHorizontal ? startY : startY + i * 50;

                await context.DrawImageAsync(image, drawX, drawY);
            }
        }
    }
}
