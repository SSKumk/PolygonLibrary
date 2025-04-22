using System.Text;

namespace CGLibrary;

public partial class Geometry<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  /// <summary>
  /// Represents an immutable matrix.
  /// The matrix is stored in a one-dimensional array.
  /// </summary>
  /// <remarks>
  /// All indexes assumed to be zero-based.
  /// </remarks>
  public class Matrix : IEquatable<Matrix> {

#region Internal storage, access properties, and convertors
    /// <summary>
    /// The internal linear storage of the matrix as a one-dimensional array.
    /// </summary>
    protected readonly TNum[] _m;

    /// <summary>
    /// Number of rows of the matrix.
    /// </summary>
    public readonly int Rows;

    /// <summary>
    /// Number of columns of the matrix.
    /// </summary>
    public readonly int Cols;

    /// <summary>
    /// Gets the value at the specified row and column in the matrix.
    /// </summary>
    /// <param name="i">The row index (zero-based).</param>
    /// <param name="j">The column index (zero-based).</param>
    /// <returns>The value of the corresponding component in the matrix.</returns>
    public TNum this[int i, int j] {
      get
        {
          Debug.Assert
            (
             i >= 0 && i < Rows
           , $"Matrix.Indexer: The row index must be within valid range. Found row index {i}, expected range [0, {Rows - 1}]."
            );
          Debug.Assert
            (
             j >= 0 && j < Cols
           , $"Matrix.Indexer: The column index must be within valid range. Found column index {j}, expected range [0, {Cols - 1}]."
            );

          return _m[i * Cols + j];
        }
    }

    /// <summary>
    /// Gets the value at the specified index in the matrix as a one-dimensional array.
    /// </summary>
    /// <param name="i">The index (zero-based).</param>
    /// <returns>The value at the specified index in the matrix.</returns>
    public TNum this[int i] {
      get
        {
          Debug.Assert
            (
             i >= 0 && i < _m.Length
           , $"Matrix.Indexer: The index must be within valid range. Found index {i}, expected range [0, {_m.Length - 1}]."
            );

          return _m[i];
        }
    }

    /// <summary>
    /// Convert a matrix to a two-dimensional array
    /// </summary>
    /// <param name="m">The matrix to be converted</param>
    /// <returns>The resultant array</returns>
    public static implicit operator TNum[,](Matrix m) {
      int     i, j, k = 0, r = m.Rows, c = m.Cols;
      TNum[,] res     = new TNum[r, c];

      for (i = 0; i < r; i++) {
        for (j = 0; j < c; j++, k++) {
          res[i, j] = m._m[k];
        }
      }

      return res;
    }

    /// <summary>
    /// Converts a two-dimensional array into a matrix.
    /// </summary>
    /// <param name="m">The two-dimensional array to be converted.</param>
    /// <returns>The resulting matrix.</returns>
    public static explicit operator Matrix(TNum[,] m) => new Matrix(m);
#endregion

#region Constructors
    /// <summary>
    /// Default constructor; creates a 1x1 zero matrix.
    /// </summary>
    public Matrix() : this(1, 1) { }

    /// <summary>
    /// Constructs a zero matrix of the specified size.
    /// </summary>
    /// <param name="n">Number of rows.</param>
    /// <param name="m">Number of columns.</param>
    private Matrix(int n, int m) {
      Debug.Assert(n > 0, $"Matrix.Ctor: Number of rows must be positive. Found {n}");
      Debug.Assert(m > 0, $"Matrix.Ctor: Number of columns must be positive. Found {m}");

      int d = n * m;
      _m = new TNum[d];
      for (int i = 0; i < d; i++) { _m[i] = Tools.Zero; }
      Rows = n;
      Cols = m;
    }

    /// <summary>
    /// Construct a new matrix based on the number of rows, columns,
    /// and a one-dimensional array containing the elements of the matrix in row-wise order.
    /// </summary>
    /// <param name="n">The number of rows in the matrix.</param>
    /// <param name="m">The number of columns in the matrix.</param>
    /// <param name="ar">The one-dimensional array containing matrix elements in row-wise order.</param>
    /// <param name="needCopy">Indicates whether a copy of the array should be made. If <c>true</c>, a copy is made;
    /// otherwise, the original array is used directly.
    /// </param>
    public Matrix(int n, int m, TNum[] ar, bool needCopy = true) {
      Debug.Assert(n > 0, $"Matrix.Ctor: Number of rows should be positive. Found {n}");
      Debug.Assert(m > 0, $"Matrix.Ctor: Number of columns should be positive. Found {m}");
      Debug.Assert
        (
         n * m == ar.Length
       , $"Matrix.Ctor: Product of sizes does not equal to number of elements in the array. Expected {n * m}, found {ar.Length}"
        );
      Debug.Assert(ar.Rank == 1, $"Matrix.Ctor: The array is not one-dimensional. Found rank {ar.Rank}");


      Rows = n;
      Cols = m;

      if (needCopy) {
        _m = new TNum[n * m];
        ar.CopyTo(_m, 0);
      }
      else {
        _m = ar;
      }
    }

    /// <summary>
    /// Constructs a matrix from a two-dimensional array.
    /// </summary>
    /// <param name="nm">The two-dimensional array used to construct the matrix.</param>
    public Matrix(TNum[,] nm) {
      Debug.Assert(nm.Length > 0, $"Number of matrix elements should be positive. Found {nm.Length}");
      Debug.Assert(nm.Rank == 2, $"Cannot initialize a matrix by an array that is not two-dimensional. Found rank {nm.Rank}");

      Rows = nm.GetLength(0);
      Cols = nm.GetLength(1);

      Debug.Assert(Rows > 0, $"Matrix cannot have a non-positive number of rows. Found {Rows}");
      Debug.Assert(Cols > 0, $"Matrix cannot have a non-positive number of columns. Found {Cols}");

      _m = new TNum[Rows * Cols];
      int k = 0;

      foreach (TNum el in nm) {
        _m[k] = el;
        k++;
      }
    }

    /// <summary>
    /// Copy constructor that creates a matrix from another matrix.
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

    /// <summary>
    /// Creates a matrix consisting of a single row from the specified vector.
    /// </summary>
    /// <param name="v">The vector used to create a matrix with one row and <c>v.SpaceDim</c> columns.</param>
    public Matrix(Vector v) {
      Rows = 1;
      Cols = v.SpaceDim;
      _m   = v.GetCopyAsArray();
    }
#endregion

#region Overrides
    /// <summary>
    /// Throws an <see cref="InvalidOperationException"/> because hash code generation is not supported for matrices.
    /// </summary>
    public override int GetHashCode() => throw new InvalidOperationException();

    /// <summary>
    /// Compares this matrix to another object for equality.
    /// </summary>
    /// <param name="obj">The object to compare with this matrix.</param>
    /// <returns><c>true</c> if the object is a matrix and is equal to this matrix; otherwise, <c>false</c>.</returns>
    /// <exception cref="ArgumentException">Thrown if the object is not a matrix.</exception>
    public override bool Equals(object? obj) {
      Debug.Assert(obj is not null, "Matrix.Equals: The object being compared cannot be null.");
      Debug.Assert(obj is Matrix, $"Matrix.Equals: The object must be a Matrix, found {obj.GetType()}.");

      return Equals((Matrix)obj);
    }

    /// <summary>
    /// Compares this matrix to another matrix for equality.
    /// </summary>
    /// <param name="m">The matrix to compare with this matrix.</param>
    /// <returns><c>true</c> if the matrices are equal; otherwise, <c>false</c>.</returns>
    public bool Equals(Matrix? m) {
      Debug.Assert(m is not null, "The matrix to compare must not be null.");

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

    /// <summary>
    /// Converts the matrix to its string representation.
    /// </summary>
    /// <returns>A string that represents the matrix.</returns>
    public override string ToString() {
      int[] maxLengths = new int[Cols];
      for (int i = 0; i < _m.Length; i++) {
        int colIndex = i % Cols;
        maxLengths[colIndex] = Math.Max(maxLengths[colIndex], _m[i].ToString()!.Length);
      }

      StringBuilder sb = new StringBuilder();
      for (int row = 0; row < Rows; row++) {
        for (int col = 0; col < Cols; col++) {
          int index = row * Cols + col;
          sb.Append(_m[index].ToString()?.PadLeft(maxLengths[col]));
          if (col != Cols - 1) { sb.Append("  "); }
        }
        sb.AppendLine();
      }

      return sb.ToString();
    }
#endregion

#region Operators
    /// <summary>
    /// Unary minus - returns the opposite matrix.
    /// </summary>
    /// <param name="m">The matrix to be reversed.</param>
    /// <returns>The opposite matrix.</returns>
    public static Matrix operator -(Matrix m) {
      int    d  = m.Rows * m.Cols, i;
      TNum[] nv = new TNum[d];

      for (i = 0; i < d; i++) {
        nv[i] = -m._m[i];
      }

      return new Matrix(m.Rows, m.Cols, nv, false);
    }

    /// <summary>
    /// Sum of two matrices.
    /// </summary>
    /// <param name="m1">The first matrix summand.</param>
    /// <param name="m2">The second matrix summand.</param>
    /// <returns>The sum of the two matrices.</returns>
    public static Matrix operator +(Matrix m1, Matrix m2) {
      Debug.Assert
        (
         m1.Rows == m2.Rows && m1.Cols == m2.Cols
       , $"Matrix.+: Cannot add two matrices of different sizes. Found m1: {m1.Rows}x{m1.Cols}, m2: {m2.Rows}x{m2.Cols}"
        );

      int    d  = m1.Rows * m1.Cols, i;
      TNum[] nv = new TNum[d];

      for (i = 0; i < d; i++) {
        nv[i] = m1._m[i] + m2._m[i];
      }

      return new Matrix(m1.Rows, m1.Cols, nv, false);
    }

    /// <summary>
    /// Difference of two vectors.
    /// </summary>
    /// <param name="m1">The matrix minuend.</param>
    /// <param name="m2">The matrix subtrahend.</param>
    /// <returns>The difference of two matrices.</returns>
    public static Matrix operator -(Matrix m1, Matrix m2) {
      Debug.Assert
        (
         m1.Rows == m2.Rows && m1.Cols == m2.Cols
       , $"Matrix.-: Cannot subtract two matrices of different sizes. Found m1: {m1.Rows}x{m1.Cols}, m2: {m2.Rows}x{m2.Cols}"
        );
      int    d  = m1.Rows * m1.Cols, i;
      TNum[] nv = new TNum[d];

      for (i = 0; i < d; i++) {
        nv[i] = m1._m[i] - m2._m[i];
      }

      return new Matrix(m1.Rows, m1.Cols, nv, false);
    }

    /// <summary>
    /// Left multiplication of a matrix by a number.
    /// </summary>
    /// <param name="a">The numeric factor.</param>
    /// <param name="m">The matrix factor.</param>
    /// <returns>The product of the matrix and the number.</returns>
    public static Matrix operator *(TNum a, Matrix m) {
      int    d  = m.Rows * m.Cols, i;
      TNum[] nv = new TNum[d];

      for (i = 0; i < d; i++) {
        nv[i] = a * m._m[i];
      }

      return new Matrix(m.Rows, m.Cols, nv, false);
    }

    /// <summary>
    /// Right multiplication of a matrix by a number.
    /// </summary>
    /// <param name="m">The matrix factor.</param>
    /// <param name="a">The numeric factor.</param>
    /// <returns>The product of the matrix and the number.</returns>
    public static Matrix operator *(Matrix m, TNum a) => a * m;

    /// <summary>
    /// Division of a matrix by a number.
    /// </summary>
    /// <param name="m">The matrix dividend.</param>
    /// <param name="a">The numeric divisor.</param>
    /// <returns>The result of the division.</returns>
    public static Matrix operator /(Matrix m, TNum a) {
      Debug.Assert(Tools.NE(a), $"Division by zero detected. Found {a}");

      int    d  = m.Rows * m.Cols, i;
      TNum[] nv = new TNum[d];

      for (i = 0; i < d; i++) {
        nv[i] = m._m[i] / a;
      }

      return new Matrix(m.Rows, m.Cols, nv, false);
    }

    /// <summary>
    /// Multiplication of a matrix by a vector at right. The vector factor and the result
    /// are considered as a column vectors.
    /// </summary>
    /// <param name="m">The matrix (first) factor.</param>
    /// <param name="v">The vector (second) factor.</param>
    /// <returns>The resultant vector.</returns>
    public static Vector operator *(Matrix m, Vector v) {
      Debug.Assert
        (
         m.Cols == v.SpaceDim
       , $"Matrix.*: Cannot multiply a matrix and a vector of improper dimensions. Matrix columns: {m.Cols}, vector dimensions: {v.SpaceDim}"
        );

      TNum[] res = new TNum[m.Rows];
      int    r   = m.Rows, c = m.Cols, i, j, k = 0;

      for (i = 0; i < r; i++) {
        res[i] = Tools.Zero;
        for (j = 0; j < c; j++, k++) {
          res[i] += m._m[k] * v[j];
        }
      }

      return new Vector(res, false);
    }

    /// <summary>
    /// Multiplication of a matrix by a vector at left. The vector factor and the result
    /// are considered as row vectors.
    /// </summary>
    /// <param name="v">The vector (first) factor.</param>
    /// <param name="m">The matrix (second) factor.</param>
    /// <returns>The resultant vector.</returns>
    public static Vector operator *(Vector v, Matrix m) {
      Debug.Assert
        (
         m.Rows == v.SpaceDim
       , $"Matrix.*: Cannot multiply a vector and a matrix of improper dimensions. Matrix rows: {m.Rows}, vector dimensions: {v.SpaceDim}"
        );

      TNum[] res = new TNum[m.Cols];
      int    r   = m.Rows, c = m.Cols, i, j, k;

      for (i = 0; i < c; i++) {
        res[i] = Tools.Zero;
        for (j = 0, k = i; j < r; j++, k += c) {
          res[i] += m._m[k] * v[j];
        }
      }

      return new Vector(res, false);
    }

    /// <summary>
    /// Multiplication of two matrices.
    /// </summary>
    /// <param name="m1">The first matrix factor.</param>
    /// <param name="m2">The second matrix factor.</param>
    /// <returns>The resultant matrix.</returns>
    public static Matrix operator *(Matrix m1, Matrix m2) {
      Debug.Assert
        (
         m1.Cols == m2.Rows
       , $"Matrix.*: Cannot multiply two matrices of improper dimensions. Matrix m1 columns: {m1.Cols}, Matrix m2 rows: {m2.Rows}"
        );

      int r = m1.Rows, c = m2.Cols, d = r * c, temp = m1.Cols, i, j, k, m1Ind, m1Start, m2Ind, resInd = 0;

      TNum[] res = new TNum[d];

      for (i = 0, m1Start = 0; i < r; i++, m1Start += temp) {
        for (j = 0; j < c; j++, resInd++) {
          res[resInd] = Tools.Zero;
          for (k = 0, m1Ind = m1Start, m2Ind = j; k < temp; k++, m1Ind++, m2Ind += c) {
            res[resInd] += m1._m[m1Ind] * m2._m[m2Ind];
          }
        }
      }

      return new Matrix(r, c, res, false);
    }

    /// <summary>
    /// Horizontal concatenation of two matrices (with equal number of rows).
    /// </summary>
    /// <param name="m1">The left concatenated matrix.</param>
    /// <param name="m2">The right concatenated matrix.</param>
    /// <returns>The resultant matrix.</returns>
    public static Matrix? hcat(Matrix? m1, Matrix? m2) {
      if (m1 is null && m2 is null) {
        return null;
      }
      if (m1 is null && m2 is not null) {
        return m2;
      }
      if (m1 is not null && m2 is null) {
        return m2;
      }

      Debug.Assert
        (
         m1!.Rows == m2!.Rows
       , $"Matrix.hcat: Cannot concatenate horizontally matrices with different number of rows. Matrix m1 rows: {m1.Rows}, Matrix m2 rows: {m2.Rows}"
        );

      int    r  = m1.Rows, c1 = m1.Cols, c2 = m2.Cols, c = c1 + c2, d = r * c, i, j, k = 0, k1 = 0, k2 = 0;
      TNum[] nv = new TNum[d];

      for (i = 0; i < r; i++) {
        for (j = 0; j < c1; j++, k++, k1++) {
          nv[k] = m1._m[k1];
        }

        for (j = 0; j < c2; j++, k++, k2++) {
          nv[k] = m2._m[k2];
        }
      }

      return new Matrix(r, c, nv, false);
    }

    /// <summary>
    /// Horizontal concatenation of matrix and a vector (with equal number of rows).
    /// </summary>
    /// <param name="m">The left concatenated matrix.</param>
    /// <param name="v">The right concatenated vector.</param>
    /// <returns>The resultant matrix.</returns>
    public static Matrix hcat(Matrix m, Vector v) {
      Debug.Assert
        (
         m.Rows == v.SpaceDim
       , $"Matrix.hcat: Cannot concatenate matrix and vector with different number of rows. Matrix rows: {m.Rows}, Vector dimension: {v.SpaceDim}"
        );

      int    r  = m.Rows, c1 = m.Cols, c = c1 + 1, d = r * c, k = 0, k1 = 0;
      TNum[] nv = new TNum[d];

      for (int i = 0; i < r; i++) {
        for (int j = 0; j < c1; j++, k++, k1++) {
          nv[k] = m._m[k1];
        }
        nv[k] = v[i]; // m.Rows == v.SpaceDim
        k++;
      }

      return new Matrix(r, c, nv, false);
    }

    /// <summary>
    /// Vertically concatenates a matrix and a vector, adding the vector as the last row.
    /// </summary>
    /// <param name="m">The matrix to which the vector will be appended.</param>
    /// <param name="v">The vector to append as a new row. Its dimension must match the number of columns in the matrix.</param>
    /// <returns>A new matrix with one additional row containing the elements of the vector.</returns>
    public static Matrix vcat(Matrix m, Vector v) {
      Debug.Assert
        (
         m.Cols == v.SpaceDim
       , $"Matrix.vcat: Cannot concatenate matrix and vector with different number of columns. Matrix cols: {m.Cols}, Vector dimension: {v.SpaceDim}"
        );

      int    r  = m.Rows + 1;
      TNum[] nv = new TNum[r * m.Cols];
      m._m.CopyTo(nv, 0);
      v.V.CopyTo(nv, m.Rows * m.Cols);

      return new Matrix(r, m.Cols, nv, false);
    }

    /// <summary>
    /// Vertically concatenates the matrix and a vector, adding the vector as the last row.
    /// </summary>
    /// <param name="v">The vector to append as a new row. Its dimension must match the number of columns in the matrix.</param>
    /// <returns>A new matrix with one additional row containing the elements of the vector.</returns>
    public Matrix vcat(Vector v) => vcat(this, v);


    /// <summary>
    /// Vertical concatenation of two matrices (with equal number of columns).
    /// </summary>
    /// <param name="m1">The upper concatenated matrix.</param>
    /// <param name="m2">The lower concatenated matrix.</param>
    /// <returns>The resultant matrix.</returns>
    public static Matrix vcat(Matrix m1, Matrix m2) {
      Debug.Assert
        (
         m1.Cols == m2.Cols
       , $"Matrix.vcat: Cannot concatenate vertically matrices with different number of columns. Matrix m1 columns: {m1.Cols}, Matrix m2 columns: {m2.Cols}"
        );

      int    d  = (m1.Rows + m2.Rows) * m1.Cols, k = 0;
      TNum[] nv = new TNum[d];

      foreach (TNum el in m1._m) {
        nv[k] = el;
        k++;
      }

      foreach (TNum el in m2._m) {
        nv[k] = el;
        k++;
      }

      return new Matrix(m1.Rows + m2.Rows, m1.Cols, nv, false);
    }
#endregion

#region Taking submatrices
    /// <summary>
    /// Construct a submatrix consisting of given rows of the original matrix.
    /// </summary>
    /// <param name="rows">List of row indices to be taken.</param>
    /// <returns>The resultant matrix.</returns>
    public Matrix TakeRows(params int[] rows) {
      int r = rows.Length;

      Debug.Assert(r > 0 && r <= Rows, $"Matrix.TakeRows: Wrong number of rows to be taken. Found {r}");

      int    d   = r * Cols, k = 0, ind, i, j;
      TNum[] res = new TNum[d];

      for (i = 0; i < rows.Length; i++) {
        for (j = 0, ind = rows[i] * Cols; j < Cols; j++, ind++, k++) {
          res[k] = _m[ind];
        }
      }

      return new Matrix(r, Cols, res, false);
    }

    /// <summary>
    /// Construct a submatrix consisting of given columns of the original matrix.
    /// </summary>
    /// <param name="cols">List of column indices to be taken.</param>
    /// <returns>The resultant matrix.</returns>
    public Matrix TakeCols(params int[] cols) {
      int c = cols.Length;

      Debug.Assert(c > 0 && c <= Cols, $"Matrix.TakeCols: Wrong number of columns to be taken. Found {c}");

      int    d   = Rows * c, k = 0, start, i, j;
      TNum[] res = new TNum[d];

      for (i = 0, start = 0; i < Rows; i++, start += Cols) {
        for (j = 0; j < c; j++, k++) {
          res[k] = _m[start + cols[j]];
        }
      }

      return new Matrix(Rows, c, res, false);
    }

    /// <summary>
    /// Construct a submatrix consisting of elements at crossing of given rows and columns of the original matrix.
    /// </summary>
    /// <param name="rows">List of row indices to be taken. If null, all row indices be taken.</param>
    /// <param name="cols">List of column indices to be taken. If null, all column indices be taken.</param>
    /// <returns>The resultant matrix.</returns>
    public Matrix TakeSubMatrix(int[]? rows, int[]? cols) {
      int r, c;
      if (rows is null) {
        rows = Enumerable.Range(0, Rows).ToArray();
        r    = Rows;
      }
      else {
        r = rows.Length;
      }


      if (cols is null) {
        cols = Enumerable.Range(0, Cols).ToArray();
        c    = Cols;
      }
      else {
        c = cols.Length;
      }

      Debug.Assert(r > 0, $"Matrix.TakeSubMatrix: Wrong number of rows to be taken. Found {r}");
      Debug.Assert(c > 0, $"Matrix.TakeSubMatrix: Wrong number of columns to be taken. Found {c}");

      int    d   = r * c, k = 0, start, i, j;
      TNum[] res = new TNum[d];

      for (i = 0; i < r; i++) {
        start = rows[i] * Cols;

        for (j = 0; j < c; j++, k++) {
          res[k] = _m[start + cols[j]];
        }
      }

      return new Matrix(r, c, res, false);
    }

    /// <summary>
    /// Retrieves a vector corresponding to the specified row in the matrix.
    /// </summary>
    /// <param name="rowInd">The row index (zero-based) from which to take the vector.</param>
    /// <returns>A vector containing the elements of the specified row.</returns>
    public Vector TakeVector(int rowInd) {
      Debug.Assert(rowInd >= 0 && rowInd < Rows, "Matrix.TakeVector: Row index must lie in [0, Rows).");

      TNum[] res = new TNum[Cols];
      int    k   = rowInd * Cols;
      for (int col = 0; col < Cols; col++) {
        res[col] = _m[k + col];
      }

      return new Vector(res, false);
    }

    /// <summary>
    /// Transposes the matrix, swapping its rows with columns.
    /// </summary>
    /// <returns>A new matrix that is the transpose of the current matrix.</returns>
    public Matrix Transpose() {
      TNum[] tm = new TNum[Cols * Rows];
      int    kt = 0;
      for (int col = 0; col < Cols; col++) {
        for (int row = 0, k = col; row < Rows; row++, k += Cols) {
          tm[kt] = _m[k];
          kt++;
        }
      }

      return new Matrix(Cols, Rows, tm, false);
    }
#endregion

#region Matrix factories
    /// <summary>
    /// Return zero square matrix n-by-n.
    /// </summary>
    /// <param name="n">Size of the matrix.</param>
    /// <returns>The resultant square zero matrix.</returns>
    public static Matrix Zero(int n) => new Matrix(n, n);

    /// <summary>
    /// Return zero rectangular matrix n-by-m.
    /// </summary>
    /// <param name="n">Number of rows.</param>
    /// <param name="m">Number of columns.</param>
    /// <returns>The resultant rectangle zero matrix.</returns>
    public static Matrix Zero(int n, int m) {
      Debug.Assert(n > 0, $"Matrix.Zero: Number of rows of a matrix should be positive. Found {n}");
      Debug.Assert(m > 0, $"Matrix.Zero: Number of columns of a matrix should be positive. Found {m}");

      return new Matrix(n, m);
    }

    /// <summary>
    /// Return zero matrix of size n-by-n containing units (ones).
    /// </summary>
    /// <param name="n">Size of the matrix.</param>
    /// <returns>The resultant rectangular matrix filled with ones.</returns>
    public static Matrix One(int n) {
      Debug.Assert(n > 0, $"Matrix.One: Size of a square matrix should be positive. Found {n}");

      return One(n, n);
    }

    /// <summary>
    /// Returns a rectangular matrix of size n-by-m filled with units (ones).
    /// </summary>
    /// <param name="n">Number of rows.</param>
    /// <param name="m">Number of columns.</param>
    /// <returns>The resultant rectangular matrix filled with ones.</returns>
    public static Matrix One(int n, int m) {
      Debug.Assert(n > 0, $"Matrix.One: Number of rows of a matrix should be positive. Found {n}");
      Debug.Assert(m > 0, $"Matrix.One: Number of columns of a matrix should be positive. Found {m}");

      int    d  = n * m, i;
      TNum[] nv = new TNum[d];

      for (i = 0; i < d; i++) {
        nv[i] = Tools.One;
      }

      return new Matrix(n, m, nv, false);
    }

    /// <summary>
    /// Return unit square matrix of size n-by-n.
    /// </summary>
    /// <param name="n">Size of the matrix.</param>
    /// <returns>The resultant square unit matrix.</returns>
    public static Matrix Eye(int n) {
      Debug.Assert(n > 0, $"Matrix.Eye: Size of a square matrix should be positive. Found {n}");

      return Eye(n, n);
    }

    /// <summary>
    /// Return rectangular unite matrix of size n-by-m.
    /// </summary>
    /// <param name="n">Number of rows.</param>
    /// <param name="m">Number of columns.</param>
    /// <returns>The resultant rectangle unit matrix.</returns>
    public static Matrix Eye(int n, int m) {
      Debug.Assert(n > 0, $"Matrix.Eye: Number of rows of a matrix should be positive. Found {n}");
      Debug.Assert(m > 0, $"Matrix.Eye: Number of columns of a matrix should be positive. Found {m}");

      int    d  = n * m, i, j, k = 0;
      TNum[] nv = new TNum[d];

      for (i = 0; i < n; i++) {
        for (j = 0; j < m; j++, k++) {
          if (i == j) {
            nv[k] = Tools.One;
          }
          else {
            nv[k] = Tools.Zero;
          }
        }
      }

      return new Matrix(n, m, nv, false);
    }

    /// <summary>
    /// Generates a matrix with specified dimensions and values in range of [a,b).
    /// </summary>
    /// <param name="dimRow">The number of rows in the matrix.</param>
    /// <param name="dimCol">The number of columns in the matrix.</param>
    /// <param name="a">The lower bound of the range of values.</param>
    /// <param name="b">The upper bound of the range of values.</param>
    /// <param name="random">An optional instance of GRandomLC to use for generating random numbers. If null default one be used.</param>
    /// <returns>A Matrix with randomly generated elements between a and b.</returns>
    public static Matrix GenMatrix(int dimRow, int dimCol, TNum a, TNum b, GRandomLC? random = null) {
      Debug.Assert(dimRow > 0, $"Matrix.GenMatrix: Number of rows of a matrix should be positive. Found {dimRow}");
      Debug.Assert(dimCol > 0, $"Matrix.GenMatrix: Number of columns of a matrix should be positive. Found {dimCol}");

      GRandomLC rnd = random ?? Tools.Random;

      TNum[] nv = new TNum[dimRow * dimCol];
      int    k  = 0;

      for (int row = 0; row < dimRow; row++) {
        for (int col = 0; col < dimCol; col++) {
          nv[k] = rnd.NextPrecise(a, b);
          k++;
        }
      }

      return new Matrix(dimRow, dimCol, nv, false);
    }

    /// <summary>
    /// Generates a matrix with specified dimensions and integer values in range of [a,b).
    /// </summary>
    /// <param name="dimRow">The number of rows in the matrix.</param>
    /// <param name="dimCol">The number of columns in the matrix.</param>
    /// <param name="a">The lower bound of the range of values.</param>
    /// <param name="b">The upper bound of the range of values.</param>
    /// <param name="random">An optional instance of GRandomLC to use for generating random numbers. If null default one be used.</param>
    /// <returns>A Matrix with randomly generated elements between a and b.</returns>
    public static Matrix GenMatrixInt(int dimRow, int dimCol, int a, int b, GRandomLC? random = null) {
      Debug.Assert(dimRow > 0, $"Matrix.GenMatrix: Number of rows of a matrix should be positive. Found {dimRow}");
      Debug.Assert(dimCol > 0, $"Matrix.GenMatrix: Number of columns of a matrix should be positive. Found {dimCol}");

      GRandomLC rnd = random ?? Tools.Random;

      TNum[] nv = new TNum[dimRow * dimCol];
      int    k  = 0;

      for (int row = 0; row < dimRow; row++) {
        for (int col = 0; col < dimCol; col++) {
          nv[k] = TConv.FromInt(rnd.NextInt(a, b));
          k++;
        }
      }

      return new Matrix(dimRow, dimCol, nv, false);
    }


    /// <summary>
    /// Generates a non-singular square matrix with the specified dimension and values in range of [a,b).
    /// </summary>
    /// <param name="dim">The dimension of the square matrix.</param>
    /// <param name="a">The lower bound of the range of values.</param>
    /// <param name="b">The upper bound of the range of values.</param>
    /// <param name="random">Optional instance of GRandomLC to use for generating random numbers. If null default one be used.</param>
    /// <returns>A non-singular Matrix with randomly generated elements between a and b.</returns>
    public static Matrix GenNonSingular(int dim, TNum a, TNum b, GRandomLC? random = null) {
      Debug.Assert(dim > 0, $"Matrix.GenNonSingular: Size of a square matrix should be positive. Found {dim}");

      TNum[,]     m       = new TNum[dim, dim];
      LinearBasis toCheck = new LinearBasis(dim, 0);
      for (int r = 0; r < dim; r++) {
        TNum[] row;
        do {
          row = GenArray(dim, a, b, random);
        } while (!toCheck.AddVector(new Vector(row, false)));

        for (int l = 0; l < dim; l++) { m[r, l] = row[l]; }
      }

      return new Matrix(m);
    }

    /// <summary>
    /// Generates a non-singular square matrix with the specified dimension and values in range of [-0.5,0.5).
    /// </summary>
    /// <param name="dim">The dimension of the square matrix.</param>
    /// <param name="random">Optional instance of GRandomLC to use for generating random numbers. If null default one be used.</param>
    /// <returns>A non-singular Matrix.</returns>
    public static Matrix GenNonSingular(int dim, GRandomLC? random = null)
      => GenNonSingular(dim, -Tools.HalfOne, Tools.HalfOne, random);

    /// <summary>
    /// Generate orthonormal matrix.
    /// </summary>
    /// <param name="dim">The dimension n of the space.</param>
    /// <param name="random">The random to be used. If null, the Random be used.</param>
    /// <returns>The orthonormal matrix d x d.</returns>
    public static Matrix GenONMatrix(int dim, GRandomLC? random = null) {
      Debug.Assert(dim > 0, $"Matrix.GenONMatrix: Size of a square matrix should be positive. Found {dim}");

      return LinearBasis.GenLinearBasis(dim, random).Basis;
    }

    /// <summary>
    /// Constructs a Hilbert matrix of given dimension. [1, 1/2 , 1/3 , ...]
    /// </summary>
    /// <param name="dim">The dimension of the Hilbert matrix.</param>
    /// <returns>A Hilbert matrix of the specified dimension.</returns>
    public static Matrix Hilbert(int dim) {
      TNum[,] m = new TNum[dim, dim];
      for (int i = 0; i < dim; i++) {
        for (int j = 0; j < dim; j++) {
          m[i, j] = Tools.One / (TConv.FromInt(i) + TConv.FromInt(j) + Tools.One);
        }
      }

      return new Matrix(m);
    }
#endregion

#region Functions
    /// <summary>
    /// Multiplies a specified rowInd of the matrix by the given vector and returns the resulting scalar value.
    /// </summary>
    /// <param name="rowInd">The number of the rowInd to be multiplied.</param>
    /// <param name="v">The vector to multiply with the matrix rowInd.</param>
    /// <returns>The resulting scalar value.</returns>
    public TNum MultiplyRowByVector(int rowInd, Vector v) {
      Debug.Assert
        (
         Cols == v.SpaceDim
       , $"Matrix.MultiplyRowByVector: The column count of the matrix and the number of the coordinates of a vector should be the same. Found column count {Cols}, number of coordinates {v.SpaceDim}."
        );

      TNum res = Tools.Zero;
      for (int col = 0, k = rowInd * Cols; col < Cols; col++, k++) {
        res += _m[k] * v[col];
      }

      return res;
    }

    /// <summary>
    /// Multiplies a specified row of the matrix by the difference of two vectors (v1 - v2)
    /// and returns the resulting scalar value.
    /// </summary>
    /// <param name="rowInd">The index of the matrix row to be multiplied.</param>
    /// <param name="v1">The first vector in the difference (v1 - v2).</param>
    /// <param name="v2">The second vector in the difference (v1 - v2).</param>
    /// <returns>Returns the resulting scalar value.</returns>
    public TNum MultiplyRowByDiffOfVectors(int rowInd, Vector v1, Vector v2) {
      Debug.Assert
        (
         Cols == v1.SpaceDim
       , $"Matrix.MultiplyRowByVector: The column count of the matrix and the number of the coordinates of a vector should be the same. Found column count {Cols}, number of coordinates {v1.SpaceDim}."
        );

      TNum result = Tools.Zero;
      for (int col = 0, k = rowInd * Cols; col < Cols; col++, k++) {
        result += _m[k] * (v1[col] - v2[col]);
      }

      return result;
    }

    /// <summary>
    /// Multiplies a specified column of the matrix by the given vector and returns the resulting scalar value.
    /// </summary>
    /// <param name="colInd">The index of the column to be multiplied.</param>
    /// <param name="vector">The vector to multiply with the matrix column.</param>
    /// <returns>The resulting scalar value.</returns>
    public TNum MultiplyColumnByVector(int colInd, Vector vector) {
      Debug.Assert
        (
         Rows == vector.SpaceDim
       , $"Matrix.MultiplyColumnByVector: The row count of the matrix and the number of the coordinates of a vector should be the same. Found row count {Rows}, number of coordinates {vector.SpaceDim}."
        );

      TNum result = Tools.Zero;
      for (int row = 0, k = colInd; row < Rows; row++, k += Cols) {
        result += _m[k] * vector[row];
      }

      return result;
    }


    /// <summary>
    /// Multiplies the matrix by its transpose.
    /// </summary>
    /// <returns>A new matrix representing the product of the matrix and its transpose.</returns>
    public Matrix MultiplyBySelfTranspose() {
      TNum[] res = new TNum[Rows * Rows];

      int resInd  = 0; // заполняет верхний треугольник матрицы результата
      int resInd2 = 0; // заполняет нижний треугольник матрицы результата
      int s       = 0; // идёт в исходной матрице по строкам
      int t       = 0; // идёт в транспонированной матрице по столбцам (реально по строкам исходной)
      for (int row = 0; row < Rows; row++, resInd += row, resInd2 = resInd, s += Cols, t = s) {
        for (int col = row; col < Rows; col++, resInd++, resInd2 += Rows, s -= Cols) {
          TNum sum = Tools.Zero;
          for (int k = 0; k < Cols; k++) {
            sum += _m[s] * _m[t];
            s++;
            t++;
          }
          res[resInd]  = sum;
          res[resInd2] = sum;
        }
      }

      return new Matrix(Rows, Rows, res, false);
    }

    /// <summary>
    /// Computes the product of the transpose of this matrix and the matrix itself (A^T * A).
    /// The result is a symmetric matrix of size <c>Cols</c> x <c>Cols</c>.
    /// </summary>
    /// <returns>A new matrix representing the product of the transpose and the matrix.</returns>
    public Matrix MultiplyTransposeBySelf() {
      throw new NotImplementedException();

      TNum[] res = new TNum[Cols * Cols];

      // Итерируем по верхнему треугольнику результирующей матрицы C[i, j], где i <= j
      // i соответствует i-му столбцу исходной матрицы A
      // j соответствует j-му столбцу исходной матрицы A
      for (int i = 0; i < Cols; i++) {
        for (int j = i; j < Cols; j++) { }
      }

      // Возвращаем новую матрицу размера Cols x Cols с полученными данными
      return new Matrix(Cols, Cols, res, false);
    }

    /// <summary>
    /// Multiplies the transposed matrix by a given vector.
    /// </summary>
    /// <param name="vector">The vector to multiply by the transposed matrix.</param>
    /// <returns>The result of multiplying the transposed matrix by the vector.</returns>
    public Vector MultiplyTransposedByVector(Vector vector) {
      Debug.Assert
        (
         vector.SpaceDim == Rows
       , $"MultiplyTransposedByVector: The vector dimension must match the number of rows in the matrix. Found vector.SpaceDim = {vector.SpaceDim}, Rows = {Rows}."
        );

      TNum[] res = new TNum[Cols];
      int    t   = 0;
      for (int col = 0; col < Cols; col++, t = col) {
        res[col] = Tools.Zero;
        for (int row = 0; row < Rows; row++, t += Cols) {
          res[col] += _m[t] * vector[row];
        }
      }

      return new Vector(res, false);
    }

    /// <summary>
    /// Computes the Reduced Row Echelon Form (RREF) of the matrix using Gauss-Jordan elimination with partial pivoting.
    /// </summary>
    /// <returns>A new matrix in RREF (each leading entry is 1, and all other entries in its column are zero).</returns>
    public Matrix ToRREF() {
      TNum[,] rrefData = this;
      int     pivotRow = 0; // Текущая ведущая строка

      for (int leadCol = 0; leadCol < Cols && pivotRow < Rows; leadCol++) {
        int maxRow = pivotRow;

        // Находим строку с максимальным по модулю элементом в текущем столбце
        TNum maxValAbs = TNum.Abs(rrefData[maxRow, leadCol]);
        for (int currentRow = pivotRow + 1; currentRow < Rows; currentRow++) {
          TNum currentValAbs = TNum.Abs(rrefData[currentRow, leadCol]);
          if (currentValAbs > maxValAbs) {
            maxValAbs   = currentValAbs;
            maxRow = currentRow;
          }
        }

        // Если максимальный элемент в столбце (ниже или на leadRow) близок к нулю,
        // то в этом столбце нет ведущего элемента, переходим к следующему столбцу.
        if (Tools.EQ(maxValAbs)) { continue; }

        // Меняем местами текущую ведущую строку и строку с найденным pivot-ом
        if (maxRow != pivotRow) {
          for (int j = 0; j < Cols; j++) {
            Tools.Swap(ref rrefData[pivotRow, j], ref rrefData[maxRow, j]);
          }
        }

        TNum maxVal = rrefData[pivotRow, leadCol];
        if (Tools.NE(maxVal, Tools.One)) { // чтоб на единицу не делить
          for (int j = leadCol; j < Cols; j++) {
            rrefData[pivotRow, j] /= maxVal;
          }
        }
        rrefData[pivotRow, leadCol] = Tools.One; // Явно устанавливаем значение 1 для точности

        for (int i = 0; i < Rows; i++) {
          if (i != pivotRow) {
            TNum factor = rrefData[i, leadCol];
            if (Tools.NE(factor)) {
              for (int j = leadCol; j < Cols; j++) {
                rrefData[i, j] -= factor * rrefData[pivotRow, j];
              }
              rrefData[i, leadCol] = Tools.Zero;// Явно устанавливаем значение 0 для точности
            }
          }
        }

        // Переходим к следующей ведущей строке
        pivotRow++;
      }

      return new Matrix(rrefData);
    }
#endregion

  }

  // /// <summary>
  // /// Represents a mutable matrix that allows modification of its elements.
  // /// This class extends the base <see cref="Matrix"/> class, providing additional functionality
  // /// for modifying matrix elements, including setting submatrices.
  // /// </summary>
  // public class MutableMatrix : Matrix {
  //
  //   /// <summary>
  //   /// Constructs a mutable matrix from the given dimensions and an array of elements.
  //   /// </summary>
  //   /// <param name="n">Number of rows in the matrix.</param>
  //   /// <param name="m">Number of columns in the matrix.</param>
  //   /// <param name="ar">Array of elements in row-major order to initialize the matrix.</param>
  //   public MutableMatrix(int n, int m, TNum[] ar) : base(n, m, ar, false) { }
  //
  //   /// <summary>
  //   /// Constructs a mutable matrix by copying the elements from an existing matrix.
  //   /// </summary>
  //   /// <param name="m">The matrix to copy.</param>
  //   public MutableMatrix(Matrix m) : base(m) { }
  //
  //   /// <summary>
  //   /// Indexer to get or set the matrix element at the specified row and column.
  //   /// </summary>
  //   /// <param name="i">The row index (zero-based).</param>
  //   /// <param name="j">The column index (zero-based).</param>
  //   /// <returns>The value of the matrix element at the specified row and column.</returns>
  //   public new TNum this[int i, int j] {
  //     get
  //       {
  //         Debug.Assert
  //           (i >= 0 && i < Rows, $"MutableMatrix.Indexer: The first index must be in the range [0, {Rows}). Found: {i}");
  //         Debug.Assert
  //           (j >= 0 && j < Cols, $"MutableMatrix.Indexer: The second index must be in the range [0, {Cols}). Found: {j}");
  //
  //         return _m[i * Cols + j];
  //       }
  //     set
  //       {
  //         Debug.Assert
  //           (i >= 0 && i < Rows, $"MutableMatrix.Indexer: The first index must be in the range [0, {Rows}). Found: {i}");
  //         Debug.Assert
  //           (j >= 0 && j < Cols, $"MutableMatrix.Indexer: The second index must be in the range [0, {Cols}). Found: {j}");
  //
  //         _m[i * Cols + j] = value;
  //       }
  //   }
  //
  //   /// <summary>
  //   /// Creates an identity matrix of size <paramref name="d"/> x <paramref name="d"/>.
  //   /// </summary>
  //   /// <param name="d">The size of the identity matrix (number of rows and columns).</param>
  //   /// <returns>A new identity matrix.</returns>
  //   public new static MutableMatrix Eye(int d) {
  //     int    k  = 0;
  //     TNum[] nv = new TNum[d * d];
  //
  //     for (int i = 0; i < d; i++) {
  //       for (int j = 0; j < d; j++, k++) {
  //         nv[k] = (i == j) ? Tools.One : Tools.Zero;
  //       }
  //     }
  //
  //     return new MutableMatrix(d, d, nv);
  //   }
  //
  //   /// <summary>
  //   /// Sets a submatrix within the current matrix at the specified starting row and column.
  //   /// </summary>
  //   /// <param name="startRow">The starting row index for the submatrix.</param>
  //   /// <param name="startCol">The starting column index for the submatrix.</param>
  //   /// <param name="subMatrix">The submatrix to be inserted.</param>
  //   /// <remarks>The size of the submatrix must fit within the bounds of the main matrix.</remarks>
  //   public void SetSubMatrix(int startRow, int startCol, Matrix subMatrix) {
  //     Debug.Assert
  //       (
  //        startRow >= 0 && startRow + subMatrix.Rows <= Rows
  //      , $"MutableMatrix.SetSubMatrix: The submatrix row range must fit within the matrix. Matrix row count: {Rows}, submatrix row count: {subMatrix.Rows}, starting at row {startRow}."
  //       );
  //     Debug.Assert
  //       (
  //        startCol >= 0 && startCol + subMatrix.Cols <= Cols
  //      , $"MutableMatrix.SetSubMatrix: The submatrix column range must fit within the matrix. Matrix column count: {Cols}, submatrix column count: {subMatrix.Cols}, starting at column {startCol}."
  //       );
  //
  //     int k     = startRow * Cols + startCol;
  //     int s     = 0;
  //     int shift = Cols - subMatrix.Cols;
  //     for (int row = 0; row < subMatrix.Rows; row++, k += shift) {
  //       for (int col = 0; col < subMatrix.Cols; col++, s++, k++) {
  //         _m[k] = subMatrix[s];
  //       }
  //     }
  //   }
  //
  //   /// <summary>
  //   /// Sets a subvector in the matrix starting from the specified startRow and column.
  //   /// </summary>
  //   /// <param name="startRow">The starting startRow index.</param>
  //   /// <param name="startCol">The column where the vector will be inserted.</param>
  //   /// <param name="vector">The vector to insert into the matrix.</param>
  //   public void SetSubVector(int startRow, int startCol, Vector vector) {
  //     Debug.Assert(startRow + vector.SpaceDim <= Rows, "SetSubVector: The vector does not fit into the matrix vertically.");
  //     Debug.Assert(startCol < Cols, "SetSubVector: The column index is out of bounds.");
  //
  //     int k = startRow * Cols + startCol;
  //     for (int i = 0; i < vector.SpaceDim; i++, k+=Cols) {
  //       _m[k] = vector[i];
  //     }
  //   }
  //
  //   /// <summary>
  //   /// Multiplies two mutable matrices and returns the result as a new mutable matrix.
  //   /// </summary>
  //   /// <param name="m1">The first matrix.</param>
  //   /// <param name="m2">The second matrix.</param>
  //   /// <returns>A new matrix that is the product of <paramref name="m1"/> and <paramref name="m2"/>.</returns>
  //   public static MutableMatrix operator *(MutableMatrix m1, MutableMatrix m2) => new((Matrix)m1 * (Matrix)m2);
  //
  // }

}
