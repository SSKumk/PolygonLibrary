using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;

namespace Tests.DoubleGeometry.Polygons;

[TestFixture]
public class ConvexPolygonMinkowskiTests {

  [Test]
  public void Sum_ProducesExpectedOctagonAndScaledRectangle() {
    ConvexPolygon cp1 = PolygonTools.RectangleTurned(0, -1, 0, 1, Tools.PI / 4);
    ConvexPolygon cp2 = PolygonTools.RectangleParallel(-1, -1, 1, 1);
    ConvexPolygon sum1 = cp1 + cp2;
    Vector2D[] expected1 =
      {
        new Vector2D(2, 1),
        new Vector2D(1, 2),
        new Vector2D(-1, 2),
        new Vector2D(-2, 1),
        new Vector2D(-2, -1),
        new Vector2D(-1, -2),
        new Vector2D(1, -2),
        new Vector2D(2, -1)
      };

    ConvexPolygon square = PolygonTools.RectangleParallel(-1, -1, 1, 1);
    ConvexPolygon sum2 = square + square;
    Vector2D[] expected2 =
      {
        new Vector2D(2, 2),
        new Vector2D(-2, 2),
        new Vector2D(-2, -2),
        new Vector2D(2, -2)
      };

    Assert.That(sum1.Contour.Count, Is.EqualTo(expected1.Length), "Sum 1: wrong number of vertices");
    foreach (Vector2D p in expected1) {
      Assert.That(sum1.Contour.Vertices.Contains(p), Is.True, "Sum 1: vertex " + p + " is not in the resultant polygon");
    }

    Assert.That(sum2.Contour.Count, Is.EqualTo(expected2.Length), "Sum 2: wrong number of vertices");
    foreach (Vector2D p in expected2) {
      Assert.That(sum2.Contour.Vertices.Contains(p), Is.True, "Sum 2: vertex " + p + " is not in the resultant polygon");
    }
  }

  [Test]
  public void Difference_ProducesExpectedLegacyResults() {
    ConvexPolygon? diff1 = PolygonTools.RectangleTurned(0, -3, 0, 3, Tools.PI / 4) - PolygonTools.RectangleParallel(-1, -1, 1, 1);
    ConvexPolygon? diff2 = PolygonTools.RectangleTurned(0, -3, 0, 3, Tools.PI / 4) - new ConvexPolygon(new[] { new Vector2D(0, 0), new Vector2D(-1, 0), new Vector2D(0, -1) }, false);
    ConvexPolygon? diff4 = PolygonTools.RectangleParallel(-1, 0, 1, 0) - PolygonTools.RectangleParallel(-3, 4, -1, 4);
    ConvexPolygon? diff5 = PolygonTools.RectangleTurned(-1, 0, 1, 0, Tools.PI / 4) - PolygonTools.RectangleTurned(-3, 4, -1, 4, Tools.PI / 4);

    Assert.Multiple(() => {
      Assert.That(diff1, Is.Not.Null);
      Assert.That(diff2, Is.Not.Null);
      Assert.That(diff4, Is.Not.Null);
      Assert.That(diff5, Is.Not.Null);
    });

    ConvexPolygonAssert.CyclicListComparison(diff1!.Contour.Vertices, new List<Vector2D> { new Vector2D(1, 0), new Vector2D(0, 1), new Vector2D(-1, 0), new Vector2D(0, -1) }, "Diff 1");
    ConvexPolygonAssert.CyclicListComparison(diff2!.Contour.Vertices, new List<Vector2D> { new Vector2D(-2, 0), new Vector2D(0, -2), new Vector2D(2.5, 0.5), new Vector2D(0.5, 2.5) }, "Diff 2");
    ConvexPolygonAssert.CyclicListComparison(diff4!.Contour.Vertices, new List<Vector2D> { new Vector2D(2, -4) }, "Diff 4");
    ConvexPolygonAssert.CyclicListComparison(diff5!.Contour.Vertices, new List<Vector2D> { new Vector2D(2, -4) }, "Diff 5");
  }

  [Test]
  public void Difference_ReturnsExpectedDegenerateOrEmptyResults() {
    ConvexPolygon? diff3 = PolygonTools.RectangleParallel(-1, -1, 1, 1) - PolygonTools.RectangleParallel(-1, 0, 1, 0);
    ConvexPolygon? diff6 = PolygonTools.RectangleTurned(-1, 0, 1, 0, Tools.PI / 4) - PolygonTools.RectangleTurned(-3.01, 4, -1, 4, Tools.PI / 4);
    ConvexPolygon? diff7 = PolygonTools.RectangleParallel(-1, 0, 1, 0) - PolygonTools.RectangleParallel(-3.01, 4, -1, 4);
    ConvexPolygon? diff8 = PolygonTools.RectangleParallel(-1, 0, 1, 0) - PolygonTools.RectangleParallel(0, -1, 0, 1);
    ConvexPolygon? diff9 = PolygonTools.RectangleParallel(-1, -1, 1, 1) - PolygonTools.Circle(0, 0, 1.01, 100);

    Assert.That(diff3, Is.Not.Null);
    ConvexPolygonAssert.CyclicListComparison(diff3!.Contour.Vertices, new List<Vector2D> { new Vector2D(0, 1), new Vector2D(0, -1) }, "Diff 3");

    Assert.That(diff6, Is.EqualTo(null), "Diff 6: the difference is not empty");
    Assert.That(diff7, Is.EqualTo(null), "Diff 7: the difference is not empty");
    Assert.That(diff8, Is.EqualTo(null), "Diff 8: the difference is not empty");
    Assert.That(diff9, Is.EqualTo(null), "Diff 9: the difference is not empty");
  }

}
