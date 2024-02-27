using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Numerics;

namespace CGLibrary;

public partial class Geometry<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  /// <summary>
  /// Class of multidimensional vector
  /// </summary>
  public class Vector {

#region Internal storage, access properties, and convertors
    /// <summary>
    /// The internal storage of the vector as a one-dimensional array
    /// </summary>
    private readonly TNum[] _v;

    /// <summary>
    /// Dimension of the vector
    /// </summary>
    public int Dim => _v.Length;

    /// <summary>
    /// Indexer access
    /// </summary>
    /// <param name="i">The index of the coordinate</param>
    /// <returns>The value of the corresponding component</returns>
    public TNum this[int i] {
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
    private TNum? length = null;

    /// <summary>
    /// Getter
    /// </summary>
    /// <returns>The length of the vector</returns>
    public TNum Length {
      get
        {
          length ??= TNum.Sqrt(Length2);

          return length.Value;
        }
    }

    /// <summary>
    /// The square of length field
    /// </summary>
    private TNum? length2 = null;

    /// <summary>
    /// Getter
    /// </summary>
    /// <returns>The square of length of the vector</returns>
    public TNum Length2 {
      get
        {
          if (length2 is null) {
            TNum res = Tools.Zero;

            for (int i = 0; i < Dim; i++) {
              res += _v[i] * _v[i];
            }

            length2 = res;
          }

          return length2.Value;
        }
    }

    /// <summary>
    /// Property showing if the vector is zero vector
    /// </summary>
    public bool IsZero => Tools.EQ(Length2);

    /// <summary>
    /// Convert a vector to a one-dimensional array
    /// </summary>
    /// <param name="v">The vector to be converted</param>
    /// <returns>The resultant array</returns>
    public static explicit operator TNum[](Vector v) => v._v;
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
        res = Tools.CMP(this[i], v![i]);

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

      res.length = TNum.MultiplicativeIdentity;

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

      res.length = TNum.MultiplicativeIdentity;

      return res;
    }


    /// <summary>
    /// The angle from the one vector to the another. It is from the interval [-pi, pi).
    /// It uses the Math.Acos to calculate the angle.
    /// </summary>
    /// <param name="v1">The first vector</param>
    /// <param name="v2">The second vector</param>
    /// <returns>The angle</returns>
    public static double AngleDouble(Vector v1, Vector v2) {
      if (v1.IsZero || v2.IsZero) {
        return 0;
      } else {
        TNum dot = (v1 * v2) / v1.Length / v2.Length;
#if DEBUG
        if (!(Tools.GE(dot, -Tools.One) && Tools.LE(dot, Tools.One))) { // !(dot >= -1 && dot <= 1)
          throw new ArgumentException($"Vector.Angle: The dot production of v1 = {v1} and v2 = {v2} is beyond [-1-eps, 1+eps]!");
        }
#endif
        if (Tools.EQ(dot, -Tools.One) && dot <= -Tools.One) {
          return Math.PI;
        }
        if (Tools.EQ(dot, Tools.One) && dot >= Tools.One) {
          return 0.0;
        }

        return Math.Acos(TConv.ToDouble(dot));
      }
    }


    /// <summary>
    /// The angle from the first vector to the another one. It is from the interval [-pi, pi).
    /// It uses the ddouble.Acos to calculate the angle.
    /// </summary>
    /// <param name="v1">The first vector</param>
    /// <param name="v2">The second vector</param>
    /// <returns>The angle.</returns>
    public static TNum Angle(Vector v1, Vector v2) {
      if (v1.IsZero || v2.IsZero) {
        return Tools.Zero;
      } else {
        TNum dot = (v1 * v2) / v1.Length / v2.Length;
#if DEBUG
        if (!(Tools.GE(dot, -Tools.One) && Tools.LE(dot, Tools.One))) { // !(dot >= -1 && dot <= 1)
          throw new ArgumentException($"Vector.Angle: The dot production of v1 = {v1} and v2 = {v2} is beyond [-1-eps, 1+eps]!");
        }
#endif
        if (Tools.EQ(dot, -Tools.One) && dot <= -Tools.One) {
          return TNum.Pi;
        }
        if (Tools.EQ(dot, Tools.One) && dot >= Tools.One) {
          return TNum.Zero;
        }

        return TNum.Acos(dot);
      }
    }

    /// <summary>
    /// Perform the projection to the plane with basis (u1,u2) by the formula:
    /// res = (this * u1) * u1 + (this * u2) * u2
    /// </summary>
    /// <param name="u1">The first basis vector of the plane.</param>
    /// <param name="u2">The second basis vector of the plane.</param>
    /// <returns>The projected vector.</returns>
    public Vector ProjectToPlane(Vector u1, Vector u2) {
#if DEBUG
      if (Dim != u1.Dim && Dim != u2.Dim) {
        throw new ArgumentException("Cannot compute a dot production of two vectors of different dimensions");
      }
#endif

      TNum fst = Tools.Zero;
      TNum snd = Tools.Zero;
      for (int i = 0; i < Dim; i++) {
        fst += _v[i] * u1[i];
        snd += _v[i] * u2[i];
      }

      TNum[] res = new TNum[Dim];
      for (int i = 0; i < Dim; i++) {
        res[i] = fst * u1[i] + snd * u2[i];
      }

      return new Vector(res);
    }

    /// <summary>
    /// Returns the string contains coordinates of a point in the specified format.
    /// x0 x1 ... xDim-1
    /// </summary>
    /// <returns>The string in the specified format.</returns>
    public string ToFileFormat() {
      string res = $"{_v[0].ToString(null, CultureInfo.InvariantCulture)}";
      int    d   = Dim, i;

      for (i = 1; i < d; i++) {
        res += $" {_v[i].ToString(null, CultureInfo.InvariantCulture)}";
      }

      return res;
    }
#endregion

#region Functions related to Vectors
    /// <summary>
    /// Orthonormalizes the given vector against the given basis.
    /// </summary>
    /// <param name="v">The input vector to orthonormalize.</param>
    /// <param name="Basis">The basis to orthonormalize against.</param>
    /// <returns>The resulting orthonormalized vector. If the basis is empty returns normalized vector.</returns>
    public static Vector OrthonormalizeAgainstBasis(Vector v, IEnumerable<Vector> Basis) {
      foreach (Vector bvec in Basis) {
        Debug.Assert(v.Dim == Basis.First().Dim, $"Dimensions are different! Found {v.Dim} expected {Basis.First().Dim}.");

        v -= (bvec * v) * bvec;
      }

      return v.NormalizeZero();
    }

    /// <summary>
    /// Orthonormalizes the given vector against given two bases. Order of bases is important.
    /// </summary>
    /// <param name="v">The input vector to orthonormalize.</param>
    /// <param name="Basis1">The first basis to orthonormalize against.</param>
    /// <param name="Basis2">The second basis to orthonormalize against.</param>
    /// <returns></returns>
    public static Vector OrthonormalizeAgainstBasis(Vector v, IEnumerable<Vector> Basis1, IEnumerable<Vector> Basis2) {
      return OrthonormalizeAgainstBasis(OrthonormalizeAgainstBasis(v, Basis1), Basis2);
    }

    /// <summary>
    /// Orthonormalizes the given collection of vectors against the given basis.
    /// </summary>
    /// <param name="Vs">The input collection of vectors to orthonormalize.</param>
    /// <param name="Basis">The basis to orthonormalize against.</param>
    /// <returns>The resulting collection of orthonormalized vectors. If the basis is empty returns normalized vectors.</returns>
    public static IEnumerable<Vector> OrthonormalizeAgainstBasis(IEnumerable<Vector> Vs, IEnumerable<Vector> Basis) {
      List<Vector> res = new List<Vector>();

      foreach (Vector v in Vs) {
        res.Add(OrthonormalizeAgainstBasis(v, Basis, res));
      }

      return res;
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
      int          dim   = V.First().Dim;
      List<Vector> Basis = BasisInit.ToList();

      foreach (Vector v in V) {
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
      return CompareTo((Vector)obj!) == 0;
    }

    public override string ToString() => ToStringWithDiffBraces('(', ')');


    public override int GetHashCode() {
      int res = 0, d = Dim;

      for (int i = 0; i < d; i++) {
        res = HashCode.Combine(res, TNum.Round(_v[i] / Tools.Eps));
      }

      return res;
    }

    public string ToStringWithDiffBraces(char braceOpen, char braceClose) {
      string res = $"{braceOpen}{_v[0].ToString(null, CultureInfo.InvariantCulture)}";
      int    d   = Dim, i;

      for (i = 1; i < d; i++) {
        res += $",{_v[i].ToString(null, CultureInfo.InvariantCulture)}";
      }

      res += $"{braceClose}";

      return res;
    }

    public string ToStringDouble(char braceOpen = '(', char braceClose = ')') {
      string res = $"{braceOpen}{TConv.ToDouble(_v[0]).ToString(null, CultureInfo.InvariantCulture)}";
      int    d   = Dim, i;

      for (i = 1; i < d; i++) {
        res += $",{TConv.ToDouble(_v[i]).ToString(null, CultureInfo.InvariantCulture)}";
      }

      res += $"{braceClose}";

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
      _v = new TNum[n];

      ComputeParameters();
    }

    /// <summary>
    /// Constructor on the basis of a one-dimensional array
    /// </summary>
    /// <param name="nv">The array</param>
    public Vector(TNum[] nv) {
#if DEBUG
      if (nv.Length <= 0) {
        throw new ArgumentException("Dimension of a vector cannot be non-positive");
      }

      if (nv.Rank != 1) {
        throw new ArgumentException("Cannot initialize a vector by a multidimensional array");
      }
#endif
      _v = new TNum[nv.Length];

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
      _v = new TNum[d];

      for (i = 0; i < d; i++) {
        _v[i] = v._v[i];
      }

      ComputeParameters();
    }

    /// <summary>
    /// Constructor based on point treated as radius-vector from 0.
    /// </summary>
    /// <param name="p">The point on which constructor construct the vector</param>
    public Vector(Point p) : this((TNum[])p) { }

    /// <summary>
    /// Constructor to a multidimensional vector from a two-dimensional vector
    /// </summary>
    /// <param name="v">The vector to be copied</param>
    /// <returns>The resultant vector</returns>
    public Vector(Vector2D v) : this(new TNum[] { v[0], v[1] }) { }

    /// <summary>
    /// Constructor to a multidimensional vector from a two-dimensional point
    /// </summary>
    /// <param name="p">The point to be copied</param>
    /// <returns>The resultant vector</returns>
    public Vector(Point2D p) : this(new TNum[] { p.x, p.y }) { }

    /// <summary>
    /// Computing fields
    /// </summary>
    private void ComputeParameters() { }
#endregion

#region Fabrics
    /// <summary>
    /// Creates the i-orth of given dimension
    /// </summary>
    /// <param name="dim">The dimension of the vector</param>
    /// <param name="pos">The position of '1'</param>
    /// <returns>The i-orth of given dimension</returns>
    public static Vector CreateOrth(int dim, int pos) {
      Debug.Assert(pos > 0, "Position should be greater than 0.");
      TNum[] orth = new TNum[dim];
      orth[pos - 1] = TNum.MultiplicativeIdentity;

      return new Vector(orth);
    }

    /// <summary>
    /// Generates a non-zero random vector of the specified dimension. Each coordinate: [-0.5, 0.5].
    /// </summary>
    /// <param name="dim">The dimension of the vector.</param>
    /// <param name="random">If null then default one be used.</param>
    /// <returns>A random vector.</returns>
    public static Vector GenVector(int dim, GRandomLC? random = null) {
      Vector res;
      do {
        res = GenVector(dim, -Tools.HalfOne, Tools.HalfOne, random);
      } while (res.IsZero);

      return res;
    }

    /// <summary>
    /// Generates an arbitrary vector of the specified dimension. Each coordinate: [a, b].
    /// </summary>
    /// <param name="dim">The dimension of the vector.</param>
    /// <param name="a">The minimum value of each coordinate.</param>
    /// <param name="b">The maximum value of each coordinate.</param>
    /// <param name="random">If null then default one be used.</param>
    /// <returns>A random vector.</returns>
    public static Vector GenVector(int dim, TNum a, TNum b, GRandomLC? random = null) =>
      new Vector(GenArray(dim, a, b, random));
#endregion

#region Operators
    /// <summary>
    /// Unary minus - the opposite vector
    /// </summary>
    /// <param name="v">The vector to be reversed</param>
    /// <returns>The opposite vector</returns>
    public static Vector operator -(Vector v) {
      int    d  = v.Dim, i;
      TNum[] nv = new TNum[d];

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
      TNum[] nv = new TNum[d];

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
      TNum[] nv = new TNum[d];

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
    public static Vector operator *(TNum a, Vector v) {
      int    d  = v.Dim, i;
      TNum[] nv = new TNum[d];

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
    public static Vector operator *(Vector v, TNum a) => a * v;

    /// <summary>
    /// Division of a vector by a number
    /// </summary>
    /// <param name="v">The vector dividend</param>
    /// <param name="a">The numeric divisor</param>
    /// <returns>The product</returns>
    public static Vector operator /(Vector v, TNum a) {
#if DEBUG
      if (Tools.EQ(a)) {
        throw new DivideByZeroException();
      }
#endif
      int    d  = v.Dim, i;
      TNum[] nv = new TNum[d];

      for (i = 0; i < d; i++) {
        nv[i] = v._v[i] / a;
      }

      return new Vector(nv);
    }

    /// <summary>
    /// Dot product.
    /// </summary>
    /// <param name="v1">The first vector factor.</param>
    /// <param name="v2">The second vector factor.</param>
    /// <returns>The product</returns>
    public static TNum operator *(Vector v1, Vector v2) {
      int d = v1.Dim, i;
#if DEBUG
      if (d != v2.Dim) {
        throw new ArgumentException("Cannot compute a dot production of two vectors of different dimensions");
      }
#endif
      TNum res = Tools.Zero;

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
      TNum l1 = v1.Length, l2 = v2.Length;

      return Tools.EQ(TNum.Abs(v1 * v2), l1 * l2);
    }

    /// <summary>
    /// Codirectionality of vectors
    /// </summary>
    /// <param name="v1">The first vector</param>
    /// <param name="v2">The second vector</param>
    /// <returns>true, if the vectors are codirected; false, otherwise</returns>
    public static bool AreCodirected(Vector v1, Vector v2) {
      TNum l1 = v1.Length, l2 = v2.Length;

      return Tools.EQ(v1 * v2, l1 * l2);
    }

    /// <summary>
    /// Counterdirectionality of vectors
    /// </summary>
    /// <param name="v1">The first vector</param>
    /// <param name="v2">The second vector</param>
    /// <returns>true, if the vectors are counterdirected; false, otherwise</returns>
    public static bool AreCounterdirected(Vector v1, Vector v2) {
      TNum l1 = v1.Length, l2 = v2.Length;

      return Tools.EQ(v1 * v2, -l1 * l2);
    }

    /// <summary>
    /// Orthogonality of vectors
    /// </summary>
    /// <param name="v1">The first vector</param>
    /// <param name="v2">The second vector</param>
    /// <returns>true, if the vectors are orthognal; false, otherwise</returns>
    public static bool AreOrthogonal(Vector v1, Vector v2) {
      TNum l1 = v1.Length, l2 = v2.Length;

      return Tools.EQ(l1) || Tools.EQ(l2) || Tools.EQ(TNum.Abs(v1 * v2 / (l1 * l2)));
    }
#endregion

  }

}
