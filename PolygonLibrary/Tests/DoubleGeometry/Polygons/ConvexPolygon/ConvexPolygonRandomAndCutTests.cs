using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;

namespace Tests.DoubleGeometry.Polygons;

[TestFixture]
public class ConvexPolygonRandomAndCutTests {

  [Test]
  public void GenerateDataForRandomPoint_ProducesValidTriangleIndexAndBarycentricWeights() {
    ConvexPolygon polygon = new ConvexPolygon(ConvexPolygonTestData.SquareVerticesClockwiseStartingTopRight, true);
    GRandomLC random = new GRandomLC(123u);

    polygon.GenerateDataForRandomPoint(out int triangleIndex, out double a, out double b, out double c, random);

    Assert.Multiple(() => {
      Assert.That(triangleIndex, Is.InRange(0, polygon.Contour.Count - 2));
      Assert.That(Tools.EQ(a + b + c, 1.0), Is.True);
      Assert.That(Tools.GE(a), Is.True);
      Assert.That(Tools.GE(b), Is.True);
      Assert.That(Tools.GE(c), Is.True);
    });
  }

  [Test]
  public void GenerateRandomPoint_ReturnsPointsInsidePolygon() {
    ConvexPolygon square = new ConvexPolygon(ConvexPolygonTestData.SquareVerticesClockwiseStartingTopRight, true);
    ConvexPolygon octagon = new ConvexPolygon(ConvexPolygonTestData.CreateOctagonVertices(), true);
    GRandomLC random1 = new GRandomLC(1u);
    GRandomLC random2 = new GRandomLC(2u);

    for (int i = 0; i < 10; i++) {
      Vector2D point = square.GenerateRandomPoint(random1);
      Assert.That(square.Contains(point), Is.True, "RandomPoint1: a point is obtained that is outside the polygon");
    }

    for (int i = 0; i < 200; i++) {
      Vector2D point = octagon.GenerateRandomPoint(random2);
      Assert.That(octagon.Contains(point), Is.True, "RandomPoint2: a point is obtained that is outside the polygon");
    }
  }

  [Test]
  public void CutConvexPolygon_ThrowsForAdjacentVerticesAndReturnsTwoPolygonsOtherwise() {
    ConvexPolygon polygon = PolygonTools.Circle(0, 0, 2, 4);

    Assert.That(() => polygon.CutConvexPolygon(0, 1), Throws.TypeOf<ArgumentException>().With.Message.EqualTo("0 and 1 is adjacent!"));
    Assert.That(() => polygon.CutConvexPolygon(0, 3), Throws.TypeOf<ArgumentException>().With.Message.EqualTo("0 and 3 is adjacent!"));

    (ConvexPolygon first, ConvexPolygon second) = polygon.CutConvexPolygon(0, 2);

    Assert.Multiple(() => {
      Assert.That(first.Contour.Count, Is.EqualTo(3));
      Assert.That(second.Contour.Count, Is.EqualTo(3));
      ConvexPolygonAssert.CyclicListComparison(
        first.Contour.Vertices,
        new List<Vector2D> { polygon.Vertices[0], polygon.Vertices[1], polygon.Vertices[2] },
        "CutConvexPolygon first"
      );
      ConvexPolygonAssert.CyclicListComparison(
        second.Contour.Vertices,
        new List<Vector2D> { polygon.Vertices[2], polygon.Vertices[3], polygon.Vertices[0] },
        "CutConvexPolygon second"
      );
    });
  }

}
