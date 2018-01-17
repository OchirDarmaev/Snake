using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SnakeGame
{
    public abstract class FigureBase
    {
        public List<Point> points;
        public abstract void Draw();
    }
}
