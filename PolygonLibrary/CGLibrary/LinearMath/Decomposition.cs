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
    /// Captures pivot-based diagnostics for a triangular factor produced by QR or LQ decomposition.
    /// </summary>
    /// <param name="PivotMagnitudes">
    /// Absolute values of the diagonal entries of the produced triangular factor.
    /// </param>
    /// <param name="RelativePivots">
    /// Pivot magnitudes normalized by the largest pivot.
    /// This vector is zero when all pivots are numerically zero.
    /// </param>
    /// <param name="NumericRank">
    /// Number of diagonal pivots treated as non-zero by the current <see cref="Tools.Eps"/> policy.
    /// </param>
    public readonly record struct FactorizationDiagnostics(Vector PivotMagnitudes, Vector RelativePivots, int NumericRank);

    /// <summary>
    /// Captures quality diagnostics for a full orthogonal factorization.
    /// </summary>
    /// <param name="ReconstructionError">
    /// Maximum absolute entrywise error in reconstructing the source matrix from the returned factors.
    /// </param>
    /// <param name="OrthogonalityError">
    /// Maximum absolute entrywise error in <c>Q^T * Q - I</c> for the returned orthogonal factor.
    /// </param>
    /// <param name="TriangularLeakage">
    /// Maximum absolute entry that should be zero outside the triangular structure of the returned factor.
    /// </param>
    public readonly record struct FactorizationQualityDiagnostics(
      TNum ReconstructionError,
      TNum OrthogonalityError,
      TNum TriangularLeakage
    );

    /// <summary>
    /// Captures both pivot-based and quality diagnostics for a full orthogonal factorization.
    /// </summary>
    /// <param name="PivotDiagnostics">Diagnostics based on diagonal pivots of the triangular factor.</param>
    /// <param name="QualityDiagnostics">Diagnostics based on reconstruction, orthogonality and triangular leakage.</param>
    public readonly record struct FullFactorizationDiagnostics(
      FactorizationDiagnostics PivotDiagnostics,
      FactorizationQualityDiagnostics QualityDiagnostics
    );

    /// <summary>
    /// Captures diagnostics for a single incremental QR or LQ update step.
    /// </summary>
    /// <param name="InputNorm">Norm of the original input vector.</param>
    /// <param name="TrailingNorm">
    /// Norm of the active trailing part that is tested for numerical independence at the current prefix.
    /// </param>
    /// <param name="Rho">
    /// Relative size of the active trailing part, computed as <c>TrailingNorm / InputNorm</c> for non-zero inputs.
    /// </param>
    /// <param name="Accepted">
    /// <c>true</c> if the update increased the active dimension; otherwise, <c>false</c>.
    /// </param>
    public readonly record struct IncrementalUpdateDiagnostics(TNum InputNorm, TNum TrailingNorm, TNum Rho, bool Accepted);

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
    /// Computes the norm of the trailing part of a vector starting from the specified prefix length.
    /// </summary>
    /// <param name="vector">Vector whose trailing block is measured.</param>
    /// <param name="currentBasisDimension">Length of the fixed prefix.</param>
    /// <returns>The Euclidean norm of the trailing part.</returns>
    private static TNum ComputeTrailingNorm(Vector vector, int currentBasisDimension) {
      TNum sum = Tools.Zero;
      for (int i = currentBasisDimension; i < vector.SpaceDim; i++) {
        sum += vector[i] * vector[i];
      }

      return TNum.Sqrt(sum);
    }

    /// <summary>
    /// Builds diagnostics for a triangular factor by inspecting its diagonal pivots.
    /// </summary>
    /// <param name="triangularFactor">Upper- or lower-triangular factor produced by QR or LQ.</param>
    /// <returns>Pivot magnitudes, relative pivots and numeric rank under the current policy.</returns>
    private static FactorizationDiagnostics BuildFactorizationDiagnostics(Matrix triangularFactor) {
      int    diagSize   = Math.Min(triangularFactor.Rows, triangularFactor.Cols);
      TNum[] pivots     = new TNum[diagSize];
      TNum[] relative   = new TNum[diagSize];
      TNum   maxPivot   = Tools.Zero;
      int    numericRank = 0;

      for (int i = 0; i < diagSize; i++) {
        TNum pivot = TNum.Abs(triangularFactor[i, i]);
        pivots[i] = pivot;
        if (Tools.NE(pivot)) {
          numericRank++;
        }
        if (Tools.GT(pivot, maxPivot)) {
          maxPivot = pivot;
        }
      }

      if (Tools.NE(maxPivot)) {
        for (int i = 0; i < diagSize; i++) {
          relative[i] = pivots[i] / maxPivot;
        }
      }

      return new FactorizationDiagnostics(new Vector(pivots, false), new Vector(relative, false), numericRank);
    }

    /// <summary>
    /// Computes the maximum absolute entrywise difference between two matrices of the same size.
    /// </summary>
    /// <param name="left">The first matrix.</param>
    /// <param name="right">The second matrix.</param>
    /// <returns>The maximum absolute entrywise difference.</returns>
    private static TNum ComputeMaxAbsDiff(Matrix left, Matrix right) {
      Debug.Assert(left.Rows == right.Rows && left.Cols == right.Cols, "ComputeMaxAbsDiff: matrix sizes must match.");

      TNum maxDiff = Tools.Zero;
      for (int row = 0; row < left.Rows; row++) {
        for (int col = 0; col < left.Cols; col++) {
          TNum diff = TNum.Abs(left[row, col] - right[row, col]);
          if (Tools.GT(diff, maxDiff)) {
            maxDiff = diff;
          }
        }
      }

      return maxDiff;
    }

    /// <summary>
    /// Computes the maximum absolute entry that violates the expected triangular structure.
    /// </summary>
    /// <param name="triangularFactor">Upper- or lower-triangular factor to inspect.</param>
    /// <param name="isUpperTriangular">
    /// <c>true</c> for an upper-triangular factor; <c>false</c> for a lower-triangular factor.
    /// </param>
    /// <returns>The maximum absolute leakage outside the expected triangular part.</returns>
    private static TNum ComputeTriangularLeakage(Matrix triangularFactor, bool isUpperTriangular) {
      TNum maxLeak = Tools.Zero;

      if (isUpperTriangular) {
        for (int row = 0; row < triangularFactor.Rows; row++) {
          for (int col = 0; col < Math.Min(row, triangularFactor.Cols); col++) {
            TNum leak = TNum.Abs(triangularFactor[row, col]);
            if (Tools.GT(leak, maxLeak)) {
              maxLeak = leak;
            }
          }
        }
      }
      else {
        for (int row = 0; row < triangularFactor.Rows; row++) {
          for (int col = row + 1; col < triangularFactor.Cols; col++) {
            TNum leak = TNum.Abs(triangularFactor[row, col]);
            if (Tools.GT(leak, maxLeak)) {
              maxLeak = leak;
            }
          }
        }
      }

      return maxLeak;
    }

    /// <summary>
    /// Builds quality diagnostics for a QR or LQ factorization.
    /// </summary>
    /// <param name="sourceMatrix">The original matrix being factorized.</param>
    /// <param name="orthogonalFactor">The returned orthogonal factor.</param>
    /// <param name="triangularFactor">The returned triangular factor.</param>
    /// <param name="triangularOnRight">
    /// <c>true</c> for QR, where the reconstruction is <c>Q * R</c> and the triangular factor is upper-triangular;
    /// <c>false</c> for LQ, where the reconstruction is <c>L * Q</c> and the triangular factor is lower-triangular.
    /// </param>
    /// <returns>Reconstruction, orthogonality and triangular-structure diagnostics.</returns>
    private static FactorizationQualityDiagnostics BuildFactorizationQualityDiagnostics(
      Matrix sourceMatrix,
      Matrix orthogonalFactor,
      Matrix triangularFactor,
      bool   triangularOnRight
    ) {
      Matrix reconstruction = triangularOnRight ? orthogonalFactor * triangularFactor : triangularFactor * orthogonalFactor;
      Matrix identity = Matrix.Eye(orthogonalFactor.Cols);

      return new FactorizationQualityDiagnostics(
        ComputeMaxAbsDiff(reconstruction, sourceMatrix),
        ComputeMaxAbsDiff(orthogonalFactor.Transpose() * orthogonalFactor, identity),
        ComputeTriangularLeakage(triangularFactor, isUpperTriangular: triangularOnRight)
      );
    }

    /// <summary>
    /// Builds diagnostics for a single incremental update step from the input norm and the tested trailing norm.
    /// </summary>
    /// <param name="inputNorm">Norm of the original input vector.</param>
    /// <param name="trailingNorm">Norm of the active trailing block.</param>
    /// <param name="accepted"><c>true</c> if the update increased the active dimension.</param>
    /// <returns>The corresponding diagnostics record.</returns>
    private static IncrementalUpdateDiagnostics BuildIncrementalUpdateDiagnostics(
      TNum inputNorm,
      TNum trailingNorm,
      bool accepted
    ) {
      TNum rho = Tools.EQ(inputNorm) ? Tools.Zero : trailingNorm / inputNorm;

      return new IncrementalUpdateDiagnostics(inputNorm, trailingNorm, rho, accepted);
    }

    /// <summary>
    /// Core Householder QR path optionally returning pivot diagnostics.
    /// </summary>
    /// <param name="A">Input matrix.</param>
    /// <returns>The orthogonal factor, the triangular factor and diagnostics on the diagonal pivots of <c>R</c>.</returns>
    private static (Matrix Q, Matrix R, FactorizationDiagnostics Diagnostics) QR_ByHouseholderCore(Matrix A) {
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

      return (qRes, rRes, BuildFactorizationDiagnostics(rRes));
    }

    /// <summary>
    /// Core Householder LQ path optionally returning pivot diagnostics.
    /// </summary>
    /// <param name="A">Input matrix.</param>
    /// <returns>The triangular factor, the orthogonal factor and diagnostics on the diagonal pivots of <c>L</c>.</returns>
    private static (Matrix L, Matrix Q, FactorizationDiagnostics Diagnostics) LQ_ByHouseholderCore(Matrix A) {
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

      return (lRes, qRes, BuildFactorizationDiagnostics(lRes));
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
      (Matrix q, Matrix r, _) = QR_ByHouseholderCore(A);

      return (q, r);
    }

    /// <summary>
    /// Computes the QR decomposition together with pivot diagnostics for the produced triangular factor.
    /// </summary>
    /// <param name="A">Input matrix of size <c>d x m</c> with <c>d &gt;= m</c>.</param>
    /// <returns>
    /// A triple <c>(Q, R, Diagnostics)</c>, where <c>Diagnostics</c> reports diagonal pivot magnitudes,
    /// their values relative to the largest pivot, and the numeric rank induced by the current <see cref="Tools.Eps"/>.
    /// </returns>
    public static (Matrix Q, Matrix R, FactorizationDiagnostics Diagnostics) QR_ByHouseholderWithDiagnostics(Matrix A)
      => QR_ByHouseholderCore(A);

    /// <summary>
    /// Computes the QR decomposition together with both pivot-based and quality diagnostics.
    /// </summary>
    /// <param name="A">Input matrix of size <c>d x m</c> with <c>d &gt;= m</c>.</param>
    /// <returns>
    /// A triple <c>(Q, R, Diagnostics)</c>, where <c>Diagnostics</c> contains both diagonal-pivot information
    /// and factorization-quality metrics.
    /// </returns>
    public static (Matrix Q, Matrix R, FullFactorizationDiagnostics Diagnostics) QR_ByHouseholderWithFullDiagnostics(Matrix A) {
      (Matrix q, Matrix r, FactorizationDiagnostics pivotDiagnostics) = QR_ByHouseholderCore(A);

      return
        (
         q,
         r,
         new FullFactorizationDiagnostics
           (
            pivotDiagnostics,
            BuildFactorizationQualityDiagnostics(A, q, r, triangularOnRight: true)
           )
        );
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
      (Matrix l, Matrix q, _) = LQ_ByHouseholderCore(A);

      return (l, q);
    }

    /// <summary>
    /// Computes the LQ decomposition together with pivot diagnostics for the produced triangular factor.
    /// </summary>
    /// <param name="A">Input matrix of size <c>m x d</c>.</param>
    /// <returns>
    /// A triple <c>(L, Q, Diagnostics)</c>, where <c>Diagnostics</c> reports diagonal pivot magnitudes,
    /// their values relative to the largest pivot, and the numeric rank induced by the current <see cref="Tools.Eps"/>.
    /// </returns>
    public static (Matrix L, Matrix Q, FactorizationDiagnostics Diagnostics) LQ_ByHouseholderWithDiagnostics(Matrix A)
      => LQ_ByHouseholderCore(A);

    /// <summary>
    /// Computes the LQ decomposition together with both pivot-based and quality diagnostics.
    /// </summary>
    /// <param name="A">Input matrix of size <c>m x d</c>.</param>
    /// <returns>
    /// A triple <c>(L, Q, Diagnostics)</c>, where <c>Diagnostics</c> contains both diagonal-pivot information
    /// and factorization-quality metrics.
    /// </returns>
    public static (Matrix L, Matrix Q, FullFactorizationDiagnostics Diagnostics) LQ_ByHouseholderWithFullDiagnostics(Matrix A) {
      (Matrix l, Matrix q, FactorizationDiagnostics pivotDiagnostics) = LQ_ByHouseholderCore(A);

      return
        (
         l,
         q,
         new FullFactorizationDiagnostics
           (
            pivotDiagnostics,
            BuildFactorizationQualityDiagnostics(A, q, l, triangularOnRight: false)
           )
        );
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
      (int newBasisDimension, _) = QR_IncrementalUpdateWithDiagnostics(ref currentQ, currentBasisDimension, v);

      return newBasisDimension;
    }

    /// <summary>
    /// Updates a square orthogonal matrix as in <see cref="QR_IncrementalUpdate(ref MatrixMutable, int, Vector)"/>
    /// and also returns the relative size of the active trailing component.
    /// </summary>
    /// <param name="currentQ">Square orthogonal matrix of size <c>d x d</c>.</param>
    /// <param name="currentBasisDimension">Fixed prefix length.</param>
    /// <param name="v">Input vector to be adapted to the current orthogonal system.</param>
    /// <returns>
    /// A pair <c>(NewBasisDimension, Diagnostics)</c>, where <c>Diagnostics.Rho</c> equals the norm
    /// of the active trailing block divided by <c>||v||</c>.
    /// </returns>
    public static (int NewBasisDimension, IncrementalUpdateDiagnostics Diagnostics) QR_IncrementalUpdateWithDiagnostics(
      ref MatrixMutable currentQ,
      int               currentBasisDimension,
      Vector            v
    ) {
      int d = currentQ.Rows;

      Debug.Assert(currentQ.Rows == currentQ.Cols, "QR_IncrementalUpdate: currentQ must be a square matrix.");
      Debug.Assert(v.SpaceDim == d, "QR_IncrementalUpdate: Vector v must have the same dimension as currentQ.");
      Debug.Assert
        (
         currentBasisDimension >= 0 && currentBasisDimension <= d
       , "QR_IncrementalUpdate: currentBasisDimension must be between 0 and d (inclusive)."
        );

      if (currentBasisDimension == d || v.IsZero) {
        return (currentBasisDimension, BuildIncrementalUpdateDiagnostics(v.Length, Tools.Zero, false));
      }


      Vector y = Matrix.MultRowVectorByMatrix(v, currentQ);
      TNum trailingNorm = ComputeTrailingNorm(y, currentBasisDimension);
      if (!TryBuildHouseholderFromTail(y, currentBasisDimension, out HouseholderData data)) {
        return (currentBasisDimension, BuildIncrementalUpdateDiagnostics(v.Length, trailingNorm, false));
      }

      ApplyHouseholderFromRightToSubmatrix(ref currentQ, 0, currentBasisDimension, data);

      return (currentBasisDimension + 1, BuildIncrementalUpdateDiagnostics(v.Length, trailingNorm, true));
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
    internal static (int NewBasisDimension, IncrementalUpdateDiagnostics Diagnostics) LQ_IncrementalUpdateCoreWithDiagnostics(
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

      if (currentBasisDimension == d || v.IsZero) {
        return (currentBasisDimension, BuildIncrementalUpdateDiagnostics(v.Length, Tools.Zero, false));
      }

      Vector y = currentQ * v;
      TNum trailingNorm = ComputeTrailingNorm(y, currentBasisDimension);
      if (!TryBuildHouseholderFromTail(y, currentBasisDimension, out HouseholderData data)) {
        return (currentBasisDimension, BuildIncrementalUpdateDiagnostics(v.Length, trailingNorm, false));
      }

      ApplyHouseholderFromLeftToSubmatrix(ref currentQ, currentBasisDimension, 0, data);
      if (alignNewBasisVectorWithInput) {
        AlignTrailingRowWithInput(ref currentQ, currentBasisDimension, data.Sign);
      }

      return (currentBasisDimension + 1, BuildIncrementalUpdateDiagnostics(v.Length, trailingNorm, true));
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
      (int newBasisDimension, _) =
        LQ_IncrementalUpdateCoreWithDiagnostics(ref currentQ, currentBasisDimension, v, alignNewBasisVectorWithInput);

      return newBasisDimension;
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
      => LQ_IncrementalUpdateWithDiagnostics(ref currentQ, currentBasisDimension, v).NewBasisDimension;

    /// <summary>
    /// Updates a square orthogonal matrix as in <see cref="LQ_IncrementalUpdate(ref MatrixMutable, int, Vector)"/>
    /// and also returns the relative size of the active trailing component.
    /// </summary>
    /// <param name="currentQ">Square orthogonal matrix of size <c>d x d</c>.</param>
    /// <param name="currentBasisDimension">Fixed prefix length.</param>
    /// <param name="v">Input vector to be adapted to the current orthogonal system.</param>
    /// <returns>
    /// A pair <c>(NewBasisDimension, Diagnostics)</c>, where <c>Diagnostics.Rho</c> equals the norm
    /// of the active trailing block divided by <c>||v||</c>.
    /// </returns>
    public static (int NewBasisDimension, IncrementalUpdateDiagnostics Diagnostics) LQ_IncrementalUpdateWithDiagnostics(
      ref MatrixMutable currentQ,
      int               currentBasisDimension,
      Vector            v
    ) => LQ_IncrementalUpdateCoreWithDiagnostics(ref currentQ, currentBasisDimension, v, alignNewBasisVectorWithInput: false);

  }

}

