using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;

namespace Tests.DoubleGeometry.Basics;

[TestFixture]
public class MatrixConstructionAndAccessTests {

  [Test]
  public void Constructor_Default_Creates1x1ZeroMatrix() {
    Matrix m = new Matrix();
    Assert.Multiple(() => {
      Assert.That(m.Rows, Is.EqualTo(1), "Rows should be 1.");
      Assert.That(m.Cols, Is.EqualTo(1), "Cols should be 1.");
      Assert.That(m[0, 0], Is.EqualTo(Tools.Zero), "Element [0,0] should be zero.");
    });
  }

  [Test]
  public void Constructor_From1DArray_WithCopy() {
    double[] data = { 1, 2, 3, 4, 5, 6 };
    Matrix m = new Matrix(2, 3, data, needCopy: true);

    Assert.Multiple(() => {
      Assert.That(m.Rows, Is.EqualTo(2));
      Assert.That(m.Cols, Is.EqualTo(3));
      Assert.That(m[0, 0], Is.EqualTo(1));
      Assert.That(m[0, 1], Is.EqualTo(2));
      Assert.That(m[0, 2], Is.EqualTo(3));
      Assert.That(m[1, 0], Is.EqualTo(4));
      Assert.That(m[1, 1], Is.EqualTo(5));
      Assert.That(m[1, 2], Is.EqualTo(6));
    });

    data[0] = 99;
    Assert.That(m[0, 0], Is.EqualTo(1), "Matrix should not change when original 1D array is modified (needCopy=true).");
  }

  [Test]
  public void Constructor_From1DArray_WithoutCopy() {
    double[] data = { 1, 2, 3, 4, 5, 6 };
    Matrix m = new Matrix(2, 3, data, false);

    Assert.Multiple(() => {
      Assert.That(m.Rows, Is.EqualTo(2));
      Assert.That(m.Cols, Is.EqualTo(3));
    });

    data[0] = 99;
    Assert.That(m[0, 0], Is.EqualTo(99), "Matrix should reflect changes in original 1D array (needCopy=false).");
  }

  [Test]
  public void Constructor_FromMatrixSubsetOfRows_WithCopy() {
    Matrix sourceM = new Matrix(3, 2, new double[] { 1, 2, 3, 4, 5, 6 });
    Matrix subM = new Matrix(2, sourceM, needCopy: true);

    Assert.Multiple(() => {
      Assert.That(subM.Rows, Is.EqualTo(2));
      Assert.That(subM.Cols, Is.EqualTo(2));
      Assert.That(subM[0, 0], Is.EqualTo(1));
      Assert.That(subM[0, 1], Is.EqualTo(2));
      Assert.That(subM[1, 0], Is.EqualTo(3));
      Assert.That(subM[1, 1], Is.EqualTo(4));
    });

    MatrixMutable mutableSourceM = new MatrixMutable(sourceM, false);
    mutableSourceM[0, 0] = 99;
    Assert.That(subM[0, 0], Is.EqualTo(1), "Sub-matrix should not change when original matrix is modified, as a copy of the subset is made.");
  }

  [Test]
  public void Constructor_From2DArray() {
    double[,] data2D = { { 1, 2, 3 }, { 4, 5, 6 } };
    Matrix m = new Matrix(data2D);

    Assert.Multiple(() => {
      Assert.That(m.Rows, Is.EqualTo(2));
      Assert.That(m.Cols, Is.EqualTo(3));
      Assert.That(m[0, 0], Is.EqualTo(1));
      Assert.That(m[1, 2], Is.EqualTo(6));
    });

    data2D[0, 0] = 99;
    Assert.That(m[0, 0], Is.EqualTo(1), "Matrix should not change when original 2D array is modified.");
  }

  [Test]
  public void Constructor_CopyConstructor_WithCopy() {
    Matrix sourceM = new Matrix(2, 2, new double[] { 1, 2, 3, 4 });
    Matrix copyM = new Matrix(sourceM, true);

    Assert.Multiple(() => {
      Assert.That(copyM.Rows, Is.EqualTo(sourceM.Rows));
      Assert.That(copyM.Cols, Is.EqualTo(sourceM.Cols));
      Assert.That(copyM[0, 0], Is.EqualTo(1));
      Assert.That(copyM[1, 1], Is.EqualTo(4));
    });

    MatrixMutable mutableSourceM = new MatrixMutable(sourceM, false);
    mutableSourceM[0, 0] = 99;
    Assert.That(copyM[0, 0], Is.EqualTo(1), "Copied matrix should not change when original is modified (needCopy=true).");
  }

  [Test]
  public void Constructor_CopyConstructor_WithoutCopy() {
    Matrix sourceM = new Matrix(2, 2, new double[] { 1, 2, 3, 4 });
    Matrix copyM = new Matrix(sourceM, false);

    MatrixMutable mutableSourceM = new MatrixMutable(sourceM, false);
    mutableSourceM[0, 0] = 99;
    Assert.That(copyM[0, 0], Is.EqualTo(99), "Copied matrix should reflect changes in original (needCopy=false).");
  }

  [Test]
  public void Constructor_FromVector_AndListOfVectors() {
    Vector vec = VectorAssert.V(1, 2, 3);
    Matrix singleRow = new Matrix(vec);
    Assert.Multiple(() => {
      Assert.That(singleRow.Rows, Is.EqualTo(1));
      Assert.That(singleRow.Cols, Is.EqualTo(3));
      Assert.That(singleRow[0, 0], Is.EqualTo(1));
      Assert.That(singleRow[0, 1], Is.EqualTo(2));
      Assert.That(singleRow[0, 2], Is.EqualTo(3));
    });

    List<Vector> rows = new List<Vector> { VectorAssert.V(1, 2, 3), VectorAssert.V(4, 5, 6) };
    Matrix m = new Matrix(rows);
    Assert.Multiple(() => {
      Assert.That(m.Rows, Is.EqualTo(2), "Rows should be 2.");
      Assert.That(m.Cols, Is.EqualTo(3), "Cols should be 3.");
      Assert.That(m[0, 0], Is.EqualTo(1));
      Assert.That(m[1, 2], Is.EqualTo(6));
    });
  }

  [Test]
  public void Indexers_ReturnElementsFromCoordinateAndLinearStorage() {
    Matrix m = new Matrix(new double[,] { { 1, 2 }, { 3, 4 } });
    Assert.Multiple(() => {
      Assert.That(m[0, 0], Is.EqualTo(1));
      Assert.That(m[0, 1], Is.EqualTo(2));
      Assert.That(m[1, 0], Is.EqualTo(3));
      Assert.That(m[1, 1], Is.EqualTo(4));
      Assert.That(m[0], Is.EqualTo(1));
      Assert.That(m[1], Is.EqualTo(2));
      Assert.That(m[2], Is.EqualTo(3));
      Assert.That(m[3], Is.EqualTo(4));
    });
  }

  [Test]
  public void CastOperators_CreateEquivalentIndependent2DArrayAndMatrix() {
    Matrix matrix = new Matrix(new double[,] { { 1, 2 }, { 3, 4 } });
    double[,] array2D = matrix;

    Assert.Multiple(() => {
      Assert.That(array2D.GetLength(0), Is.EqualTo(2));
      Assert.That(array2D.GetLength(1), Is.EqualTo(2));
      Assert.That(array2D[0, 0], Is.EqualTo(1));
      Assert.That(array2D[1, 1], Is.EqualTo(4));
    });

    MatrixMutable mutableMatrix = new MatrixMutable(matrix, false);
    mutableMatrix[0, 0] = 99;
    Assert.That(array2D[0, 0], Is.EqualTo(1), "2D array from implicit cast should be a copy.");

    double[,] sourceArray2D = { { 1, 2 }, { 3, 4 } };
    Matrix fromArray = (Matrix)sourceArray2D;
    sourceArray2D[0, 0] = 99;
    Assert.That(fromArray[0, 0], Is.EqualTo(1), "Matrix from explicit cast should be a copy of 2D array data.");
  }

}
