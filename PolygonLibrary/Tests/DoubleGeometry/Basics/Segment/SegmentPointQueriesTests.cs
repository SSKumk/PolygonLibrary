using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;

namespace Tests.DoubleGeometry.Basics;

[TestFixture]
public class SegmentPointQueriesTests {

  [Test]
  public void SegmentContainsPointTest() {
    double a = 0.531;
    Vector2D[] p = new Vector2D[]
      { /* 0 */ new Vector2D(1, 2)
      , /* 1 */ new Vector2D(5, 4)
      , /* 2 */ new Vector2D(3, 3)
      , /* 3 */ new Vector2D(1 + 2 * a, 2 + a)
      , /* 4 */ new Vector2D(1 + 2 * a, 2 + a + 1e-8)
      , /* 5 */ new Vector2D(1 + 2 * a, 2 + a + 1e-4)
      , /* 6 */ new Vector2D(-1, 1)
      , /* 7 */ new Vector2D(7, 5)
      , /* 8 */ new Vector2D(5, 5)
      , /* 9 */ new Vector2D(-2, 5)
      };
    bool[] res = new bool[] { true, true, true, true, true, false, false, false, false, false };
    Segment s = new Segment(p[0], p[1]);

    for (int i = 0; i < 10; i++) {
      Assert.That(res[i], Is.EqualTo(s.ContainsPoint(p[i])), "The ContainsPoint test #" + i + " failed");
    }
  }

  [Test]
  public void IsEndPointAndIsInnerPoint_DistinguishEndpointsInnerAndOuterPoints() {
    Segment s = new Segment(new Vector2D(1.0, 2.0), new Vector2D(5.0, 4.0));
    Vector2D inner = new Vector2D(3.0, 3.0);
    Vector2D outer = new Vector2D(7.0, 5.0);

    Assert.Multiple(() => {
      Assert.That(s.IsEndPoint(s[0]), Is.True);
      Assert.That(s.IsEndPoint(s[1]), Is.True);
      Assert.That(s.IsEndPoint(inner), Is.False);
      Assert.That(s.IsEndPoint(outer), Is.False);
      Assert.That(s.IsInnerPoint(inner), Is.True);
      Assert.That(s.IsInnerPoint(s[0]), Is.False);
      Assert.That(s.IsInnerPoint(s[1]), Is.False);
      Assert.That(s.IsInnerPoint(outer), Is.False);
    });
  }

  [Test]
  public void ComputeAtPoint_WorksForNonVerticalSegmentAndMatchesEndpoints() {
    Segment s = new Segment(new Vector2D(1.0, 2.0), new Vector2D(5.0, 4.0));

    Assert.Multiple(() => {
      Assert.That(Tools.EQ(s.ComputeAtPoint(1.0), 2.0), Is.True);
      Assert.That(Tools.EQ(s.ComputeAtPoint(5.0), 4.0), Is.True);
      Assert.That(Tools.EQ(s.ComputeAtPoint(3.0), 3.0), Is.True);
    });
  }

}
