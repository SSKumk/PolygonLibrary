using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;

namespace Tests.DoubleGeometry.Polygons;

internal static class PolygonToolsAssert {

  public static void HasSingleContour(ConvexPolygon polygon, int pointCount, string tooManyContoursMessage, string wrongPointCountMessage) {
    Assert.Multiple(() => {
      Assert.That(polygon.Contours, Has.Count.EqualTo(1), tooManyContoursMessage);
      Assert.That(polygon.Contour, Has.Count.EqualTo(pointCount), wrongPointCountMessage);
    });
  }

  public static void HasTurnedRectangleGeometry(ConvexPolygon polygon, double alpha, int iterationIndex, double cosAbs) {
    for (int k = 1; k < polygon.Contour.Count; k++) {
      int j = k - 1;
      int l = (k + 1) % polygon.Contour.Count;

      Vector2D edge1Original = polygon.Contour[k] - polygon.Contour[j];
      Vector2D edge2Original = polygon.Contour[l] - polygon.Contour[k];
      Vector2D edge1 = edge1Original.Normalize();
      Vector2D edge2 = edge2Original.Normalize();

      Assert.Multiple(() => {
        Assert.That(
          Tools.EQ(double.Abs(edge1 * Vector2D.E1), cosAbs) || Tools.EQ(double.Abs(edge1 * Vector2D.E2), cosAbs),
          "Turned rectangle, alpha = " + alpha + ", i = " + iterationIndex + ": the edge " + edge1Original + " has wrong slope"
        );
        Assert.That(
          Tools.EQ(edge1 * edge2),
          "Turned rectangle, alpha = " + alpha + ", i = " + iterationIndex + ": the edges " + edge1Original + " and " + edge2Original + " are not orthogonal"
        );
      });
    }
  }

}
