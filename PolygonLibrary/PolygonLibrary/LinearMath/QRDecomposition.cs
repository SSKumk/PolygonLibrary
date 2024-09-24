namespace CGLibrary;

public partial class Geometry<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  public class QRDecomposition {

    /// <summary>
    /// Does the QR-decomposition of given m x n matrix A.
    /// </summary>
    /// <param name="A">The matrix A to be decomposed.</param>
    /// <returns>A = Q*R. m x m Q - orthonormal matrix (Q^-1 = Q^T). m x n R - upper triangle matrix.</returns>
    public static (Matrix Q, Matrix R) ByReflection(Matrix A) {
      int    n = A.Rows;
      int    m = A.Cols;

      Debug.Assert(n >= m, "QRDecomposition.ByReflection: Can't decompose the system which n > m.");
      Matrix Q = Matrix.Eye(n);
      Matrix R = A;

      int t = Math.Min(n - 1, m);
      for (int k = 0; k < t; k++) {
        Vector x = R.TakeVector(k).SubVector(k, R.Rows - 1);
        if (!x.IsZero) {
          TNum[] v = x.GetAsArray();
          // v = x + sign(x_1)*||x||*e1.   (x1,x2,x3) + (p,0,0)
          int sx0    = Tools.Sign(x[0]);
          int sign = sx0 == 0 ? 1 : sx0;
          v[0] = x[0] + TConv.FromInt(sign) * x.Length;

          Vector u = new Vector(v, false);


          // todo А не переписать ли это всё на массивы?
          Matrix        Householder = Matrix.Eye(n - k) - Tools.Two * u.OuterProduct(u) / (u * u);
          MutableMatrix Hk          = MutableMatrix.Eye(n);
          Hk.SetSubMatrix(k, k, Householder);

          R = Hk * R;
          Q = Q * Hk;
        }
      }

      // Console.WriteLine($"Q:\n{Q}");
      // Console.WriteLine($"QR = R: {(Q*R).Equals(A)}");

      return (Q, R);
    }

  }

}
