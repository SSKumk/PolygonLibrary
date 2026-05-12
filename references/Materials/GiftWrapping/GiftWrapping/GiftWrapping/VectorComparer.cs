using System;
using System.Collections.Generic;
using GiftWrapping.Structures;

namespace GiftWrapping
{
    public class VectorComparer:IEqualityComparer<Hyperplane>
    {
        private const double Accuracy = 1000000;
        public bool Equals(Hyperplane x, Hyperplane y)
        {
            return !ReferenceEquals(null, x) && x.Equals(y);
        }

        public int GetHashCode(Hyperplane obj)
        {
            // int res = 0;
            // for (int i = 0; i < obj.Normal.Dim; i++)
            // {
            //     int num =  (int)(obj.Normal[i] * Accuracy);
            //     res += num.GetHashCode();
            //     res += i.GetHashCode();
            // }
            //
            // res += (int)(obj.NumericVariable * Accuracy);

            int hc = obj.Dimension;
            for (int i = 0; i < obj.Dimension; ++i)
            {
                hc = unchecked(hc * 314159 + (int)(obj.Normal[i] * Accuracy));
            }
            return hc;

        }
    }
}