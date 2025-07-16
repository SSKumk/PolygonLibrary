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

  //Helper for comparing result arrays with tolerance
  public static void AssertArraysAreEqual(double[]? a1, double[]? a2, string message = "") {
    if (a1 is null && a2 is null) {
      return; // Оба null, считаем равными
    }
    Assert.That(a1, Is.Not.Null, $"Array 1 is not null, but Array 2 is. {message}");
    Assert.That(a2, Is.Not.Null, $"Array 2 is not null, but Array 1 is. {message}");

    Assert.That(a1.Length, Is.EqualTo(a2.Length), $"Array lengths differ. {message}");
    for (int i = 0; i < a1.Length; i++) {
      Assert.That(Tools.EQ(a1[i], a2[i]), Is.True, $"Array component {i} differs. Expected: {a2[i]}, Got: {a1[i]}. {message}");
    }
  }
}
