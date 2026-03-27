using CGLibrary;
using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;

namespace Tests.DoubleGeometry.Polygons;

[TestFixture]
public class BasicPolygonConstructionTests {

  [Test]
  public void Constructor_List_CreatesSingleCounterclockwiseContourAndCopiesVertices() {
    List<Vector2D> vertices = PolylineTestData.CreateUnitSquareVertices();
    BasicPolygon polygon = new BasicPolygonCtorProbe(vertices, false, false);

    Assert.Multiple(() => {
      Assert.That(polygon.Contours.Count, Is.EqualTo(1));
      Assert.That(polygon.Contours[0].Orientation, Is.EqualTo(PolylineOrientation.Counterclockwise));
      Assert.That(polygon.Contours[0].Vertices, Is.SameAs(vertices));
      Assert.That(polygon.Vertices, Is.Not.SameAs(vertices));
      Assert.That(polygon.Vertices, Is.EqualTo(vertices));
    });
  }

  [Test]
  public void Constructor_Array_CopiesVerticesForContourAndVertexStorage() {
    Vector2D[] vertices = PolylineTestData.CreateUnitSquareVertices().ToArray();
    BasicPolygon polygon = new BasicPolygonCtorProbe(vertices, false, false);

    vertices[0] = new Vector2D(-10.0, -10.0);

    Assert.Multiple(() => {
      Assert.That(polygon.Contours.Count, Is.EqualTo(1));
      Assert.That(polygon.Contours[0].Orientation, Is.EqualTo(PolylineOrientation.Counterclockwise));
      Assert.That(polygon.Vertices, Is.Not.Null);
      Assert.That(polygon.Vertices[0], Is.EqualTo(Vector2D.Zero));
      Assert.That(polygon.Contours[0][0], Is.EqualTo(Vector2D.Zero));
    });
  }

}
