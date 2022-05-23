using System;
using System.Collections.Generic;
using System.Linq;
using PolygonLibrary.Basics;

namespace PolygonLibrary.Toolkit;

/// <summary>
/// Class with general purpose procedures
/// </summary>
public partial class Tools
{
  #region Double comparison
  /// <summary>
  /// Absolute accuracy for comparison
  /// </summary>
  private static double _eps = 1e-8;

  /// <summary>
  /// Property to deal with the accuracy
  /// </summary>
  /// <exception cref="ArgumentOutOfRangeException">Is thrown when the precision parameter is not positive</exception>
  public static double Eps
  {
    get { return _eps; }
    set
    {
#if DEBUG
      if (value <= 0) {
        throw new ArgumentOutOfRangeException("Non-positive precision parameter");
      }
#endif
      _eps = value;
    }
  }

  /// <summary>
  /// Comparer of two doubles with the precision
  /// </summary>
  /// <param name="a">The first number</param>
  /// <param name="b">The second number</param>
  /// <returns>+1, if a &gt; b; -1, if a &lt; b; 0, otherwise</returns>
  public static int CMP(double a, double b = 0)
  {
    if (Tools.EQ(a, b)) {
      return 0;
    } else if (a > b) {
      return +1;
    } else {
      return -1;
    }
  }

  /// <summary>
  /// Equality of two doubles
  /// </summary>
  /// <param name="a">The first number</param>
  /// <param name="b">The second number</param>
  /// <returns>true, if |a-b| &lt; eps; false, otherwise</returns>
  public static bool EQ(double a, double b = 0) => Math.Abs(a - b) < _eps;

  /// <summary>
  /// Non-equality of two doubles
  /// </summary>
  /// <param name="a">The first number</param>
  /// <param name="b">The second number</param>
  /// <returns>true, if |a-b| &gt;= eps; false, otherwise</returns>
  public static bool NE(double a, double b = 0) => !EQ(a, b);

  /// <summary>
  /// "Greater" comparison
  /// </summary>
  /// <param name="a">The first number</param>
  /// <param name="b">The second number</param>
  /// <returns>true, if a &gt;= b+eps; false, otherwise</returns>
  public static bool GT(double a, double b = 0) => a >= b + _eps;

  /// <summary>
  /// "Greater or equal" comparison
  /// </summary>
  /// <param name="a">The first number</param>
  /// <param name="b">The second number</param>
  /// <returns>true, if a &gt;= b-eps; false, otherwise</returns>
  public static bool GE(double a, double b = 0) => a > b - _eps;

  /// <summary>
  /// "Less" comparison
  /// </summary>
  /// <param name="a">The first number</param>
  /// <param name="b">The second number</param>
  /// <returns>true, if a &lt;= b-eps; false, otherwise</returns>
  public static bool LT(double a, double b = 0) => a <= b - _eps;

  /// <summary>
  /// "Less or equal" comparison
  /// </summary>
  /// <param name="a">The first number</param>
  /// <param name="b">The second number</param>
  /// <returns>true, if a &lt; b+eps; false, otherwise</returns>
  public static bool LE(double a, double b = 0) => a < b + _eps;

  /// <summary>
  /// Type of a comparer of doubles with the respect to given precision
  /// </summary>
  public class DoubleComparer : IComparer<double>
  {
    private readonly double _epsLocal;

    public DoubleComparer(double eps) => _epsLocal = eps;

    public int Compare(double a, double b) {
      double oldEPS = Tools.Eps;
      Tools.Eps = _epsLocal;
      int res = Tools.CMP(a, b);
      Tools.Eps = oldEPS;
      return res;
    }
  #endregion

  #region Common procedures
  /// <summary>
  /// Signum function based of approximate comparison of doubles
  /// </summary>
  /// <param name="x">The value which sign should be found</param>
  /// <returns>The sign of x</returns>
  public static int Sign(double x)
  {
    if (Tools.EQ(x)) {
      return 0;
    } else if (Tools.GT(x)) {
      return +1;
    } else {
      return -1;
    }
  }

  /// <summary>
  /// Changing two values
  /// </summary>
  /// <typeparam name="T">The type of the objects to be swapped</typeparam>
  /// <param name="a">The first object</param>
  /// <param name="b">The second object</param>
  public static void Swap<T>(ref T a, ref T b)
  {
    (a, b) = (b, a);
  }

  /// <summary>
  /// Projecting a set of n-dimensional points to the plane by means of a matrix 2 x n
  /// </summary>
  /// <param name="m">The projection matrix</param>
  /// <param name="ps">The set of multidimensional points</param>
  /// <returns></returns>
  public static List<Point2D> Project2D(Matrix m, List<Point> ps)
  {
#if DEBUG
    if (m.Rows != 2) {
      throw new ArgumentException("For a projection to the plane a matrix is given with " +
                                  m.Rows + " rows!");
    }
#endif
    List<Point2D> res = ps.Select(p =>
    {
#if DEBUG
      if (p.Dim != m.Cols) {
        throw new ArgumentException("During projection to the plane a point with wrong dimension has been found!");
      }
#endif
      return (Point2D)(m * (Vector)p);
    }).ToList();

    return res;
  }
  #endregion
}