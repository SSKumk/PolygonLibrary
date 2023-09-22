using System;
using System.Diagnostics;
using System.Globalization;
using System.Numerics;

namespace CGLibrary;

public partial class Geometry<TNum, TConv> where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {


/// <summary>
/// Class of a matrix. Indices are assumed to be zero-based
/// </summary>
public class Matrix : IEquatable<Matrix> {

#region Internal storage, access properties, and convertors
  /// <summary>
  /// The internal linear storage of the matrix as a one-dimensional array
  /// </summary>
  private readonly TNum[] _m;

  /// <summary>
  /// Number of rows of the matrix
  /// </summary>
  public readonly int Rows;

  /// <summary>
  /// Number of columns of the matrix
  /// </summary>
  public readonly int Cols;

  /// <summary>
  /// Indexer access
  /// </summary>
  /// <param name="i">The row counted from zero</param>
  /// <param name="j">The column counted from zero</param>
  /// <returns>The value of the corresponding component</returns>
  public TNum this[int i, int j] {
    get
      {
#if DEBUG
        if (i < 0 || i >= Rows) {
          throw new IndexOutOfRangeException("The first index is out of range");
        }

        if (j < 0 || j >= Cols) {
          throw new IndexOutOfRangeException("The second index is out of range");
        }
#endif
        return _m[i * Cols + j];
      }
  }

  /// <summary>
  /// Convert a matrix to a two-dimensional array
  /// </summary>
  /// <param name="m">The matrix to be converted</param>
  /// <returns>The resultant array</returns>
  public static implicit operator TNum[,](Matrix m) {
    int       i, j, k = 0, r = m.Rows, c = m.Cols;
    TNum[,] res     = new TNum[r, c];

    for (i = 0; i < r; i++) {
      for (j = 0; j < c; j++, k++) {
        res[i, j] = m._m[k];
      }
    }

    return res;
  }

  /// <summary>
  /// Converting a two-dimensional array to a matrix
  /// </summary>
  /// <param name="m">Array to be converted</param>
  /// <returns>The resultant matrix</returns>
  public static explicit operator Matrix(TNum[,] m) => new Matrix(m);
#endregion

#region Constructors
  /// <summary>
  /// Default constructor; creates a 1x1 zero matrix
  /// </summary>
  public Matrix() : this(1, 1) { }

  /// <summary>
  /// The default construct producing a zero matrix of given size
  /// </summary>
  /// <param name="n">Number of rows</param>
  /// <param name="m">Number of columns</param>
  public Matrix(int n, int m) {
#if DEBUG
    if (n <= 0 || m <= 0) {
      throw new ArgumentException("Dimension of a matrix cannot be non-positive");
    }
#endif
    _m   = new TNum[n * m];
    Rows = n;
    Cols = m;
  }

  /// <summary>
  /// Constructor on the basis of information about sizes and one-dimensional array
  /// that contains elements of the matrix in row-wise order
  /// </summary>
  /// <param name="n">Number of rows</param>
  /// <param name="m">Number of columns</param>
  /// <param name="ar">The array</param>
  public Matrix(int n, int m, TNum[] ar) {
#if DEBUG
    if (n <= 0) {
      throw new ArgumentException("Number of rows should be positive");
    }

    if (m <= 0) {
      throw new ArgumentException("Number of columns should be positive");
    }

    if (n * m != ar.Length) {
      throw new ArgumentException("Product of sizes does not equal to number of elements in the array");
    }

    if (ar.Rank != 1) {
      throw new ArgumentException("The array is not one-dimensional");
    }
#endif
    Rows = n;
    Cols = m;
    _m   = ar;
  }

  /// <summary>
  /// Constructor on the basis of a two-dimensional array
  /// </summary>
  /// <param name="nm">The new array</param>
  public Matrix(TNum[,] nm) {
#if DEBUG
    if (nm.Length <= 0) {
      throw new ArgumentException("Number of matrix elements should be positive");
    }

    if (nm.Rank != 2) {
      throw new ArgumentException("Cannot initialize a matrix by a array that is not two-dimensional");
    }
#endif
    Rows = nm.GetLength(0);
    Cols = nm.GetLength(1);
#if DEBUG
    if (Rows <= 0) {
      throw new ArgumentException("Matrix cannot have a non-positive number of rows");
    }

    if (Cols <= 0) {
      throw new ArgumentException("Matrix cannot have a non-positive number of columns");
    }
#endif
    _m = new TNum[Rows * Cols];
    int k = 0;

    foreach (TNum el in nm) {
      _m[k] = el;
      k++;
    }
  }

  /// <summary>
  /// Copying constructor
  /// </summary>
  /// <param name="m">The matrix to be copied</param>
  public Matrix(Matrix m) {
    Rows = m.Rows;
    Cols = m.Cols;
    _m   = new TNum[Rows * Cols];
    int k = 0;

    foreach (TNum el in m._m) {
      _m[k] = el;
      k++;
    }
  }
#endregion

#region Overrides
  public override bool Equals(object? obj) {
#if DEBUG
    if (obj is not Matrix matrix) {
      throw new ArgumentException($"{obj} is not a Matrix.");
    }
#endif
    return Equals((Matrix)obj);
  }

  public bool Equals(Matrix? m) {
    Debug.Assert(m != null, nameof(m) + " != null");

    if (Rows != m.Rows || Cols != m.Cols) {
      return false;
    }

    int k, kMax = Rows * Cols;

    for (k = 0; k < kMax; k++) {
      if (Tools.NE(_m[k], m._m[k])) {
        return false;
      }
    }

    return true;
  }

  public override string ToString() {
    string res = "[";
    int    k   = 0, i, j;

    for (i = 0; i < Rows; i++) {
      res += $" [{_m[k].ToString(null, CultureInfo.InvariantCulture)}";
      k++;

      for (j = 1; j < Cols; j++) {
        res += $", {_m[k].ToString(null, CultureInfo.InvariantCulture)}";
        k++;
      }

      res += "]";
    }

    res += " ]";

    return res;
  }

  public override int GetHashCode() {
    int res = 0;
    res = HashCode.Combine(res, Rows);
    res = HashCode.Combine(res, Cols);

    foreach (TNum el in _m) {
      res = HashCode.Combine(res, TNum.Round(el / Tools.Eps));
    }

    return res;
  }
#endregion

#region Operators
  /// <summary>
  /// Unary minus - the opposite matrix
  /// </summary>
  /// <param name="m">The matrix to be reversed</param>
  /// <returns>The opposite vector</returns>
  public static Matrix operator -(Matrix m) {
    int      d  = m.Rows * m.Cols, i;
    TNum[] nv = new TNum[d];

    for (i = 0; i < d; i++) {
      nv[i] = -m._m[i];
    }

    return new Matrix(m.Rows, m.Cols, nv);
  }

  /// <summary>
  /// Sum of two matrices
  /// </summary>
  /// <param name="m1">The first matrix summand</param>
  /// <param name="m2">The second matrix summand</param>
  /// <returns>The sum</returns>
  public static Matrix operator +(Matrix m1, Matrix m2) {
#if DEBUG
    if (m1.Rows != m2.Rows || m1.Cols != m2.Cols) {
      throw new ArgumentException("Cannot add two matrices of different sizes");
    }
#endif
    int      d  = m1.Rows * m1.Cols, i;
    TNum[] nv = new TNum[d];

    for (i = 0; i < d; i++) {
      nv[i] = m1._m[i] + m2._m[i];
    }

    return new Matrix(m1.Rows, m1.Cols, nv);
  }

  /// <summary>
  /// Difference of two vectors
  /// </summary>
  /// <param name="m1">The matrix minuend</param>
  /// <param name="m2">The matrix subtrahend</param>
  /// <returns>The difference</returns>
  public static Matrix operator -(Matrix m1, Matrix m2) {
#if DEBUG
    if (m1.Rows != m2.Rows || m1.Cols != m2.Cols) {
      throw new ArgumentException("Cannot subtract two matrices of different sizes");
    }
#endif
    int      d  = m1.Rows * m1.Cols, i;
    TNum[] nv = new TNum[d];

    for (i = 0; i < d; i++) {
      nv[i] = m1._m[i] - m2._m[i];
    }

    return new Matrix(m1.Rows, m1.Cols, nv);
  }

  /// <summary>
  /// Left multiplication of a matrix by a number
  /// </summary>
  /// <param name="a">The numeric factor</param>
  /// <param name="m">The matrix factor</param>
  /// <returns>The product</returns>
  public static Matrix operator *(TNum a, Matrix m) {
    int      d  = m.Rows * m.Cols, i;
    TNum[] nv = new TNum[d];

    for (i = 0; i < d; i++) {
      nv[i] = a * m._m[i];
    }

    return new Matrix(m.Rows, m.Cols, nv);
  }

  /// <summary>
  /// Right multiplication of a matrix by a number
  /// </summary>
  /// <param name="m">The matrix factor</param>
  /// <param name="a">The numeric factor</param>
  /// <returns>The product</returns>
  public static Matrix operator *(Matrix m, TNum a) => a * m;

  /// <summary>
  /// Division of a matrix by a number
  /// </summary>
  /// <param name="m">The matrix dividend</param>
  /// <param name="a">The numeric divisor</param>
  /// <returns>The product</returns>
  public static Matrix operator /(Matrix m, TNum a) {
#if DEBUG
    if (Tools.EQ(a)) {
      throw new DivideByZeroException();
    }
#endif
    int      d  = m.Rows * m.Cols, i;
    TNum[] nv = new TNum[d];

    for (i = 0; i < d; i++) {
      nv[i] = m._m[i] / a;
    }

    return new Matrix(m.Rows, m.Cols, nv);
  }

  /// <summary>
  /// Multiplication of a matrix by a vector at right. The vector factor and the result
  /// are considered as a column vectors
  /// </summary>
  /// <param name="m">The matrix (first) factor</param>
  /// <param name="v">The vector (second) factor</param>
  /// <returns>The resultant vector</returns>
  public static Vector operator *(Matrix m, Vector v) {
#if DEBUG
    if (m.Cols != v.Dim) {
      throw new ArgumentException("Cannot multiply a matrix and a vector of improper dimensions");
    }
#endif
    TNum[] res = new TNum[m.Rows];
    int      r   = m.Rows, c = m.Cols, i, j, k = 0;

    for (i = 0; i < r; i++) {
      for (j = 0; j < c; j++, k++) {
        res[i] += m._m[k] * v[j];
      }
    }

    return new Vector(res);
  }

  /// <summary>
  /// Multiplication of a matrix by a vector at left. The vector factor and the result
  /// are considered as row vectors
  /// </summary>
  /// <param name="v">The vector (first) factor</param>
  /// <param name="m">The matrix (second) factor</param>
  /// <returns>The resultant vector</returns>
  public static Vector operator *(Vector v, Matrix m) {
#if DEBUG
    if (m.Rows != v.Dim) {
      throw new ArgumentException("Cannot multiply a matrix and a vector of improper dimensions");
    }
#endif
    TNum[] res = new TNum[m.Cols];
    int      r   = m.Rows, c = m.Cols, i, j, k;

    for (i = 0; i < c; i++) {
      for (j = 0, k = i; j < r; j++, k += c) {
        res[i] += m._m[k] * v[j];
      }
    }

    return new Vector(res);
  }

  /// <summary>
  /// Multiplication of a matrix by a point at right. The point factor is considered
  /// as a column vector. The result is considered as a point
  /// </summary>
  /// <param name="m">The matrix (first) factor</param>
  /// <param name="p">The point (second) factor</param>
  /// <returns>The resultant vector</returns>
  public static Point operator *(Matrix m, Point p) {
#if DEBUG
    if (m.Cols != p.Dim) {
      throw new ArgumentException("Cannot multiply a matrix and a point of improper dimensions");
    }
#endif
    TNum[] res = new TNum[m.Rows];
    int      r   = m.Rows, c = m.Cols, i, j, k = 0;

    for (i = 0; i < r; i++) {
      for (j = 0; j < c; j++, k++) {
        res[i] += m._m[k] * p[j];
      }
    }

    return new Point(res);
  }

  /// <summary>
  /// Multiplication of two matrices
  /// </summary>
  /// <param name="m1">The first matrix factor</param>
  /// <param name="m2">The second matrix factor</param>
  /// <returns>The resultant matrix</returns>
  public static Matrix operator *(Matrix m1, Matrix m2) {
#if DEBUG
    if (m1.Cols != m2.Rows) {
      throw new ArgumentException("Cannot multiply two matrices of improper dimensions");
    }
#endif
    int      r   = m1.Rows, c = m2.Cols, d = r * c, temp = m1.Cols, i, j, k, m1Ind, m1Start, m2Ind, resInd = 0;
    TNum[] res = new TNum[d];

    for (i = 0, m1Start = 0; i < r; i++, m1Start += temp) {
      for (j = 0; j < c; j++, resInd++) {
        for (k = 0, m1Ind = m1Start, m2Ind = j; k < temp; k++, m1Ind++, m2Ind += c) {
          res[resInd] += m1._m[m1Ind] * m2._m[m2Ind];
        }
      }
    }

    return new Matrix(r, c, res);
  }

  /// <summary>
  /// Horizontal concatenation of two matrices (with equal number of rows)
  /// </summary>
  /// <param name="m1">The left concatenated matrix</param>
  /// <param name="m2">The right concatenated matrix</param>
  /// <returns>The resultant matrix</returns>
  public static Matrix hcat(Matrix m1, Matrix m2) {
#if DEBUG
    if (m1.Rows != m2.Rows) {
      throw new ArgumentException("Cannot concatenate horizontally matrices with different number of rows");
    }
#endif
    int      r  = m1.Rows, c1 = m1.Cols, c2 = m2.Cols, c = c1 + c2, d = r * c, i, j, k = 0, k1 = 0, k2 = 0;
    TNum[] nv = new TNum[d];

    for (i = 0; i < r; i++) {
      for (j = 0; j < c1; j++, k++, k1++) {
        nv[k] = m1._m[k1];
      }

      for (j = 0; j < c1; j++, k++, k2++) {
        nv[k] = m2._m[k2];
      }
    }

    return new Matrix(r, c, nv);
  }

  /// <summary>
  /// Vertical concatenation of two matrices (with equal number of columns)
  /// </summary>
  /// <param name="m1">The upper concatenated matrix</param>
  /// <param name="m2">The lower concatenated matrix</param>
  /// <returns>The resultant matrix</returns>
  public static Matrix vcat(Matrix m1, Matrix m2) {
#if DEBUG
    if (m1.Cols != m2.Cols) {
      throw new ArgumentException("Cannot concatenate vertically matrices with different number of columns");
    }
#endif
    int      d  = (m1.Rows + m2.Rows) * m1.Cols, k = 0;
    TNum[] nv = new TNum[d];

    foreach (TNum el in m1._m) {
      nv[k] = el;
      k++;
    }

    foreach (TNum el in m2._m) {
      nv[k] = el;
      k++;
    }

    return new Matrix(m1.Rows + m2.Rows, m1.Cols, nv);
  }
#endregion

#region Taking submatrices
  /// <summary>
  /// Construct a submatrix consisting of given rows of the original matrix
  /// </summary>
  /// <param name="rows">List of row indices to be taken</param>
  /// <returns>The resultant matrix</returns>
  public Matrix TakeRows(int[] rows) {
    int r = rows.Length;
#if DEBUG
    if (r <= 0) {
      throw new ArgumentException("Wrong number of rows to be taken");
    }
#endif
    int      c   = Cols, d = r * c, k = 0, ind, i, j;
    TNum[] res = new TNum[d];

    for (i = 0; i < rows.Length; i++) {
      for (j = 0, ind = rows[i] * c; j < c; j++, ind++, k++) {
        res[k] = _m[ind];
      }
    }

    return new Matrix(r, c, res);
  }

  /// <summary>
  /// Construct a submatrix consisting of given columns of the original matrix
  /// </summary>
  /// <param name="cols">List of column indices to be taken</param>
  /// <returns>The resultant matrix</returns>
  public Matrix TakeCols(int[] cols) {
    int c = cols.Length;
#if DEBUG
    if (c <= 0) {
      throw new ArgumentException("Wrong number of columns to be taken");
    }
#endif
    int      r   = Rows, d = r * c, k = 0, start, i, j;
    TNum[] res = new TNum[d];

    for (i = 0, start = 0; i < r; i++, start += Cols) {
      for (j = 0; j < c; j++, k++) {
        res[k] = _m[start + cols[j]];
      }
    }

    return new Matrix(r, c, res);
  }

  /// <summary>
  /// Construct a submatrix consisting of elements at crossing of given rows and columns of the original matrix
  /// </summary>
  /// <param name="rows">List of row indices to be taken</param>
  /// <param name="cols">List of column indices to be taken</param>
  /// <returns>The resultant matrix</returns>
  public Matrix TakeElems(int[] rows, int[] cols) {
    int r = rows.Length, c = cols.Length;
#if DEBUG
    if (r <= 0) {
      throw new ArgumentException("Wrong number of rows to be taken");
    }

    if (c <= 0) {
      throw new ArgumentException("Wrong number of columns to be taken");
    }
#endif
    int      d   = r * c, k = 0, start, i, j;
    TNum[] res = new TNum[d];

    for (i = 0; i < r; i++) {
      start = rows[i] * Cols;

      for (j = 0; j < c; j++, k++) {
        res[k] = _m[start + cols[j]];
      }
    }

    return new Matrix(r, c, res);
  }
#endregion

#region Matrix factories
  /// <summary>
  /// Return zero square matrix n-by-n
  /// </summary>
  /// <param name="n">Size of the matrix</param>
  /// <returns>The resultant matrix</returns>
  public static Matrix Zero(int n) => new Matrix(n, n);

  /// <summary>
  /// Return zero rectangular matrix n-by-m
  /// </summary>
  /// <param name="n">Number of rows</param>
  /// <param name="m">Number of columns</param>
  /// <returns>The resultant matrix</returns>
  public static Matrix Zero(int n, int m) {
#if DEBUG
    if (n <= 0) {
      throw new ArgumentException("Number of rows of a matrix should be positive");
    }

    if (m <= 0) {
      throw new ArgumentException("Number of columns of a matrix should be positive");
    }
#endif
    return new Matrix(n, m);
  }

  /// <summary>
  /// Return zero matrix n-by-n containing units
  /// </summary>
  /// <param name="n">Size of the matrix</param>
  /// <returns>The resultant matrix</returns>
  public static Matrix One(int n) {
#if DEBUG
    if (n <= 0) {
      throw new ArgumentException("Size of a square matrix should be positive");
    }
#endif
    return One(n, n);
  }

  /// <summary>
  /// Return rectangular matrix n-by-m containing units
  /// </summary>
  /// <param name="n">Number of rows</param>
  /// <param name="m">Number of columns</param>
  /// <returns>The resultant matrix</returns>
  public static Matrix One(int n, int m) {
#if DEBUG
    if (n <= 0) {
      throw new ArgumentException("Number of rows of a matrix should be positive");
    }

    if (m <= 0) {
      throw new ArgumentException("Number of columns of a matrix should be positive");
    }
#endif
    int      d  = n * m, i;
    TNum[] nv = new TNum[d];

    for (i = 0; i < d; i++) {
      nv[i] = Tools.One;
    }

    return new Matrix(n, m, nv);
  }

  /// <summary>
  /// Return unit square matrix n-by-n
  /// </summary>
  /// <param name="n">Size of the matrix</param>
  /// <returns>The resultant matrix</returns>
  public static Matrix Eye(int n) {
#if DEBUG
    if (n <= 0) {
      throw new ArgumentException("Size of a square matrix should be positive");
    }
#endif
    return Eye(n, n);
  }

  /// <summary>
  /// Return rectangular unite matrix n-by-m
  /// </summary>
  /// <param name="n">Number of rows</param>
  /// <param name="m">Number of columns</param>
  /// <returns>The resultant matrix</returns>
  public static Matrix Eye(int n, int m) {
#if DEBUG
    if (n <= 0) {
      throw new ArgumentException("Number of rows of a matrix should be positive");
    }

    if (m <= 0) {
      throw new ArgumentException("Number of columns of a matrix should be positive");
    }
#endif
    int      d  = n * m, i, j, k = 0;
    TNum[] nv = new TNum[d];

    for (i = 0; i < n; i++) {
      for (j = 0; j < m; j++, k++) {
        if (i == j) {
          nv[k] = Tools.One;
        } else {
          nv[k] = Tools.Zero;
        }
      }
    }

    return new Matrix(n, m, nv);
  }
#endregion

}

}
