using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using PolygonLibrary.Toolkit;

namespace PolygonLibrary.Basics;

/// <summary>
/// Class of multidimensional vector
/// </summary>
public class Vector : IComparable<Vector> {

#region Internal storage, access properties, and convertors
  /// <summary>
  /// The internal storage of the vector as a one-dimensional array
  /// </summary>
  private readonly double[] _v;

  /// <summary>
  /// Dimension of the vector
  /// </summary>
  public int Dim => _v.Length;

  /// <summary>
  /// Indexer access
  /// </summary>
  /// <param name="i">The index: 0 - the abscissa, 1 - the ordinate</param>
  /// <returns>The value of the corresponding component</returns>
  public double this[int i] {
    get
      {
#if DEBUG
        if (i < 0 || i >= Dim) {
          throw new IndexOutOfRangeException();
        }
#endif
        return _v[i];
      }
  }

  /// <summary>
  /// The length field
  /// </summary>
  private double? length = null;

  /// <summary>
  /// Getter
  /// </summary>
  /// <returns>The length of the vector</returns>
  public double Length {
    get
      {
        if (length is null) {
          double res = 0;

          for (int i = 0; i < Dim; i++) {
            res += _v[i] * _v[i];
          }

          length = Math.Sqrt(res);
        }

        return length.Value;
      }
  }

  /// <summary>
  /// Property showing if the vector is zero vector
  /// </summary>
  public bool IsZero => Tools.EQ(Length);

  /// <summary>
  /// Convert a vector to a one-dimensional array
  /// </summary>
  /// <param name="v">The vector to be converted</param>
  /// <returns>The resultant array</returns>
  public static implicit operator double[](Vector v) => v._v;

  /// <summary>
  /// Converting a one-dimensional array to a vector
  /// </summary>
  /// <param name="v">Array to be converted</param>
  /// <returns>The resultant vector</returns>
  public static explicit operator Vector(double[] v) => new Vector(v);
  
  public Point ToPoint() => new Point(_v);
#endregion

#region Comparing
  /// <summary>
  /// Vector comparer realizing the lexicographic order
  /// </summary>
  /// <param name="v">The vector to be compared with</param>
  /// <returns>+1, if this object greater than v; 0, if they are equal; -1, otherwise</returns>
  public int CompareTo(Vector? v) {
    int d = Dim, res;
#if DEBUG
    Debug.Assert(v is not null, nameof(v) + " != null");

    if (d != v.Dim) {
      throw new ArgumentException("Cannot compare vectors of different dimensions");
    }
#endif
    for (int i = 0; i < d; i++) {
      res = Tools.CMP(this[i], v[i]);

      if (res != 0) {
        return res;
      }
    }

    return 0;
  }

  /// <summary>
  /// Equality of vectors
  /// </summary>
  /// <param name="v1">The first vector</param>
  /// <param name="v2">The second vector</param>
  /// <returns>true, if the vectors coincide; false, otherwise</returns>
  public static bool operator ==(Vector v1, Vector v2) {
    int d = v1.Dim, res;
#if DEBUG
    if (d != v2.Dim) {
      throw new ArgumentException("Cannot compare vectors of different dimensions");
    }
#endif
    for (int i = 0; i < d; i++) {
      res = Tools.CMP(v1[i], v2[i]);

      if (res != 0) {
        return false;
      }
    }

    return true;
  }

  /// <summary>
  /// Non-equality of vectors
  /// </summary>
  /// <param name="v1">The first vector</param>
  /// <param name="v2">The second vector</param>
  /// <returns>true, if the vectors do not coincide; false, otherwise</returns>
  public static bool operator !=(Vector v1, Vector v2) {
    int d = v1.Dim, res;
#if DEBUG
    if (d != v2.Dim) {
      throw new ArgumentException("Cannot compare vectors of different dimensions");
    }
#endif
    for (int i = 0; i < d; i++) {
      res = Tools.CMP(v1[i], v2[i]);

      if (res != 0) {
        return true;
      }
    }

    return false;
  }

  /// <summary>
  /// Check whether one vector is greater than another (in lexicographic order)
  /// </summary>
  /// <param name="v1">The first vector</param>
  /// <param name="v2">The second vector</param>
  /// <returns>true, if p1 > p2; false, otherwise</returns>
  public static bool operator >(Vector v1, Vector v2) => v1.CompareTo(v2) > 0;

  /// <summary>
  /// Check whether one vector is greater or equal than another (in lexicographic order)
  /// </summary>
  /// <param name="v1">The first vector</param>
  /// <param name="v2">The second vector</param>
  /// <returns>true, if p1 >= p2; false, otherwise</returns>
  public static bool operator >=(Vector v1, Vector v2) => v1.CompareTo(v2) >= 0;

  /// <summary>
  /// Check whether one vector is less than another (in lexicographic order)
  /// </summary>
  /// <param name="v1">The first vector</param>
  /// <param name="v2">The second vector</param>
  /// <returns>true, if p1 is less than p2; false, otherwise</returns>
  public static bool operator <(Vector v1, Vector v2) => v1.CompareTo(v2) < 0;

  /// <summary>
  /// Check whether one vector is less or equal than another (in lexicographic order)
  /// </summary>
  /// <param name="v1">The first vector</param>
  /// <param name="v2">The second vector</param>
  /// <returns>true, if p1 is less than or equal to p2; false, otherwise</returns>
  public static bool operator <=(Vector v1, Vector v2) => v1.CompareTo(v2) <= 0;
#endregion

#region Miscellaneous procedures
  /// <summary>
  /// Normalization of the vector
  /// </summary>
  /// <returns>
  /// The normalized vector.
  /// </returns>
  /// <exception cref="DivideByZeroException">
  /// Is thrown if the vector is zero 
  /// </exception>
  public Vector Normalize() {
#if DEBUG
    if (Tools.EQ(Length)) {
      throw new DivideByZeroException();
    }
#endif
    Vector res = new Vector(Dim);

    for (int i = 0; i < Dim; i++) {
      res._v[i] = _v[i] / Length;
    }

    res.length = 1;

    return res;
  }

  /// <summary>
  /// Normalization of the vector with the zero vector check
  /// </summary>
  /// <returns>
  /// The normalized vector. If the vector is zero, then zero is returned
  /// </returns>
  public Vector NormalizeZero() {
    if (Tools.EQ(Length)) {
      return this;
    }

    Vector res = new Vector(Dim);

    for (int i = 0; i < Dim; i++) {
      res._v[i] = _v[i] / Length;
    }

    res.length = 1;

    return res;
  }

  /// <summary>
  /// Angle from the one vector to another from the interval [-pi, pi) 
  /// </summary>
  /// <param name="v1">The first vector</param>
  /// <param name="v2">The second vector</param>
  /// <returns>The angle; the angle between a zero vector and any other equals zero</returns>
  public static double Angle(Vector v1, Vector v2) {
    if (Tools.EQ(v1.Length) || Tools.EQ(v2.Length)) {
      return 0;
    } else {
      return Math.Acos((v1 * v2) / v1.Length / v2.Length);
    }
  }
#endregion

#region Functions related to Vectors
  /// <summary>
  /// Orthonormalizes the given vector against the given basis and returns the result.
  /// </summary>
  /// <param name="v">The input vector to orthonormalize.</param>
  /// <param name="Basis">The basis to orthonormalize against.</param>
  /// <returns>The resulting orthonormalized vector. If the basis is empty returns normalized vector</returns>
  public static Vector OrthonormalizeAgainstBasis(Vector v, IEnumerable<Vector> Basis) {
    foreach (Vector bvec in Basis) {
      v -= (bvec * v) * bvec;
    }

    return v.NormalizeZero();
  }

  /// <summary>
  /// Computes an orthonormal system from a given set of vectors using the Gram-Schmidt algorithm.
  /// </summary>
  /// <param name="V">An collection of vectors to use in the orthonormalizing process.</param>
  /// <returns>An orthonormal system of less or equal dimension than the input vectors.</returns>
  public static List<Vector> GramSchmidt(IEnumerable<Vector> V) {
#if DEBUG
    if (!V.Any()) {
      throw new ArgumentException($"Set of vectors {V} must have at least one element!");
    }

    if (V.First().IsZero) {
      throw new ArgumentException($"The first vector from {V} can't be Zero!");
    }
#endif
    return GramSchmidtMain(new[] { V.First().Normalize() }, V);
  }

  /// <summary>
  /// Computes an orthonormal system for an union of orthonormal system and set of vectors using the Gram-Schmidt algorithm.
  /// </summary>
  /// <param name="Orthonormal">The collection of orthonormal vectors.</param>
  /// <param name="V">The collection of vectors.</param>
  /// <returns>A list of orthonormal vectors.</returns>
  public static List<Vector> GramSchmidt(IEnumerable<Vector> Orthonormal, IEnumerable<Vector> V) {
    return GramSchmidtMain(Orthonormal.ToList(), V);
  }

  /// <summary>
  /// Computes an orthonormal basis from a given set of vectors using the Gram-Schmidt algorithm.
  /// </summary>
  /// <param name="BasisInit">A collection of vectors forming the initial basis.</param>
  /// <param name="V">An enumerable collection of vectors to use in the orthonormalizing process.</param>
  /// <returns>An orthonormal basis of the same dimension less or equal than the input vectors.</returns>
  private static List<Vector> GramSchmidtMain(IEnumerable<Vector> BasisInit, IEnumerable<Vector> V) {
    int dim   = V.First().Dim;
    var Basis = BasisInit.ToList();

    foreach (Vector v in V) {
      Debug.Assert(v.Dim == dim, $"Dimensions are different! Found {v.Dim} expected {dim}.");

      Vector conceivable = OrthonormalizeAgainstBasis(v, Basis);

      if (!conceivable.IsZero) {
        Basis.Add(conceivable);
      }

      if (Basis.Count == dim) { //We found Basis
        break;
      }
    }

    return Basis;
  }
#endregion

#region Overrides
  public override bool Equals(object? obj) {
#if DEBUG
    if (obj is not Vector vector) {
      throw new ArgumentException($"{obj} is not a Vector!");
    }
#endif
    return CompareTo(vector) == 0;
  }

  public override string ToString() {
    string res = "(" + _v[0];
    int    d   = Dim, i;

    for (i = 1; i < d; i++) {
      res += ";" + _v[i];
    }

    res += ")";

    return res;
  }

  public override int GetHashCode() {
    int res = 0, d = Dim, i;

    for (i = 0; i < d; i++) {
      res += _v[i].GetHashCode();
    }

    return res;
  }
#endregion

#region Constructors
  /// <summary>
  /// The default construct producing the zero vector
  /// </summary>
  /// <param name="n">The dimension of the vector</param>
  public Vector(int n) {
#if DEBUG
    if (n <= 0) {
      throw new ArgumentException("Dimension of a vector cannot be non-positive");
    }
#endif
    _v = new double[n];

    ComputeParameters();
  }

  /// <summary>
  /// Constructor on the basis of a one-dimensional array
  /// </summary>
  /// <param name="nv">The array</param>
  public Vector(double[] nv) {
#if DEBUG
    if (nv.Length <= 0) {
      throw new ArgumentException("Dimension of a vector cannot be non-positive");
    }

    if (nv.Rank != 1) {
      throw new ArgumentException("Cannot initialize a vector by a multidimensional array");
    }
#endif
    _v = new double[nv.Length];

    for (int i = 0; i < nv.Length; i++) {
      _v[i] = nv[i];
    }

    ComputeParameters();
  }

  /// <summary>
  /// Copying constructor
  /// </summary>
  /// <param name="v">The vector to be copied</param>
  public Vector(Vector v) {
    int d = v.Dim, i;
    _v = new double[d];

    for (i = 0; i < d; i++) {
      _v[i] = v._v[i];
    }

    ComputeParameters();
  }

  /// <summary>
  /// Computing fields
  /// </summary>
  private void ComputeParameters() { }
#endregion

#region Operators
  /// <summary>
  /// Unary minus - the opposite vector
  /// </summary>
  /// <param name="v">The vector to be reversed</param>
  /// <returns>The opposite vector</returns>
  public static Vector operator -(Vector v) {
    int      d  = v.Dim, i;
    double[] nv = new double[d];

    for (i = 0; i < d; i++) {
      nv[i] = -v._v[i];
    }

    return new Vector(nv);
  }

  /// <summary>
  /// Sum of two vectors
  /// </summary>
  /// <param name="v1">The first vector summand</param>
  /// <param name="v2">The second vector summand</param>
  /// <returns>The sum</returns>
  public static Vector operator +(Vector v1, Vector v2) {
    int d = v1.Dim, i;
#if DEBUG
    if (d != v2.Dim) {
      throw new ArgumentException("Cannot add two vectors of different dimensions");
    }
#endif
    double[] nv = new double[d];

    for (i = 0; i < d; i++) {
      nv[i] = v1._v[i] + v2._v[i];
    }

    return new Vector(nv);
  }

  /// <summary>
  /// Difference of two vectors
  /// </summary>
  /// <param name="v1">The vector minuend</param>
  /// <param name="v2">The vector subtrahend</param>
  /// <returns>The difference</returns>
  public static Vector operator -(Vector v1, Vector v2) {
    int d = v1.Dim, i;
#if DEBUG
    if (d != v2.Dim) {
      throw new ArgumentException("Cannot subtract two vectors of different dimensions");
    }
#endif
    double[] nv = new double[d];

    for (i = 0; i < d; i++) {
      nv[i] = v1._v[i] - v2._v[i];
    }

    return new Vector(nv);
  }

  /// <summary>
  /// Left multiplication of a vector by a number
  /// </summary>
  /// <param name="a">The numeric factor</param>
  /// <param name="v">The vector factor</param>
  /// <returns>The product</returns>
  public static Vector operator *(double a, Vector v) {
    int      d  = v.Dim, i;
    double[] nv = new double[d];

    for (i = 0; i < d; i++) {
      nv[i] = a * v._v[i];
    }

    return new Vector(nv);
  }

  /// <summary>
  /// Right multiplication of a vector by a number
  /// </summary>
  /// <param name="v">The vector factor</param>
  /// <param name="a">The numeric factor</param>
  /// <returns>The product</returns>
  public static Vector operator *(Vector v, double a) => a * v;

  /// <summary>
  /// Division of a vector by a number
  /// </summary>
  /// <param name="v">The vector dividend</param>
  /// <param name="a">The numeric divisor</param>
  /// <returns>The product</returns>
  public static Vector operator /(Vector v, double a) {
#if DEBUG
    if (Tools.EQ(a)) {
      throw new DivideByZeroException();
    }
#endif
    int      d  = v.Dim, i;
    double[] nv = new double[d];

    for (i = 0; i < d; i++) {
      nv[i] = v._v[i] / a;
    }

    return new Vector(nv);
  }

  /// <summary>
  /// Dot product 
  /// </summary>
  /// <param name="v1">The first vector factor</param>
  /// <param name="v2">The first vector factor</param>
  /// <returns>The product</returns>
  public static double operator *(Vector v1, Vector v2) {
    int d = v1.Dim, i;
#if DEBUG
    if (d != v2.Dim) {
      throw new ArgumentException("Cannot compute a dot production of two vectors of different dimensions");
    }
#endif
    double res = 0;

    for (i = 0; i < d; i++) {
      res += v1._v[i] * v2._v[i];
    }

    return res;
  }

  /// <summary>
  /// Parallelity of vectors
  /// </summary>
  /// <param name="v1">The first vector</param>
  /// <param name="v2">The second vector</param>
  /// <returns>true, if the vectors are parallel; false, otherwise</returns>
  public static bool AreParallel(Vector v1, Vector v2) {
    double l1 = v1.Length, l2 = v2.Length;

    return Tools.EQ(Math.Abs(v1 * v2), l1 * l2);
  }

  /// <summary>
  /// Codirectionality of vectors
  /// </summary>
  /// <param name="v1">The first vector</param>
  /// <param name="v2">The second vector</param>
  /// <returns>true, if the vectors are codirected; false, otherwise</returns>
  public static bool AreCodirected(Vector v1, Vector v2) {
    double l1 = v1.Length, l2 = v2.Length;

    return Tools.EQ(v1 * v2, l1 * l2);
  }

  /// <summary>
  /// Counterdirectionality of vectors
  /// </summary>
  /// <param name="v1">The first vector</param>
  /// <param name="v2">The second vector</param>
  /// <returns>true, if the vectors are counterdirected; false, otherwise</returns>
  public static bool AreCounterdirected(Vector v1, Vector v2) {
    double l1 = v1.Length, l2 = v2.Length;

    return Tools.EQ(v1 * v2, -l1 * l2);
  }

  /// <summary>
  /// Orthogonality of vectors
  /// </summary>
  /// <param name="v1">The first vector</param>
  /// <param name="v2">The second vector</param>
  /// <returns>true, if the vectors are orthognal; false, otherwise</returns>
  public static bool AreOrthogonal(Vector v1, Vector v2) {
    double l1 = v1.Length, l2 = v2.Length;

    return Tools.EQ(l1) || Tools.EQ(l2) || Tools.EQ(Math.Abs(v1 * v2 / (l1 * l2)));
  }
#endregion

}
