using System.Collections.Generic;
using System.Linq;
using GiftWrapping.Structures;

namespace GiftWrappingTest
{
    public static class PointHelper
    {
        public static PlanePoint ToPlanePoint(this Point point)
        {
            return new PlanePoint(point);
        }

        public static PlanePoint[] ToPlanePoint(this IEnumerable<Point> points)
        {
            return points.Select(ToPlanePoint).ToArray();
        }
    }
}