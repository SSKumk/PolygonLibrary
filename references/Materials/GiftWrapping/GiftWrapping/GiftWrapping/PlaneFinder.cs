using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using GiftWrapping.Helpers;
using GiftWrapping.LinearEquations;
using GiftWrapping.Structures;

namespace GiftWrapping
{
    public class PlaneFinder
    {
        public Hyperplane FindFirstPlane(IList<PlanePoint> points)
        {
            int dim = points[0].Dim;
            PlanePoint minPlanePoint = points.Min();
            Vector[] mainBasis = GetFirstBasis(dim);
            bool[] availablePoints = new bool[points.Count];
            availablePoints[points.IndexOf(minPlanePoint)] = true;
            for (int i = 0; i < dim - 1; i++)
            {
                Vector[] basis = mainBasis.Where((_, i1) => i1 != i).ToArray();
                Vector mainVector = mainBasis[i];
                double minCos = double.MaxValue;
                int processedPoint = default;
                Vector[] nextBasis = default;
                Vector normV = default;
                for (int j = 0; j < points.Count; j++)
                {
                    if (availablePoints[j]) continue;
                    Vector vector = Point.ToVector(minPlanePoint, points[j]);
                    Vector ortVector = basis.GetOrthonormalVector(vector);
                    if(Tools.EQ(ortVector.Length)) 
                        continue;
                    Vector[] tempBasis = SetVector(mainBasis, vector.Normalize(), i);
                    double newCos = ortVector.Cos(mainVector);
                    if (Tools.GT(newCos, minCos)) continue;
                    processedPoint = j;
                    minCos = newCos;
                    //normV = vector.Normalize();
                    nextBasis = tempBasis;
                }
                availablePoints[processedPoint] = true;
                mainBasis = nextBasis;
            }
            Hyperplane plane = HyperplaneBuilder.Create(minPlanePoint, mainBasis);
            plane.SetOrientationNormal(points);
            return plane;
        }

        private Vector GetFirstNormal(int dimension)
        {
            double[] normal = new double[dimension];
            normal[0] = -1;

            return new Vector(normal);
        }

        private Vector[] GetFirstBasis(int dimension)
        {
            Vector[] vectors = new Vector[dimension - 1];
            for (int i = 0; i < vectors.Length; i++)
            {
                double[] cells = new double[dimension];
                cells[i + 1] = 1;
                vectors[i] = new Vector(cells);
            }

            return vectors;
        }

        private Vector[] SetVector(Vector[] vectors, Vector vector, int index)
        {
            Vector[] newVectors = (Vector[]) vectors.Clone();
            newVectors[index] = vector;
            return newVectors;
        }

        private Vector FindNormal(Vector[] basis)
        {
            Matrix leftSide = basis.ToHorizontalMatrix();
            double[] rightSide = new double[basis.Length];

            return GaussWithChoiceSolveSystem.FindAnswer(leftSide, rightSide);
        }
    }
}