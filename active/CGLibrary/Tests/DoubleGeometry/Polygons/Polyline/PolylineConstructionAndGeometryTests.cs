using CGLibrary;
using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;

namespace Tests.DoubleGeometry.Polygons;

[TestFixture]
public class PolylineConstructionAndGeometryTests {

  private sealed class PolylineProbe : Polyline {

    public PolylineProbe(List<Vector2D> vertices, PolylineOrientation orientation) : base(vertices, orientation, false, false) { }

    public double GetEdgeAngle(int index) => EdgeAngle(index);

  }

  [Test]
  public void Constructor_Default_CreatesOnePointPolylineAtOrigin() {
    Polyline polyline = new Polyline();

    Assert.Multiple(() => {
      Assert.That(polyline.Count, Is.EqualTo(1));
      Assert.That(polyline.IsEmpty, Is.False);
      Assert.That(polyline.Orientation, Is.EqualTo(PolylineOrientation.Counterclockwise));
      Assert.That(polyline[0], Is.EqualTo(Vector2D.Zero));
      Assert.That(polyline.Vertices.Count, Is.EqualTo(1));
      Assert.That(polyline.Vertices[0], Is.EqualTo(Vector2D.Zero));
      Assert.That(Tools.EQ(polyline.Square), Is.True);
      Assert.That(polyline.Edges, Is.Empty);
    });
  }

  [Test]
  public void Constructor_List_PreservesSuppliedVerticesAndDeclaredOrientation() {
    List<Vector2D> vertices = PolylineTestData.CreateUnitSquareVertices();
    Polyline polyline = new Polyline(vertices, PolylineOrientation.Clockwise, false, false);

    Assert.Multiple(() => {
      Assert.That(polyline.Count, Is.EqualTo(vertices.Count));
      Assert.That(polyline.Vertices, Is.SameAs(vertices));
      Assert.That(polyline.Orientation, Is.EqualTo(PolylineOrientation.Clockwise));
      Assert.That(polyline[0], Is.EqualTo(vertices[0]));
      Assert.That(polyline[3], Is.EqualTo(vertices[3]));
    });
  }

  [Test]
  public void Constructor_Array_CopiesVerticesIntoInternalList() {
    Vector2D[] vertices = PolylineTestData.CreateUnitSquareVertices().ToArray();
    Polyline polyline = new Polyline(vertices, PolylineOrientation.Clockwise, false, false);

    vertices[0] = new Vector2D(-10.0, -10.0);

    Assert.Multiple(() => {
      Assert.That(polyline.Count, Is.EqualTo(4));
      Assert.That(polyline.Orientation, Is.EqualTo(PolylineOrientation.Clockwise));
      Assert.That(polyline[0], Is.EqualTo(Vector2D.Zero));
      Assert.That(polyline.Vertices[0], Is.EqualTo(Vector2D.Zero));
    });
  }

  [Test]
  public void Constructor_EmptyVertexList_CreatesEmptyPolylineWithZeroSquareAndNoEdges() {
    Polyline polyline = new Polyline(new List<Vector2D>(), PolylineOrientation.Counterclockwise, false, false);

    Assert.Multiple(() => {
      Assert.That(polyline.Count, Is.Zero);
      Assert.That(polyline.IsEmpty, Is.True);
      Assert.That(polyline.Edges, Is.Empty);
      Assert.That(Tools.EQ(polyline.Square), Is.True);
      Assert.That(polyline.Orientation, Is.EqualTo(PolylineOrientation.Counterclockwise));
    });
  }

  [Test]
  public void Edges_BuildClosedChainAndCacheResult() {
    Polyline polyline = new Polyline(PolylineTestData.CreateUnitSquareVertices(), PolylineOrientation.Counterclockwise, false, false);

    List<Segment> firstAccess = polyline.Edges;
    List<Segment> secondAccess = polyline.Edges;

    Assert.Multiple(() => {
      Assert.That(secondAccess, Is.SameAs(firstAccess));
      Assert.That(firstAccess.Count, Is.EqualTo(4));
      Assert.That(firstAccess[0][0], Is.EqualTo(new Vector2D(0.0, 0.0)));
      Assert.That(firstAccess[0][1], Is.EqualTo(new Vector2D(1.0, 0.0)));
      Assert.That(firstAccess[1][0], Is.EqualTo(new Vector2D(1.0, 0.0)));
      Assert.That(firstAccess[1][1], Is.EqualTo(new Vector2D(1.0, 1.0)));
      Assert.That(firstAccess[3][0], Is.EqualTo(new Vector2D(0.0, 1.0)));
      Assert.That(firstAccess[3][1], Is.EqualTo(new Vector2D(0.0, 0.0)));
    });
  }

  [Test]
  public void Indexer_IsCyclicForPositiveAndNegativeIndices() {
    Polyline polyline = new Polyline(PolylineTestData.CreateUnitSquareVertices(), PolylineOrientation.Counterclockwise, false, false);

    Assert.Multiple(() => {
      Assert.That(polyline[4], Is.EqualTo(polyline[0]));
      Assert.That(polyline[5], Is.EqualTo(polyline[1]));
      Assert.That(polyline[-1], Is.EqualTo(polyline[3]));
      Assert.That(polyline[-2], Is.EqualTo(polyline[2]));
    });
  }

  [Test]
  public void Square_ComputesSignedAreaFromVertexOrder() {
    Polyline ccw = new Polyline(PolylineTestData.CreateUnitSquareVertices(), PolylineOrientation.Counterclockwise, false, false);
    Polyline cw = new Polyline(PolylineTestData.CreateUnitSquareVerticesClockwise(), PolylineOrientation.Clockwise, false, false);

    Assert.Multiple(() => {
      Assert.That(Tools.EQ(ccw.Square, 1.0), Is.True);
      Assert.That(Tools.EQ(cw.Square, -1.0), Is.True);
    });
  }

  [Test]
  public void EdgeAngle_ComputesPolarAngleForEachEdgeCyclically() {
    PolylineProbe polyline = new PolylineProbe(PolylineTestData.CreateUnitSquareVertices(), PolylineOrientation.Counterclockwise);

    Assert.Multiple(() => {
      Assert.That(Tools.EQ(polyline.GetEdgeAngle(0), 0.0), Is.True);
      Assert.That(Tools.EQ(polyline.GetEdgeAngle(1), Tools.PI / 2.0), Is.True);
      Assert.That(Tools.EQ(polyline.GetEdgeAngle(2), Tools.PI), Is.True);
      Assert.That(Tools.EQ(polyline.GetEdgeAngle(3), -Tools.PI / 2.0), Is.True);
      Assert.That(Tools.EQ(polyline.GetEdgeAngle(7), -Tools.PI / 2.0), Is.True);
    });
  }

}
