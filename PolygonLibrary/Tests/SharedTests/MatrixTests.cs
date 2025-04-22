using NUnit.Framework;
using CGLibrary;
using static CGLibrary.Geometry<double, Tests.DConvertor>;
using static Tests.SharedTests.StaticHelpers;
using System;

namespace Tests.SharedTests;

[TestFixture]
public class MatrixTests {

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

#region RREF Tests
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
  public void ToRREF_MatrixWithZeroColumn() {
    Matrix m        = M(new[] { 1.0, 0.0, 2.0 }, new[] { 0.0, 0.0, 0.0 }, new[] { 0.0, 0.0, 1.0 });
    Matrix expected = M(new[] { 1.0, 0.0, 0.0 }, new[] { 0.0, 0.0, 1.0 }, new[] { 0.0, 0.0, 0.0 });
    Matrix rref     = m.ToRREF();
    AssertMatricesAreEqual(rref, expected, "RREF with zero column.");
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
  public void ToRREF_MatrixAlreadyInRREF() {
    Matrix m    = M(new[] { 1.0, 1.0, 0.0, 1.0 }, new[] { 0.0, 0.0, 1.0, 1.0 }, new[] { 0.0, 0.0, 0.0, 0.0 });
    Matrix rref = m.ToRREF();
    AssertMatricesAreEqual(rref, m, "RREF of matrix already in RREF should be itself.");
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
#endregion

}
