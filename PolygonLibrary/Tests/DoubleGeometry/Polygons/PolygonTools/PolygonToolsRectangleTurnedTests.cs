using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;

namespace Tests.DoubleGeometry.Polygons;

[TestFixture]
public class PolygonToolsRectangleTurnedTests {

  [Test]
  public void RectangleTurned_CoincidentPoints_ReturnSinglePointForAllAngles() {
    Vector2D point = new Vector2D(1.0, 2.0);

    for (int i = 0; i <= 6; i++) {
      double alpha = i * Tools.PI / 6.0;
      ConvexPolygon rectangle = PolygonTools.RectangleTurned(point, point, alpha);

      PolygonToolsAssert.HasSingleContour(
        rectangle,
        1,
        "Turned rectangle, alpha = " + alpha + ", i = " + i + ": too many contours",
        "Turned rectangle, alpha = " + alpha + ", i = " + i + ": too many points in the contour"
      );
      Assert.That(
        rectangle.Contour[0],
        Is.EqualTo(point),
        "Turned rectangle, alpha = " + alpha + ", i = " + i + ": wrong point in the contour"
      );
    }
  }

  [TestCase(1.0, 2.0, 1.0, 4.0)]
  [TestCase(1.0, 4.0, 1.0, 2.0)]
  [TestCase(2.0, 1.0, 4.0, 1.0)]
  [TestCase(4.0, 1.0, 2.0, 1.0)]
  public void RectangleTurned_AxisAlignedDiagonals_BecomeSegmentsAtSpecialAnglesAndRectanglesOtherwise(double x1, double y1, double x2, double y2) {
    const int N = 9;
    double a0 = Tools.PI / N;

    for (int i = 0; i <= N; i++) {
      double alpha = i * a0;
      double cosAbs = double.Abs(double.Cos(alpha));
      ConvexPolygon rectangle = PolygonTools.RectangleTurned(new Vector2D(x1, y1), new Vector2D(x2, y2), alpha);

      Assert.That(
        rectangle.Contours,
        Has.Count.EqualTo(1),
        "Turned rectangle, alpha = " + alpha + ", i = " + i + ", i = " + i + ": too many contours"
      );

      if (Tools.NE(alpha) && Tools.NE(alpha, Tools.PI / 2.0) && Tools.NE(alpha, Tools.PI)) {
        Assert.That(
          rectangle.Contour,
          Has.Count.EqualTo(4),
          "Turned rectangle, alpha = " + alpha + ", i = " + i + ": too many points in the contour"
        );
        PolygonToolsAssert.HasTurnedRectangleGeometry(rectangle, alpha, i, cosAbs);
      }
      else {
        Assert.That(
          rectangle.Contour,
          Has.Count.EqualTo(2),
          "Turned rectangle, alpha = " + alpha + ", i = " + i + ": too many points in the contour"
        );
      }
    }
  }

  [TestCase(-2.0, 0.0, 4.0, 1.0)]
  [TestCase(4.0, 1.0, -2.0, 0.0)]
  [TestCase(-2.0, 3.0, 4.0, 1.0)]
  [TestCase(4.0, 1.0, -2.0, 3.0)]
  public void RectangleTurned_GeneralDiagonals_KeepFourVerticesAndOrthogonalEdges(double x1, double y1, double x2, double y2) {
    const int N = 9;
    double a0 = Tools.PI / N;

    for (int i = 0; i <= N; i++) {
      double alpha = i * a0;
      double cosAbs = double.Abs(double.Cos(alpha));
      ConvexPolygon rectangle = PolygonTools.RectangleTurned(new Vector2D(x1, y1), new Vector2D(x2, y2), alpha);

      Assert.Multiple(() => {
        Assert.That(
          rectangle.Contours,
          Has.Count.EqualTo(1),
          "Turned rectangle, alpha = " + alpha + ", i = " + i + ", i = " + i + ": too many contours"
        );
        Assert.That(
          rectangle.Contour,
          Has.Count.EqualTo(4),
          "Turned rectangle, alpha = " + alpha + ", i = " + i + ": too many points in the contour"
        );
      });

      PolygonToolsAssert.HasTurnedRectangleGeometry(rectangle, alpha, i, cosAbs);
    }
  }

}
