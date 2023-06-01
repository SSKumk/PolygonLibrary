using System.Diagnostics;
using NUnit.Framework;
using PolygonLibrary.Basics;
using PolygonLibrary.Polyhedra.ConvexPolyhedra.GiftWrapping;
using PolygonLibrary.Toolkit;


namespace Tests.GW_hDTests;

[TestFixture]
public class SubClassesTests {

  [Test]
  public void Test1() {
    List<Point> Ps = new List<Point>()
      {
        new Point(new double[] { 1, 2 })
      , new Point(new double[] { 3, 4 })
      , new Point(new double[] { 7, 4 })
      , new Point(new double[] { 2, 3 })
      , new Point(new double[] { 11.0/3, 10.0/3 })
      };

    IEnumerable<SubPoint> S                  = Ps.Select(s => new SubPoint(s, new SubPoint(s, null, s), s));
    List<Point2D>         convexPolygon2D    = Convexification.ArcHull2D(S.Select(s => new SubPoint2D(s)));
    IEnumerable<SubPoint> subConvexPolygon2D = convexPolygon2D.Select(v => ((SubPoint2D)v).SubPoint);
  }

}
