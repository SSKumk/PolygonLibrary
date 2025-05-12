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

    /// <summary>
    /// Adds vector <paramref name="v"/> to the orthonormal basis represented by <paramref name="currentQ"/>
    /// using a QR decomposition update via Householder reflections.
    /// </summary>
    /// <param name="currentQ">The current orthogonal d×d matrix. Updated in-place if <paramref name="v"/> is independent.</param>
    /// <param name="currentBasisDimension">Number of basis vectors already in <paramref name="currentQ"/> (0 ≤ value ≤ d).</param>
    /// <param name="v">The vector to add.</param>
    /// <returns>
    /// <c>currentBasisDimension + 1</c> if <paramref name="v"/> is linearly independent; otherwise, returns <paramref name="currentBasisDimension"/>.
    /// </returns>
    public static int QR_FullUpdate(ref MutableMatrix currentQ, int currentBasisDimension, Vector v) {
      int d = currentQ.Rows;

      Debug.Assert(currentQ.Rows == currentQ.Cols, "QR_FullUpdate: currentQ must be a square matrix.");
      Debug.Assert(v.SpaceDim == d, "QR_FullUpdate: Vector v must have the same dimension as currentQ.");
      // currentBasisDimension - это количество уже существующих векторов в базисе.
      // Может быть от 0 (пустой базис) до d (базис полон).
      Debug.Assert
        (
         currentBasisDimension >= 0 && currentBasisDimension <= d
       , "QR_FullUpdate: currentBasisDimension must be between 0 and d (inclusive)."
        );

      if (currentBasisDimension == d || v.IsZero) { return currentBasisDimension; }


      Vector y = Matrix.MultRowVectorByMatrix(v, currentQ);

      int    orthSize = d - currentBasisDimension;
      TNum[] orthData = new TNum[orthSize];
      for (int i = 0; i < orthSize; i++) {
        orthData[i] = y[currentBasisDimension + i];
      }
      Vector orthPart = new Vector(orthData);

      TNum rho = orthPart.Length;
      if (orthPart.IsZero) { return currentBasisDimension; }


      TNum[] houseData = orthPart.GetCopyAsArray();
      TNum   sign      = TConv.FromInt(Tools.Sign(orthPart[0]));
      if (Tools.EQ(orthPart[0], Tools.Zero)) {
        sign = Tools.One;
      }
      houseData[0] += sign * rho;
      Vector house = new Vector(houseData);

      if (house.IsZero) {
        return currentBasisDimension + 1;
      }

      TNum beta = Tools.Two / house.Length2;
      for (int row = 0; row < d; row++) {
        TNum dot = Tools.Zero;
        for (int j = 0; j < orthSize; j++) { dot += currentQ[row, currentBasisDimension + j] * house[j]; }
        for (int j = 0; j < orthSize; j++) { currentQ[row, currentBasisDimension + j] -= dot * (beta * house[j]); }
      }

      return currentBasisDimension + 1;
    }


    /// <summary>
    /// Adds vector (representing a row) <paramref name="v"/> to the orthonormal basis
    /// represented by the first <paramref name="currentBasisDimension"/> rows of <paramref name="currentQ"/>
    /// using an LQ decomposition update via Householder reflections.
    /// </summary>
    /// <param name="currentQ">The current orthogonal d×d matrix. Its rows are updated in-place if <paramref name="v"/> is independent.</param>
    /// <param name="currentBasisDimension">Number of basis vectors (rows) already in <paramref name="currentQ"/> (0 ≤ value ≤ d).</param>
    /// <param name="v">The d×1 column vector that semantically represents the row to add.</param>
    /// <returns>
    /// <c>currentBasisDimension + 1</c> if <paramref name="v"/> is linearly independent from the current basis rows;
    /// otherwise, returns <paramref name="currentBasisDimension"/>.
    /// If update occurs, <paramref name="currentQ"/> is modified.
    /// </returns>
    public static int LQ_FullUpdate(ref MutableMatrix currentQ, int currentBasisDimension, Vector v) {
      int d = currentQ.Rows; // Размерность пространства d

      // --- Предусловия ---
      Debug.Assert(currentQ.Rows == currentQ.Cols, "LQ_FullUpdate: currentQ must be a square matrix.");
      Debug.Assert(v.SpaceDim == d, "LQ_FullUpdate: Vector v must have the same dimension as currentQ.");
      Debug.Assert
        (
         currentBasisDimension >= 0 && currentBasisDimension <= d
       , "LQ_FullUpdate: currentBasisDimension must be between 0 and d (inclusive)."
        );

      if (currentBasisDimension == d || v.IsZero) { return currentBasisDimension; }

      Vector y = currentQ * v;

      int    orthSize = d - currentBasisDimension;
      TNum[] orthData = new TNum[orthSize];
      for (int k = 0; k < orthSize; k++) {
        orthData[k] = y[currentBasisDimension + k];
      }
      Vector orthPart = new Vector(orthData);

      TNum rho = orthPart.Length;
      if (orthPart.IsZero) { return currentBasisDimension; }

      TNum[] houseData = orthPart.GetCopyAsArray();
      TNum   sign      = TConv.FromInt(Tools.Sign(orthPart[0]));
      if (Tools.EQ(orthPart[0], Tools.Zero)) {
        sign = Tools.One;
      }
      houseData[0] += sign * rho;
      Vector house = new Vector(houseData);

      if (house.IsZero) { return currentBasisDimension + 1; }

      TNum   beta          = Tools.Two / house.Length2;
      TNum[] projectionRow = new TNum[d];
      for (int col = 0; col < d; col++) {
        TNum dot = Tools.Zero;
        for (int i = 0; i < orthSize; i++) {
          dot += house[i] * currentQ[currentBasisDimension + i, col];
        }
        projectionRow[col] = dot;
      }

      for (int i = 0; i < orthSize; i++)  {
        int global_row_index = currentBasisDimension + i;
        for (int col = 0; col < d; col++) {
          currentQ[global_row_index, col] -= beta * house[i] * projectionRow[col];
        }
      }

      return currentBasisDimension + 1;
    }

  }

}
