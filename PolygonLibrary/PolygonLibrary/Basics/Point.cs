using System;
using System.Collections.Generic;

using PolygonLibrary.Toolkit;

namespace PolygonLibrary.Basics; 

/// <summary>
/// Class of point in multidimensional space (elements of multidimensional affine space). 
/// It is connected ideologically to the class Vector of multidimensional vectors
/// </summary>
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
      if (i < 0 || i >= Dim) {
        throw new IndexOutOfRangeException();
      }
#endif
      return _p[i];
    }
    protected set
    {
#if DEBUG
      if (i < 0 || i >= Dim) {
        throw new IndexOutOfRangeException();
      }
#endif
      _p[i] = value;
    }
  }

  /// <summary>
  /// Convert a point to a one-dimensional array
  /// </summary>
  /// <param name="v">The point to be converted</param>
  /// <returns>The resultant array</returns>
  public static implicit operator double[](Point v) => v._p;

  /// <summary>
  /// Converting a one-dimensional array to a point
  /// </summary>
  /// <param name="p">Array to be converted</param>
  /// <returns>The resultant point</returns>
  public static explicit operator Point(double[] p) => new Point(p);

  /// <summary>
  /// Explicit convertor to the point from a vector
  /// </summary>
  /// <param name="v">The vector to be converted</param>
  /// <returns>The point, which is the endpoint of the given vector</returns>
  public static explicit operator Point(Vector v) => new Point((double[])v);
  #endregion

  #region Comparing
  /// <summary>
  /// Point comparer realizing the lexicographic order
  /// </summary>
  /// <param name="p">The point to be compared with</param>
  /// <returns>+1, if this object greater than v; 0, if they are equal; -1, otherwise</returns>
  public int CompareTo(Point p)
  {
    int d = Dim, res;
#if DEBUG
    if (d != p.Dim) {
      throw new ArgumentException("Cannot compare points of different dimensions");
    }
#endif
    for (int i = 0; i < d; i++)
    {
      res = Tools.CMP(this._p[i], p._p[i]);
      if (res != 0) {
        return res;
      }
    }
    return 0;
  }

  /// <summary>
  /// Equality of points
  /// </summary>
  /// <param name="p1">The first point</param>
  /// <param name="p2">The second point</param>
  /// <returns>true, if the points coincide; false, otherwise</returns>
  public static bool operator ==(Point p1, Point p2)
  {
    int d = p1.Dim, res;
#if DEBUG
    if (d != p2.Dim) {
      throw new ArgumentException("Cannot compare vectors of different dimensions");
    }
#endif
    for (int i = 0; i < d; i++)
    {
      res = Tools.CMP(p1._p[i], p2._p[i]);
      if (res != 0) {
        return false;
      }
    }
    return true;
  }

  /// <summary>
  /// Non-equality of points
  /// </summary>
  /// <param name="p1">The first points</param>
  /// <param name="p2">The second point</param>
  /// <returns>true, if the points do not coincide; false, otherwise</returns>
  public static bool operator !=(Point p1, Point p2)
  {
    int d = p1.Dim, res;
#if DEBUG
    if (d != p2.Dim) {
      throw new ArgumentException("Cannot compare vectors of different dimensions");
    }
#endif
    for (int i = 0; i < d; i++)
    {
      res = Tools.CMP(p1._p[i], p2._p[i]);
      if (res != 0) {
        return true;
      }
    }
    return false;
  }

  /// <summary>
  /// Check whether one point is greater than another (in lexicographic order)
  /// </summary>
  /// <param name="p1">The first point</param>
  /// <param name="p2">The second point</param>
  /// <returns>true, if p1 &gt; p2; false, otherwise</returns>
  public static bool operator >(Point p1, Point p2) => p1.CompareTo(p2) > 0;

  /// <summary>
  /// Check whether one point is greater or equal than another (in lexicographic order)
  /// </summary>
  /// <param name="p1">The first point</param>
  /// <param name="p2">The second point</param>
  /// <returns>true, if p1 &gt;= p2; false, otherwise</returns>
  public static bool operator >=(Point p1, Point p2) => p1.CompareTo(p2) >= 0;

  /// <summary>
  /// Check whether one point is less than another (in lexicographic order)
  /// </summary>
  /// <param name="p1">The first point</param>
  /// <param name="p2">The second point</param>
  /// <returns>true, if p1 &lt; p2; false, otherwise</returns>
  public static bool operator <(Point p1, Point p2) => p1.CompareTo(p2) < 0;

  /// <summary>
  /// Check whether one point is less or equal than another (in lexicographic order)
  /// </summary>
  /// <param name="p1">The first point</param>
  /// <param name="p2">The second point</param>
  /// <returns>true, if p1 &lt;= p2; false, otherwise</returns>
  public static bool operator <=(Point p1, Point p2) => p1.CompareTo(p2) <= 0;
  #endregion

  #region Miscellaneous procedures
  /// <summary>
  /// Distance to the origin
  /// </summary>
  public double Dist
  {
    get
    {
      double res;
      int i, d = Dim;
      for (i = 0, res = 0; i < d; i++) {
        res += _p[i] * _p[i];
      }

      return Math.Sqrt(res);
    }
  }

  /// <summary>
  /// Get zero point of given dimension
  /// </summary>
  /// <param name="n">The dimension of the point</param>
  /// <returns>The zero point</returns>
  public static Point GetOrigin(int n)
  {
    double[] orig = new double[n];
    return new Point(orig);
  }

  /// <summary>
  /// Storage of the flag showing whether the point is zero
  /// </summary>
  private bool? isZero;

  /// <summary>
  /// Property showing whether the point iz zero;
  /// It computes the corresponding flag if necessary
  /// </summary>
  public bool IsZero
  {
    get
    {
      if (!isZero.HasValue)
      {
        isZero = true;
        for (int i = 0; i < Dim && isZero.Value; i++) {
          isZero = Tools.EQ(this[i]);
        }
      }
      return isZero.Value;
    }
  }
  #endregion

  #region Overrides
  public override bool Equals(object obj)
  {
#if DEBUG
    if (obj is not Point point) {
      throw new ArgumentException($"{obj} is not a Point.");
    }
#endif
    return CompareTo(point) == 0;
  }

  public override string ToString()
  {
    string res = "{" + _p[0];
    int d = Dim, i;
    for (i = 1; i < d; i++) {
      res += ";" + _p[i];
    }

    res += "}";
    return res;
  }

  public override int GetHashCode()
  {
    int res = 0, d = Dim, i;
    for (i = 0; i < d; i++) {
      res += _p[i].GetHashCode();
    }

    return res;
  }
  #endregion

  #region Constructors
  /// <summary>
  /// The default construct producing the origin point of a n-dimensional space
  /// </summary>
  /// <param name="n">The dimension of the point</param>
  public Point(int n)
  {
#if DEBUG
    if (n <= 0) {
      throw new ArgumentException("Dimension of a point cannot be non-positive");
    }
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
    if (np.Length <= 0) {
      throw new ArgumentException("Dimension of a point cannot be non-positive");
    }

    if (np.Rank != 1) {
      throw new ArgumentException("Cannot initialize a point by a multidimensional array");
    }
#endif
    _p = np;
  }

  /// <summary>
  /// Copying constructor from another point
  /// </summary>
  /// <param name="p">The point to be copied</param>
  public Point(Point p)
  {
    int d = p.Dim, i;
    _p = new double[d];
    for (i = 0; i < d; i++) {
      _p[i] = p._p[i];
    }
  }

  /// <summary>
  /// Copying constructor from a two-dimensional point
  /// </summary>
  /// <param name="p">The point to be copied</param>
  public Point(Point2D p) => _p = new double[2] {p.x, p.y};

  /// <summary>
  /// Copying constructor from a vector
  /// </summary>
  /// <param name="v">The vector to be copied</param>
  public Point(Vector v) => _p = (double[])v;

  /// <summary>
  /// Copying constructor from a two-dimensional vector
  /// </summary>
  /// <param name="v">The vector to be copied</param>
  public Point(Vector2D v) => _p = new double[2] {v.x, v.y};
  #endregion

  #region Operators
  /// <summary>
  /// Linear combination of two points 
  /// </summary>
  /// <param name="p1">The first point</param>
  /// <param name="w1">The weight of the first point</param>
  /// <param name="p2">The second point</param>
  /// <param name="w2">The weight of the second point</param>
  /// <returns>The resultant point</returns>
  public static Point LinearCombination(Point p1, double w1, Point p2, double w2)
  {
#if DEBUG
    if (p1.Dim != p2.Dim) {
      throw new ArgumentException("Cannot combine two point of different dimensions");
    }
#endif
    double[] coords = new double[p1.Dim];
    for (int i = 0; i < p1.Dim; i++) {
      coords[i] = w1*p1[i] + w2*p2[i];
    }

    return new Point(coords);
  }

  /// <summary>
  /// Linear combination of a collection of points 
  /// </summary>
  /// <param name="ps">Collection of the points</param>
  /// <param name="ws">Collection of the weights (has at least, the same number of elements as the collection of points)</param>
  /// <returns>The resultant point</returns>
  public static Point LinearCombination(IEnumerable<Point> ps, IEnumerable<double> ws)
  {
    IEnumerator<Point> enPoint = ps.GetEnumerator();
    IEnumerator<double> enWeight = ws.GetEnumerator();

    if (!enPoint.MoveNext()) {
      throw new ArgumentException("No points in the collection to combine");
    }

    if (!enWeight.MoveNext()) {
      throw new ArgumentException("No weights in the collection to combine");
    }

    int dim = enPoint.Current.Dim;
    double[] coords = new double[dim];

    do
    {
#if DEBUG
      if (enPoint.Current.Dim != dim) {
        throw new ArgumentException("Dimension of a point in the collection differs from the dimension of the point");
      }
#endif
      for (int i = 0; i < dim; i++) {
        coords[i] += enPoint.Current[i] * enWeight.Current;
      }
    } while (enPoint.MoveNext() && enWeight.MoveNext());
      
    enPoint.Dispose();
    enWeight.Dispose();
      
    return new Point(coords);
  }

  /// <summary>
  /// Unary minus - the opposite point
  /// </summary>
  /// <param name="p">The point to be reversed</param>
  /// <returns>The opposite point</returns>
  public static Point operator -(Point p)
  {
    int d = p.Dim, i;
    double[] np = new double[d];
    for (i = 0; i < d; i++) {
      np[i] = -p._p[i];
    }

    return new Point(np);
  }

  /// <summary>
  /// Sum of a point and a vector
  /// </summary>
  /// <param name="p">The first point summand</param>
  /// <param name="v">The second vector summand</param>
  /// <returns>The point, which is shift of the original point to the direction of the vector</returns>
  public static Point operator +(Point p, Vector v)
  {
    int d = p.Dim, i;
#if DEBUG
    if (d != v.Dim) {
      throw new ArgumentException("Cannot add a point and a vector of different dimensions");
    }
#endif
    double[] np = new double[d];
    for (i = 0; i < d; i++) {
      np[i] = p._p[i] + v[i];
    }

    return new Point(np);
  }

  /// <summary>
  /// Difference of a point and a vector
  /// </summary>
  /// <param name="p">The point minuend</param>
  /// <param name="v">The vector subtrahend</param>
  /// <returns>The point, which is shift of the original point to the opposite direction of the vector</returns>
  public static Point operator -(Point p, Vector v)
  {
    int d = p.Dim, i;
#if DEBUG
    if (d != v.Dim) {
      throw new ArgumentException("Cannot subtract a point and a vector of different dimensions");
    }
#endif
    double[] np = new double[d];
    for (i = 0; i < d; i++) {
      np[i] = p._p[i] - v[i];
    }

    return new Point(np);
  }

  /// <summary>
  /// Difference of two points 
  /// </summary>
  /// <param name="p1">The point minuend</param>
  /// <param name="p2">The point subtrahend</param>
  /// <returns>The vector directed from the second point to the first one</returns>
  public static Vector operator -(Point p1, Point p2)
  {
    int d = p1.Dim, i;
#if DEBUG
    if (d != p2.Dim) {
      throw new ArgumentException("Cannot subtract two points of different dimensions");
    }
#endif
    double[] nv = new double[d];
    for (i = 0; i < d; i++) {
      nv[i] = p1._p[i] - p2._p[i];
    }

    return new Vector(nv);
  }

  /// <summary>
  /// Left multiplication of a point by a number
  /// </summary>
  /// <param name="a">The numeric factor</param>
  /// <param name="p">The point factor</param>
  /// <returns>The product</returns>
  public static Point operator *(double a, Point p)
  {
    int d = p.Dim, i;
    double[] np = new double[d];
    for (i = 0; i < d; i++) {
      np[i] = a * p._p[i];
    }

    return new Point(np);
  }

  /// <summary>
  /// Right multiplication of a point by a number
  /// </summary>
  /// <param name="p">The point factor</param>
  /// <param name="a">The numeric factor</param>
  /// <returns>The product</returns>
  public static Point operator *(Point p, double a) => a * p;

  /// <summary>
  /// Division of a point by a number
  /// </summary>
  /// <param name="p">The vector dividend</param>
  /// <param name="a">The numeric divisor</param>
  /// <returns>The quotient</returns>
  public static Point operator /(Point p, double a)
  {
#if DEBUG
    if (Tools.EQ(a)) {
      throw new DivideByZeroException();
    }
#endif
    int d = p.Dim, i;
    double[] np = new double[d];
    for (i = 0; i < d; i++) {
      np[i] = p._p[i] / a;
    }

    return new Point(np);
  }
  #endregion
}