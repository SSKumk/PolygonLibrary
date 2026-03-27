using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;
using static Tests.DoubleGeometry.Basics.MatrixAssert;
using static Tests.DoubleGeometry.Basics.VectorAssert;

namespace Tests.DoubleGeometry.Basics;

[TestFixture]
public class MatrixStructureAndExtractionTests {

  [Test]
  public void HCatAndVCat_StaticAndInstanceVariants_WorkAsExpected() {
    Matrix m1 = new Matrix(new double[,] { { 1, 2 }, { 3, 4 } });
    Matrix m2 = new Matrix(new double[,] { { 5, 6 }, { 7, 8 } });
    Vector col = V(5, 6);
    Vector row = V(5, 6);

    AreEqual(Matrix.hcat(m1, m2), new Matrix(new double[,] { { 1, 2, 5, 6 }, { 3, 4, 7, 8 } }));
    AreEqual(Matrix.hcat(m1, col), new Matrix(new double[,] { { 1, 2, 5 }, { 3, 4, 6 } }));
    AreEqual(m1.hcat(col), Matrix.hcat(m1, col));

    AreEqual(Matrix.vcat(m1, row), new Matrix(new double[,] { { 1, 2 }, { 3, 4 }, { 5, 6 } }));
    AreEqual(m1.vcat(row), Matrix.vcat(m1, row));
    AreEqual(Matrix.vcat(m1, m2), new Matrix(new double[,] { { 1, 2 }, { 3, 4 }, { 5, 6 }, { 7, 8 } }));
  }

  [Test]
  public void TakeRowsColsSubMatrixAndVectors_ReturnExpectedSlices() {
    Matrix source = M(
      new[] { 1.0, 2.0, 3.0, 4.0 },
      new[] { 5.0, 6.0, 7.0, 8.0 },
      new[] { 9.0, 10.0, 11.0, 12.0 }
    );

    AreEqual(source.TakeRows(2, 0), M(
      new[] { 9.0, 10.0, 11.0, 12.0 },
      new[] { 1.0, 2.0, 3.0, 4.0 }
    ));
    AreEqual(source.TakeCols(3, 1), M(
      new[] { 4.0, 2.0 },
      new[] { 8.0, 6.0 },
      new[] { 12.0, 10.0 }
    ));
    AreEqual(source.TakeSubMatrix(new[] { 2, 0 }, new[] { 3, 1 }), M(
      new[] { 12.0, 10.0 },
      new[] { 4.0, 2.0 }
    ));
    AreEqual(source.TakeSubMatrix(null, new[] { 1, 3 }), M(
      new[] { 2.0, 4.0 },
      new[] { 6.0, 8.0 },
      new[] { 10.0, 12.0 }
    ));
    AreEqual(source.TakeSubMatrix(new[] { 1, 2 }, null), M(
      new[] { 5.0, 6.0, 7.0, 8.0 },
      new[] { 9.0, 10.0, 11.0, 12.0 }
    ));
    AreEqual(source.TakeSubMatrix(null, null), source);

    AreEqual(source.TakeRowVector(1), V(5, 6, 7, 8));
    AreEqual(source.TakeColumnVector(2), V(3, 7, 11));
  }

  [Test]
  public void Transpose_HandlesRectangularSquareAndVectorLikeMatrices() {
    Matrix rectangular = new Matrix(new double[,] { { 1, 2, 3 }, { 4, 5, 6 } });
    AreEqual(rectangular.Transpose(), new Matrix(new double[,] { { 1, 4 }, { 2, 5 }, { 3, 6 } }));
    AreEqual(rectangular.Transpose().Transpose(), rectangular);

    Matrix square = new Matrix(new double[,] { { 1, 2 }, { 3, 4 } });
    AreEqual(square.Transpose(), new Matrix(new double[,] { { 1, 3 }, { 2, 4 } }));

    Matrix rowVectorMatrix = new Matrix(new double[,] { { 1, 2, 3 } });
    AreEqual(rowVectorMatrix.Transpose(), new Matrix(new double[,] { { 1 }, { 2 }, { 3 } }));

    Matrix columnVectorMatrix = new Matrix(new double[,] { { 1 }, { 2 }, { 3 } });
    AreEqual(columnVectorMatrix.Transpose(), new Matrix(new double[,] { { 1, 2, 3 } }));
  }

}
