using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;
using static Tests.DoubleGeometry.Basics.MatrixAssert;
using static Tests.DoubleGeometry.Basics.VectorAssert;

namespace Tests.DoubleGeometry.Basics;

[TestFixture]
public class MatrixArithmeticTests {

  [Test]
  public void UnaryMinusAdditionSubtractionAndScalarArithmetic_WorkAsExpected() {
    Matrix m = new Matrix(new double[,] { { 1, -2 }, { 3, 0 } });
    Matrix neg = -m;
    Assert.Multiple(() => {
      Assert.That(neg[0, 0], Is.EqualTo(-1));
      Assert.That(neg[0, 1], Is.EqualTo(2));
      Assert.That(neg[1, 0], Is.EqualTo(-3));
      Assert.That(neg[1, 1], Is.EqualTo(0));
      Assert.That(neg.Rows, Is.EqualTo(m.Rows));
      Assert.That(neg.Cols, Is.EqualTo(m.Cols));
    });

    Matrix m1 = new Matrix(new double[,] { { 1, 2 }, { 3, 4 } });
    Matrix m2 = new Matrix(new double[,] { { 5, 6 }, { 7, 8 } });
    AreEqual(m1 + m2, new Matrix(new double[,] { { 6, 8 }, { 10, 12 } }));
    AreEqual(m2 - m1, new Matrix(new double[,] { { 4, 4 }, { 4, 4 } }));
    AreEqual(3.0 * m1, new Matrix(new double[,] { { 3, 6 }, { 9, 12 } }));
    AreEqual(m1 * 3.0, new Matrix(new double[,] { { 3, 6 }, { 9, 12 } }));
    AreEqual(new Matrix(new double[,] { { 2, 4 }, { 6, 8 } }) / 2.0, new Matrix(new double[,] { { 1, 2 }, { 3, 4 } }));
  }

  [Test]
  public void MatrixAndVectorMultiplication_WorkAsExpected() {
    Matrix m1 = new Matrix(new double[,] { { 1, 2, 3 }, { 4, 5, 6 } });
    Matrix m2 = new Matrix(new double[,] { { 7, 8 }, { 9, 10 }, { 11, 12 } });
    Matrix result = m1 * m2;

    Assert.Multiple(() => {
      Assert.That(result.Rows, Is.EqualTo(2));
      Assert.That(result.Cols, Is.EqualTo(2));
      Assert.That(result[0, 0], Is.EqualTo(58));
      Assert.That(result[0, 1], Is.EqualTo(64));
      Assert.That(result[1, 0], Is.EqualTo(139));
      Assert.That(result[1, 1], Is.EqualTo(154));
    });

    Vector mv = m1 * V(7, 8, 9);
    AreEqual(mv, V(50, 122));

    Vector rowResult = Matrix.MultRowVectorByMatrix(V(1, 2), new Matrix(new double[,] { { 10, 11, 12 }, { 20, 21, 22 } }));
    AreEqual(rowResult, V(50, 53, 56));

    Vector transposedResult = m1.MultiplyTransposedByVector(V(10, 1));
    AreEqual(transposedResult, V(14, 25, 36));
  }

  [Test]
  public void MultiplyBySelfTranspose_ReturnsSymmetricProduct() {
    Matrix square = new Matrix(new double[,] { { 1, 2 }, { 3, 4 } });
    Matrix squareExpected = new Matrix(new double[,] { { 5, 11 }, { 11, 25 } });
    AreEqual(square.MultiplyBySelfTranspose(), squareExpected);

    Matrix rectangular = new Matrix(new double[,] { { 1, 2, 3 }, { 4, 5, 6 } });
    Matrix rectangularExpected = new Matrix(new double[,] { { 14, 32 }, { 32, 77 } });
    AreEqual(rectangular.MultiplyBySelfTranspose(), rectangularExpected);
  }

}
