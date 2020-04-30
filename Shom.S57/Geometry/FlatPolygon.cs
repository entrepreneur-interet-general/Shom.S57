using System.Collections.Generic;

namespace S57
{
    public class FlatPolygon : Geometry
    {
        public List<Point> points = new List<Point>();
        public List<int> holesIndices = new List<int>();
    }
}
