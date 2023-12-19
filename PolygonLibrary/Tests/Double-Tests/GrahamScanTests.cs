using CGLibrary;
using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;

namespace Tests.Double_Tests; 

[TestFixture]
public class GrahamScanTests {

  [Test]
  public void SimpleScanCHTest() {
    RandomLC? r = new RandomLC(10);

    List<Point2D> expected = new List<Point2D>(), orig = new List<Point2D>();

    expected.Add(new Point2D(0, 0));
    expected.Add(new Point2D(1, 0));
    expected.Add(new Point2D(1, 1));
    expected.Add(new Point2D(0, 1));

    // Preparing the initial set of points

    // The vertices
    orig.AddRange(expected);
    orig.Add(new Point2D(0.5, 0.5));

    // Shuffle the points
    orig.Shuffle(r);

    List<Point2D> hull = Convexification.GrahamHull(orig);

    Assert.Multiple
      (
       () => {
         Assert.That(hull, Has.Count.EqualTo(expected.Count), "Wrong number of convex hull vertices");
         for (int i = 0; i < expected.Count; i++) {
           Assert.That
             (
              hull[i]
            , Is.EqualTo(expected[i])
            , "Wrong " + i + "th vertex of the convex hull vertices. " + "It is expected " + expected[i] + ", but there is " +
              hull[i]
             );
         }
       }
      );
  }

  [Test]
  public void SquareScanCHTest() {
    RandomLC? r = new RandomLC(10);

    List<Point2D> expected = new List<Point2D>(), orig = new List<Point2D>();

    expected.Add(new Point2D(0, 0));
    expected.Add(new Point2D(1, 0));
    expected.Add(new Point2D(1, 1));
    expected.Add(new Point2D(0, 1));

    // Preparing the initial set of points

    // The vertices
    orig.AddRange(expected);

    // Some random internal points
    for (int i = 0; i < 10; i++) {
      orig.Add(new Point2D(r.NextDouble(), r.NextDouble()));
    }

    // Some points in the initial cutting line
    orig.Add(new Point2D(0.3, 0.3));
    orig.Add(new Point2D(0.73, 0.73));

    // Some points in the edges of the convex hull
    orig.Add(new Point2D(0, 0.3));
    orig.Add(new Point2D(0, 0.8));
    orig.Add(new Point2D(1, 0.45));
    orig.Add(new Point2D(0.55, 0));
    orig.Add(new Point2D(0.65, 1));

    // Copies of the vertices
    orig.Add(new Point2D(0, 0));
    orig.Add(new Point2D(0, 0));
    orig.Add(new Point2D(1, 0));
    orig.Add(new Point2D(1, 0));

    // Shuffle the points
    orig.Shuffle(r);

    List<Point2D> hull = Convexification.GrahamHull(orig);

    Assert.That(hull, Has.Count.EqualTo(expected.Count), "Wrong number of convex hull vertices");
    for (int i = 0; i < expected.Count; i++) {
      Assert.That
        (
         hull[i]
       , Is.EqualTo(expected[i])
       , "Wrong " + i + "th vertex of the convex hull vertices. " + "It is expected " + expected[i] + ", but there is " + hull[i]
        );
    }
  }

  [Test]
  public void HexagonScanCHTest() {
    RandomLC? r = new RandomLC(10);

    List<Point2D> expected = new List<Point2D>(), orig = new List<Point2D>();

    expected.Add(new Point2D(0, 1));
    expected.Add(new Point2D(1, 0));
    expected.Add(new Point2D(2, 0));
    expected.Add(new Point2D(3, 1));
    expected.Add(new Point2D(2, 2));
    expected.Add(new Point2D(1, 2));

    // Preparing the initial set of points

    // The vertices
    orig.AddRange(expected);

    // Some random internal points
    for (int i = 0; i < 10; i++) {
      double x, y;
      do {
        x = 3 * r.NextDouble();
        y = 2 * r.NextDouble();
      } while (y > x + 1 || y < -x + 1 || y > -x + 4 || y < x - 2);
      orig.Add(new Point2D(x, y));
    }

    // Some points in the initial cutting line
    orig.Add(new Point2D(0.3, 1));
    orig.Add(new Point2D(1.73, 1));
    orig.Add(new Point2D(2.999, 1));

    // Some points in the edges of the convex hull
    orig.Add(new Point2D(1, 1.3));
    orig.Add(new Point2D(1, 1.8));
    orig.Add(new Point2D(1, 0.45));
    orig.Add(new Point2D(2.55, 0.55));
    orig.Add(new Point2D(2.65, 1.35));
    orig.Add(new Point2D(2.05, 1.95));
    orig.Add(new Point2D(1.65, 2));
    orig.Add(new Point2D(0.55, 1.55));
    orig.Add(new Point2D(0.55, 1.55));
    orig.Add(new Point2D(0.55, 0.45));

    // Copies of the vertices
    orig.Add(new Point2D(0, 1));
    orig.Add(new Point2D(0, 1));
    orig.Add(new Point2D(2, 2));
    orig.Add(new Point2D(1, 2));

    // Shuffle the points
    orig.Shuffle(r);

    List<Point2D> hull = Convexification.GrahamHull(orig);

    Assert.Multiple
      (
       () => {
         Assert.That(hull, Has.Count.EqualTo(expected.Count), "Wrong number of convex hull vertices");
         for (int i = 0; i < expected.Count; i++) {
           Assert.That
             (
              hull[i]
            , Is.EqualTo(expected[i])
            , "Wrong " + i + "th vertex of the convex hull vertices. " + "It is expected " + expected[i] + ", but there is " +
              hull[i]
             );
         }
       }
      );
  }


  // Точки S[2] и S[3] находятся на расстоянии 1.185e-08
  [Test]
  public void ClosePointsTest() {
    List<Point2D> S = new List<Point2D>()
      {
        new Point2D(0, 0)
      , new Point2D(1.2258222706569017, -4.85722573273506E-17)
      , new Point2D(1.2293137478570382, 0.3202558730518721)
      , new Point2D(1.2293137419301374, 0.3202558833175651)
      , new Point2D(1.2293563013567925, 7.979727989493313E-17)
      , new Point2D(1.4142135623730951, -7.632783294297951E-17)
      , new Point2D(0.7071067811865474, 1.2247448713915892)
      };

    Console.WriteLine((S[3] - S[2]).Length);

    List<Point2D> res = Convexification.GrahamHull(S);

    Assert.That(new HashSet<Point2D>(res).SetEquals(Convexification.ArcHull2D(S, false)), "Sets are not equal!");
  }

  // S[0] и S[1] в double почти одинаковы!
  [Test]
  public void ClosePoints2Test() {
    List<Point2D> S = new List<Point2D>()
      {
        new Point2D(1, 1.0000000000000002)
      , new Point2D(1, 1.6653345369377348E-16)
      , new Point2D(0, 0)
      , new Point2D(5.955924281381142E-05, 1)
      , new Point2D(1.6653345369377348E-16, 1)
      };
    // Console.WriteLine((S[3] - S[4]).Length);

    List<Point2D> res = Convexification.GrahamHull(S);

    Assert.That(new HashSet<Point2D>(res).SetEquals(Convexification.ArcHull2D(S, false)), "Sets are not equal!");
  }


  [Test]
  public void TriangleTest() {
    List<Point2D> S = new List<Point2D>()
      {
        new Point2D(0.0, 0.0)
      , new Point2D(-5.551115123125783E-17, 0.9999999999999999)
      , new Point2D(1, 3.885780586188048E-16)
      };

    HashSet<Point2D> additionalPoints = new HashSet<Point2D>(S)
      {
        new Point2D(0.608885066492689, 1.942890293094024E-16)
      , new Point2D(0.489370962020329, 0.5106290379796712)
      , new Point2D(-8.326672684688674E-17, 0.4332029766946106)
      };

    List<Point2D> res = Convexification.GrahamHull(S.Union(additionalPoints));

    Assert.That(new HashSet<Point2D>(res).SetEquals(S), "Sets are not equal!");
  }

}