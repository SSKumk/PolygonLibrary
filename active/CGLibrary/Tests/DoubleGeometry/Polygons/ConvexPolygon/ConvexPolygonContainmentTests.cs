using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;

namespace Tests.DoubleGeometry.Polygons;

[TestFixture]
public class ConvexPolygonContainmentTests {

  [Test]
  public void Contains_AcceptsInsideAndBoundaryPointsAndRejectsOutsidePointsOnDenseCircleApproximation() {
    ConvexPolygon polygon = PolygonTools.Circle(10, 10, 2, 100);
    Vector2D
      inside1 = new Vector2D(11.5, 10.1),
      inside2 = new Vector2D(10, 10),
      inside3 = new Vector2D(8.9, 10),
      boundary1 = new Vector2D(polygon.Contour[0]),
      boundary2 = new Vector2D(polygon.Contour[1]),
      boundary3 = new Vector2D(polygon.Contour[^1]),
      boundary4 = new Vector2D(10, 8),
      boundary5 = new Vector2D(8, 10),
      boundary6 = (Vector2D)((Vector2D)polygon.Contour[0] + (Vector2D)polygon.Contour[1]) / 2,
      boundary7 = (Vector2D)((Vector2D)polygon.Contour[0] + (Vector2D)polygon.Contour[^1]) / 2,
      boundary8 = (Vector2D)((Vector2D)polygon.Contour[55] + (Vector2D)polygon.Contour[56]) / 2,
      outside1 = new Vector2D(12.1, 10),
      outside2 = polygon.Contour[0] + 1.1 * (polygon.Contour[1] - polygon.Contour[0]),
      outside3 = polygon.Contour[0] + 1.1 * (polygon.Contour[^1] - polygon.Contour[0]),
      outside4 = new Vector2D(7.9, 10),
      outside5 = new Vector2D(6, 11);

    Assert.Multiple(() => {
      Assert.That(polygon.Contains(inside1), Is.True, "inside1");
      Assert.That(polygon.Contains(inside2), Is.True, "inside2");
      Assert.That(polygon.Contains(inside3), Is.True, "inside3");
      Assert.That(polygon.Contains(boundary1), Is.True, "boundary1");
      Assert.That(polygon.Contains(boundary2), Is.True, "boundary2");
      Assert.That(polygon.Contains(boundary3), Is.True, "boundary3");
      Assert.That(polygon.Contains(boundary4), Is.True, "boundary4");
      Assert.That(polygon.Contains(boundary5), Is.True, "boundary5");
      Assert.That(polygon.Contains(boundary6), Is.True, "boundary6");
      Assert.That(polygon.Contains(boundary7), Is.True, "boundary7");
      Assert.That(polygon.Contains(boundary8), Is.True, "boundary8");
      Assert.That(polygon.Contains(outside1), Is.False, "outside1");
      Assert.That(polygon.Contains(outside2), Is.False, "outside2");
      Assert.That(polygon.Contains(outside3), Is.False, "outside3");
      Assert.That(polygon.Contains(outside4), Is.False, "outside4");
      Assert.That(polygon.Contains(outside5), Is.False, "outside5");
    });
  }

  [Test]
  public void Contains_RespectsLegacyOctagonCases() {
    ConvexPolygon polygon = new ConvexPolygon(ConvexPolygonTestData.CreateOctagonVertices(), false);
    List<Vector2D> testPoints =
      new() {
        new Vector2D(-2, -1),
        new Vector2D(-1, -3), new Vector2D(-2, -3), new Vector2D(-3, -3), new Vector2D(-3, 0), new Vector2D(-3, 1),
        new Vector2D(-1.5, -1.5), new Vector2D(-1, -2), new Vector2D(0, -3),
        new Vector2D(-2, 0), new Vector2D(-2, 1), new Vector2D(-2, 3),
        new Vector2D(-1, -1.5), new Vector2D(0, -2), new Vector2D(2, -3),
        new Vector2D(0, -0.5), new Vector2D(2, 0), new Vector2D(6, 1),
        new Vector2D(0, 1), new Vector2D(1, 2), new Vector2D(3, 4),
        new Vector2D(-11.0 / 6, 0), new Vector2D(-1.5, -1.5), new Vector2D(-1, 4)
      };
    bool[] expected =
      {
        true,
        false, false, false, false, false,
        true, true, false,
        true, true, false,
        true, true, false,
        true, true, false,
        true, true, false,
        true, true, false
      };

    for (int i = 0; i < testPoints.Count; i++) {
      Assert.That(expected[i], Is.EqualTo(polygon.Contains(testPoints[i])), "ContainsTest2, test #" + i + " failed, point = " + testPoints[i]);
    }
  }

  [Test]
  public void ContainsInside_AcceptsOnlyStrictlyInteriorPointsOnDenseCircleApproximation() {
    ConvexPolygon polygon = PolygonTools.Circle(10, 10, 2, 100);
    Vector2D
      inside1 = new Vector2D(11.5, 10.1),
      inside2 = new Vector2D(10, 10),
      inside3 = new Vector2D(8.9, 10),
      boundary1 = new Vector2D(polygon.Contour[0]),
      boundary2 = new Vector2D(polygon.Contour[1]),
      boundary3 = new Vector2D(polygon.Contour[^1]),
      boundary4 = new Vector2D(10, 8),
      boundary5 = new Vector2D(8, 10),
      boundary6 = (Vector2D)((Vector2D)polygon.Contour[0] + (Vector2D)polygon.Contour[1]) / 2,
      boundary7 = (Vector2D)((Vector2D)polygon.Contour[0] + (Vector2D)polygon.Contour[^1]) / 2,
      boundary8 = (Vector2D)((Vector2D)polygon.Contour[55] + (Vector2D)polygon.Contour[56]) / 2,
      outside1 = new Vector2D(12.1, 10),
      outside2 = polygon.Contour[0] + 1.1 * (polygon.Contour[1] - polygon.Contour[0]),
      outside3 = polygon.Contour[0] + 1.1 * (polygon.Contour[^1] - polygon.Contour[0]),
      outside4 = new Vector2D(7.9, 10),
      outside5 = new Vector2D(6, 11);

    Assert.Multiple(() => {
      Assert.That(polygon.ContainsInside(inside1), Is.True, "inside1");
      Assert.That(polygon.ContainsInside(inside2), Is.True, "inside2");
      Assert.That(polygon.ContainsInside(inside3), Is.True, "inside3");
      Assert.That(polygon.ContainsInside(boundary1), Is.False, "boundary1");
      Assert.That(polygon.ContainsInside(boundary2), Is.False, "boundary2");
      Assert.That(polygon.ContainsInside(boundary3), Is.False, "boundary3");
      Assert.That(polygon.ContainsInside(boundary4), Is.False, "boundary4");
      Assert.That(polygon.ContainsInside(boundary5), Is.False, "boundary5");
      Assert.That(polygon.ContainsInside(boundary6), Is.False, "boundary6");
      Assert.That(polygon.ContainsInside(boundary7), Is.False, "boundary7");
      Assert.That(polygon.ContainsInside(boundary8), Is.False, "boundary8");
      Assert.That(polygon.ContainsInside(outside1), Is.False, "outside1");
      Assert.That(polygon.ContainsInside(outside2), Is.False, "outside2");
      Assert.That(polygon.ContainsInside(outside3), Is.False, "outside3");
      Assert.That(polygon.ContainsInside(outside4), Is.False, "outside4");
      Assert.That(polygon.ContainsInside(outside5), Is.False, "outside5");
    });
  }

  [Test]
  public void ContainsInside_RespectsLegacyOctagonCases() {
    ConvexPolygon polygon = new ConvexPolygon(ConvexPolygonTestData.CreateOctagonVertices());
    List<Vector2D> testPoints =
      new() {
        new Vector2D(-2, -1),
        new Vector2D(-1, -3), new Vector2D(-2, -3), new Vector2D(-3, -3), new Vector2D(-3, 0), new Vector2D(-3, 1),
        new Vector2D(-1.5, -1.5), new Vector2D(-1, -2), new Vector2D(0, -3),
        new Vector2D(-2, 0), new Vector2D(-2, 1), new Vector2D(-2, 3),
        new Vector2D(-1, -1.5), new Vector2D(0, -2), new Vector2D(2, -3),
        new Vector2D(0, -0.5), new Vector2D(2, 0), new Vector2D(6, 1),
        new Vector2D(0, 1), new Vector2D(1, 2), new Vector2D(3, 4),
        new Vector2D(-11.0 / 6, 0), new Vector2D(-1.5, -1.5), new Vector2D(-1, 4)
      };
    bool[] expected =
      {
        false,
        false, false, false, false, false,
        false, false, false,
        false, false, false,
        true, false, false,
        true, false, false,
        true, false, false,
        true, false, false
      };

    for (int i = 0; i < testPoints.Count; i++) {
      Assert.That(expected[i], Is.EqualTo(polygon.ContainsInside(testPoints[i])), "ContainsTest2, test #" + i + " failed, point = " + testPoints[i]);
    }
  }

}
