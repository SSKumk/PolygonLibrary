using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;

namespace Tests.DoubleGeometry.Polygons;

[TestFixture]
public class PolygonToolsCircleAndEllipseTests {

  [Test]
  public void Circle_ZeroRadius_ReturnsSinglePointPolygon() {
    Vector2D center = new Vector2D(2.0, -1.0);
    ConvexPolygon circle = PolygonTools.Circle(center, 0.0, 12);

    Assert.Multiple(() => {
      Assert.That(circle.Contours, Has.Count.EqualTo(1));
      Assert.That(circle.Contour, Has.Count.EqualTo(1));
      Assert.That(circle.Contour[0], Is.EqualTo(center));
    });
  }

  [Test]
  public void Circle_PositiveRadius_ReturnsRegularPolygonOnRequestedCircle() {
    double x = 2.0;
    double y = -1.0;
    double radius = 3.0;
    int n = 6;
    ConvexPolygon circle = PolygonTools.Circle(x, y, radius, n);
    Vector2D center = new Vector2D(x, y);

    Assert.That(circle.Contour, Has.Count.EqualTo(n));
    Assert.That(circle.Contour[0], Is.EqualTo(new Vector2D(x + radius, y)));

    foreach (Vector2D vertex in circle.Contour.Vertices) {
      Assert.That(Tools.EQ(Vector2D.Dist(vertex, center), radius), Is.True);
    }
  }

  [Test]
  public void Circle_WithAdditionalAngle_RotatesFirstVertex() {
    double x = 0.0;
    double y = 0.0;
    double radius = 2.0;
    double a0 = Tools.PI / 4.0;
    ConvexPolygon circle = PolygonTools.Circle(x, y, radius, 8, a0);

    Assert.That(
      circle.Contour[0],
      Is.EqualTo(new Vector2D(radius * double.Cos(a0), radius * double.Sin(a0)))
    );
  }

  [Test]
  public void Ellipse_BothSemiaxesZero_ReturnsSinglePointPolygon() {
    Vector2D center = new Vector2D(3.0, -2.0);
    ConvexPolygon ellipse = PolygonTools.Ellipse(center.x, center.y, 0.0, 0.0, 16);

    Assert.Multiple(() => {
      Assert.That(ellipse.Contours, Has.Count.EqualTo(1));
      Assert.That(ellipse.Contour, Has.Count.EqualTo(1));
      Assert.That(ellipse.Contour[0], Is.EqualTo(center));
    });
  }

  [Test]
  public void Ellipse_ZeroMinorSemiaxis_ReturnsSegmentAlongRotatedMajorAxis() {
    double x = 1.0;
    double y = -1.0;
    double a = 3.0;
    double phi = Tools.PI / 6.0;
    ConvexPolygon ellipse = PolygonTools.Ellipse(x, y, a, 0.0, 12, phi, 0.0);

    Vector2D p1 = new Vector2D(x + a * double.Cos(phi), y + a * double.Sin(phi));
    Vector2D p2 = new Vector2D(x - a * double.Cos(phi), y - a * double.Sin(phi));

    Assert.Multiple(() => {
      Assert.That(ellipse.Contours, Has.Count.EqualTo(1));
      Assert.That(ellipse.Contour, Has.Count.EqualTo(2));
      Assert.That(ellipse.Contour.Vertices, Does.Contain(p1));
      Assert.That(ellipse.Contour.Vertices, Does.Contain(p2));
    });
  }

  [Test]
  public void Ellipse_ZeroMajorSemiaxis_ReturnsSegmentAlongRotatedMinorAxis() {
    double x = 1.0;
    double y = -1.0;
    double b = 2.0;
    double phi = Tools.PI / 6.0;
    ConvexPolygon ellipse = PolygonTools.Ellipse(x, y, 0.0, b, 12, phi, 0.0);

    Vector2D p1 = new Vector2D(x - b * double.Sin(phi), y + b * double.Cos(phi));
    Vector2D p2 = new Vector2D(x + b * double.Sin(phi), y - b * double.Cos(phi));

    Assert.Multiple(() => {
      Assert.That(ellipse.Contours, Has.Count.EqualTo(1));
      Assert.That(ellipse.Contour, Has.Count.EqualTo(2));
      Assert.That(ellipse.Contour.Vertices, Does.Contain(p1));
      Assert.That(ellipse.Contour.Vertices, Does.Contain(p2));
    });
  }

  [Test]
  public void Ellipse_PositiveSemiaxes_ReturnVerticesOnRotatedEllipse() {
    double x = 1.0;
    double y = -2.0;
    double a = 4.0;
    double b = 2.0;
    double phi = Tools.PI / 6.0;
    double a0 = Tools.PI / 8.0;
    int n = 10;
    ConvexPolygon ellipse = PolygonTools.Ellipse(x, y, a, b, n, phi, a0);

    Assert.That(ellipse.Contour, Has.Count.EqualTo(n));

    foreach (Vector2D vertex in ellipse.Contour.Vertices) {
      double dx = vertex.x - x;
      double dy = vertex.y - y;
      double localX = dx * double.Cos(phi) + dy * double.Sin(phi);
      double localY = -dx * double.Sin(phi) + dy * double.Cos(phi);
      double normalized = localX * localX / (a * a) + localY * localY / (b * b);

      Assert.That(Tools.EQ(normalized, 1.0), Is.True);
    }
  }

}
