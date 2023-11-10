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
    // FaceLattice x = fl_p1p2.ProjectTo(aBasis);
    // FaceLattice y = x.TranslateToOriginal(aBasis);

    // Проекции "туда" и "сюда" вроде работают
    //! А вот и нет ! У точки Super-ы пропали!

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
    var p2 = new Point(new double[] { 1, 0, 0 });
    var p3 = new Point(new double[] { 0, 1, 0 });
    var p4 = new Point(new double[] { 0, 0, 1 });
    var p5 = new Point(new double[] { 1, 0, 1 });

    var t1 = new Point(new double[] { 0.5, 0, 0 });
    var t2 = new Point(new double[] { 0.6, 0.3, 0 });
    var t3 = new Point(new double[] { 0, 0.8, 0 });

    List<Point> sqXY = new List<Point>() { p0, p2, p1, p3 };
    List<Point> sqXZ = new List<Point>() { p0, p2, p4, p5 };
    List<Point> trigXY = new List<Point>() { t1, t2, t3 };

    FaceLattice v0 = new FaceLattice(p0);
    FaceLattice v1 = new FaceLattice(p1);
    FaceLattice v4 = new FaceLattice(p4);

    FaceLattice s1 = GiftWrapping.WrapFaceLattice(new List<Point>() { p0, p1 });
    FaceLattice s2 = GiftWrapping.WrapFaceLattice(new List<Point>() { p0, p2 });
    FaceLattice s3 = GiftWrapping.WrapFaceLattice(new List<Point>() { p0, p3 });

    FaceLattice s5 = GiftWrapping.WrapFaceLattice(new List<Point>() { p0, p5 });


    FaceLattice q1 = GiftWrapping.WrapFaceLattice(sqXY);
    FaceLattice trig1 = GiftWrapping.WrapFaceLattice(trigXY);


    // Начинаем с малых размерностей тестировать:
    // 0:
    // Тут надо глазами смотреть, так как GW не умеет работать с роем из одной точки.
    // FaceLattice VertexSolo = MinkSumSDas(v1, v1); // Ok
    // FaceLattice Vertex = MinkSumSDas(v4, v1); // Ok

    // FaceLattice v0s1 = MinkSumSDas(v0, s1); // Ок
    // Assert.That(v0s1, Is.EqualTo(MinkSumCH(v0, s1)));

    // FaceLattice v0q1 = MinkSumSDas(v0, q1);  // Ок
    // Assert.That(v0q1, Is.EqualTo(MinkSumCH(v0, q1)));

    // // 1:
    // FaceLattice s1s1 = MinkSumSDas(s1, s1);  // Ок
    // Assert.That(s1s1, Is.EqualTo(MinkSumCH(s1, s1)));
    // FaceLattice s2s3 = MinkSumSDas(s2, s3);  // Ок
    // Assert.That(s2s3, Is.EqualTo(MinkSumCH(s2, s3)));
    // FaceLattice s2q1 = MinkSumSDas(s2, q1);  // ! error (ProjectTo неверный)
    // Assert.That(s2q1, Is.EqualTo(MinkSumCH(s2, q1)));

    // 2: //! error (ProjectTo неверный)
    // FaceLattice q1q1 = MinkSumSDas(q1, q1); 
    // Assert.That(q1q1, Is.EqualTo(MinkSumCH(q1, q1)));
    // FaceLattice q1trig1 = MinkSumSDas(q1, trig1); //
    // Assert.That(q1trig1, Is.EqualTo(MinkSumCH(q1, trig1)));

    // // 3:
    // FaceLattice q1s5 = MinkSumSDas(q1, s5);
    // Assert.That(q1s5, Is.EqualTo(MinkSumCH(q1, s5)));   // Ок

    // // High-dim
    FaceLattice cube5dCH = GiftWrapping.WrapFaceLattice(Cube5D_list);
    // FaceLattice Cube5D = MinkSumSDas(cube5dCH, new FaceLattice(new Point(new double[] { 0, 0, 0, 0, 0 })));
    // Assert.That(Cube5D, Is.EqualTo(cube5dCH));

    FaceLattice Cube5D_2 = MinkSumSDas(cube5dCH, cube5dCH);
    Assert.That(Cube5D_2, Is.EqualTo(MinkSumCH(cube5dCH, cube5dCH)));


  }


  [Test]
  public void MinkowskiSDas2D() {
    var u0 = new Point(new double[] { 0, 0 });
    var u1 = new Point(new double[] { 1, 0 });
    var u2 = new Point(new double[] { 0, 1 });
    var u3 = new Point(new double[] { 1, 1 });

    FaceLattice su1 = GiftWrapping.WrapFaceLattice(new List<Point> { u0, u1 });
    FaceLattice su2 = GiftWrapping.WrapFaceLattice(new List<Point> { u0, u2 });
    FaceLattice su3 = GiftWrapping.WrapFaceLattice(new List<Point> { u0, u3 });

    // Единичный квадрат
    FaceLattice qu1_GW = GiftWrapping.WrapFaceLattice(new List<Point> { u0, u1, u2, u3 });
    FaceLattice qu1 = MinkSumSDas(su1, su2);
    Assert.That(qu1, Is.EqualTo(qu1_GW));

    // Прямоугольник
    FaceLattice su1_qu1 = MinkSumSDas(su1, qu1);
    Assert.That(su1_qu1, Is.EqualTo(MinkSumCH(su1, qu1)));

    // Шестиугольник
    FaceLattice su3_qu1 = MinkSumSDas(su3, qu1);
    Assert.That(su3_qu1, Is.EqualTo(MinkSumCH(su3, qu1)));

    // Квадрат в два раза больший
    FaceLattice double_qu1 = MinkSumSDas(qu1, qu1);
    Assert.That(double_qu1, Is.EqualTo(MinkSumCH(qu1_GW, qu1_GW)));

    // ВАУ, что-то работает!
  }
}


