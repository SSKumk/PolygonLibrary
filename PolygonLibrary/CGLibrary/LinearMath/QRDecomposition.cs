namespace CGLibrary;

public partial class Geometry<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  public class QRDecomposition {
    /// <summary>
    /// Performs the QR-decomposition of the given m x n matrix A using Householder reflections.
    /// </summary>
    /// <param name="A">The matrix A to be decomposed.</param>
    /// <returns>A tuple (Q, R) where A = Q * R.
    /// Q is an m x m orthonormal matrix (Q^-1 = Q^T), and R is an m x n upper triangular matrix.</returns>
    public static (Matrix Q, Matrix R) ByReflection(Matrix A) {
      int n = A.Rows;
      int m = A.Cols;

      Debug.Assert(n >= m, "QRDecomposition.ByReflection: Can't decompose the system which n > m.");

      TNum[,] R = A;
      TNum[,] Q = new TNum[n, n]; // Q = Eye
      for (int i = 0; i < n; i++) {
        for (int j = 0; j < n; j++) {
          Q[i, j] = Tools.Zero;
        }
      }
      for (int i = 0; i < n; i++) {
        Q[i, i] = Tools.One;
      }

      int t = Math.Min(n - 1, m);
      for (int k = 0; k < t; k++) {
        TNum[] v     = new TNum[n - k]; // Вектор Хаусхолдера
        TNum   normX = Tools.Zero;
        for (int i = 0; i < n - k; i++) {
          v[i]  =  R[k + i, k];
          normX += v[i] * v[i];
        }
        normX = TNum.Sqrt(normX);

        if (Tools.NE(normX)) {
          int sign = TNum.Sign(v[0]) == 0 ? 1 : TNum.Sign(v[0]);
          v[0] += TConv.FromInt(sign) * normX; //v = x + sign(x_1)*||x||*e1.

          // Вычисляем beta = 2/(v^T * v) (нормировка)
          TNum vTv = Tools.Zero;
          foreach (TNum s in v) { vTv += s * s; }
          TNum beta = Tools.Two / vTv;

          // Основное свойство матриц Хаусхолдера
          // P = I - beta*v*v^T, A - matrix
          // PA = A - (beta*v)(v^T*A)
          // AP = A - (Av)(beta*v)^T

          // Меняем R. Столбцы до k-го уже посчитаны. Их менять не нужно. Ко всем остальным надо применить преобразование P*R_k
          for (int j = k; j < m; j++) { //
            TNum vTr = Tools.Zero;
            for (int i = 0; i < n - k; i++) { // v^T*R
              vTr += v[i] * R[k + i, j];
            }

            for (int i = 0; i < n - k; i++) {
              R[k + i, j] -= beta * v[i] * vTr; // R - (beta*v)(v^T*R)
            }
          }

          // Меняем Q.
          for (int j = 0; j < n; j++) {
            TNum vTq = Tools.Zero;
            for (int i = 0; i < n - k; i++) { // v^T*Q
              vTq += v[i] * Q[j, k + i];
            }

            for (int i = 0; i < n - k; i++) {
              Q[j, k + i] -= beta * v[i] * vTq; // R - (beta*v)(v^T*Q)
            }
          }
        }
      }

      Matrix q = new Matrix(Q);
      Matrix r = new Matrix(R);

      Debug.Assert((q * r).Equals(A), $"QRDecomposition.ByReflection: Q*R != A");


      return (q, r);
    }

  }

}
