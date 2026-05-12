using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;

namespace Tests.DoubleGeometry.Polygons;

[TestFixture]
public class ConvexPolygonConstructionTests {

  [Test]
  public void Constructor_Vector2DWithoutConvexify_PreservesSingleContourVertices() {
    ConvexPolygon polygon = new ConvexPolygon(ConvexPolygonTestData.SquareVerticesClockwiseStartingTopRight, false);

    Assert.Multiple(() => {
      Assert.That(polygon.Contours, Has.Count.EqualTo(1));
      Assert.That(polygon.Contour, Has.Count.EqualTo(4));
      Assert.That(polygon.Vertices, Is.EqualTo(ConvexPolygonTestData.SquareVerticesClockwiseStartingTopRight));
    });
  }

  [Test]
  public void Constructor_Vector2DWithConvexify_BuildsConvexHullFromCrossedPointSet() {
    ConvexPolygon polygon = new ConvexPolygon(ConvexPolygonTestData.CrossedSquarePoints, true);
    List<Vector2D> expected =
      new() {
        new Vector2D(-1, -1),
        new Vector2D(1, -1),
        new Vector2D(1, 1),
        new Vector2D(-1, 1)
      };

    Assert.Multiple(() => {
      Assert.That(polygon.Contours, Has.Count.EqualTo(1));
      ConvexPolygonAssert.CyclicListComparison(polygon.Contour.Vertices, expected, "CreateCPOfPointsTest2");
    });
  }

  [Test]
  public void Constructor_VectorEnumerable_ProjectsToVector2DVertices() {
    ConvexPolygon polygon = new ConvexPolygon(ConvexPolygonTestData.CreateSquareVectors(), false);

    Assert.That(polygon.Vertices, Is.EqualTo(ConvexPolygonTestData.SquareVerticesClockwiseStartingTopRight));
  }

  [Test]
  public void Constructor_SupportFunction_ComputesContourVerticesLazily() {
    SupportFunction supportFunction = new SupportFunction(ConvexPolygonTestData.AxisAlignedSquareGammaPairs);
    ConvexPolygon polygon = new ConvexPolygon(supportFunction);
    List<Vector2D> expected =
      new() {
        new Vector2D(1, -1),
        new Vector2D(1, 1),
        new Vector2D(-1, 1),
        new Vector2D(-1, -1)
      };

    Assert.Multiple(() => {
      Assert.That(polygon.SF, Is.SameAs(supportFunction));
      ConvexPolygonAssert.CyclicListComparison(polygon.Contour.Vertices, expected, "CreateCPOfCFTest1");
      Assert.That(polygon.Vertices, Is.EqualTo(polygon.Contour.Vertices));
    });
  }

  [Test]
  public void SupportFunction_IsComputedFromContourWhenPolygonStartsFromVertices() {
    ConvexPolygon polygon = new ConvexPolygon(ConvexPolygonTestData.SquareVerticesClockwiseStartingTopRight, true);
    SupportFunction? supportFunction = polygon.SF;

    Assert.Multiple(() => {
      Assert.That(supportFunction, Is.Not.Null);
      Assert.That(supportFunction, Has.Count.EqualTo(4));
      Assert.That(supportFunction![0], Is.EqualTo(new GammaPair(new Vector2D(0, -1), 1)));
      Assert.That(supportFunction[1], Is.EqualTo(new GammaPair(new Vector2D(1, 0), 1)));
      Assert.That(supportFunction[2], Is.EqualTo(new GammaPair(new Vector2D(0, 1), 1)));
      Assert.That(supportFunction[3], Is.EqualTo(new GammaPair(new Vector2D(-1, 0), 1)));
    });
  }

}
