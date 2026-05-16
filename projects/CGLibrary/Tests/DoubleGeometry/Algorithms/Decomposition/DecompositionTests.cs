using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;
using static Tests.DoubleGeometry.Basics.MatrixAssert;
using static Tests.DoubleGeometry.Basics.VectorAssert;

namespace Tests.DoubleGeometry.Algorithms;

[TestFixture]
public class DecompositionTests {

  private static void AssertOrthonormal(Matrix q, string message = "") {
    AreEqual(q * q.Transpose(), Matrix.Eye(q.Rows), $"Q * Q^T != I. {message}");
    AreEqual(q.Transpose() * q, Matrix.Eye(q.Rows), $"Q^T * Q != I. {message}");
  }

  private static void AssertUpperTriangular(Matrix r, string message = "") {
    for (int row = 0; row < r.Rows; row++) {
      for (int col = 0; col < Math.Min(row, r.Cols); col++) {
        Assert.That(
          Tools.EQ(r[row, col]),
          Is.True,
          $"Entry [{row},{col}] should be zero in upper-triangular matrix. {message}"
        );
      }
    }
  }

  private static void AssertLowerTriangular(Matrix l, string message = "") {
    for (int row = 0; row < l.Rows; row++) {
      for (int col = row + 1; col < l.Cols; col++) {
        Assert.That(
          Tools.EQ(l[row, col]),
          Is.True,
          $"Entry [{row},{col}] should be zero in lower-triangular matrix. {message}"
        );
      }
    }
  }

  private static void AssertTailZero(Vector vector, int fromIndex, string message = "") {
    for (int i = fromIndex; i < vector.SpaceDim; i++) {
      Assert.That(Tools.EQ(vector[i]), Is.True, $"Coordinate {i} should be zero. {message}");
    }
  }

  private static double MaxAbsDiff(Matrix left, Matrix right) {
    double maxDiff = 0.0;
    for (int row = 0; row < left.Rows; row++) {
      for (int col = 0; col < left.Cols; col++) {
        maxDiff = Math.Max(maxDiff, Math.Abs(left[row, col] - right[row, col]));
      }
    }

    return maxDiff;
  }

  private static double MaxTriangularLeakage(Matrix triangularFactor, bool isUpperTriangular) {
    double maxLeak = 0.0;

    if (isUpperTriangular) {
      for (int row = 0; row < triangularFactor.Rows; row++) {
        for (int col = 0; col < Math.Min(row, triangularFactor.Cols); col++) {
          maxLeak = Math.Max(maxLeak, Math.Abs(triangularFactor[row, col]));
        }
      }
    }
    else {
      for (int row = 0; row < triangularFactor.Rows; row++) {
        for (int col = row + 1; col < triangularFactor.Cols; col++) {
          maxLeak = Math.Max(maxLeak, Math.Abs(triangularFactor[row, col]));
        }
      }
    }

    return maxLeak;
  }

  private static Matrix Rotation3D(int axis1, int axis2, double angle) {
    MatrixMutable rotation = MatrixMutable.Eye(3);
    double        c        = Math.Cos(angle);
    double        s        = Math.Sin(angle);

    rotation[axis1, axis1] = c;
    rotation[axis1, axis2] = -s;
    rotation[axis2, axis1] = s;
    rotation[axis2, axis2] = c;

    return new Matrix(rotation, false);
  }

  [Test]
  public void QR_ByHouseholder_SquareMatrix_SatisfiesFactorizationInvariants() {
    Matrix a = M(new[] { 12.0, -51.0, 4.0 }, new[] { 6.0, 167.0, -68.0 }, new[] { -4.0, 24.0, -41.0 });

    (Matrix q, Matrix r) = Decomposition.QR_ByHouseholder(a);

    Assert.Multiple(() => {
      AssertOrthonormal(q, "Square QR");
      AssertUpperTriangular(r, "Square QR");
      AreEqual(q * r, a, "Q * R should reconstruct A for square QR.");
    });
  }

  [Test]
  public void QR_ByHouseholder_TallMatrix_SatisfiesFactorizationInvariants() {
    Matrix a = M(new[] { 1.0, 2.0 }, new[] { 3.0, 4.0 }, new[] { 5.0, 6.0 });

    (Matrix q, Matrix r) = Decomposition.QR_ByHouseholder(a);

    Assert.Multiple(() => {
      Assert.That(q.Rows, Is.EqualTo(3));
      Assert.That(q.Cols, Is.EqualTo(3));
      Assert.That(r.Rows, Is.EqualTo(3));
      Assert.That(r.Cols, Is.EqualTo(2));
      AssertOrthonormal(q, "Tall QR");
      AssertUpperTriangular(r, "Tall QR");
      AreEqual(q * r, a, "Q * R should reconstruct A for tall QR.");
    });
  }

  [Test]
  public void QR_ByHouseholder_RankDeficientMatrix_PreservesReconstructionAndUpperTriangularShape() {
    Matrix a = M(new[] { 1.0, 2.0 }, new[] { 2.0, 4.0 }, new[] { 3.0, 6.0 });

    (Matrix q, Matrix r) = Decomposition.QR_ByHouseholder(a);

    Assert.Multiple(() => {
      AssertOrthonormal(q, "Rank-deficient QR");
      AssertUpperTriangular(r, "Rank-deficient QR");
      AreEqual(q * r, a, "Q * R should reconstruct rank-deficient A.");
    });
  }

  [Test]
  public void QR_ByHouseholderWithDiagnostics_ReturnsPivotMagnitudesRelativePivotsAndNumericRank() {
    Matrix a = M(new[] { 3.0, -2.0 }, new[] { 0.0, 1.0 }, new[] { 0.0, 0.0 });

    (Matrix q, Matrix r, Decomposition.FactorizationDiagnostics diagnostics) = Decomposition.QR_ByHouseholderWithDiagnostics(a);

    Assert.Multiple(() => {
      AssertOrthonormal(q, "QR diagnostics");
      AssertUpperTriangular(r, "QR diagnostics");
      AreEqual(q * r, a, "Q * R should reconstruct A for QR diagnostics.");
      Assert.That(diagnostics.NumericRank, Is.EqualTo(2));
      Assert.That(Tools.EQ(diagnostics.PivotMagnitudes[0], 3.0), Is.True);
      Assert.That(Tools.EQ(diagnostics.PivotMagnitudes[1], 1.0), Is.True);
      Assert.That(Tools.EQ(diagnostics.RelativePivots[0], 1.0), Is.True);
      Assert.That(Tools.EQ(diagnostics.RelativePivots[1], 1.0 / 3.0), Is.True);
    });
  }

  [Test]
  public void QR_ByHouseholderWithFullDiagnostics_ReportsQualityMetricsConsistentWithReturnedFactors() {
    Matrix q0 = Rotation3D(0, 1, Math.PI / 4) * Rotation3D(1, 2, Math.PI / 6);
    Matrix r0 = M(new[] { 3.0, -2.0 }, new[] { 0.0, 1.0 }, new[] { 0.0, 0.0 });
    Matrix a  = (q0 * r0) * 1e-8;

    (Matrix q, Matrix r, Decomposition.FullFactorizationDiagnostics diagnostics) = Decomposition.QR_ByHouseholderWithFullDiagnostics(a);

    Assert.Multiple(() => {
      AreEqual(q * r, a, "Q * R should reconstruct the near-Tools.Eps QR input.");
      Assert.That(
        Tools.EQ(diagnostics.QualityDiagnostics.ReconstructionError, MaxAbsDiff(q * r, a)),
        Is.True
      );
      Assert.That(
        Tools.EQ(diagnostics.QualityDiagnostics.OrthogonalityError, MaxAbsDiff(q.Transpose() * q, Matrix.Eye(q.Cols))),
        Is.True
      );
      Assert.That(
        Tools.EQ(diagnostics.QualityDiagnostics.TriangularLeakage, MaxTriangularLeakage(r, isUpperTriangular: true)),
        Is.True
      );
    });
  }

  [Test]
  public void QR_ByHouseholderWithFullDiagnostics_ScaleBelowToolsEps_DegeneratesToIdentityAndOriginalMatrix() {
    Matrix q0 = Rotation3D(0, 1, Math.PI / 4) * Rotation3D(1, 2, Math.PI / 6);
    Matrix r0 = M(new[] { 3.0, -2.0 }, new[] { 0.0, 1.0 }, new[] { 0.0, 0.0 });
    Matrix a  = (q0 * r0) * 1e-9;

    (Matrix q, Matrix r, Decomposition.FullFactorizationDiagnostics diagnostics) = Decomposition.QR_ByHouseholderWithFullDiagnostics(a);

    Assert.Multiple(() => {
      AreEqual(q, Matrix.Eye(3), "Below Tools.Eps the orthogonal factor should stay at identity.");
      AreEqual(r, a, "Below Tools.Eps the triangular factor should remain equal to the input matrix.");
      Assert.That(
        Tools.EQ(diagnostics.QualityDiagnostics.TriangularLeakage, MaxTriangularLeakage(r, isUpperTriangular: true)),
        Is.True
      );
    });
  }

  [Test]
  public void LQ_ByHouseholder_WideMatrix_SatisfiesFactorizationInvariants() {
    Matrix a = M(new[] { 1.0, 2.0, 3.0 }, new[] { 4.0, 5.0, 6.0 });

    (Matrix l, Matrix q) = Decomposition.LQ_ByHouseholder(a);

    Assert.Multiple(() => {
      Assert.That(l.Rows, Is.EqualTo(2));
      Assert.That(l.Cols, Is.EqualTo(3));
      Assert.That(q.Rows, Is.EqualTo(3));
      Assert.That(q.Cols, Is.EqualTo(3));
      AssertOrthonormal(q, "Wide LQ");
      AssertLowerTriangular(l, "Wide LQ");
      AreEqual(l * q, a, "L * Q should reconstruct A for wide LQ.");
    });
  }

  [Test]
  public void LQ_ByHouseholder_SquareMatrix_SatisfiesFactorizationInvariants() {
    Matrix a = M(new[] { 2.0, -1.0 }, new[] { 4.0, 3.0 });

    (Matrix l, Matrix q) = Decomposition.LQ_ByHouseholder(a);

    Assert.Multiple(() => {
      AssertOrthonormal(q, "Square LQ");
      AssertLowerTriangular(l, "Square LQ");
      AreEqual(l * q, a, "L * Q should reconstruct A for square LQ.");
    });
  }

  [Test]
  public void LQ_ByHouseholder_RankDeficientWideMatrix_PreservesReconstructionAndLowerTriangularShape() {
    Matrix a = M(new[] { 1.0, 2.0, 3.0 }, new[] { 2.0, 4.0, 6.0 });

    (Matrix l, Matrix q) = Decomposition.LQ_ByHouseholder(a);

    Assert.Multiple(() => {
      AssertOrthonormal(q, "Rank-deficient wide LQ");
      AssertLowerTriangular(l, "Rank-deficient wide LQ");
      AreEqual(l * q, a, "L * Q should reconstruct rank-deficient wide matrix.");
    });
  }

  [Test]
  public void LQ_ByHouseholder_TallMatrix_SatisfiesFactorizationInvariants() {
    Matrix a = M(new[] { 1.0, 2.0 }, new[] { 3.0, 4.0 }, new[] { 5.0, 6.0 });

    (Matrix l, Matrix q) = Decomposition.LQ_ByHouseholder(a);

    Assert.Multiple(() => {
      Assert.That(l.Rows, Is.EqualTo(3));
      Assert.That(l.Cols, Is.EqualTo(2));
      Assert.That(q.Rows, Is.EqualTo(2));
      Assert.That(q.Cols, Is.EqualTo(2));
      AssertOrthonormal(q, "Tall LQ");
      AssertLowerTriangular(l, "Tall LQ");
      AreEqual(l * q, a, "L * Q should reconstruct A for tall LQ.");
    });
  }

  [Test]
  public void LQ_ByHouseholderWithDiagnostics_ReturnsPivotMagnitudesRelativePivotsAndNumericRank() {
    Matrix a = M(new[] { 2.0, 0.0, 0.0 }, new[] { 1.0, 0.5, 0.0 });

    (Matrix l, Matrix q, Decomposition.FactorizationDiagnostics diagnostics) = Decomposition.LQ_ByHouseholderWithDiagnostics(a);

    Assert.Multiple(() => {
      AssertOrthonormal(q, "LQ diagnostics");
      AssertLowerTriangular(l, "LQ diagnostics");
      AreEqual(l * q, a, "L * Q should reconstruct A for LQ diagnostics.");
      Assert.That(diagnostics.NumericRank, Is.EqualTo(2));
      Assert.That(Tools.EQ(diagnostics.PivotMagnitudes[0], 2.0), Is.True);
      Assert.That(Tools.EQ(diagnostics.PivotMagnitudes[1], 0.5), Is.True);
      Assert.That(Tools.EQ(diagnostics.RelativePivots[0], 1.0), Is.True);
      Assert.That(Tools.EQ(diagnostics.RelativePivots[1], 0.25), Is.True);
    });
  }

  [Test]
  public void LQ_ByHouseholderWithFullDiagnostics_WellConditionedCase_ReportsSmallQualityErrors() {
    Matrix a = M(new[] { 1.0, 2.0, 3.0 }, new[] { 4.0, 5.0, 6.0 });

    (Matrix l, Matrix q, Decomposition.FullFactorizationDiagnostics diagnostics) = Decomposition.LQ_ByHouseholderWithFullDiagnostics(a);

    Assert.Multiple(() => {
      AreEqual(l * q, a, "L * Q should reconstruct the well-conditioned LQ input.");
      Assert.That(
        Tools.EQ(diagnostics.QualityDiagnostics.ReconstructionError, MaxAbsDiff(l * q, a)),
        Is.True
      );
      Assert.That(
        Tools.EQ(diagnostics.QualityDiagnostics.OrthogonalityError, MaxAbsDiff(q.Transpose() * q, Matrix.Eye(q.Cols))),
        Is.True
      );
      Assert.That(
        Tools.EQ(diagnostics.QualityDiagnostics.TriangularLeakage, MaxTriangularLeakage(l, isUpperTriangular: false)),
        Is.True
      );
    });
  }

  [Test]
  public void QR_IncrementalUpdate_IndependentVector_ExtendsBasisAndZeroesTailCoordinates() {
    MatrixMutable currentQ = MatrixMutable.Eye(3);
    Vector v = V(1, 2, 2);

    int newDim = Decomposition.QR_IncrementalUpdate(ref currentQ, 0, v);
    Vector coords = Matrix.MultRowVectorByMatrix(v, currentQ);

    Assert.Multiple(() => {
      Assert.That(newDim, Is.EqualTo(1));
      AssertOrthonormal(new Matrix(currentQ, true), "QR_IncrementalUpdate independent");
      Assert.That(Tools.NE(coords[0]), Is.True);
      AssertTailZero(coords, 1, "QR_IncrementalUpdate should eliminate tail coordinates after adding first basis vector.");
    });
  }

  [Test]
  public void QR_IncrementalUpdate_DependentOrZeroVector_DoesNotChangeBasis() {
    MatrixMutable currentQ = MatrixMutable.Eye(3);
    Vector basisVector = V(1, 2, 2);
    int basisDim = Decomposition.QR_IncrementalUpdate(ref currentQ, 0, basisVector);
    Matrix snapshot = new Matrix(currentQ, true);

    int afterDependent = Decomposition.QR_IncrementalUpdate(ref currentQ, basisDim, basisVector * 3.0);
    int afterZero = Decomposition.QR_IncrementalUpdate(ref currentQ, afterDependent, V(0, 0, 0));

    Assert.Multiple(() => {
      Assert.That(afterDependent, Is.EqualTo(basisDim));
      Assert.That(afterZero, Is.EqualTo(basisDim));
      AreEqual(new Matrix(currentQ, true), snapshot, "Dependent and zero vectors should not modify currentQ.");
    });
  }

  [Test]
  public void QR_IncrementalUpdateWithDiagnostics_ReturnsRhoForAcceptedAndRejectedSteps() {
    MatrixMutable currentQ = MatrixMutable.Eye(3);
    Vector basisVector = V(1, 2, 2);

    (int basisDim, Decomposition.IncrementalUpdateDiagnostics accepted) =
      Decomposition.QR_IncrementalUpdateWithDiagnostics(ref currentQ, 0, basisVector);
    Matrix snapshot = new Matrix(currentQ, true);
    (int afterDependent, Decomposition.IncrementalUpdateDiagnostics rejected) =
      Decomposition.QR_IncrementalUpdateWithDiagnostics(ref currentQ, basisDim, basisVector * 3.0);

    Assert.Multiple(() => {
      Assert.That(basisDim, Is.EqualTo(1));
      Assert.That(accepted.Accepted, Is.True);
      Assert.That(Tools.EQ(accepted.Rho, 1.0), Is.True);
      Assert.That(Tools.EQ(accepted.TrailingNorm, basisVector.Length), Is.True);
      Assert.That(afterDependent, Is.EqualTo(basisDim));
      Assert.That(rejected.Accepted, Is.False);
      Assert.That(Tools.EQ(rejected.Rho), Is.True);
      AreEqual(new Matrix(currentQ, true), snapshot, "Rejected QR diagnostic step should not modify currentQ.");
    });
  }

  [Test]
  public void QR_IncrementalUpdate_SecondIndependentVector_ExtendsPrefixAndZeroesNewTailCoordinates() {
    MatrixMutable currentQ = MatrixMutable.Eye(3);
    Vector first = V(1, 2, 2);
    Vector second = V(0, 1, -1);

    int basisDim = Decomposition.QR_IncrementalUpdate(ref currentQ, 0, first);
    int newDim = Decomposition.QR_IncrementalUpdate(ref currentQ, basisDim, second);
    Vector firstCoords = Matrix.MultRowVectorByMatrix(first, currentQ);
    Vector secondCoords = Matrix.MultRowVectorByMatrix(second, currentQ);

    Assert.Multiple(() => {
      Assert.That(newDim, Is.EqualTo(2));
      AssertOrthonormal(new Matrix(currentQ, true), "QR_IncrementalUpdate second vector");
      Assert.That(Tools.NE(firstCoords[0]), Is.True);
      AssertTailZero(firstCoords, 1, "First vector should remain adapted to the first prefix.");
      Assert.That(Tools.NE(secondCoords[1]), Is.True);
      AssertTailZero(secondCoords, 2, "Second vector should have zero tail after the second update.");
    });
  }

  [Test]
  public void LQ_IncrementalUpdate_IndependentVector_ExtendsBasisAndZeroesTailCoordinates() {
    MatrixMutable currentQ = MatrixMutable.Eye(3);
    Vector v = V(2, 1, 2);

    int newDim = Decomposition.LQ_IncrementalUpdate(ref currentQ, 0, v);
    Vector coords = currentQ * v;

    Assert.Multiple(() => {
      Assert.That(newDim, Is.EqualTo(1));
      AssertOrthonormal(new Matrix(currentQ, true), "LQ_IncrementalUpdate independent");
      Assert.That(Tools.NE(coords[0]), Is.True);
      AssertTailZero(coords, 1, "LQ_IncrementalUpdate should eliminate tail coordinates after adding first basis row.");
    });
  }

  [Test]
  public void LQ_IncrementalUpdate_DependentOrZeroVector_DoesNotChangeBasis() {
    MatrixMutable currentQ = MatrixMutable.Eye(3);
    Vector basisVector = V(2, 1, 2);
    int basisDim = Decomposition.LQ_IncrementalUpdate(ref currentQ, 0, basisVector);
    Matrix snapshot = new Matrix(currentQ, true);

    int afterDependent = Decomposition.LQ_IncrementalUpdate(ref currentQ, basisDim, basisVector * 5.0);
    int afterZero = Decomposition.LQ_IncrementalUpdate(ref currentQ, afterDependent, V(0, 0, 0));

    Assert.Multiple(() => {
      Assert.That(afterDependent, Is.EqualTo(basisDim));
      Assert.That(afterZero, Is.EqualTo(basisDim));
      AreEqual(new Matrix(currentQ, true), snapshot, "Dependent and zero vectors should not modify currentQ.");
    });
  }

  [Test]
  public void LQ_IncrementalUpdateWithDiagnostics_ReturnsRhoForAcceptedAndRejectedSteps() {
    MatrixMutable currentQ = MatrixMutable.Eye(3);
    Vector basisVector = V(2, 1, 2);

    (int basisDim, Decomposition.IncrementalUpdateDiagnostics accepted) =
      Decomposition.LQ_IncrementalUpdateWithDiagnostics(ref currentQ, 0, basisVector);
    Matrix snapshot = new Matrix(currentQ, true);
    (int afterDependent, Decomposition.IncrementalUpdateDiagnostics rejected) =
      Decomposition.LQ_IncrementalUpdateWithDiagnostics(ref currentQ, basisDim, basisVector * 5.0);

    Assert.Multiple(() => {
      Assert.That(basisDim, Is.EqualTo(1));
      Assert.That(accepted.Accepted, Is.True);
      Assert.That(Tools.EQ(accepted.Rho, 1.0), Is.True);
      Assert.That(Tools.EQ(accepted.TrailingNorm, basisVector.Length), Is.True);
      Assert.That(afterDependent, Is.EqualTo(basisDim));
      Assert.That(rejected.Accepted, Is.False);
      Assert.That(Tools.EQ(rejected.Rho), Is.True);
      AreEqual(new Matrix(currentQ, true), snapshot, "Rejected LQ diagnostic step should not modify currentQ.");
    });
  }

  [Test]
  public void LQ_IncrementalUpdate_SecondIndependentVector_ExtendsPrefixAndZeroesNewTailCoordinates() {
    MatrixMutable currentQ = MatrixMutable.Eye(3);
    Vector first = V(2, 1, 2);
    Vector second = V(1, -1, 0);

    int basisDim = Decomposition.LQ_IncrementalUpdate(ref currentQ, 0, first);
    int newDim = Decomposition.LQ_IncrementalUpdate(ref currentQ, basisDim, second);
    Vector firstCoords = currentQ * first;
    Vector secondCoords = currentQ * second;

    Assert.Multiple(() => {
      Assert.That(newDim, Is.EqualTo(2));
      AssertOrthonormal(new Matrix(currentQ, true), "LQ_IncrementalUpdate second vector");
      Assert.That(Tools.NE(firstCoords[0]), Is.True);
      AssertTailZero(firstCoords, 1, "First vector should remain adapted to the first prefix.");
      Assert.That(Tools.NE(secondCoords[1]), Is.True);
      AssertTailZero(secondCoords, 2, "Second vector should have zero tail after the second update.");
    });
  }

}

