using System.Globalization;

namespace CGLibrary;

public partial class Geometry<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum>
{

  /// <summary>
  /// Class of vectors in the plane
  /// </summary>
  public class Vector2D : IComparable<Vector2D>
  {

    #region Comparing
    /// <summary>
    /// Vector comparer realizing the lexicographic order; coordinates are compared by precision
    /// </summary>
    /// <param name="v1">The first point to compare.</param>
    /// <param name="v2">The second point to be compared with.</param>
    /// <returns>+1, if this object greater than v; 0, if they are equal; -1, otherwise</returns>
    public static int CompareToNoEps(Vector2D v1, Vector2D v2)
    {
      int xRes = v1.x.CompareTo(v2.x);
      if (xRes != 0)
      {
        return xRes;
      }

      return v1.y.CompareTo(v2.y);
    }

    /// <summary>
    /// Vector comparer realizing the lexicographic order
    /// </summary>
    /// <param name="v">The vector to be compared with</param>
    /// <returns>+1, if this object greater than v; 0, if they are equal; -1, otherwise</returns>
    public int CompareTo(Vector2D? v)
    {
      if (v is null) { return 1; } // null < this (always)

      int xRes = Tools.CMP(x, v.x);
      if (xRes != 0)
      {
        return xRes;
      }
      else
      {
        return Tools.CMP(y, v.y);
      }
    }

    /// <summary>
    /// Equality of vectors
    /// </summary>
    /// <param name="v1">The first vector</param>
    /// <param name="v2">The second vector</param>
    /// <returns>true, if the vectors coincide; false, otherwise</returns>
    public static bool operator ==(Vector2D v1, Vector2D v2) => Tools.EQ(v1.x, v2.x) && Tools.EQ(v1.y, v2.y);

    /// <summary>
    /// Non-equality of vectors
    /// </summary>
    /// <param name="v1">The first vector</param>
    /// <param name="v2">The second vector</param>
    /// <returns>true, if the vectors do not coincide; false, otherwise</returns>
    public static bool operator !=(Vector2D v1, Vector2D v2) => !(v1 == v2);

    /// <summary>
    /// Check whether one vector is greater than another (in lexicographic order)
    /// </summary>
    /// <param name="v1">The first vector</param>
    /// <param name="v2">The second vector</param>
    /// <returns>true, if p1 > p2; false, otherwise</returns>
    public static bool operator >(Vector2D v1, Vector2D v2) => v1.CompareTo(v2) > 0;

    /// <summary>
    /// Check whether one vector is greater or equal than another (in lexicographic order)
    /// </summary>
    /// <param name="v1">The first vector</param>
    /// <param name="v2">The second vector</param>
    /// <returns>true, if p1 >= p2; false, otherwise</returns>
    public static bool operator >=(Vector2D v1, Vector2D v2) => v1.CompareTo(v2) >= 0;

    /// <summary>
    /// Check whether one vector is less than another (in lexicographic order)
    /// </summary>
    /// <param name="v1">The first vector</param>
    /// <param name="v2">The second vector</param>
    /// <returns>true, if p1 is less than p2; false, otherwise</returns>
    public static bool operator <(Vector2D v1, Vector2D v2) => v1.CompareTo(v2) < 0;

    /// <summary>
    /// Check whether one vector is less or equal than another (in lexicographic order)
    /// </summary>
    /// <param name="v1">The first vector</param>
    /// <param name="v2">The second vector</param>
    /// <returns>true, if p1 is less than or equal to p2; false, otherwise</returns>
    public static bool operator <=(Vector2D v1, Vector2D v2) => v1.CompareTo(v2) <= 0;
    #endregion

    #region Access properties
    /// <summary>
    /// The abscissa
    /// </summary>
    public readonly TNum x;

    /// <summary>
    /// The ordinate
    /// </summary>
    public readonly TNum y;

    /// <summary>
    /// Indexer access
    /// </summary>
    /// <param name="i">The index: 0 - the abscissa, 1 - the ordinate</param>
    /// <returns>The value of the corresponding component</returns>
    public TNum this[int i]
      => i switch
      {
        0 => x
           ,
        1 => y
           ,
        _ => throw new IndexOutOfRangeException()
      };


    /// <summary>
    /// The length field
    /// </summary>
    private TNum? length = null;

    /// <summary>
    /// length of the vector
    /// </summary>
    public TNum Length
    {
      get
      {
        length ??= TNum.Sqrt(x * x + y * y);

        return length.Value;
      }
    }

    /// <summary>
    /// Property showing if the vector2D is zero vector
    /// </summary>
    public bool IsZero => Tools.EQ(Length);

    /// <summary>
    /// The polar angle field
    /// </summary>
    private TNum? polarAngle = null;

    /// <summary>
    /// The polar angle of the vector in the range (-pi;pi]
    /// </summary>
    public TNum PolarAngle
    {
      get
      {
        if (polarAngle is null)
        {
          polarAngle = Tools.Atan2(y, x);
          // КОСТЫЛИЩЕ - DUCT TAPE
          if (Tools.EQ(PolarAngle, -Tools.PI))
            polarAngle = -Tools.PI;
        }

        return polarAngle.Value;
      }
    }
    #endregion

    #region Miscellaneous procedures
    /// <summary>
    /// Distance to the origin
    /// </summary>
    public TNum Abs => TNum.Sqrt(x * x + y * y);

    /// <summary>
    /// Compute the distance between two points
    /// </summary>
    /// <param name="p1">The first point</param>
    /// <param name="p2">The second point</param>
    /// <returns>The distance between the given points</returns>
    public static TNum Dist(Vector2D p1, Vector2D p2) => TNum.Sqrt(Vector2D.Dist2(p1, p2));

    /// <summary>
    /// Compute square of the distance between two points
    /// </summary>
    /// <param name="p1">The first point</param>
    /// <param name="p2">The second point</param>
    /// <returns>The square of the distance between the given points</returns>
    public static TNum Dist2(Vector2D p1, Vector2D p2) => TNum.Pow(p1.x - p2.x, Tools.Two) + TNum.Pow(p1.y - p2.y, Tools.Two);

    /// <summary>
    /// Normalization of the vector
    /// </summary>
    /// <returns>
    /// The normalized vector.
    /// </returns>
    /// <exception cref="DivideByZeroException">
    /// Is thrown if the vector is zero
    /// </exception>
    public Vector2D Normalize()
    {
#if DEBUG
      if (IsZero)
      {
        throw new DivideByZeroException();
      }
#endif
      return new Vector2D(x / Length, y / Length);
    }

    /// <summary>
    /// Normalization of the vector with the zero vector check
    /// </summary>
    /// <returns>
    /// The normalized vector. If the vector is zero, then zero is returned
    /// </returns>
    public Vector2D NormalizeZero() => IsZero ? this : new Vector2D(x / Length, y / Length);


    /// <summary>
    /// Construct a vector obtained from the current one by a clockwise 90-degree turn
    /// </summary>
    /// <returns>The turned vector</returns>
    public Vector2D TurnCW() => new Vector2D(y, -x);

    /// <summary>
    /// Construct a vector obtained from the current one by a counterclockwise 90-degree turn
    /// </summary>
    /// <returns>The turned vector</returns>
    public Vector2D TurnCCW() => new Vector2D(-y, x);

    /// <summary>
    /// Construct a vector obtained from the current one by a (counterclockwise) turn by a given angle
    /// </summary>
    /// <param name="angle">The turn angle</param>
    /// <returns>The turned vector</returns>
    public Vector2D Turn(TNum angle)
    {
      (TNum sn, TNum cs) = TNum.SinCos(angle);
      // TNum cs = TNum.Cos(angle), sn = TNum.Sin(angle);

      return new Vector2D(cs * x - sn * y, sn * x + cs * y);
    }

    /// <summary>
    /// Checks if the current vector is strictly between the two given, that is,
    /// the angle counted counterclockwise from the first vector to the current one
    /// is less than the angle counted counterclockwise from the first vector to the second one.
    /// With that, the angle angle counted counterclockwise from the first vector to the current one
    /// should be greater than zero.
    /// </summary>
    /// <param name="v1">The first vector, the clockwise boundary of the cone</param>
    /// <param name="v2">The second vector, the counterclockwise boundary of the cone</param>
    /// <returns>Flag showing whether the vector belongs to the given cone</returns>
    public bool IsBetween(Vector2D v1, Vector2D v2)
    {
      TNum a1 = Vector2D.Angle2PI(v1, this), a2 = Vector2D.Angle2PI(v1, v2);

      return Tools.GT(a1) && Tools.LT(a1, a2);
    }

    /// <summary>
    /// Angle from the one vector to another from the interval (-pi, pi]
    /// (counted counterclockwise if positive, or clockwise if negative)
    /// </summary>
    /// <param name="v1">The first vector</param>
    /// <param name="v2">The second vector</param>
    /// <returns>The angle; the angle between a zero vector and any other equals zero</returns>
    public static TNum Angle(Vector2D v1, Vector2D v2)
    {
      if (v1.IsZero || v2.IsZero)
      {
        return Tools.Zero;
      }
      else
      {
        TNum s = (v1 ^ v2), c = (v1 * v2);

        return Tools.Atan2(s, c);
      }
    }

    /// <summary>
    /// Angle from the one vector to another from the interval [0, 2\pi) (counted counterclockwise)
    /// </summary>
    /// <param name="v1">The first vector</param>
    /// <param name="v2">The second vector</param>
    /// <returns>The angle; the angle between a zero vector and any other equals zero</returns>
    public static TNum Angle2PI(Vector2D v1, Vector2D v2)
    {
      TNum a = Angle(v1, v2);

      return a < Geometry<TNum, TConv>.Tools.Zero ? a + Tools.Two * Tools.PI : a;
    }
    #endregion

    #region Convertors
    /// <summary>
    /// Explicit convertor to a two-dimensional vector from a multidimensional vector of general kind
    /// </summary>
    /// <param name="v">The vector to be converted</param>
    /// <returns>The resultant vector</returns>
    public static explicit operator Vector2D(Vector v)
    {
#if DEBUG
      if (v.Dim != 2)
      {
        throw new ArgumentException("A multidimensional vector is tried to be converted to a two-dimensional vector!");
      }
#endif
      return new Vector2D(v[0], v[1]);
    }
    #endregion

    #region Overrides
    public override bool Equals(object? obj)
    {
#if DEBUG
      if (obj is not Vector2D v)
      {
        throw new ArgumentException($"{obj} is not a Vector2D!");
      }
#endif
      return CompareTo((Vector2D)obj!) == 0;
    }

    public override string ToString()
      => $"({x.ToString(null, CultureInfo.InvariantCulture)};{y.ToString(null, CultureInfo.InvariantCulture)})";
    #endregion

    #region Constructors and factories
    /// <summary>
    /// The default construct producing the zero vector
    /// </summary>
    public Vector2D()
    {
      x = Geometry<TNum, TConv>.Tools.Zero;
      y = Geometry<TNum, TConv>.Tools.Zero;
    }

    /// <summary>
    /// Coordinate constructor
    /// </summary>
    /// <param name="nx">The new abscissa</param>
    /// <param name="ny">The new ordinate</param>
    public Vector2D(TNum nx, TNum ny)
    {
      x = nx;
      y = ny;
    }

    /// <summary>
    /// Copying constructor
    /// </summary>
    /// <param name="v">The vector to be copied</param>
    public Vector2D(Vector2D v)
    {
      x = v.x;
      y = v.y;
    }

    /// <summary>
    /// Factory producing a two-dimensional vector by its polar coordinates
    /// </summary>
    /// <param name="angle">The polar angle (in radians!)</param>
    /// <param name="radius">The radius (can be negative!)</param>
    /// <returns>The resultant vector</returns>
    public static Vector2D FromPolar(TNum angle, TNum radius) => new Vector2D(radius * TNum.Cos(angle), radius * TNum.Sin(angle));
    #endregion

    #region Operators
    /// <summary>
    /// Unary minus - the opposite vector
    /// </summary>
    /// <param name="v">The vector to be reversed</param>
    /// <returns>The opposite vector</returns>
    public static Vector2D operator -(Vector2D v) => new Vector2D(-v.x, -v.y);

    /// <summary>
    /// Sum of two vectors
    /// </summary>
    /// <param name="v1">The first vector summand</param>
    /// <param name="v2">The second vector summand</param>
    /// <returns>The sum</returns>
    public static Vector2D operator +(Vector2D v1, Vector2D v2) => new Vector2D(v1.x + v2.x, v1.y + v2.y);

    /// <summary>
    /// Difference of two vectors
    /// </summary>
    /// <param name="v1">The vector minuend</param>
    /// <param name="v2">The vector subtrahend</param>
    /// <returns>The difference</returns>
    public static Vector2D operator -(Vector2D v1, Vector2D v2) => new Vector2D(v1.x - v2.x, v1.y - v2.y);

    /// <summary>
    /// Left multiplication of a vector by a number
    /// </summary>
    /// <param name="a">The numeric factor</param>
    /// <param name="v">The vector factor</param>
    /// <returns>The product</returns>
    public static Vector2D operator *(TNum a, Vector2D v) { return new Vector2D(a * v.x, a * v.y); }

    /// <summary>
    /// Right multiplication of a vector by a number
    /// </summary>
    /// <param name="v">The vector factor</param>
    /// <param name="a">The numeric factor</param>
    /// <returns>The product</returns>
    public static Vector2D operator *(Vector2D v, TNum a) { return new Vector2D(a * v.x, a * v.y); }

    /// <summary>
    /// Division of a vector by a number
    /// </summary>
    /// <param name="v">The vector dividend</param>
    /// <param name="a">The numeric divisor</param>
    /// <returns>The product</returns>
    public static Vector2D operator /(Vector2D v, TNum a)
    {
#if DEBUG
      if (Tools.EQ(a))
      {
        throw new DivideByZeroException();
      }
#endif
      return new Vector2D(v.x / a, v.y / a);
    }

    /// <summary>
    /// Dot product
    /// </summary>
    /// <param name="v1">The first vector factor</param>
    /// <param name="v2">The first vector factor</param>
    /// <returns>The product</returns>
    public static TNum operator *(Vector2D v1, Vector2D v2) => v1.x * v2.x + v1.y * v2.y;

    /// <summary>
    /// Pseudoscalar product (z-component of outer product)
    /// </summary>
    /// <param name="v1">The first vector factor</param>
    /// <param name="v2">The first vector factor</param>
    /// <returns>The z-component of the product</returns>
    public static TNum operator ^(Vector2D v1, Vector2D v2) => v1.x * v2.y - v1.y * v2.x;
    #endregion

    /// <summary>
    /// Parallelism of vectors
    /// </summary>
    /// <param name="v1">The first vector</param>
    /// <param name="v2">The second vector</param>
    /// <returns>true, if the vectors are parallel; false, otherwise</returns>
    public static bool AreParallel(Vector2D v1, Vector2D v2) => Tools.EQ(TNum.Abs(v1 * v2), v1.Length * v2.Length);

    /// <summary>
    /// Codirectionality of vectors
    /// </summary>
    /// <param name="v1">The first vector</param>
    /// <param name="v2">The second vector</param>
    /// <returns>true, if the vectors are codirected; false, otherwise</returns>
    public static bool AreCodirected(Vector2D v1, Vector2D v2) => Tools.EQ(v1 * v2, v1.Length * v2.Length);

    /// <summary>
    /// Counterdirectionality of vectors
    /// </summary>
    /// <param name="v1">The first vector</param>
    /// <param name="v2">The second vector</param>
    /// <returns>true, if the vectors are counterdirected; false, otherwise</returns>
    public static bool AreCounterdirected(Vector2D v1, Vector2D v2) => Tools.EQ(v1 * v2, -v1.Length * v2.Length);

    /// <summary>
    /// Orthogonality of vectors
    /// </summary>
    /// <param name="v1">The first vector</param>
    /// <param name="v2">The second vector</param>
    /// <returns>true, if the vectors are orthogonal; false, otherwise</returns>
    public static bool AreOrthogonal(Vector2D v1, Vector2D v2)
      => v1.IsZero || v2.IsZero || Tools.EQ(TNum.Abs(v1 * v2 / v1.Length / v1.Length));

    /// <summary>
    /// Linear combination of two points
    /// </summary>
    /// <param name="p1">The first point</param>
    /// <param name="w1">The weight of the first point</param>
    /// <param name="p2">The second point</param>
    /// <param name="w2">The weight of the second point</param>
    /// <returns>The resultant point</returns>
    public static Vector2D LinearCombination(Vector2D p1, TNum w1, Vector2D p2, TNum w2)
      => new Vector2D(w1 * p1.x + w2 * p2.x, w1 * p1.y + w2 * p2.y);

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
    public static Vector2D LinearCombination(
        Vector2D p1
      , TNum w1
      , Vector2D p2
      , TNum w2
      , Vector2D p3
      , TNum w3
      )
      => new Vector2D(w1 * p1.x + w2 * p2.x + w3 * p3.x, w1 * p1.y + w2 * p2.y + w3 * p3.y);

    /// <summary>
    /// Linear combination of a collection of points
    /// </summary>
    /// <param name="ps">Collection of the points</param>
    /// <param name="ws">Collection of the weights (has at least, the same number of elements as the collection of points)</param>
    /// <returns>The resultant point</returns>
    public static Vector2D LinearCombination(IEnumerable<Vector2D> ps, IEnumerable<TNum> ws)
    {
      using IEnumerator<Vector2D> enPoint = ps.GetEnumerator();
      using IEnumerator<TNum> enWeight = ws.GetEnumerator();
      TNum x = Tools.Zero, y = Tools.Zero;
      while (enPoint.MoveNext() && enWeight.MoveNext())
      {
        x += enPoint.Current.x * enWeight.Current;
        y += enPoint.Current.y * enWeight.Current;
      }

      return new Vector2D(x, y);
    }


    #region Vector constants
    /// <summary>
    /// The zero vector
    /// </summary>
    public static readonly Vector2D Zero = new Vector2D(Tools.Zero, Tools.Zero);

    /// <summary>
    /// The first unit vector
    /// </summary>
    public static readonly Vector2D E1 = new Vector2D(Tools.One, Tools.Zero);

    /// <summary>
    /// The second unit vector
    /// </summary>
    public static readonly Vector2D E2 = new Vector2D(Tools.Zero, Tools.One);
    #endregion

  }

}
