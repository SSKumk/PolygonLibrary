using System;

namespace GiftWrapping
{
  /// <summary>
  /// Class with general purpose procedures
  /// </summary>
  public class Tools
  {
#region Double comparison
    /// <summary>
    /// Absolute accuracy for comparison
    /// </summary>
    static private double _eps = 1e-6;

    /// <summary>
    /// Property to deal with the accuracy
    /// </summary>
    static public double Eps
    {
      get { return _eps; }
      set
      {
#if DEBUG
        if (value <= 0)
          throw new ArgumentOutOfRangeException();
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
    static public int CMP(double a, double b = 0)
    {
      if (Tools.EQ(a, b))
        return 0;
      else if (a > b)
        return +1;
      else
        return -1;
    }

    /// <summary>
    /// Equality of two doubles
    /// </summary>
    /// <param name="a">The first number</param>
    /// <param name="b">The second number</param>
    /// <returns>true, if |a-b| &lt; eps; false, otherwise</returns>
    static public bool EQ(double a, double b = 0)
    {
      return Math.Abs(a - b) < _eps;
    }

    /// <summary>
    /// Non-equality of two doubles
    /// </summary>
    /// <param name="a">The first number</param>
    /// <param name="b">The second number</param>
    /// <returns>true, if |a-b| &gt;= eps; false, otherwise</returns>
    static public bool NE(double a, double b = 0)
    {
      return !EQ(a, b);
    }

    /// <summary>
    /// "Greater" comparison
    /// </summary>
    /// <param name="a">The first number</param>
    /// <param name="b">The second number</param>
    /// <returns>true, if a &gt;= b+eps; false, otherwise</returns>
    static public bool GT(double a, double b = 0)
    {
      return a >= b + _eps;
    }

    /// <summary>
    /// "Greater or equal" comparison
    /// </summary>
    /// <param name="a">The first number</param>
    /// <param name="b">The second number</param>
    /// <returns>true, if a &gt;= b-eps; false, otherwise</returns>
    static public bool GE(double a, double b = 0)
    {
      return a > b - _eps;
    }

    /// <summary>
    /// "Less" comparison
    /// </summary>
    /// <param name="a">The first number</param>
    /// <param name="b">The second number</param>
    /// <returns>true, if a &lt;= b-eps; false, otherwise</returns>
    static public bool LT(double a, double b = 0)
    {
      return a <= b - _eps;
    }

    /// <summary>
    /// "Less or equal" comparison
    /// </summary>
    /// <param name="a">The first number</param>
    /// <param name="b">The second number</param>
    /// <returns>true, if a &lt; b+eps; false, otherwise</returns>
    static public bool LE(double a, double b = 0)
    {
      return a < b + _eps;
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
      if (Tools.EQ(x))
        return 0;
      else if (x > 0)
        return +1;
      else
        return -1;
    }

    /// <summary>
    /// Changing two values
    /// </summary>
    /// <typeparam name="T">The type of the objects to be swaped</typeparam>
    /// <param name="a">The first object</param>
    /// <param name="b">The second object</param>
    public static void Swap<T>(ref T a, ref T b)
    {
      T temp = a;
      a = b;
      b = temp;
    }
#endregion
  }
}