using System.Diagnostics.CodeAnalysis;

namespace CGLibrary;

public partial class Geometry<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  /// <summary>
  /// Provides functionality for solving systems of linear equations using Gaussian elimination.
  /// It can find an unique solution for square (n x n) and overdetermined (m x n, m > n) systems.
  /// For underdetermined systems (m &lt; n) or systems with no or infinite solutions, it reports failure.
  /// </summary>
  public class GaussSLE {

    /// <summary>
    /// Defines options for selecting pivot elements during Gaussian elimination.
    /// </summary>
    public enum GaussChoice {

      No      // No specific choice made.
    , RowWise // Choose pivots in a row-wise manner.
    , ColWise // Choose pivots in a column-wise manner.
    , All     // Choose pivots from the entire matrix.

    }

#region Fields
    private readonly int         _row;
    private readonly int         _col;
    private readonly TNum[,]     _A;
    private readonly TNum[]      _b;
    private readonly TNum[]?     _result;
    private          GaussChoice _gaussChoice;

    private readonly int[] _indARow;
    private readonly int[] _indACol;
    private readonly int[] _indB;
    /// <summary>
    /// Indicates whether the last call to <see cref="Solve"/> found a unique solution.
    /// </summary>
    private          bool  isSuccess;
#endregion

#region Constructors
    /// <summary>
    /// Constructs a new instance of GaussSLE with the specified dimensions.
    /// </summary>
    /// <param name="row">The number of rows in the matrix A.</param>
    /// <param name="col">The number of columns in the matrix A.</param>
    public GaussSLE(int row, int col) {
      _row     = row;
      _col     = col;
      _A       = new TNum[row, col];
      _b       = new TNum[row];
      _result  = new TNum[col];
      _indARow = new int[row];
      _indACol = new int[col];
      _indB    = new int[row];
    }


    /// <summary>
    /// Constructs a new instance of GaussSLE with the specified dimensions, functions for generating the matrix A and vector b, and a choice for pivot selection.
    /// </summary>
    /// <param name="AFunc">Function provides the coefficients of the matrix A.</param>
    /// <param name="bFunc">Function provides the right side vector b.</param>
    /// <param name="row">The number of rows in the matrix A.</param>
    /// <param name="col">The number of columns in the matrix A.</param>
    /// <param name="gaussChoice">Optional choice for pivot selection during Gaussian elimination.</param>
    public GaussSLE(
        Func<int, int, TNum> AFunc
      , Func<int, TNum>      bFunc
      , int                  row
      , int                  col
      , GaussChoice          gaussChoice = GaussChoice.All
      ) : this(row, col) {
      SetSystem(AFunc, bFunc, row, col, gaussChoice);
    }

    /// <summary>
    /// Constructs a new instance of GaussSLE with the specified dimensions, functions for generating the matrix A and vector b, and a choice for pivot selection.
    /// </summary>
    /// <param name="A">The coefficient matrix A.</param>
    /// <param name="b">The right side vector b.</param>
    /// <param name="gaussChoice">Optional choice for pivot selection during Gaussian elimination.</param>
    public GaussSLE(TNum[,] A, TNum[] b, GaussChoice gaussChoice = GaussChoice.All) : this(A.GetLength(0), A.GetLength(1)) {
      SetSystem(A, b, gaussChoice);
    }
#endregion

#region Methods
    /// <summary>
    /// Sets the choice for pivot selection.
    /// </summary>
    /// <param name="gaussChoice">Choice for pivot selection during Gaussian elimination.</param>
    public void SetGaussChoice(GaussChoice gaussChoice) { _gaussChoice = gaussChoice; }

    /// <summary>
    /// Sets the system of linear equations to be solved by this instance.
    /// </summary>
    /// <param name="AFunc">Function provides the coefficients of the matrix A.</param>
    /// <param name="bFunc">Function provides the right side vector b.</param>
    /// <param name="row">The number of rows in the matrix A.</param>
    /// <param name="col">The number of columns in the matrix A.</param>
    /// <param name="gaussChoice">Choice for pivot selection during Gaussian elimination.</param>
    public void SetSystem(
        Func<int, int, TNum> AFunc
      , Func<int, TNum>      bFunc
      , int                  row
      , int                  col
      , GaussChoice          gaussChoice = GaussChoice.All
      ) {
      _gaussChoice = gaussChoice;
      for (int r = 0; r < _row; r++) {
        for (int l = 0; l < _col; l++) {
          _A[r, l] = AFunc(r, l);
        }
        _b[r] = bFunc(r);
      }
    }

    /// <summary>
    /// Sets the system of linear equations to be solved by this instance using provided matrix A and vector b.
    /// </summary>
    /// <param name="A">The matrix A representing the system of linear equations.</param>
    /// <param name="b">The vector b representing the right-hand side of the system.</param>
    /// <param name="gaussChoice">Choice for pivot selection during Gaussian elimination.</param>
    private void SetSystem(TNum[,] A, TNum[] b, GaussChoice gaussChoice = GaussChoice.All) {
      Debug.Assert
        (
         A.GetLength(0) == _row
       , $"The amount of rows in A must be equal to initial parameter row. Found {A.GetLength(0)} row = {_row}"
        );
      Debug.Assert
        (
         A.GetLength(1) == _col
       , $"The amount of columns in A must be equal to initial parameter col. Found {A.GetLength(1)} row = {_col}"
        );
      Debug.Assert
        (b.Length == _row, $"The amount of rows in b must be equal to initial parameter row. Found {b.Length} row = {_row}");
      _gaussChoice = gaussChoice;

      for (int r = 0; r < _row; r++) {
        for (int l = 0; l < _col; l++) {
          _A[r, l] = A[r, l];
        }
        _b[r] = b[r];
      }
    }

    /// <summary>
    /// Solves the system of linear equations using Gaussian elimination.
    /// The method can find a unique solution for square (n x n) and overdetermined (m x n, m > n) systems.
    /// </summary>
    public void Solve() {
      isSuccess = false; // По умолчанию считаем, что решения нет

      // Система, в которой уравнений меньше, чем неизвестных (m < n),
      // не может иметь единственного решения.
      if (_row < _col) {
        return;
      }

      // Инициализация индексных массивов
      for (int i = 0; i < _row; i++) {
        _indARow[i] = i;
        _indB[i]    = i;
      }
      for (int i = 0; i < _col; i++) {
        _indACol[i] = i;
      }

      // --- Прямой ход Гаусса ---
      for (int k = 0; k < _col; k++) { // последний элемент будем обрабатывать отдельно
        int  maxRowWiseInd = k;
        int  maxColWiseInd = k;
        TNum absMaxEl      = Tools.Abs(_A[_indARow[k], _indACol[k]]);

        if (_gaussChoice != GaussChoice.No) {
          int lcol = k, rcol = _col - 1, rrow = _row - 1, lrow = k;
          switch (_gaussChoice) {
            case GaussChoice.RowWise: lcol = rcol = k; break;
            case GaussChoice.ColWise: lrow = rrow = k; break;
            case GaussChoice.All:     break; // Область поиска уже установлена [k.._row-1, k.._col-1]
          }

          for (int i = lrow; i <= rrow; i++) {
            for (int j = lcol; j <= rcol; j++) {
              TNum curAbs = Tools.Abs(_A[_indARow[i], _indACol[j]]);
              if (curAbs > absMaxEl) { // ищем самый большой, честно
                absMaxEl      = curAbs;
                maxRowWiseInd = i;
                maxColWiseInd = j;
              }
            }
          }

          // Перестановка индексов строк и столбцов для использования пивота
          Tools.Swap(ref _indB[k], ref _indB[maxRowWiseInd]);
          Tools.Swap(ref _indARow[k], ref _indARow[maxRowWiseInd]);
          Tools.Swap(ref _indACol[k], ref _indACol[maxColWiseInd]);
        }
        // Если максимальный элемент в оставшейся подматрице равен нулю,
        // то ранг матрицы меньше n. Единственного решения нет.
        if (Tools.EQ(absMaxEl)) {
          return; // isSuccess остается false
        }

        // Нормировка k-й строки
        TNum pivotInv = TNum.One / _A[_indARow[k], _indACol[k]];
        for (int i = k + 1; i < _row; i++) { // Обнуление элементов под пивотом в k-м столбце
          TNum t = _A[_indARow[i], _indACol[k]] * pivotInv;
          _b[_indB[i]] -= t * _b[_indB[k]];
          for (int j = k; j < _col; j++) {
            _A[_indARow[i], _indACol[j]] -= t * _A[_indARow[k], _indACol[j]];
          }
        }
      }

      // --- Проверка на совместность для переопределенных систем (m > n) ---
      // После прямого хода в нижних (m - n) строках матрицы A должны быть нули.
      // Проверяем, равны ли нулю соответствующие элементы вектора b.
      for (int i = _col; i < _row; i++) {
        if (Tools.NE(_b[_indB[i]])) { // Найдено уравнение вида 0 = c, где c != 0. Система несовместна.
          return;                     // isSuccess остается false
        }
      }

      // --- Обратный ход Гаусса ---
      // Решаем систему, которая теперь является квадратной n x n.
      for (int k = _col - 1; k >= 0; k--) {
        TNum sum = Tools.Zero;
        for (int i = k + 1; i < _col; i++) {
          sum += _A[_indARow[k], _indACol[i]] * _result![_indACol[i]];
        }
        _result![_indACol[k]] = (_b[_indB[k]] - sum) / _A[_indARow[k], _indACol[k]];
      }

      isSuccess = true; // Если мы дошли до сюда, значит, уникальное решение найдено.
    }

    /// <summary>
    /// Retrieves the solution of the system of linear equations as an array of numbers.
    /// </summary>
    /// <param name="result">Output parameter that receives the solution vector.</param>
    /// <returns><c>True</c> if the system has a unique solution, otherwise <c>false</c>.</returns>
    public bool GetSolution([NotNullWhen(true)] out TNum[]? result) {
      result = isSuccess is false ? null : _result;

      return isSuccess;
    }

    /// <summary>
    /// Retrieves the solution of the system of linear equations as a Vector.
    /// </summary>
    /// <param name="result">Output parameter that receives the solution point.</param>
    /// <returns><c>True</c> if the system has a unique solution, otherwise <c>false</c>.</returns>
    public bool GetSolution([NotNullWhen(true)] out Vector? result) {
      result = isSuccess is false ? null : new Vector(_result!);

      return isSuccess;
    }
#endregion

#region Factories
    /// <summary>
    /// Solves a system of linear equations defined by generator functions.
    /// Finds a unique solution if one exists for a square (n x n) or overdetermined (m x n, m > n) system.
    /// </summary>
    /// <param name="AFunc">A function that provides the coefficients of the m x n matrix A, taking (row, column) indices.</param>
    /// <param name="bFunc">A function that provides the elements of the m-element right-side vector b, taking a row index.</param>
    /// <param name="rows">The number of rows in the system (equations).</param>
    /// <param name="cols">The number of columns in the system (variables).</param>
    /// <param name="gaussChoice">Specifies the strategy for choosing pivot elements.</param>
    /// <param name="result">Output parameter that receives the n-element solution vector if it is unique.</param>
    /// <returns><c>True</c> if the system has a unique solution, otherwise <c>false</c>.</returns>
    public static bool Solve(
        Func<int, int, TNum>            AFunc
      , Func<int, TNum>                 bFunc
      , int                             rows
      , int                             cols
      , GaussChoice                     gaussChoice
      , [NotNullWhen(true)] out TNum[]? result
      ) {
      GaussSLE gaussSLE = new GaussSLE(AFunc, bFunc, rows, cols, gaussChoice);
      gaussSLE.Solve();

      return gaussSLE.GetSolution(out result);
    }

    /// <summary>
    /// Solves a system of linear equations represented by a rectangular matrix.
    /// Finds a unique solution if one exists for a square (n x n) or overdetermined (m x n, m > n) system.
    /// </summary>
    /// <param name="A">The m x n coefficient matrix A.</param>
    /// <param name="b">The m-element right side vector b.</param>
    /// <param name="gaussChoice">Specifies the strategy for choosing pivot elements.</param>
    /// <param name="result">Output parameter that receives the n-element solution vector if it is unique.</param>
    /// <returns><c>True</c> if the system has a unique solution, otherwise <c>false</c>.</returns>
    public static bool Solve(
        TNum[,]                         A
      , TNum[]                          b
      , GaussChoice                     gaussChoice
      , [NotNullWhen(true)] out TNum[]? result
      ) {
      int rows = A.GetLength(0);

      if (rows != b.Length) {
        throw new ArgumentException("The number of rows in matrix A must match the length of vector b.");
      }

      GaussSLE gaussSLE = new GaussSLE((TNum[,])A.Clone(), (TNum[])b.Clone(), gaussChoice);
      gaussSLE.Solve();

      return gaussSLE.GetSolution(out result);
    }

    /// <summary>
    /// Finds the unique intersection point of a set of hyperplanes by solving the system of linear equations they represent.
    /// A unique solution can be found if the system is square (number of hyperplanes equals space dimension) or
    /// overdetermined (more hyperplanes than the space dimension) but consistent.
    /// </summary>
    /// <param name="HPs">A collection of hyperplanes whose intersection is to be found.</param>
    /// <param name="gaussChoice">Specifies the strategy for choosing pivot elements during Gaussian elimination.</param>
    /// <param name="result"> If successful, the coordinates of the unique intersection point; otherwise, <c>null</c>. </param>
    /// <returns><c>true</c> if a unique intersection point is found; otherwise, <c>false</c>.</returns>
    public static bool Solve(
        IEnumerable<HyperPlane>         HPs
      , GaussChoice                     gaussChoice
      , [NotNullWhen(true)] out TNum[]? result
      ) {
      HyperPlane[] hps = HPs.ToArray();

      if (hps.Length == 0) {
        result = null;

        return false;
      }

      return Solve
        (
         (i, j) => hps[i].Normal[j]
       , i => hps[i].ConstantTerm
       , hps.Length
       , hps[0].SpaceDim
       , gaussChoice
       , out result
        );
    }
#endregion

  }

}
