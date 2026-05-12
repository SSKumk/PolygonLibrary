using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;

namespace Tests.DoubleGeometry.Basics;

internal static class CauchyMatrixAssert {

  public static void AreEqual(Matrix actual, Matrix expected, double tolerance, string message = "") {
    Assert.That(actual.Rows, Is.EqualTo(expected.Rows), $"Row count differs. {message}");
    Assert.That(actual.Cols, Is.EqualTo(expected.Cols), $"Column count differs. {message}");

    for (int i = 0; i < actual.Rows; i++) {
      for (int j = 0; j < actual.Cols; j++) {
        Assert.That(actual[i, j], Is.EqualTo(expected[i, j]).Within(tolerance), $"Matrix entry [{i},{j}] differs. {message}");
      }
    }
  }

}
