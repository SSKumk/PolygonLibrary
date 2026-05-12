using CGLibrary;
using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;

namespace Tests.DoubleGeometry.Polygons;

[TestFixture]
public class BasicPolygonLazyAggregationTests {

  [Test]
  public void Vertices_ComputesConcatenationOfContourVerticesAndCachesResult() {
    Polyline outer = new Polyline(PolylineTestData.CreateUnitSquareVertices(), PolylineOrientation.Counterclockwise, false, false);
    Polyline inner = new Polyline(
      new List<Vector2D> {
        new Vector2D(2.0, 0.0),
        new Vector2D(3.0, 0.0),
        new Vector2D(2.5, 1.0)
      },
      PolylineOrientation.Counterclockwise,
      false,
      false
    );
    BasicPolygon polygon = new BasicPolygonFromContoursProbe(new List<Polyline> { outer, inner });

    List<Vector2D> firstAccess = polygon.Vertices;
    List<Vector2D> secondAccess = polygon.Vertices;

    Assert.Multiple(() => {
      Assert.That(secondAccess, Is.SameAs(firstAccess));
      Assert.That(firstAccess, Is.EqualTo(outer.Vertices.Concat(inner.Vertices).ToList()));
    });
  }

  [Test]
  public void Edges_ComputesAllContourEdgesAndSortsThem() {
    Polyline outer = new Polyline(PolylineTestData.CreateUnitSquareVertices(), PolylineOrientation.Counterclockwise, false, false);
    Polyline inner = new Polyline(
      new List<Vector2D> {
        new Vector2D(2.0, 0.0),
        new Vector2D(3.0, 0.0),
        new Vector2D(2.5, 1.0)
      },
      PolylineOrientation.Counterclockwise,
      false,
      false
    );
    BasicPolygon polygon = new BasicPolygonFromContoursProbe(new List<Polyline> { outer, inner });

    List<Segment> firstAccess = polygon.Edges;
    List<Segment> secondAccess = polygon.Edges;
    List<Segment> expected = outer.Edges.Concat(inner.Edges).ToList();
    expected.Sort();

    Assert.Multiple(() => {
      Assert.That(secondAccess, Is.SameAs(firstAccess));
      Assert.That(firstAccess, Is.EqualTo(expected));
    });
  }

  [Test]
  public void Contours_ComputesSingleCounterclockwiseConvexHullFromVerticesAndCachesResult() {
    List<Vector2D> vertices =
      new() {
        new Vector2D(1.0, 0.0),
        new Vector2D(0.0, 1.0),
        new Vector2D(0.5, 0.5),
        new Vector2D(1.0, 1.0),
        new Vector2D(0.0, 0.0),
        new Vector2D(0.0, 0.0)
      };
    BasicPolygon polygon = new BasicPolygonFromVerticesProbe(vertices);

    List<Polyline> firstAccess = polygon.Contours;
    List<Polyline> secondAccess = polygon.Contours;

    Assert.Multiple(() => {
      Assert.That(secondAccess, Is.SameAs(firstAccess));
      Assert.That(firstAccess.Count, Is.EqualTo(1));
      Assert.That(firstAccess[0].Orientation, Is.EqualTo(PolylineOrientation.Counterclockwise));
      Assert.That(firstAccess[0].Vertices, Is.EqualTo(PolylineTestData.CreateUnitSquareVertices()));
      Assert.That(polygon.Vertices, Is.EqualTo(vertices));
    });
  }

}
