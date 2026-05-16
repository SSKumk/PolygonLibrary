using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;

namespace Tests.DoubleGeometry.Basics;

public static class VectorAssert {

  public static Vector V(params double[] coords) => new(coords);

  public static void AreEqual(Vector actual, Vector expected, string message = "") {
    Assert.That(actual.SpaceDim, Is.EqualTo(expected.SpaceDim), $"Vector dimensions differ. {message}");
    for (int i = 0; i < actual.SpaceDim; i++) {
      Assert.That(Tools.EQ(actual[i], expected[i]), Is.True, $"Vector component {i} differs. Expected: {expected[i]}, Got: {actual[i]}. {message}");
    }
  }

  public static void ArraysAreEqual(double[] actual, double[] expected, string message = "") {
    Assert.That(actual.Length, Is.EqualTo(expected.Length), $"Array lengths differ. {message}");
    for (int i = 0; i < actual.Length; i++) {
      Assert.That(Tools.EQ(actual[i], expected[i]), Is.True, $"Array component {i} differs. Expected: {expected[i]}, Got: {actual[i]}. {message}");
    }
  }

}
