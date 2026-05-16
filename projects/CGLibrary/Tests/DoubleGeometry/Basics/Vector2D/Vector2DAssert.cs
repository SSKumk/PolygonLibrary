using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;

namespace Tests.DoubleGeometry.Basics;

internal static class Vector2DAssert {

  public static void AreEqual(Vector2D actual, Vector2D expected, string message = "") {
    Assert.Multiple(() => {
      Assert.That(Tools.EQ(actual.x, expected.x), Is.True, $"X differs. Expected {expected.x}, got {actual.x}. {message}");
      Assert.That(Tools.EQ(actual.y, expected.y), Is.True, $"Y differs. Expected {expected.y}, got {actual.y}. {message}");
    });
  }

}
