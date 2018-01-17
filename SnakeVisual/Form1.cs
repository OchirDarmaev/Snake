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
        Game game;
        int sizeFigureDraw = 15;
        bool test = false;
        public Form1()
        {
            InitializeComponent();

            DoubleBuffered = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            test = false;
            StartNewGame();
        }
        private void StartNewGame()
        {
            if (test)
            {
                StartTestGame();
            }
            StartNewGame(0);
        }

        private void StartNewGame(int level)
        {
            game = new Game();
            game.LoadLevel(level);
            game.Start();
            timer1.Interval = 200;
            timer1.Enabled = true;
        }

        private void StartTestGame()
        {
            
            game = new Game();
            game.LoadLevel(0);
            game.Start();
            game.AutoPilot = true;
            timer1.Enabled = false;
            while (Step());
            
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up: game.ChangeDirection(Directions.Up); break;
                case Keys.Down: game.ChangeDirection(Directions.Down); break;
                case Keys.Left: game.ChangeDirection(Directions.Left); break;
                case Keys.Right: game.ChangeDirection(Directions.Right); break;
                case Keys.A: game.AutoPilot = !game.AutoPilot; break;
                case Keys.T: if (!test) test = true; StartNewGame(); break;
                case Keys.F:
                    if (timer1.Interval > 10)
                        timer1.Interval -= 10;
                    break;
                case Keys.L:
                    timer1.Interval += 10;
                    break;
                case Keys.N:
                    StartNewGame();
                    break;
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
                    break;
                default: break;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

            if (!Step())
            {
                var res = MessageBox.Show("You LOSE!\n Press 'N' for new game");
                    if (res == DialogResult.OK)
                        StartNewGame();
            }
        }

        private bool Step()
        {
            if (game.Move())
            {
                if (!test)
                {
                    Invalidate();
                    labelInfo.Text = $"Size {game.Size} \nSpeed {timer1.Interval}\nAutoPilot {game.AutoPilot}";
                }
                return true;
            }
            else
            {
                timer1.Enabled = false;
                if (test)
                {
                    Invalidate();
                    labelInfo.Text = $"Size {game.Size} \nSpeed {timer1.Interval}\nAutoPilot {game.AutoPilot}";
                    var res = MessageBox.Show("You LOSE!\n Press 'N' for new game");
                }
                return false;
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            List<SnakeGame.Point> points = game.GetAllPoints();

            foreach (var item in points)
            {
                switch (item.Figure)
                {
                    case Figures.Head:
                        graphics.FillEllipse(Brushes.Yellow, item.X * sizeFigureDraw, item.Y * sizeFigureDraw, sizeFigureDraw, sizeFigureDraw); break;
                    case Figures.Barrier:
                        graphics.FillEllipse(Brushes.Brown, item.X * sizeFigureDraw, item.Y * sizeFigureDraw, sizeFigureDraw, sizeFigureDraw); break;
                    case Figures.Wall:
                        graphics.FillEllipse(Brushes.RosyBrown, item.X * sizeFigureDraw, item.Y * sizeFigureDraw, sizeFigureDraw, sizeFigureDraw); break;
                    case Figures.Food:
                        graphics.FillEllipse(Brushes.Green, item.X * sizeFigureDraw, item.Y * sizeFigureDraw, sizeFigureDraw, sizeFigureDraw); break;
                    case Figures.Body:
                        graphics.FillEllipse(Brushes.YellowGreen, item.X * sizeFigureDraw, item.Y * sizeFigureDraw, sizeFigureDraw, sizeFigureDraw); break;
                    case Figures.Tail:
                        graphics.FillEllipse(Brushes.OrangeRed, item.X * sizeFigureDraw + 2, item.Y * sizeFigureDraw + 2, sizeFigureDraw - 4, sizeFigureDraw - 4); break;
                    case Figures.Way:
                        graphics.FillEllipse(Brushes.Red, item.X * sizeFigureDraw + 4, item.Y * sizeFigureDraw + 4, sizeFigureDraw - 8, sizeFigureDraw - 8); break;
                    case Figures.PosibleWay:
                        graphics.FillEllipse(Brushes.Gray, item.X * sizeFigureDraw + 4, item.Y * sizeFigureDraw + 4, sizeFigureDraw - 8, sizeFigureDraw - 8); break;
                    case Figures.EmptySpace:
                        break;
                }
            }
        }




    }
}
