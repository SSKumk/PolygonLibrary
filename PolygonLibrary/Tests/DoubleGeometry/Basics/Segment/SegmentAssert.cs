using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;

namespace Tests.DoubleGeometry.Basics;

internal static class SegmentAssert {

  public static void AreEqual(Vector2D actual, Vector2D expected, string message = "") {
    Assert.Multiple(() => {
      Assert.That(Tools.EQ(actual.x, expected.x), Is.True, $"X differs. Expected {expected.x}, got {actual.x}. {message}");
      Assert.That(Tools.EQ(actual.y, expected.y), Is.True, $"Y differs. Expected {expected.y}, got {actual.y}. {message}");
    });
  }

  public static void HasPointSet(CrossInfo info, Vector2D p1, Vector2D p2, string message = "") {
    Assert.That(info.fp, Is.Not.Null, $"First point is null. {message}");
    Assert.That(info.sp, Is.Not.Null, $"Second point is null. {message}");

    bool directOrder = info.fp! == p1 && info.sp! == p2;
    bool reverseOrder = info.fp! == p2 && info.sp! == p1;

    Assert.That(directOrder || reverseOrder, Is.True, $"Unexpected overlap endpoints. {message}");
  }

}
