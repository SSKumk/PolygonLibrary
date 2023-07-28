using NUnit.Framework;
using PolygonLibrary.Basics;
using PolygonLibrary.Segments;

namespace Tests;

[TestFixture]
public class SegmentCrossTests {
  [Test]
  public void SegmentContainsPointTest() {
    double a = 0.531;
    Point2D[] p = new Point2D[]
      { /* 0 */ new Point2D(1, 2), /* 1 */ new Point2D(5, 4), /* 2 */ new Point2D(3, 3)
      , /* 3 */ new Point2D(1 + 2 * a, 2 + a), /* 4 */ new Point2D(1 + 2 * a, 2 + a + 1e-8)
      , /* 5 */ new Point2D(1 + 2 * a, 2 + a + 1e-4), /* 6 */ new Point2D(-1, 1), /* 7 */ new Point2D(7, 5)
      , /* 8 */ new Point2D(5, 5), /* 9 */ new Point2D(-2, 5)
      };
    bool[] res = new bool[]
      {
        true, true, true, true, true
      , false, false, false, false, false
      };
    Segment s = new Segment(p[0], p[1]);

    for (int i = 0; i < 10; i++) {
      Assert.That(res[i], Is.EqualTo(s.ContainsPoint(p[i])), "The ContainsPoint test #" + i + " failed");
    }
  }

  private readonly Vector2D v1 = new Vector2D(1, 0)
  , v2 = new Vector2D(3, 0);

  private readonly Point2D
// Main segment points    
    a = new Point2D(2, 3)
  , b = new Point2D(5, 3);

  // List of lists points for different groups of segments
  List<List<Point2D>> groups =
    {
      // The 1st vertical group
      { new Point2D(2, 6), new Point2D(2, 4), a, new Point2D(2, 2), new Point2D(2, 0) }, 
      groups[0].Select(p => p+v1),
      
    };

  /// <summary>
  /// Horizontal segment
  /// </summary>
  [Test]
  public void SegementCrossTest1() { }
}