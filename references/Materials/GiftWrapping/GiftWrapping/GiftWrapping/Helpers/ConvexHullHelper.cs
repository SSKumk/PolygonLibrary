using System.Collections.Generic;
using GiftWrapping.Structures;

namespace GiftWrapping.Helpers
{
    public static class ConvexHullHelper
    {
        public static ConvexHull2d ToConvexHull2d(this IList<PlanePoint> points)=> new ConvexHull2d(points);
    }
}