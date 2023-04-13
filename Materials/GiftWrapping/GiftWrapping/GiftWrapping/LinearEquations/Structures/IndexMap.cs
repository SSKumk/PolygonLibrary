using System;
using System.Linq;

namespace GiftWrapping.LinearEquations
{
    public class IndexMap
    {
        public int[] _indexes { get; }

        public int Length { get; }

        public int this[Index i]
        {
            set => _indexes[i]= value;
            get => _indexes[i];
        }

        public IndexMap(int[] map)
        {
            Length = map.Length;
            _indexes = (int[])map.Clone();
        }

        public IndexMap(int length)
        {
            Length = length;
            _indexes = Enumerable.Range(0, length).ToArray();
        }
    }
}