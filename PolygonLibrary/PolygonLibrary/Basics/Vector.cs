using System.Globalization;

namespace CGLibrary;

public partial class Geometry<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  /// <summary>
  /// Class of multidimensional vector
  /// </summary>
  public class Vector : IComparable<Vector> {

#region Internal storage, access properties, and convertors
    /// <summary>
    /// The internal storage of the vector as a one-dimensional array
    /// </summary>
    private readonly TNum[] _v;

    /// <summary>
    /// Gets the coordinates of the vector as array.
    /// </summary>
    /// <returns>The array of vector coordinates.</returns>
    public TNum[] GetAsArray() {
      TNum[] v = new TNum[Dim];
      _v.CopyTo(v, 0); 
      return v;
    }

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
    /// The vector length field
    /// </summary>
    private TNum? _length = null;

    /// <summary>
    /// Getter for vector length
    /// </summary>
    /// <returns>The length of the vector</returns>
    public TNum Length {
      get
        {
          _length ??= TNum.Sqrt(Length2);

          return _length.Value;
        }
    }

    /// <summary>
    /// The square of vector length field
    /// </summary>
    private TNum? length2 = null;

    /// <summary>
    /// Getter for the squared length of the vector
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
    public bool IsZero => Tools.EQ(Length);

    /// <summary>
    /// Convert a vector to a one-dimensional array
    /// </summary>
    /// <param name="v">The vector to be converted</param>
    /// <returns>The resultant array</returns>
    public static explicit operator TNum[](Vector v) => v.GetAsArray();
#endregion

#region Comparing
    /// <summary>
    /// Vector comparer realizing the lexicographic order
    /// </summary>
    /// <param name="other">The vector to be compared with</param>
    /// <returns>+1, if this object greater than other; 0, if they are equal; -1, otherwise</returns>
    public int CompareTo(Vector? other) {
      int d = Dim, res;
#if DEBUG
      Debug.Assert(other is not null, $"Vector.CompareTo: second vector is null!");

      if (d != other.Dim) {
        throw new ArgumentException($"Vector.CompareTo: Cannot compare vectors of different dimensions. This = {this} and other = {other}");
      }
#endif
      for (int i = 0; i < d; i++) {
        res = Tools.CMP(this[i], other![i]);

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
    public static bool operator ==(Vector v1, Vector v2) => v1.CompareTo(v2) == 0;

    /// <summary>
    /// Non-equality of vectors
    /// </summary>
    /// <param name="v1">The first vector</param>
    /// <param name="v2">The second vector</param>
    /// <returns>true, if the vectors do not coincide; false, otherwise</returns>
    public static bool operator !=(Vector v1, Vector v2) => v1.CompareTo(v2) != 0;

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
      if (IsZero) {
        throw new DivideByZeroException();
      }
#endif
      Vector res = new Vector(Dim);

      for (int i = 0; i < Dim; i++) {
        res._v[i] = _v[i] / Length;
      }

      res._length = TNum.MultiplicativeIdentity;

      return res;
    }

    /// <summary>
    /// Normalization of the vector with the zero vector check
    /// </summary>
    /// <returns>
    /// The normalized vector. If the vector is zero, then zero is returned.
    /// </returns>
    public Vector NormalizeZero() {
      if (IsZero) {
        return Zero(Dim);
      }

      return Normalize();
    }

    /// <summary>
    /// The angle from the first vector to the another one. It is from the interval [-pi, pi).
    /// It uses the TNum.Acos to calculate the angle.
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
        if (Tools.LT(dot, Tools.MinusOne) || Tools.GT(dot, Tools.One)) { // dot < -1 || dot > 1)
          throw new ArgumentException($"Vector.Angle: The dot production of v1 = {v1} and v2 = {v2} is beyond [-1-eps, 1+eps]!");
        }
#endif
        // The intervals [-1-eps,-1] and [1,1+eps] are contracted to -1 and 1 respectively
        if (Tools.EQ(dot, Tools.MinusOne) && dot <= Tools.MinusOne) {
          return TNum.Pi;
        }
        if (Tools.EQ(dot, Tools.One) && dot >= Tools.One) {
          return TNum.Zero;
        }

        // Consider regular case (-1,1)
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
    /// Expands the vector to a higher dimension.
    /// </summary>
    /// <param name="d">The target dimension to expand to. Must be greater than the current dimension of the vector.</param>
    /// <param name="val">The value to expand with.</param>
    /// <returns>A new vector in the target dimension, with the last coordinates sets to val.</returns>
    public Vector LiftUp(int d, TNum val) {
      Debug.Assert(d > Dim, "Vector.LiftUp: Can't lift to lower dimension!");
      TNum[] np = new TNum[d];
      for (int i = 0; i < Dim; i++) {
        np[i] = _v[i];
      }
      for (int i = Dim; i < d; i++) {
        np[i] = val;
      }

      return new Vector(np);
    }

    /// <summary>
    /// Returns the string contains coordinates of a point in the specified format.
    /// x0 x1 ... xDim-1
    /// </summary>
    /// <returns>The string in the specified format.</returns>
    public string ToStrSepBySpace() => string.Join(' ', _v.Select(v => TConv.ToDouble(v).ToString(null, CultureInfo.InvariantCulture)));
#endregion

#region Functions related to Vectors
    /// <summary>
    /// Computes the outer product of the current vector and the given vector.
    /// </summary>
    /// <param name="v">The vector to compute the outer product with.</param>
    /// <returns>A matrix representing the outer product of the two vectors.</returns>
    public Matrix OuterProduct(Vector v) {
      TNum[,] result = new TNum[Dim, v.Dim];
      for (int i = 0; i < Dim; i++) {
        for (int j = 0; j < v.Dim; j++) {
          result[i, j] = this[i] * v[j];
        }
      }

      return new Matrix(result);
    }

    /// <summary>
    /// Extracts a subvector from the current vector, starting from the specified index and ending at the specified index.
    /// </summary>
    /// <param name="startIndex">The starting index of the subvector (inclusive).</param>
    /// <param name="endIndex">The ending index of the subvector (inclusive).</param>
    /// <returns>A new vector containing the elements from the specified range.</returns>
    public Vector SubVector(int startIndex, int endIndex) {
      Debug.Assert(startIndex <= endIndex, "Vector.SubVector: start index must be less or equal than end index!");
      Debug.Assert(startIndex >= 0, "Vector.SubVector: start index must be non negative.");
      Debug.Assert(endIndex < Dim, "Vector.SubVector: end index must be lesser than dimension of the vector.");

      int    length      = endIndex - startIndex + 1;
      TNum[] subElements = new TNum[length];
      Array.Copy(_v, startIndex, subElements, 0, length);

      return new Vector(subElements);
    }
#endregion

#region Overrides
    public override int GetHashCode() => throw new InvalidOperationException(); //HashCode.Combine(IsZero, Dim);

    public override bool Equals(object? obj) {
#if DEBUG
      if (obj is not Vector vector) {
        throw new ArgumentException($"{obj} is not a Vector!");
      }
#endif
      return CompareTo((Vector)obj!) == 0;
    }

    public override string ToString() => ToStringWithBraces('(', ')', ',');


    public string ToStringWithBraces(char braceOpen, char braceClose, char delim) =>
      braceOpen + string.Join(delim, _v.Select(v => TConv.ToDouble(v).ToString(null, CultureInfo.InvariantCulture))) + braceClose;

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
    }

    /// <summary>
    /// Constructor on the basis of a one-dimensional array
    /// </summary>
    /// <param name="nv">The array</param>
    public Vector(TNum[] nv) {  // TODO: bool needCopy = true
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
    }

    /// <summary>
    /// Copying constructor
    /// </summary>
    /// <param name="v">The vector to be copied</param>
    public Vector(Vector v) {
      _v = v.GetAsArray();
    }

    /// <summary>
    /// Constructor to a multidimensional vector from a two-dimensional vector
    /// </summary>
    /// <param name="v">The vector to be copied</param>
    /// <returns>The resultant vector</returns>
    public Vector(Vector2D v) : this(new TNum[] { v[0], v[1] }) { }
#endregion

#region Fabrics
    /// <summary>
    /// Makes the zero vector of given dimension.
    /// </summary>
    /// <param name="dim">The dimension of the vector.</param>
    /// <returns>The zero vector.</returns>
    public static Vector Zero(int dim) {
      Debug.Assert(dim > 0, $"Vector.Zero: The dimension of the vector must be greater than 0! Found dim = {dim}");
      TNum[] orig = new TNum[dim];

      return new Vector(orig);
    }

    /// <summary>
    /// Makes the i-th orth of given dimension. (1,0,0) == MakeOrth(3,1).
    /// </summary>
    /// <param name="dim">The dimension of the vector.</param>
    /// <param name="pos">The position of '1'.</param>
    /// <returns>The i-orth of given dimension.</returns>
    public static Vector MakeOrth(int dim, int pos) {
      Debug.Assert(pos > 0 && pos <= dim, "Vector.MakeOrth: Position should be greater than 0 and less or equal than dimension of the vector.");
      TNum[] orth = new TNum[dim];
      orth[pos - 1] = TNum.MultiplicativeIdentity;

      return new Vector(orth);
    }

    /// <summary>
    /// Makes the vector of '1' of given dimension.
    /// </summary>
    /// <param name="dim">The dimension of the vector.</param>
    /// <returns>The vector of ones.</returns>
    public static Vector Ones(int dim) {
      Debug.Assert(dim > 0, $"Vector.Ones: The dimension of the vector must be greater than 0! Found dim = {dim}");

      TNum[] ones = new TNum[dim];
      for (int i = 0; i < dim; i++) {
        ones[i] = Tools.One;
      }

      return new Vector(ones);
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
    /// Generates an arbitrary vector of the specified dimension. Each coordinate: [a, b).
    /// </summary>
    /// <param name="dim">The dimension of the vector.</param>
    /// <param name="a">The minimum value of each coordinate.</param>
    /// <param name="b">The maximum value of each coordinate.</param>
    /// <param name="random">If null, then default one be used.</param>
    /// <returns>A random vector.</returns>
    public static Vector GenVector(int dim, TNum a, TNum b, GRandomLC? random = null) => new Vector(GenArray(dim, a, b, random));

    /// <summary>
    /// Generates an arbitrary vector of integers of the specified dimension. Each coordinate: [a, b).
    /// </summary>
    /// <param name="dim">The dimension of the vector.</param>
    /// <param name="a">The minimum value of each coordinate.</param>
    /// <param name="b">The maximum value of each coordinate.</param>
    /// <param name="random">If null, then default one be used.</param>
    /// <returns>A random vector.</returns>
    public static Vector GenVectorInt(int dim, int a, int b, GRandomLC? random = null) => new Vector(GenArrayInt(dim, a, b, random));
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
#endregion

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

    /// <summary>
    /// Linear combination of two points
    /// </summary>
    /// <param name="p1">The first point</param>
    /// <param name="w1">The weight of the first point</param>
    /// <param name="p2">The second point</param>
    /// <param name="w2">The weight of the second point</param>
    /// <returns>The resultant point</returns>
    public static Vector LinearCombination(Vector p1, TNum w1, Vector p2, TNum w2) {
#if DEBUG
      if (p1.Dim != p2.Dim) {
        throw new ArgumentException("Cannot combine two point of different dimensions");
      }
#endif
      TNum[] coords = new TNum[p1.Dim];

      for (int i = 0; i < p1.Dim; i++) {
        coords[i] = w1 * p1[i] + w2 * p2[i];
      }

      return new Vector(coords);
    }

    /// <summary>
    /// Linear combination of a collection of points
    /// </summary>
    /// <param name="ps">Collection of the points</param>
    /// <param name="ws">Collection of the weights (has at least, the same number of elements as the collection of points)</param>
    /// <returns>The resultant point</returns>
    public static Vector LinearCombination(IEnumerable<Vector> ps, IEnumerable<TNum> ws) {
      using IEnumerator<Vector> enPoint  = ps.GetEnumerator();
      using IEnumerator<TNum>   enWeight = ws.GetEnumerator();

#if DEBUG
      if (!enPoint.MoveNext()) {
        throw new ArgumentException("No points in the collection to combine");
      }

      if (!enWeight.MoveNext()) {
        throw new ArgumentException("No weights in the collection to combine");
      }
#else
      enPoint.MoveNext();
      enWeight.MoveNext();
#endif


      int    dim    = enPoint.Current.Dim;
      TNum[] coords = new TNum[dim];

      do {
#if DEBUG
        if (enPoint.Current.Dim != dim) {
          throw new ArgumentException("Dimension of a point in the collection differs from the dimension of the point");
        }
#endif
        for (int i = 0; i < dim; i++) {
          coords[i] += enPoint.Current[i] * enWeight.Current;
        }
      } while (enPoint.MoveNext() && enWeight.MoveNext());

      return new Vector(coords);
    }

  }

}
