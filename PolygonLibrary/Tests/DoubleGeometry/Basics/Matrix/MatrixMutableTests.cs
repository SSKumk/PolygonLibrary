using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;
using static Tests.DoubleGeometry.Basics.MatrixAssert;
using static Tests.DoubleGeometry.Basics.VectorAssert;

namespace Tests.DoubleGeometry.Basics;

[TestFixture]
public class MatrixMutableTests {

  [Test]
  public void ConstructorsAndIndexer_Setter_WorkAsExpected() {
    MatrixMutable fromArray = new MatrixMutable(2, 2, new[] { 1.0, 2.0, 3.0, 4.0 }, true);
    Assert.That(fromArray[1, 1], Is.EqualTo(4.0));

    Matrix source = M(new[] { 1.0, 2.0 }, new[] { 3.0, 4.0 });
    MatrixMutable copy = new MatrixMutable(source, true);
    MatrixMutable shared = new MatrixMutable(source, false);

    copy[0, 0] = 10.0;
    Assert.That(source[0, 0], Is.EqualTo(1.0));

    shared[0, 0] = 20.0;
    Assert.That(source[0, 0], Is.EqualTo(20.0));
    Assert.That(shared[0, 0], Is.EqualTo(20.0));
    Assert.That(shared[0, 1], Is.EqualTo(2.0));
    Assert.That(shared[1, 0], Is.EqualTo(3.0));
    Assert.That(shared[1, 1], Is.EqualTo(4.0));
  }

  [Test]
  public void MutableTransposeAndEye_ReturnMutableMatrices() {
    MatrixMutable eye = MatrixMutable.Eye(3);
    AreEqual(eye, Matrix.Eye(3));

    MatrixMutable transposed = new MatrixMutable(M(new[] { 1.0, 2.0, 3.0 }, new[] { 4.0, 5.0, 6.0 }), true).Transpose();
    AreEqual(transposed, M(new[] { 1.0, 4.0 }, new[] { 2.0, 5.0 }, new[] { 3.0, 6.0 }));

    transposed[0, 0] = 100.0;
    Assert.That(transposed[0, 0], Is.EqualTo(100.0));
  }

  [Test]
  public void SetSubMatrixAndSetSubVector_InsertDataIntoExpectedPosition() {
    MatrixMutable target = new MatrixMutable(Matrix.Zero(4, 4), true);
    target.SetSubMatrix(1, 1, M(
      new[] { 1.0, 2.0 },
      new[] { 3.0, 4.0 }
    ));
    AreEqual(target, M(
      new[] { 0.0, 0.0, 0.0, 0.0 },
      new[] { 0.0, 1.0, 2.0, 0.0 },
      new[] { 0.0, 3.0, 4.0, 0.0 },
      new[] { 0.0, 0.0, 0.0, 0.0 }
    ));

    target.SetSubVector(0, 3, V(5, 6, 7, 8));
    AreEqual(target, M(
      new[] { 0.0, 0.0, 0.0, 5.0 },
      new[] { 0.0, 1.0, 2.0, 6.0 },
      new[] { 0.0, 3.0, 4.0, 7.0 },
      new[] { 0.0, 0.0, 0.0, 8.0 }
    ));
  }

  [Test]
  public void SwapRowBlocks_MovesTopBlockToBottomPreservingOrder() {
    MatrixMutable source = new MatrixMutable(M(
      new[] { 1.0, 10.0, 100.0 },
      new[] { 2.0, 20.0, 200.0 },
      new[] { 3.0, 30.0, 300.0 }
    ), true);

    MatrixMutable swapped = MatrixMutable.SwapRowBlocks(source, 3, 1);
    AreEqual(swapped, M(
      new[] { 2.0, 20.0, 200.0 },
      new[] { 3.0, 30.0, 300.0 },
      new[] { 1.0, 10.0, 100.0 }
    ));
  }

  [Test]
  public void MutableMultiplication_ReturnsMutableResultWithCorrectValues() {
    MatrixMutable left = new MatrixMutable(M(
      new[] { 1.0, 2.0 },
      new[] { 3.0, 4.0 }
    ), true);
    MatrixMutable right = new MatrixMutable(M(
      new[] { 5.0, 6.0 },
      new[] { 7.0, 8.0 }
    ), true);

    MatrixMutable result = left * right;
    AreEqual(result, M(
      new[] { 19.0, 22.0 },
      new[] { 43.0, 50.0 }
    ));

    result[0, 0] = 100.0;
    Assert.That(result[0, 0], Is.EqualTo(100.0));
  }

}
