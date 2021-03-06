using NUnit.Framework;

using PolygonLibrary.Basics;
using PolygonLibrary.Toolkit;

namespace Tests
{
  [TestFixtureAttribute]
  public class ArcHullTests
  {
    [Test]
    public void SimpleArcCHTest()
    {
      Random r = new Random(10);

      List<Point2D>
        expected = new List<Point2D>(),
        orig = new List<Point2D>();

      expected.Add(new Point2D(0, 0));
      expected.Add(new Point2D(1, 0));
      expected.Add(new Point2D(1, 1));
      expected.Add(new Point2D(0, 1));

      // Preparing the initial set of points

      // The vertices
      orig.AddRange(expected);
      orig.Add(new Point2D(0.5, 0.5));

      // Shuffle the points
      Tools.Shuffle(orig, r);

      List<Point2D> hull = Convexification.ArcHull2D(orig);

      Assert.Multiple(() => {
        Assert.That(hull, Has.Count.EqualTo(expected.Count), "Wrong number of convex hull vertices");
        for (int i = 0; i < expected.Count; i++) {
          Assert.That(hull[i], Is.EqualTo(expected[i]), "Wrong " + i + "th vertex of the convex hull vertices. " +
                                                        "It is expected " + expected[i] + ", but there is " + hull[i]);
        }
      });
    }

    [Test]
    public void SquareArcCHTest()
    {
      Random r = new Random(10);

      List<Point2D> 
        expected = new List<Point2D>(), 
        orig = new List<Point2D>();

      expected.Add (new Point2D(0,0));
      expected.Add (new Point2D(1,0));
      expected.Add (new Point2D(1,1));
      expected.Add(new Point2D(0, 1));

      // Preparing the initial set of points

      // The vertices
      orig.AddRange (expected);

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
      Tools.Shuffle (orig, r);

      List<Point2D> hull = Convexification.ArcHull2D(orig);

      Assert.That(hull, Has.Count.EqualTo(expected.Count), "Wrong number of convex hull vertices");
      for (int i = 0; i < expected.Count; i++) {
        Assert.That(hull[i], Is.EqualTo(expected[i]), "Wrong " + i + "th vertex of the convex hull vertices. " + 
                                              "It is expected " + expected[i] + ", but there is " + hull[i]);
      }
    }

    [Test]
    public void HexagonArcCHTest()
    {
      Random r = new Random(10);

      List<Point2D>
        expected = new List<Point2D>(),
        orig = new List<Point2D>();

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
      for (int i = 0; i < 10; i++)
      {
        double x, y;
        do
        {
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
      Tools.Shuffle(orig, r);

      List<Point2D> hull = Convexification.ArcHull2D(orig);

      Assert.Multiple(() => {
        Assert.That(hull, Has.Count.EqualTo(expected.Count), "Wrong number of convex hull vertices");
        for (int i = 0; i < expected.Count; i++) {
          Assert.That(hull[i], Is.EqualTo(expected[i]), "Wrong " + i + "th vertex of the convex hull vertices. " +
                                                "It is expected " + expected[i] + ", but there is " + hull[i]);
        }
      });
    }

  }
}
