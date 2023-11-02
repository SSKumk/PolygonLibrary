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

    // FaceLattice trl = GiftWrapping.WrapFaceLattice(new Point[] { p1, p2, p3 });

    FaceLattice P = GiftWrapping.WrapFaceLattice(Cube5D_list);

    FaceLatticeNode top = P.Maximum;
    FaceLatticeNode bot = top;
    while (bot.Sub is not null) {
      bot = bot.Sub.First();
    }
    FaceLatticeNode? top2 = bot;
    while (top2.Super is not null) {
      top2 = top2.Super.Last();
    }
    Assert.IsTrue(ReferenceEquals(top, top2));

    // Assert.IsTrue(false);




    // В плоскости p01-p02-p03
    // SubPoint s1 = new SubPoint(p1, s01, p01);
    // SubPoint s2 = new SubPoint(p2, s02, p02);
    // SubPoint s3 = new SubPoint(p3, s03, p03);

    // // В плоскости p01-p02-p04
    // SubPoint u1 = new SubPoint(p1, s01, p01);
    // SubPoint u2 = new SubPoint(p2, s02, p02);
    // SubPoint u4 = new SubPoint(p3, s04, p04);

    /*
     * Идея с ZeroDimensional оказалась неуспешной. Так как
     * public override HashSet<SubZeroDimensional> Vertices => _vertices ??= new HashSet<SubZeroDimensional> { new SubZeroDimensional(Vertex, Primal) };
     * Невозможно получить вершину у такого многогранника -- происходит рекурсивное построение самого себя, что выливается в stack overflow.
     *
     * Но кажется, что всё можно решить в рамках SubPoint, если при построении 2D-объектов использовать не текущий SubPoint, а
     * оригинальный (Original), тогда одинаковые рёбра многоугольников, построенных на разных проекциях из исходного роя
     * будут содержать ссылки на одинаковые точки из исходного роя.
     */
    //
    //

    // // Овыпукляем p01-p02-p03
    // var ch_top_help = Convexification.GrahamHull(new List<SubPoint>() { s1, s2, s3 }.Select(s => new SubPoint2D(s)));
    // var ch_top      = ch_top_help.Select(v => ((SubPoint2D)v).SubPoint.Original).ToList();
    //
    // SubTwoDimensional P1 = new SubTwoDimensional(ch_top);
    //
    // // Овыпукляем p01-p02-p04
    // var ch_side_help = Convexification.GrahamHull(new List<SubPoint>() { u1, u2, u4 }.Select(s => new SubPoint2D(s)));
    // var ch_side      = ch_top_help.Select(v => ((SubPoint2D)v).SubPoint.Original).ToList();
    //
    // SubTwoDimensional P2 = new SubTwoDimensional(ch_side);
  }

}

