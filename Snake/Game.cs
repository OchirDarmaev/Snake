using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SnakeGame
{
    public class Game : FigureBase
    {
        public int _width;
        public int _height;
        public int _level;
        // это должен быть отдельный класс
        public Barrier _barrier;
        public Snake _snake;
        public Food _food;
        public GameMode _gameMode;
        public SpeedGame _speedGame;

        public Game()
        {
        }

        public void Start()
        {
            points = new List<Point>();
            _food = new Food(_width, _height);
            for (int i = 0; i < _width; i++)
            {
                points.Add(new Point(i, 0, Figures.Wall));
                points.Add(new Point(i, _height, Figures.Wall));
            }
            for (int i = 0; i < _height; i++)
            {
                points.Add(new Point(0, i, Figures.Wall));
                points.Add(new Point(_width, i, Figures.Wall));
            }
            points.Add(new Point(_width, _height, Figures.Wall));
            for (int i = 1; i < _height; i++)
            {
                for (int j = 1; j < _width; j++)
                {
                    if (points.Find(x=>x.X==i&&x.Y==j) == null)
                        points.Add(new Point(i, j, Figures.EmptySpace));
                }
            }
        }

        public void ChangeDirection(Directions direction)
        {
                    _snake.DirectionControl = direction;
        }

        public bool Move()
        {
            bool res;
            Point var = _food.GetPoint();
            res = _snake.Move(ref var);
            if (var == null)
                _food.Create();
            return res;
        }

        public void Pause()
        {
        }

        public void Сontinue()
        {
        }

        public void CreateFood()
        {

        }

        public void Exit()
        {
        }

        public void LoadLevel()
        {
            if (_level == 0)
            {
                _barrier = new Barrier();
                _barrier.AddBarrier(3, 1);
                _barrier.AddBarrier(3, 2);
                _barrier.AddBarrier(3, 3);

                EditSittings(GameMode.HitWall, SpeedGame.Normal, 30, 25);
                _snake = new Snake(5, _width, _height, Directions.Right, _gameMode, _barrier);
            }
        }

        public void EditLevel(int level)
        {
            _level = level;
        }
        public void EditSpeed(SpeedGame speedGame)
        {
            _speedGame = speedGame;
        }
        public void EditSittings(GameMode gameMode, SpeedGame speedGame, int width, int heigth)
        {
            _gameMode = gameMode;
            _speedGame = speedGame;
            _width = width;
            _height = heigth;
        }

        public override void Draw()
        {
            throw new NotImplementedException();
        }
    }
}
