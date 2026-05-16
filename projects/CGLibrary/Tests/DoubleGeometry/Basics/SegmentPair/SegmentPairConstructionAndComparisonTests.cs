using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;

namespace Tests.DoubleGeometry.Basics;

[TestFixture]
public class SegmentPairConstructionAndComparisonTests {

  private static Segment SegmentAt(double x1, double y1, double x2, double y2) => new(new Vector2D(x1, y1), new Vector2D(x2, y2));

  [Test]
  public void Constructor_SortsSegmentsByCompareTo() {
    Segment greater = SegmentAt(5, 0, 6, 0);
    Segment lesser = SegmentAt(1, 0, 2, 0);

    SegmentPair pair = new SegmentPair(greater, lesser);

    Assert.Multiple(() => {
      Assert.That(pair.s1, Is.EqualTo(lesser));
      Assert.That(pair.s2, Is.EqualTo(greater));
    });
  }

  [Test]
  public void Constructor_ReturnsSameNormalizedPairForReversedArguments() {
    Segment first = SegmentAt(1, 0, 2, 0);
    Segment second = SegmentAt(5, 0, 6, 0);

    SegmentPair direct = new SegmentPair(first, second);
    SegmentPair reversed = new SegmentPair(second, first);

    Assert.Multiple(() => {
      Assert.That(direct.s1, Is.EqualTo(reversed.s1));
      Assert.That(direct.s2, Is.EqualTo(reversed.s2));
      Assert.That(direct.CompareTo(reversed), Is.EqualTo(0));
    });
  }

  [Test]
  public void CompareTo_UsesFirstSegmentBeforeSecondSegment() {
    Segment a = SegmentAt(0, 0, 1, 0);
    Segment b = SegmentAt(2, 0, 3, 0);
    Segment c = SegmentAt(4, 0, 5, 0);
    Segment d = SegmentAt(6, 0, 7, 0);

    SegmentPair byFirstLeft = new SegmentPair(a, d);
    SegmentPair byFirstRight = new SegmentPair(b, c);
    SegmentPair sameFirstLowerSecond = new SegmentPair(a, c);
    SegmentPair sameFirstGreaterSecond = new SegmentPair(a, d);

    Assert.Multiple(() => {
      Assert.That(byFirstLeft.CompareTo(byFirstRight), Is.LessThan(0));
      Assert.That(byFirstRight.CompareTo(byFirstLeft), Is.GreaterThan(0));
      Assert.That(sameFirstLowerSecond.CompareTo(sameFirstGreaterSecond), Is.LessThan(0));
      Assert.That(sameFirstGreaterSecond.CompareTo(sameFirstLowerSecond), Is.GreaterThan(0));
    });
  }

  [Test]
  public void CompareTo_TreatsPairsWithSameSegmentsAsEqual() {
    Segment first = SegmentAt(1, 1, 2, 2);
    Segment second = SegmentAt(3, 3, 4, 4);

    SegmentPair left = new SegmentPair(first, second);
    SegmentPair right = new SegmentPair(second, first);

    Assert.That(left.CompareTo(right), Is.EqualTo(0));
  }

  [Test]
  public void SortedSet_DoesNotStoreDuplicatePairsWithReversedArguments() {
    Segment first = SegmentAt(1, 1, 2, 2);
    Segment second = SegmentAt(3, 3, 4, 4);

    SortedSet<SegmentPair> set = new SortedSet<SegmentPair> {
      new SegmentPair(first, second),
      new SegmentPair(second, first)
    };

    Assert.That(set.Count, Is.EqualTo(1));
  }

}
