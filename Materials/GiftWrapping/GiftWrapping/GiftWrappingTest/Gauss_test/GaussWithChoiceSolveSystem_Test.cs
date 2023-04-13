using System;
using GiftWrapping.Helpers;
using GiftWrapping.LinearEquations;
using GiftWrapping.Structures;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace GiftWrappingTest.Gauss_test
{
    [TestFixture]
    public class GaussWithChoiceSolveSystem_Test
    {
        private static readonly object[] SetLinearEquationsSystems =
        {
            new object[] {new Matrix(2, 3, new double[] {1, 0, 0, 0, 1, 0}), new Vector(new double[] {0, 0})},
            new object[] {new Matrix(2, 4, new double[] {0,1, 0, 0, 0, 0, 1, 0}), new Vector(new double[] {0, 0})},
            new object[] {new Matrix(2, 4, new double[] {1,0,0, 0, 0, 1,0, 0}), new Vector(new double[] {0, 0})},
            new object[] {new Matrix(2, 3, new double[] {0, 1, 0, 0, 0, 1}), new Vector(new double[] {0, 0})},
            new object[] {new Matrix(2, 3, new double[] {0, 0, 1, 0, 1, 0}), new Vector(new double[] {0, 0})},
            new object[] {new Matrix(2, 3, new double[] {1, 2, 3, 3, 5, 7}), new Vector(new double[] {3, 0})},
            new object[] {new Matrix(2, 3, new double[] {4, 0, 4, 0, 0, 1}), new Vector(new double[] {0, 0})},
            new object[] {new Matrix(2, 3, new double[] {4, 0, 4, 0, 1, 0}), new Vector(new double[] {0, 0})},
        };



        [Test, TestCaseSource(nameof(SetLinearEquationsSystems))]
        public void GetAnswer_AnyMatrix_ReturnVector(Matrix matrix, Vector vector)
        {

            Vector result = GaussWithChoiceSolveSystem.FindAnswer(matrix, vector);
            Vector rightSide = CalculateRightSide(matrix, result);

            Assert.AreEqual(vector, rightSide);
        }


        private Vector CalculateRightSide(Matrix matrix, Vector variables)
        {
            double[] rigtSide = new double[matrix.Rows];
            for (int i = 0; i < matrix.Rows; i++)
            {
                for (int j = 0; j < matrix.Cols; j++)
                {
                    rigtSide[i] += matrix[i, j] * variables[j];
                }
            }

            return new Vector(rigtSide);
        }



        [Test]
        public void GetAnswer_DeterminateMatrix_ReturnVector()
        {
            double[,] points = new double[3, 3]
            {
                {1, 2, 3},
                {3, 5, 7},
                {1, 3, 4}
            };
            Matrix matrix = new Matrix(points);
            Vector vector = new Vector(new double[3] {3, 0, 1});
            Vector expect = new Vector(new double[3] {-4, -13, 11});


            Vector result = GaussWithChoiceSolveSystem.FindAnswer(matrix, vector);

            Assert.AreEqual(expect, result);
        }
    }
}