using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GiftWrapping.Structures;

namespace GiftWrapping
{
    public class GiftWrapping2d:IAlgorithm
    {
        public IFace FindConvexHull(IList<PlanePoint> points)
        {
            if (points.Count == 3)
            {
                return new ConvexHull2d(points);
            }
            List<PlanePoint> hullPoints = new List<PlanePoint>();
            int first = points.IndexOf(points.Min());
            Vector currentVector = new Vector(new double[] { 0, -1 });
            int currentPlanePoint = first;
            bool[] processedPoint = new bool[points.Count];
            do
            {
                hullPoints.Add(points[currentPlanePoint]);
                double maxCos = double.MinValue;
                double maxLen = double.MinValue;
                int next = 0;
                Vector maxVector = currentVector;
                for (int i = 0; i < points.Count; i++)
                {
                    if (currentPlanePoint == i || processedPoint[i]) continue;
                    Vector newVector = Point.ToVector(points[currentPlanePoint], points[i]);
                    double newCos = currentVector * newVector;
                    newCos /= newVector.Length * currentVector.Length;
                    if (Tools.GT(newCos, maxCos))
                    {
                        maxCos = newCos;
                        next = i;
                        maxLen = Point.Length(points[currentPlanePoint], points[next]);
                        maxVector = newVector;
                    }
                    else if (Tools.EQ(maxCos, newCos))
                    {
                        double dist = Point.Length(points[currentPlanePoint], points[i]);
                        if (Tools.LT(maxLen, dist))
                        {
                            next = i;
                            maxVector = newVector;
                            maxLen = dist;
                        }
                    }
                }

                processedPoint[next] = true;
                currentPlanePoint = next;
                currentVector = maxVector;
            } while (first != currentPlanePoint);

            ConvexHull2d hull = new ConvexHull2d(hullPoints);
            return hull;
        }
    }
}