using CGLibrary;
using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;

namespace Tests.DoubleGeometry.Toolkit;

[TestFixture]
public class ConvexificationTests {

  private static void AssertHullEquals(IReadOnlyList<Vector2D> actual, params Vector2D[] expected) {
    Assert.Multiple(() => {
      Assert.That(actual, Has.Count.EqualTo(expected.Length));
      for (int i = 0; i < expected.Length; i++) {
        Assert.That(actual[i], Is.EqualTo(expected[i]), $"Wrong hull vertex at index {i}.");
      }
    });
  }

  private static IEnumerable<Vector2D> Enumerate(params Vector2D[] points) {
    foreach (Vector2D point in points) {
      yield return point;
    }
  }

  [Test]
  public void QuickHull2D_SquareWithInteriorPointsAndDuplicates_ReturnsExpectedBoundary() {
    RandomLC random = new RandomLC(10u);

    List<Vector2D> points =
      [
        new(0, 0),
        new(1, 0),
        new(1, 1),
        new(0, 1),
        new(0.5, 0.5),
        new(0.3, 0.3),
        new(0.73, 0.73),
        new(0, 0.3),
        new(0, 0.8),
        new(1, 0.45),
        new(0.55, 0),
        new(0.65, 1),
        new(0, 0),
        new(1, 0)
      ];

    points.Shuffle(random);

    List<Vector2D> hull = Convexification.QuickHull2D(points);

    AssertHullEquals(hull, new Vector2D(0, 1), new Vector2D(0, 0), new Vector2D(1, 0), new Vector2D(1, 1));
  }

  [Test]
  public void ArcHull2D_Square_ReturnsExpectedBoundaryInCounterclockwiseOrder() {
    RandomLC random = new RandomLC(10u);

    List<Vector2D> points =
      [
        new(0, 0),
        new(1, 0),
        new(1, 1),
        new(0, 1),
        new(0.5, 0.5)
      ];

    points.Shuffle(random);

    List<Vector2D> hull = Convexification.ArcHull2D(points);

    AssertHullEquals(hull, new Vector2D(0, 0), new Vector2D(1, 0), new Vector2D(1, 1), new Vector2D(0, 1));
  }

  [Test]
  public void ArcHull2D_SortByPrecisionFalse_UsesProcessorComparisonBranch() {
    List<Vector2D> points =
      [
        new(0, 0),
        new(1, 0),
        new(1, 1),
        new(0, 1),
        new(0.5, 0.5),
        new(0, 0),
        new(1, 1)
      ];

    List<Vector2D> hull = Convexification.ArcHull2D(points, false);

    AssertHullEquals(hull, new Vector2D(0, 0), new Vector2D(1, 0), new Vector2D(1, 1), new Vector2D(0, 1));
  }

  [Test]
  public void GrahamHull_MaterializesNonCollectionEnumerable_AndReturnsExpectedTriangle() {
    IEnumerable<Vector2D> enumerable =
      Enumerate
        (
         new Vector2D(0.0, 0.0),
         new Vector2D(-5.551115123125783E-17, 0.9999999999999999),
         new Vector2D(1, 3.885780586188048E-16),
         new Vector2D(0.608885066492689, 1.942890293094024E-16),
         new Vector2D(0.489370962020329, 0.5106290379796712),
         new Vector2D(-8.326672684688674E-17, 0.4332029766946106)
        );

    List<Vector2D> hull = Convexification.GrahamHull(enumerable);

    Assert.That
      (
       new SortedSet<Vector2D>(hull).SetEquals
         (
          [
            new Vector2D(0.0, 0.0),
            new Vector2D(-5.551115123125783E-17, 0.9999999999999999),
            new Vector2D(1, 3.885780586188048E-16)
          ]
         )
     , Is.True
      );
  }

  [Test]
  public void AllAlgorithms_HandleEmptySingleAndTwoPointSets() {
    Vector2D p0 = new Vector2D(0, 0);
    Vector2D p1 = new Vector2D(1, 1);

    Assert.Multiple(() => {
      Assert.That(Convexification.QuickHull2D([]), Is.Empty);
      Assert.That(Convexification.ArcHull2D([]), Is.Empty);
      Assert.That(Convexification.GrahamHull([]), Is.Empty);

      AssertHullEquals(Convexification.QuickHull2D([p0]), p0);
      AssertHullEquals(Convexification.ArcHull2D([p0]), p0);
      AssertHullEquals(Convexification.GrahamHull([p0]), p0);

      AssertHullEquals(Convexification.QuickHull2D([p0, p1]), p0, p1);
      AssertHullEquals(Convexification.ArcHull2D([p0, p1]), p0, p1);
      AssertHullEquals(Convexification.GrahamHull([p0, p1]), p0, p1);
    });
  }

  [Test]
  public void AllAlgorithms_OnCollinearPoints_ReturnOnlyExtremeEndpoints() {
    List<Vector2D> points =
      [
        new(0, 0),
        new(1, 0),
        new(2, 0),
        new(3, 0),
        new(1, 0),
        new(2, 0)
      ];

    Assert.Multiple(() => {
      AssertHullEquals(Convexification.QuickHull2D(points), new Vector2D(0, 0), new Vector2D(3, 0));
      AssertHullEquals(Convexification.ArcHull2D(points), new Vector2D(0, 0), new Vector2D(3, 0));
      AssertHullEquals(Convexification.GrahamHull(points), new Vector2D(0, 0), new Vector2D(3, 0));
    });
  }

}
