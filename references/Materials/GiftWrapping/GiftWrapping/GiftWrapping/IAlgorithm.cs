using System.Collections.Generic;
using GiftWrapping.Structures;

namespace GiftWrapping
{
    public interface IAlgorithm
    {
        IFace FindConvexHull(IList<PlanePoint> points);
    }
}