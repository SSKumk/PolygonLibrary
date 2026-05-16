using CGLibrary;
using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;

namespace Tests.DoubleGeometry.Polygons;

[TestFixture]
public class PolylineContainmentTests {

  private static void AssertContainsAndInside(Polyline polyline, Vector2D point, string containsMessage, string insideMessage) {
    Assert.Multiple(() => {
      Assert.That(polyline.ContainsPoint(point), containsMessage);
      Assert.That(polyline.ContainsPointInside(point), insideMessage);
    });
  }

  private static void AssertOutside(Polyline polyline, Vector2D point, string containsMessage, string insideMessage) {
    Assert.Multiple(() => {
      Assert.That(polyline.ContainsPoint(point), Is.False, containsMessage);
      Assert.That(polyline.ContainsPointInside(point), Is.False, insideMessage);
    });
  }

  [Test]
  public void ContainsPoint_UnitSquare_AcceptsInteriorDiagonalPointsNearTopRightCorner() {
    Polyline polyline = new Polyline(PolylineTestData.CreateUnitSquareVertices(), PolylineOrientation.Counterclockwise, false, false);

    for (int i = 1; i <= 10; i++) {
      double x = 1.0 - 1.0 / double.Pow(2.0, i);
      Vector2D point = new Vector2D(x, x);
      AssertContainsAndInside(
        polyline,
        point,
        "${i}th test: the polyline does not contain the point, which should contain",
        "${i}th test: the polyline does not contain the point inside it, which should contain"
      );
    }
  }

  [Test]
  public void ContainsPoint_UnitSquare_AcceptsInteriorPointsNearBottomEdge() {
    Polyline polyline = new Polyline(PolylineTestData.CreateUnitSquareVertices(), PolylineOrientation.Counterclockwise, false, false);

    for (int i = 1; i <= 10; i++) {
      double x = 1.0 - 1.0 / double.Pow(2.0, i);
      Vector2D point = new Vector2D(x, 0.1);
      AssertContainsAndInside(
        polyline,
        point,
        "${i}th test: the polyline does not contain the point, which should contain",
        "${i}th test: the polyline does not contain the point inside it, which should contain"
      );
    }
  }

  [Test]
  public void ContainsPoint_UnitSquare_AcceptsInteriorDiagonalPointsNearOrigin() {
    Polyline polyline = new Polyline(PolylineTestData.CreateUnitSquareVertices(), PolylineOrientation.Counterclockwise, false, false);

    for (int i = 1; i <= 10; i++) {
      double x = 1.0 / double.Pow(2.0, i);
      Vector2D point = new Vector2D(x, x);
      AssertContainsAndInside(
        polyline,
        point,
        "${i}th test: the polyline does not contain the point, which should contain",
        "${i}th test: the polyline does not contain the point inside it, which should contain"
      );
    }
  }

  [Test]
  public void ContainsPoint_UnitSquare_RejectsExteriorPointsToTheRightAndOutsideTheBoundingBox() {
    Polyline polyline = new Polyline(PolylineTestData.CreateUnitSquareVertices(), PolylineOrientation.Counterclockwise, false, false);

    for (int i = 1; i <= 10; i++) {
      double x = 1.0 + 1.0 / double.Pow(2.0, i);

      AssertOutside(
        polyline,
        new Vector2D(x, x),
        "${i}th test: the polyline does contain the point, which should not contain",
        "${i}th test: the polyline does contain the point inside it, which should not contain"
      );

      AssertOutside(
        polyline,
        new Vector2D(x, 0.1),
        "${i}th test: the polyline does contain the point, which should not contain",
        "${i}th test: the polyline does contain the point inside it, which should not contain"
      );
    }
  }

  [Test]
  public void ContainsPoint_UnitSquare_RejectsExteriorPointsToTheLeft() {
    Polyline polyline = new Polyline(PolylineTestData.CreateUnitSquareVertices(), PolylineOrientation.Counterclockwise, false, false);

    for (int i = 1; i <= 10; i++) {
      double x = -1.0 / double.Pow(2.0, i);
      Vector2D point = new Vector2D(x, x);
      AssertOutside(
        polyline,
        point,
        "${i}th test: the polyline does contain the point, which should not contain",
        "${i}th test: the polyline does contain the point inside it, which should not contain"
      );
    }
  }

  [Test]
  public void ContainsPoint_UnitSquare_TreatsVerticesAndEdgePointsAsBoundaryOnly() {
    Polyline polyline = new Polyline(PolylineTestData.CreateUnitSquareVertices(), PolylineOrientation.Counterclockwise, false, false);
    List<Vector2D> vertices = PolylineTestData.CreateUnitSquareVertices();

    for (int i = 0; i < vertices.Count; i++) {
      Assert.Multiple(() => {
        Assert.That(
          polyline.ContainsPoint(vertices[i]),
          "${i}th test: the polyline does not contain the point, which should contain"
        );
        Assert.That(
          polyline.ContainsPointInside(vertices[i]),
          Is.False,
          "${i}th test: the polyline does contain the point inside it, which should not contain"
        );
      });
    }

    Assert.Multiple(() => {
      Assert.That(polyline.ContainsPoint(new Vector2D(1.0, 0.5)), Is.True);
      Assert.That(polyline.ContainsPointInside(new Vector2D(1.0, 0.5)), Is.False);
      Assert.That(polyline.ContainsPoint(new Vector2D(1.0000001, 0.5)), Is.False);
      Assert.That(polyline.ContainsPointInside(new Vector2D(1.0000001, 0.5)), Is.False);
    });
  }

  [Test]
  public void ContainsPoint_IndentedPolyline_AcceptsInteriorPointsInMainBodyAndNearDentEdge() {
    Polyline polyline = new Polyline(PolylineTestData.CreateIndentedPentagonVertices(), PolylineOrientation.Counterclockwise, false, false);

    for (int i = 1; i <= 10; i++) {
      double xRight = 1.0 - 1.0 / double.Pow(2.0, i);
      double xLeft = 1.0 / double.Pow(2.0, i);
      double xDent = 0.1 + 1.0 / double.Pow(2.0, i);

      AssertContainsAndInside(
        polyline,
        new Vector2D(xRight, xRight),
        "${i}th test: the polyline does not contain the point, which should contain",
        "${i}th test: the polyline does not contain the point inside, which should contain"
      );

      AssertContainsAndInside(
        polyline,
        new Vector2D(xRight, 0.1),
        "${i}th test: the polyline does not contain the point, which should contain",
        "${i}th test: the polyline does not contain the point inside, which should contain"
      );

      AssertContainsAndInside(
        polyline,
        new Vector2D(xLeft, xLeft),
        "${i}th test: the polyline does not contain the point, which should contain",
        "${i}th test: the polyline does not contain the point inside, which should contain"
      );

      AssertContainsAndInside(
        polyline,
        new Vector2D(xDent, 0.5),
        "${i}th test: the polyline does not contain the point, which should contain",
        "${i}th test: the polyline does not contain the point inside, which should contain"
      );
    }
  }

  [Test]
  public void ContainsPoint_IndentedPolyline_RejectsExteriorPointsAtRightLeftAndInConcaveCut() {
    Polyline polyline = new Polyline(PolylineTestData.CreateIndentedPentagonVertices(), PolylineOrientation.Counterclockwise, false, false);

    for (int i = 1; i <= 10; i++) {
      double xRight = 1.0 + 1.0 / double.Pow(2.0, i);
      double xLeft = -1.0 / double.Pow(2.0, i);
      double xDent = 0.1 - 1.0 / double.Pow(2.0, i);

      AssertOutside(
        polyline,
        new Vector2D(xRight, xRight),
        "${i}th test: the polyline does contain the point, which should not contain",
        "${i}th test: the polyline does contain the point inside, which should not contain"
      );

      AssertOutside(
        polyline,
        new Vector2D(xRight, 0.1),
        "${i}th test: the polyline does contain the point, which should not contain",
        "${i}th test: the polyline does contain the point inside, which should not contain"
      );

      AssertOutside(
        polyline,
        new Vector2D(xLeft, xLeft),
        "${i}th test: the polyline does contain the point, which should not contain",
        "${i}th test: the polyline does contain the point inside, which should not contain"
      );

      AssertOutside(
        polyline,
        new Vector2D(xDent, 0.5),
        "${i}th test: the polyline does contain the point, which should not contain",
        "${i}th test: the polyline does contain the point inside, which should not contain"
      );
    }
  }

  [Test]
  public void ContainsPoint_IndentedPolyline_TreatsVerticesAndConcaveBoundaryPointsAsBoundaryOnly() {
    List<Vector2D> vertices = PolylineTestData.CreateIndentedPentagonVertices();
    Polyline polyline = new Polyline(vertices, PolylineOrientation.Counterclockwise, false, false);

    for (int i = 0; i < vertices.Count; i++) {
      Assert.Multiple(() => {
        Assert.That(
          polyline.ContainsPoint(vertices[i]),
          "${i}th test: the polyline does not contain the point, which should contain"
        );
        Assert.That(
          polyline.ContainsPointInside(vertices[i]),
          Is.False,
          "${i}th test: the polyline does contain the point inside, which should not contain"
        );
      });
    }

    Assert.Multiple(() => {
      Assert.That(polyline.ContainsPoint(new Vector2D(0.05, 0.75)), Is.True);
      Assert.That(polyline.ContainsPointInside(new Vector2D(0.05, 0.75)), Is.False);
      Assert.That(polyline.ContainsPoint(new Vector2D(1.0, 0.5)), Is.True);
      Assert.That(polyline.ContainsPointInside(new Vector2D(1.0, 0.5)), Is.False);
      Assert.That(polyline.ContainsPoint(new Vector2D(1.0000001, 0.5)), Is.False);
      Assert.That(polyline.ContainsPointInside(new Vector2D(1.0000001, 0.5)), Is.False);
    });
  }

  [Test]
  public void ContainsPoint_WorksForClockwiseEnumeratedPolylineAsWell() {
    Polyline polyline = new Polyline(PolylineTestData.CreateUnitSquareVerticesClockwise(), PolylineOrientation.Clockwise, false, false);
    Vector2D inside = new Vector2D(0.25, 0.25);
    Vector2D outside = new Vector2D(1.25, 0.25);

    Assert.Multiple(() => {
      Assert.That(polyline.ContainsPoint(inside), Is.True);
      Assert.That(polyline.ContainsPointInside(inside), Is.True);
      Assert.That(polyline.ContainsPoint(outside), Is.False);
      Assert.That(polyline.ContainsPointInside(outside), Is.False);
    });
  }

}
