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
  public void FaceLatticeFromGWTest() {
    Point p0 = new Point(new double[] { 0, 0, 0, 0 });
    Point p1 = new Point(new double[] { 1, 0, 0, 0 });
    Point p2 = new Point(new double[] { 0, 1, 0, 0 });
    Point p3 = new Point(new double[] { 0, 0, 1, 0 });
    Point p4 = new Point(new double[] { 1, 1, 0, 0 });
    Point p5 = new Point(new double[] { 0, 1, 1, 0 });
    Point p6 = new Point(new double[] { 1, 0, 1, 0 });
    Point p7 = new Point(new double[] { 1, 1, 1, 0 });

    // 0 dim
    // FaceLattice fl_p0 = GiftWrapping.WrapFaceLattice(new List<Point>() { p0 }); //? правильно ли, что GW не умеет работать с точкой?

    // 1 dim
    FaceLattice fl_p1p2 = GiftWrapping.WrapFaceLattice(new List<Point>() { p1, p2 });

    // 2 dim
    FaceLattice fl_p3p5p6p7 = GiftWrapping.WrapFaceLattice(new List<Point>() { p3, p5, p6, p7 });

    // 3 dim
    FaceLattice fl_all = GiftWrapping.WrapFaceLattice(new List<Point>() { p0, p1, p2, p3, p4, p5, p6, p7 });

    // Симплекс
    FaceLattice fl_simplex4D = GiftWrapping.WrapFaceLattice(SimplexRND4D_list);
    // Вроде FaceLattice на основе GW работает как надо.


    // Тестирую проекции:
    AffineBasis aBasis2D = new AffineBasis(2, 4);
    AffineBasis aBasis3D = new AffineBasis(3, 4);

    AffineBasis aBasis = new AffineBasis(p7, new List<Vector>() { p1 - p0, p2 - p0 });
    FaceLattice x = fl_p1p2.ProjectTo(aBasis);
    FaceLattice y = x.TranslateToOriginal(aBasis);

    // Проекции "туда" и "сюда" вроде работают

  }

  [Test]
  public void LowerGW() {
    HashSet<Point> S = new HashSet<Point>()
      {
        new Point(new double[] { 0, 0, 0, 0})
      , new Point(new double[] { 1, 0, 0, 0})
      , new Point(new double[] { 0, 1, 0, 0})
      , new Point(new double[] { 0, 0, 1, 0})
      , new Point(new double[] { 1, 1, 0, 0})
      , new Point(new double[] { 0, 1, 1, 0})
      , new Point(new double[] { 1, 0, 1, 0})
      , new Point(new double[] { 1, 1, 1, 0})
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
  public void Some() {
    var p0 = new Point(new double[] { 0 });
    var p1 = new Point(new double[] { 1 });
    var p2 = new Point(new double[] { 1, 1, 0, 0 });

    var S = new List<Point>() { p0, p1 };
    var xx = GiftWrapping.WrapFaceLattice(S);
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
    var p5 = new Point(new double[] { 1, 0, 1 });

    List<Point> sqXY = new List<Point>() { p0, p2, p1, p3 };
    List<Point> sqXZ = new List<Point>() { p0, p2, p4, p5 };

    FaceLattice fl_sqXY = GiftWrapping.WrapFaceLattice(sqXY);
    FaceLattice fl_sqXZ = GiftWrapping.WrapFaceLattice(sqXZ);

    FaceLattice fl_cubeCH = MinkSumCH(fl_sqXY, fl_sqXZ);
    FaceLattice fl_cubeIN = MinkowskiSDas(fl_sqXY, fl_sqXZ);

    // FLNode v0 = new FLNode(0, new HashSet<Point>() { p0 }, p0, new AffineBasis(p0));
    // FLNode v1 = new FLNode(0, new HashSet<Point>() { p1 }, p1, new AffineBasis(p1));
    // FLNode v2 = new FLNode(0, new HashSet<Point>() { p2 }, p2, new AffineBasis(p2));
    // FLNode v3 = new FLNode(0, new HashSet<Point>() { p3 }, p3, new AffineBasis(p3));
    // FLNode v4 = new FLNode(0, new HashSet<Point>() { p4 }, p4, new AffineBasis(p4));
    // FLNode v5 = new FLNode(0, new HashSet<Point>() { p5 }, p5, new AffineBasis(p5));

    // FLNode s1 = new FLNode(1, new HashSet<Point>() { p0, p2 }, new HashSet<FLNode>() { v0, v2 });
    // FLNode s2 = new FLNode(1, new HashSet<Point>() { p0, p3 }, new HashSet<FLNode>() { v0, v3 });
    // FLNode s3 = new FLNode(1, new HashSet<Point>() { p1, p2 }, new HashSet<FLNode>() { v1, v2 });
    // FLNode s4 = new FLNode(1, new HashSet<Point>() { p1, p3 }, new HashSet<FLNode>() { v1, v3 });

    // FLNode s5 = new FLNode(1, new HashSet<Point>() { p0, p4 }, new HashSet<FLNode>() { v0, v4 });
    // FLNode s6 = new FLNode(1, new HashSet<Point>() { p5, p4 }, new HashSet<FLNode>() { v5, v4 });
    // FLNode s7 = new FLNode(1, new HashSet<Point>() { p2, p5 }, new HashSet<FLNode>() { v2, v5 });

    // FLNode q1 = new FLNode(2, new HashSet<Point>() { p0, p1, p2, p3 }, new HashSet<FLNode>() { s1, s2, s3, s4 });
    // FLNode q2 = new FLNode(2, new HashSet<Point>() { p0, p2, p4, p5 }, new HashSet<FLNode>() { s1, s5, s6, s7 });

    // Начинаем с малых размерностей тестировать:
    // 0:

    // FaceLattice Vertex = MinkowskiSDas(v1, v1); // Ok
    // FaceLattice Vertex = MinkowskiSDas(v1, v4); // Ok
    // FaceLattice Segment = MinkowskiSDas(v0, s3);  //? Не Ок пока не умеем сумму не полной размерности!
    // FaceLattice Square = MinkowskiSDas(v0, q1);     //? Не Ок пока не умеем сумму не полной размерности!

    // 1:
    // FaceLattice Segment = MinkowskiSDas(s1, s1); //? Не Ок пока не умеем сумму не полной размерности!
    // FaceLattice Segment = MinkowskiSDas(s1, s4); //? Не Ок пока не умеем сумму не полной размерности!
    // FaceLattice Square = MinkowskiSDas(s1, s3);  //? Не Ок
    // FaceLattice Cube = MinkowskiSDas(s5, q1);  // Ok

    // 2:
    // FaceLattice Square = MinkowskiSDas(q1, q1); //? Не Ок пока не умеем сумму не полной размерности!

    // 3:
    // FaceLattice Cube = MinkowskiSDas(MinkowskiSDas(q1, s5).Top, MinkowskiSDas(q1, s5).Top); //! error

    // FaceLattice Cube = MinkowskiSDas(q1, s5);

    // FaceLattice Square = MinkowskiSDas(s1, s2);

  }

}

