using System;

namespace GiftWrapping.Structures
{
    public class Point : IComparable<Point>
    {
        #region Internal storage, access properties, and convertors
        /// <summary>
        /// The internal storage of the point as a one-dimensional array
        /// </summary>
        private readonly double[] _p;
       
        /// <summary>
        /// Dimension of the point
        /// </summary>
        public int Dim => _p.Length;

        /// <summary>
        /// Indexer access
        /// </summary>
        /// <param name="i">The index: 0 - the abscissa, 1 - the ordinate</param>
        /// <returns>The value of the corresponding component</returns>
        public double this[int i]
        {
            get
            {
#if DEBUG
                if (i < 0 || i >= Dim)
                    throw new IndexOutOfRangeException();
#endif
                return _p[i];
            }
        }

        /// <summary>
        /// Convert a point to a one-dimensional array
        /// </summary>
        /// <param name="p">The point to be converted</param>
        /// <returns>The resultant array</returns>
        public static implicit operator double[](Point p)
        {
            return p._p;
        }

        /// <summary>
        /// Convert a point to a vector
        /// </summary>
        /// <param name="p">The point to be converted</param>
        /// <returns>The resultant vector</returns>
        public static implicit operator Vector(Point p)
        {
            return new Vector((double[])p);
        }

        /// <summary>
        /// Converting a one-dimensional array to a point
        /// </summary>
        /// <param name="p">Array to be converted</param>
        /// <returns>The resultant point</returns>
        public static explicit operator Point(double[] p)
        {
            return new Point(p);
        }
        #endregion

        #region Comparing
        /// <summary>
        /// Point comparer realizing the lexicographic order
        /// </summary>
        /// <param name="v">The point to be compared with</param>
        /// <returns>+1, if this object greater than p; 0, if they are equal; -1, otherwise</returns>
        public int CompareTo(Point v)
        {
            int d = Dim, res;
#if DEBUG
            if (d != v.Dim)
                throw new ArgumentException("Cannot compare points of different dimensions");
#endif
            for (int i = 0; i < d; i++)
            {
                res = Tools.CMP(this[i], v[i]);
                if (res != 0)
                    return res;
            }

            return 0;
        }

        /// <summary>
        /// Equality of point
        /// </summary>
        /// <param name="p1">The first point</param>
        /// <param name="p2">The second point</param>
        /// <returns>true, if the point coincide; false, otherwise</returns>
        static public bool operator ==( Point p1, Point p2)
        {
            int d = p1.Dim, res;
#if DEBUG
            if (d != p2.Dim)
                throw new ArgumentException("Cannot compare points of different dimensions");
#endif
            for (int i = 0; i < d; i++)
            {
                res = Tools.CMP(p1[i], p2[i]);
                if (res != 0)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Non-equality of point
        /// </summary>
        /// <param name="p1">The first point</param>
        /// <param name="p2">The second point</param>
        /// <returns>true, if the points do not coincide; false, otherwise</returns>
        static public bool operator !=(Point p1, Point p2)
        {
            int d = p1.Dim, res;
#if DEBUG
            if (d != p2.Dim)
                throw new ArgumentException("Cannot compare points of different dimensions");
#endif
            for (int i = 0; i < d; i++)
            {
                res = Tools.CMP(p1[i], p2[i]);
                if (res != 0)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Check whether one point is greater than another (in lexicographic order)
        /// </summary>
        /// <param name="p1">The first point</param>
        /// <param name="p2">The second point</param>
        /// <returns>true, if p1 > p2; false, otherwise</returns>
        static public bool operator >(Point p1, Point p2)
        {
            return p1.CompareTo(p2) > 0;
        }

        /// <summary>
        /// Check whether one point is greater or equal than another (in lexicographic order)
        /// </summary>
        /// <param name="p1">The first point</param>
        /// <param name="p2">The second point</param>
        /// <returns>true, if p1 >= p2; false, otherwise</returns>
        static public bool operator >=(Point p1, Point p2)
        {
            return p1.CompareTo(p2) >= 0;
        }

        /// <summary>
        /// Check whether one point is less than another (in lexicographic order)
        /// </summary>
        /// <param name="p1">The first point</param>
        /// <param name="p2">The second point</param>
        /// <returns>true, if p1 is less than p2; false, otherwise</returns>
        static public bool operator <(Point p1, Point p2)
        {
            return p1.CompareTo(p2) < 0;
        }

        /// <summary>
        /// Check whether one point is less or equal than another (in lexicographic order)
        /// </summary>
        /// <param name="p1">The first point</param>
        /// <param name="p2">The second point</param>
        /// <returns>true, if p1 is less than or equal to p2; false, otherwise</returns>
        static public bool operator <=(Point p1, Point p2)
        {
            return p1.CompareTo(p2) <= 0;
        }
        #endregion

        #region Miscellaneous procedures

        public static Vector ToVector(Point begin, Point end)
        {
            Point vector = end - begin;
            
            return new Vector(vector);
        }

        /// <summary>
        /// Length from the one point to another 
        /// (counted counterclock- or clockwise)
        /// </summary>
        /// <param name="p1">The first point</param>
        /// <param name="p2">The second point</param>
        public static double Length(Point p1, Point p2)
        {
            int d = p1.Dim;
            double sum = 0;
#if DEBUG
            if (d != p2.Dim)
                throw new ArgumentException("Cannot subtract two point of different dimensions");
        #endif
            Point v = p1 - p2;
            for (int i = 0; i < p1.Dim; i++)
            {
                sum += v[i] * v[i];
            }
            return Math.Sqrt(sum);
        }
        #endregion

        #region Overrides
        public override bool Equals(object obj)
        {
        #if DEBUG
            if (!(obj is Point))
                throw new ArgumentException();
        #endif
            Point v = (Point)obj;
            return this.CompareTo(v) == 0;
        }

        public bool Equals(Point other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return this.CompareTo(other) == 0;
        }

        public override string ToString()
        {
            string res =_p[0].ToString();
            int d = Dim, i;
            for (i = 1; i < d; i++)
                res += " " + _p[i];
            // res += ")";
            return res;
        }

        public override int GetHashCode()
        {
            int res = 0;
            for (int i = 0; i < _p.Length; i++)
                res += _p[i].GetHashCode();
            return res;
        }
        #endregion

        #region Constructors
        /// <summary>
        /// The default construct producing the zero point
        /// </summary>
        /// <param name="n">The dimension of the point</param>
        public Point(int n)
        {
#if DEBUG
            if (n <= 0)
                throw new ArgumentException("Dimension of a point cannot be non-positive");
#endif
            _p = new double[n];

        }

        /// <summary>
        /// Constructor on the basis of a one-dimensional array
        /// </summary>
        /// <param name="np">The array</param>
        public Point(double[] np)
        {
#if DEBUG
            if (np.Length <= 0)
                throw new ArgumentException("Dimension of a point cannot be non-positive");
            if (np.Rank != 1)
                throw new ArgumentException("Cannot initialize a point by a multidimensional array");
#endif
            _p = new double[np.Length];
            for (int i = 0; i < np.Length; i++) _p[i] = np[i];

        }

        /// <summary>
        /// Copying constructor
        /// </summary>
        /// <param name="p">The point to be copied</param>
        public Point(Point p)
        {
            int d = p.Dim, i;
            _p = new double[d];
            for (i = 0; i < d; i++)
                _p[i] = p._p[i];

        }

        #endregion

        #region Operators
        /// <summary>
        /// Unary minus - the opposite point
        /// </summary>
        /// <param name="p">The point to be reversed</param>
        /// <returns>The opposite point</returns>
        static public Point operator -(Point p)
        {
            int d = p.Dim, i;
            double[] nv = new double[d];
            for (i = 0; i < d; i++)
                nv[i] = -p._p[i];
            return new Point(nv);
        }

        /// <summary>
        /// Sum of two point
        /// </summary>
        /// <param name="v1">The first point summand</param>
        /// <param name="v2">The second point summand</param>
        /// <returns>The sum</returns>
        static public Point operator +(Point v1, Point v2)
        {
            int d = v1.Dim, i;
#if DEBUG
            if (d != v2.Dim)
                throw new ArgumentException("Cannot add two points of different dimensions");
#endif
            double[] nv = new double[d];
            for (i = 0; i < d; i++)
                nv[i] = v1._p[i] + v2._p[i];
            return new Point(nv);
        }

        /// <summary>
        /// Difference of two point
        /// </summary>
        /// <param name="p1">The point minuend</param>
        /// <param name="p2">The point subtrahend</param>
        /// <returns>The differece</returns>
        public static Point operator -(Point p1, Point p2)
        {
            int d = p1.Dim, i;
#if DEBUG
            if (d != p2.Dim)
                throw new ArgumentException("Cannot subtract two Point of different dimensions");
#endif
            double[] nv = new double[d];
            for (i = 0; i < d; i++)
                nv[i] = p1._p[i] - p2._p[i];
            return new Point(nv);
        }

        /// <summary>
        /// Left multiplication of a point by a number
        /// </summary>
        /// <param name="a">The numeric factor</param>
        /// <param name="p">The point factor</param>
        /// <returns>The product</returns>
        static public Point operator *(double a, Point p)
        {
            int d = p.Dim, i;
            double[] nv = new double[d];
            for (i = 0; i < d; i++)
                nv[i] = a * p._p[i];
            return new Point(nv);
        }

        /// <summary>
        /// Right multiplication of a point by a number
        /// </summary>
        /// <param name="p">The point factor</param>
        /// <param name="a">The numeric factor</param>
        /// <returns>The product</returns>
        static public Point operator *(Point p, double a)
        {
            return a * p;
        }

        /// <summary>
        /// Division of a point by a number
        /// </summary>
        /// <param name="p">The point dividend</param>
        /// <param name="a">The numeric divisor</param>
        /// <returns>The product</returns>
        static public Point operator /(Point p, double a)
        {
#if DEBUG
            if (Tools.EQ(a))
                throw new DivideByZeroException();
#endif
            int d = p.Dim, i;
            double[] nv = new double[d];
            for (i = 0; i < d; i++)
                nv[i] = p._p[i] / a;
            return new Point(nv);
        }
#endregion
    }
}