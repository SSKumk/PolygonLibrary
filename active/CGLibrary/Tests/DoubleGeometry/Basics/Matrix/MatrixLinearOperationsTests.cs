using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;
using static Tests.DoubleGeometry.Basics.MatrixAssert;
using static Tests.DoubleGeometry.Basics.VectorAssert;

namespace Tests.DoubleGeometry.Basics;

[TestFixture]
public class MatrixLinearOperationsTests {

  [Test]
  public void StandardFactories_CreateExpectedMatrices() {
    AreEqual(Matrix.Zero(2), M(new[] { 0.0, 0.0 }, new[] { 0.0, 0.0 }));
    AreEqual(Matrix.Zero(2, 3), M(new[] { 0.0, 0.0, 0.0 }, new[] { 0.0, 0.0, 0.0 }));
    AreEqual(Matrix.One(2), M(new[] { 1.0, 1.0 }, new[] { 1.0, 1.0 }));
    AreEqual(Matrix.One(2, 3), M(new[] { 1.0, 1.0, 1.0 }, new[] { 1.0, 1.0, 1.0 }));
    AreEqual(Matrix.Eye(3), M(new[] { 1.0, 0.0, 0.0 }, new[] { 0.0, 1.0, 0.0 }, new[] { 0.0, 0.0, 1.0 }));
    AreEqual(Matrix.Eye(3, 2), M(new[] { 1.0, 0.0 }, new[] { 0.0, 1.0 }, new[] { 0.0, 0.0 }));
    AreEqual(Matrix.Eye(2, 3), M(new[] { 1.0, 0.0, 0.0 }, new[] { 0.0, 1.0, 0.0 }));
  }

  [Test]
  public void Generators_RespectShapeDeterminismAndCoreInvariants() {
    GRandomLC random1 = new GRandomLC(17);
    GRandomLC random2 = new GRandomLC(17);
    Matrix gen1 = Matrix.GenMatrix(2, 3, -2.0, 3.0, random1);
    Matrix gen2 = Matrix.GenMatrix(2, 3, -2.0, 3.0, random2);
    AreEqual(gen1, gen2);
    Assert.That(gen1.Rows, Is.EqualTo(2));
    Assert.That(gen1.Cols, Is.EqualTo(3));
    for (int r = 0; r < gen1.Rows; r++) {
      for (int c = 0; c < gen1.Cols; c++) {
        Assert.That(gen1[r, c], Is.GreaterThanOrEqualTo(-2.0));
        Assert.That(gen1[r, c], Is.LessThanOrEqualTo(3.0));
      }
    }

    GRandomLC randomInt1 = new GRandomLC(23);
    GRandomLC randomInt2 = new GRandomLC(23);
    Matrix genInt1 = Matrix.GenMatrixInt(2, 3, -2, 3, randomInt1);
    Matrix genInt2 = Matrix.GenMatrixInt(2, 3, -2, 3, randomInt2);
    AreEqual(genInt1, genInt2);
    for (int r = 0; r < genInt1.Rows; r++) {
      for (int c = 0; c < genInt1.Cols; c++) {
        Assert.That(genInt1[r, c], Is.GreaterThanOrEqualTo(-2.0));
        Assert.That(genInt1[r, c], Is.LessThanOrEqualTo(3.0));
        Assert.That(Tools.EQ(genInt1[r, c], Math.Round(genInt1[r, c])), Is.True, $"Value {genInt1[r, c]} should be an integer.");
      }
    }

    Matrix dim1 = Matrix.GenNonSingular(1, -2.0, 3.0, new GRandomLC(31));
    Assert.That(Tools.NE(dim1[0, 0]), Is.True, "1x1 non-singular matrix element should not be zero if range allows.");

    Matrix nonSingular = Matrix.GenNonSingular(3, -1.0, 2.0, new GRandomLC(41));
    IsRREF(nonSingular.ToRREF(), "Generated non-singular matrix should reduce to an RREF with full rank.");
    AreEqual(nonSingular.ToRREF(), Matrix.Eye(3), "Generated non-singular matrix should reduce to identity.");

    Matrix on = Matrix.GenONMatrix(3, new GRandomLC(43));
    AreEqual(on * on.Transpose(), Matrix.Eye(3), "M * M^T should be Identity for ON matrix.");
    AreEqual(on.Transpose() * on, Matrix.Eye(3), "M^T * M should be Identity for ON matrix.");

    Matrix hilbert = Matrix.Hilbert(3);
    for (int r = 0; r < hilbert.Rows; r++) {
      for (int c = 0; c < hilbert.Cols; c++) {
        Assert.That(Tools.EQ(hilbert[r, c], Tools.One / (r + c + Tools.One)), Is.True);
      }
    }
  }

  [Test]
  public void RowAndColumnLinearFunctions_WorkAsExpected() {
    Matrix mRows = new Matrix(new double[,] { { 1, 2, 3 }, { 4, 5, 6 } });
    Vector v = V(10, 1, 0.1);

    Assert.That(Tools.EQ(mRows.MultiplyRowByVector(0, v), 12.3), Is.True);
    Assert.That(Tools.EQ(mRows.MultiplyRowByVector(1, v), 45.6), Is.True);

    Vector v2 = V(1, 1, 1);
    Assert.That(Tools.EQ(mRows.MultiplyRowByDiffOfVectors(0, v, v2), 6.3), Is.True);
    Assert.That(Tools.EQ(mRows.MultiplyRowByDiffOfVectors(1, v, v2), 30.6), Is.True);

    Matrix mCols = new Matrix(new double[,] { { 1, 4 }, { 2, 5 }, { 3, 6 } });
    Assert.That(Tools.EQ(mCols.MultiplyColumnByVector(0, v), 12.3), Is.True);
    Assert.That(Tools.EQ(mCols.MultiplyColumnByVector(1, v), 45.6), Is.True);
  }

  [Test]
  public void ToRREF_HandlesIdentityZeroAndRepresentativeCases() {
    Matrix id = Matrix.Eye(3);
    Matrix rrefId = id.ToRREF();
    AreEqual(rrefId, id, "RREF of identity matrix should be identity.");
    IsRREF(rrefId, "Identity matrix check");

    Matrix zero = Matrix.Zero(2, 3);
    Matrix rrefZero = zero.ToRREF();
    AreEqual(rrefZero, zero, "RREF of zero matrix should be zero.");
    IsRREF(rrefZero, "Zero matrix check");

    AreEqual(M(new[] { 5.0 }).ToRREF(), M(new[] { 1.0 }));
    AreEqual(M(new[] { 0.0 }).ToRREF(), M(new[] { 0.0 }));

    Matrix squareInvertible = M(new[] { 1.0, 2.0 }, new[] { 3.0, 4.0 });
    Matrix rrefSquareInvertible = squareInvertible.ToRREF();
    AreEqual(rrefSquareInvertible, Matrix.Eye(2), "RREF of invertible 2x2 matrix.");
    IsRREF(rrefSquareInvertible);

    Matrix squareSingular = M(new[] { 1.0, 2.0 }, new[] { 2.0, 4.0 });
    Matrix rrefSquareSingular = squareSingular.ToRREF();
    AreEqual(rrefSquareSingular, M(new[] { 1.0, 2.0 }, new[] { 0.0, 0.0 }), "RREF of singular 2x2 matrix.");
    IsRREF(rrefSquareSingular);

    Matrix squareNeedsSwap = M(new[] { 0.0, 1.0 }, new[] { 1.0, 0.0 });
    Matrix rrefSquareNeedsSwap = squareNeedsSwap.ToRREF();
    AreEqual(rrefSquareNeedsSwap, Matrix.Eye(2), "RREF needing row swap.");
    IsRREF(rrefSquareNeedsSwap);

    Matrix wide = M(new[] { 1.0, 1.0, 2.0, 3.0 }, new[] { 2.0, 2.0, 5.0, 7.0 });
    Matrix rrefWide = wide.ToRREF();
    AreEqual(rrefWide, M(new[] { 1.0, 1.0, 0.0, 1.0 }, new[] { 0.0, 0.0, 1.0, 1.0 }), "RREF of wide matrix (rank deficient).");
    IsRREF(rrefWide);

    Matrix tall = M(new[] { 1.0, 0.0 }, new[] { 0.0, 1.0 }, new[] { 2.0, 3.0 });
    Matrix rrefTall = tall.ToRREF();
    AreEqual(rrefTall, Matrix.Eye(3, 2), "RREF of tall matrix (full rank).");
    IsRREF(rrefTall);

    Matrix pivoting = M(new[] { Tools.Eps, 1.0 }, new[] { 1.0, 1.0 });
    Matrix rrefPivoting = pivoting.ToRREF();
    AreEqual(rrefPivoting, Matrix.Eye(2), "RREF requiring partial pivoting.");
    IsRREF(rrefPivoting);

    Matrix negative = M(new[] { -2.0, 4.0, -6.0 }, new[] { 3.0, -6.0, 9.0 });
    Matrix rrefNegative = negative.ToRREF();
    AreEqual(rrefNegative, M(new[] { 1.0, -2.0, 3.0 }, new[] { 0.0, 0.0, 0.0 }));
    IsRREF(rrefNegative);

    Matrix original = M(new[] { 1.0, 2.0 }, new[] { 3.0, 4.0 });
    Matrix copy = M(new[] { 1.0, 2.0 }, new[] { 3.0, 4.0 });
    original.ToRREF();
    AreEqual(original, copy);
  }

}
