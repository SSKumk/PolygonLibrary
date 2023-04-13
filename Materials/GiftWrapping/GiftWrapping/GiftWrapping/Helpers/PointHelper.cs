using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using GiftWrapping.LinearEquations;
using GiftWrapping.Structures;

namespace GiftWrapping.Helpers
{
    public static class PointHelper
    {
        public static bool HaveSameDimension(this IEnumerable<Point> points)
        {
            int dim = points.First().Dim;
            return points.All(v => v.Dim == dim);
        }

        public static Vector[] ToVectors(this IList<PlanePoint> points)
        {
            Vector[] vectors = new Vector[points.Count - 1];
            Point firstPoint = points[0];
            for (int i = 1; i < points.Count; i++)
            {
                vectors[i - 1] = Point.ToVector(firstPoint, points[i]);
            }

            return vectors;
        }

        public static Matrix ToMatrix(this IList<Point> points)
        {
            if (!points.HaveSameDimension())
            {
                throw new ArgumentException("Basis don't have same dimension");
            }
            int n = points.Count, m = points[0].Dim;
            double[] cells = new double[n * m];

            for (int i = 0; i < n; i++)
            {
                Array.Copy(points[i], 0, cells, m * i, m);
            }

            return new Matrix(n, m, cells.ToArray());
        }
        public static Matrix ToMatrix(this Point point)
        {
            return new Matrix(1, point.Dim, point);
        }



     

    }
}