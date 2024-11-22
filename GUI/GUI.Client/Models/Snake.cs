using Blazor.Extensions.Canvas.Canvas2D;
using static System.Formats.Asn1.AsnWriter;
using System.Xml.Linq;
using System.Diagnostics;
using System.Text.Json.Serialization;
using System.Drawing;

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
            private set;
        }
        /// <summary>
        /// public property for the snake object's name
        /// </summary>
        public string name
        {
            get;
            private set;
        }
        /// <summary>
        /// public property for the list of points representing the snake object's body
        /// </summary>
        public List<Point2D> body
        {
            get;
            private set;
        }
        /// <summary>
        /// getter and setter property for the snake's orientation
        /// </summary>
        public Point2D dir
        {
            get;
            private set;
        }
        /// <summary>
        /// public property to get and set the snake object's score
        /// </summary>
        public int score
        {
            get;
            private set;
        }
        /// <summary>
        /// public property to determine if the snake died on the most recent frame
        /// </summary>
        public bool died
        {
            get;
            private set;
        }
        /// <summary>
        /// public property for alive field denoting whether the snake is alive or dead in the given frame.
        /// </summary>
        public bool alive 
        {
            get;
            private set;
        }
        /// <summary>
        /// public property to set whether or not the snake object is disconnected on a frame
        /// </summary>
        public bool dc
        {
            get;
            private set;
        }
        /// <summary>
        /// public property to get and set whether or not a player joined on a frame
        /// </summary>
        public bool join
        {
            get; 
            private set;
        }

        private readonly Random random = new Random();

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

        /// <summary>
        /// Constructor for wall class when JSON deserialization is provided
        /// </summary>
        /// <param name="snake"></param>
        /// <param name="name"></param>
        /// <param name="body"></param>
        /// <param name="dir"></param>
        /// <param name="score"></param>
        /// <param name="died"></param>
        /// <param name="alive"></param>
        /// <param name="dc"></param>
        /// <param name="join"></param>
        [JsonConstructor]
        public Snake(int snake, string name, List<Point2D> body, Point2D dir, int score, bool died, bool alive, bool dc, bool join)
        {
            this.snake = snake;
            this.name = name;
            this.body = body;
            this.dir = dir;
            this.score = score;
            this.died = died;
            this.alive = alive;
            this.dc = dc;
            this.join = join;
        }
        /// <summary>
        /// Helper method for drawing snakes
        /// </summary>
        /// <param name="context"></param>
        public async Task DrawAlive(Canvas2DContext context)
        {
            // Set snake color based on ID
            await context.SetLineWidthAsync(10);
            //find the tail and head of the snake
            Point2D head = body.Last();
            Point2D tail = body[0];
            string[] stripeColors;
            if (!died)
                stripeColors = [GetSnakeColor(snake), GetSnakeColor(snake + 1)];
            else
                stripeColors = ["red", "red"];
            int stripeWidth = 10; // Width of each stripe
                                  //draw body pieces
            for (int i = 0; i < body.Count - 1; i++)
            {
                Point2D start = body[i];
                Point2D end = body[i + 1];
                await context.BeginPathAsync();
                await context.MoveToAsync(start.X, start.Y);
                await context.LineToAsync(end.X, end.Y);
                // Alternate stripe colors
                await context.SetStrokeStyleAsync(stripeColors[i % stripeColors.Length]);
                await context.SetLineWidthAsync(stripeWidth);
                await context.StrokeAsync();
                // Draw rounded joint if there's a next segment
                if (i < body.Count - 2)
                {
                    Point2D next = body[i + 2];
                    // Check if there is a turn at the current joint
                    if (start.X != next.X && start.Y != next.Y)
                    {
                        // Calculate joint center point
                        int centerX = end.X;
                        int centerY = end.Y;
                        await context.BeginPathAsync();
                        await context.ArcAsync(centerX, centerY, stripeWidth / 2, 0, 2 * Math.PI);
                        await context.SetFillStyleAsync(stripeColors[(i + 1) % stripeColors.Length]);
                        await context.FillAsync();
                    }
                }
            }
            // Draw rounded tail
            await context.SetFillStyleAsync(GetSnakeColor(snake)); // Tail color matches base color
            await context.BeginPathAsync();
            await context.ArcAsync(tail.X, tail.Y, stripeWidth / 2, 0, 2 * Math.PI);
            await context.FillAsync();
            // Draw rounded head
            Point2D headPoint = body.Last();
            await context.SetFillStyleAsync("rgba(0, 0, 0, 0.75)"); // Head color matches base color
            await context.BeginPathAsync();
            await context.ArcAsync(headPoint.X, headPoint.Y, stripeWidth / 1.5, 0, 2 * Math.PI);
            await context.FillAsync();
            // Draw name and score near the head
            await context.SetFillStyleAsync("black");
            await context.FillTextAsync($"{name} ({score})", headPoint.X + 10, headPoint.Y - 10);
        }

        public async Task DrawDead(Canvas2DContext context)
        {
            await context.SetStrokeStyleAsync("red");
            await context.SetLineWidthAsync(10);
            if (body.Count > 0)
            {
                await context.BeginPathAsync();
                Point2D tail = body[0];
                await context.MoveToAsync(tail.X, tail.Y);

                foreach (Point2D point in body)
                {
                    await context.LineToAsync(point.X, point.Y);
                }
            }
            await context.StrokeAsync();
        }
        /// <summary>
        /// Helper method for determining the color of the snake based on their ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private string GetSnakeColor(int id)
        {
            string[] colors = {"blue", "green", "yellow", "purple", "orange", "pink", "cyan" };
            return colors[id % colors.Length];
        }
    }
}
