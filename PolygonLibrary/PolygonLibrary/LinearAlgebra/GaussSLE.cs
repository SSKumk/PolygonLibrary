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
        TNum maxEl         = A[IndARow[k], IndACol[k]];
        int  maxRowWiseInd = k;
        int  maxColWiseInd = k;
        switch (gaussChose) {
          case GaussChose.RowWise: {
            maxEl = FindAbsMaxInArray(Enumerable.Range(k, N - k).Select(j => A[IndARow[j], IndACol[k]]).ToArray(), out int maxInd );
            maxRowWiseInd = IndACol[k] + maxInd;

            Tools.Swap(ref IndB[k], ref IndB[maxRowWiseInd]);
            Tools.Swap(ref IndARow[k], ref IndARow[maxRowWiseInd]);

            break;
          }

          // case GaussChose.ColWise: {
          //   maxEl = FindAbsMaxInArray
          //     (Enumerable.Range(k, N - k).Select(j => A[IndARow[k], IndACol[j]]).ToArray(), out int maxInd);
          //   maxColWiseInd = IndACol[k] + maxInd; // todo Как правильно его вычислять?
          //
          //   Tools.Swap(ref IndACol[k], ref IndACol[maxColWiseInd]);
          //
          //   break;
          // }

          // case GaussChose.All: { todo тут надо о чём-то думать!
          //   for (int row = k; row < N; row++) {
          //     TNum max = FindAbsMaxInArray
          //       (Enumerable.Range(row, N - row).Select(j => A[IndARow[row], IndACol[j]]).ToArray(), out maxColWiseInd);
          //     if (max > maxEl) {
          //       maxEl         =  max;
          //       maxRowWiseInd =  row;
          //       maxColWiseInd += row;
          //     }
          //   }
          //
          //
          //   break;
          // }
        }
        Console.WriteLine($"max = {maxEl,-4} row = {maxRowWiseInd,-2} col = {maxColWiseInd}");

        if (Tools.EQ(maxEl)) { //Если все элементы в строке нулевые
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
      TNum[] res = new TNum[N];
      int    n   = b.Length - 1; // Максимальный валидный индекс
      res[n] = b[IndB[n]] / A[IndARow[n], IndACol[n]];
      for (int k = n - 1; k >= 0; k--) {
        TNum sum = Tools.Zero;
        for (int i = k + 1; i < N; i++) { sum += A[IndARow[k], IndACol[i]] * res[i]; }
        res[k] = (b[IndB[k]] - sum) / A[IndARow[k], IndACol[k]];
      }

      return true;
    }

  }

}
