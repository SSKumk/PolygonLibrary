using DoubleDouble;
using NUnit.Framework;
using DDecomposition = CGLibrary.Geometry<double, Tests.DConvertor>.Decomposition;
using DMatrix = CGLibrary.Geometry<double, Tests.DConvertor>.Matrix;
using DTools = CGLibrary.Geometry<double, Tests.DConvertor>.Tools;
using DDDecomposition = CGLibrary.Geometry<DoubleDouble.ddouble, Tests.DDConvertor>.Decomposition;
using DDMatrix = CGLibrary.Geometry<DoubleDouble.ddouble, Tests.DDConvertor>.Matrix;
using DDTools = CGLibrary.Geometry<DoubleDouble.ddouble, Tests.DDConvertor>.Tools;

namespace Tests.DoubleGeometry.Research.DecompositionResearch;

[TestFixture]
public class QRStabilityResearchTests {

  private double _oldDoubleEps;
  private ddouble _oldDdEps;

  [SetUp]
  public void SaveAndSetEps() {
    _oldDoubleEps = DTools.Eps;
    _oldDdEps = DDTools.Eps;

    DTools.Eps = 1e-8;
    DDTools.Eps = (ddouble)1e-30;
  }

  [TearDown]
  public void RestoreEps() {
    DTools.Eps = _oldDoubleEps;
    DDTools.Eps = _oldDdEps;
  }

  [Test]
  public void QR_ByHouseholder_Scale1eMinus7_RemainsStableAsFactorizationAndTriangularForm() {
    DMatrix a = BuildScaledDouble(1e-7);
    (DMatrix q, DMatrix r) = DDecomposition.QR_ByHouseholder(a);

    double reconstructionError = ComputeMaxAbsDiff(q * r, a);
    double orthogonalityError = ComputeMaxAbsDiff(q.Transpose() * q, DMatrix.Eye(q.Cols));
    double relativeLeakage = ComputeBelowDiagonalLeakage(r) / 1e-7;

    Assert.Multiple(() => {
      Assert.That(reconstructionError, Is.LessThan(1e-18));
      Assert.That(orthogonalityError, Is.LessThan(1e-12));
      Assert.That(relativeLeakage, Is.LessThan(1e-8));
    });
  }

  [Test]
  public void QR_ByHouseholder_Scale1eMinus8_ReconstructionStaysSmallButTriangularFormBecomesUnreliable() {
    DMatrix a = BuildScaledDouble(1e-8);
    (DMatrix q, DMatrix r) = DDecomposition.QR_ByHouseholder(a);

    double reconstructionError = ComputeMaxAbsDiff(q * r, a);
    double relativeLeakage = ComputeBelowDiagonalLeakage(r) / 1e-8;

    Assert.Multiple(() => {
      Assert.That(reconstructionError, Is.LessThan(1e-18));
      Assert.That(relativeLeakage, Is.GreaterThan(1e-1));
    });
  }

  [Test]
  public void QR_ByHouseholder_Scale1eMinus9_DegeneratesToIdentityAndOriginalMatrix() {
    DMatrix a = BuildScaledDouble(1e-9);
    (DMatrix q, DMatrix r) = DDecomposition.QR_ByHouseholder(a);

    Assert.Multiple(() => {
      Assert.That(ComputeMaxAbsDiff(q, DMatrix.Eye(3)), Is.LessThan(1e-15));
      Assert.That(ComputeMaxAbsDiff(r, a), Is.LessThan(1e-15));
      Assert.That(ComputeBelowDiagonalLeakage(r) / 1e-9, Is.GreaterThan(1.0));
    });
  }

  [Test]
  public void QR_ByHouseholder_AtToolsEpsPivot_DoubleDropsNumericRankWhilePivotMagnitudeIsStillClose() {
    const double scale = 1.0;
    const double delta = 1e-8;

    DMatrix a = BuildNearRankDeficientDouble(scale, delta);
    (DMatrix q, DMatrix r) = DDecomposition.QR_ByHouseholder(a);
    double expectedPivot = scale * delta;
    double pivotRatio = Math.Abs(r[1, 1]) / expectedPivot;

    Assert.Multiple(() => {
      Assert.That(CountNonZeroDiagonal(r), Is.EqualTo(1));
      Assert.That(pivotRatio, Is.GreaterThan(0.9));
      Assert.That(ComputeMaxAbsDiff(q * r, a), Is.LessThan(1e-12));
    });
  }

  [Test]
  public void QR_ByHouseholder_HighPrecisionReference_KeepsSecondPivotAtTheSameThresholdCase() {
    ddouble scale = 1.0;
    ddouble delta = 1e-8;

    DDMatrix a = BuildNearRankDeficientDD(scale, delta);
    (DDMatrix q, DDMatrix r) = DDDecomposition.QR_ByHouseholder(a);
    ddouble expectedPivot = scale * delta;
    ddouble pivotRatio = ddouble.Abs(r[1, 1]) / expectedPivot;

    Assert.Multiple(() => {
      Assert.That(CountNonZeroDiagonal(r), Is.EqualTo(2));
      Assert.That((double)pivotRatio, Is.GreaterThan(0.999999));
      Assert.That(ComputeMaxAbsDiff(q * r, a), Is.LessThan(1e-24));
    });
  }

  [Test]
  public void QR_ByHouseholder_ScaleShiftedThresholdCase_ShowsThatNumericRankDependsOnAbsoluteSecondPivot() {
    DMatrix a = BuildNearRankDeficientDouble(scale: 1e4, delta: 1e-12);
    (DMatrix _, DMatrix r) = DDecomposition.QR_ByHouseholder(a);

    Assert.That(CountNonZeroDiagonal(r), Is.EqualTo(1));
  }

  private static DMatrix BuildScaledDouble(double scale) => scale * BuildBaseDouble();

  private static DMatrix BuildBaseDouble() {
    DMatrix q0 = MakeRotation3D_Double(0, 1, Math.PI / 4.0) * MakeRotation3D_Double(1, 2, Math.PI / 6.0);
    DMatrix r0 = new(new double[,] {
      { 3.0, -2.0 },
      { 0.0, 1.0 },
      { 0.0, 0.0 },
    });

    return q0 * r0;
  }

  private static DMatrix BuildNearRankDeficientDouble(double scale, double delta) {
    DMatrix q0 = MakeRotation3D_Double(0, 2, Math.PI / 3.0) * MakeRotation3D_Double(0, 1, Math.PI / 7.0);
    DMatrix r = new(new double[,] {
      { scale, scale },
      { 0.0, scale * delta },
      { 0.0, 0.0 },
    });

    return q0 * r;
  }

  private static DDMatrix BuildNearRankDeficientDD(ddouble scale, ddouble delta) {
    DDMatrix q0 = MakeRotation3D_DD(0, 2, ddouble.PI / 3.0) * MakeRotation3D_DD(0, 1, ddouble.PI / 7.0);
    DDMatrix r = new(new ddouble[,] {
      { scale, scale },
      { 0.0, scale * delta },
      { 0.0, 0.0 },
    });

    return q0 * r;
  }

  private static DMatrix MakeRotation3D_Double(int axis1, int axis2, double angle) {
    double c = Math.Cos(angle);
    double s = Math.Sin(angle);
    double[,] data = {
      { 1.0, 0.0, 0.0 },
      { 0.0, 1.0, 0.0 },
      { 0.0, 0.0, 1.0 },
    };

    data[axis1, axis1] = c;
    data[axis1, axis2] = -s;
    data[axis2, axis1] = s;
    data[axis2, axis2] = c;

    return new DMatrix(data);
  }

  private static DDMatrix MakeRotation3D_DD(int axis1, int axis2, ddouble angle) {
    ddouble c = ddouble.Cos(angle);
    ddouble s = ddouble.Sin(angle);
    ddouble[,] data = {
      { 1.0, 0.0, 0.0 },
      { 0.0, 1.0, 0.0 },
      { 0.0, 0.0, 1.0 },
    };

    data[axis1, axis1] = c;
    data[axis1, axis2] = -s;
    data[axis2, axis1] = s;
    data[axis2, axis2] = c;

    return new DDMatrix(data);
  }

  private static double ComputeBelowDiagonalLeakage(DMatrix matrix) {
    double maxLeak = 0.0;
    for (int row = 0; row < matrix.Rows; row++) {
      for (int col = 0; col < Math.Min(row, matrix.Cols); col++) {
        maxLeak = Math.Max(maxLeak, Math.Abs(matrix[row, col]));
      }
    }

    return maxLeak;
  }

  private static int CountNonZeroDiagonal(DMatrix matrix) {
    int count = 0;
    for (int i = 0; i < Math.Min(matrix.Rows, matrix.Cols); i++) {
      if (DTools.NE(matrix[i, i])) {
        count++;
      }
    }

    return count;
  }

  private static int CountNonZeroDiagonal(DDMatrix matrix) {
    int count = 0;
    for (int i = 0; i < Math.Min(matrix.Rows, matrix.Cols); i++) {
      if (DDTools.NE(matrix[i, i])) {
        count++;
      }
    }

    return count;
  }

  private static double ComputeMaxAbsDiff(DMatrix actual, DMatrix expected) {
    double maxDiff = 0.0;
    for (int row = 0; row < actual.Rows; row++) {
      for (int col = 0; col < actual.Cols; col++) {
        maxDiff = Math.Max(maxDiff, Math.Abs(actual[row, col] - expected[row, col]));
      }
    }

    return maxDiff;
  }

  private static double ComputeMaxAbsDiff(DDMatrix actual, DDMatrix expected) {
    ddouble maxDiff = 0.0;
    for (int row = 0; row < actual.Rows; row++) {
      for (int col = 0; col < actual.Cols; col++) {
        maxDiff = ddouble.Max(maxDiff, ddouble.Abs(actual[row, col] - expected[row, col]));
      }
    }

    return (double)maxDiff;
  }

}
