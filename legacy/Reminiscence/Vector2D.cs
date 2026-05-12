using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PolygonLibrary
{
  /// <summary>
  /// Class of vectors in the plane
  /// </summary>
  public class Vector2D : IComparable<Vector2D>
  {
    #region Comparing
    /// <summary>
    /// Vector comparer realizing the lexicographic order
    /// </summary>
    /// <param name="v">The vector to be compared with</param>
    /// <returns>+1, if this object greater than v; 0, if they are equal; -1, otherwise</returns>
    public int CompareTo(Vector2D v)
    {
      int xRes = Tools.CMP(_x, v._x);
      if (xRes != 0)
        return xRes;
      else
        return Tools.CMP(_y, v._y);
    }

    /// <summary>
    /// Equality of vectors
    /// </summary>
    /// <param name="v1">The first vector</param>
    /// <param name="v2">The second vector</param>
    /// <returns>true, if the vectors coincide; false, otherwise</returns>
    static public bool operator == (Vector2D v1, Vector2D v2)
    {
      return Tools.EQ(v1.x, v2.x) && Tools.EQ(v1.y, v2.y);
    }

    /// <summary>
    /// Non-equality of vectors
    /// </summary>
    /// <param name="v1">The first vector</param>
    /// <param name="v2">The second vector</param>
    /// <returns>true, if the vectors do not coincide; false, otherwise</returns>
    static public bool operator !=(Vector2D v1, Vector2D v2)
    {
      return !(v1 == v2);
    }

    /// <summary>
    /// Check whether one vector is greater than another (in lexicographic order)
    /// </summary>
    /// <param name="v1">The first vector</param>
    /// <param name="v2">The second vector</param>
    /// <returns>true, if v1 > v2; false, otherwise</returns>
    static public bool operator >(Vector2D v1, Vector2D v2)
    {
      return v1.CompareTo(v2) > 0;
    }

    /// <summary>
    /// Check whether one vector is greater or equal than another (in lexicographic order)
    /// </summary>
    /// <param name="v1">The first vector</param>
    /// <param name="v2">The second vector</param>
    /// <returns>true, if v1 >= v2; false, otherwise</returns>
    static public bool operator >=(Vector2D v1, Vector2D v2)
    {
      return v1.CompareTo(v2) >= 0;
    }

    /// <summary>
    /// Check whether one vector is less than another (in lexicographic order)
    /// </summary>
    /// <param name="v1">The first vector</param>
    /// <param name="v2">The second vector</param>
    /// <returns>true, if v1 < v2; false, otherwise</returns>
    static public bool operator <(Vector2D v1, Vector2D v2)
    {
      return v1.CompareTo(v2) < 0;
    }

    /// <summary>
    /// Check whether one vector is less or equal than another (in lexicographic order)
    /// </summary>
    /// <param name="v1">The first vector</param>
    /// <param name="v2">The second vector</param>
    /// <returns>true, if v1 <= v2; false, otherwise</returns>
    static public bool operator <=(Vector2D v1, Vector2D v2)
    {
      return v1.CompareTo(v2) <= 0;
    }
    #endregion

    #region Private storage
    /// <summary>
    /// The abscissa
    /// </summary>
    private double _x;

    /// <summary>
    /// The ordinate
    /// </summary>
    private double _y;
    #endregion

    #region Access properties
    /// <summary>
    /// The abscissa
    /// </summary>
    public double x
    {
      get { return _x; }
    }

    /// <summary>
    /// The ordinate
    /// </summary>
    public double y
    {
      get { return _y; }
    }

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
        if (i == 0)
          return _x;
        else if (i == 1)
          return _y;
        else
          throw new IndexOutOfRangeException();
#else
        if (i == 0)
          return _x;
        else 
          return _y;
#endif
      }
    }
    #endregion

    #region Miscellaneous procedures
    /// <summary>
    /// Length of the vector
    /// </summary>
    public double Length
    {
      get { return Math.Sqrt(_x * _x + _y * _y); }
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
      _x /= l;
      _y /= l;
    }

    /// <summary>
    /// Angle from the one vector to another from the interval [-pi, pi) 
    /// (counted counterclock- or clockwise)
    /// </summary>
    /// <param name="v1">The first vector</param>
    /// <param name="v2">The second vector</param>
    /// <returns>The angle; the angle between a zero vector and any other equals zero</returns>
    public static double Angle(Vector2D v1, Vector2D v2)
    {
      double l1 = v1.Length, l2 = v2.Length;
      if (Tools.EQ(l1) || Tools.EQ(l2))
        return 0;
      else
      {
        double
          l = l1 * l2,
          s = (v1 ^ v2) / l,
          c = (v1 * v2) / l;
        return Math.Atan2(s, c);
      }
    }

    /// <summary>
    /// Angle from the one vector to another from the interval [0, 2\pi) (counted counterclockwise)
    /// </summary>
    /// <param name="v1">The first vector</param>
    /// <param name="v2">The second vector</param>
    /// <returns>The angle; the angle between a zero vector and any other equals zero</returns>
    public static double Angle2PI(Vector2D v1, Vector2D v2)
    {
      double a = Angle (v1, v2);
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
      if (!(obj is Vector2D))
        throw new ArgumentException();
#endif
      Vector2D v = obj as Vector2D;
      return Tools.EQ(_x, v._x) && Tools.EQ(_y, v._y);
    }

    public override string ToString()
    {
      return "(" + _x + ";" + _y + ")";
    }

    public override int GetHashCode()
    {
      return (int)_x + (int)_y;
    }
    #endregion

    #region Constructors
    /// <summary>
    /// Coordinate constructor
    /// </summary>
    /// <param name="nx">The new abscissa</param>
    /// <param name="ny">The new ordinate</param>
    public Vector2D(double nx, double ny)
    {
      _x = nx;
      _y = ny;
    }

    /// <summary>
    /// Copying constructor
    /// </summary>
    /// <param name="v">The vector to be copied</param>
    public Vector2D(Vector2D v)
    {
      _x = v._x;
      _y = v._y;
    }
    #endregion

    #region Operators
    /// <summary>
    /// Sum of two vectors
    /// </summary>
    /// <param name="v1">The first vector summand</param>
    /// <param name="v2">The second vector summand</param>
    /// <returns>The sum</returns>
    static public Vector2D operator +(Vector2D v1, Vector2D v2)
    {
      return new Vector2D(v1.x + v2.x, v1.y + v2.y);
    }

    /// <summary>
    /// Difference of two vectors
    /// </summary>
    /// <param name="v1">The vector minuend</param>
    /// <param name="v2">The vector subtrahend</param>
    /// <returns>The differece</returns>
    static public Vector2D operator -(Vector2D v1, Vector2D v2)
    {
      return new Vector2D(v1.x - v2.x, v1.y - v2.y);
    }

    /// <summary>
    /// Left multiplication of a vector by a number
    /// </summary>
    /// <param name="a">The numeric factor</param>
    /// <param name="v">The vector factor</param>
    /// <returns>The product</returns>
    static public Vector2D operator *(double a, Vector2D v)
    {
      return new Vector2D(a * v.x, a * v.y);
    }

    /// <summary>
    /// Right multiplication of a vector by a number
    /// </summary>
    /// <param name="v">The vector factor</param>
    /// <param name="a">The numeric factor</param>
    /// <returns>The product</returns>
    static public Vector2D operator *(Vector2D v, double a)
    {
      return new Vector2D(a * v.x, a * v.y);
    }

    /// <summary>
    /// Division of a vector by a number
    /// </summary>
    /// <param name="v">The vector dividend</param>
    /// <param name="a">The numeric divisor</param>
    /// <returns>The product</returns>
    static public Vector2D operator /(Vector2D v, double a)
    {
#if DEBUG
      if (Tools.EQ(a))
        throw new DivideByZeroException();
#endif
      return new Vector2D(v.x / a, v.y / a);
    }

    /// <summary>
    /// Dot product 
    /// </summary>
    /// <param name="v1">The first vector factor</param>
    /// <param name="v2">The first vector factor</param>
    /// <returns>The product</returns>
    static public double operator *(Vector2D v1, Vector2D v2)
    {
      return v1.x * v2.x + v1.y * v2.y;
    }

    /// <summary>
    /// pseudoscalar product (z-component of outer product) 
    /// </summary>
    /// <param name="v1">The first vector factor</param>
    /// <param name="v2">The first vector factor</param>
    /// <returns>The z-component of the product</returns>
    static public double operator ^(Vector2D v1, Vector2D v2)
    {
      return v1.x * v2.y - v1.y * v2.x;
    }

    /// <summary>
    /// Parallelity of vectors
    /// </summary>
    /// <param name="v1">The first vector</param>
    /// <param name="v2">The second vector</param>
    /// <returns>true, if the vectors are parallel; false, otherwise</returns>
    static public bool AreParallel(Vector2D v1, Vector2D v2)
    {
      double l1 = v1.Length, l2 = v2.Length;
      return Tools.EQ(Math.Abs(v1 * v2), l1 * l2);
    }

    /// Codirectionality of vectors
    /// </summary>
    /// <param name="v1">The first vector</param>
    /// <param name="v2">The second vector</param>
    /// <returns>true, if the vectors are codirected; false, otherwise</returns>
    static public bool AreCodirected(Vector2D v1, Vector2D v2)
    {
      double l1 = v1.Length, l2 = v2.Length;
      return Tools.EQ(v1 * v2, l1 * l2);
    }

    /// Counterdirectionality of vectors
    /// </summary>
    /// <param name="v1">The first vector</param>
    /// <param name="v2">The second vector</param>
    /// <returns>true, if the vectors are counterdirected; false, otherwise</returns>
    static public bool AreCounterdirected(Vector2D v1, Vector2D v2)
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
    static public bool AreOrthogonal(Vector2D v1, Vector2D v2)
    {
      double l1 = v1.Length, l2 = v2.Length;
      return Tools.EQ(l1) || Tools.EQ(l2) || Tools.EQ(Math.Abs(v1 * v2 / (l1 * l2)));
    }
    #endregion

    #region Vector constants
    /// <summary>
    /// The zero vector
    /// </summary>
    public static readonly Vector2D Zero = new Vector2D(0, 0);

    /// <summary>
    /// The first orth
    /// </summary>
    public static readonly Vector2D e1 = new Vector2D(1, 0);

    /// <summary>
    /// The second orth
    /// </summary>
    public static readonly Vector2D e2 = new Vector2D(0, 1);
    #endregion
  }
}
