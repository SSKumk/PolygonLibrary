using System;
using System.Collections.Generic;
using GiftWrapping.Structures;

namespace GiftWrapping.LinearEquations
{
    public class IndexVector
    {
        private readonly IndexMap _rowIndexes;
        private readonly double[] _vector;

        private int Dim { get; }

        public double this[Index i]
        {
            get => _vector[_rowIndexes[i]];
            set => _vector[_rowIndexes[i]] = value;
        }

        public IndexVector(double[] vector)
        {
            this._vector = vector;
            Dim = vector.Length;
            _rowIndexes = new IndexMap(Dim);
        }

        public void SwapCoordinates(Index index1, Index index2)
        {
            int temp = _rowIndexes[index1];
            _rowIndexes[index1] = _rowIndexes[index2];
            _rowIndexes[index2] = temp;
        }

        public static implicit operator double[](IndexVector v)
        {
            return v._vector;
        }

        public static implicit operator Vector(IndexVector v)
        {
            return new Vector(v._vector);
        }
    }
}