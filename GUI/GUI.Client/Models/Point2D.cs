namespace GUI.Client.Models
{
    /// <summary>
    /// Class to represent 2D poiunts used for drawing and locations of objects in the game updated on eachf rame via the server.
    /// <authors>
    /// Brian Keller & Wyatt Young
    /// </authors>
    /// <versions>
    /// November 17th, 2024
    /// </versions>
    /// </summary>
    public class Point2D
    {
        /// <summary>
        /// public property for the X coordinate of a Point2D object
        /// </summary>
        public int X { get; set; }
        /// <summary>
        /// public property for the Y coordinate of a Point2D object
        /// </summary>
        public int Y { get; set; }
        /// <summary>
        /// constructor for Point2D object with x and y arguments
        /// </summary>
        /// <param name="X">x coordinate of point on frame</param>
        /// <param name="Y">y coordinate of point on frame</param>
        public Point2D(int X, int Y) 
        {
            this.X = X;
            this.Y = Y;
        }
        /// <summary>
        /// default constructor for Point2D pbject when an x and y coordinate are not provided. Defaults each coordinate to 0,0
        /// </summary>
        public Point2D() 
        {
            this.X = 0;
            this.Y = 0;
        }
        public override string ToString()
        {
            return $"({X}, {Y})";
        }
    }
}
