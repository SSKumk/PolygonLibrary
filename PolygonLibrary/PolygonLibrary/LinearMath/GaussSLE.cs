using System;
using System.Diagnostics;
using System.Linq;
using System.Numerics;

namespace CGLibrary;

public partial class Geometry<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  /// <summary>
  /// Provides functionality for solving systems of linear equations using Gaussian elimination.
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
    private readonly TNum[]      _result;
    private          GaussChoice _gaussChoice;

    private readonly int[] _indARow;
    private readonly int[] _indACol;
    private readonly int[] _indB;
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
    public GaussSLE(Func<int, int, TNum> AFunc
                  , Func<int, TNum>      bFunc
                  , int                  row
                  , int                  col
                  , GaussChoice          gaussChoice = GaussChoice.All) : this(row, col) {
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
    public void SetSystem(Func<int, int, TNum> AFunc
                        , Func<int, TNum>      bFunc
                        , int                  row
                        , int                  col
                        , GaussChoice          gaussChoice = GaussChoice.All) {
      Debug.Assert(row == _row, $"The amount of rows in A must be equal to initial parameter row. Found {row} row = {_row}");
      Debug.Assert(col == _col, $"The amount of columns in A must be equal to initial parameter row. Found {col} row = {_row}");
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
    public void SetSystem(TNum[,] A, TNum[] b, GaussChoice gaussChoice = GaussChoice.All) {
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
    /// </summary>
    public void Solve() {
      for (int i = 0; i < _row; i++) { // Установили индексные массивы
        _indARow[i] = i;
        _indACol[i] = i;
        _indB[i]    = i;
      }

      int maxRowInd = _row - 1;
      int maxColInd = _col - 1;
      for (int k = 0; k < maxRowInd; k++) { // последний элемент будем обрабатывать отдельно
        int  maxRowWiseInd = k;
        int  maxColWiseInd = k;
        int  lcol          = k, rcol = k, rrow = k, lrow = k;
        TNum absMaxEl      = Tools.Abs(_A[_indARow[k], _indACol[k]]);

        switch (_gaussChoice) {
          case GaussChoice.No: {
            if (Tools.EQ(absMaxEl)) {
              isSuccess = false;

              return;
            } // Если очередной элемент на диагонали ноль, то решения нет

            break;
          } // Всё выставленно куда надо.

          case GaussChoice.RowWise: {
            lcol = rcol = k;
            lrow = k;
            rrow = maxRowInd;

            break;
          }
          case GaussChoice.ColWise: {
            lrow = rrow = k;
            lcol = k;
            rcol = maxColInd;

            break;
          }
          case GaussChoice.All: {
            lrow = k;
            lcol = k;
            rrow = maxRowInd;
            rcol = maxColInd;

            break;
          }
        }

        for (int i = lrow; i <= rrow; i++) {
          for (int j = lcol; j <= rcol; j++) {
            TNum curAbs = Tools.Abs(_A[_indARow[i], _indACol[j]]);
            if (curAbs > absMaxEl) {
              absMaxEl      = curAbs;
              maxRowWiseInd = i;
              maxColWiseInd = j;
            }
          }
        }

        Tools.Swap(ref _indB[k], ref _indB[maxRowWiseInd]);
        Tools.Swap(ref _indARow[k], ref _indARow[maxRowWiseInd]);
        Tools.Swap(ref _indACol[k], ref _indACol[maxColWiseInd]);

        if (Tools.EQ(absMaxEl)) { //Если все элементы в строке нулевые
          isSuccess = false;

          return;
        }

        for (int i = k + 1; i < _row; i++) {
          TNum t = _A[_indARow[i], _indACol[k]] / _A[_indARow[k], _indACol[k]];
          _b[_indB[i]] -= t * _b[_indB[k]];
          for (int j = k; j < _row; j++) {
            _A[_indARow[i], _indACol[j]] -= t * _A[_indARow[k], _indACol[j]];
          }
        }
      }

      if (Tools.EQ(_A[_indARow[maxRowInd], _indACol[maxRowInd]])) {
        isSuccess = false;

        return;
      }
      _result[_indACol[maxRowInd]] = _b[_indB[maxRowInd]] / _A[_indARow[maxRowInd], _indACol[maxRowInd]];
      for (int k = maxRowInd - 1; k >= 0; k--) {
        TNum sum = Tools.Zero;
        for (int i = k + 1; i < _row; i++) { sum += _A[_indARow[k], _indACol[i]] * _result[_indACol[i]]; }
        _result[_indACol[k]] = (_b[_indB[k]] - sum) / _A[_indARow[k], _indACol[k]];
      }

      isSuccess = true;
    }

    /// <summary>
    /// Retrieves the solution of the system of linear equations as an array of numbers.
    /// </summary>
    /// <param name="result">Output parameter that receives the solution vector.</param>
    /// <returns><c>True</c> if the system has a unique solution, otherwise <c>false</c>.</returns>
    public bool GetSolution(out TNum[] result) {
      result = _result;

      return isSuccess;
    }

    /// <summary>
    /// Retrieves the solution of the system of linear equations as a Vector.
    /// </summary>
    /// <param name="result">Output parameter that receives the solution point.</param>
    /// <returns><c>True</c> if the system has a unique solution, otherwise <c>false</c>.</returns>
    public bool GetSolution(out Vector result) {
      result = new Vector(_result);

      return isSuccess;
    }
#endregion

#region Fabrics
    /// <summary>
    /// Solves the system of linear equations represented by the given functions.
    /// </summary>
    /// <param name="AFunc">Function provides the coefficients of the matrix A.</param>
    /// <param name="bFunc">Function provides the right side vector b.</param>
    /// <param name="dim">Dimension of the square matrix A and the length of vector b.</param>
    /// <param name="gaussChoice">Specifies the strategy for choosing pivot elements.</param>
    /// <param name="result">Output parameter that receives the solution vector if it unique.</param>
    /// <returns><c>True</c> if the system has a unique solution, otherwise <c>false</c>.</returns>
    public static bool Solve(Func<int, int, TNum> AFunc
                           , Func<int, TNum>      bFunc
                           , int                  dim
                           , GaussChoice          gaussChoice
                           , out TNum[]           result) {
      GaussSLE gaussSLE = new GaussSLE(AFunc, bFunc, dim, dim, gaussChoice);
      gaussSLE.Solve();

      return gaussSLE.GetSolution(out result);
    }

    /// <summary>
    /// Solves the system of linear equations represented by the given matrices.
    /// </summary>
    /// <param name="A">The coefficient matrix A.</param>
    /// <param name="b">The right side vector b.</param>
    /// <param name="gaussChoice">Specifies the strategy for choosing pivot elements.</param>
    /// <param name="result">Output parameter that receives the solution vector if it unique.</param>
    /// <returns><c>True</c> if the system has a unique solution, otherwise <c>false</c>.</returns>
    public static bool SolveImmutable(TNum[,] A, TNum[] b, GaussChoice gaussChoice, out TNum[] result) {
      GaussSLE gaussSLE = new GaussSLE((TNum[,])A.Clone(), (TNum[])b.Clone(), gaussChoice);
      gaussSLE.Solve();

      return gaussSLE.GetSolution(out result);
    }
#endregion

  }

}
