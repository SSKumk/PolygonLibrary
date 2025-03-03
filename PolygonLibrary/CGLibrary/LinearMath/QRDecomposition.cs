namespace CGLibrary;

public partial class Geometry<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  public class QRDecomposition {

    // /// <summary>
    // /// Solves a system of linear equations (SLE) represented by a set of hyperplanes.
    // /// </summary>
    // /// <param name="HPs">
    // /// The set of hyperplanes representing the linear equations.
    // /// </param>
    // /// <returns>
    // /// The solution vector x that satisfies all hyperplane equations (if a unique solution exists).
    // /// If the system is overdetermined, the solution is found in the least squares sense.
    // /// </returns>
    // public static Vector SolveSLE(IEnumerable<HyperPlane> HPs) {
    //   HyperPlane[] hps = HPs.ToArray();
    //
    //   int rows = hps.Length;
    //   int cols = hps[0].SpaceDim;
    //
    //   Debug.Assert
    //     (
    //      rows >= cols
    //    , "QRDecomposition.SolveSLE: The number of hyperplanes must be greater than or equal to the dimension of the space."
    //     );
    //
    //   TNum[,] A = new TNum[rows, cols];
    //   TNum[]  b = new TNum[rows];
    //   for (int i = 0; i < rows; i++) {
    //     Debug.Assert(hps[i].SpaceDim == cols, "QRDecomposition.SolveSLE: All hyperplanes must have the same dimensionality.");
    //     for (int j = 0; j < hps[i].SpaceDim; j++) {
    //       A[i, j] = hps[i].Normal[j];
    //     }
    //     b[i] = hps[i].ConstantTerm;
    //   }
    //
    //   return SolveSLE(new Matrix(A), new Vector(b));
    // }
    //
    // /// <summary>
    // /// Solves the system of linear equations Ax=b using QR decomposition.
    // /// </summary>
    // /// <param name="A">The matrix A: m x n, where m >= n.</param>
    // /// <param name="b">The vector b.</param>
    // /// <returns>The solution vector x.</returns>
    // public static Vector SolveSLE(Matrix A, Vector b) {
    //   // QRx=b
    //   (Matrix Q, Matrix R) = ByReflection(A);
    //
    //   // Rx = Q^T*b
    //   Vector y = Q.MultiplyTransposedByVector(b);
    //
    //   // Rx = y
    //   int    n = R.Cols;
    //   TNum[] x = new TNum[n];
    //   for (int i = n - 1; i >= 0; i--) {
    //     x[i] = y[i];
    //     for (int j = i + 1; j < n; j++) {
    //       x[i] -= R[i, j] * x[j];
    //     }
    //     x[i] /= R[i, i];
    //   }
    //
    //   Debug.Assert((A * new Vector(x, false)).Equals(b),"QRDecomposition.SolveSLE: x is not a solution of the system Ax = b!");
    //
    //   return new Vector(x, false);
    // }

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
