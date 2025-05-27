using NUnit.Framework;
using CGLibrary;
using static CGLibrary.Geometry<double, Tests.DConvertor>;
using static Tests.SharedTests.StaticHelpers;
using System;
using System.Linq;


namespace Tests.SharedTests;

[TestFixture]
public class MatrixTests {

#region Constructors, Properties and Type Converters
  [Test]
  public void Constructor_Default_Creates1x1ZeroMatrix() {
    Matrix m = new Matrix();
    Assert.Multiple
      (()
         => {
         Assert.That(m.Rows, Is.EqualTo(1), "Rows should be 1.");
         Assert.That(m.Cols, Is.EqualTo(1), "Cols should be 1.");
         Assert.That(m[0, 0], Is.EqualTo(Tools.Zero), "Element [0,0] should be zero.");
       }
      );
  }

  [Test]
  public void Constructor_From1DArray_WithCopy() {
    double[] data = { 1, 2, 3, 4, 5, 6 };
    Matrix   m    = new Matrix(2, 3, data, needCopy: true); // needCopy = true by default

    Assert.Multiple
      (()
         => {
         Assert.That(m.Rows, Is.EqualTo(2));
         Assert.That(m.Cols, Is.EqualTo(3));
         Assert.That(m[0, 0], Is.EqualTo(1));
         Assert.That(m[0, 1], Is.EqualTo(2));
         Assert.That(m[0, 2], Is.EqualTo(3));
         Assert.That(m[1, 0], Is.EqualTo(4));
         Assert.That(m[1, 1], Is.EqualTo(5));
         Assert.That(m[1, 2], Is.EqualTo(6));
       }
      );

    data[0] = 99;
    Assert.That(m[0, 0], Is.EqualTo(1), "Matrix should not change when original 1D array is modified (needCopy=true).");
  }

  [Test]
  public void Constructor_From1DArray_WithoutCopy() {
    double[] data = { 1, 2, 3, 4, 5, 6 };
    Matrix   m    = new Matrix(2, 3, data, false);

    Assert.Multiple
      (()
         => {
         Assert.That(m.Rows, Is.EqualTo(2));
         Assert.That(m.Cols, Is.EqualTo(3));
       }
      );

    data[0] = 99;
    Assert.That(m[0, 0], Is.EqualTo(99), "Matrix should reflect changes in original 1D array (needCopy=false).");
  }

  [Test]
  public void Constructor_FromMatrixSubsetOfRows_WithCopy() {
    Matrix sourceM = new Matrix(3, 2, new double[] { 1, 2, 3, 4, 5, 6 });
    Matrix subM    = new Matrix(2, sourceM, needCopy: true);

    Assert.Multiple
      (()
         => {
         Assert.That(subM.Rows, Is.EqualTo(2));
         Assert.That(subM.Cols, Is.EqualTo(2));
         Assert.That(subM[0, 0], Is.EqualTo(1));
         Assert.That(subM[0, 1], Is.EqualTo(2));
         Assert.That(subM[1, 0], Is.EqualTo(3));
         Assert.That(subM[1, 1], Is.EqualTo(4));
       }
      );

    MatrixMutable mutableSourceM = new MatrixMutable(sourceM, false);
    mutableSourceM[0, 0] = 99;

    Assert.That
      (
       subM[0, 0]
     , Is.EqualTo(1)
     , "Sub-matrix should not change when original matrix is modified, as a copy of the subset is made."
      );
  }

  [Test]
  public void Constructor_From2DArray() {
    double[,] data2D = { { 1, 2, 3 }, { 4, 5, 6 } };
    Matrix    m      = new Matrix(data2D);

    Assert.Multiple
      (()
         => {
         Assert.That(m.Rows, Is.EqualTo(2));
         Assert.That(m.Cols, Is.EqualTo(3));
         Assert.That(m[0, 0], Is.EqualTo(1));
         Assert.That(m[1, 2], Is.EqualTo(6));
       }
      );

    data2D[0, 0] = 99;
    Assert.That(m[0, 0], Is.EqualTo(1), "Matrix should not change when original 2D array is modified.");
  }

  [Test]
  public void Constructor_CopyConstructor_WithCopy() {
    Matrix sourceM = new Matrix(2, 2, new double[] { 1, 2, 3, 4 });
    Matrix copyM   = new Matrix(sourceM, true);

    Assert.Multiple
      (()
         => {
         Assert.That(copyM.Rows, Is.EqualTo(sourceM.Rows));
         Assert.That(copyM.Cols, Is.EqualTo(sourceM.Cols));
         Assert.That(copyM[0, 0], Is.EqualTo(1));
         Assert.That(copyM[1, 1], Is.EqualTo(4));
       }
      );

    MatrixMutable mutableSourceM = new MatrixMutable(sourceM, false);
    mutableSourceM[0, 0] = 99;
    Assert.That(copyM[0, 0], Is.EqualTo(1), "Copied matrix should not change when original is modified (needCopy=true).");
  }

  [Test]
  public void Constructor_CopyConstructor_WithoutCopy() {
    Matrix sourceM = new Matrix(2, 2, new double[] { 1, 2, 3, 4 });
    Matrix copyM   = new Matrix(sourceM, false);

    MatrixMutable mutableSourceM = new MatrixMutable(sourceM, false);
    mutableSourceM[0, 0] = 99;
    Assert.That(copyM[0, 0], Is.EqualTo(99), "Copied matrix should reflect changes in original (needCopy=false).");
  }

  [Test]
  public void Constructor_FromVector() {
    Vector vec = V(1, 2, 3);
    Matrix m   = new Matrix(vec);

    Assert.Multiple
      (()
         => {
         Assert.That(m.Rows, Is.EqualTo(1));
         Assert.That(m.Cols, Is.EqualTo(3));
         Assert.That(m[0, 0], Is.EqualTo(1));
         Assert.That(m[0, 1], Is.EqualTo(2));
         Assert.That(m[0, 2], Is.EqualTo(3));
       }
      );
  }

  [Test]
  public void Indexer_2D_Get_ValidIndices() {
    Matrix m = new Matrix(new double[,] { { 1, 2 }, { 3, 4 } });
    Assert.Multiple
      (()
         => {
         Assert.That(m[0, 0], Is.EqualTo(1));
         Assert.That(m[0, 1], Is.EqualTo(2));
         Assert.That(m[1, 0], Is.EqualTo(3));
         Assert.That(m[1, 1], Is.EqualTo(4));
       }
      );
  }

  [Test]
  public void Indexer_1D_Get_ValidIndices() {
    Matrix m = new Matrix(new double[,] { { 1, 2 }, { 3, 4 } });
    Assert.Multiple
      (()
         => {
         Assert.That(m[0], Is.EqualTo(1));
         Assert.That(m[1], Is.EqualTo(2));
         Assert.That(m[2], Is.EqualTo(3));
         Assert.That(m[3], Is.EqualTo(4));
       }
      );
  }

  [Test]
  public void ImplicitOperator_To2DArray() {
    Matrix    matrix  = new Matrix(new double[,] { { 1, 2 }, { 3, 4 } });
    double[,] array2D = matrix;

    Assert.Multiple
      (()
         => {
         Assert.That(array2D.GetLength(0), Is.EqualTo(2));
         Assert.That(array2D.GetLength(1), Is.EqualTo(2));
         Assert.That(array2D[0, 0], Is.EqualTo(1));
         Assert.That(array2D[1, 1], Is.EqualTo(4));
       }
      );

    MatrixMutable mutableMatrix = new MatrixMutable(matrix, false);
    mutableMatrix[0, 0] = 99;
    Assert.That(array2D[0, 0], Is.EqualTo(1), "2D array from implicit cast should be a copy.");
  }

  [Test]
  public void ExplicitOperator_From2DArray() {
    double[,] array2D = { { 1, 2 }, { 3, 4 } };
    Matrix    matrix  = (Matrix)array2D;

    Assert.Multiple
      (()
         => {
         Assert.That(matrix.Rows, Is.EqualTo(2));
         Assert.That(matrix.Cols, Is.EqualTo(2));
         Assert.That(matrix[0, 0], Is.EqualTo(1));
         Assert.That(matrix[1, 1], Is.EqualTo(4));
       }
      );

    array2D[0, 0] = 99;
    Assert.That(matrix[0, 0], Is.EqualTo(1), "Matrix from explicit cast should be a copy of 2D array data.");
  }
#endregion

#region Overrides (Equals, GetHashCode, ToString)
  [Test]
  public void Equals_NullObject_ReturnsFalse() {
    Matrix m1 = new Matrix(new double[,] { { 1, 2 }, { 3, 4 } });
    Assert.That(m1, Is.Not.EqualTo(null));
  }

  [Test]
  public void Equals_SameInstance_ReturnsTrue() {
    Matrix m1 = new Matrix(new double[,] { { 1, 2 }, { 3, 4 } });
    Assert.That(m1, Is.EqualTo(m1));
  }

  [Test]
  public void Equals_EqualMatrices_ReturnsTrue() {
    Matrix m1 = new Matrix(new double[,] { { 1, 2 }, { 3, 4 } });
    Matrix m2 = new Matrix(new double[,] { { 1, 2 }, { 3, 4 } });
    Assert.That(m1, Is.EqualTo(m2));
  }

  [Test]
  public void Equals_DifferentDimensions_Rows_ReturnsFalse() {
    Matrix m1 = new Matrix(new double[,] { { 1, 2 }, { 3, 4 } }); // 2x2
    Matrix m2 = new Matrix(new double[,] { { 1, 2 } });           // 1x2
    Assert.That(m1, Is.Not.EqualTo(m2));
  }

  [Test]
  public void Equals_DifferentDimensions_Cols_ReturnsFalse() {
    Matrix m1 = new Matrix(new double[,] { { 1, 2 }, { 3, 4 } }); // 2x2
    Matrix m2 = new Matrix(new double[,] { { 1 }, { 3 } });       // 2x1
    Assert.That(m1, Is.Not.EqualTo(m2));
  }

  [Test]
  public void Equals_DifferentValues_ReturnsFalse() {
    Matrix m1 = new Matrix(new double[,] { { 1, 2 }, { 3, 4 } });
    Matrix m2 = new Matrix(new double[,] { { 1, 2 }, { 3, 5 } }); // Last element different
    Assert.That(m1, Is.Not.EqualTo(m2));
  }

  [Test]
  public void Equals_FloatingPointTolerance() {
    double val = 1.0 / 3.0;
    Matrix m1  = new Matrix(new double[,] { { val } });
    Matrix m2  = new Matrix(new double[,] { { 0.3333333333333331 } }); // Slightly different representation
    Matrix m3  = new Matrix(new double[,] { { 0.3333333333333337 } }); // Slightly different representation
    Matrix m4  = new Matrix(new double[,] { { 0.4 } });                // Clearly different

    Assert.That(m1, Is.EqualTo(m2), "m1 should equal m2 within tolerance.");
    Assert.That(m1, Is.EqualTo(m3), "m1 should equal m3 within tolerance.");
    Assert.That(m1, Is.Not.EqualTo(m4), "m1 should not equal m4.");
  }

  [Test]
  public void GetHashCode_ThrowsInvalidOperationException() {
    Matrix m = new Matrix();
    Assert.Throws<InvalidOperationException>(() => m.GetHashCode());
  }

  [Test]
  public void ToString_FormatsCorrectly() {
    Matrix m        = new Matrix(new double[,] { { 1, 100 }, { 1000, 10 } });
    string expected = "   1  100" + Environment.NewLine + "1000   10" + Environment.NewLine;
    Assert.That(m.ToString(), Is.EqualTo(expected));

    Matrix   m2    = new Matrix(new double[,] { { 1.23, 2.3 }, { 33.444, 4.0 } });
    string   str2  = m2.ToString();
    string[] lines = str2.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
    Assert.That(lines.Length, Is.EqualTo(2));        // 2 rows
    Assert.That(lines[0].Contains("1.23"), Is.True); // Check if elements are present
    Assert.That(lines[0].Contains("2.3"), Is.True);
    Assert.That(lines[1].Contains("33.444"), Is.True);
    Assert.That(lines[1].Contains("4"), Is.True);
  }
#endregion

#region Operators
  [Test]
  public void Operator_UnaryMinus() {
    Matrix m      = new Matrix(new double[,] { { 1, -2 }, { 3, 0 } });
    Matrix result = -m;
    Assert.Multiple
      (()
         => {
         Assert.That(result[0, 0], Is.EqualTo(-1));
         Assert.That(result[0, 1], Is.EqualTo(2));
         Assert.That(result[1, 0], Is.EqualTo(-3));
         Assert.That(result[1, 1], Is.EqualTo(0));
         Assert.That(result.Rows, Is.EqualTo(m.Rows));
         Assert.That(result.Cols, Is.EqualTo(m.Cols));
       }
      );
  }

  [Test]
  public void Operator_Addition() {
    Matrix m1     = new Matrix(new double[,] { { 1, 2 }, { 3, 4 } });
    Matrix m2     = new Matrix(new double[,] { { 5, 6 }, { 7, 8 } });
    Matrix result = m1 + m2;
    Assert.Multiple
      (()
         => {
         Assert.That(result[0, 0], Is.EqualTo(6));
         Assert.That(result[0, 1], Is.EqualTo(8));
         Assert.That(result[1, 0], Is.EqualTo(10));
         Assert.That(result[1, 1], Is.EqualTo(12));
       }
      );
  }

  [Test]
  public void Operator_Subtraction() {
    Matrix m1     = new Matrix(new double[,] { { 5, 8 }, { 10, 12 } });
    Matrix m2     = new Matrix(new double[,] { { 1, 2 }, { 3, 4 } });
    Matrix result = m1 - m2;
    Assert.Multiple
      (()
         => {
         Assert.That(result[0, 0], Is.EqualTo(4));
         Assert.That(result[0, 1], Is.EqualTo(6));
         Assert.That(result[1, 0], Is.EqualTo(7));
         Assert.That(result[1, 1], Is.EqualTo(8));
       }
      );
  }

  [Test]
  public void Operator_ScalarMultiplication_Left() {
    double scalar = 3;
    Matrix m      = new Matrix(new double[,] { { 1, 2 }, { 3, 4 } });
    Matrix result = scalar * m;
    Assert.Multiple
      (()
         => {
         Assert.That(result[0, 0], Is.EqualTo(3));
         Assert.That(result[0, 1], Is.EqualTo(6));
         Assert.That(result[1, 0], Is.EqualTo(9));
         Assert.That(result[1, 1], Is.EqualTo(12));
       }
      );
  }

  [Test]
  public void Operator_ScalarMultiplication_Right() {
    double scalar = 3;
    Matrix m      = new Matrix(new double[,] { { 1, 2 }, { 3, 4 } });
    Matrix result = m * scalar; // Uses the left multiplication operator
    Assert.Multiple
      (()
         => {
         Assert.That(result[0, 0], Is.EqualTo(3));
         Assert.That(result[0, 1], Is.EqualTo(6));
         Assert.That(result[1, 0], Is.EqualTo(9));
         Assert.That(result[1, 1], Is.EqualTo(12));
       }
      );
  }

  [Test]
  public void Operator_ScalarDivision() {
    double scalar = 2;
    Matrix m      = new Matrix(new double[,] { { 2, 4 }, { 6, 8 } });
    Matrix result = m / scalar;
    Assert.Multiple
      (()
         => {
         Assert.That(result[0, 0], Is.EqualTo(1));
         Assert.That(result[0, 1], Is.EqualTo(2));
         Assert.That(result[1, 0], Is.EqualTo(3));
         Assert.That(result[1, 1], Is.EqualTo(4));
       }
      );
  }

  [Test]
  public void Operator_MatrixMultiplication() {
    Matrix m1     = new Matrix(new double[,] { { 1, 2, 3 }, { 4, 5, 6 } });        // 2x3
    Matrix m2     = new Matrix(new double[,] { { 7, 8 }, { 9, 10 }, { 11, 12 } }); // 3x2
    Matrix result = m1 * m2;                                                       // Expected 2x2
    Assert.Multiple
      (()
         => {
         Assert.That(result.Rows, Is.EqualTo(2));
         Assert.That(result.Cols, Is.EqualTo(2));
         // 1*7 + 2*9 + 3*11 = 7 + 18 + 33 = 58
         // 1*8 + 2*10 + 3*12 = 8 + 20 + 36 = 64
         // 4*7 + 5*9 + 6*11 = 28 + 45 + 66 = 139
         // 4*8 + 5*10 + 6*12 = 32 + 50 + 72 = 154
         Assert.That(result[0, 0], Is.EqualTo(58));
         Assert.That(result[0, 1], Is.EqualTo(64));
         Assert.That(result[1, 0], Is.EqualTo(139));
         Assert.That(result[1, 1], Is.EqualTo(154));
       }
      );
  }

  [Test]
  public void Operator_MatrixVectorMultiplication() {
    Matrix m      = new Matrix(new double[,] { { 1, 2, 3 }, { 4, 5, 6 } }); // 2x3
    Vector v      = V(7, 8, 9);                                             // 3x1 (as column vector)
    Vector result = m * v;                                                  // Expected 2x1

    Assert.Multiple
      (()
         => {
         Assert.That(result.SpaceDim, Is.EqualTo(2));
         // 1*7 + 2*8 + 3*9 = 7 + 16 + 27 = 50
         // 4*7 + 5*8 + 6*9 = 28 + 40 + 54 = 122
         Assert.That(result[0], Is.EqualTo(50));
         Assert.That(result[1], Is.EqualTo(122));
       }
      );
  }

  [Test]
  public void HCat_TwoMatrices_Valid() {
    Matrix  m1     = new Matrix(new double[,] { { 1, 2 }, { 3, 4 } });
    Matrix  m2     = new Matrix(new double[,] { { 5, 6 }, { 7, 8 } });
    Matrix? result = Matrix.hcat(m1, m2);

    Assert.IsNotNull(result);
    Assert.Multiple
      (()
         => {
         Assert.That(result.Rows, Is.EqualTo(2));
         Assert.That(result.Cols, Is.EqualTo(4));
         Assert.That(result[0, 0], Is.EqualTo(1));
         Assert.That(result[0, 1], Is.EqualTo(2));
         Assert.That(result[0, 2], Is.EqualTo(5));
         Assert.That(result[0, 3], Is.EqualTo(6));
         Assert.That(result[1, 0], Is.EqualTo(3));
         Assert.That(result[1, 1], Is.EqualTo(4));
         Assert.That(result[1, 2], Is.EqualTo(7));
         Assert.That(result[1, 3], Is.EqualTo(8));
       }
      );
  }

  [Test]
  public void HCat_MatrixAndVector_Valid() {
    Matrix m      = new Matrix(new double[,] { { 1, 2 }, { 3, 4 } });
    Vector v      = V(5, 6); // Vector has 2 elements, matches matrix rows
    Matrix result = Matrix.hcat(m, v);

    Assert.Multiple
      (()
         => {
         Assert.That(result.Rows, Is.EqualTo(2));
         Assert.That(result.Cols, Is.EqualTo(3));
         Assert.That(result[0, 0], Is.EqualTo(1));
         Assert.That(result[0, 1], Is.EqualTo(2));
         Assert.That(result[0, 2], Is.EqualTo(5));
         Assert.That(result[1, 0], Is.EqualTo(3));
         Assert.That(result[1, 1], Is.EqualTo(4));
         Assert.That(result[1, 2], Is.EqualTo(6));
       }
      );
  }

  [Test]
  public void HCat_InstanceMethod_MatrixAndVector_Valid() {
    Matrix m      = new Matrix(new double[,] { { 1, 2 }, { 3, 4 } });
    Vector v      = V(5, 6);
    Matrix result = m.hcat(v); // Instance method call

    Assert.Multiple
      (()
         => {
         Assert.That(result.Rows, Is.EqualTo(2));
         Assert.That(result.Cols, Is.EqualTo(3));
         Assert.That(result[0, 2], Is.EqualTo(5));
         Assert.That(result[1, 2], Is.EqualTo(6));
       }
      );
  }

  [Test]
  public void VCat_MatrixAndVector_Valid() {
    Matrix m      = new Matrix(new double[,] { { 1, 2 }, { 3, 4 } });
    Vector v      = V(5, 6); // Vector has 2 elements, matches matrix cols
    Matrix result = Matrix.vcat(m, v);

    Assert.Multiple
      (()
         => {
         Assert.That(result.Rows, Is.EqualTo(3));
         Assert.That(result.Cols, Is.EqualTo(2));
         Assert.That(result[0, 0], Is.EqualTo(1));
         Assert.That(result[1, 1], Is.EqualTo(4));
         Assert.That(result[2, 0], Is.EqualTo(5));
         Assert.That(result[2, 1], Is.EqualTo(6));
       }
      );
  }

  [Test]
  public void VCat_InstanceMethod_MatrixAndVector_Valid() {
    Matrix m      = new Matrix(new double[,] { { 1, 2 }, { 3, 4 } });
    Vector v      = V(5, 6);
    Matrix result = m.vcat(v); // Instance method call

    Assert.Multiple
      (()
         => {
         Assert.That(result.Rows, Is.EqualTo(3));
         Assert.That(result.Cols, Is.EqualTo(2));
         Assert.That(result[2, 0], Is.EqualTo(5));
         Assert.That(result[2, 1], Is.EqualTo(6));
       }
      );
  }

  [Test]
  public void VCat_TwoMatrices_Valid() {
    Matrix m1     = new Matrix(new double[,] { { 1, 2 }, { 3, 4 } });
    Matrix m2     = new Matrix(new double[,] { { 5, 6 }, { 7, 8 } });
    Matrix result = Matrix.vcat(m1, m2);

    Assert.Multiple
      (()
         => {
         Assert.That(result.Rows, Is.EqualTo(4));
         Assert.That(result.Cols, Is.EqualTo(2));
         Assert.That(result[0, 0], Is.EqualTo(1));
         Assert.That(result[1, 1], Is.EqualTo(4));
         Assert.That(result[2, 0], Is.EqualTo(5));
         Assert.That(result[3, 1], Is.EqualTo(8));
       }
      );
  }

  // Debug.Asserts for mismatched dimensions in operators are not tested here.
#endregion

#region Taking Submatrices and Transposition
  [Test]
  public void TakeRows_Valid() {
    Matrix m      = new Matrix(new double[,] { { 1, 2, 3 }, { 4, 5, 6 }, { 7, 8, 9 } });
    Matrix result = m.TakeRows(0, 2); // Take first and third row
    Assert.Multiple
      (()
         => {
         Assert.That(result.Rows, Is.EqualTo(2));
         Assert.That(result.Cols, Is.EqualTo(3));
         Assert.That(result[0, 0], Is.EqualTo(1));
         Assert.That(result[0, 1], Is.EqualTo(2));
         Assert.That(result[0, 2], Is.EqualTo(3));
         Assert.That(result[1, 0], Is.EqualTo(7));
         Assert.That(result[1, 1], Is.EqualTo(8));
         Assert.That(result[1, 2], Is.EqualTo(9));
       }
      );
  }

  [Test]
  public void TakeCols_Valid() {
    Matrix m      = new Matrix(new double[,] { { 1, 2, 3 }, { 4, 5, 6 }, { 7, 8, 9 } });
    Matrix result = m.TakeCols(0, 2); // Take first and third column
    Assert.Multiple
      (()
         => {
         Assert.That(result.Rows, Is.EqualTo(3));
         Assert.That(result.Cols, Is.EqualTo(2));
         Assert.That(result[0, 0], Is.EqualTo(1));
         Assert.That(result[0, 1], Is.EqualTo(3));
         Assert.That(result[1, 0], Is.EqualTo(4));
         Assert.That(result[1, 1], Is.EqualTo(6));
         Assert.That(result[2, 0], Is.EqualTo(7));
         Assert.That(result[2, 1], Is.EqualTo(9));
       }
      );
  }

  [Test]
  public void TakeSubMatrix_RowsAndCols_Valid() {
    Matrix m      = new Matrix(new double[,] { { 1, 2, 3, 10 }, { 4, 5, 6, 11 }, { 7, 8, 9, 12 } });
    Matrix result = m.TakeSubMatrix(new int[] { 0, 2 }, new int[] { 1, 3 }); // Rows 0,2 and Cols 1,3
    // Expected:
    // 2  10
    // 8  12
    Assert.Multiple
      (()
         => {
         Assert.That(result.Rows, Is.EqualTo(2));
         Assert.That(result.Cols, Is.EqualTo(2));
         Assert.That(result[0, 0], Is.EqualTo(2));
         Assert.That(result[0, 1], Is.EqualTo(10));
         Assert.That(result[1, 0], Is.EqualTo(8));
         Assert.That(result[1, 1], Is.EqualTo(12));
       }
      );
  }

  [Test]
  public void TakeSubMatrix_AllRows_SpecificCols_Valid() {
    Matrix m      = new Matrix(new double[,] { { 1, 2, 3 }, { 4, 5, 6 } });
    Matrix result = m.TakeSubMatrix(null, new int[] { 0, 2 }); // All rows, cols 0 and 2
    // Expected:
    // 1 3
    // 4 6
    Assert.Multiple
      (()
         => {
         Assert.That(result.Rows, Is.EqualTo(2));
         Assert.That(result.Cols, Is.EqualTo(2));
         Assert.That(result[0, 0], Is.EqualTo(1));
         Assert.That(result[0, 1], Is.EqualTo(3));
         Assert.That(result[1, 0], Is.EqualTo(4));
         Assert.That(result[1, 1], Is.EqualTo(6));
       }
      );
  }

  [Test]
  public void TakeSubMatrix_SpecificRows_AllCols_Valid() {
    Matrix m      = new Matrix(new double[,] { { 1, 2, 3 }, { 4, 5, 6 }, { 7, 8, 9 } });
    Matrix result = m.TakeSubMatrix(new int[] { 0, 2 }, null); // Rows 0 and 2, all columns
    // Expected:
    // 1 2 3
    // 7 8 9
    Assert.Multiple
      (()
         => {
         Assert.That(result.Rows, Is.EqualTo(2));
         Assert.That(result.Cols, Is.EqualTo(3));
         Assert.That(result[0, 0], Is.EqualTo(1));
         Assert.That(result[0, 1], Is.EqualTo(2));
         Assert.That(result[0, 2], Is.EqualTo(3));
         Assert.That(result[1, 0], Is.EqualTo(7));
         Assert.That(result[1, 1], Is.EqualTo(8));
         Assert.That(result[1, 2], Is.EqualTo(9));
       }
      );
  }

  [Test]
  public void TakeSubMatrix_AllRows_AllCols_ReturnsCopy() {
    Matrix m      = new Matrix(new double[,] { { 1, 2 }, { 3, 4 } });
    Matrix result = m.TakeSubMatrix(null, null);
    Assert.That(m, Is.EqualTo(result), "Taking all rows and all columns should produce an equal matrix.");
    Assert.That(ReferenceEquals(m, result), Is.False, "Taking all rows and all columns should produce a new instance.");
  }

  [Test]
  public void TakeRowVector_Valid() {
    Matrix m       = new Matrix(new double[,] { { 1, 2, 3 }, { 4, 5, 6 } });
    Vector rowVec0 = m.TakeRowVector(0);
    Vector rowVec1 = m.TakeRowVector(1);
    Assert.That(rowVec0, Is.EqualTo(V(1, 2, 3)));
    Assert.That(rowVec1, Is.EqualTo(V(4, 5, 6)));
  }

  [Test]
  public void TakeColumnVector_Valid() {
    Matrix m       = new Matrix(new double[,] { { 1, 2, 3 }, { 4, 5, 6 } });
    Vector colVec0 = m.TakeColumnVector(0);
    Vector colVec1 = m.TakeColumnVector(1);
    Vector colVec2 = m.TakeColumnVector(2);
    Assert.That(colVec0, Is.EqualTo(V(1, 4)));
    Assert.That(colVec1, Is.EqualTo(V(2, 5)));
    Assert.That(colVec2, Is.EqualTo(V(3, 6)));
  }

  [Test]
  public void Transpose_Valid() {
    Matrix m  = new Matrix(new double[,] { { 1, 2, 3 }, { 4, 5, 6 } }); // 2x3
    Matrix mT = m.Transpose();                                          // Expected 3x2
    Assert.Multiple
      (()
         => {
         Assert.That(mT.Rows, Is.EqualTo(3));
         Assert.That(mT.Cols, Is.EqualTo(2));
         Assert.That(mT[0, 0], Is.EqualTo(1));
         Assert.That(mT[0, 1], Is.EqualTo(4));
         Assert.That(mT[1, 0], Is.EqualTo(2));
         Assert.That(mT[1, 1], Is.EqualTo(5));
         Assert.That(mT[2, 0], Is.EqualTo(3));
         Assert.That(mT[2, 1], Is.EqualTo(6));
       }
      );
  }

  [Test]
  public void Transpose_SquareMatrix() {
    Matrix m  = new Matrix(new double[,] { { 1, 2 }, { 3, 4 } });
    Matrix mT = m.Transpose();
    Assert.Multiple
      (()
         => {
         Assert.That(mT[0, 0], Is.EqualTo(1));
         Assert.That(mT[0, 1], Is.EqualTo(3));
         Assert.That(mT[1, 0], Is.EqualTo(2));
         Assert.That(mT[1, 1], Is.EqualTo(4));
       }
      );
  }

  [Test]
  public void Transpose_RowVectorMatrix() {
    Matrix m  = new Matrix(V(1, 2, 3)); // 1x3
    Matrix mT = m.Transpose();          // Expected 3x1
    Assert.Multiple
      (()
         => {
         Assert.That(mT.Rows, Is.EqualTo(3));
         Assert.That(mT.Cols, Is.EqualTo(1));
         Assert.That(mT[0, 0], Is.EqualTo(1));
         Assert.That(mT[1, 0], Is.EqualTo(2));
         Assert.That(mT[2, 0], Is.EqualTo(3));
       }
      );
  }

  [Test]
  public void Transpose_ColumnVectorMatrix() {
    Matrix m  = new Matrix(new double[,] { { 1 }, { 2 }, { 3 } }); // 3x1
    Matrix mT = m.Transpose();                                     // Expected 1x3
    Assert.Multiple
      (()
         => {
         Assert.That(mT.Rows, Is.EqualTo(1));
         Assert.That(mT.Cols, Is.EqualTo(3));
         Assert.That(mT[0, 0], Is.EqualTo(1));
         Assert.That(mT[0, 1], Is.EqualTo(2));
         Assert.That(mT[0, 2], Is.EqualTo(3));
       }
      );
  }
#endregion

#region Matrix Factories
  [Test]
  public void Zero_SquareMatrix() {
    Matrix m = Matrix.Zero(2);
    Assert.Multiple
      (()
         => {
         Assert.That(m.Rows, Is.EqualTo(2));
         Assert.That(m.Cols, Is.EqualTo(2));
         Assert.That(m[0, 0], Is.EqualTo(Tools.Zero));
         Assert.That(m[0, 1], Is.EqualTo(Tools.Zero));
         Assert.That(m[1, 0], Is.EqualTo(Tools.Zero));
         Assert.That(m[1, 1], Is.EqualTo(Tools.Zero));
       }
      );
  }

  [Test]
  public void Zero_RectangularMatrix() {
    Matrix m = Matrix.Zero(2, 3);
    Assert.Multiple
      (()
         => {
         Assert.That(m.Rows, Is.EqualTo(2));
         Assert.That(m.Cols, Is.EqualTo(3));
         for (int r = 0; r < m.Rows; r++) {
           for (int c = 0; c < m.Cols; c++) {
             Assert.That(m[r, c], Is.EqualTo(Tools.Zero), $"Element at ({r},{c}) should be zero.");
           }
         }
       }
      );
  }

  [Test]
  public void One_SquareMatrix() {
    Matrix m = Matrix.One(2);
    Assert.Multiple
      (()
         => {
         Assert.That(m.Rows, Is.EqualTo(2));
         Assert.That(m.Cols, Is.EqualTo(2));
         Assert.That(m[0, 0], Is.EqualTo(Tools.One));
         Assert.That(m[0, 1], Is.EqualTo(Tools.One));
         Assert.That(m[1, 0], Is.EqualTo(Tools.One));
         Assert.That(m[1, 1], Is.EqualTo(Tools.One));
       }
      );
  }

  [Test]
  public void One_RectangularMatrix() {
    Matrix m = Matrix.One(2, 3);
    Assert.Multiple
      (()
         => {
         Assert.That(m.Rows, Is.EqualTo(2));
         Assert.That(m.Cols, Is.EqualTo(3));
         for (int r = 0; r < m.Rows; r++) {
           for (int c = 0; c < m.Cols; c++) {
             Assert.That(m[r, c], Is.EqualTo(Tools.One), $"Element at ({r},{c}) should be one.");
           }
         }
       }
      );
  }

  [Test]
  public void Eye_SquareMatrix() {
    Matrix    m        = Matrix.Eye(3);
    double[,] expected = { { 1, 0, 0 }, { 0, 1, 0 }, { 0, 0, 1 } };
    Assert.Multiple
      (()
         => {
         Assert.That(m.Rows, Is.EqualTo(3));
         Assert.That(m.Cols, Is.EqualTo(3));
         for (int r = 0; r < 3; r++)
         for (int c = 0; c < 3; c++)
           Assert.That(m[r, c], Is.EqualTo(expected[r, c]));
       }
      );
  }

  [Test]
  public void Eye_RectangularMatrix_MoreRows() {
    Matrix    m        = Matrix.Eye(3, 2); // 3 rows, 2 cols
    double[,] expected = { { 1, 0 }, { 0, 1 }, { 0, 0 } };
    Assert.Multiple
      (()
         => {
         Assert.That(m.Rows, Is.EqualTo(3));
         Assert.That(m.Cols, Is.EqualTo(2));
         for (int r = 0; r < 3; r++)
         for (int c = 0; c < 2; c++)
           Assert.That(m[r, c], Is.EqualTo(expected[r, c]));
       }
      );
  }

  [Test]
  public void Eye_RectangularMatrix_MoreCols() {
    Matrix    m        = Matrix.Eye(2, 3); // 2 rows, 3 cols
    double[,] expected = { { 1, 0, 0 }, { 0, 1, 0 } };
    Assert.Multiple
      (()
         => {
         Assert.That(m.Rows, Is.EqualTo(2));
         Assert.That(m.Cols, Is.EqualTo(3));
         for (int r = 0; r < 2; r++)
         for (int c = 0; c < 3; c++)
           Assert.That(m[r, c], Is.EqualTo(expected[r, c]));
       }
      );
  }

  [Test]
  public void GenMatrix_CreatesMatrixWithCorrectDimensionsAndValuesInRange() {
    GRandomLC random = new GRandomLC(123); // Seeded for reproducibility
    Matrix    m      = Matrix.GenMatrix(2, 3, 10.0, 20.0, random);
    Assert.Multiple
      (()
         => {
         Assert.That(m.Rows, Is.EqualTo(2));
         Assert.That(m.Cols, Is.EqualTo(3));
         for (int r = 0; r < m.Rows; r++) {
           for (int c = 0; c < m.Cols; c++) {
             Assert.That(m[r, c], Is.GreaterThanOrEqualTo(10.0).And.LessThan(20.0));
           }
         }
       }
      );
  }

  [Test]
  public void GenMatrixInt_CreatesMatrixWithCorrectDimensionsAndIntegerValuesInRange() {
    GRandomLC random = new GRandomLC(123);
    Matrix    m      = Matrix.GenMatrixInt(2, 3, 5, 10, random); // Range [5, 10) -> integers 5,6,7,8,9
    Assert.Multiple
      (()
         => {
         Assert.That(m.Rows, Is.EqualTo(2));
         Assert.That(m.Cols, Is.EqualTo(3));
         for (int r = 0; r < m.Rows; r++) {
           for (int c = 0; c < m.Cols; c++) {
             double val = m[r, c];
             Assert.That(val, Is.GreaterThanOrEqualTo(5.0).And.LessThan(10.0));
             Assert.That(Tools.EQ(val, Math.Round(val)), Is.True, $"Value {val} should be an integer.");
           }
         }
       }
      );
  }

  [Test]
  public void GenNonSingular_Dim1() {
    Matrix m = Matrix.GenNonSingular(1, 1.0, 2.0);
    Assert.That(m.Rows, Is.EqualTo(1));
    Assert.That(m.Cols, Is.EqualTo(1));
    Assert.That(Tools.NE(m[0, 0]), Is.True, "1x1 non-singular matrix element should not be zero if range allows.");
  }


  [Test]
  public void GenNonSingular_CreatesNonSingularMatrix([Values(2, 3)] int dim) {
    GRandomLC random = new GRandomLC((uint)dim + 42); // Different seed per dimension
    Matrix    m      = Matrix.GenNonSingular(dim, -5.0, 5.0, random);
    Assert.That(m.Rows, Is.EqualTo(dim));
    Assert.That(m.Cols, Is.EqualTo(dim));

    // Check non-singularity by converting to RREF and seeing if it's an identity matrix
    Matrix rref          = m.ToRREF();
    bool   isNonSingular = true;
    for (int i = 0; i < dim; i++) {
      if (!Tools.EQ(rref[i, i], Tools.One)) { // Check diagonal elements are 1
        isNonSingular = false;

        break;
      }
      for (int j = 0; j < dim; j++) { // Check off-diagonal are 0 (for square RREF of non-singular)
        if (i != j && !Tools.EQ(rref[i, j], Tools.Zero)) {
          isNonSingular = false;

          break;
        }
      }

      if (!isNonSingular)
        break;
    }
    Assert.That(isNonSingular, Is.True, $"Generated matrix for dim={dim} should be non-singular (RREF is Identity). RREF:\n{rref}");
  }

  [Test]
  public void GenNonSingular_DefaultRange_CreatesNonSingularMatrix([Values(2, 3)] int dim) {
    GRandomLC random = new GRandomLC((uint)dim + 100);
    Matrix    m      = Matrix.GenNonSingular(dim, random); // Uses range [-0.5, 0.5)
    Assert.That(m.Rows, Is.EqualTo(dim));
    Assert.That(m.Cols, Is.EqualTo(dim));
    // Check that elements are within default range [-0.5, 0.5)
    for (int r = 0; r < m.Rows; r++) {
      for (int c = 0; c < m.Cols; c++) {
        Assert.That(m[r, c], Is.GreaterThanOrEqualTo(-0.5).And.LessThan(0.5), $"Element at ({r},{c}) is out of default range.");
      }
    }
    Matrix rref          = m.ToRREF();
    bool   isNonSingular = true;
    for (int i = 0; i < dim; i++) {
      if (!Tools.EQ(rref[i, i], Tools.One)) {
        isNonSingular = false;

        break;
      }
    }
    Assert.That
      (isNonSingular, Is.True, $"Generated matrix (default range) for dim={dim} should be non-singular (RREF has {dim} pivots).");
  }

  [Test]
  public void GenONMatrix_CreatesOrthonormalMatrix([Values(2, 3)] int dim) {
    GRandomLC random = new GRandomLC((uint)dim + 200);
    Matrix    m      = Matrix.GenONMatrix(dim, random);
    Assert.That(m.Rows, Is.EqualTo(dim));
    Assert.That(m.Cols, Is.EqualTo(dim));

    // Check M * M^T = I
    Matrix mT      = m.Transpose();
    Matrix product = m * mT;
    Matrix eye     = Matrix.Eye(dim);

    bool productIsIdentity = true;
    for (int r = 0; r < dim; r++) {
      for (int c = 0; c < dim; c++) {
        if (!Tools.EQ(product[r, c], eye[r, c])) {
          productIsIdentity = false;

          break;
        }
      }

      if (!productIsIdentity)
        break;
    }
    Assert.That
      (productIsIdentity, Is.True, $"M * M^T should be Identity for ON matrix. Dim={dim}.\nProduct:\n{product}\nEye:\n{eye}");

    // Also check M^T * M = I
    Matrix productT           = mT * m;
    bool   productTIsIdentity = true;
    for (int r = 0; r < dim; r++) {
      for (int c = 0; c < dim; c++) {
        if (!Tools.EQ(productT[r, c], eye[r, c])) {
          productTIsIdentity = false;

          break;
        }
      }

      if (!productTIsIdentity)
        break;
    }
    Assert.That
      (productTIsIdentity, Is.True, $"M^T * M should be Identity for ON matrix. Dim={dim}.\nProduct:\n{productT}\nEye:\n{eye}");
  }

  [Test]
  public void Hilbert_CreatesCorrectMatrix([Values(1, 2, 3)] int dim) {
    Matrix h = Matrix.Hilbert(dim);
    Assert.That(h.Rows, Is.EqualTo(dim));
    Assert.That(h.Cols, Is.EqualTo(dim));
    for (int r = 0; r < dim; r++) {
      for (int c = 0; c < dim; c++) {
        Assert.That(Tools.EQ(h[r, c], Tools.One / (r + c + Tools.One)), Is.True);
      }
    }
  }
#endregion

#region Functions
  [Test]
  public void MultiplyRowByVector_Valid() {
    Matrix m = new Matrix(new double[,] { { 1, 2, 3 }, { 4, 5, 6 } });
    Vector v = V(10, 1, 0.1);

    double resRow0 = m.MultiplyRowByVector(0, v); // 1*10 + 2*1 + 3*0.1 = 10 + 2 + 0.3 = 12.3
    double resRow1 = m.MultiplyRowByVector(1, v); // 4*10 + 5*1 + 6*0.1 = 40 + 5 + 0.6 = 45.6

    Assert.That(Tools.EQ(resRow0, 12.3), Is.True);
    Assert.That(Tools.EQ(resRow1, 45.6), Is.True);
  }

  [Test]
  public void MultiplyRowByDiffOfVectors_Valid() {
    Matrix m  = new Matrix(new double[,] { { 1, 2, 3 }, { 4, 5, 6 } });
    Vector v1 = V(10, 1, 0.1);
    Vector v2 = V(1, 1, 1);
    // v1-v2 = (9,0,-0.9)

    double resRow0 = m.MultiplyRowByDiffOfVectors(0, v1, v2); // 1*9 + 2*0 + 3*(-0.9) = 9 + 0 - 2.7 = 6.3
    double resRow1 = m.MultiplyRowByDiffOfVectors(1, v1, v2); // 4*9 + 5*0 + 6*(-0.9) = 36 + 0 - 5.4 = 30.6

    Assert.That(Tools.EQ(resRow0, 6.3), Is.True);
    Assert.That(Tools.EQ(resRow1, 30.6), Is.True);
  }

  [Test]
  public void MultiplyColumnByVector_Valid() {
    Matrix m = new Matrix(new double[,] { { 1, 4 }, { 2, 5 }, { 3, 6 } }); // 3x2
    Vector v = V(10, 1, 0.1);                                              // Vector for columns, dim = Rows = 3

    double resCol0 = m.MultiplyColumnByVector(0, v); // Col0: (1,2,3). 1*10 + 2*1 + 3*0.1 = 10+2+0.3 = 12.3
    double resCol1 = m.MultiplyColumnByVector(1, v); // Col1: (4,5,6). 4*10 + 5*1 + 6*0.1 = 40+5+0.6 = 45.6

    Assert.That(Tools.EQ(resCol0, 12.3), Is.True);
    Assert.That(Tools.EQ(resCol1, 45.6), Is.True);
  }

  [Test]
  public void MultiplyBySelfTranspose_SquareMatrix() {
    Matrix m = new Matrix(new double[,] { { 1, 2 }, { 3, 4 } }); // m
    // mT = {{1,3},{2,4}}
    // m * mT = {{1,2},{3,4}} * {{1,3},{2,4}} = {{1*1+2*2, 1*3+2*4}, {3*1+4*2, 3*3+4*4}}
    //        = {{1+4, 3+8}, {3+8, 9+16}} = {{5, 11}, {11, 25}}
    Matrix result   = m.MultiplyBySelfTranspose();
    Matrix expected = new Matrix(new double[,] { { 5, 11 }, { 11, 25 } });
    Assert.That(result, Is.EqualTo(expected));
  }

  [Test]
  public void MultiplyBySelfTranspose_RectangularMatrix() {
    Matrix m = new Matrix(new double[,] { { 1, 2, 3 }, { 4, 5, 6 } }); // 2x3
    // mT = {{1,4},{2,5},{3,6}} (3x2)
    // m * mT = {{1,2,3},{4,5,6}} * {{1,4},{2,5},{3,6}}
    //        = {{1*1+2*2+3*3, 1*4+2*5+3*6}, {4*1+5*2+6*3, 4*4+5*5+6*6}}
    //        = {{1+4+9, 4+10+18}, {4+10+18, 16+25+36}}
    //        = {{14, 32}, {32, 77}}
    Matrix result   = m.MultiplyBySelfTranspose();
    Matrix expected = new Matrix(new double[,] { { 14, 32 }, { 32, 77 } });
    Assert.That(result, Is.EqualTo(expected));
  }

  [Test]
  public void MultRowVectorByMatrix_Valid() {
    Vector v = V(1, 2);                                                      // row vector 1x2
    Matrix m = new Matrix(new double[,] { { 10, 11, 12 }, { 20, 21, 22 } }); // 2x3
    // v * m = [1,2] * [[10,11,12],[20,21,22]]
    //       = [1*10+2*20, 1*11+2*21, 1*12+2*22]
    //       = [10+40, 11+42, 12+44]
    //       = [50, 53, 56]
    Vector result   = Matrix.MultRowVectorByMatrix(v, m);
    Vector expected = V(50, 53, 56);
    Assert.That(result, Is.EqualTo(expected));
  }

  [Test]
  public void MultiplyTransposedByVector_Valid() {
    Matrix m = new Matrix(new double[,] { { 1, 2, 3 }, { 4, 5, 6 } }); // 2x3
    // mT = {{1,4},{2,5},{3,6}} (3x2)
    Vector v = V(10, 1); // vector dim = m.Rows = 2
    // mT * v = {{1,4},{2,5},{3,6}} * {{10},{1}}
    //        = {{1*10+4*1},{2*10+5*1},{3*10+6*1}}
    //        = {{10+4},{20+5},{30+6}}
    //        = {{14},{25},{36}}
    Vector result   = m.MultiplyTransposedByVector(v);
    Vector expected = V(14, 25, 36);
    Assert.That(result, Is.EqualTo(expected));
  }

  [Test]
  public void ToRREF_IdentityMatrix_ReturnsIdentity() {
    Matrix m    = Matrix.Eye(3);
    Matrix rref = m.ToRREF();
    Assert.That(rref, Is.EqualTo(Matrix.Eye(3)));
  }

  [Test]
  public void ToRREF_ZeroMatrix_ReturnsZero() {
    Matrix m    = Matrix.Zero(2, 3);
    Matrix rref = m.ToRREF();
    Assert.That(rref, Is.EqualTo(Matrix.Zero(2, 3)));
  }

  [Test]
  public void ToRREF_SimpleMatrix1() {
    Matrix m = new Matrix(new double[,] { { 1, 2, 3 }, { 4, 5, 6 } });
    // Expected RREF:
    // [1 2  3] ~ [1  2  3] ~ [1  2  3] ~ [1  0 -1]
    // [4 5  6]   [0 -3 -6]   [0  1  2]   [0  1  2]
    // R2 = R2 - 4*R1
    // R2 = R2 / -3
    // R1 = R1 - 2*R2
    Matrix expectedRREF = new Matrix(new double[,] { { 1, 0, -1 }, { 0, 1, 2 } });
    Matrix rref         = m.ToRREF();
    Assert.That(rref, Is.EqualTo(expectedRREF), $"RREF was:\n{rref}\nExpected:\n{expectedRREF}");
  }

  [Test]
  public void ToRREF_SimpleMatrix2_RequiresRowSwap() {
    Matrix m = new Matrix(new double[,] { { 0, 1, 2 }, { 1, 2, 1 } });
    // Expected RREF:
    // [0 1 2] ~ [1 2 1] ~ [1 2 1] ~ [1 0 -3]
    // [1 2 1]   [0 1 2]   [0 1 2]   [0 1  2]
    // Swap R1, R2
    // R1 = R1 - 2*R2
    Matrix expectedRREF = new Matrix(new double[,] { { 1, 0, -3 }, { 0, 1, 2 } });
    Matrix rref         = m.ToRREF();
    Assert.That(rref, Is.EqualTo(expectedRREF), $"RREF was:\n{rref}\nExpected:\n{expectedRREF}");
  }

  [Test]
  public void ToRREF_MatrixWithZeroColumn() {
    Matrix m = new Matrix(new double[,] { { 1, 0, 2 }, { 2, 0, 3 } });
    // Expected RREF:
    // [1 0 2] ~ [1 0  2] ~ [1 0 0]
    // [2 0 3]   [0 0 -1] ~ [0 0 1]
    // R2 = R2 - 2*R1
    // R2 = R2 / -1
    // R1 = R1 - 2*R2
    Matrix expectedRREF = new Matrix(new double[,] { { 1, 0, 0 }, { 0, 0, 1 } });
    Matrix rref         = m.ToRREF();
    Assert.That(rref, Is.EqualTo(expectedRREF), $"RREF was:\n{rref}\nExpected:\n{expectedRREF}");
  }

  [Test]
  public void ToRREF_MatrixAlreadyInRREF() {
    Matrix m    = new Matrix(new double[,] { { 1, 0, 5, 0 }, { 0, 1, -2, 0 }, { 0, 0, 0, 1 } });
    Matrix rref = m.ToRREF();
    Assert.That(rref, Is.EqualTo(m));
  }

  [Test]
  public void ToRREF_MatrixLeadsToZeroRows() {
    Matrix m = new Matrix(new double[,] { { 1, 2, 3 }, { 2, 4, 6 }, { 3, 6, 9 } });
    // Expected RREF:
    // [1 2 3]
    // [0 0 0]
    // [0 0 0]
    Matrix expectedRREF = new Matrix(new double[,] { { 1, 2, 3 }, { 0, 0, 0 }, { 0, 0, 0 } });
    Matrix rref         = m.ToRREF();
    Assert.That(rref, Is.EqualTo(expectedRREF), $"RREF was:\n{rref}\nExpected:\n{expectedRREF}");
  }

  [Test]
  public void ToRREF_TallMatrix() { // More rows than columns
    Matrix m = new Matrix(new double[,] { { 1, 1 }, { 2, 2 }, { 1, 0 } });
    // [1 1]     [1 1]     [1 1]     [1 0]
    // [2 2] ~R2-2R1~ [0 0] ~swapR2,R3~ [0 -1] ~-R2~ [0 1]
    // [1 0]     [0 -1]    [0 0]     [0 0]
    // Then R1-R2
    Matrix expectedRREF = new Matrix(new double[,] { { 1, 0 }, { 0, 1 }, { 0, 0 } });
    Matrix rref         = m.ToRREF();
    Assert.That(rref, Is.EqualTo(expectedRREF), $"RREF was:\n{rref}\nExpected:\n{expectedRREF}");
  }

  [Test]
  public void ToRREF_WideMatrix() { // More columns than rows
    Matrix m = new Matrix(new double[,] { { 1, 2, 3, 4 }, { 2, 4, 7, 9 } });
    // [1 2 3 4]  ~R2-2R1~ [1 2 3 4] ~R1-3R2~ [1 2 0 1]
    // [2 4 7 9]           [0 0 1 1]          [0 0 1 1]
    Matrix expectedRREF = new Matrix(new double[,] { { 1, 2, 0, 1 }, { 0, 0, 1, 1 } });
    Matrix rref         = m.ToRREF();
    Assert.That(rref, Is.EqualTo(expectedRREF), $"RREF was:\n{rref}\nExpected:\n{expectedRREF}");
  }

  [Test]
  public void ToRREF_IdentityMatrix() {
    Matrix id   = Matrix.Eye(3);
    Matrix rref = id.ToRREF();
    AssertMatricesAreEqual(rref, id, "RREF of identity matrix should be identity.");
    AssertIsRREF(rref, "Identity matrix check");
  }

  [Test]
  public void ToRREF_ZeroMatrix() {
    Matrix zero = Matrix.Zero(2, 3);
    Matrix rref = zero.ToRREF();
    AssertMatricesAreEqual(rref, zero, "RREF of zero matrix should be zero.");
    AssertIsRREF(rref, "Zero matrix check");
  }


  [Test]
  public void ToRREF_SingleElementMatrix() {
    Matrix m1 = M(new[] { 5.0 });
    AssertMatricesAreEqual(m1.ToRREF(), M(new[] { 1.0 }));

    Matrix m2 = M(new[] { 0.0 });
    AssertMatricesAreEqual(m2.ToRREF(), M(new[] { 0.0 }));
  }

  [Test]
  public void ToRREF_OriginalMatrixUntouched() {
    Matrix original = M(new[] { 1.0, 2.0 }, new[] { 3.0, 4.0 });
    Matrix copy     = M(new[] { 1.0, 2.0 }, new[] { 3.0, 4.0 });
    original.ToRREF();
    AssertMatricesAreEqual(original, copy);
  }

  [Test]
  public void ToRREF_SquareInvertibleMatrix() {
    Matrix m        = M(new[] { 1.0, 2.0 }, new[] { 3.0, 4.0 });
    Matrix expected = Matrix.Eye(2);
    Matrix rref     = m.ToRREF();
    AssertMatricesAreEqual(rref, expected, "RREF of invertible 2x2 matrix.");
    AssertIsRREF(rref);
  }

  [Test]
  public void ToRREF_SquareSingularMatrix() {
    Matrix m        = M(new[] { 1.0, 2.0 }, new[] { 2.0, 4.0 });
    Matrix expected = M(new[] { 1.0, 2.0 }, new[] { 0.0, 0.0 });
    Matrix rref     = m.ToRREF();
    AssertMatricesAreEqual(rref, expected, "RREF of singular 2x2 matrix.");
    AssertIsRREF(rref);
  }

  [Test]
  public void ToRREF_SquareNeedsRowSwap() {
    Matrix m        = M(new[] { 0.0, 1.0 }, new[] { 1.0, 0.0 });
    Matrix expected = Matrix.Eye(2);
    Matrix rref     = m.ToRREF();
    AssertMatricesAreEqual(rref, expected, "RREF needing row swap.");
    AssertIsRREF(rref);
  }

  [Test]
  public void ToRREF_SquareMoreComplex() {
    Matrix m        = M(new[] { 1.0, 2.0, 3.0 }, new[] { 2.0, 5.0, 8.0 }, new[] { 1.0, 1.0, 1.0 });
    Matrix expected = M(new[] { 1.0, 0.0, -1.0 }, new[] { 0.0, 1.0, 2.0 }, new[] { 0.0, 0.0, 0.0 });
    Matrix rref     = m.ToRREF();
    AssertMatricesAreEqual(rref, expected, "RREF of 3x3 matrix.");
    AssertIsRREF(rref);
  }

  [Test]
  public void ToRREF_WideMatrix_FullRank() {
    Matrix m        = M(new[] { 1.0, 0.0, 2.0 }, new[] { 0.0, 1.0, 3.0 });
    Matrix expected = M(new[] { 1.0, 0.0, 2.0 }, new[] { 0.0, 1.0, 3.0 });
    Matrix rref     = m.ToRREF();
    AssertMatricesAreEqual(rref, expected, "RREF of wide matrix (full rank).");
    AssertIsRREF(rref);
  }

  [Test]
  public void ToRREF_WideMatrix_RankDeficient() {
    Matrix m        = M(new[] { 1.0, 1.0, 2.0, 3.0 }, new[] { 2.0, 2.0, 5.0, 7.0 });
    Matrix expected = M(new[] { 1.0, 1.0, 0.0, 1.0 }, new[] { 0.0, 0.0, 1.0, 1.0 });
    Matrix rref     = m.ToRREF();
    AssertMatricesAreEqual(rref, expected, "RREF of wide matrix (rank deficient).");
    AssertIsRREF(rref);
  }

  [Test]
  public void ToRREF_TallMatrix_FullRank() {
    Matrix m        = M(new[] { 1.0, 0.0 }, new[] { 0.0, 1.0 }, new[] { 2.0, 3.0 });
    Matrix expected = Matrix.Eye(3, 2);
    Matrix rref     = m.ToRREF();
    AssertMatricesAreEqual(rref, expected, "RREF of tall matrix (full rank).");
    AssertIsRREF(rref);
  }

  [Test]
  public void ToRREF_TallMatrix_RankDeficient() {
    Matrix m        = M(new[] { 1.0, 1.0 }, new[] { 2.0, 2.0 }, new[] { 3.0, 3.0 });
    Matrix expected = M(new[] { 1.0, 1.0 }, new[] { 0.0, 0.0 }, new[] { 0.0, 0.0 });
    Matrix rref     = m.ToRREF();
    AssertMatricesAreEqual(rref, expected, "RREF of tall matrix (rank deficient).");
    AssertIsRREF(rref);
  }

  [Test]
  public void ToRREF_MatrixRequiringPartialPivoting() {
    Matrix m        = M(new[] { Tools.Eps, 1.0 }, new[] { 1.0, 1.0 });
    Matrix expected = Matrix.Eye(2);
    Matrix rref     = m.ToRREF();
    AssertMatricesAreEqual(rref, expected, "RREF requiring partial pivoting.");
    AssertIsRREF(rref);
  }

  [Test]
  public void ToRREF_NegativePivotAndValues() {
    Matrix m        = M(new[] { -2.0, 4, -6 }, new[] { 3.0, -6, 9 });
    Matrix expected = M(new[] { 1.0, -2, 3 }, new[] { 0.0, 0, 0 });
    Matrix rref     = m.ToRREF();
    AssertMatricesAreEqual(rref, expected);
    AssertIsRREF(rref);
  }

  // Helper для сравнения матриц с точностью
  private static void AssertMatricesAreEqual(Matrix actual, Matrix expected, string message = "") {
    Assert.That(actual.Rows, Is.EqualTo(expected.Rows), $"Matrix row counts differ. {message}");
    Assert.That(actual.Cols, Is.EqualTo(expected.Cols), $"Matrix column counts differ. {message}");
    for (int i = 0; i < actual.Rows; i++) {
      for (int j = 0; j < actual.Cols; j++) {
        Assert.That
          (
           Tools.EQ(actual[i, j], expected[i, j])
         , Is.True
         , $"Matrices differ at [{i},{j}]. Expected: {expected[i, j]:F15}, Got: {actual[i, j]:F15}. {message}"
          );
      }
    }
  }


  // Helper для проверки, является ли матрица в RREF
  private static void AssertIsRREF(Matrix m, string message = "") {
    int leadCol = -1;                // Индекс столбца предыдущего ведущего элемента
    for (int i = 0; i < m.Rows; i++) // Идем по строкам
    {
      int currentRowLeadCol = -1;
      // Находим первый ненулевой элемент (ведущий) в строке i
      for (int j = 0; j < m.Cols; j++) {
        if (!Tools.EQ(m[i, j])) {
          currentRowLeadCol = j;

          break;
        }
      }

      if (currentRowLeadCol != -1) // Если строка не нулевая
      {
        // 1. Ведущий элемент должен быть правее ведущего элемента предыдущей строки
        Assert.That
          (
           currentRowLeadCol
         , Is.GreaterThan(leadCol)
         , $"Leading entry in row {i} (col {currentRowLeadCol}) is not to the right of previous leading entry (col {leadCol}). {message}"
          );
        // 2. Ведущий элемент должен быть равен 1
        Assert.That
          (
           Tools.EQ(m[i, currentRowLeadCol], 1.0)
         , Is.True
         , $"Leading entry at [{i},{currentRowLeadCol}] is {m[i, currentRowLeadCol]}, not 1. {message}"
          );
        // 3. Все остальные элементы в столбце ведущего элемента должны быть 0
        for (int rowIdx = 0; rowIdx < m.Rows; rowIdx++) {
          if (rowIdx != i) {
            Assert.That
              (
               Tools.EQ(m[rowIdx, currentRowLeadCol], 0.0)
             , Is.True
             , $"Entry at [{rowIdx},{currentRowLeadCol}] should be 0 (column of leading entry [{i},{currentRowLeadCol}]). Found {m[rowIdx, currentRowLeadCol]}. {message}"
              );
          }
        }
        leadCol = currentRowLeadCol; // Обновляем столбец последнего ведущего элемента
      }
      else // Строка нулевая
      {
        // Все последующие строки тоже должны быть нулевыми (это следует из п.1, но можно проверить явно)
        for (int rowIdx = i + 1; rowIdx < m.Rows; rowIdx++) {
          for (int colIdx = 0; colIdx < m.Cols; colIdx++) {
            Assert.That
              (
               Tools.EQ(m[rowIdx, colIdx], 0.0)
             , Is.True
             , $"Row {rowIdx} should be zero because row {i} is zero. Found non-zero at [{rowIdx},{colIdx}]. {message}"
              );
          }
        }

        break; // Остальные строки нулевые, выходим из внешнего цикла
      }
    }
  }

  private static Matrix M(params double[][] rows) {
    int numRows = rows.Length;

    if (numRows == 0)
      throw new ArgumentException("Cannot create a matrix with zero rows using this helper.");

    int numCols = rows[0].Length;

    if (numCols == 0)
      throw new ArgumentException("Cannot create a matrix with zero columns using this helper.");

    double[] data1d = new double[numRows * numCols];
    int      k      = 0;
    for (int i = 0; i < numRows; i++) {
      Assert.That(rows[i].Length, Is.EqualTo(numCols), $"Row {i} has wrong number of columns.");
      for (int j = 0; j < numCols; j++) {
        data1d[k++] = rows[i][j];
      }
    }

    return new Matrix(numRows, numCols, data1d, needCopy: false);
  }
#endregion

}
