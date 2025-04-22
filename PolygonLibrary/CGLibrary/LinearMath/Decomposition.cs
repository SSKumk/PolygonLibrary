namespace CGLibrary;

public partial class Geometry<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  public class Decomposition {

    /// <summary>
    /// Performs the QR-decomposition of the given d x m matrix A using Householder reflections.
    /// </summary>
    /// <param name="A">The matrix A to be decomposed.</param>
    /// <returns>
    /// A tuple (Q, R) where A = Q * R.
    /// Q is an d x d orthonormal matrix (Q^-1 = Q^T)
    /// R is an d x m upper triangular matrix.</returns>
    public static (Matrix Q, Matrix R) QR_ByReflection(Matrix A) {
      int d = A.Rows;
      int m = A.Cols;

      Debug.Assert(d >= m, "Decomposition.QR_ByReflection: Can't decompose the system which d < m.");

      TNum[,] R = A;
      TNum[,] Q = new TNum[d, d]; // Q = Eye
      for (int i = 0; i < d; i++) {
        for (int j = 0; j < d; j++) {
          Q[i, j] = Tools.Zero;
        }
      }
      for (int i = 0; i < d; i++) {
        Q[i, i] = Tools.One;
      }

      int t = Math.Min(d - 1, m);
      for (int k = 0; k < t; k++) {
        TNum[] v     = new TNum[d - k]; // Вектор Хаусхолдера
        TNum   normX = Tools.Zero;
        for (int i = 0; i < d - k; i++) {
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
            for (int i = 0; i < d - k; i++) { // v^T*R
              vTr += v[i] * R[k + i, j];
            }

            for (int i = 0; i < d - k; i++) {
              R[k + i, j] -= beta * v[i] * vTr; // R - (beta*v)(v^T*R)
            }
          }

          // Меняем Q.
          for (int j = 0; j < d; j++) {
            TNum vTq = Tools.Zero;
            for (int i = 0; i < d - k; i++) { // v^T*Q
              vTq += v[i] * Q[j, k + i];
            }

            for (int i = 0; i < d - k; i++) {
              Q[j, k + i] -= beta * v[i] * vTq; // R - (beta*v)(v^T*Q)
            }
          }
        }
      }

      Matrix q = new Matrix(Q);
      Matrix r = new Matrix(R);

      Debug.Assert((q * r).Equals(A), $"Decomposition.QR_ByReflection: Q*R != A");


      return (q, r);
    }


    /// <summary>
    /// Performs the LQ-decomposition of the given m×d matrix A using Householder reflections.
    /// </summary>
    /// <param name="A">The matrix A to be decomposed.</param>
    /// <returns>
    /// A tuple (L, Q) where A = L * Q.
    /// Q is an d x d orthonormal matrix (Q^-1 = Q^T).
    /// L is an m x d lower triangular matrix.</returns>
    public static (Matrix L, Matrix Q) LQ_ByReflection(Matrix A) {
      (Matrix Q_T, Matrix R_T) = QR_ByReflection(A.Transpose());

      return (R_T.Transpose(), Q_T.Transpose());
    }

  }

}
