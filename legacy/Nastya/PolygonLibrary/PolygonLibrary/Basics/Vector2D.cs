using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PolygonLibrary.Toolkit;

namespace PolygonLibrary.Basics
{
  /// <summary>
  /// Class of vectors (elements of linear space) in the plane
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
      int xRes = Tools.CMP(x, v.x);
      if (xRes != 0)
        return xRes;
      else
        return Tools.CMP(y, v.y);
    }

    /// <summary>
    /// Equality of vectors
    /// </summary>
    /// <param name="v1">The first vector</param>
    /// <param name="v2">The second vector</param>
    /// <returns>true, if the vectors coincide; false, otherwise</returns>
    static public bool operator ==(Vector2D v1, Vector2D v2)
    {
      return v1.CompareTo(v2) == 0;
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

    #region Access properties
    /// <summary>
    /// The abscissa
    /// </summary>
    public double x { get; protected set; }

    /// <summary>
    /// The ordinate
    /// </summary>
    public double y { get; protected set; }

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
          return x;
        else if (i == 1)
          return y;
        else
          throw new IndexOutOfRangeException();
#else
        if (i == 0)
          return x;
        else 
          return y;
#endif
      }
    }
    #endregion

    #region Miscellaneous procedures
    /// <summary>
    /// Length of the vector
    /// </summary>
    public double Length { get { return Math.Sqrt(x * x + y * y); } }

    /// <summary>
    /// The polar angle of the vector
    /// </summary>
    public double PolarAngle { get { return Math.Atan2(y, x); } }

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
      x /= l;
      y /= l;
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
      double a = Angle(v1, v2);
      if (a < 0)
        return a + 2 * Math.PI;
      else
        return a;
    }

    /// <summary>
    /// Construct a new vector that is the result of turn of the current vector by a given angle
    /// </summary>
    /// <param name="a">Turn angle (in radians)</param>
    /// <returns>The new vector</returns>
    public Vector2D Turn(double a)
    {
      double c = Math.Cos(a), s = Math.Sin(a);
      return new Vector2D(x * c - y * s, x * s + y * c);
    }

    /// <summary>
    /// Construct a new vector that is the result of clockwise turn of the current vector by a right angle
    /// </summary>
    /// <returns>The new vector</returns>
    public Vector2D TurnCW()
    {
      return new Vector2D(y, -x);
    }

    /// <summary>
    /// Construct a new vector that is the result of counterclockwise turn of the current vector by a right angle
    /// </summary>
    /// <returns>The new vector</returns>
    public Vector2D TurnCCW()
    {
      return new Vector2D(-y, x);
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
      return this.CompareTo(v) == 0;
    }

    public override string ToString()
    {
      return "(" + x + ";" + y + ")";
    }

    public override int GetHashCode()
    {
      return x.GetHashCode() + y.GetHashCode();
    }
    #endregion

    #region Constructors
    /// <summary>
    /// The default construct producing the zero vector
    /// </summary>
    public Vector2D()
    {
      x = 0;
      y = 0;
    }

    /// <summary>
    /// Coordinate constructor
    /// </summary>
    /// <param name="nx">The new abscissa</param>
    /// <param name="ny">The new ordinate</param>
    public Vector2D(double nx, double ny)
    {
      x = nx;
      y = ny;
    }

    /// <summary>
    /// Copying constructor from another vector
    /// </summary>
    /// <param name="v">The vector to be copied</param>
    public Vector2D(Vector2D v)
    {
      x = v.x;
      y = v.y;
    }

    /// <summary>
    /// Copying constructor from a point
    /// </summary>
    /// <param name="p">The point to be copied</param>
    public Vector2D(Point2D p)
    {
      x = p.x;
      y = p.y;
    }
    #endregion

    #region Convertors
    /// <summary>
    /// Explicit convertor to the vector from a point
    /// </summary>
    /// <param name="p">The point to be converted</param>
    /// <returns>The radius vector of the given point</returns>
    static public explicit operator Vector2D(Point2D p)
    {
      return new Vector2D(p.x, p.y);
    }

    /// <summary>
    /// Explicit convertor to a two-dimensional vector from a multidimensional point of gereral kind
    /// </summary>
    /// <param name="v">The point to be converted</param>
    /// <returns>The resultant vector</returns>
    static public explicit operator Vector2D (Point p)
    {
#if DEBUG
      if (p.Dim != 2)
        throw new ArgumentException("A multidimensional point is tried to be converted to a two-dimensional vector!");
#endif
      return new Vector2D(p[0], p[1]);
    }

    /// <summary>
    /// Explicit convertor to a two-dimensional vector from a multidimensional vector of gereral kind
    /// </summary>
    /// <param name="v">The point to be converted</param>
    /// <returns>The resultant point</returns>
    static public explicit operator Vector2D (Vector v)
    {
#if DEBUG
      if (v.Dim != 2)
        throw new ArgumentException("A multidimensional vector is tried to be converted to a two-dimensional point!");
#endif
      return new Vector2D(v[0], v[1]);
    }
    #endregion

    #region Operators
    /// <summary>
    /// Unary minus - the opposite vector
    /// </summary>
    /// <param name="v">The vector to be reversed</param>
    /// <returns>The opposite vector</returns>
    static public Vector2D operator -(Vector2D v)
    {
      return new Vector2D(-v.x, -v.y);
    }

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
    /// <returns>The quotient</returns>
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
    /// Pseudoscalar product (z-component of outer product) 
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

    /// <summary>
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

    /// <summary>
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
