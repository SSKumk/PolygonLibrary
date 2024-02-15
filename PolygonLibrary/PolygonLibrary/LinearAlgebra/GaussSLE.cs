using System;
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

    //
    private static TNum[] Naive(TNum[,] A, TNum[] b) {
      int N = b.Length;
      for (int k = 0; k < N - 1; k++) { // последний элемент будем обрабатывать отдельно
        for (int i = k + 1; i < N; i++) {
          TNum t = A[i, k] / A[k, k];
          b[i] -= t * b[k];
          for (int j = k; j < N; j++) {
            A[i, j] -= t * A[k, j];
          }
        }
      }
      TNum[] res = new TNum[N];
      int    n   = b.Length - 1; // Максимальный валидный индекс
      res[n] = b[n] / A[n, n];
      for (int k = n - 1; k >= 0; k--) {
        TNum sum = Tools.Zero;
        for (int i = k + 1; i < N; i++) { sum += A[k, i] * res[i]; }
        res[k] = (b[k] - sum) / A[k, k];
      }

      return res;
    }

    public enum GaussChose {

      No
    , RowWise
    , ColWise
    , All

    }

    private static TNum FindAbsMaxInArray(TNum[] a, out int ind) {
      ind = 0;
      TNum max = Tools.Abs(a[0]);
      for (int i = 1; i < a.Length; i++) {
        TNum abs = Tools.Abs(a[i]);
        if (abs > max) {
          max = abs;
          ind = i;
        }
      }

      return max;
    }

    public static bool Solve(TNum[,] A, TNum[] b, GaussChose gaussChose, out TNum[] result) {
      if (gaussChose == GaussChose.No) {
        result = Naive(A, b);

        return true;
      }
      int N = b.Length;
      result = new TNum[N];

      int[] IndARow = Enumerable.Range(0, N).ToArray();
      int[] IndACol = Enumerable.Range(0, N).ToArray();
      int[] IndB    = Enumerable.Range(0, N).ToArray();

      for (int k = 0; k < N - 1; k++) { // последний элемент будем обрабатывать отдельно
        int  maxRowWiseInd = k;
        int  maxColWiseInd = k;
        TNum absMaxEl      = Tools.Abs(A[IndARow[k], IndACol[k]]);
        switch (gaussChose) {
          case GaussChose.RowWise: {
            absMaxEl = FindAbsMaxInArray
              (Enumerable.Range(k, N - k).Select(j => A[IndARow[j], IndACol[k]]).ToArray(), out int maxInd);
            maxRowWiseInd = IndARow[k] + maxInd;

            Tools.Swap(ref IndB[k], ref IndB[maxRowWiseInd]);
            Tools.Swap(ref IndARow[k], ref IndARow[maxRowWiseInd]);

            break;
          }

          case GaussChose.ColWise: {
            absMaxEl = FindAbsMaxInArray
              (Enumerable.Range(k, N - k).Select(j => A[IndARow[k], IndACol[j]]).ToArray(), out int maxInd);
            maxColWiseInd = IndACol[k] + maxInd;

            Tools.Swap(ref IndACol[k], ref IndACol[maxColWiseInd]);

            break;
          }

          case GaussChose.All: {
            for (int row = k; row < N; row++) {
              TNum absMax = FindAbsMaxInArray
                (Enumerable.Range(k, N - k).Select(j => A[IndARow[row], IndACol[j]]).ToArray(), out int maxColInd);
              if (absMax > absMaxEl) {
                absMaxEl      = absMax;
                maxRowWiseInd = row;
                maxColWiseInd = IndACol[k] + maxColInd;
              }
            }
            Tools.Swap(ref IndB[k], ref IndB[maxRowWiseInd]);
            Tools.Swap(ref IndARow[k], ref IndARow[maxRowWiseInd]);
            Tools.Swap(ref IndACol[k], ref IndACol[maxColWiseInd]);


            break;
          }
        }
        Console.WriteLine($"max = {absMaxEl,-4} row = {maxRowWiseInd,-2} col = {maxColWiseInd}");

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
