using System;

namespace PolygonLibrary.Basics{
  /// <summary>
  /// Class of vectors in the plane
  /// </summary>
  public class Vector2D : IComparable<Vector2D>{
    #region Comparing
    /// <summary>
    /// Vector comparer realizing the lexicographic order
    /// </summary>
    /// <param name="v">The vector to be compared with</param>
    /// <returns>+1, if this object greater than v; 0, if they are equal; -1, otherwise</returns>
    public int CompareTo(Vector2D v) {
      int xRes = Tools.CMP(X, v.X);
      if (xRes != 0) {
        return xRes;
      } else {
        return Tools.CMP(Y, v.Y);
      }
    }

    /// <summary>
    /// Equality of vectors
    /// </summary>
    /// <param name="v1">The first vector</param>
    /// <param name="v2">The second vector</param>
    /// <returns>true, if the vectors coincide; false, otherwise</returns>
    public static bool operator ==(Vector2D v1, Vector2D v2) => Tools.EQ(v1.X, v2.X) && Tools.EQ(v1.Y, v2.Y);

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
    /// <returns>true, if v1 > v2; false, otherwise</returns>
    public static bool operator >(Vector2D v1, Vector2D v2) => v1.CompareTo(v2) > 0;

    /// <summary>
    /// Check whether one vector is greater or equal than another (in lexicographic order)
    /// </summary>
    /// <param name="v1">The first vector</param>
    /// <param name="v2">The second vector</param>
    /// <returns>true, if v1 >= v2; false, otherwise</returns>
    public static bool operator >=(Vector2D v1, Vector2D v2) => v1.CompareTo(v2) >= 0;

    /// <summary>
    /// Check whether one vector is less than another (in lexicographic order)
    /// </summary>
    /// <param name="v1">The first vector</param>
    /// <param name="v2">The second vector</param>
    /// <returns>true, if v1 is less than v2; false, otherwise</returns>
    public static bool operator <(Vector2D v1, Vector2D v2) => v1.CompareTo(v2) < 0;

    /// <summary>
    /// Check whether one vector is less or equal than another (in lexicographic order)
    /// </summary>
    /// <param name="v1">The first vector</param>
    /// <param name="v2">The second vector</param>
    /// <returns>true, if v1 is less than or equal to v2; false, otherwise</returns>
    public static bool operator <=(Vector2D v1, Vector2D v2) => v1.CompareTo(v2) <= 0;
    #endregion

    #region Access properties
    /// <summary>
    /// The abscissa
    /// </summary>
    public double X { get; }

    /// <summary>
    /// The ordinate
    /// </summary>
    public double Y { get; }

    /// <summary>
    /// Indexer access
    /// </summary>
    /// <param name="i">The index: 0 - the abscissa, 1 - the ordinate</param>
    /// <returns>The value of the corresponding component</returns>
    public double this[int i] {
      get {
        if (i == 0) {
          return X;
        }  else if (i == 1) {
          return Y;
        } else {
          throw new IndexOutOfRangeException();
        }
      }
    }

    /// <summary>
    /// length of the vector
    /// </summary>
    public double Length { get; private set; }

    /// <summary>
    /// The polar angle of the vector
    /// </summary>
    public double PolarAngle { get; private set; }
    #endregion

    #region Miscellaneous procedures
    /// <summary>
    /// Normalization of the vector
    /// </summary>
    public Vector2D Normalize() {
#if DEBUG
      if (Tools.EQ(Length))
        throw new DivideByZeroException();
#endif
      return new Vector2D(X / Length, Y / Length);
    }

    /// <summary>
    /// Angle from the one vector to another from the interval [-pi, pi) 
    /// (counted counterclockwise if positive, or clockwise if negative)
    /// </summary>
    /// <param name="v1">The first vector</param>
    /// <param name="v2">The second vector</param>
    /// <returns>The angle; the angle between a zero vector and any other equals zero</returns>
    public static double Angle(Vector2D v1, Vector2D v2) {
      if (Tools.EQ(v1.Length) || Tools.EQ(v2.Length))
        return 0;
      else {
        double
          l = v1.Length * v2.Length, s = (v1 ^ v2) / l, c = (v1 * v2) / l;
        return Math.Atan2(s, c);
      }
    }

    /// <summary>
    /// Angle from the one vector to another from the interval [0, 2\pi) (counted counterclockwise)
    /// </summary>
    /// <param name="v1">The first vector</param>
    /// <param name="v2">The second vector</param>
    /// <returns>The angle; the angle between a zero vector and any other equals zero</returns>
    public static double Angle2PI(Vector2D v1, Vector2D v2) {
      double a = Angle(v1, v2);
      if (a < 0)
        return a + 2 * Math.PI;
      else
        return a;
    }
    #endregion

    #region Overrides
    public override bool Equals(object obj) {
      Vector2D v = obj as Vector2D;
#if DEBUG
      if (v is null) {
        throw new ArgumentException();
      }
#endif
      return CompareTo(v) == 0;
    }

    public override string ToString() => "(" + X + ";" + Y + ")";

    public override int GetHashCode() => X.GetHashCode() + Y.GetHashCode();
    #endregion

    #region Constructors
    /// <summary>
    /// The default construct producing the zero vector
    /// </summary>
    public Vector2D() {
      X = 0;
      Y = 0;

      ComputeParameters();
    }

    /// <summary>
    /// Coordinate constructor
    /// </summary>
    /// <param name="nx">The new abscissa</param>
    /// <param name="ny">The new ordinate</param>
    public Vector2D(double nx, double ny) {
      X = nx;
      Y = ny;

      ComputeParameters();
    }

    /// <summary>
    /// Copying constructor
    /// </summary>
    /// <param name="v">The vector to be copied</param>
    public Vector2D(Vector2D v) {
      X = v.X;
      Y = v.Y;

      ComputeParameters();
    }

    /// <summary>
    /// Computing parameters of the vector for future usage
    /// </summary>
    private void ComputeParameters() {
      Length = Math.Sqrt(X * X + Y * Y);
      PolarAngle = Math.Atan2(Y, X);
    }
    #endregion

    #region Operators
    /// <summary>
    /// Unary minus - the opposite vector
    /// </summary>
    /// <param name="v">The vector to be reversed</param>
    /// <returns>The opposite vector</returns>
    public static Vector2D operator -(Vector2D v) => new Vector2D(-v.X, -v.Y);

    /// <summary>
    /// Sum of two vectors
    /// </summary>
    /// <param name="v1">The first vector summand</param>
    /// <param name="v2">The second vector summand</param>
    /// <returns>The sum</returns>
    public static Vector2D operator +(Vector2D v1, Vector2D v2) => new Vector2D(v1.X + v2.X, v1.Y + v2.Y);

    /// <summary>
    /// Difference of two vectors
    /// </summary>
    /// <param name="v1">The vector minuend</param>
    /// <param name="v2">The vector subtrahend</param>
    /// <returns>The difference</returns>
    public static Vector2D operator -(Vector2D v1, Vector2D v2) => new Vector2D(v1.X - v2.X, v1.Y - v2.Y);

    /// <summary>
    /// Left multiplication of a vector by a number
    /// </summary>
    /// <param name="a">The numeric factor</param>
    /// <param name="v">The vector factor</param>
    /// <returns>The product</returns>
    public static Vector2D operator *(double a, Vector2D v) => new Vector2D(a * v.X, a * v.Y);

    /// <summary>
    /// Right multiplication of a vector by a number
    /// </summary>
    /// <param name="v">The vector factor</param>
    /// <param name="a">The numeric factor</param>
    /// <returns>The product</returns>
    public static Vector2D operator *(Vector2D v, double a) => new Vector2D(a * v.X, a * v.Y);

    /// <summary>
    /// Division of a vector by a number
    /// </summary>
    /// <param name="v">The vector dividend</param>
    /// <param name="a">The numeric divisor</param>
    /// <returns>The product</returns>
    public static Vector2D operator /(Vector2D v, double a) {
#if DEBUG
      if (Tools.EQ(a)) {
        throw new DivideByZeroException();
      }
#endif
      return new Vector2D(v.X / a, v.Y / a);
    }

    /// <summary>
    /// Dot product 
    /// </summary>
    /// <param name="v1">The first vector factor</param>
    /// <param name="v2">The first vector factor</param>
    /// <returns>The product</returns>
    public static double operator *(Vector2D v1, Vector2D v2) => v1.X * v2.X + v1.Y * v2.Y;

    /// <summary>
    /// Pseudoscalar product (z-component of outer product) 
    /// </summary>
    /// <param name="v1">The first vector factor</param>
    /// <param name="v2">The first vector factor</param>
    /// <returns>The z-component of the product</returns>
    public static double operator ^(Vector2D v1, Vector2D v2) => v1.X * v2.Y - v1.Y * v2.X;

    /// <summary>
    /// Parallelism of vectors
    /// </summary>
    /// <param name="v1">The first vector</param>
    /// <param name="v2">The second vector</param>
    /// <returns>true, if the vectors are parallel; false, otherwise</returns>
    public static bool AreParallel(Vector2D v1, Vector2D v2) => Tools.EQ(Math.Abs(v1 * v2), v1.Length * v2.Length);

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
    public static bool AreOrthogonal(Vector2D v1, Vector2D v2) =>
      Tools.EQ(v1.Length) || Tools.EQ(v2.Length) ||
      Tools.EQ(Math.Abs(v1 * v2 / v1.Length / v1.Length));
    #endregion

    #region Vector constants
    /// <summary>
    /// The zero vector
    /// </summary>
    public static readonly Vector2D Zero = new Vector2D(0, 0);

    /// <summary>
    /// The first unit vector
    /// </summary>
    public static readonly Vector2D E1 = new Vector2D(1, 0);

    /// <summary>
    /// The second unit vector
    /// </summary>
    public static readonly Vector2D E2 = new Vector2D(0, 1);
    #endregion
  }
}
