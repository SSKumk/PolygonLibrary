using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;

namespace Tests.DoubleGeometry.Basics;

internal static class Line2DAssert {

  public static void AreEqual(Vector2D actual, Vector2D expected, string message = "") {
    Assert.Multiple(() => {
      Assert.That(Tools.EQ(actual.x, expected.x), Is.True, $"X differs. Expected {expected.x}, got {actual.x}. {message}");
      Assert.That(Tools.EQ(actual.y, expected.y), Is.True, $"Y differs. Expected {expected.y}, got {actual.y}. {message}");
    });
  }

  public static void HasInvariantGeometry(Line2D line, Vector2D pointOnLine) {
    Assert.Multiple(() => {
      Assert.That(Tools.EQ(line[pointOnLine]), Is.True, "Point on line should satisfy Ax + By + C = 0.");
      Assert.That(Tools.EQ(line.Direct.Length, 1.0), Is.True, "Direct should be normalized.");
      Assert.That(Tools.EQ(line.Normal.Length, 1.0), Is.True, "Normal should be normalized.");
      Assert.That(Tools.EQ(line.Direct * line.Normal), Is.True, "Direct and normal should be orthogonal.");
      Assert.That(Tools.EQ(line.A, line.Normal.x), Is.True, "A should match Normal.x.");
      Assert.That(Tools.EQ(line.B, line.Normal.y), Is.True, "B should match Normal.y.");
      Assert.That(Tools.EQ(line.C, -(line.Normal * pointOnLine)), Is.True, "C should be consistent with a point on the line.");
    });
  }

}
