using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using GiftWrapping.LinearEquations;
using GiftWrapping.Structures;

namespace GiftWrapping.Helpers
{
    public static class HyperplaneBuilder
    {
        public static Hyperplane Create(IList<PlanePoint> points)
        {
            if (!points.HaveSameDimension())
            {
                throw new ArgumentException("Basis don't have same dimension");
            }
            if (points.Count != points[0].Dim)
            {
                throw new ArgumentException("Number of points is not equal to dimension.");
            }
            Vector[] vectors = points.ToVectors();
            Hyperplane hyperplane = Create(points.First(), vectors);
            

            return hyperplane;
        }
        public static Hyperplane Create(PlanePoint point, Vector[] vectors)
        {
            if (!vectors.HaveSameDimension())
            {
                throw new ArgumentException("Vectors don't have same dimension");
            }
            if (point.Dim != vectors[0].Dim)
            {
                throw new ArgumentException("Vectors and points have different dimensions.");
            }

            Matrix leftSide = vectors.ToHorizontalMatrix();

            Vector normal = ComputeNormal(leftSide);

            return new Hyperplane(point, normal)
            {
                Basis = vectors.GetOrthonormalBasis()
            };
        }
      

        private static Vector ComputeNormal(Matrix leftSide)
        {
            Vector rightSide = new Vector(leftSide.Rows);
            try
            {
                return GaussWithChoiceSolveSystem.FindAnswer(leftSide, rightSide);
            }
            catch (InvalidOperationException)
            {
                throw new ArgumentException("Vectors are linearly dependent");
            }
        }

        public static IList<PlanePoint> GetPlanePoints(this Hyperplane h, IList<PlanePoint> points)
        {
            List<PlanePoint> result = new List<PlanePoint>();
            for (int i = 0; i < points.Count; i++)
            {
                if (h.IsPointInPlane(points[i]))
                {
                    result.Add(points[i]);
                }
            }

            return result;
        }
    }
}