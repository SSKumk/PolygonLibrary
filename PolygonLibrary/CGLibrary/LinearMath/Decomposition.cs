namespace CGLibrary;

public partial class Geometry<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  /// <summary>
  /// Contains Householder-based matrix decomposition routines and orthogonal update primitives.
  /// </summary>
  /// <remarks>
  /// The class provides full QR and LQ decompositions together with incremental QR and LQ updates
  /// of a square orthogonal matrix. All updates are implemented through Householder reflectors
  /// acting on trailing rows or columns.
  /// </remarks>
  public class Decomposition {

    /// <summary>
    /// Stores the parameters of a Householder reflector acting on a trailing block.
    /// </summary>
    /// <param name="House">The Householder vector <c>u</c>.</param>
    /// <param name="Beta">The scalar coefficient in <c>H = I - beta * u * u^T</c>.</param>
    /// <param name="Sign">The sign used when constructing the reflector.</param>
    /// <param name="OrthSize">The size of the trailing block on which the reflector acts.</param>
    private readonly record struct HouseholderData(Vector House, TNum Beta, TNum Sign, int OrthSize);

    /// <summary>
    /// Builds the Householder reflector that annihilates the trailing part of a vector.
    /// </summary>
    /// <param name="transformedVector">
    /// Input vector whose coordinates starting from <paramref name="currentBasisDimension"/>
    /// define the trailing part to be processed.
    /// </param>
    /// <param name="currentBasisDimension">
    /// The prefix length that must remain unchanged.
    /// Only coordinates with indices greater than or equal to this value participate in the reflector construction.
    /// </param>
    /// <param name="data">
    /// On success, receives the Householder data for the trailing block.
    /// On failure, receives the default value.
    /// </param>
    /// <returns>
    /// <c>true</c> if the trailing part is non-zero and a reflector was constructed;
    /// otherwise, <c>false</c>.
    /// </returns>
    private static bool TryBuildHouseholderFromTail(
      Vector transformedVector,
      int    currentBasisDimension,
      out HouseholderData data
    ) {
      int orthSize = transformedVector.SpaceDim - currentBasisDimension;
      TNum[] orthData = new TNum[orthSize];
      for (int i = 0; i < orthSize; i++) {
        orthData[i] = transformedVector[currentBasisDimension + i];
      }
      Vector orthPart = new Vector(orthData);
      if (orthPart.IsZero) {
        data = default;
        return false;
      }

      TNum[] houseData = orthPart.GetCopyAsArray();
      TNum   sign      = TConv.FromInt(Tools.Sign(orthPart[0]));
      if (Tools.EQ(orthPart[0], Tools.Zero)) {
        sign = Tools.One;
      }

      houseData[0] += sign * orthPart.Length;
      Vector house = new Vector(houseData);
      if (house.IsZero) {
        data = default;
        return false;
      }

      data = new HouseholderData(house, Tools.Two / house.Length2, sign, orthSize);

      return true;
    }

    /// <summary>
    /// Left-multiplies a trailing row block of a matrix by a Householder reflector.
    /// </summary>
    /// <param name="matrix">Matrix to be updated in place.</param>
    /// <param name="startRow">
    /// Index of the first affected row. Only rows from this index onward are transformed.
    /// </param>
    /// <param name="startCol">
    /// Index of the first affected column. Only columns from this index onward are updated.
    /// </param>
    /// <param name="data">Householder reflector data for the active trailing block.</param>
    /// <remarks>
    /// Applies <c>H * A_sub</c>, where <c>H = I - beta * u * u^T</c> acts on the trailing row block.
    /// </remarks>
    private static void ApplyHouseholderFromLeftToSubmatrix(
      ref MatrixMutable matrix,
      int               startRow,
      int               startCol,
      HouseholderData   data
    ) {
      for (int col = startCol; col < matrix.Cols; col++) {
        TNum dot = Tools.Zero;
        for (int i = 0; i < data.OrthSize; i++) {
          dot += data.House[i] * matrix[startRow + i, col];
        }

        for (int i = 0; i < data.OrthSize; i++) {
          matrix[startRow + i, col] -= data.Beta * data.House[i] * dot;
        }
      }
    }

    /// <summary>
    /// Right-multiplies a trailing column block of a matrix by a Householder reflector.
    /// </summary>
    /// <param name="matrix">Matrix to be updated in place.</param>
    /// <param name="startRow">
    /// Index of the first affected row. Only rows from this index onward are updated.
    /// </param>
    /// <param name="startCol">
    /// Index of the first affected column. Only columns from this index onward are transformed.
    /// </param>
    /// <param name="data">Householder reflector data for the active trailing block.</param>
    /// <remarks>
    /// Applies <c>A_sub * H</c>, where <c>H = I - beta * u * u^T</c> acts on the trailing column block.
    /// </remarks>
    private static void ApplyHouseholderFromRightToSubmatrix(
      ref MatrixMutable matrix,
      int               startRow,
      int               startCol,
      HouseholderData   data
    ) {
      for (int row = startRow; row < matrix.Rows; row++) {
        TNum dot = Tools.Zero;
        for (int j = 0; j < data.OrthSize; j++) {
          dot += matrix[row, startCol + j] * data.House[j];
        }

        for (int j = 0; j < data.OrthSize; j++) {
          matrix[row, startCol + j] -= dot * (data.Beta * data.House[j]);
        }
      }
    }

    /// <summary>
    /// Flips the sign of the leading row of the trailing block if requested by the reflector construction.
    /// </summary>
    /// <param name="currentQ">Square orthogonal matrix to be updated in place.</param>
    /// <param name="currentBasisDimension">Index of the first row in the trailing block.</param>
    /// <param name="sign">
    /// Sign chosen during reflector construction.
    /// The row is flipped only when this value equals <c>1</c>.
    /// </param>
    /// <remarks>
    /// This helper is used only when a caller wants a fixed sign convention for the leading row
    /// of the updated trailing block.
    /// </remarks>
    private static void AlignTrailingRowWithInput(
      ref MatrixMutable currentQ,
      int               currentBasisDimension,
      TNum              sign
    ) {
      if (Tools.NE(sign, Tools.One)) { return; }

      int d = currentQ.Rows;
      for (int col = 0; col < d; col++) {
        currentQ[currentBasisDimension, col] = -currentQ[currentBasisDimension, col];
      }
    }

    /// <summary>
    /// Computes the QR decomposition of a matrix by Householder reflections.
    /// </summary>
    /// <param name="A">Input matrix of size <c>d x m</c> with <c>d &gt;= m</c>.</param>
    /// <returns>
    /// A pair <c>(Q, R)</c> such that <c>A = Q * R</c>, where:
    /// <list type="bullet">
    /// <item><description><c>Q</c> is a square orthogonal matrix of size <c>d x d</c>,</description></item>
    /// <item><description><c>R</c> is an upper-triangular matrix of size <c>d x m</c>.</description></item>
    /// </list>
    /// </returns>
    /// <remarks>
    /// The algorithm applies Householder reflections from the left to annihilate entries below the diagonal
    /// and accumulates the same reflectors into the orthogonal factor <c>Q</c>.
    /// </remarks>
    public static (Matrix Q, Matrix R) QR_ByHouseholder(Matrix A) {
      int d = A.Rows;
      int m = A.Cols;

      Debug.Assert(d >= m, "Decomposition.QR_ByHouseholder: Can't decompose the system which d < m.");

      MatrixMutable r = new MatrixMutable(A, true);
      MatrixMutable q = MatrixMutable.Eye(d);

      int t = Math.Min(d - 1, m);
      for (int k = 0; k < t; k++) {
        Vector column = r.TakeColumnVector(k);
        if (!TryBuildHouseholderFromTail(column, k, out HouseholderData data)) { continue; }

        ApplyHouseholderFromLeftToSubmatrix(ref r, k, k, data);
        ApplyHouseholderFromRightToSubmatrix(ref q, 0, k, data);
      }

      Matrix qRes = new Matrix(q, false);
      Matrix rRes = new Matrix(r, false);

      Debug.Assert((qRes * rRes).Equals(A), $"Decomposition.QR_ByHouseholder: Q*R != A");


      return (qRes, rRes);
    }


    /// <summary>
    /// Computes the LQ decomposition of a matrix by Householder reflections.
    /// </summary>
    /// <param name="A">Input matrix of size <c>m x d</c>.</param>
    /// <returns>
    /// A pair <c>(L, Q)</c> such that <c>A = L * Q</c>, where:
    /// <list type="bullet">
    /// <item><description><c>L</c> is a lower-triangular matrix of size <c>m x d</c>,</description></item>
    /// <item><description><c>Q</c> is a square orthogonal matrix of size <c>d x d</c>.</description></item>
    /// </list>
    /// </returns>
    /// <remarks>
    /// The algorithm applies Householder reflections from the right to annihilate entries above the diagonal
    /// and accumulates the same reflectors into the orthogonal factor <c>Q</c>.
    /// </remarks>
    public static (Matrix L, Matrix Q) LQ_ByHouseholder(Matrix A) {
      int m = A.Rows;
      int d = A.Cols;

      MatrixMutable l = new MatrixMutable(A, true);
      MatrixMutable q = MatrixMutable.Eye(d);

      int t = Math.Min(m, d - 1);
      for (int k = 0; k < t; k++) {
        Vector row = l.TakeRowVector(k);
        if (!TryBuildHouseholderFromTail(row, k, out HouseholderData data)) { continue; }

        ApplyHouseholderFromRightToSubmatrix(ref l, k, k, data);
        ApplyHouseholderFromLeftToSubmatrix(ref q, k, 0, data);
      }

      Matrix lRes = new Matrix(l, false);
      Matrix qRes = new Matrix(q, false);

      Debug.Assert((lRes * qRes).Equals(A), $"Decomposition.LQ_ByHouseholder: L*Q != A");

      return (lRes, qRes);
    }

    /// <summary>
    /// Updates a square orthogonal matrix so that a given vector has zero trailing coordinates
    /// after right multiplication by the updated matrix.
    /// </summary>
    /// <param name="currentQ">
    /// Square orthogonal matrix of size <c>d x d</c>. It is modified in place when the update succeeds.
    /// </param>
    /// <param name="currentBasisDimension">
    /// Prefix length that is considered fixed.
    /// Only the trailing columns starting from this index may change.
    /// </param>
    /// <param name="v">
    /// Input vector of dimension <c>d</c>. After a successful update, the coordinates of
    /// <paramref name="v"/> in the updated orthogonal system are zero from
    /// <paramref name="currentBasisDimension"/> + 1 onward.
    /// </param>
    /// <returns>
    /// <paramref name="currentBasisDimension"/> + 1 if the trailing part of the transformed vector is non-zero;
    /// otherwise, returns <paramref name="currentBasisDimension"/>.
    /// </returns>
    /// <remarks>
    /// The update applies a Householder reflector to the trailing columns of <paramref name="currentQ"/>
    /// so that the transformed vector has zero coordinates after the new active position.
    /// </remarks>
    public static int QR_IncrementalUpdate(ref MatrixMutable currentQ, int currentBasisDimension, Vector v) {
      int d = currentQ.Rows;

      Debug.Assert(currentQ.Rows == currentQ.Cols, "QR_IncrementalUpdate: currentQ must be a square matrix.");
      Debug.Assert(v.SpaceDim == d, "QR_IncrementalUpdate: Vector v must have the same dimension as currentQ.");
      Debug.Assert
        (
         currentBasisDimension >= 0 && currentBasisDimension <= d
       , "QR_IncrementalUpdate: currentBasisDimension must be between 0 and d (inclusive)."
        );

      if (currentBasisDimension == d || v.IsZero) { return currentBasisDimension; }


      Vector y = Matrix.MultRowVectorByMatrix(v, currentQ);
      if (!TryBuildHouseholderFromTail(y, currentBasisDimension, out HouseholderData data)) {
        return currentBasisDimension;
      }

      ApplyHouseholderFromRightToSubmatrix(ref currentQ, 0, currentBasisDimension, data);

      return currentBasisDimension + 1;
    }


    /// <summary>
    /// Updates a square orthogonal matrix so that a given vector has zero trailing coordinates
    /// after left multiplication by the updated matrix.
    /// </summary>
    /// <param name="currentQ">
    /// Square orthogonal matrix of size <c>d x d</c>. It is modified in place when the update succeeds.
    /// </param>
    /// <param name="currentBasisDimension">
    /// Prefix length that is considered fixed.
    /// Only the trailing rows starting from this index may change.
    /// </param>
    /// <param name="v">
    /// Input vector of dimension <c>d</c>. After a successful update, the coordinates of
    /// <paramref name="v"/> in the updated orthogonal system are zero from
    /// <paramref name="currentBasisDimension"/> + 1 onward.
    /// </param>
    /// <param name="alignNewBasisVectorWithInput">
    /// If <c>true</c>, applies an additional sign convention to the leading row of the trailing block
    /// after the Householder update.
    /// </param>
    /// <returns>
    /// <paramref name="currentBasisDimension"/> + 1 if the trailing part of the transformed vector is non-zero;
    /// otherwise, returns <paramref name="currentBasisDimension"/>.
    /// </returns>
    /// <remarks>
    /// This is the row-wise counterpart of <see cref="QR_IncrementalUpdate(ref MatrixMutable, int, Vector)"/>.
    /// It applies a Householder reflector to the trailing rows of <paramref name="currentQ"/>.
    /// The optional sign alignment is an additional convention layer on top of the algebraic update.
    /// </remarks>
    internal static int LQ_IncrementalUpdateCore(
      ref MatrixMutable currentQ,
      int               currentBasisDimension,
      Vector            v,
      bool              alignNewBasisVectorWithInput
    ) {
      int d = currentQ.Rows;

      Debug.Assert(currentQ.Rows == currentQ.Cols, "LQ_IncrementalUpdate: currentQ must be a square matrix.");
      Debug.Assert(v.SpaceDim == d, "LQ_IncrementalUpdate: Vector v must have the same dimension as currentQ.");
      Debug.Assert
        (
         currentBasisDimension >= 0 && currentBasisDimension <= d
       , "LQ_IncrementalUpdate: currentBasisDimension must be between 0 and d (inclusive)."
        );

      if (currentBasisDimension == d || v.IsZero) { return currentBasisDimension; }

      Vector y = currentQ * v;
      if (!TryBuildHouseholderFromTail(y, currentBasisDimension, out HouseholderData data)) {
        return currentBasisDimension;
      }

      ApplyHouseholderFromLeftToSubmatrix(ref currentQ, currentBasisDimension, 0, data);
      if (alignNewBasisVectorWithInput) {
        AlignTrailingRowWithInput(ref currentQ, currentBasisDimension, data.Sign);
      }

      return currentBasisDimension + 1;
    }

    /// <summary>
    /// Updates a square orthogonal matrix so that a given vector has zero trailing coordinates
    /// after left multiplication by the updated matrix.
    /// </summary>
    /// <param name="currentQ">
    /// Square orthogonal matrix of size <c>d x d</c>. It is modified in place when the update succeeds.
    /// </param>
    /// <param name="currentBasisDimension">
    /// Prefix length that is considered fixed.
    /// Only the trailing rows starting from this index may change.
    /// </param>
    /// <param name="v">
    /// Input vector of dimension <c>d</c>. After a successful update, the coordinates of
    /// <paramref name="v"/> in the updated orthogonal system are zero from
    /// <paramref name="currentBasisDimension"/> + 1 onward.
    /// </param>
    /// <returns>
    /// <paramref name="currentBasisDimension"/> + 1 if the trailing part of the transformed vector is non-zero;
    /// otherwise, returns <paramref name="currentBasisDimension"/>.
    /// </returns>
    /// <remarks>
    /// This public wrapper performs the algebraic LQ update without any extra sign convention.
    /// </remarks>
    public static int LQ_IncrementalUpdate(ref MatrixMutable currentQ, int currentBasisDimension, Vector v)
      => LQ_IncrementalUpdateCore(ref currentQ, currentBasisDimension, v, alignNewBasisVectorWithInput: false);

  }

}

