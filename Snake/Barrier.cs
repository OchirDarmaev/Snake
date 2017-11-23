using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SnakeGame
{
    public class Barrier : FigureBase
    {
        public Barrier()
        {
            points = new List<Point>();
        }
        public void AddBarrier(int x, int y)
        {
            points.Add(new Point(x, y, Figures.Barrier));
        }
        public override void Draw()
        {
            throw new NotImplementedException();
        }
    }

}
