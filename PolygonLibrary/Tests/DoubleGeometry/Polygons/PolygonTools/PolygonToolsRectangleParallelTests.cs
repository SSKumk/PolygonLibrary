using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;

namespace Tests.DoubleGeometry.Polygons;

[TestFixture]
public class PolygonToolsRectangleParallelTests {

  [Test]
  public void RectangleParallel_CoincidentPoints_ReturnsSinglePointPolygon() {
    Vector2D point = new Vector2D(1.0, 2.0);
    ConvexPolygon rectangle = PolygonTools.RectangleParallel(point, point);

    PolygonToolsAssert.HasSingleContour(
      rectangle,
      1,
      "Vector rectangle: too many contours",
      "Vector rectangle: too many points in the contour"
    );
    Assert.That(rectangle.Contour[0], Is.EqualTo(point), "Vector rectangle: wrong point in the contour");
  }

  [TestCase(1.0, 2.0, 1.0, 3.0, "Vertical segment rectangle 1")]
  [TestCase(1.0, 3.0, 1.0, 2.0, "Vertical segment rectangle 2")]
  [TestCase(1.0, 2.0, 5.0, 2.0, "Horizontal segment rectangle 1")]
  [TestCase(5.0, 2.0, 1.0, 2.0, "Horizontal segment rectangle 2")]
  public void RectangleParallel_AxisAlignedDegenerateCases_ReturnSegments(double x1, double y1, double x2, double y2, string label) {
    ConvexPolygon rectangle = PolygonTools.RectangleParallel(new Vector2D(x1, y1), new Vector2D(x2, y2));

    PolygonToolsAssert.HasSingleContour(
      rectangle,
      2,
      label + ": too many contours",
      label + ": wrong number points in the contour"
    );
  }

  [TestCase(1.0, 2.0, 5.0, 3.0, "LL-RU rectangle 1")]
  [TestCase(5.0, 3.0, 1.0, 2.0, "LL-RU rectangle 2")]
  [TestCase(1.0, 2.0, 5.0, -3.0, "LU-RL rectangle 1")]
  [TestCase(5.0, -3.0, 1.0, 2.0, "LU-RL rectangle 2")]
  public void RectangleParallel_NonDegenerateCases_ReturnFourVertices(double x1, double y1, double x2, double y2, string label) {
    ConvexPolygon rectangle = PolygonTools.RectangleParallel(new Vector2D(x1, y1), new Vector2D(x2, y2));

    PolygonToolsAssert.HasSingleContour(
      rectangle,
      4,
      label + ": too many contours",
      label + ": wrong number points in the contour"
    );
  }

}
