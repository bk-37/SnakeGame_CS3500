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
        /// private field for a walls unique id
        /// </summary>
        private int wall;
        /// <summary>
        /// private field for Point2D object representing the first endpoint of the wall.
        /// </summary>
        private Point2D p1;
        /// <summary>
        /// private field for Point2D object representing the second endpoint of the wall.
        /// </summary>
        private Point2D p2;
        /// <summary>
        /// public property for walls unique id
        /// </summary>
        public int wall{get; set;}
        /// <summary>
        /// public property for first Point2D endpoint of wall
        /// </summary>
        public Point2D p1 { get; set;}
        /// <summary>
        /// public property for second Point2D endpoint of wall
        /// </summary>
        public Point2D p2 { get; set; }
        /// <summary>
        /// default constructor object representing wall object from server. ID set to 0 to denote default construction.
        /// </summary>
        public Wall() 
        {
            this.wall = 0;
            this.p1 = new();
            this.p2 = new();
        }
    }
}
