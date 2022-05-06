﻿using System;

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
      int xRes = Tools.CMP(x, v.x);
      if (xRes != 0) {
        return xRes;
      } else {
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
    public double this[int i] =>
      i switch {
        0 => x
        , 1 => y
        , _ => throw new IndexOutOfRangeException()
      };

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
      if (Tools.EQ(Length)) {
        throw new DivideByZeroException();
      }
#endif
      return new Vector2D(x / Length, y / Length);
    }

    /// <summary>
    /// Angle from the one vector to another from the interval [-pi, pi) 
    /// (counted counterclockwise if positive, or clockwise if negative)
    /// </summary>
    /// <param name="v1">The first vector</param>
    /// <param name="v2">The second vector</param>
    /// <returns>The angle; the angle between a zero vector and any other equals zero</returns>
    public static double Angle(Vector2D v1, Vector2D v2) {
      if (Tools.EQ(v1.Length) || Tools.EQ(v2.Length)) {
        return 0;
      } else {
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
      if (a < 0) {
        return a + 2 * Math.PI;
      } else {
        return a;
      }
    }
    #endregion

    #region Convertors
    /// <summary>
    /// Explicit convertor to a two-dimensional vector from a two-dimensional point
    /// </summary>
    /// <param name="p">The point to be converted</param>
    /// <returns>The vector, which is the endpoint of the given vector</returns>
    public static explicit operator Vector2D(Point2D p) => new Vector2D(p.x, p.y);

    /// <summary>
    /// Explicit convertor to a two-dimensional vector from a multidimensional vector of general kind
    /// </summary>
    /// <param name="v">The vector to be converted</param>
    /// <returns>The resultant vector</returns>
    public static explicit operator Vector2D(Vector v) {
#if DEBUG
      if (v.Dim != 2) {
        throw new ArgumentException("A multidimensional point is tried to be converted to a two-dimensional point!");
      }
#endif
      return new Vector2D(v[0], v[1]);
    }

    /// <summary>
    /// Explicit convertor to a two-dimensional vector from a multidimensional point of general kind
    /// </summary>
    /// <param name="p">The point to be converted</param>
    /// <returns>The resultant point</returns>
    public static explicit operator Vector2D(Point p) {
#if DEBUG
      if (p.Dim != 2) {
        throw new ArgumentException("A multidimensional vector is tried to be converted to a two-dimensional point!");
      }
#endif
      return new Vector2D(p[0], p[1]);
    }
    #endregion
    
    #region Overrides
    public override bool Equals(object obj) {
#if DEBUG
      if (!(obj is Vector2D v)) {
        throw new ArgumentException();
      }
#endif
      return CompareTo(v) == 0;
    }

    public override string ToString() => "(" + x + ";" + y + ")";

    public override int GetHashCode() => x.GetHashCode() + y.GetHashCode();
    #endregion

    #region Constructors
    /// <summary>
    /// The default construct producing the zero vector
    /// </summary>
    public Vector2D() {
      x = 0;
      y = 0;

      ComputeParameters();
    }

    /// <summary>
    /// Coordinate constructor
    /// </summary>
    /// <param name="nx">The new abscissa</param>
    /// <param name="ny">The new ordinate</param>
    public Vector2D(double nx, double ny) {
      x = nx;
      y = ny;

      ComputeParameters();
    }

    /// <summary>
    /// Copying constructor
    /// </summary>
    /// <param name="v">The vector to be copied</param>
    public Vector2D(Vector2D v) {
      x = v.x;
      y = v.y;

      ComputeParameters();
    }

    /// <summary>
    /// Computing parameters of the vector for future usage
    /// </summary>
    private void ComputeParameters() {
      Length = Math.Sqrt(x * x + y * y);
      PolarAngle = Math.Atan2(y, x);
    }
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
    public static Vector2D operator *(double a, Vector2D v) => new Vector2D(a * v.x, a * v.y);

    /// <summary>
    /// Right multiplication of a vector by a number
    /// </summary>
    /// <param name="v">The vector factor</param>
    /// <param name="a">The numeric factor</param>
    /// <returns>The product</returns>
    public static Vector2D operator *(Vector2D v, double a) => new Vector2D(a * v.x, a * v.y);

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
      return new Vector2D(v.x / a, v.y / a);
    }

    /// <summary>
    /// Dot product 
    /// </summary>
    /// <param name="v1">The first vector factor</param>
    /// <param name="v2">The first vector factor</param>
    /// <returns>The product</returns>
    public static double operator *(Vector2D v1, Vector2D v2) => v1.x * v2.x + v1.y * v2.y;

    /// <summary>
    /// Pseudoscalar product (z-component of outer product) 
    /// </summary>
    /// <param name="v1">The first vector factor</param>
    /// <param name="v2">The first vector factor</param>
    /// <returns>The z-component of the product</returns>
    public static double operator ^(Vector2D v1, Vector2D v2) => v1.x * v2.y - v1.y * v2.x;

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
