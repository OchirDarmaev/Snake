using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SnakeGame
{
    public class Snake : FigureBase
    {
        public Directions _direction;
        public int _lenght;
        public int _width;
        public int _height;
        public GameMode _gameMod;
        public Barrier _barrier;
        public Directions DirectionControl { get; set; }

        public Snake(int lengthSnake, int width, int height, Directions direction, GameMode gameMod, Barrier barrier)
        {
            if (width <= 0)
                throw new Exception();
            if (height <= 0)
                throw new Exception();
            if (height <= 0)
                throw new Exception();
            if (lengthSnake >= width || lengthSnake >= height)
                throw new Exception("Length of snake not be more size place ");
            _width = width;
            _height = height;
            _direction = direction;
            _gameMod = gameMod;
            _lenght = lengthSnake;
            _barrier = barrier;

            int middleWidth = _width / 2;
            int middleHeight = _height / 2;
            points = new List<Point>();

            for (int i = 0; i < _lenght; i++)
            {
                // Add Tail
                if (i == 0)
                    points.Add(new Point(middleWidth + i, middleHeight, Figures.Tail));
                // Add Head
                else if (i == _lenght - 1)
                    points.Add(new Point(middleWidth + i, middleHeight, Figures.Head));
                // Add Body
                else
                    points.Add(new Point(middleWidth + i, middleHeight, Figures.Body));
            }

        }

        public bool Move(ref Point food)
        {
            //todo поворот перед стеной делает не корректно
            if (points.Count == 0)
                throw new Exception("Length of snake not be null ");
            int x = points.Last().X;
            int y = points.Last().Y;

            // Запрет разворота на 180
            // Если попытка поворота на 180 градусов
            if ((_direction == Directions.Right && DirectionControl == Directions.Left) ||
                (_direction == Directions.Left && DirectionControl == Directions.Right) ||
                (_direction == Directions.Up && DirectionControl == Directions.Down) ||
                (_direction == Directions.Down && DirectionControl == Directions.Up))
            {
                //_direction;
            }
            else
            {
                _direction = DirectionControl;
            }
            switch (_direction)
            {
                case Directions.Up:
                    y -= 1;
                    break;
                case Directions.Down:
                    y += 1;
                    break;
                case Directions.Left:
                    x -= 1;
                    break;
                case Directions.Right:
                    x += 1;
                    break;
            }
            bool confused = false;
            bool hitTheWall = false;
            bool hitTheBarrier = false;

            // TODO учесть GameMod
            foreach (var point in points)
            {
                confused = (point.X == x && point.Y == y);
                hitTheWall = (x > _width || x < 1) || (y > _height || y < 1);
                if (confused || hitTheWall)
                    break;
            }
            foreach (var point in _barrier.points)
            {
                hitTheBarrier = (x == point.X) && (y == point.Y);
                if (hitTheBarrier)
                    break;
            }
            if (food.X == x && food.Y == y)
            {
                food = null;
                points[0].Figure = Figures.Tail;
                points.Last().Figure = Figures.Body;
            }
            else
            {
                points.RemoveAt(0);
                points[0].Figure = Figures.Tail;
                points.Last().Figure = Figures.Body;
            }
            if (confused || hitTheWall || hitTheBarrier)
            {
                return false;
            }
            else
            {
                points.Add(new Point(x, y, Figures.Head));
                return true;
            }

        }

        public virtual void DrawBarriers()
        {
            throw new NotImplementedException();
        }

        public override void Draw()
        {
            throw new NotImplementedException();
        }
    }
}
