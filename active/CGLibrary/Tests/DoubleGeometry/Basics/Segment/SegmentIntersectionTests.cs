using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;

namespace Tests.DoubleGeometry.Basics;

[TestFixture]
public class SegmentIntersectionTests {

  [Test]
  public void Intersect_CrossingAtInnerPoint_ReturnsSinglePointAndInnerTypes() {
    Segment s1 = new Segment(new Vector2D(0.0, 0.0), new Vector2D(4.0, 0.0));
    Segment s2 = new Segment(new Vector2D(2.0, -1.0), new Vector2D(2.0, 1.0));

    CrossInfo info = Segment.Intersect(s1, s2);

    Assert.Multiple(() => {
      Assert.That(info.crossType, Is.EqualTo(CrossType.SinglePoint));
      Assert.That(info.sp, Is.Null);
      SegmentAssert.AreEqual(info.fp!, new Vector2D(2.0, 0.0));
      Assert.That(info.fTypeS1, Is.EqualTo(IntersectPointPos.Inner));
      Assert.That(info.fTypeS2, Is.EqualTo(IntersectPointPos.Inner));
      Assert.That(info.s1, Is.SameAs(s1));
      Assert.That(info.s2, Is.SameAs(s2));
    });
  }

  [Test]
  public void Intersect_EndpointTouch_CorrectlyMarksPointPositions() {
    Segment s1 = new Segment(new Vector2D(0.0, 0.0), new Vector2D(4.0, 0.0));
    Segment s2 = new Segment(new Vector2D(4.0, 0.0), new Vector2D(4.0, 3.0));

    CrossInfo info = Segment.Intersect(s1, s2);

    Assert.Multiple(() => {
      Assert.That(info.crossType, Is.EqualTo(CrossType.SinglePoint));
      Assert.That(info.sp, Is.Null);
      SegmentAssert.AreEqual(info.fp!, new Vector2D(4.0, 0.0));
      Assert.That(info.fTypeS1, Is.EqualTo(IntersectPointPos.End));
      Assert.That(info.fTypeS2, Is.EqualTo(IntersectPointPos.Begin));
    });
  }

  [Test]
  public void Intersect_NonParallelButSeparatedSegments_ReturnNoCross() {
    Segment s1 = new Segment(new Vector2D(0.0, 0.0), new Vector2D(1.0, 0.0));
    Segment s2 = new Segment(new Vector2D(2.0, -1.0), new Vector2D(2.0, 1.0));

    CrossInfo info = Segment.Intersect(s1, s2);

    Assert.Multiple(() => {
      Assert.That(info.crossType, Is.EqualTo(CrossType.NoCross));
      Assert.That(info.fp, Is.Null);
      Assert.That(info.sp, Is.Null);
    });
  }

  [Test]
  public void Intersect_CollinearOverlap_ReturnsOverlapAndEndpoints() {
    Segment s1 = new Segment(new Vector2D(0.0, 0.0), new Vector2D(4.0, 0.0));
    Segment s2 = new Segment(new Vector2D(2.0, 0.0), new Vector2D(6.0, 0.0));

    CrossInfo info = Segment.Intersect(s1, s2);

    Assert.Multiple(() => {
      Assert.That(info.crossType, Is.EqualTo(CrossType.Overlap));
      SegmentAssert.HasPointSet(info, new Vector2D(2.0, 0.0), new Vector2D(4.0, 0.0));
      Assert.That(info.fTypeS1, Is.EqualTo(IntersectPointPos.Inner));
      Assert.That(info.sTypeS1, Is.EqualTo(IntersectPointPos.End));
      Assert.That(info.fTypeS2, Is.EqualTo(IntersectPointPos.Begin));
      Assert.That(info.sTypeS2, Is.EqualTo(IntersectPointPos.Inner));
    });
  }

  [Test]
  public void Intersect_CollinearTouchByEndpoint_ReturnsSinglePoint() {
    Segment s1 = new Segment(new Vector2D(0.0, 0.0), new Vector2D(4.0, 0.0));
    Segment s2 = new Segment(new Vector2D(4.0, 0.0), new Vector2D(6.0, 0.0));

    CrossInfo info = Segment.Intersect(s1, s2);

    Assert.Multiple(() => {
      Assert.That(info.crossType, Is.EqualTo(CrossType.SinglePoint));
      Assert.That(info.sp, Is.Null);
      SegmentAssert.AreEqual(info.fp!, new Vector2D(4.0, 0.0));
      Assert.That(info.fTypeS1, Is.EqualTo(IntersectPointPos.End));
      Assert.That(info.fTypeS2, Is.EqualTo(IntersectPointPos.Begin));
    });
  }

  [Test]
  public void Intersect_CollinearDisjointSegments_ReturnNoCross() {
    Segment s1 = new Segment(new Vector2D(0.0, 0.0), new Vector2D(4.0, 0.0));
    Segment s2 = new Segment(new Vector2D(5.0, 0.0), new Vector2D(6.0, 0.0));

    CrossInfo info = Segment.Intersect(s1, s2);

    Assert.Multiple(() => {
      Assert.That(info.crossType, Is.EqualTo(CrossType.NoCross));
      Assert.That(info.fp, Is.Null);
      Assert.That(info.sp, Is.Null);
    });
  }

  [Test]
  public void Intersect_IsGeometricallySymmetricByArguments() {
    Segment s1 = new Segment(new Vector2D(0.0, 0.0), new Vector2D(4.0, 0.0));
    Segment s2 = new Segment(new Vector2D(2.0, -1.0), new Vector2D(2.0, 1.0));

    CrossInfo info12 = Segment.Intersect(s1, s2);
    CrossInfo info21 = Segment.Intersect(s2, s1);

    Assert.Multiple(() => {
      Assert.That(info12.crossType, Is.EqualTo(info21.crossType));
      SegmentAssert.AreEqual(info12.fp!, info21.fp!);
    });
  }

  [Test]
  public void Intersect_ReversingEndpointOrder_DoesNotChangeIntersectionGeometry() {
    Segment s1 = new Segment(new Vector2D(0.0, 0.0), new Vector2D(4.0, 0.0));
    Segment s2 = new Segment(new Vector2D(2.0, 0.0), new Vector2D(6.0, 0.0));
    Segment reversed = new Segment(new Vector2D(4.0, 0.0), new Vector2D(0.0, 0.0));

    CrossInfo direct = Segment.Intersect(s1, s2);
    CrossInfo reversedInfo = Segment.Intersect(reversed, s2);

    Assert.Multiple(() => {
      Assert.That(direct.crossType, Is.EqualTo(reversedInfo.crossType));
      SegmentAssert.HasPointSet(direct, new Vector2D(2.0, 0.0), new Vector2D(4.0, 0.0), "Direct orientation");
      SegmentAssert.HasPointSet(reversedInfo, new Vector2D(2.0, 0.0), new Vector2D(4.0, 0.0), "Reversed orientation");
    });
  }

}
