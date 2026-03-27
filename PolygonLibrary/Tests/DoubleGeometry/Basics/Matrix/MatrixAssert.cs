using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;
using static Tests.DoubleGeometry.Basics.VectorAssert;

namespace Tests.DoubleGeometry.Basics;

public static class MatrixAssert {

  public static Matrix M(params double[][] rows) {
    int numRows = rows.Length;
    if (numRows == 0) {
      throw new ArgumentException("Cannot create a matrix with zero rows using this helper.");
    }

    int numCols = rows[0].Length;
    if (numCols == 0) {
      throw new ArgumentException("Cannot create a matrix with zero columns using this helper.");
    }

    double[] data1d = new double[numRows * numCols];
    int k = 0;
    for (int i = 0; i < numRows; i++) {
      Assert.That(rows[i].Length, Is.EqualTo(numCols), $"Row {i} has wrong number of columns.");
      for (int j = 0; j < numCols; j++) {
        data1d[k++] = rows[i][j];
      }
    }

    return new Matrix(numRows, numCols, data1d, needCopy: false);
  }

  public static void AreEqual(Matrix actual, Matrix expected, string message = "") {
    Assert.That(actual.Rows, Is.EqualTo(expected.Rows), $"Matrix row counts differ. {message}");
    Assert.That(actual.Cols, Is.EqualTo(expected.Cols), $"Matrix column counts differ. {message}");
    for (int i = 0; i < actual.Rows; i++) {
      for (int j = 0; j < actual.Cols; j++) {
        Assert.That(
          Tools.EQ(actual[i, j], expected[i, j]),
          Is.True,
          $"Matrices differ at [{i},{j}]. Expected: {expected[i, j]:F15}, Got: {actual[i, j]:F15}. {message}"
        );
      }
    }
  }

  public static void IsRREF(Matrix m, string message = "") {
    int leadCol = -1;
    for (int i = 0; i < m.Rows; i++) {
      int currentRowLeadCol = -1;
      for (int j = 0; j < m.Cols; j++) {
        if (!Tools.EQ(m[i, j])) {
          currentRowLeadCol = j;
          break;
        }
      }

      if (currentRowLeadCol != -1) {
        Assert.That(
          currentRowLeadCol,
          Is.GreaterThan(leadCol),
          $"Leading entry in row {i} (col {currentRowLeadCol}) is not to the right of previous leading entry (col {leadCol}). {message}"
        );
        Assert.That(
          Tools.EQ(m[i, currentRowLeadCol], 1.0),
          Is.True,
          $"Leading entry at [{i},{currentRowLeadCol}] is {m[i, currentRowLeadCol]}, not 1. {message}"
        );
        for (int rowIdx = 0; rowIdx < m.Rows; rowIdx++) {
          if (rowIdx != i) {
            Assert.That(
              Tools.EQ(m[rowIdx, currentRowLeadCol], 0.0),
              Is.True,
              $"Entry at [{rowIdx},{currentRowLeadCol}] should be 0 (column of leading entry [{i},{currentRowLeadCol}]). Found {m[rowIdx, currentRowLeadCol]}. {message}"
            );
          }
        }
        leadCol = currentRowLeadCol;
      } else {
        for (int rowIdx = i + 1; rowIdx < m.Rows; rowIdx++) {
          for (int colIdx = 0; colIdx < m.Cols; colIdx++) {
            Assert.That(
              Tools.EQ(m[rowIdx, colIdx], 0.0),
              Is.True,
              $"Row {rowIdx} should be zero because row {i} is zero. Found non-zero at [{rowIdx},{colIdx}]. {message}"
            );
          }
        }
        break;
      }
    }
  }

}
