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
          Debug.Assert(i >= 0 && i < Dim, $"Vector.Indexer: Index out of range. Found index: {i}, dimension: {Dim}");

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
      if (other is null) { return 1; } // null < this (always)

      int d = Dim, res;

      Debug.Assert
        (
         d == other.Dim
       , $"Vector.CompareTo: Can not compare vectors of different dimensions. Dimensions: this = {d}, other = {other.Dim}"
        );

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
      }
      else {
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
      Debug.Assert
        (
         u1.Dim == Dim && u2.Dim == Dim
       , $"Vector.ProjectToPlane: Cannot compute a dot production of two vectors of different dimensions. Found {u1.Dim} and {u2.Dim}."
        );

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

      return new Vector(res, false);
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

      return new Vector(np, false);
    }
#endregion

#region Functions related to Vectors
    /// <summary>
    /// Computes the outer product of the current vector and the given vector.
    /// </summary>
    /// <param name="v">The vector to compute the outer product with.</param>
    /// <returns>A matrix representing the outer product of the two vectors.</returns>
    public Matrix OuterProduct(Vector v) {
      int rows = Dim, cols = v.Dim;

      TNum[] result = new TNum[rows * cols];
      int k = 0;
      for (int i = 0; i < rows; i++) {
        for (int j = 0; j < cols; j++) {
          result[k] = this[i] * v[j];
          k++;
        }
      }

      return new Matrix(rows, cols, result, false);
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

      return new Vector(subElements, false);
    }
#endregion

#region Overrides
    public override int GetHashCode() => throw new InvalidOperationException();

    public override bool Equals(object? obj) {
      if (obj is not Vector) {
        return false;
      }

      return CompareTo((Vector)obj!) == 0;
    }

    /// <summary>
    /// Creates a string representation of the vector using
    /// parentheses and a comma as a delimiter.
    /// </summary>
    /// <returns>A string representing the vector.</returns>
    public override string ToString() => ToStringBraceAndDelim('(', ')', ',');

    /// <summary>
    /// Creates a string representation of the vector using
    /// specified brace characters and a delimiter.
    /// </summary>
    /// <param name="braceOpen">The opening brace character.</param>
    /// <param name="braceClose">The closing brace character.</param>
    /// <param name="delim">The delimiter character.</param>
    /// <returns>A string representing the vector with the given braces and delimiter.</returns>
    public string ToStringBraceAndDelim(char braceOpen, char braceClose, char delim)
      => braceOpen + string.Join
           (delim, _v.Select(v => TConv.ToDouble(v).ToString(null, CultureInfo.InvariantCulture))) + braceClose;
#endregion

#region Constructors
    /// <summary>
    /// The default construct producing the zero vector.
    /// </summary>
    /// <param name="n">The dimension of the vector.</param>
    public Vector(int n) {
      Debug.Assert(n > 0, $"Vector.Ctor: Dimension of a vector cannot be non-positive. Found {n}.");

      _v = new TNum[n];
    }

    /// <summary>
    /// Constructor on the basis of a one-dimensional array
    /// </summary>
    /// <param name="nv">The array</param>
    /// <param name="needCopy">Indicates whether a copy of the array should be made. If <c>true</c>, a copy is made; otherwise, the original array is used directly.</param>
    public Vector(TNum[] nv, bool needCopy = true) {
      Debug.Assert(nv.Length > 0, $"Vector.Ctor: Dimension of a vector cannot be non-positive. Found {nv.Length}.");
      Debug.Assert(nv.Rank == 1, $"Vector.Ctor: Cannot initialize a vector by a multidimensional array. Found {nv.Rank}.");

      if (needCopy) {
        _v = new TNum[nv.Length];

        for (int i = 0; i < nv.Length; i++) {
          _v[i] = nv[i];
        }
      }
      else {
        _v = nv;
      }
    }

    /// <summary>
    /// Copying constructor.
    /// </summary>
    /// <param name="v">The vector to be copied.</param>
    public Vector(Vector v) { _v = v.GetAsArray(); }

    /// <summary>
    /// Constructor to a multidimensional vector from a two-dimensional vector.
    /// </summary>
    /// <param name="v">The vector to be copied</param>
    /// <returns>The resultant vector</returns>
    public Vector(Vector2D v) : this(new TNum[] { v[0], v[1] }, false) { }
#endregion

#region Fabrics
    /// <summary>
    /// Makes the zero vector of given dimension.
    /// </summary>
    /// <param name="dim">The dimension of the vector.</param>
    /// <returns>The zero vector.</returns>
    public static Vector Zero(int dim) {
      Debug.Assert(dim > 0, $"Vector.Zero: The dimension of the vector must be greater than 0! Found dim = {dim}");

      return new Vector(new TNum[dim], false);
    }

    /// <summary>
    /// Makes the i-th orth of given dimension. (1,0,0) == MakeOrth(3,1).
    /// </summary>
    /// <param name="dim">The dimension of the vector.</param>
    /// <param name="pos">The position of '1'.</param>
    /// <returns>The i-orth of given dimension.</returns>
    public static Vector MakeOrth(int dim, int pos) {
      Debug.Assert
        (
         pos > 0 && pos <= dim
       , "Vector.MakeOrth: Position should be greater than 0 and less or equal than dimension of the vector."
        );

      TNum[] orth = new TNum[dim];
      orth[pos - 1] = TNum.MultiplicativeIdentity;

      return new Vector(orth, false);
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

      return new Vector(ones, false);
    }

    /// <summary>
    /// Generates a non-zero random vector of the specified dimension. Each coordinate: [-0.5, 0.5].
    /// </summary>
    /// <param name="dim">The dimension of the vector.</param>
    /// <param name="random">If null then default one be used.</param>
    /// <returns>A random vector.</returns>
    public static Vector GenVector(int dim, GRandomLC? random = null) {
      Debug.Assert(dim > 0, $"Vector.GenVector: Number of coordinates of a vector should be positive. Found {dim}");

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
    public static Vector GenVector(int dim, TNum a, TNum b, GRandomLC? random = null)
      => new Vector(GenArray(dim, a, b, random), false);

    /// <summary>
    /// Generates an arbitrary vector of integers of the specified dimension. Each coordinate: [a, b).
    /// </summary>
    /// <param name="dim">The dimension of the vector.</param>
    /// <param name="a">The minimum value of each coordinate.</param>
    /// <param name="b">The maximum value of each coordinate.</param>
    /// <param name="random">If null, then default one be used.</param>
    /// <returns>A random vector.</returns>
    public static Vector GenVectorInt(int dim, int a, int b, GRandomLC? random = null)
      => new Vector(GenArrayInt(dim, a, b, random), false);
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

      return new Vector(nv, false);
    }

    /// <summary>
    /// Sum of two vectors of equal dimension
    /// </summary>
    /// <param name="v1">The first vector summand.</param>
    /// <param name="v2">The second vector summand.</param>
    /// <returns>The sum</returns>
    public static Vector operator +(Vector v1, Vector v2) {
      Debug.Assert(v1.Dim == v2.Dim, $"Vector.+: Can not add two vectors of different dimensions. Found {v1.Dim} and {v2.Dim}.");

      int    d  = v1.Dim, i;
      TNum[] nv = new TNum[d];

      for (i = 0; i < d; i++) {
        nv[i] = v1._v[i] + v2._v[i];
      }

      return new Vector(nv, false);
    }

    /// <summary>
    /// Difference of two vectors of the same dimensions
    /// </summary>
    /// <param name="v1">The vector minuend</param>
    /// <param name="v2">The vector subtrahend</param>
    /// <returns>The difference</returns>
    public static Vector operator -(Vector v1, Vector v2) {
      Debug.Assert
        (v1.Dim == v2.Dim, $"Vector.+: Cannot subtract two vectors of different dimensions. Found {v1.Dim} and {v2.Dim}.");

      int    d  = v1.Dim, i;
      TNum[] nv = new TNum[d];

      for (i = 0; i < d; i++) {
        nv[i] = v1._v[i] - v2._v[i];
      }

      return new Vector(nv, false);
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

      return new Vector(nv, false);
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
      Debug.Assert(Tools.EQ(a), $"Vector./: Can not divide by zero.");

      int    d  = v.Dim, i;
      TNum[] nv = new TNum[d];

      for (i = 0; i < d; i++) {
        nv[i] = v._v[i] / a;
      }

      return new Vector(nv, false);
    }

    /// <summary>
    /// Dot product.
    /// </summary>
    /// <param name="v1">The first vector factor.</param>
    /// <param name="v2">The second vector factor.</param>
    /// <returns>The product</returns>
    public static TNum operator *(Vector v1, Vector v2) {
      Debug.Assert
        (
         v1.Dim == v2.Dim
       , $"Vector.+: Cannot compute a dot production of two vectors of different dimensions. Found {v1.Dim} and {v2.Dim}."
        );

      //TODO: Все if #DEBUG ~~~> Debug.Assert  по всей библиотеке

      int  d   = v1.Dim, i;
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
    /// <returns>true, if the vectors are orthogonal; false, otherwise</returns>
    public static bool AreOrthogonal(Vector v1, Vector v2) {
      TNum l1 = v1.Length, l2 = v2.Length;

      return Tools.EQ(l1) || Tools.EQ(l2) || Tools.EQ(TNum.Abs(v1 * v2 / (l1 * l2)));
    }

    /// <summary>
    /// Linear combination of two vectors
    /// </summary>
    /// <param name="v1">The first vector</param>
    /// <param name="w1">The weight of the first vector</param>
    /// <param name="v2">The second vector</param>
    /// <param name="w2">The weight of the second vector</param>
    /// <returns>The resultant vector</returns>
    public static Vector LinearCombination(Vector v1, TNum w1, Vector v2, TNum w2) {
      Debug.Assert
        (v1.Dim == v2.Dim, $"Vector.+: Cannot combine two vectors of different dimensions. Found {v1.Dim} and {v2.Dim}.");

      TNum[] coords = new TNum[v1.Dim];

      for (int i = 0; i < v1.Dim; i++) {
        coords[i] = w1 * v1[i] + w2 * v2[i];
      }

      return new Vector(coords, false);
    }

    /// <summary>
    /// Linear combination of a collection of vectors with weights
    /// </summary>
    /// <param name="Vs">Collection of the vectors</param>
    /// <param name="Ws">Collection of the weights (has at least the same number of elements as the collection of vectors)</param>
    /// <returns>The resultant vector</returns>
    public static Vector LinearCombination(IReadOnlyList<Vector> Vs, IReadOnlyList<TNum> Ws) {
      Debug.Assert(Vs.Count > 0, "Vector collection must contain at least one element. Found 0");
      Debug.Assert
        (
         Vs.Count == Ws.Count
       , $"The number of vectors must match the number of weights. Found {Vs.Count} vectors and {Ws.Count} weights"
        );

      Vector result = Vs[0] * Ws[0];

      for (int i = 0; i < Vs.Count; i++) {
        result += Vs[i] * Ws[i];
      }

      return result;
    }


  }

}
