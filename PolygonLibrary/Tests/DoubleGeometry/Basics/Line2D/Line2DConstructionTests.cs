using System;
using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;

namespace Tests.DoubleGeometry.Basics;

[TestFixture]
public class Line2DConstructionTests {

  [Test]
  public void Constructor_Default_CreatesOxAxis() {
    Line2D line = new Line2D();

    Assert.Multiple(() => {
      Assert.That(Tools.EQ(line.A), Is.True);
      Assert.That(Tools.EQ(line.B, 1.0), Is.True);
      Assert.That(Tools.EQ(line.C), Is.True);
      Line2DAssert.AreEqual(line.Direct, Vector2D.E1);
      Line2DAssert.AreEqual(line.Normal, Vector2D.E2);
      Assert.That(line.PassesThrough(new Vector2D(-3.0, 0.0)), Is.True);
      Assert.That(Tools.GT(line[new Vector2D(0.0, 2.0)]), Is.True);
      Assert.That(Tools.LT(line[new Vector2D(0.0, -2.0)]), Is.True);
    });

    Line2DAssert.HasInvariantGeometry(line, new Vector2D(0.0, 0.0));
  }

  [Test]
  public void Constructor_TwoPoints_HorizontalLine_UsesLeftHalfPlane() {
    Vector2D p1 = new Vector2D(0.0, 0.0);
    Vector2D p2 = new Vector2D(2.0, 0.0);
    Line2D   line = new Line2D(p1, p2);

    Assert.Multiple(() => {
      Assert.That(line.PassesThrough(p1), Is.True);
      Assert.That(line.PassesThrough(p2), Is.True);
      Line2DAssert.AreEqual(line.Direct, Vector2D.E1);
      Line2DAssert.AreEqual(line.Normal, Vector2D.E2);
      Assert.That(Tools.GT(line[new Vector2D(0.0, 1.0)]), Is.True);
      Assert.That(Tools.LT(line[new Vector2D(0.0, -1.0)]), Is.True);
    });

    Line2DAssert.HasInvariantGeometry(line, p1);
  }

  [Test]
  public void Constructor_TwoPoints_VerticalLine_UsesLeftHalfPlane() {
    Vector2D p1 = new Vector2D(0.0, 0.0);
    Vector2D p2 = new Vector2D(0.0, 3.0);
    Line2D   line = new Line2D(p1, p2);

    Assert.Multiple(() => {
      Assert.That(line.PassesThrough(p1), Is.True);
      Assert.That(line.PassesThrough(p2), Is.True);
      Line2DAssert.AreEqual(line.Direct, Vector2D.E2);
      Line2DAssert.AreEqual(line.Normal, new Vector2D(-1.0, 0.0));
      Assert.That(Tools.GT(line[new Vector2D(-1.0, 0.0)]), Is.True);
      Assert.That(Tools.LT(line[new Vector2D(1.0, 0.0)]), Is.True);
    });

    Line2DAssert.HasInvariantGeometry(line, p1);
  }

  [Test]
  public void Constructor_TwoPoints_InclinedLine_NormalizesDirectionAndNormal() {
    Vector2D p1 = new Vector2D(1.0, 1.0);
    Vector2D p2 = new Vector2D(3.0, 3.0);
    Line2D   line = new Line2D(p1, p2);
    double   coord = double.Sqrt(0.5);

    Assert.Multiple(() => {
      Assert.That(line.PassesThrough(p1), Is.True);
      Assert.That(line.PassesThrough(p2), Is.True);
      Line2DAssert.AreEqual(line.Direct, new Vector2D(coord, coord));
      Line2DAssert.AreEqual(line.Normal, new Vector2D(-coord, coord));
      Assert.That(Tools.GT(line[new Vector2D(1.0, 2.0)]), Is.True);
      Assert.That(Tools.LT(line[new Vector2D(2.0, 1.0)]), Is.True);
    });

    Line2DAssert.HasInvariantGeometry(line, p1);
  }

  [Test]
  public void Constructor_TwoPoints_ReversedPoints_PreserveGeometryButFlipOrientation() {
    Vector2D p1 = new Vector2D(0.0, 0.0);
    Vector2D p2 = new Vector2D(2.0, 0.0);
    Line2D   forward = new Line2D(p1, p2);
    Line2D   reversed = new Line2D(p2, p1);

    Vector2D upper = new Vector2D(0.0, 1.0);
    Vector2D lower = new Vector2D(0.0, -1.0);

    Assert.Multiple(() => {
      Assert.That(forward.PassesThrough(p1), Is.True);
      Assert.That(forward.PassesThrough(p2), Is.True);
      Assert.That(reversed.PassesThrough(p1), Is.True);
      Assert.That(reversed.PassesThrough(p2), Is.True);
      Line2DAssert.AreEqual(reversed.Direct, -forward.Direct);
      Line2DAssert.AreEqual(reversed.Normal, -forward.Normal);
      Assert.That(Tools.GT(forward[upper]), Is.True);
      Assert.That(Tools.LT(reversed[upper]), Is.True);
      Assert.That(Tools.LT(forward[lower]), Is.True);
      Assert.That(Tools.GT(reversed[lower]), Is.True);
    });

    Line2DAssert.HasInvariantGeometry(forward, p1);
    Line2DAssert.HasInvariantGeometry(reversed, p1);
  }

  [Test]
  public void Constructor_TwoPointsAndExternalPoint_OrientsPositiveHalfPlaneTowardsExternalPoint() {
    Vector2D p1 = new Vector2D(0.0, 0.0);
    Vector2D p2 = new Vector2D(2.0, 0.0);
    Vector2D external = new Vector2D(0.0, -1.0);
    Line2D   line = new Line2D(p1, p2, external);

    Assert.Multiple(() => {
      Assert.That(line.PassesThrough(p1), Is.True);
      Assert.That(line.PassesThrough(p2), Is.True);
      Assert.That(Tools.GT(line[external]), Is.True);
      Assert.That(Tools.LT(line[new Vector2D(0.0, 1.0)]), Is.True);
      Line2DAssert.AreEqual(line.Direct, new Vector2D(-1.0, 0.0));
      Line2DAssert.AreEqual(line.Normal, new Vector2D(0.0, -1.0));
    });

    Line2DAssert.HasInvariantGeometry(line, p1);
  }

  [Test]
  public void Constructor_TwoPointsAndExternalPoint_ThrowsWhenExternalPointBelongsToLine() {
    Vector2D p1 = new Vector2D(0.0, 0.0);
    Vector2D p2 = new Vector2D(2.0, 0.0);
    Vector2D p3 = new Vector2D(10.0, 0.0);

    Assert.That(
      () => new Line2D(p1, p2, p3),
      Throws.TypeOf<ArgumentException>().With.Message.EqualTo("The point that should define the positive halfplane belongs to the line")
    );
  }

  [Test]
  public void Factory_PointAndDirect_CreatesLineFromPointAndDirection() {
    Vector2D point = new Vector2D(1.0, 2.0);
    Vector2D direct = new Vector2D(4.0, 0.0);
    Line2D   line = Line2D.Line2D_PointAndDirect(point, direct);

    Assert.Multiple(() => {
      Assert.That(line.PassesThrough(point), Is.True);
      Line2DAssert.AreEqual(line.Direct, Vector2D.E1);
      Line2DAssert.AreEqual(line.Normal, Vector2D.E2);
      Assert.That(Tools.GT(line[new Vector2D(1.0, 3.0)]), Is.True);
      Assert.That(Tools.LT(line[new Vector2D(1.0, 1.0)]), Is.True);
    });

    Line2DAssert.HasInvariantGeometry(line, point);
  }

  [Test]
  public void Factory_PointAndDirect_WithExternalPoint_UsesRequestedOrientation() {
    Vector2D point = new Vector2D(0.0, 0.0);
    Vector2D direct = new Vector2D(2.0, 0.0);
    Vector2D external = new Vector2D(0.0, -2.0);
    Line2D   line = Line2D.Line2D_PointAndDirect(point, direct, external);

    Assert.Multiple(() => {
      Assert.That(line.PassesThrough(point), Is.True);
      Assert.That(Tools.GT(line[external]), Is.True);
      Assert.That(Tools.LT(line[new Vector2D(0.0, 2.0)]), Is.True);
    });

    Line2DAssert.HasInvariantGeometry(line, point);
  }

  [Test]
  public void Factory_PointAndNormal_CreatesLineFromPointAndNormal() {
    Vector2D point = new Vector2D(0.0, 2.0);
    Vector2D normal = new Vector2D(0.0, 5.0);
    Line2D   line = Line2D.Line2D_PointAndNormal(point, normal);

    Assert.Multiple(() => {
      Assert.That(line.PassesThrough(point), Is.True);
      Line2DAssert.AreEqual(line.Normal, Vector2D.E2);
      Line2DAssert.AreEqual(line.Direct, new Vector2D(1.0, 0.0));
      Assert.That(Tools.GT(line[new Vector2D(0.0, 3.0)]), Is.True);
      Assert.That(Tools.LT(line[new Vector2D(0.0, 1.0)]), Is.True);
    });

    Line2DAssert.HasInvariantGeometry(line, point);
  }

  [Test]
  public void CopyConstructor_CopiesEquivalentLine() {
    Line2D source = new Line2D(new Vector2D(1.0, 1.0), new Vector2D(3.0, 1.0), new Vector2D(2.0, 2.0));
    Line2D copy = new Line2D(source);

    Assert.Multiple(() => {
      Assert.That(Tools.EQ(copy.A, source.A), Is.True);
      Assert.That(Tools.EQ(copy.B, source.B), Is.True);
      Assert.That(Tools.EQ(copy.C, source.C), Is.True);
      Line2DAssert.AreEqual(copy.Direct, source.Direct);
      Line2DAssert.AreEqual(copy.Normal, source.Normal);
      Assert.That(Tools.GT(copy[new Vector2D(2.0, 2.0)]), Is.True);
      Assert.That(Tools.LT(copy[new Vector2D(2.0, 0.0)]), Is.True);
    });

    Line2DAssert.HasInvariantGeometry(copy, new Vector2D(1.0, 1.0));
  }

}
