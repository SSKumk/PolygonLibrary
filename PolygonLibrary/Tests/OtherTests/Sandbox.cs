using System.Diagnostics;
using System.Globalization;
using CGLibrary;
using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;
using static Tests.ToolsTests.TestsPolytopes<double, Tests.DConvertor>;
using static Tests.ToolsTests.TestsBase<double, Tests.DConvertor>;
using System.IO;
namespace Tests.OtherTests;

[TestFixture]
public class Sandbox {

  [Test]
  public void LowerGW() {
    HashSet<Point> S = new HashSet<Point>()
      {
        new Point(new double[] { 0, 0, 0, 0, 0})
      , new Point(new double[] { 1, 0, 0, 0, 0})
      , new Point(new double[] { 0, 0, 1, 0, 0})
      , new Point(new double[] { 0, 0, 0, 1, 0})
      , new Point(new double[] { 1, 0, 1, 0, 0})
      , new Point(new double[] { 0, 0, 1, 1, 0})
      , new Point(new double[] { 1, 0, 0, 1, 0})
      , new Point(new double[] { 1, 0, 1, 1, 0})
      };

    GiftWrapping P = new GiftWrapping(S);

  }
  [Test]
  public void ConvexPolytopTest() {
    // Рой
    var p01 = new Point(new double[] { 0, 0, 0 });
    var p02 = new Point(new double[] { 1, 0, 0 });
    var p03 = new Point(new double[] { 0, 1, 0 });
    var p04 = new Point(new double[] { 0, 0, 1 });


    // Изначальные SubPoint-ы
    var s01 = new SubPoint(p01, null, p01);
    var s02 = new SubPoint(p02, null, p02);
    var s03 = new SubPoint(p03, null, p03);
    var s04 = new SubPoint(p04, null, p04);

    // Точки в подпространстве
    var p1 = new Point(new Point2D(0, 0));
    var p2 = new Point(new Point2D(1, 0));
    var p3 = new Point(new Point2D(0, 1));

    FaceLattice P = GiftWrapping.WrapFaceLattice(new Point[] { p1, p2, p3 });

    // FaceLattice P = GiftWrapping.WrapFaceLattice(Cube5D_list);

    FLNode top = P.Top;
    FLNode bot = top;
    while (bot.Sub is not null) {
      bot = bot.Sub.First();
    }
    FLNode? top2 = bot;
    while (top2.Super is not null) {
      top2 = top2.Super.Last();
    }
    Assert.IsTrue(ReferenceEquals(top, top2));
  }


  [Test]
  public void MinkSumTest() {
    // Тестовый пример, собранный руками:
    // квадрат _+_ отрезок == куб
    // квадрат в xOy, отрезок в Oz
    var p0 = new Point(new double[] { 0, 0, 0 });
    var p1 = new Point(new double[] { 1, 1, 0 });
    var p2 = new Point(new double[] { 1, 0, 0 });
    var p3 = new Point(new double[] { 0, 1, 0 });

    var p4 = new Point(new double[] { 0, 0, 1 });

    FLNode v0 = new FLNode(0, new HashSet<Point>() { p0 }, p0, new AffineBasis(p0));
    FLNode v1 = new FLNode(0, new HashSet<Point>() { p1 }, p1, new AffineBasis(p1));
    FLNode v2 = new FLNode(0, new HashSet<Point>() { p2 }, p2, new AffineBasis(p2));
    FLNode v3 = new FLNode(0, new HashSet<Point>() { p3 }, p3, new AffineBasis(p3));
    FLNode v4 = new FLNode(0, new HashSet<Point>() { p4 }, p4, new AffineBasis(p4));

    FLNode s1 = new FLNode(1, new HashSet<Point>() { p0, p2 }, new HashSet<FLNode>() { v0, v2 });
    FLNode s2 = new FLNode(1, new HashSet<Point>() { p0, p3 }, new HashSet<FLNode>() { v0, v3 });
    FLNode s3 = new FLNode(1, new HashSet<Point>() { p1, p2 }, new HashSet<FLNode>() { v1, v2 });
    FLNode s4 = new FLNode(1, new HashSet<Point>() { p1, p3 }, new HashSet<FLNode>() { v1, v3 });

    FLNode s5 = new FLNode(1, new HashSet<Point>() { p0, p4 }, new HashSet<FLNode>() { v0, v4 });

    FLNode q1 = new FLNode(2, new HashSet<Point>() { p0, p1, p2, p3 }, new HashSet<FLNode>() { s1, s2, s3, s4 });

    FaceLattice seg1 = new FaceLattice(s1.Vertices, s1);
    FaceLattice seg2 = new FaceLattice(s2.Vertices, s2);
    FaceLattice seg5 = new FaceLattice(s5.Vertices, s5);

    FaceLattice squ = new FaceLattice(q1.Vertices, q1);

    // FaceLattice Cube = MinkowskiSDas(squ, seg5);

    FaceLattice Square = MinkowskiSDas(seg1, seg2);

  }

}

