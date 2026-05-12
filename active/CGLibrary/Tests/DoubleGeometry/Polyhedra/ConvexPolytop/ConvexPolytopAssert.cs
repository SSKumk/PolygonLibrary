using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;

namespace Tests.DoubleGeometry.Polyhedra;

internal static class ConvexPolytopAssert {

  public static Vector V(params double[] coords) => new(coords);

  public static void AssertVertexSetEquals(IEnumerable<Vector> actual, IEnumerable<Vector> expected, string message = "") {
    SortedSet<Vector> actualSet = new(actual);
    SortedSet<Vector> expectedSet = new(expected);

    Assert.That(actualSet.SetEquals(expectedSet), Is.True, message);
  }

  public static void AssertVectorsAreEqual(Vector actual, Vector expected, string message = "") {
    Assert.That(actual.SpaceDim, Is.EqualTo(expected.SpaceDim), $"Vector dimensions differ. {message}");

    for (int i = 0; i < actual.SpaceDim; i++) {
      Assert.That(Tools.EQ(actual[i], expected[i]), Is.True, $"Vector component {i} differs. Expected: {expected[i]}, Got: {actual[i]}. {message}");
    }
  }

}
