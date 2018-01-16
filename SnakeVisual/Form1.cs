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
       
        public Form1()
        {
            InitializeComponent();
            
            DoubleBuffered = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            StartNewGame(0);
        }
        private void StartNewGame(int level)
        {
            game = new Game();
            game.LoadLevel(level);
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
                case Keys.A: game.AutoPilot = !game.AutoPilot; break;

                case Keys.F:
                    if (timer1.Interval > 10)
                        timer1.Interval -= 10;
                    break;
                case Keys.L:
                    timer1.Interval += 10;
                    break;
                case Keys.N:
                    StartNewGame(0);
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
            if (game.Move())
            {
                Invalidate();

                labelInfo.Text = $"Size {game.Size} \nSpeed {timer1.Interval}\nAutoPilot {game.AutoPilot}";
            }
            else
            {
                timer1.Enabled = false;

                MessageBox.Show("You LOSE!\n Press 'N' for new game");
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            List<SnakeGame.Point> points = game.GetAllPoints();
            
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
                    case Figures.PosibleWay:
                        graphics.FillEllipse(Brushes.Gray, item.X * size + 4, item.Y * size + 4, size - 8, size - 8); break;
                    case Figures.EmptySpace:
                        break;
                }
            }
        }




    }
}
