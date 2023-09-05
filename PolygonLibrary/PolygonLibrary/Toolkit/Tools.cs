using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using PolygonLibrary.Basics;

namespace PolygonLibrary.Toolkit;

public partial class Geometry<TNum>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum> {

  /// <summary>
  /// Class with general purpose procedures
  /// </summary>
  public partial class Tools {

#region Constants
    // /// <summary>
    // /// The constant 2*PI
    // /// </summary>
    // public /* static */ const TNum PI2 = 2 * Math.PI;
#endregion

#region Double comparison
    /// <summary>
    /// Absolute accuracy for comparison
    /// </summary>
    private static TNum _eps;

    /// <summary>
    /// Property to deal with the accuracy
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">Is thrown when the precision parameter is not positive</exception>
    public static TNum Eps {
      get { return _eps; }
      set
        {
#if DEBUG
          if (value <= TNum.AdditiveIdentity) {
            throw new ArgumentOutOfRangeException("Non-positive precision parameter");
          }
#endif
          _eps = value;
        }
    }

    /// <summary>
    /// Compares given number with the Zero with precision.
    /// </summary>
    /// <param name="a">The number.</param>
    /// <returns>+1, if a &gt; b; -1, if a &lt; b; 0, otherwise.</returns>
    public static int CMP(TNum a) {
      if (Tools.EQ(a, TNum.AdditiveIdentity)) {
        return 0;
      } else if (a > TNum.AdditiveIdentity) {
        return +1;
      } else {
        return -1;
      }
    }

    /// <summary>
    /// Comparer of two numbers with the precision.
    /// </summary>
    /// <param name="a">The first number.</param>
    /// <param name="b">The second number.</param>
    /// <returns>+1, if a &gt; b; -1, if a &lt; b; 0, otherwise.</returns>
    public static int CMP(TNum a, TNum b) { return CMP(a - b); }


    /// <summary>
    /// Equality of given number to Zero.
    /// </summary>
    /// <param name="a">The number.</param>
    /// <returns><c>true</c>, if |a| &lt; eps; <c>false</c>, otherwise.</returns>
    public static bool EQ(TNum a) => TNum.Abs(a) < _eps;


    /// <summary>
    /// Equality of two numbers with precision.
    /// </summary>
    /// <param name="a">The first number.</param>
    /// <param name="b">The second number.</param>
    /// <returns><c>true</c>, if |a-b| &lt; eps; <c>false</c>, otherwise.</returns>
    public static bool EQ(TNum a, TNum b) => EQ(a - b);

    /// <summary>
    /// Non-equality of given number to Zero.
    /// </summary>
    /// <param name="a">The number.</param>
    /// <returns><c>true</c>, if |a| &gt;= eps; <c>false</c>, otherwise.</returns>
    public static bool NE(TNum a) => !EQ(a);

    /// <summary>
    /// Non-equality of two numbers with precision.
    /// </summary>
    /// <param name="a">The first number.</param>
    /// <param name="b">The second number.</param>
    /// <returns><c>true</c>, if |a-b| &gt;= eps; <c>false</c>, otherwise.</returns>
    public static bool NE(TNum a, TNum b) => !EQ(a, b);

    /// <summary>
    /// "Greater" comparison.
    /// </summary>
    /// <param name="a">The number.</param>
    /// <returns><c>true</c>, if a &gt;= eps; <c>false</c>, otherwise.</returns>
    public static bool GT(TNum a) => a >= _eps;

    /// <summary>
    /// "Greater" comparison.
    /// </summary>
    /// <param name="a">The first number.</param>
    /// <param name="b">The second number.</param>
    /// <returns><c>true</c>, if a &gt;= b+eps; <c>false</c>, otherwise.</returns>
    public static bool GT(TNum a, TNum b) => GT(a - b);

    /// <summary>
    /// "Greater or equal" comparison.
    /// </summary>
    /// <param name="a">The number.</param>
    /// <returns><c>true</c>, if a &gt;= -eps; <c>false</c>, otherwise.</returns>
    public static bool GE(TNum a) => a > -_eps;

    /// <summary>
    /// "Greater or equal" comparison.
    /// </summary>
    /// <param name="a">The first number.</param>
    /// <param name="b">The second number.</param>
    /// <returns><c>true</c>, if a &gt;= b-eps; <c>false</c>, otherwise.</returns>
    public static bool GE(TNum a, TNum b) => GE(a - b);

    /// <summary>
    /// "Less" comparison.
    /// </summary>
    /// <param name="a">The number.</param>
    /// <returns><c>true</c>, if a &lt;= -eps; <c>false</c>, otherwise.</returns>
    public static bool LT(TNum a) => a <= -_eps;

    /// <summary>
    /// "Less" comparison.
    /// </summary>
    /// <param name="a">The first number.</param>
    /// <param name="b">The second number.</param>
    /// <returns><c>true</c>, if a &lt;= b-eps; <c>false</c>, otherwise.</returns>
    public static bool LT(TNum a, TNum b) => LT(a - b);

    /// <summary>
    /// "Less or equal" comparison.
    /// </summary>
    /// <param name="a">The first number.</param>
    /// <returns><c>true</c>, if a &lt; eps; <c>false</c>, otherwise.</returns>
    public static bool LE(TNum a) => a < _eps;

    /// <summary>
    /// "Less or equal" comparison.
    /// </summary>
    /// <param name="a">The first number.</param>
    /// <param name="b">The second number.</param>
    /// <returns><c>true</c>, if a &lt; b+eps; <c>false</c>, otherwise.</returns>
    public static bool LE(TNum a, TNum b) => LE(a - b);

    /// <summary>
    /// Type of a comparer of numbers with the respect to given precision
    /// </summary>
    public class DoubleComparer : IComparer<TNum> {

      private readonly TNum _epsLocal;

      public DoubleComparer(TNum eps) => _epsLocal = eps;

      public int Compare(TNum a, TNum b) {
        TNum oldEPS = Tools.Eps;
        Tools.Eps = _epsLocal;
        int res = Tools.CMP(a, b);
        Tools.Eps = oldEPS;

        return res;
      }

    }
#endregion

#region Common procedures
    /// <summary>
    /// Signum function based of approximate comparison of numbers
    /// </summary>
    /// <param name="x">The value which sign should be found</param>
    /// <returns>The sign of x</returns>
    public static int Sign(TNum x) {
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
    public static void Swap<T>(ref T a, ref T b) => (a, b) = (b, a);

    /// <summary>
    /// Projecting a set of n-dimensional points to the plane by means of a matrix 2 x n
    /// </summary>
    /// <param name="m">The projection matrix</param>
    /// <param name="ps">The set of multidimensional points</param>
    /// <returns>List of two-dimensional projections</returns>
    public static List<Point2D> Project2D(Matrix m, IEnumerable<Point> ps) {
#if DEBUG
      if (m.Rows != 2) {
        throw new ArgumentException("For a projection to the plane a matrix is given with " + m.Rows + " rows!");
      }
#endif
      List<Point2D> res = ps.Select
                             (
                              p => {
#if DEBUG
                                if (p.Dim != m.Cols) {
                                  throw new ArgumentException
                                    ("During projection to the plane a point with wrong dimension has been found!");
                                }
#endif
                                return (Point2D)(m * p);
                              }
                             )
                            .ToList();

      return res;
    }
#endregion

  }

}
