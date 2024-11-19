namespace GUI.Client.Models
{
    /// <summary>
    /// Class to represent snake objects in each frame update of game. Contains attributes thare sent from server JSON in each frame.
    /// <authors>
    /// Brian Keller & Wyatt Young
    /// </authors>
    /// <versions>
    /// November 17th, 2024
    /// </versions>
    /// </summary>
    public class Snake
    {
        /// <summary>
        /// public property for the id number of the snake object
        /// </summary>
        public int snake
        {
            get;
            set;
        }
        /// <summary>
        /// public property for the snake object's name
        /// </summary>
        public string name
        {
            get;
            set;
        }
        /// <summary>
        /// public property for the list of points representing the snake object's body
        /// </summary>
        public List<Point2D> body
        {
            get;
            set;
        }
        /// <summary>
        /// getter and setter property for the snake's orientation
        /// </summary>
        public Point2D dir
        {
            get;
            set;
        }
        /// <summary>
        /// public property to get and set the snake object's score
        /// </summary>
        public int score
        {
            get;
            set;
        }
        /// <summary>
        /// public property to determine if the snake died on the most recent frame
        /// </summary>
        public bool died
        {
            get;
            set;
        }
        /// <summary>
        /// public property for alive field denoting whether the snake is alive or dead in the given frame.
        /// </summary>
        public bool alive 
        {
            get;
            set;
        }
        /// <summary>
        /// public property to set whether or not the snake object is disconnected on a frame
        /// </summary>
        public bool dc
        {
            get;
            set;
        }
        /// <summary>
        /// public property to get and set whether or not a player joined on a frame
        /// </summary>
        public bool join
        {
            get; 
            set;
        }
        /// <summary>
        /// Default constructor for snake class. Fields set to values that are common to every snake upon startup. ID set to 0 to denote default construction.
        /// </summary>
        public Snake() 
        {
            this.snake = 0;
            this.name = "";
            this.body = new List<Point2D>();
            this.dir = new Point2D();
            this.score = 0;
            this.died = false;
            this.alive = true;
            this.dc = false;
            this.join = true;
        }
        /// <summary>
        /// Constructor for snake class when player id is provided. Fields set to values that are common to every snake upon startup.
        /// </summary>
        public Snake(int id)
        {
            this.snake = id;
            this.name = "";
            this.body = new List<Point2D>();
            this.dir = new Point2D();
            this.score = 0;
            this.died = false;
            this.alive = true;
            this.dc = false;
            this.join = true;
        }
    }
}
