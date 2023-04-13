using System.Collections.Generic;
using GiftWrapping.Structures;

namespace GiftWrapping
{
    public class GiftWrapping
    {
        public ICell ComputeConvexHull(IList<Point> points)
        {
            return new ConvexHull(3);
        }
    }
}