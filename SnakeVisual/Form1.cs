using SnakeGame;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SnakeVisual
{
    public partial class Form1 : Form
    {
        //TODO инкапсулировать новый функционал
        Game game;
        Graphics graphics;
        List<List<System.Drawing.Point>> shortWays;
        List<System.Drawing.Point> shortestWay;
        bool AutoPilot = false;

        public Form1()
        {
            InitializeComponent();
            graphics = this.CreateGraphics();
            game = new Game();
            this.DoubleBuffered = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            NewGame();
        }
        private void NewGame()
        {
            game.EditLevel(0);
            game.LoadLevel();
            game.Start();
            timer1.Enabled = true;
        }
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up: game.ChangeDirection(Directions.Up); break;
                case Keys.Down: game.ChangeDirection(Directions.Down); break;
                case Keys.Left: game.ChangeDirection(Directions.Left); break;
                case Keys.Right: game.ChangeDirection(Directions.Right); break;
                case Keys.F: if (timer1.Interval > 50) timer1.Interval -= 50; break;
                case Keys.L: timer1.Interval += 50; break;
                case Keys.N: NewGame(); break;
                case Keys.P:
                    if (timer1.Enabled)
                    {

                        timer1.Enabled = false;
                        MessageBox.Show("Пауза для продолжения нажмите 'P'");
                    }
                    else
                    {
                        timer1.Enabled = true;
                    }
                    ; break;
                case Keys.A: AutoPilot = !AutoPilot; break;
                default: break;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (game.Move())
            {
                this.Invalidate();
                if (AutoPilot)
                {
                    AutoFindWay();
                }
                var autoPilotState = AutoPilot ? "Enabled" : "Disabled";
                
                labelInfo.Text = $"Size {game._snake.points.Count} \nSpeed {timer1.Interval}\nAutoPilot {autoPilotState}";
            }
            else
            {
                timer1.Enabled = false;

                MessageBox.Show("You LOSE!\n Press 'N' for new game");
            }
        }

        private void AutoFindWay()
         {
            //TODO Не правильно отрабатывает
            shortestWay = FindWay();
            if (shortestWay != null)
            {
                var target = shortestWay[1];
                var headPoint = game._snake.points.Find(x => x.Figure == Figures.Head);
                if (headPoint.X - 1 == target.X && headPoint.Y == target.Y) game.ChangeDirection(Directions.Left);
                if (headPoint.X + 1 == target.X && headPoint.Y == target.Y) game.ChangeDirection(Directions.Right);
                if (headPoint.X == target.X && headPoint.Y - 1 == target.Y) game.ChangeDirection(Directions.Up);
                if (headPoint.X == target.X && headPoint.Y + 1 == target.Y) game.ChangeDirection(Directions.Down);
            }
            else
            {
                var headPoint = game._snake.points.Find(x => x.Figure == Figures.Head);
                var points = GetAllPoints();
                var left = points.Find(x => x.X == headPoint.X - 1 && x.Y == headPoint.Y && x.Figure == Figures.EmptySpace);
                var right = points.Find(x => x.X == headPoint.X + 1 && x.Y == headPoint.Y && x.Figure == Figures.EmptySpace);
                var up = points.Find(x => x.X == headPoint.X && x.Y == headPoint.Y - 1 && x.Figure == Figures.EmptySpace);
                var down = points.Find(x => x.X == headPoint.X && x.Y == headPoint.Y + 1 && x.Figure == Figures.EmptySpace);
                if (left != null)

                { game.ChangeDirection(Directions.Left); return; }
                if (right != null)
                { game.ChangeDirection(Directions.Right); return; }
                if (up != null)
                { game.ChangeDirection(Directions.Up); return; }
                if (down != null)
                { game.ChangeDirection(Directions.Down); return; }
            }
        }
        private List<System.Drawing.Point> FindWay()
        {
            shortWays = new List<List<System.Drawing.Point>>();
            List<SnakeGame.Point> points = GetAllPoints();
            int heihgt = game._height;
            int width = game._width;

            // 0 EmptySpace
            // -1 Wall , Barriers, Snake
            // 2 target
            int[,] simplePoints = new int[heihgt + 6, width + 6];

            foreach (var item in points)
            {
                switch (item.Figure)
                {
                    case Figures.EmptySpace: simplePoints[item.X, item.Y] = 0; break;
                    case Figures.Food: simplePoints[item.X, item.Y] = 2; break;
                    default: simplePoints[item.X, item.Y] = -1; break;
                }
            }

            List<List<System.Drawing.Point>> possibleWays = new List<List<System.Drawing.Point>>();
            List<System.Drawing.Point> firstWay = new List<System.Drawing.Point>();

            var start = points.Find(x => x.Figure == Figures.Head);
            firstWay.Add(new System.Drawing.Point(start.X, start.Y));
            possibleWays.Add(firstWay);

            int maxSteps = start.X * start.Y;
            int count = 0;
            while (count <= maxSteps)
            {
                count++;
                List<List<System.Drawing.Point>> waysBuffer = new List<List<System.Drawing.Point>>();
                foreach (var way in possibleWays)
                {
                    var point = way.Last();

                    int x = point.X;
                    int y = point.Y;

                    int left = 0;
                    int right = 0;
                    int up = 0;
                    int down = 0;

                    if (AddList(ref simplePoints, waysBuffer, way, x + 1, y) != -1) right++;
                    if (AddList(ref simplePoints, waysBuffer, way, x, y + 1) != -1) down++;
                    if (AddList(ref simplePoints, waysBuffer, way, x - 1, y) != -1) left++;
                    if (AddList(ref simplePoints, waysBuffer, way, x, y - 1) != -1) up++;

                    // Если сумма 4 путей 0 значит нет пути
                }
                foreach (var item in waysBuffer)
                {
                    possibleWays.Add(item);
                }


            }
            if (shortWays.Count != 0)
            {
                List<System.Drawing.Point> shotestWay = shortWays[0];
                foreach (var item in shortWays)
                {
                    if (shotestWay.Count > item.Count)
                        shotestWay = item;
                }
                return shotestWay;
            }
            else
                return null;
        }

        private int AddList(ref int[,] mass, List<List<System.Drawing.Point>> pathsBuffer, List<System.Drawing.Point> path, int x, int y)

        {
            if (mass[x, y] == 2)
            {
                List<System.Drawing.Point> pathBuffer = new List<System.Drawing.Point>();
                foreach (var item in path)
                {
                    pathBuffer.Add(item);
                }
                pathBuffer.Add(new System.Drawing.Point(x, y));
                shortWays.Add(pathBuffer);
                return 1;
            }

            if (mass[x, y] == 0)
            {
                mass[x, y] = -2;
                List<System.Drawing.Point> pathBuffer = new List<System.Drawing.Point>();
                foreach (var item in path)
                {
                    pathBuffer.Add(item);
                }
                pathBuffer.Add(new System.Drawing.Point(x, y));
                pathsBuffer.Add(pathBuffer);
                return 0;
            }
            else
            {
                return -1;
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            List<SnakeGame.Point> points = GetAllPoints();
            if (shortestWay != null)
                foreach (var item in shortestWay)
                {
                    points.Add(new SnakeGame.Point(item.X, item.Y, Figures.Way));
                }
            int size = 15;
            foreach (var item in points)
            {
                switch (item.Figure)
                {
                    case Figures.Barrier:
                        graphics.FillEllipse(Brushes.Brown, item.X * size, item.Y * size, size, size); break;
                    case Figures.Wall:
                        graphics.FillEllipse(Brushes.RosyBrown, item.X * size, item.Y * size, size, size); break;
                    case Figures.Food:
                        graphics.FillEllipse(Brushes.Green, item.X * size, item.Y * size, size, size); break;
                    case Figures.Head:
                        graphics.FillEllipse(Brushes.Yellow, item.X * size, item.Y * size, size, size); break;
                    case Figures.Body:
                        graphics.FillEllipse(Brushes.YellowGreen, item.X * size, item.Y * size, size, size); break;
                    case Figures.Tail:
                        graphics.FillEllipse(Brushes.OrangeRed, item.X * size + 2, item.Y * size + 2, size - 4, size - 4); break;
                    case Figures.Way:
                        graphics.FillEllipse(Brushes.Red, item.X * size + 4, item.Y * size + 4, size - 8, size - 8); break;
                    case Figures.EmptySpace:
                        break;
                }
            }
        }
        /// <summary>
        /// Получить все точки игры
        /// </summary>
        /// <returns></returns>
        private List<SnakeGame.Point> GetAllPoints()
        {
            List<SnakeGame.Point> points = new List<SnakeGame.Point>();
            points.AddRange(game.points);
            points.AddRange(game._barrier.points);
            points.AddRange(game._snake.points);
            points.AddRange(game._food.points);

            //UpdatePoints(points, );
            return points;
        }

        private static void UpdatePoints(List<SnakeGame.Point> points, List<SnakeGame.Point> pointsAdd)
        {
            foreach (var item in pointsAdd)
            {
                var pt = points.Find(x => x.X == item.X && x.Y == item.Y);
                if (pt != null)
                    points[points.IndexOf(pt)] = item;
            }
        }

    }
}
