using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;

namespace Tests.DoubleGeometry.Basics;

[TestFixture]
public class Line2DQueriesAndIntersectionTests {

  [Test]
  public void Indexer_ReturnsExpectedSignsForPointsInDifferentHalfPlanes() {
    Line2D line = new Line2D(new Vector2D(0.0, 0.0), new Vector2D(2.0, 0.0));

    Assert.Multiple(() => {
      Assert.That(Tools.EQ(line[new Vector2D(10.0, 0.0)]), Is.True);
      Assert.That(Tools.GT(line[new Vector2D(0.0, 3.0)]), Is.True);
      Assert.That(Tools.LT(line[new Vector2D(0.0, -3.0)]), Is.True);
    });
  }

  [Test]
  public void PassesThrough_ReturnsTrueForPointOnLineAndFalseOtherwise() {
    Line2D line = new Line2D(new Vector2D(0.0, 1.0), new Vector2D(2.0, 3.0));

    Assert.Multiple(() => {
      Assert.That(line.PassesThrough(new Vector2D(1.0, 2.0)), Is.True);
      Assert.That(line.PassesThrough(new Vector2D(2.0, 2.0)), Is.False);
    });
  }

  [Test]
  public void Reorient_KeepsSameGeometricLineButFlipsHalfPlanes() {
    Line2D original = new Line2D(new Vector2D(0.0, 0.0), new Vector2D(2.0, 0.0));
    Line2D reoriented = original.Reorient();
    Vector2D pointOnLine = new Vector2D(1.0, 0.0);
    Vector2D positive = new Vector2D(0.0, 1.0);
    Vector2D negative = new Vector2D(0.0, -1.0);

    Assert.Multiple(() => {
      Assert.That(reoriented.PassesThrough(pointOnLine), Is.True);
      Assert.That(Tools.EQ(reoriented[pointOnLine]), Is.True);
      Assert.That(Tools.GT(original[positive]), Is.True);
      Assert.That(Tools.LT(reoriented[positive]), Is.True);
      Assert.That(Tools.LT(original[negative]), Is.True);
      Assert.That(Tools.GT(reoriented[negative]), Is.True);
      Line2DAssert.AreEqual(reoriented.Direct, -original.Direct);
      Line2DAssert.AreEqual(reoriented.Normal, -original.Normal);
    });

    Line2DAssert.HasInvariantGeometry(reoriented, pointOnLine);
  }

  [Test]
  public void Reorient_Twice_ReturnsEquivalentOrientation() {
    Line2D original = new Line2D(new Vector2D(1.0, 1.0), new Vector2D(3.0, 3.0));
    Line2D twice = original.Reorient().Reorient();
    Vector2D left = new Vector2D(1.0, 2.0);
    Vector2D right = new Vector2D(2.0, 1.0);

    Assert.Multiple(() => {
      Assert.That(Tools.EQ(twice.A, original.A), Is.True);
      Assert.That(Tools.EQ(twice.B, original.B), Is.True);
      Assert.That(Tools.EQ(twice.C, original.C), Is.True);
      Line2DAssert.AreEqual(twice.Direct, original.Direct);
      Line2DAssert.AreEqual(twice.Normal, original.Normal);
      Assert.That(Tools.GT(twice[left]), Is.True);
      Assert.That(Tools.LT(twice[right]), Is.True);
    });
  }

  [Test]
  public void Intersect_HorizontalAndVerticalLines_ReturnSinglePoint() {
    Line2D horizontal = new Line2D(new Vector2D(0.0, 1.0), new Vector2D(2.0, 1.0));
    Line2D vertical = new Line2D(new Vector2D(3.0, 0.0), new Vector2D(3.0, 5.0));

    Line2D.LineCrossType crossType = Line2D.Intersect(horizontal, vertical, out Vector2D? point);

    Assert.Multiple(() => {
      Assert.That(crossType, Is.EqualTo(Line2D.LineCrossType.SinglePoint));
      Assert.That(point, Is.Not.Null);
      Line2DAssert.AreEqual(point!, new Vector2D(3.0, 1.0));
    });
  }

  [Test]
  public void Intersect_TwoInclinedLines_ReturnSinglePoint() {
    Line2D first = new Line2D(new Vector2D(0.0, 0.0), new Vector2D(2.0, 2.0));
    Line2D second = new Line2D(new Vector2D(0.0, 2.0), new Vector2D(2.0, 0.0));

    Line2D.LineCrossType crossType = Line2D.Intersect(first, second, out Vector2D? point);

    Assert.Multiple(() => {
      Assert.That(crossType, Is.EqualTo(Line2D.LineCrossType.SinglePoint));
      Assert.That(point, Is.Not.Null);
      Line2DAssert.AreEqual(point!, new Vector2D(1.0, 1.0));
    });
  }

  [Test]
  public void Intersect_DistinctParallelLines_ReturnParallelAndNullPoint() {
    Line2D first = new Line2D(new Vector2D(0.0, 0.0), new Vector2D(2.0, 0.0));
    Line2D second = new Line2D(new Vector2D(0.0, 2.0), new Vector2D(2.0, 2.0));

    Line2D.LineCrossType crossType = Line2D.Intersect(first, second, out Vector2D? point);

    Assert.Multiple(() => {
      Assert.That(crossType, Is.EqualTo(Line2D.LineCrossType.Parallel));
      Assert.That(point, Is.Null);
    });
  }

  [Test]
  public void Intersect_CoincidentLines_ReturnOverlapAndNullPoint() {
    Line2D first = new Line2D(new Vector2D(0.0, 0.0), new Vector2D(2.0, 2.0));
    Line2D second = new Line2D(new Vector2D(1.0, 1.0), new Vector2D(3.0, 3.0));

    Line2D.LineCrossType crossType = Line2D.Intersect(first, second, out Vector2D? point);

    Assert.Multiple(() => {
      Assert.That(crossType, Is.EqualTo(Line2D.LineCrossType.Overlap));
      Assert.That(point, Is.Null);
    });
  }

  [Test]
  public void Intersect_IsSymmetricByArguments() {
    Line2D first = new Line2D(new Vector2D(0.0, 0.0), new Vector2D(2.0, 2.0));
    Line2D second = new Line2D(new Vector2D(0.0, 2.0), new Vector2D(2.0, 0.0));

    Line2D.LineCrossType firstType = Line2D.Intersect(first, second, out Vector2D? firstPoint);
    Line2D.LineCrossType secondType = Line2D.Intersect(second, first, out Vector2D? secondPoint);

    Assert.Multiple(() => {
      Assert.That(firstType, Is.EqualTo(secondType));
      Assert.That(firstPoint, Is.Not.Null);
      Assert.That(secondPoint, Is.Not.Null);
      Line2DAssert.AreEqual(firstPoint!, secondPoint!);
    });
  }

  [Test]
  public void Intersect_UsesEpsilonForNearParallelAndNearCoincidentCases() {
    Line2D baseLine = new Line2D(new Vector2D(0.0, 0.0), new Vector2D(2.0, 0.0));
    Line2D nearCoincident = new Line2D(new Vector2D(0.0, Tools.Eps / 2.0), new Vector2D(2.0, Tools.Eps / 2.0));
    Line2D clearlyParallel = new Line2D(new Vector2D(0.0, 2.0 * Tools.Eps), new Vector2D(2.0, 2.0 * Tools.Eps));

    Line2D.LineCrossType overlapType = Line2D.Intersect(baseLine, nearCoincident, out Vector2D? overlapPoint);
    Line2D.LineCrossType parallelType = Line2D.Intersect(baseLine, clearlyParallel, out Vector2D? parallelPoint);

    Assert.Multiple(() => {
      Assert.That(overlapType, Is.EqualTo(Line2D.LineCrossType.Overlap), "Offset below epsilon is treated as overlap.");
      Assert.That(overlapPoint, Is.Null);
      Assert.That(parallelType, Is.EqualTo(Line2D.LineCrossType.Parallel), "Offset above epsilon is treated as parallel.");
      Assert.That(parallelPoint, Is.Null);
    });
  }

}
