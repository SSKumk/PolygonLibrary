using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PolygonLibrary.Toolkit;

namespace PolygonLibrary.Basics
{
  /// <summary>
  /// Class of multidimensional vector (elements of multidimensional linear space)
  /// </summary>
  public class Vector : IComparable<Vector>
  {
    #region Internal storage, access properties, and convertors
    /// <summary>
    /// The internal storage of the vector as a one-dimensional array
    /// </summary>
    private double[] _v;

    /// <summary>
    /// Dimension of the vector
    /// </summary>
    public int Dim { get { return _v.Length; } }

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
        return _v[i];
      }
      protected set
      {
#if DEBUG
        if (i < 0 || i >= Dim)
          throw new IndexOutOfRangeException();
#endif
        _v[i] = value;
      }
    }

    /// <summary>
    /// Convert a vector to a one-dimensional array
    /// </summary>
    /// <param name="v">The vector to be converted</param>
    /// <returns>The resultant array</returns>
    public static implicit operator double[](Vector v) { return v._v; }

    /// <summary>
    /// Converting a one-dimensional array to a vector
    /// </summary>
    /// <param name="v">Array to be converted</param>
    /// <returns>The resultant vector</returns>
    public static explicit operator Vector(double[] v)
    {
      return new Vector(v);
    }

    /// <summary>
    /// Explicit convertor to the vector from a point
    /// </summary>
    /// <param name="p">The point to be converted</param>
    /// <returns>The point, which is the endpoint of the given vector</returns>
    static public explicit operator Vector(Point p)
    {
      return new Vector((double[])p);
    }
    #endregion

    #region Comparing
    /// <summary>
    /// Vector comparer realizing the lexicographic order
    /// </summary>
    /// <param name="v">The vector to be compared with</param>
    /// <returns>+1, if this object greater than v; 0, if they are equal; -1, otherwise</returns>
    public int CompareTo(Vector v)
    {
      int d = Dim, res;
#if DEBUG
      if (d != v.Dim)
        throw new ArgumentException("Cannot compare vectors of different dimensions");
#endif
      for (int i = 0; i < d; i++)
      {
        res = Tools.CMP(this._v[i], v._v[i]);
        if (res != 0)
          return res;
      }
      return 0;
    }

    /// <summary>
    /// Equality of vectors
    /// </summary>
    /// <param name="v1">The first vector</param>
    /// <param name="v2">The second vector</param>
    /// <returns>true, if the vectors coincide; false, otherwise</returns>
    static public bool operator ==(Vector v1, Vector v2)
    {
      int d = v1.Dim, res;
#if DEBUG
      if (d != v2.Dim)
        throw new ArgumentException("Cannot compare vectors of different dimensions");
#endif
      for (int i = 0; i < d; i++)
      {
        res = Tools.CMP(v1[i], v2[i]);
        if (res != 0)
          return false;
      }
      return true;
    }

    /// <summary>
    /// Non-equality of vectors
    /// </summary>
    /// <param name="v1">The first vector</param>
    /// <param name="v2">The second vector</param>
    /// <returns>true, if the vectors do not coincide; false, otherwise</returns>
    static public bool operator !=(Vector v1, Vector v2)
    {
      int d = v1.Dim, res;
#if DEBUG
      if (d != v2.Dim)
        throw new ArgumentException("Cannot compare vectors of different dimensions");
#endif
      for (int i = 0; i < d; i++)
      {
        res = Tools.CMP(v1[i], v2[i]);
        if (res != 0)
          return true;
      }
      return false;
    }

    /// <summary>
    /// Check whether one vector is greater than another (in lexicographic order)
    /// </summary>
    /// <param name="v1">The first vector</param>
    /// <param name="v2">The second vector</param>
    /// <returns>true, if v1 > v2; false, otherwise</returns>
    static public bool operator >(Vector v1, Vector v2)
    {
      return v1.CompareTo(v2) > 0;
    }

    /// <summary>
    /// Check whether one vector is greater or equal than another (in lexicographic order)
    /// </summary>
    /// <param name="v1">The first vector</param>
    /// <param name="v2">The second vector</param>
    /// <returns>true, if v1 >= v2; false, otherwise</returns>
    static public bool operator >=(Vector v1, Vector v2)
    {
      return v1.CompareTo(v2) >= 0;
    }

    /// <summary>
    /// Check whether one vector is less than another (in lexicographic order)
    /// </summary>
    /// <param name="v1">The first vector</param>
    /// <param name="v2">The second vector</param>
    /// <returns>true, if v1 < v2; false, otherwise</returns>
    static public bool operator <(Vector v1, Vector v2)
    {
      return v1.CompareTo(v2) < 0;
    }

    /// <summary>
    /// Check whether one vector is less or equal than another (in lexicographic order)
    /// </summary>
    /// <param name="v1">The first vector</param>
    /// <param name="v2">The second vector</param>
    /// <returns>true, if v1 <= v2; false, otherwise</returns>
    static public bool operator <=(Vector v1, Vector v2)
    {
      return v1.CompareTo(v2) <= 0;
    }
    #endregion

    #region Miscellaneous procedures
    /// <summary>
    /// Length of the vector
    /// </summary>
    public double Length
    {
      get
      {
        double res;
        int i, d = Dim;
        for (i = 0, res = 0; i < Dim; i++)
          res += _v[i] * _v[i];
        return Math.Sqrt(res);
      }
    }

    /// <summary>
    /// Normalization of the vector
    /// </summary>
    public void Normalize()
    {
      double l = Length;
#if DEBUG
      if (l == 0)
        throw new DivideByZeroException();
#endif
      int i, d = Dim;
      for (i = 0; i < d; i++)
        _v[i] /= l;
    }

    /// <summary>
    /// Angle from the one vector to another from the interval [-pi, pi) 
    /// (counted counterclock- or clockwise)
    /// </summary>
    /// <param name="v1">The first vector</param>
    /// <param name="v2">The second vector</param>
    /// <returns>The angle; the angle between a zero vector and any other equals zero</returns>
    public static double Angle(Vector v1, Vector v2)
    {
      double l1 = v1.Length, l2 = v2.Length;
      if (Tools.EQ(l1) || Tools.EQ(l2))
        return 0;
      else
        return Math.Acos((v1 * v2) / (l1 * l2));
    }

    /// <summary>
    /// Angle from the one vector to another from the interval [0, 2\pi) (counted counterclockwise)
    /// </summary>
    /// <param name="v1">The first vector</param>
    /// <param name="v2">The second vector</param>
    /// <returns>The angle; the angle between a zero vector and any other equals zero</returns>
    public static double Angle2PI(Vector v1, Vector v2)
    {
      double a = Angle(v1, v2);
      if (a < 0)
        return a + 2 * Math.PI;
      else
        return a;
    }
    #endregion

    #region Overrides
    public override bool Equals(object obj)
    {
#if DEBUG
      if (!(obj is Vector))
        throw new ArgumentException();
#endif
      Vector v = obj as Vector;
      return this.CompareTo(v) == 0;
    }

    public override string ToString()
    {
      string res = "(" + _v[0];
      int d = Dim, i;
      for (i = 1; i < d; i++)
        res += ";" + _v[i];
      res += ")";
      return res;
    }

    public override int GetHashCode()
    {
      int res = 0, d = Dim, i;
      for (i = 0; i < d; i++)
        res += _v[i].GetHashCode();
      return res;
    }
    #endregion

    #region Constructors
    /// <summary>
    /// The default construct producing the zero vector
    /// </summary>
    /// <param name="n">The dimension of the vector</param>
    public Vector(int n)
    {
#if DEBUG
      if (n <= 0)
        throw new ArgumentException("Dimension of a vector cannot be non-positive");
#endif
      _v = new double[n];
    }

    /// <summary>
    /// Constructor on the basis of a one-dimensional array
    /// </summary>
    /// <param name="nv">The array</param>
    public Vector(double[] nv)
    {
#if DEBUG
      if (nv.Length <= 0)
        throw new ArgumentException("Dimension of a vector cannot be non-positive");
      if (nv.Rank != 1)
        throw new ArgumentException("Cannot initialize a vector by a multidimaensional array");
#endif
      _v = nv;
    }

    /// <summary>
    /// Copying constructor from another vector
    /// </summary>
    /// <param name="v">The vector to be copied</param>
    public Vector(Vector v)
    {
      int d = v.Dim, i;
      _v = new double[d];
      for (i = 0; i < d; i++)
        _v[i] = v._v[i];
    }

    /// <summary>
    /// Copying constructor from a point
    /// </summary>
    /// <param name="p">The point to be copied</param>
    public Vector(Point p)
    {
      _v = (double[])p;
    }
    #endregion

    #region Operators
    /// <summary>
    /// Unary minus - the opposite vector
    /// </summary>
    /// <param name="v">The vector to be reversed</param>
    /// <returns>The opposite vector</returns>
    static public Vector operator -(Vector v)
    {
      int d = v.Dim, i;
      double[] nv = new double[d];
      for (i = 0; i < d; i++)
        nv[i] = -v._v[i];
      return new Vector(nv);
    }

    /// <summary>
    /// Sum of two vectors
    /// </summary>
    /// <param name="v1">The first vector summand</param>
    /// <param name="v2">The second vector summand</param>
    /// <returns>The sum</returns>
    static public Vector operator +(Vector v1, Vector v2)
    {
      int d = v1.Dim, i;
#if DEBUG
      if (d != v2.Dim)
        throw new ArgumentException("Cannot add two vectors of different dimensions");
#endif
      double[] nv = new double[d];
      for (i = 0; i < d; i++)
        nv[i] = v1._v[i] + v2._v[i];
      return new Vector(nv);
    }

    /// <summary>
    /// Difference of two vectors
    /// </summary>
    /// <param name="v1">The vector minuend</param>
    /// <param name="v2">The vector subtrahend</param>
    /// <returns>The differece</returns>
    static public Vector operator -(Vector v1, Vector v2)
    {
      int d = v1.Dim, i;
#if DEBUG
      if (d != v2.Dim)
        throw new ArgumentException("Cannot subtract two vectors of different dimensions");
#endif
      double[] nv = new double[d];
      for (i = 0; i < d; i++)
        nv[i] = v1._v[i] - v2._v[i];
      return new Vector(nv);
    }

    /// <summary>
    /// Left multiplication of a vector by a number
    /// </summary>
    /// <param name="a">The numeric factor</param>
    /// <param name="v">The vector factor</param>
    /// <returns>The product</returns>
    static public Vector operator *(double a, Vector v)
    {
      int d = v.Dim, i;
      double[] nv = new double[d];
      for (i = 0; i < d; i++)
        nv[i] = a * v._v[i];
      return new Vector(nv);
    }

    /// <summary>
    /// Right multiplication of a vector by a number
    /// </summary>
    /// <param name="v">The vector factor</param>
    /// <param name="a">The numeric factor</param>
    /// <returns>The product</returns>
    static public Vector operator *(Vector v, double a)
    {
      return a * v;
    }

    /// <summary>
    /// Division of a vector by a number
    /// </summary>
    /// <param name="v">The vector dividend</param>
    /// <param name="a">The numeric divisor</param>
    /// <returns>The product</returns>
    static public Vector operator /(Vector v, double a)
    {
#if DEBUG
      if (Tools.EQ(a))
        throw new DivideByZeroException();
#endif
      int d = v.Dim, i;
      double[] nv = new double[d];
      for (i = 0; i < d; i++)
        nv[i] = v._v[i] / a;
      return new Vector(nv);
    }

    /// <summary>
    /// Dot product 
    /// </summary>
    /// <param name="v1">The first vector factor</param>
    /// <param name="v2">The first vector factor</param>
    /// <returns>The product</returns>
    static public double operator *(Vector v1, Vector v2)
    {
      int d = v1.Dim, i;
#if DEBUG
      if (d != v2.Dim)
        throw new ArgumentException("Cannot compute a dot production of two vectors of different dimensions");
#endif
      double res = 0;
      for (i = 0; i < d; i++)
        res += v1._v[i] * v2._v[i];
      return res;
    }

    /// <summary>
    /// Parallelity of vectors
    /// </summary>
    /// <param name="v1">The first vector</param>
    /// <param name="v2">The second vector</param>
    /// <returns>true, if the vectors are parallel; false, otherwise</returns>
    static public bool AreParallel(Vector v1, Vector v2)
    {
      double l1 = v1.Length, l2 = v2.Length;
      return Tools.EQ(Math.Abs(v1 * v2), l1 * l2);
    }

    /// <summary>
    /// Codirectionality of vectors
    /// </summary>
    /// <param name="v1">The first vector</param>
    /// <param name="v2">The second vector</param>
    /// <returns>true, if the vectors are codirected; false, otherwise</returns>
    static public bool AreCodirected(Vector v1, Vector v2)
    {
      double l1 = v1.Length, l2 = v2.Length;
      return Tools.EQ(v1 * v2, l1 * l2);
    }

    /// <summary>
    /// Counterdirectionality of vectors
    /// </summary>
    /// <param name="v1">The first vector</param>
    /// <param name="v2">The second vector</param>
    /// <returns>true, if the vectors are counterdirected; false, otherwise</returns>
    static public bool AreCounterdirected(Vector v1, Vector v2)
    {
      double l1 = v1.Length, l2 = v2.Length;
      return Tools.EQ(v1 * v2, -l1 * l2);
    }

    /// <summary>
    /// Orthogonality of vectors
    /// </summary>
    /// <param name="v1">The first vector</param>
    /// <param name="v2">The second vector</param>
    /// <returns>true, if the vectors are orthognal; false, otherwise</returns>
    static public bool AreOrthogonal(Vector v1, Vector v2)
    {
      double l1 = v1.Length, l2 = v2.Length;
      return Tools.EQ(l1) || Tools.EQ(l2) || Tools.EQ(Math.Abs(v1 * v2 / (l1 * l2)));
    }
    #endregion
  }
}
