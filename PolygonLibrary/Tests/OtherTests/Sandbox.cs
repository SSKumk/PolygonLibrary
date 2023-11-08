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
  public void MinkSumTest() {

    // квадрат в xOy, отрезок в Oz
    var p0 = new Point(new double[] { 0, 0, 0 });
    var p1 = new Point(new double[] { 1, 1, 0 });
    var p1_ = new Point(new double[] { 2, 2, 0 });
    var p2 = new Point(new double[] { 1, 0, 0 });
    var p3 = new Point(new double[] { 0, 1, 0 });
    var p4 = new Point(new double[] { 0, 0, 1 });
    var p5 = new Point(new double[] { 1, 0, 1 });

    List<Point> sqXY = new List<Point>() { p0, p2, p1, p3 };
    List<Point> sqXZ = new List<Point>() { p0, p2, p4, p5 };

    FaceLattice v0 = new FaceLattice(p0);
    FaceLattice v1 = new FaceLattice(p1);
    FaceLattice v4 = new FaceLattice(p4);

    FaceLattice s1 = GiftWrapping.WrapFaceLattice(new List<Point>() { p0, p1 });
    FaceLattice s1_ = GiftWrapping.WrapFaceLattice(new List<Point>() { p0, p1_ });

    FaceLattice s2 = GiftWrapping.WrapFaceLattice(new List<Point>() { p0, p2 });
    FaceLattice s3 = GiftWrapping.WrapFaceLattice(new List<Point>() { p0, p3 });
    FaceLattice s4 = GiftWrapping.WrapFaceLattice(new List<Point>() { p0, p4 });


    FaceLattice q1 = GiftWrapping.WrapFaceLattice(sqXY);

    // Начинаем с малых размерностей тестировать:
    // 0:

    // FaceLattice VertexSolo = MinkowskiSDas(v1, v1); // Ok
    // FaceLattice Vertex = MinkowskiSDas(v4, v1); // Ok
    // FaceLattice Segment = MinkowskiSDas(v0, s1); // Ок
    // FaceLattice Square = MinkowskiSDas(v0, q1);  // Ок

    // 1:
    // FaceLattice SegmentSolo = MinkowskiSDas(s1, s1);  // Ок
    // FaceLattice Segment = MinkowskiSDas(s1, s1_); // Ок
    // FaceLattice Square = MinkowskiSDas(s2, s3);  // Ок
    // FaceLattice Cube = MinkowskiSDas(s2, q1);  // ! error

    //2D-case //! MinkowskiSDas(qu1, vu1); !!
    {
      // var u0 = new Point(new double[] { 0, 0 });
      // var u1 = new Point(new double[] { 1, 0 });
      // var u2 = new Point(new double[] { 0, 1 });

      // FaceLattice vu1 = GiftWrapping.WrapFaceLattice(new List<Point> { u0, u1 });
      // FaceLattice vu2 = GiftWrapping.WrapFaceLattice(new List<Point> { u0, u2 });

      // FaceLattice qu1 = MinkowskiSDas(vu1, vu2);

      // FaceLattice double_qu1 = MinkowskiSDas(qu1, vu1);

    }


    // 2:
    // FaceLattice Square = MinkowskiSDas(q1, q1); //! Какая-то хрень получается

    // 3:
    // FaceLattice Cube = MinkowskiSDas(MinkowskiSDas(q1, s5).Top, MinkowskiSDas(q1, s5).Top); //! error

    // FaceLattice Cube = MinkowskiSDas(q1, s5);

    // FaceLattice Square = MinkowskiSDas(s1, s2);

    // FaceLattice cube5d = GiftWrapping.WrapFaceLattice(Cube5D_list);
    // FaceLattice Cube5D = MinkowskiSDas(cube5d, cube5d);

  }

}

