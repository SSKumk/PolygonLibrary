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
      TNum[] v = new TNum[SpaceDim];
      _v.CopyTo(v, 0);

      return v;
    }

    /// <summary>
    /// Dimension of the vector
    /// </summary>
    public int SpaceDim => _v.Length;

    /// <summary>
    /// Indexer access
    /// </summary>
    /// <param name="i">The index of the coordinate</param>
    /// <returns>The value of the corresponding component</returns>
    public TNum this[int i] {
      get
        {
          Debug.Assert(i >= 0 && i < SpaceDim, $"Vector.Indexer: Index out of range. Found index: {i}, dimension: {SpaceDim}");

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

            for (int i = 0; i < SpaceDim; i++) {
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

      int d = SpaceDim, res;

      Debug.Assert
        (
         d == other.SpaceDim
       , $"Vector.CompareTo: Can not compare vectors of different dimensions. Dimensions: this = {d}, other = {other.SpaceDim}"
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
    /// Normalizes the vector.
    /// </summary>
    /// <returns>The normalized vector.</returns>
    public Vector Normalize() {
      Debug.Assert(!IsZero, "Vector.Normalize: Can't normalize a zero vector.");

      TNum[] res = new TNum[SpaceDim];
      for (int i = 0; i < SpaceDim; i++) {
        res[i] = _v[i] / Length;
      }

      return new Vector(res, TNum.MultiplicativeIdentity, false);
    }

    /// <summary>
    /// Normalizes the vector with a check for the zero vector.
    /// </summary>
    /// <returns>
    /// The normalized vector. If the length of the vector is zero, then the zero vector of the corresponding dimension is returned.
    /// </returns>
    public Vector NormalizeZero() => IsZero ? Zero(SpaceDim) : Normalize();

    /// <summary>
    /// Perform the projection to a 2D affine space with the origin o and basis (u1,u2) by the formula:
    ///<c>res = ([this - o] * u1) * u1 + ([this - o] * u2) * u2</c>
    /// </summary>
    /// <param name="o">The origin of the affine space.</param>
    /// <param name="u1">The first basis vector of the plane.</param>
    /// <param name="u2">The second basis vector of the plane.</param>
    /// <returns>The projected vector.</returns>
    public Vector ProjectTo2DAffineSpace(Vector o, Vector u1, Vector u2) {
      Debug.Assert
        (
         u1.SpaceDim == SpaceDim && u2.SpaceDim == SpaceDim
       , $"Vector.ProjectToPlane: Cannot compute a dot production of two vectors of different dimensions. Found {u1.SpaceDim} and {u2.SpaceDim}."
        );

      TNum fst = Tools.Zero;
      TNum snd = Tools.Zero;
      for (int i = 0; i < SpaceDim; i++) {
        TNum vi = _v[i] - o[i];
        fst += vi * u1[i];
        snd += vi * u2[i];
      }

      TNum[] res = new TNum[SpaceDim];
      for (int i = 0; i < SpaceDim; i++) {
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
      Debug.Assert(d > SpaceDim, "Vector.LiftUp: Can't lift up to the lower dimension!");

      TNum[] np = new TNum[d];
      for (int i = 0; i < SpaceDim; i++) {
        np[i] = _v[i];
      }
      for (int i = SpaceDim; i < d; i++) {
        np[i] = val;
      }

      return new Vector(np, false);
    }
#endregion

#region Functions related to Vectors
    /// <summary>
    /// The cosine of angle from the first vector to the another one.
    /// It uses the TNum.Acos to calculate the angle
    /// </summary>
    /// <param name="v1">The first vector</param>
    /// <param name="v2">The second vector</param>
    /// <returns>The cosine</returns>
    public static TNum CosAngle(Vector v1, Vector v2) {
      if (v1.IsZero || v2.IsZero) {
        return Tools.One;
      }
      else {
        return (v1 * v2) / v1.Length / v2.Length;
      }
    }

    /// <summary>
    /// Returns the angle from <paramref name="v1"/> to <paramref name="v2"/> in the interval <c>[0, π]</c>.
    /// Uses <c>TNum.Acos</c> for computation.
    /// </summary>
    /// <param name="v1">The first vector</param>
    /// <param name="v2">The second vector</param>
    /// <returns>The angle.</returns>
    public static TNum Angle(Vector v1, Vector v2) {
      TNum dot = Vector.CosAngle(v1, v2);
      Debug.Assert
        (
         Tools.GE(dot, Tools.MinusOne) && Tools.LE(dot, Tools.One)
       , $"Vector.Angle: The dot product of v1 = {v1} and v2 = {v2} is beyond [-1-{Tools.Eps}, 1+{Tools.Eps}]! Found value: {dot}"
        );

      if (Tools.EQ(dot, Tools.MinusOne)) {
        return TNum.Pi;
      }
      if (Tools.EQ(dot, Tools.One)) {
        return TNum.Zero;
      }

      // Consider regular case (-1,1)
      return TNum.Acos(dot);
    }

    /// <summary>
    /// Computes the outer product of the current vector and the given vector.
    /// </summary>
    /// <param name="v">The vector to compute the outer product with.</param>
    /// <returns>A matrix representing the outer product of the two vectors.</returns>
    public Matrix OuterProduct(Vector v) {
      int rows = SpaceDim, cols = v.SpaceDim;

      TNum[] res = new TNum[rows * cols];
      int    k   = 0;
      for (int i = 0; i < rows; i++) {
        for (int j = 0; j < cols; j++) {
          res[k] = _v[i] * v._v[j];
          k++;
        }
      }

      return new Matrix(rows, cols, res, false);
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
      Debug.Assert(endIndex < SpaceDim, "Vector.SubVector: end index must be lesser than dimension of the vector.");

      int    length      = endIndex - startIndex + 1;
      TNum[] subElements = new TNum[length];
      Array.Copy(_v, startIndex, subElements, 0, length);

      return new Vector(subElements, false);
    }


    /// <summary>
    /// Affine dot product. (v1 - origin) * v2.
    /// </summary>
    /// <param name="origin">The origin of the affine space.</param>
    /// <param name="v1">The first vector factor.</param>
    /// <param name="v2">The second vector factor.</param>
    /// <returns>The product</returns>
    public static TNum AffMul(Vector v1, Vector origin, Vector v2) {
      Debug.Assert
        (
         v1.SpaceDim == v2.SpaceDim
       , $"Vector.AffMul: Cannot compute a dot production of two vectors of different dimensions. Found {v1.SpaceDim} and {v2.SpaceDim}."
        );

      int  d   = v1.SpaceDim, i;
      TNum res = Tools.Zero;

      for (i = 0; i < d; i++) {
        res += (v1._v[i] - origin._v[i]) * v2._v[i];
      }

      return res;
    }

    /// <summary>
    /// Multiplies the first vector by a scalar and adds the second vector.
    /// </summary>
    /// <param name="v1">The vector to be multiplied by the scalar.</param>
    /// <param name="a">The scalar to multiply the first vector.</param>
    /// <param name="v2">The vector to be added.</param>
    /// <returns>A new vector as the result of the multiplication and addition.</returns>
    public static Vector MulByNumAndAdd(Vector v1, TNum a, Vector v2) {
      TNum[] res = new TNum[v1.SpaceDim];

      for (int i = 0; i < res.Length; i++) {
        res[i] = v1._v[i] * a + v2._v[i];
      }

      return new Vector(res, false);
    }
#endregion

#region Overrides
    public override int GetHashCode() => throw new InvalidOperationException();

    public override bool Equals(object? obj) {
      if (obj is not Vector vector) {
        return false;
      }

      return CompareTo(vector) == 0;
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
    public string ToStringBraceAndDelim(char? braceOpen, char? braceClose, char delim)
      => (braceOpen is null ? "" : braceOpen) + string.Join
           (delim, _v.Select(v => TConv.ToDouble(v).ToString(null, CultureInfo.InvariantCulture))) +
         (braceClose is null ? "" : braceClose);
#endregion

#region Constructors
    /// <summary>
    /// The default construct producing the zero vector.
    /// </summary>
    /// <param name="n">The dimension of the vector.</param>
    public Vector(int n) {
      Debug.Assert(n > 0, $"Vector.Ctor: Dimension of a vector cannot be non-positive. Found {n}.");

      _v = new TNum[n];
      for (int i = 0; i < _v.Length; i++) {
        _v[i] = Tools.Zero;
      }
    }

    /// <summary>
    /// Constructs a new vector using the specified one-dimensional array of components.
    /// </summary>
    /// <param name="nv">An array of vector components.</param>
    /// <param name="needCopy">
    /// Indicates whether a copy of the array should be made.
    /// If <c>true</c>, a copy of the array is created; otherwise, the original array is used directly.
    /// </param>
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
    /// Construct a new vector with the specified components and length.
    /// </summary>
    /// <param name="nv">An array of vector components.</param>
    /// <param name="vlen">The length of the vector.</param>
    /// <param name="needCopy">
    /// Indicates whether a copy of the array should be made.
    /// If <c>true</c>, a copy of the array is created; otherwise, the original array is used directly.
    /// </param>
    private Vector(TNum[] nv, TNum vlen, bool needCopy = true) : this(nv, needCopy) { _length = vlen; }

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

#region Factories
    /// <summary>
    /// Makes the zero vector of given dimension.
    /// </summary>
    /// <param name="dim">The dimension of the vector.</param>
    /// <returns>The zero vector.</returns>
    public static Vector Zero(int dim) {
      Debug.Assert(dim > 0, $"Vector.Zero: The dimension of the vector must be greater than 0! Found dim = {dim}");

      return new Vector(dim);
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
      for (int i = 0; i < orth.Length; i++) {
        orth[i] = Tools.Zero;
      }
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
      int    d  = v.SpaceDim, i;
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
      Debug.Assert
        (
         v1.SpaceDim == v2.SpaceDim
       , $"Vector.+: Can not add two vectors of different dimensions. Found {v1.SpaceDim} and {v2.SpaceDim}."
        );

      int    d  = v1.SpaceDim, i;
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
        (
         v1.SpaceDim == v2.SpaceDim
       , $"Vector.+: Cannot subtract two vectors of different dimensions. Found {v1.SpaceDim} and {v2.SpaceDim}."
        );

      int    d  = v1.SpaceDim, i;
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
      int    d  = v.SpaceDim, i;
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
      Debug.Assert(Tools.NE(a), $"Vector./: Can't divide by zero.");

      int    d  = v.SpaceDim, i;
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
         v1.SpaceDim == v2.SpaceDim
       , $"Vector.+: Cannot compute a dot production of two vectors of different dimensions. Found {v1.SpaceDim} and {v2.SpaceDim}."
        );

      int  d   = v1.SpaceDim, i;
      TNum res = Tools.Zero;

      for (i = 0; i < d; i++) {
        res += v1._v[i] * v2._v[i];
      }

      return res;
    }
#endregion

#region Static methods
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
        (
         v1.SpaceDim == v2.SpaceDim
       , $"Vector.+: Cannot combine two vectors of different dimensions. Found {v1.SpaceDim} and {v2.SpaceDim}."
        );

      TNum[] coords = new TNum[v1.SpaceDim];

      for (int i = 0; i < v1.SpaceDim; i++) {
        coords[i] = w1 * v1[i] + w2 * v2[i];
      }

      return new Vector(coords, false);
    }

    /// <summary>
    /// Linear combination of a collection of vectors with weights.
    /// </summary>
    /// <param name="Vs">Collection of the vectors.</param>
    /// <param name="Ws">Collection of the weights (has at least the same number of elements as the collection of vectors).</param>
    /// <returns>The resultant vector.</returns>
    public static Vector LinearCombination(IReadOnlyList<Vector> Vs, IReadOnlyList<TNum> Ws) {
      Debug.Assert(Vs.Count > 0, "Vector collection must contain at least one element. Found 0");
      Debug.Assert
        (
         Vs.Count == Ws.Count
       , $"The number of vectors must match the number of weights. Found {Vs.Count} vectors and {Ws.Count} weights"
        );

      Vector result = Zero(Vs[0].SpaceDim);

      for (int i = 0; i < Vs.Count; i++) {
        result += Vs[i] * Ws[i];
      }

      return result;
    }

  }
#endregion

}
