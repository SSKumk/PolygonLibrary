using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Numerics;

namespace CGLibrary;

/// <summary>
/// Represents an interface for converting between TNum and other numeric types.
/// </summary>
/// <typeparam name="TNum">The type of number.</typeparam>
public interface INumConvertor<TNum> where TNum : INumber<TNum> {

  /// <summary>
  /// Converts a TNum value to a double.
  /// </summary>
  /// <param name="from">The value to convert.</param>
  /// <returns>The converted double value.</returns>
  public static abstract double ToDouble(TNum from);

  /// <summary>
  /// Converts a double value to TNum.
  /// </summary>
  /// <param name="from">The value to convert.</param>
  /// <returns>The converted TNum value.</returns>
  public static abstract TNum FromDouble(double from);

  /// <summary>
  /// Converts a TNum value to an integer.
  /// </summary>
  /// <param name="from">The value to convert.</param>
  /// <returns>The converted integer value.</returns>
  public static abstract int ToInt(TNum from);

  /// <summary>
  /// Converts an integer value to TNum.
  /// </summary>
  /// <param name="from">The value to convert.</param>
  /// <returns>The converted TNum value.</returns>
  public static abstract TNum FromInt(int from);

  /// <summary>
  /// Converts a TNum value to an unsigned integer.
  /// </summary>
  /// <param name="from">The value to convert.</param>
  /// <returns>The converted unsigned integer value.</returns>
  public static abstract uint ToUInt(TNum from);

  /// <summary>
  /// Converts an unsigned integer value to TNum.
  /// </summary>
  /// <param name="from">The value to convert.</param>
  /// <returns>The converted TNum value.</returns>
  public static abstract TNum FromUInt(uint from);

}

public partial class Geometry<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  /// <summary>
  /// Class with general purpose procedures
  /// </summary>
  public class Tools {

#region Fields
    /// <summary>
    /// The random generator.
    /// </summary>
    public static readonly RandomLC rnd = new RandomLC();

    /// <summary>
    /// Absolute accuracy for comparison
    /// </summary>
    private static TNum _eps = TConv.FromDouble(1e-10);
#endregion

#region Constants
    /// <summary>
    /// The absolute accuracy expressed in double.
    /// </summary>
    public static readonly double EpsDouble = TConv.ToDouble(_eps);

    /// <summary>
    /// Represents the positive infinity number.
    /// </summary>
    public static readonly TNum PositiveInfinity = TNum.MultiplicativeIdentity / TNum.AdditiveIdentity;

    /// <summary>
    /// Represents the Zero-value of TNum ('0').
    /// </summary>
    public static readonly TNum Zero = TNum.AdditiveIdentity;

    /// <summary>
    /// Represents the Half of One-value of TNum ('0.5').
    /// </summary>
    public static readonly TNum HalfOne = TConv.FromDouble(0.5);

    /// <summary>
    /// Represents the One-value of TNum ('1').
    /// </summary>
    public static readonly TNum One = TNum.MultiplicativeIdentity;

    /// <summary>
    /// Represents the value 2.
    /// </summary>
    public static readonly TNum Two = TNum.MultiplicativeIdentity + TNum.MultiplicativeIdentity;

    /// <summary>
    /// Represents the value 6.
    /// </summary>
    public static readonly TNum Six = Two + Two + Two;

    /// <summary>
    /// Represents the value of PI.
    /// </summary>
    public static readonly TNum PI = TNum.Abs(TNum.Acos(-One));

    /// <summary>
    /// Represents half of the value of PI.
    /// </summary>
    public static readonly TNum HalfPI = PI / Two;

    /// <summary>
    /// Represents doubled value of the PI.
    /// </summary>
    public static readonly TNum PI2 = PI * Two;
#endregion

#region Double comparison
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
            throw new ArgumentOutOfRangeException("Tools Eps: Non-positive precision parameter");
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
      if (EQ(a)) {
        return 0;
      }
      if (GT(a)) {
        return +1;
      }

      return -1;
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
    /// <returns><c>true</c>, if a &gt; eps; <c>false</c>, otherwise.</returns>
    public static bool GT(TNum a) => a > _eps;

    /// <summary>
    /// "Greater" comparison.
    /// </summary>
    /// <param name="a">The first number.</param>
    /// <param name="b">The second number.</param>
    /// <returns><c>true</c>, if a &gt; b+eps; <c>false</c>, otherwise.</returns>
    public static bool GT(TNum a, TNum b) => GT(a - b);

    /// <summary>
    /// "Greater or equal" comparison.
    /// </summary>
    /// <param name="a">The number.</param>
    /// <returns><c>true</c>, if a &gt;= -eps; <c>false</c>, otherwise.</returns>
    public static bool GE(TNum a) => a >= -_eps;

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
    /// <returns><c>true</c>, if a &lt; -eps; <c>false</c>, otherwise.</returns>
    public static bool LT(TNum a) => a < -_eps;

    /// <summary>
    /// "Less" comparison.
    /// </summary>
    /// <param name="a">The first number.</param>
    /// <param name="b">The second number.</param>
    /// <returns><c>true</c>, if a &lt; b-eps; <c>false</c>, otherwise.</returns>
    public static bool LT(TNum a, TNum b) => LT(a - b);

    /// <summary>
    /// "Less or equal" comparison.
    /// </summary>
    /// <param name="a">The first number.</param>
    /// <returns><c>true</c>, if a &lt;= eps; <c>false</c>, otherwise.</returns>
    public static bool LE(TNum a) => a <= _eps;

    /// <summary>
    /// "Less or equal" comparison.
    /// </summary>
    /// <param name="a">The first number.</param>
    /// <param name="b">The second number.</param>
    /// <returns><c>true</c>, if a &lt;= b+eps; <c>false</c>, otherwise.</returns>
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
      if (EQ(x)) {
        return 0;
      }
      if (GT(x)) {
        return +1;
      }

      return -1;
    }

    /// <summary>
    /// Changing two values
    /// </summary>
    /// <typeparam name="T">The type of the objects to be swapped</typeparam>
    /// <param name="a">The first object</param>
    /// <param name="b">The second object</param>
    public static void Swap<T>(ref T a, ref T b) => (a, b) = (b, a);

    /// <summary>
    /// Calculates the angle, in radians, between the positive x-axis and the point (x, y).
    /// Diapason (-PI, +PI].
    /// </summary>
    /// <param name="y">The y-coordinate of the point.</param>
    /// <param name="x">The x-coordinate of the point.</param>
    /// <returns>The angle, in radians, between the positive x-axis and the point (x, y).</returns>
    public static TNum Atan2(TNum y, TNum x) {
      if (EQ(x) && EQ(y)) {
        return Zero;
      }

      if (TNum.Abs(x) >= TNum.Abs(y)) {
        TNum yx = y / x;

        return GE(x) ? TNum.Atan(yx) : (GE(y) ? TNum.Atan(yx) + PI : TNum.Atan(yx) - PI);
      } else {
        TNum xy = x / y;

        return GE(y) ? (HalfPI - TNum.Atan(xy)) : (-HalfPI - TNum.Atan(xy));
      }
    }

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

    /// <summary>
    /// Generates all possible combinations of a given size from a set of numbers.
    /// </summary>
    /// <param name="n">The total number of items.</param>
    /// <param name="k">The size of each combination.</param>
    /// <returns>An enumerable sequence of integer arrays representing the combinations.</returns>
    public static IEnumerable<int[]> GetCombinations(int n, int k) {
      Debug.Assert(n > 0, $"Tools.GetCombinations: n must be positive! Found {n}.");
      Debug.Assert(k > 0, $"Tools.GetCombinations: n must be positive! Found {k}.");
      Debug.Assert(k <= n, $"Tools.GetCombinations: n must be greater or equal than k! Found n - k = {n - k}.");

      int[] combination = new int[k];
      for (int i = 0; i < k; i++) { combination[i] = i; }

      do {
        yield return combination;
      } while (NextCombination(combination, n, k));
    }

    /// <summary>
    /// Advances the current combination to the next one in lexicographical order.
    /// </summary>
    /// <param name="combination">The current combination array.</param>
    /// <param name="n">The total number of items.</param>
    /// <param name="k">The size of each combination.</param>
    /// <returns>True if the next combination was found, false otherwise.</returns>
    private static bool NextCombination(int[] combination, int n, int k) {
      for (int i = k - 1; i >= 0; i--) {
        if (combination[i] <= n - k + i - 1) {
          combination[i]++;
          for (int j = i + 1; j < k; j++) {
            combination[j] = combination[j - 1] + 1;
          }

          return true;
        }
      }

      return false;
    }

#endregion

  }

}
