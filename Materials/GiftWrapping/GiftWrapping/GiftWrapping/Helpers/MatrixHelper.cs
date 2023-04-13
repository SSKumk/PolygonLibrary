using System;
using GiftWrapping.Structures;

namespace GiftWrapping.Helpers
{
    public static class MatrixHelper
    {
        public static Vector[] ToColumnVectors(this Matrix matrix)
        {
            Vector[] vectors = new Vector[matrix.Cols];

            double[] cells = new double[matrix.Rows];
            for (int i = 0; i < matrix.Cols; i++)
            {
                for (int j = 0; j < matrix.Rows; j++)
                {
                    cells[i] = matrix[j, i];
                }
                vectors[i] = new Vector(cells);
            }

            return vectors;
        }

        public static Vector[] ToRowVectors(this Matrix matrix)
        {
            Vector[] vectors = new Vector[matrix.Rows];

            double[] cells = new double[matrix.Cols];
            for (int i = 0; i < matrix.Rows; i++)
            {
                for (int j = 0; j < matrix.Cols; j++)
                {
                    cells[i] = matrix[i, j];
                }
                vectors[i] = new Vector(cells);
            }

            return vectors;
        }

    }


}