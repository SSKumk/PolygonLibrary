using NUnit.Framework;

namespace Tests.SharedTests;

using static CGLibrary.Geometry<double, DConvertor>; // Your using for easy access

public static class StaticHelpers {


  // Helper for creating vectors easily
  public static Vector V(params double[] coords) => new Vector(coords);

  // Helper for comparing vectors with tolerance
  public static void AssertVectorsAreEqual(Vector v1, Vector v2, string message = "") {
    Assert.That(v1.SpaceDim, Is.EqualTo(v2.SpaceDim), $"Vector dimensions differ. {message}");
    for (int i = 0; i < v1.SpaceDim; i++) {
      Assert.That(Tools.EQ(v1[i], v2[i]), Is.True, $"Vector component {i} differs. Expected: {v2[i]}, Got: {v1[i]}. {message}");
    }
  }
}
