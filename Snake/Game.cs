using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SnakeGame
{
    public class Game : FigureBase
    {
        private int _width;
        private int _height;
        private Barrier _barrier;
        private Snake _snake;
        private Food _food;
        private GameMode _gameMode;
        private SpeedGame _speedGame;

        private List<List<Point>> _shortWays;
        private List<Point> _mainWay;
        public bool AutoPilot { get; set; } = false;
        public int Size { get { return _snake.points.Count; } }

        public int FoodCount { get; private set; }

        public Game()
        {
        }

        public void Start()
        {
            points = new List<Point>();
            _food = new Food(_width,_height);
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
            for (int i = 1; i < _width - 1; i++)
            {
                for (int j = 1; j < _height - 1; j++)
                {
                    if (points.Find(x => x.X == i && x.Y == j) == null)
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
            var foodPoint = _food.points;
            res = _snake.Move(ref foodPoint);
            // Добавление еды на карту
            if (foodPoint.Count < FoodCount)
            {
                for (int i = 0; i < FoodCount - foodPoint.Count; i++)
                {
                    Random rand = new Random(unchecked((int)DateTime.Now.Millisecond));
                    int x;
                    int y;
                    var points = GetAllPoints();
                    do
                    {
                        x = rand.Next(_width);
                        y = rand.Next(_height);
                    } while (points.Exists(p => p.X == x && p.Y == y && p.Figure != Figures.EmptySpace));
                    _food.AddFood(x, y);
                }
            }

            if (AutoPilot)
            {
                AutoFindWay();
            }
            return res;
        }

        public void Pause()
        {
            throw new System.AggregateException();
        }

        public void Сontinue()
        {
            throw new System.AggregateException();
        }

        public void CreateFood()
        {
            throw new System.AggregateException();
        }

        public void Exit()
        {
            throw new System.AggregateException();

        }

        public void LoadLevel(int level = 0)
        {
            if (level == 0)
            {
                _barrier = new Barrier();

                int x = 3;
                int y = 4;
                int z = 15;
                AddBarrier(x, y);
                AddBarrier(x+z, y);
                AddBarrier(x, y+z-y);
                AddBarrier(x+z, y+z-y);
                FoodCount = 3;
                EditSittings(GameMode.HitWall, SpeedGame.Normal, 30, 25);
                _snake = new Snake(5, _width, _height, Directions.Right, _gameMode, _barrier);
            }
        }

        private void AddBarrier(int x, int y, int dlina = 8)
        {
            for (int j = x; j < x+dlina ; j += 3)
            {
                for (int i = y; i < y+dlina; i++)
                {
                    _barrier.AddBarrier(j, i);
                }
            }
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

        private void AutoFindWay()
        {
            _mainWay = null;
            _mainWay = FindShortestWay();
            if (_mainWay == null)
                _mainWay = FindWayOutImpasse();
            if (_mainWay != null)
                TurnToPoint(_mainWay[1]);
        }

        private List<Point> FindWayOutImpasse()
        {
            //TODO 1.проноз как выйти из тупика
            //TODO 2. получить путь выхода
            AutoPilot = false;
            return null;
            //throw new NotImplementedException("Нет реализации поиска выхода из тупика");
        }

        /// <summary>
        /// Направить змейку на путь
        /// </summary>
        private void TurnToPoint(Point point)
        {
            var fistDirectionPoint = point;
            var headPoint = _snake.points.Find(x => x.Figure == Figures.Head);
            if (headPoint.X - 1 == fistDirectionPoint.X && headPoint.Y == fistDirectionPoint.Y) ChangeDirection(Directions.Left);
            if (headPoint.X + 1 == fistDirectionPoint.X && headPoint.Y == fistDirectionPoint.Y) ChangeDirection(Directions.Right);
            if (headPoint.X == fistDirectionPoint.X && headPoint.Y - 1 == fistDirectionPoint.Y) ChangeDirection(Directions.Up);
            if (headPoint.X == fistDirectionPoint.X && headPoint.Y + 1 == fistDirectionPoint.Y) ChangeDirection(Directions.Down);
        }

        private List<Point> FindShortestWay()
        {
            //TODO предупредить попадание в тупик, в петлю
            _shortWays = new List<List<Point>>();
            _mainWay = new List<Point>();
            var points = GetAllPoints();


            //TODO расчитать оптимальный размер для поля точек
            int[,] simplePoints = new int[_width + 5, _height + 5];
            bool food = false;

            foreach (var item in points)
            {

                switch (item.Figure)
                {
                    case Figures.EmptySpace: simplePoints[item.X, item.Y] = (int)Figures.EmptySpace; break;
                    case Figures.Food: simplePoints[item.X, item.Y] = (int)Figures.Food; food = true; break;
                    default: simplePoints[item.X, item.Y] = -1; break;
                }
            }
            // Нет цели - нет пути
            if (!food)
                return null;

            var headPoint = points.Find(x => x.Figure == Figures.Head);
            // Нет головы
            if (headPoint == null)
                return null;
            var firstWay = new List<Point>
            {
                headPoint
            };
            var possibleWays = new List<List<Point>>
            {
                firstWay
            };
            //TODO вычислить сколько проходов надо
            //TODO не делать перебор всех возможных путей.
            //самый короткий путь ведь можно просчитать по формуле
            int maxSteps = _height + _width + _snake.points.Count + _barrier.points.Count;
            for (int i = 0; i < maxSteps; i++)
            {
                var PossibleWaysBuffer = new List<List<Point>>();
                foreach (var way in possibleWays)
                {
                    var point = way.Last();

                    int x = point.X;
                    int y = point.Y;

                    int left = 0;
                    int right = 0;
                    int up = 0;
                    int down = 0;

                    if (AddWay(ref simplePoints, PossibleWaysBuffer, way, x + 1, y) != -1) right++;
                    if (AddWay(ref simplePoints, PossibleWaysBuffer, way, x, y + 1) != -1) down++;
                    if (AddWay(ref simplePoints, PossibleWaysBuffer, way, x - 1, y) != -1) left++;
                    if (AddWay(ref simplePoints, PossibleWaysBuffer, way, x, y - 1) != -1) up++;

                    // Если сумма 4 путей == 0, значит нет пути
                }
                foreach (var item in PossibleWaysBuffer)
                {
                    possibleWays.Add(item);
                }
                // уменьшение количества путей
                // так как после поиска их становится больше 1000 и сильно влияет на производительность
                if (_shortWays.Count > 0) break;
            }
            if (_shortWays.Count != 0)
            {
                List<Point> shortestWay = _shortWays[0];
                foreach (var item in _shortWays)
                {
                    if (shortestWay.Count > item.Count)
                        shortestWay = item;
                }
                for (int i = 0; i < shortestWay.Count; i++)
                {
                    if (shortestWay[i].Figure == Figures.PosibleWay)
                        shortestWay[i].Figure = Figures.Way;
                }
                return shortestWay;
            }
            else
                return null;
        }

        private int AddWay(ref int[,] mas, List<List<Point>> possibleWays, List<Point> way, int x, int y)

        {
            // Точка является целью
            if (mas[x, y] == (int)Figures.Food)
            {
                var wayBuffer = new List<Point>();
                foreach (var item in way)
                {
                    wayBuffer.Add(item);
                }
                wayBuffer.Add(new Point(x, y, Figures.Way));
                _shortWays.Add(wayBuffer);
                return 1;
            }
            // Точка является свободным местом
            if (mas[x, y] == (int)Figures.EmptySpace)
            {
                mas[x, y] = -2;
                var wayBuffer = new List<Point>();
                foreach (var item in way)
                {
                    wayBuffer.Add(item);
                }
                wayBuffer.Add(new Point(x, y, Figures.PosibleWay));
                possibleWays.Add(wayBuffer);
                return 0;
            }
            else
            {
                return -1;
            }
        }
        /// <summary>
        /// Получить все точки игры
        /// </summary>
        /// <returns></returns>
        public List<Point> GetAllPoints()
        {
            var allPoints = new List<Point>();
            allPoints.AddRange(points);
            allPoints.AddRange(_barrier.points);
            allPoints.AddRange(_snake.points);
            allPoints.AddRange(_food.points);

            if (_mainWay != null)
                allPoints.AddRange(_mainWay);

            if (_shortWays != null)
                foreach (var item in _shortWays)
                    allPoints.AddRange(item);

            return allPoints;
        }

        public override void Draw()
        {
            throw new NotImplementedException();
        }
    }
}
