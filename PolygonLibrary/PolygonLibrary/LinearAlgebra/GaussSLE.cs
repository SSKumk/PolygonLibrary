using System;
using System.Diagnostics;
using System.Linq;
using System.Numerics;

namespace CGLibrary;

public partial class Geometry<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  // Пока решаем всё в предположении, что A размера d x d невырожденная квадратная матрица (самый простой случай)
  // столбец b размера d x 1
  public class GaussSLE {

    public enum GaussChoice {

      No
    , RowWise
    , ColWise
    , All

    }

    public static bool SolveNoChange(TNum[,] A, TNum[] b, GaussChoice gaussChoice, out TNum[] result) {
      return Solve((TNum[,])A.Clone(), (TNum[])b.Clone(), gaussChoice, out result);
    }

    public static bool Solve(TNum[,] A, TNum[] b, GaussChoice gaussChoice, out TNum[] result) {
      Debug.Assert(A.GetLength(0) == b.Length, "The length of vector b must be equal to the length of the first row of A matrix.");

      int N = b.Length;
      result = new TNum[N];

      int[] IndARow = Enumerable.Range(0, N).ToArray();
      int[] IndACol = Enumerable.Range(0, N).ToArray();
      int[] IndB    = Enumerable.Range(0, N).ToArray();

      for (int k = 0; k < N - 1; k++) { // последний элемент будем обрабатывать отдельно
        int  maxRowWiseInd = k;
        int  maxColWiseInd = k;
        int  lcol          = k, rcol = k, rrow = k, lrow = k;
        TNum absMaxEl      = Tools.Abs(A[IndARow[k], IndACol[k]]);

        switch (gaussChoice) {
          case GaussChoice.No: {
            if (Tools.EQ(absMaxEl)) {
              return false;
            }

            break;
          } // Всё выставленно куда надо.

          case GaussChoice.RowWise: {
            lcol = rcol = k;
            lrow = k;
            rrow = N - 1;

            break;
          }
          case GaussChoice.ColWise: {
            lrow = rrow = k;
            lcol = k;
            rcol = N - 1;

            break;
          }
          case GaussChoice.All: {
            lrow = k;
            lcol = k;
            rrow = N - 1;
            rcol = N - 1;

            break;
          }
        }

        for (int i = lrow; i <= rrow; i++) {
          for (int j = lcol; j <= rcol; j++) {
            TNum curAbs = Tools.Abs(A[IndARow[i], IndACol[j]]);
            if (curAbs > absMaxEl) {
              absMaxEl      = curAbs;
              maxRowWiseInd = i;
              maxColWiseInd = j;
            }
          }
        }

        Tools.Swap(ref IndB[k], ref IndB[maxRowWiseInd]);
        Tools.Swap(ref IndARow[k], ref IndARow[maxRowWiseInd]);
        Tools.Swap(ref IndACol[k], ref IndACol[maxColWiseInd]);

        if (Tools.EQ(absMaxEl)) { //Если все элементы в строке нулевые
          return false;
        }

        for (int i = k + 1; i < N; i++) {
          TNum t = A[IndARow[i], IndACol[k]] / A[IndARow[k], IndACol[k]];
          b[IndB[i]] -= t * b[IndB[k]];
          for (int j = k; j < N; j++) {
            A[IndARow[i], IndACol[j]] -= t * A[IndARow[k], IndACol[j]];
          }
        }
      }

      int n = b.Length - 1; // Максимальный валидный индекс
      if (Tools.EQ(A[IndARow[n], IndACol[n]])) {
        return false;
      }
      result[IndACol[n]] = b[IndB[n]] / A[IndARow[n], IndACol[n]];
      for (int k = n - 1; k >= 0; k--) {
        TNum sum = Tools.Zero;
        for (int i = k + 1; i < N; i++) { sum += A[IndARow[k], IndACol[i]] * result[IndACol[i]]; }
        result[IndACol[k]] = (b[IndB[k]] - sum) / A[IndARow[k], IndACol[k]];
      }

      return true;
    }

  }

}
