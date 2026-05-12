using System;
using GiftWrapping.Structures;

namespace GiftWrapping.LinearEquations
{
    public class LinearEquations
    {
        public IndexVector Vector { get; }

        public IndexMatrix Matrix { get; }

        public IndexVector Variables { get; }

        public LinearEquations(Matrix leftSide, Vector rightSide)
        {
            Matrix = new IndexMatrix(leftSide);
            Vector = new IndexVector(rightSide);
            Variables = new IndexVector (new double[Matrix.Cols]);
        }

        public LinearEquations(double[,] leftSide, double[] rightSide)
        {
            Matrix = new IndexMatrix(leftSide);
            Vector = new IndexVector(rightSide);
            Variables = new IndexVector(new double[Matrix.Cols]);
        }

        public void SwapRows(Index index1, Index index2)
        {
            Matrix.SwapRows(index1, index2);
            Vector.SwapCoordinates(index1,index2);
        }

        public void SwapColumns(Index index1, Index index2)
        {
            Matrix.SwapColumns(index1,index2);
            Variables.SwapCoordinates(index1, index2);
        }
    }
}