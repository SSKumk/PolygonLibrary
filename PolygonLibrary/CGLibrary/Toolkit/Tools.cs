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

// Фиг вам. Типы double / ddouble не реализуют интерфейс IGeometryNumber. Обёртка -- дорого. Исходники у double -- нет.
// /// <summary>
// /// Represents a number type used in geometric computations that supports
// /// various mathematical operations such as trigonometric, power, and root functions.
// /// </summary>
// /// <typeparam name="T">
// /// The specific type that implements this interface. It must implement all required
// /// mathematical operations, including trigonometric, power, root functions, and
// /// support floating-point arithmetic.
// /// </typeparam>
// public interface IGeometryNumber<T> : INumber<T>
//                                     , ITrigonometricFunctions<T>
//                                     , IPowerFunctions<T>
//                                     , IRootFunctions<T>
//                                     , IFloatingPoint<T>
//                                     , IFormattable where T : IGeometryNumber<T>;
//
//
// public partial class Geometry<TNum, TConv> where TNum : struct, IGeometryNumber<TNum> where TConv : INumConvertor<TNum>

public partial class Geometry<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  public enum BallType {
    Ball_1,
    Ball_2,
    Ball_oo
  }
  
  /// <summary>
  /// Class with general purpose procedures
  /// </summary>
  public class Tools {

#region Fields
    /// <summary>
    /// The random generator.
    /// </summary>
    public static readonly GRandomLC Random = new GRandomLC();

    /// <summary>
    /// Absolute accuracy for comparison
    /// </summary>
    private static TNum _eps = TConv.FromDouble(1e-8);
#endregion

#region Constants
    // /// <summary>
    // /// Represents the negative infinity number.
    // /// </summary>
    // public static readonly TNum NegativeInfinity = -TNum.MultiplicativeIdentity / TNum.AdditiveIdentity;

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
    /// Represents the Minus one value of TNum ('-1').
    /// </summary>
    public static readonly TNum MinusOne = -TNum.MultiplicativeIdentity;

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
    public static readonly TNum PI = TNum.Pi;//TNum.Abs(TNum.Acos(-One));

    /// <summary>
    /// Represents half of the value of PI.
    /// </summary>
    public static readonly TNum HalfPI = PI / Two;

    /// <summary>
    /// Represents doubled value of the PI.
    /// </summary>
    public static readonly TNum PI2 = PI * Two;
#endregion

#region Comparison
    /// <summary>
    /// Property to deal with the accuracy
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">Is thrown when the precision parameter is not positive</exception>
    public static TNum Eps {
      get => _eps;
      set
        {
          Debug.Assert(value > TNum.AdditiveIdentity, $"Tools.Eps: Non-positive precision parameter. Found {value}");
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
    /// Implements a comparer for TNum values.
    /// </summary>
    public class TNumComparer : IComparer<TNum> {
      public int Compare(TNum a, TNum b) => CMP(a, b);
    }

    /// <summary>
    /// Global comparator for TNum that uses the current value of Tools.Eps for comparison.
    /// </summary>
    public static TNumComparer TComp = new TNumComparer();
#endregion

#region Common procedures

    /// <summary>
    /// Initializes an array of <c>TNum</c> with the specified size,
    /// setting all elements to <c>Zero</c>.
    /// </summary>
    /// <param name="k">The size of the array.</param>
    /// <returns>An array of <c>TNum</c> initialized to <c>Zero</c>.</returns>
    public static TNum[] InitTNumArray(int k) {
      TNum[] array = new TNum[k];
      for (int i = 0; i < array.Length; i++) {
        array[i] = Zero;
      }

      return array;
    }

    /// <summary>
    /// Initializes a two-dimensional array of <c>TNum</c> with the specified dimensions, setting all elements to <c>Zero</c>.
    /// </summary>
    /// <param name="row">The number of rows in the array.</param>
    /// <param name="col">The number of columns in the array.</param>
    /// <returns>A two-dimensional array of <c>TNum</c> initialized to <c>Zero</c>.</returns>
    public static TNum[,] InitTNum2DArray(int row, int col) {
      TNum[,] array2D = new TNum[row, col];
      for (int i = 0; i < row; i++) {
        for (int j = 0; j < col; j++) {
          array2D[i, j] = Zero;
        }
      }

      return array2D;
    }

    /// <summary>
    /// Signum function based of approximate comparison of numbers.
    /// </summary>
    /// <param name="x">The value which sign should be found.</param>
    /// <returns>+1, if a &gt; b; -1, if a &lt; b; 0, otherwise.</returns>
    public static int Sign(TNum x) => CMP(x);

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
      }
      else {
        TNum xy = x / y;

        return GE(y) ? (HalfPI - TNum.Atan(xy)) : (-HalfPI - TNum.Atan(xy));
      }
    }

    /// <summary>
    /// Returns an absolute value of the number.
    /// </summary>
    /// <param name="x">Given number.</param>
    /// <returns>The absolute value. |x|.</returns>
    public static TNum Abs(TNum x) {
      if (EQ(x)) {
        return Zero;
      }
      if (x > Zero) {
        return x;
      }

      return -x;
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
