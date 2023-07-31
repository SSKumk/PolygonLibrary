using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using PolygonLibrary.Toolkit;

namespace PolygonLibrary.Basics;

/// <summary>
/// Class of points (elements of affine space) in the plane. 
/// It is connected ideologically to the class Vector2D of planar vectors
/// </summary>
public class Point2D : IComparable<Point2D> {

#region Comparing
  /// <summary>
  /// Point comparer realizing the lexicographic order; coordinates are compared by precision
  /// </summary>
  /// <param name="v">The point to be compared with</param>
  /// <returns>+1, if this object greater than v; 0, if they are equal; -1, otherwise</returns>
  public static int CompareToNoEps(Point2D v1, Point2D v2) {
    int xRes = v1.x.CompareTo(v2.x);
    if (xRes != 0) {
      return xRes;
    } else {
      return v1.y.CompareTo(v2.y);
    }
  }

  /// <summary>
  /// Point comparer realizing the lexicographic order; coordinates are compared by precision
  /// </summary>
  /// <param name="v">The point to be compared with</param>
  /// <returns>+1, if this object greater than v; 0, if they are equal; -1, otherwise</returns>
  public int CompareTo(Point2D? v) {
    Debug.Assert(v is not null, nameof(v) + " != null");
    int xRes = Tools.CMP(x, v.x);
    if (xRes != 0) {
      return xRes;
    } else {
      return Tools.CMP(y, v.y);
    }
  }

  /// <summary>
  /// Equality of points
  /// </summary>
  /// <param name="p1">The first point</param>
  /// <param name="p2">The second point</param>
  /// <returns>true, if the points coincide; false, otherwise</returns>
  public static bool operator ==(Point2D p1, Point2D p2) => p1.CompareTo(p2) == 0;

  /// <summary>
  /// Non-equality of points
  /// </summary>
  /// <param name="p1">The first points</param>
  /// <param name="p2">The second point</param>
  /// <returns>true, if the points do not coincide; false, otherwise</returns>
  public static bool operator !=(Point2D p1, Point2D p2) => p1.CompareTo(p2) != 0;

  /// <summary>
  /// Check whether one point is greater than another (in lexicographic order)
  /// </summary>
  /// <param name="p1">The first point</param>
  /// <param name="p2">The second point</param>
  /// <returns>true, if p1 &gt; p2; false, otherwise</returns>
  public static bool operator >(Point2D p1, Point2D p2) => p1.CompareTo(p2) > 0;

  /// <summary>
  /// Check whether one point is greater or equal than another (in lexicographic order)
  /// </summary>
  /// <param name="p1">The first point</param>
  /// <param name="p2">The second point</param>
  /// <returns>true, if p1 &gt;= p2; false, otherwise</returns>
  public static bool operator >=(Point2D p1, Point2D p2) => p1.CompareTo(p2) >= 0;

  /// <summary>
  /// Check whether one point is less than another (in lexicographic order)
  /// </summary>
  /// <param name="p1">The first point</param>
  /// <param name="p2">The second point</param>
  /// <returns>true, if p1 &lt; p2; false, otherwise</returns>
  public static bool operator <(Point2D p1, Point2D p2) => p1.CompareTo(p2) < 0;

  /// <summary>
  /// Check whether one point is less or equal than another (in lexicographic order)
  /// </summary>
  /// <param name="p1">The first point</param>
  /// <param name="p2">The second point</param>
  /// <returns>true, if p1 &lt;= p2; false, otherwise</returns>
  public static bool operator <=(Point2D p1, Point2D p2) => p1.CompareTo(p2) <= 0;
#endregion

#region Access properties
  /// <summary>
  /// The abscissa
  /// </summary>
  public readonly double x;

  /// <summary>
  /// The ordinate
  /// </summary>
  public readonly double y;

  /// <summary>
  /// Indexer access
  /// </summary>
  /// <param name="i">The index: 0 - the abscissa, 1 - the ordinate</param>
  /// <returns>The value of the corresponding component</returns>
  public double this[int i] {
    get
      {
#if DEBUG
        return i switch
                 {
                   0 => x
                 , 1 => y
                 , _ => throw new IndexOutOfRangeException()
                 };
#else
      return i == 0 ? x : y;
#endif
      }
  }
#endregion

#region Miscellaneous procedures
  /// <summary>
  /// Distance to the origin
  /// </summary>
  public double Abs => Math.Sqrt(x * x + y * y);

  /// <summary>
  /// The polar angle of the point
  /// </summary>
  public double PolarAngle => Math.Atan2(y, x);

  /// <summary>
  /// Compute the distance between two points
  /// </summary>
  /// <param name="p1">The first point</param>
  /// <param name="p2">The second point</param>
  /// <returns>The distance between the given points</returns>
  public static double Dist(Point2D p1, Point2D p2) => Math.Sqrt(Point2D.Dist2(p1, p2));

  /// <summary>
  /// Compute square of the distance between two points
  /// </summary>
  /// <param name="p1">The first point</param>
  /// <param name="p2">The second point</param>
  /// <returns>The square of the distance between the given points</returns>
  public static double Dist2(Point2D p1, Point2D p2) => Math.Pow(p1.x - p2.x, 2) + Math.Pow(p1.y - p2.y, 2);
#endregion

#region Convertors
  /// <summary>
  /// Explicit convertor to a two-dimensional point from a two-dimensional vector
  /// </summary>
  /// <param name="v">The vector to be converted</param>
  /// <returns>The point, which is the endpoint of the given vector</returns>
  public static explicit operator Point2D(Vector2D v) => new Point2D(v.x, v.y);

  /// <summary>
  /// Explicit convertor to a two-dimensional point from a multidimensional point of general kind
  /// </summary>
  /// <param name="p">The point to be converted</param>
  /// <returns>The resultant point</returns>
  public static explicit operator Point2D(Point p) {
#if DEBUG
    if (p.Dim != 2) {
      throw new ArgumentException("A multidimensional point is tried to be converted to a two-dimensional point!");
    }
#endif
    return new Point2D(p[0], p[1]);
  }

  /// <summary>
  /// Explicit convertor to a two-dimensional point from a multidimensional vector of general kind
  /// </summary>
  /// <param name="v">The point to be converted</param>
  /// <returns>The resultant point</returns>
  public static explicit operator Point2D(Vector v) {
#if DEBUG
    if (v.Dim != 2) {
      throw new ArgumentException("A multidimensional vector is tried to be converted to a two-dimensional point!");
    }
#endif
    return new Point2D(v[0], v[1]);
  }
#endregion

#region Overrides
  public override bool Equals(object? obj) {
#if DEBUG
    if (obj is not Point2D point2D) {
      throw new ArgumentException($"{obj} is not a Point2D!");
    }
#endif
    return CompareTo((Point2D)obj) == 0;
  }

  public override string ToString() =>
    "(" + x.ToString(CultureInfo.InvariantCulture) + "," + y.ToString(CultureInfo.InvariantCulture) + ")";

  public override int GetHashCode() {
    int res = 0;
    res = HashCode.Combine(res, (int)(x / Tools.Eps));
    res = HashCode.Combine(res, (int)(y / Tools.Eps));

    return res;
  }
#endregion

#region Constructors
  /// <summary>
  /// The default construct producing the origin point 
  /// </summary>
  public Point2D() {
    x = 0;
    y = 0;
  }

  /// <summary>
  /// Coordinate constructor
  /// </summary>
  /// <param name="nx">The new abscissa</param>
  /// <param name="ny">The new ordinate</param>
  public Point2D(double nx, double ny) {
    x = nx;
    y = ny;
  }

  /// <summary>
  /// Copying constructor from another point
  /// </summary>
  /// <param name="p">The point to be copied</param>
  public Point2D(Point2D p) {
    x = p.x;
    y = p.y;
  }

  /// <summary>
  /// Copying constructor from a vector
  /// </summary>
  /// <param name="v">The vector to be copied</param>
  public Point2D(Vector2D v) {
    x = v.x;
    y = v.y;
  }
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
  public static Point2D LinearCombination(Point2D p1, double w1, Point2D p2, double w2) =>
    new Point2D(w1 * p1.x + w2 * p2.x, w1 * p1.y + w2 * p2.y);

  /// <summary>
  /// Linear combination of three points 
  /// </summary>
  /// <param name="p1">The first point</param>
  /// <param name="w1">The weight of the first point</param>
  /// <param name="p2">The second point</param>
  /// <param name="w2">The weight of the second point</param>
  /// <param name="p3">The third point</param>
  /// <param name="w3">The weight of the third point</param>
  /// <returns>The resultant point</returns>
  public static Point2D LinearCombination(Point2D p1
                                        , double  w1
                                        , Point2D p2
                                        , double  w2
                                        , Point2D p3
                                        , double  w3) => new Point2D(w1 * p1.x + w2 * p2.x + w3 * p3.x, w1 * p1.y + w2 * p2.y + w3 * p3.y);

  /// <summary>
  /// Linear combination of a collection of points 
  /// </summary>
  /// <param name="ps">Collection of the points</param>
  /// <param name="ws">Collection of the weights (has at least, the same number of elements as the collection of points)</param>
  /// <returns>The resultant point</returns>
  public static Point2D LinearCombination(IEnumerable<Point2D> ps, IEnumerable<double> ws) {
    IEnumerator<Point2D> enPoint  = ps.GetEnumerator();
    IEnumerator<double>  enWeight = ws.GetEnumerator();
    double               x        = 0, y = 0;
    while (enPoint.MoveNext() && enWeight.MoveNext()) {
      x += enPoint.Current.x * enWeight.Current;
      y += enPoint.Current.y * enWeight.Current;
    }

    enPoint.Dispose();
    enWeight.Dispose();

    return new Point2D(x, y);
  }

  /// <summary>
  /// Unary minus - the opposite point
  /// </summary>
  /// <param name="p">The point to be reversed</param>
  /// <returns>The opposite point</returns>
  public static Point2D operator -(Point2D p) => new Point2D(-p.x, -p.y);

  /// <summary>
  /// Sum of a point and a vector
  /// </summary>
  /// <param name="p">The first point summand</param>
  /// <param name="v">The second vector summand</param>
  /// <returns>The point, which is shift of the original point to the direction of the vector</returns>
  public static Point2D operator +(Point2D p, Vector2D v) => new Point2D(p.x + v.x, p.y + v.y);

  /// <summary>
  /// Difference of a point and a vector
  /// </summary>
  /// <param name="p">The point minuend</param>
  /// <param name="v">The vector subtrahend</param>
  /// <returns>The point, which is shift of the original point to the opposite direction of the vector</returns>
  public static Point2D operator -(Point2D p, Vector2D v) => new Point2D(p.x - v.x, p.y - v.y);

  /// <summary>
  /// Difference of two points 
  /// </summary>
  /// <param name="p1">The point minuend</param>
  /// <param name="p2">The point subtrahend</param>
  /// <returns>The vector directed from the second point to the first one</returns>
  public static Vector2D operator -(Point2D p1, Point2D p2) => new Vector2D(p1.x - p2.x, p1.y - p2.y);

  /// <summary>
  /// Left multiplication of a point by a number
  /// </summary>
  /// <param name="a">The numeric factor</param>
  /// <param name="p">The point factor</param>
  /// <returns>The product</returns>
  public static Point2D operator *(double a, Point2D p) => new Point2D(a * p.x, a * p.y);

  /// <summary>
  /// Right multiplication of a point by a number
  /// </summary>
  /// <param name="p">The point factor</param>
  /// <param name="a">The numeric factor</param>
  /// <returns>The product</returns>
  public static Point2D operator *(Point2D p, double a) => new Point2D(a * p.x, a * p.y);

  /// <summary>
  /// Division of a point by a number
  /// </summary>
  /// <param name="p">The vector dividend</param>
  /// <param name="a">The numeric divisor</param>
  /// <returns>The quotient</returns>
  public static Point2D operator /(Point2D p, double a) {
#if DEBUG
    if (Tools.EQ(a)) {
      throw new DivideByZeroException();
    }
#endif
    return new Point2D(p.x / a, p.y / a);
  }
#endregion

#region Point constants
  /// <summary>
  /// The zero vector
  /// </summary>
  public static readonly Point2D Origin = new Point2D(0, 0);
#endregion

}
