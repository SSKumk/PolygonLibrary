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

  [Test]
  public void QR_ByReflection_SquareMatrix_SatisfiesFactorizationInvariants() {
    Matrix a = M(new[] { 12.0, -51.0, 4.0 }, new[] { 6.0, 167.0, -68.0 }, new[] { -4.0, 24.0, -41.0 });

    (Matrix q, Matrix r) = Decomposition.QR_ByReflection(a);

    Assert.Multiple(() => {
      AssertOrthonormal(q, "Square QR");
      AssertUpperTriangular(r, "Square QR");
      AreEqual(q * r, a, "Q * R should reconstruct A for square QR.");
    });
  }

  [Test]
  public void QR_ByReflection_TallMatrix_SatisfiesFactorizationInvariants() {
    Matrix a = M(new[] { 1.0, 2.0 }, new[] { 3.0, 4.0 }, new[] { 5.0, 6.0 });

    (Matrix q, Matrix r) = Decomposition.QR_ByReflection(a);

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
  public void QR_ByReflection_RankDeficientMatrix_PreservesReconstructionAndUpperTriangularShape() {
    Matrix a = M(new[] { 1.0, 2.0 }, new[] { 2.0, 4.0 }, new[] { 3.0, 6.0 });

    (Matrix q, Matrix r) = Decomposition.QR_ByReflection(a);

    Assert.Multiple(() => {
      AssertOrthonormal(q, "Rank-deficient QR");
      AssertUpperTriangular(r, "Rank-deficient QR");
      AreEqual(q * r, a, "Q * R should reconstruct rank-deficient A.");
    });
  }

  [Test]
  public void LQ_ByReflection_WideMatrix_SatisfiesFactorizationInvariants() {
    Matrix a = M(new[] { 1.0, 2.0, 3.0 }, new[] { 4.0, 5.0, 6.0 });

    (Matrix l, Matrix q) = Decomposition.LQ_ByReflection(a);

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
  public void LQ_ByReflection_SquareMatrix_SatisfiesFactorizationInvariants() {
    Matrix a = M(new[] { 2.0, -1.0 }, new[] { 4.0, 3.0 });

    (Matrix l, Matrix q) = Decomposition.LQ_ByReflection(a);

    Assert.Multiple(() => {
      AssertOrthonormal(q, "Square LQ");
      AssertLowerTriangular(l, "Square LQ");
      AreEqual(l * q, a, "L * Q should reconstruct A for square LQ.");
    });
  }

  [Test]
  public void QR_FullUpdate_IndependentVector_ExtendsBasisAndZeroesTailCoordinates() {
    MatrixMutable currentQ = MatrixMutable.Eye(3);
    Vector v = V(1, 2, 2);

    int newDim = Decomposition.QR_FullUpdate(ref currentQ, 0, v);
    Vector coords = Matrix.MultRowVectorByMatrix(v, currentQ);

    Assert.Multiple(() => {
      Assert.That(newDim, Is.EqualTo(1));
      AssertOrthonormal(new Matrix(currentQ, true), "QR_FullUpdate independent");
      Assert.That(Tools.NE(coords[0]), Is.True);
      AssertTailZero(coords, 1, "QR_FullUpdate should eliminate tail coordinates after adding first basis vector.");
    });
  }

  [Test]
  public void QR_FullUpdate_DependentOrZeroVector_DoesNotChangeBasis() {
    MatrixMutable currentQ = MatrixMutable.Eye(3);
    Vector basisVector = V(1, 2, 2);
    int basisDim = Decomposition.QR_FullUpdate(ref currentQ, 0, basisVector);
    Matrix snapshot = new Matrix(currentQ, true);

    int afterDependent = Decomposition.QR_FullUpdate(ref currentQ, basisDim, basisVector * 3.0);
    int afterZero = Decomposition.QR_FullUpdate(ref currentQ, afterDependent, V(0, 0, 0));

    Assert.Multiple(() => {
      Assert.That(afterDependent, Is.EqualTo(basisDim));
      Assert.That(afterZero, Is.EqualTo(basisDim));
      AreEqual(new Matrix(currentQ, true), snapshot, "Dependent and zero vectors should not modify currentQ.");
    });
  }

  [Test]
  public void LQ_FullUpdate_IndependentVector_ExtendsBasisAndZeroesTailCoordinates() {
    MatrixMutable currentQ = MatrixMutable.Eye(3);
    Vector v = V(2, 1, 2);

    int newDim = Decomposition.LQ_FullUpdate(ref currentQ, 0, v);
    Vector coords = currentQ * v;

    Assert.Multiple(() => {
      Assert.That(newDim, Is.EqualTo(1));
      AssertOrthonormal(new Matrix(currentQ, true), "LQ_FullUpdate independent");
      Assert.That(Tools.NE(coords[0]), Is.True);
      AssertTailZero(coords, 1, "LQ_FullUpdate should eliminate tail coordinates after adding first basis row.");
    });
  }

  [Test]
  public void LQ_FullUpdate_DependentOrZeroVector_DoesNotChangeBasis() {
    MatrixMutable currentQ = MatrixMutable.Eye(3);
    Vector basisVector = V(2, 1, 2);
    int basisDim = Decomposition.LQ_FullUpdate(ref currentQ, 0, basisVector);
    Matrix snapshot = new Matrix(currentQ, true);

    int afterDependent = Decomposition.LQ_FullUpdate(ref currentQ, basisDim, basisVector * 5.0);
    int afterZero = Decomposition.LQ_FullUpdate(ref currentQ, afterDependent, V(0, 0, 0));

    Assert.Multiple(() => {
      Assert.That(afterDependent, Is.EqualTo(basisDim));
      Assert.That(afterZero, Is.EqualTo(basisDim));
      AreEqual(new Matrix(currentQ, true), snapshot, "Dependent and zero vectors should not modify currentQ.");
    });
  }

}
