using System.Text;

namespace CGLibrary;

public partial class Geometry<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
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
    protected readonly TNum[] _m;

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
          Debug.Assert(i >= 0 && i < Rows, "Matrix.Indexer: The first index is out of range");
          Debug.Assert(j >= 0 && j < Cols, "Matrix.Indexer: The second index is out of range");

          return _m[i * Cols + j];
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
      Debug.Assert(n > 0 && m > 0, "Matrix.Ctor: Dimension of a matrix cannot be non-positive");

      _m   = new TNum[n * m];
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
    /// <param name="needCopy">Indicates whether a copy of the array should be made. If <c>true</c>, a copy is made; otherwise, the original array is used directly.</param>
    public Matrix(int n, int m, TNum[] ar, bool needCopy) {
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
    /// Constructor on the basis of a two-dimensional array
    /// </summary>
    /// <param name="nm">The new array</param>
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

    /// <summary>
    /// Construct the matrix based on a given vector.
    /// </summary>
    /// <param name="v">The vector used to construct the matrix.</param>
    public Matrix(Vector v) {
      Rows = v.Dim;
      Cols = 1;
      _m   = v.GetAsArray();
    }
#endregion

#region Overrides
    public override int GetHashCode() => throw new InvalidOperationException(); //HashCode.Combine(Rows, Cols);

    public override bool Equals(object? obj) {
#if DEBUG
      if (obj is not Matrix matrix) {
        throw new ArgumentException($"{obj} is not a Matrix.");
      }
#endif
      return Equals((Matrix)obj!);
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


    // public override string ToString() {
    //   string res = "{";
    //   int    k   = 0, i, j;
    //
    //   for (i = 0; i < Rows; i++) {
    //     res += $"{{{_m[k].ToString(null, CultureInfo.InvariantCulture)}";
    //     k++;
    //
    //     for (j = 1; j < Cols; j++) {
    //       res += $", {_m[k].ToString(null, CultureInfo.InvariantCulture)}";
    //       k++;
    //     }
    //
    //     res += "}";
    //     if (i < Rows - 1) {
    //       res += ",";
    //     }
    //   }
    //
    //   res += "}";
    //
    //   return res;
    // }

    public override string ToString() {
      // Вычисляем максимальную длину числа в каждом столбце
      var maxLengths = new int[Cols];
      for (int i = 0; i < _m.Length; i++) {
        int colIndex = i % Cols;
        maxLengths[colIndex] = Math.Max(maxLengths[colIndex], _m[i].ToString()!.Length);
      }

      // Формируем строку вывода
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
    /// Unary minus - the opposite matrix
    /// </summary>
    /// <param name="m">The matrix to be reversed</param>
    /// <returns>The opposite vector</returns>
    public static Matrix operator -(Matrix m) {
      int    d  = m.Rows * m.Cols, i;
      TNum[] nv = new TNum[d];

      for (i = 0; i < d; i++) {
        nv[i] = -m._m[i];
      }

      return new Matrix(m.Rows, m.Cols, nv, false);
    }

    /// <summary>
    /// Sum of two matrices
    /// </summary>
    /// <param name="m1">The first matrix summand</param>
    /// <param name="m2">The second matrix summand</param>
    /// <returns>The sum</returns>
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
    /// Difference of two vectors
    /// </summary>
    /// <param name="m1">The matrix minuend</param>
    /// <param name="m2">The matrix subtrahend</param>
    /// <returns>The difference</returns>
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
    /// Left multiplication of a matrix by a number
    /// </summary>
    /// <param name="a">The numeric factor</param>
    /// <param name="m">The matrix factor</param>
    /// <returns>The product</returns>
    public static Matrix operator *(TNum a, Matrix m) {
      int    d  = m.Rows * m.Cols, i;
      TNum[] nv = new TNum[d];

      for (i = 0; i < d; i++) {
        nv[i] = a * m._m[i];
      }

      return new Matrix(m.Rows, m.Cols, nv, false);
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
    /// are considered as a column vectors
    /// </summary>
    /// <param name="m">The matrix (first) factor</param>
    /// <param name="v">The vector (second) factor</param>
    /// <returns>The resultant vector</returns>
    public static Vector operator *(Matrix m, Vector v) {
      Debug.Assert
        (
         m.Cols == v.Dim
       , $"Matrix.*: Cannot multiply a matrix and a vector of improper dimensions. Matrix columns: {m.Cols}, vector dimensions: {v.Dim}"
        );

      TNum[] res = new TNum[m.Rows];
      int    r   = m.Rows, c = m.Cols, i, j, k = 0;

      for (i = 0; i < r; i++) {
        for (j = 0; j < c; j++, k++) {
          res[i] += m._m[k] * v[j];
        }
      }

      return new Vector(res, false);
    }

    /// <summary>
    /// Multiplication of a matrix by a vector at left. The vector factor and the result
    /// are considered as row vectors
    /// </summary>
    /// <param name="v">The vector (first) factor</param>
    /// <param name="m">The matrix (second) factor</param>
    /// <returns>The resultant vector</returns>
    public static Vector operator *(Vector v, Matrix m) {
      Debug.Assert
        (
         m.Rows == v.Dim
       , $"Matrix.*: Cannot multiply a vector and a matrix of improper dimensions. Matrix rows: {m.Rows}, vector dimensions: {v.Dim}"
        );

      TNum[] res = new TNum[m.Cols];
      int    r   = m.Rows, c = m.Cols, i, j, k;

      for (i = 0; i < c; i++) {
        for (j = 0, k = i; j < r; j++, k += c) {
          res[i] += m._m[k] * v[j];
        }
      }

      return new Vector(res, false);
    }

    /// <summary>
    /// Multiplication of two matrices
    /// </summary>
    /// <param name="m1">The first matrix factor</param>
    /// <param name="m2">The second matrix factor</param>
    /// <returns>The resultant matrix</returns>
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

    // /// <summary>
    // /// Horizontal concatenation of matrix and a vector (with equal number of rows).
    // /// </summary>
    // /// <param name="m">The left concatenated matrix.</param>
    // /// <param name="v">The right concatenated vector.</param>
    // /// <returns>The resultant matrix.</returns>
    // public static Matrix hcat(Matrix? m, Vector v) => m is null ? new Matrix(v) : hcat(m, new Matrix(v))!;
    // // TODO: Сделать без вспомогательного создания промежуточной матрицы!

    /// <summary>
    /// Horizontal concatenation of matrix and a vector (with equal number of rows).
    /// </summary>
    /// <param name="m">The left concatenated matrix.</param>
    /// <param name="v">The right concatenated vector.</param>
    /// <returns>The resultant matrix.</returns>
    public static Matrix hcat(Matrix? m, Vector v) {
      if (m is null) {
        return new Matrix(v);
      }

      Debug.Assert
        (
         m.Rows == v.Dim
       , $"Matrix.hcat: Cannot concatenate matrix and vector with different number of rows. Matrix rows: {m.Rows}, Vector dimension: {v.Dim}"
        );

      int    r  = m.Rows, c1 = m.Cols, c = c1 + 1, d = r * c, k = 0, k1 = 0;
      TNum[] nv = new TNum[d];

      for (int i = 0; i < r; i++) {
        for (int j = 0; j < c1; j++, k++, k1++) {
          nv[k] = m._m[k1];
        }
        nv[k] = v[i]; // m.Rows == v.Dim
        k++;
      }

      return new Matrix(r, c, nv, false);
    }

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
    /// Construct a submatrix consisting of given rows of the original matrix
    /// </summary>
    /// <param name="rows">List of row indices to be taken</param>
    /// <returns>The resultant matrix</returns>
    public Matrix TakeRows(int[] rows) {
      int r = rows.Length;

      Debug.Assert(r > 0, $"Matrix.TakeRows: Wrong number of rows to be taken. Found {r}");

      int    c   = Cols, d = r * c, k = 0, ind, i, j;
      TNum[] res = new TNum[d];

      for (i = 0; i < rows.Length; i++) {
        for (j = 0, ind = rows[i] * c; j < c; j++, ind++, k++) {
          res[k] = _m[ind];
        }
      }

      return new Matrix(r, c, res, false);
    }

    /// <summary>
    /// Construct a submatrix consisting of given columns of the original matrix
    /// </summary>
    /// <param name="cols">List of column indices to be taken</param>
    /// <returns>The resultant matrix</returns>
    public Matrix TakeCols(int[] cols) {
      int c = cols.Length;

      Debug.Assert(c > 0, $"Matrix.TakeCols: Wrong number of columns to be taken. Found {c}");

      int    r   = Rows, d = r * c, k = 0, start, i, j;
      TNum[] res = new TNum[d];

      for (i = 0, start = 0; i < r; i++, start += Cols) {
        for (j = 0; j < c; j++, k++) {
          res[k] = _m[start + cols[j]];
        }
      }

      return new Matrix(r, c, res, false);
    }

    /// <summary>
    /// Construct a submatrix consisting of elements at crossing of given rows and columns of the original matrix
    /// </summary>
    /// <param name="rows">List of row indices to be taken. If null, all row indices be taken.</param>
    /// <param name="cols">List of column indices to be taken. If null, all column indices be taken.</param>
    /// <returns>The resultant matrix</returns>
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

    public Vector TakeVector(int col) {
      Debug.Assert(col >= 0 && col < Cols, "Matrix.TakeVector: Column index must lie in [0, Cols).");

      TNum[] res = new TNum[Rows];
      for (int i = 0; i < Rows; i++) {
        res[i] = this[i, col];
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
        int k = col;
        for (int row = 0; row < Rows; row++) {
          tm[kt] =  _m[k];
          k      += Cols;
          kt++;
        }
      }

      return new Matrix(Cols, Rows, tm, false);
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
      Debug.Assert(n > 0, $"Matrix.Zero: Number of rows of a matrix should be positive. Found {n}");
      Debug.Assert(m > 0, $"Matrix.Zero: Number of columns of a matrix should be positive. Found {m}");

      return new Matrix(n, m);
    }

    /// <summary>
    /// Return zero matrix n-by-n containing units
    /// </summary>
    /// <param name="n">Size of the matrix</param>
    /// <returns>The resultant matrix</returns>
    public static Matrix One(int n) {
      Debug.Assert(n > 0, $"Matrix.One: Size of a square matrix should be positive. Found {n}");

      return One(n, n);
    }

    /// <summary>
    /// Return rectangular matrix n-by-m containing units
    /// </summary>
    /// <param name="n">Number of rows</param>
    /// <param name="m">Number of columns</param>
    /// <returns>The resultant matrix</returns>
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
    /// Return unit square matrix n-by-n
    /// </summary>
    /// <param name="n">Size of the matrix</param>
    /// <returns>The resultant matrix</returns>
    public static Matrix Eye(int n) {
      Debug.Assert(n > 0, $"Matrix.Eye: Size of a square matrix should be positive. Found {n}");

      return Eye(n, n);
    }

    /// <summary>
    /// Return rectangular unite matrix n-by-m
    /// </summary>
    /// <param name="n">Number of rows</param>
    /// <param name="m">Number of columns</param>
    /// <returns>The resultant matrix</returns>
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
    /// <param name="dim">The dimension d of the space.</param>
    /// <param name="random">The random to be used. If null, the Random be used.</param>
    /// <returns>The orthonormal matrix d x d.</returns>
    public static Matrix GenONMatrix(int dim, GRandomLC? random = null) {
      Debug.Assert(dim > 0, $"Matrix.GenONMatrix: Size of a square matrix should be positive. Found {dim}");

      return LinearBasis.GenLinearBasis(dim, random).Basis!;
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
    /// Projects the given vector onto the subspace defined by the columns of the current matrix.
    /// </summary>
    /// <param name="v">The vector to project.</param>
    /// <returns>A new vector representing the projection of given vector onto the subspace.</returns>
      public Vector ProjectOntoSubspace(Vector v) {
      TNum[] proj = new TNum[Rows];

      for (int j = 0; j < Cols; j++) {
        TNum dotProduct = Tools.Zero;
        int  colIndex   = j;
        for (int i = 0; i < Rows; i++) {
          dotProduct += v[i] * _m[colIndex];
          colIndex   += Cols;
        }
        colIndex = j;
        for (int i = 0; i < Rows; i++) {
          proj[i]  += dotProduct * _m[colIndex];
          colIndex += Cols;
        }
      }

      return new Vector(proj, false);
    }
#endregion

  }

  public class MutableMatrix : Matrix {

    public MutableMatrix(int n, int m, TNum[] ar) : base(n, m, ar, false) { }

    public MutableMatrix(Matrix m) : base(m) { }

    public new TNum this[int i, int j] {
      get
        {
          Debug.Assert(i >= 0 && i < Rows, "MutableMatrix.Indexer: The first index is out of range");
          Debug.Assert(j >= 0 && j < Cols, "MutableMatrix.Indexer: The second index is out of range");

          return _m[i * Cols + j];
        }

      set
        {
          Debug.Assert(i >= 0 && i < Rows, "MutableMatrix.Indexer: The first index is out of range");
          Debug.Assert(j >= 0 && j < Cols, "MutableMatrix.Indexer: The second index is out of range");

          _m[i * Cols + j] = value;
        }
    }

    public new static MutableMatrix Eye(int d) {
      int    i, j, k = 0;
      TNum[] nv      = new TNum[d * d];

      for (i = 0; i < d; i++) {
        for (j = 0; j < d; j++, k++) {
          if (i == j) {
            nv[k] = Tools.One;
          }
          else {
            nv[k] = Tools.Zero;
          }
        }
      }

      return new MutableMatrix(d, d, nv);
    }

    public void SetSubMatrix(int startRow, int startCol, int numRows, int numCols, Matrix subMatrix) {
      Debug.Assert
        (
         numRows == subMatrix.Rows && numCols == subMatrix.Cols
       , $"MutableMatrix.SetSubMatrix: Submatrix dimensions must match the specified region dimensions. Expected dimensions: {numRows}x{numCols}, found: {subMatrix.Rows}x{subMatrix.Cols}"
        );

      for (int i = 0; i < numRows; i++) {
        for (int j = 0; j < numCols; j++) {
          _m[(startRow + i) * Cols + (startCol + j)] = subMatrix[i, j];
        }
      }
    }

    public static MutableMatrix operator *(MutableMatrix m1, MutableMatrix m2) => new((Matrix)m1 * (Matrix)m2);

  }

}
