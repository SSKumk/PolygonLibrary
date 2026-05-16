using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;

namespace Tests.DoubleGeometry.Basics;

[TestFixture]
public class MatrixComparisonAndFormattingTests {

  [Test]
  public void Equals_ReturnsExpectedResultsForShapeValuesAndTolerance() {
    Matrix m1 = new Matrix(new double[,] { { 1, 2 }, { 3, 4 } });
    Matrix m2 = new Matrix(new double[,] { { 1, 2 }, { 3, 4 } });
    Matrix mRows = new Matrix(new double[,] { { 1, 2 } });
    Matrix mCols = new Matrix(new double[,] { { 1 }, { 3 } });
    Matrix mDiff = new Matrix(new double[,] { { 1, 2 }, { 3, 5 } });
    Matrix tol1 = new Matrix(new double[,] { { 1.0 / 3.0 } });
    Matrix tol2 = new Matrix(new double[,] { { 0.3333333333333331 } });
    Matrix tol3 = new Matrix(new double[,] { { 0.3333333333333337 } });
    Matrix tol4 = new Matrix(new double[,] { { 0.4 } });

    Assert.Multiple(() => {
      Assert.That(m1, Is.Not.EqualTo(null));
      Assert.That(m1.Equals(m1), Is.True);
      Assert.That(m1, Is.EqualTo(m2));
      Assert.That(m1, Is.Not.EqualTo(mRows));
      Assert.That(m1, Is.Not.EqualTo(mCols));
      Assert.That(m1, Is.Not.EqualTo(mDiff));
      Assert.That(tol1, Is.EqualTo(tol2), "m1 should equal m2 within tolerance.");
      Assert.That(tol1, Is.EqualTo(tol3), "m1 should equal m3 within tolerance.");
      Assert.That(tol1, Is.Not.EqualTo(tol4), "m1 should not equal m4.");
    });
  }

  [Test]
  public void GetHashCode_ThrowsInvalidOperationException() {
    Matrix m = new Matrix();
    Assert.Throws<InvalidOperationException>(() => m.GetHashCode());
  }

  [Test]
  public void CompareTo_UsesShapeThenLexicographicOrder() {
    Matrix m1 = new Matrix(new double[,] { { 1 } });
    Matrix m2 = new Matrix(new double[,] { { 1 }, { 2 } });
    Matrix m3 = new Matrix(new double[,] { { 1, 2 } });
    Matrix lexSmall = new Matrix(new double[,] { { 1, 2 }, { 3, 4 } });
    Matrix lexLarge = new Matrix(new double[,] { { 1, 2 }, { 3, 5 } });
    Matrix largerFirst = new Matrix(new double[,] { { 2, 0 }, { 0, 0 } });
    Matrix smallerFirst = new Matrix(new double[,] { { 1, 9 }, { 9, 9 } });

    Assert.Multiple(() => {
      Assert.That(m1.CompareTo(null), Is.EqualTo(1));
      Assert.That(lexSmall.CompareTo(new Matrix(new double[,] { { 1, 2 }, { 3, 4 } })), Is.EqualTo(0));
      Assert.That(m1.CompareTo(m2), Is.LessThan(0), "Matrix with fewer rows should be smaller.");
      Assert.That(m2.CompareTo(m1), Is.GreaterThan(0), "Matrix with more rows should be larger.");
      Assert.That(m1.CompareTo(m3), Is.LessThan(0), "Matrix with fewer columns should be smaller.");
      Assert.That(m3.CompareTo(m1), Is.GreaterThan(0), "Matrix with more columns should be larger.");
      Assert.That(lexSmall.CompareTo(lexLarge), Is.LessThan(0));
      Assert.That(largerFirst.CompareTo(smallerFirst), Is.GreaterThan(0));
    });
  }

  [Test]
  public void CompareTo_RespectsTransitivityAndSymmetry() {
    Matrix m1 = new Matrix(new double[,] { { 1, 0 } });
    Matrix m2 = new Matrix(new double[,] { { 2, 0 } });
    Matrix m3 = new Matrix(new double[,] { { 3, 0 } });

    Assert.Multiple(() => {
      Assert.That(m1.CompareTo(m2), Is.LessThan(0));
      Assert.That(m2.CompareTo(m3), Is.LessThan(0));
      Assert.That(m1.CompareTo(m3), Is.LessThan(0));
    });

    int comparison1 = m1.CompareTo(m2);
    int comparison2 = m2.CompareTo(m1);
    Assert.That(Math.Sign(comparison1), Is.EqualTo(-Math.Sign(comparison2)));
  }

  [Test]
  public void ToString_FormatsRowsAndAlignsColumns() {
    Matrix m = new Matrix(new double[,] { { 1, 100 }, { 1000, 10 } });
    string expected = "   1  100" + Environment.NewLine + "1000   10" + Environment.NewLine;
    Assert.That(m.ToString(), Is.EqualTo(expected));

    Matrix m2 = new Matrix(new double[,] { { 1.23, 2.3 }, { 33.444, 4.0 } });
    string str2 = m2.ToString();
    string[] lines = str2.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
    Assert.That(lines.Length, Is.EqualTo(2));
    Assert.That(lines[0].Contains("1.23"), Is.True);
    Assert.That(lines[0].Contains("2.3"), Is.True);
    Assert.That(lines[1].Contains("33.444"), Is.True);
    Assert.That(lines[1].Contains("4"), Is.True);
  }

}
