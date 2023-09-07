using NUnit.Framework;
using CGLibrary;
using G = CGLibrary.Geometry<double, DConvertor>;

namespace Tests {
[TestFixtureAttribute]
public class ArcHullTests {

  [Test]
  public void SimpleArcCHTest() {
    RandomLC? r = new RandomLC(10);

    List<G.Point2D> expected = new List<G.Point2D>(), orig = new List<G.Point2D>();

    expected.Add(new G.Point2D(0, 0));
    expected.Add(new G.Point2D(1, 0));
    expected.Add(new G.Point2D(1, 1));
    expected.Add(new G.Point2D(0, 1));

    // Preparing the initial set of points

    // The vertices
    orig.AddRange(expected);
    orig.Add(new G.Point2D(0.5, 0.5));

    // Shuffle the points
    orig.Shuffle(r);

    List<G.Point2D> hull = G.Convexification.ArcHull2D(orig);

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
  public void SquareArcCHTest() {
    RandomLC? r = new RandomLC(10);

    List<G.Point2D> expected = new List<G.Point2D>(), orig = new List<G.Point2D>();

    expected.Add(new G.Point2D(0, 0));
    expected.Add(new G.Point2D(1, 0));
    expected.Add(new G.Point2D(1, 1));
    expected.Add(new G.Point2D(0, 1));

    // Preparing the initial set of points

    // The vertices
    orig.AddRange(expected);

    // Some random internal points
    for (int i = 0; i < 10; i++) {
      orig.Add(new G.Point2D(r.NextDouble(), r.NextDouble()));
    }

    // Some points in the initial cutting line
    orig.Add(new G.Point2D(0.3, 0.3));
    orig.Add(new G.Point2D(0.73, 0.73));

    // Some points in the edges of the convex hull
    orig.Add(new G.Point2D(0, 0.3));
    orig.Add(new G.Point2D(0, 0.8));
    orig.Add(new G.Point2D(1, 0.45));
    orig.Add(new G.Point2D(0.55, 0));
    orig.Add(new G.Point2D(0.65, 1));

    // Copies of the vertices
    orig.Add(new G.Point2D(0, 0));
    orig.Add(new G.Point2D(0, 0));
    orig.Add(new G.Point2D(1, 0));
    orig.Add(new G.Point2D(1, 0));

    // Shuffle the points
    orig.Shuffle(r);

    List<G.Point2D> hull =G.Convexification.ArcHull2D(orig);

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
  public void HexagonArcCHTest() {
    RandomLC? r = new RandomLC(10);

    List<G.Point2D> expected = new List<G.Point2D>(), orig = new List<G.Point2D>();

    expected.Add(new G.Point2D(0, 1));
    expected.Add(new G.Point2D(1, 0));
    expected.Add(new G.Point2D(2, 0));
    expected.Add(new G.Point2D(3, 1));
    expected.Add(new G.Point2D(2, 2));
    expected.Add(new G.Point2D(1, 2));

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
      orig.Add(new G.Point2D(x, y));
    }

    // Some points in the initial cutting line
    orig.Add(new G.Point2D(0.3, 1));
    orig.Add(new G.Point2D(1.73, 1));
    orig.Add(new G.Point2D(2.999, 1));

    // Some points in the edges of the convex hull
    orig.Add(new G.Point2D(1, 1.3));
    orig.Add(new G.Point2D(1, 1.8));
    orig.Add(new G.Point2D(1, 0.45));
    orig.Add(new G.Point2D(2.55, 0.55));
    orig.Add(new G.Point2D(2.65, 1.35));
    orig.Add(new G.Point2D(2.05, 1.95));
    orig.Add(new G.Point2D(1.65, 2));
    orig.Add(new G.Point2D(0.55, 1.55));
    orig.Add(new G.Point2D(0.55, 1.55));
    orig.Add(new G.Point2D(0.55, 0.45));

    // Copies of the vertices
    orig.Add(new G.Point2D(0, 1));
    orig.Add(new G.Point2D(0, 1));
    orig.Add(new G.Point2D(2, 2));
    orig.Add(new G.Point2D(1, 2));

    // Shuffle the points
    orig.Shuffle(r);

    List<G.Point2D> hull =G.Convexification.ArcHull2D(orig);

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

  //   [Test]
  //   public void AntonsTest() { 
  //     List<Point2D> S = new List<Point2D>()
  //       {
  //         new Point2D(0, 0)
  //       , new Point2D(1.2258222706569017, -4.85722573273506E-17)
  //       , new Point2D(1.2293137478570382, 0.3202558730518721)
  //       , new Point2D(1.2293137419301374, 0.3202558833175651)
  //       , new Point2D(1.2293563013567925, 7.979727989493313E-17)
  //       , new Point2D(1.4142135623730951, -7.632783294297951E-17)
  //       , new Point2D(0.7071067811865474, 1.2247448713915892)
  //       };
  //
  //     List<Point2D> res = Convexification.ArcHull2D(S);    }

}
}
