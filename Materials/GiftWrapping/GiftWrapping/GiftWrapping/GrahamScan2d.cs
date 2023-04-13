using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GiftWrapping.Structures;

namespace GiftWrapping
{
    public class GrahamScan2d:IAlgorithm
    {
        public IFace FindConvexHull(IList<PlanePoint> points)
        {
          
            List<PlanePoint> sortPoints = points.ToList().MergeSort<PlanePoint>();
           
            List<PlanePoint> up = new List<PlanePoint>(), 
                down = new List<PlanePoint>();
            PlanePoint minPoint = sortPoints[0];
            PlanePoint maxPoint = sortPoints[^1];
            up.Add(minPoint);
            down.Add(minPoint);
            Stopwatch sp = new Stopwatch();
            sp.Start();
            for (int i = 1; i < sortPoints.Count; i++)
            {
                if (i == sortPoints.Count - 1 || cw(minPoint, sortPoints[i], maxPoint))
                {
                    while (up.Count >= 2 && !cw(up[^2], up[^1], sortPoints[i]))
                    {
                        up.RemoveAt(up.Count-1);
                    }
                    up.Add(sortPoints[i]);
                    
                }
                if (i == sortPoints.Count - 1 || cww(minPoint, sortPoints[i], maxPoint))
                {
                    while (down.Count >= 2 && !cww(down[^2], down[^1], sortPoints[i]))
                    {
                        down.RemoveAt(down.Count - 1);
                    }
                    down.Add(sortPoints[i]);
                }
            }
            sp.Stop();
            long ret = sp.ElapsedMilliseconds;
            // for (int i = 0; i < up.Count(); ++i)
            //     points.Add(up[i]);
            for (int i = down.Count() - 2; i > 0; --i)
                up.Add(down[i]);
            return new ConvexHull2d(up);
        }

        bool cw(PlanePoint a, PlanePoint b, PlanePoint c)
        {
            double res = a[0] * (b[1] - c[1]) + b[0] * (c[1] - a[1]) + c[0] * (a[1] - b[1]);
            return Tools.LT(res);
        }

        bool cww(PlanePoint a, PlanePoint b, PlanePoint c)
        {
            double res = a[0] * (b[1] - c[1]) + b[0] * (c[1] - a[1]) + c[0] * (a[1] - b[1]);
            return Tools.GT(res);
        }


        private static void InternalQuickSort(IList<PlanePoint> collection, int leftmostIndex, int rightmostIndex)
        {
            if (leftmostIndex >= rightmostIndex) return;
            int wallIndex = InternalPartition(collection, leftmostIndex, rightmostIndex);
            InternalQuickSort(collection, leftmostIndex, wallIndex - 1);
            InternalQuickSort(collection, wallIndex + 1, rightmostIndex);
        }


        private static int InternalPartition(IList<PlanePoint> collection, int leftmostIndex, int rightmostIndex)
        {
            int pivotIndex = rightmostIndex;
            int wallIndex = leftmostIndex;
            PlanePoint pivotValue = collection[pivotIndex];

            for (int i = leftmostIndex; i <= (rightmostIndex - 1); i++)
            {
                if (collection[i] <= pivotValue)
                {
                    PlanePoint t = collection[i];
                    collection[i] = collection[wallIndex];
                    collection[wallIndex] = t;
                    wallIndex++;
                }
            }

            var temp = collection[wallIndex];
            collection[wallIndex] = collection[pivotIndex];
            collection[wallIndex] = temp;

            return wallIndex;
        }
    }
}