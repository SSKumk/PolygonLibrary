using NUnit.Framework;
using PolygonLibrary.Basics;
using PolygonLibrary.Polygons.ConvexPolygons;

namespace Tests;

public partial class ConvexPolygonTests {

  [Category("ConvexPolygonTests"), Test]
  public void Intersection1() {
    List<Point2D> P_List = new List<Point2D>()
      {
        new Point2D(0, 0)
      , new Point2D(2, 0)
      , new Point2D(2, 2)
      , new Point2D(0, 2)
      };
    List<Point2D> Q_List = new List<Point2D>()
      {
        new Point2D(-1, 1.5)
      , new Point2D(0.5, -1)
      , new Point2D(1, 1)
      };
    List<Point2D> answerList = new List<Point2D>()
      {
        new Point2D(0, 1.25)
      , new Point2D(0, 0)
      , new Point2D(0.75, 0)
      , new Point2D(1, 1)
      };
    var P = new ConvexPolygon(P_List);
    var Q = new ConvexPolygon(Q_List);

    var resPQ = ConvexPolygon.IntersectionPolygon(P, Q);
    var resQP = ConvexPolygon.IntersectionPolygon(Q, P);
    CyclicListComparison(answerList, resPQ.Vertices, "Intersection1: P-Q");
    CyclicListComparison(answerList, resQP.Vertices, "Intersection1: Q-P");
  }

  [Category("ConvexPolygonTests"), Test]
  public void Intersection2() {
    List<Point2D> P_List = new List<Point2D>()
      {
        new Point2D(0, 0)
      , new Point2D(2, 0)
      , new Point2D(2, 2)
      , new Point2D(0, 2)
      };
    List<Point2D> Q_List = new List<Point2D>()
      {
        new Point2D(1, 1.5)
      , new Point2D(1.5, 0.5)
      , new Point2D(3, 1)
      };
    List<Point2D> answerList = new List<Point2D>()
      {
        new Point2D(2, 1.25)
      , new Point2D(1, 1.5)
      , new Point2D(1.5, 0.5)
      , new Point2D(2, 0.666666666666666)
      };
    var P = new ConvexPolygon(P_List);
    var Q = new ConvexPolygon(Q_List);

    var resPQ = ConvexPolygon.IntersectionPolygon(P, Q);
    var resQP = ConvexPolygon.IntersectionPolygon(Q, P);
    CyclicListComparison(answerList, resPQ.Vertices, "Intersection2: P-Q");
    CyclicListComparison(answerList, resQP.Vertices, "Intersection2: Q-P");
  }

  [Category("ConvexPolygonTests"), Test]
  public void Intersection3() {
    List<Point2D> P_List = new List<Point2D>()
      {
        new Point2D(0, 0)
      , new Point2D(2, 0)
      , new Point2D(2, 2)
      , new Point2D(0, 2)
      };
    List<Point2D> Q_List = new List<Point2D>()
      {
        new Point2D(1, 2.5)
      , new Point2D(2.5, 0)
      , new Point2D(3, 2)
      };
    List<Point2D> answerList = new List<Point2D>()
      {
        new Point2D(2, 2)
      , new Point2D(1.3, 2)
      , new Point2D(2, 0.8333333333333333)
      };
    var P = new ConvexPolygon(P_List);
    var Q = new ConvexPolygon(Q_List);

    var resPQ = ConvexPolygon.IntersectionPolygon(P, Q);
    var resQP = ConvexPolygon.IntersectionPolygon(Q, P);
    CyclicListComparison(answerList, resPQ.Vertices, "Intersection3: P-Q");
    CyclicListComparison(answerList, resQP.Vertices, "Intersection3: Q-P");
  }
  //
  // [Category("ConvexPolygonTests"), Test]
  // public void Intersection2() {
  //   List<Point2D> P_List = new List<Point2D>()
  //     {
  //       new Point2D(0, 0)
  //     , new Point2D(2, 0)
  //     , new Point2D(2, 2)
  //     , new Point2D(0, 2)
  //     };
  //   List<Point2D> Q_List = new List<Point2D>()
  //     {
  //       new Point2D(1, 2.5)
  //     , new Point2D(2.5, 0)
  //     , new Point2D(3, 2)
  //     };
  //   List<Point2D> answerList = new List<Point2D>()
  //     {
  //       new Point2D(2, 2)
  //     , new Point2D(1.3, 2)
  //     , new Point2D(2, 0.8333333333333333)
  //     };
  //   ConvexPolygon square   = new ConvexPolygon(P_List);
  //   ConvexPolygon triangle = new ConvexPolygon(Q_List);
  //
  //   var res = ConvexPolygon.IntersectionPolygon(square, triangle);
  //   CyclicListComparison(answerList, res.Vertices, "Intersection2");
  // }
  //
  // [Category("ConvexPolygonTests"), Test]
  // public void Intersection3() {
  //   List<Point2D> P_List = new List<Point2D>()
  //     {
  //       new Point2D(0, 0)
  //     , new Point2D(2, 0)
  //     , new Point2D(2, 2)
  //     , new Point2D(0, 2)
  //     };
  //   List<Point2D> Q_List = new List<Point2D>()
  //     {
  //       new Point2D(2, 1.5)
  //     , new Point2D(0, 2.5)
  //     , new Point2D(1, 0)
  //     };
  //   List<Point2D> answerList = new List<Point2D>()
  //     {
  //       new Point2D(1, 0)
  //     , new Point2D(2, 1.5)
  //     , new Point2D(1, 2)
  //     , new Point2D(0.2, 2)
  //     };
  //   ConvexPolygon square   = new ConvexPolygon(P_List);
  //   ConvexPolygon triangle = new ConvexPolygon(Q_List);
  //
  //   var res = ConvexPolygon.IntersectionPolygon(square, triangle);
  //   CyclicListComparison(answerList, res.Vertices, "Intersection3");
  // }
  //
  // [Category("ConvexPolygonTests"), Test]
  // public void Intersection4() {
  //   List<Point2D> P_List = new List<Point2D>()
  //     {
  //       new Point2D(0, 0)
  //     , new Point2D(2, 0)
  //     , new Point2D(2, 2)
  //     , new Point2D(0, 2)
  //     };
  //   List<Point2D> Q_List = new List<Point2D>()
  //     {
  //       new Point2D(-1.5, 1)
  //     , new Point2D(-0.5, -2)
  //     , new Point2D(0, 0)
  //     };
  //
  //   ConvexPolygon square   = new ConvexPolygon(P_List);
  //   ConvexPolygon triangle = new ConvexPolygon(Q_List);
  //
  //   var res = ConvexPolygon.IntersectionPolygon(square, triangle);
  //   Assert.Null(res, "Intersection4");
  // }
  //
  // //todo 1) Поправить Contains на случай только внутренности
  // //todo 2) Понять, что делать с (0,0) вектором
  // [Category("ConvexPolygonTests"), Test]
  // public void Intersection5() {
  //   List<Point2D> P_List = new List<Point2D>()
  //     {
  //       new Point2D(0, 0)
  //     , new Point2D(2, 0)
  //     , new Point2D(2, 2)
  //     , new Point2D(0, 2)
  //     };
  //   List<Point2D> Q_List = new List<Point2D>()
  //     {
  //       new Point2D(2, 2)
  //     , new Point2D(3.5, 1)
  //     , new Point2D(4, 2.5)
  //     };
  //
  //   ConvexPolygon square   = new ConvexPolygon(P_List);
  //   ConvexPolygon triangle = new ConvexPolygon(Q_List);
  //
  //   var res = ConvexPolygon.IntersectionPolygon(square, triangle);
  //   Assert.Null(res, "Intersection5");
  // }
  //
  // [Category("ConvexPolygonTests"), Test]
  // public void Intersection6() {
  //   List<Point2D> P_List = new List<Point2D>()
  //     {
  //       new Point2D(0, 0)
  //     , new Point2D(2, 0)
  //     , new Point2D(2, 2)
  //     , new Point2D(0, 2)
  //     };
  //   List<Point2D> Q_List = new List<Point2D>()
  //     {
  //       new Point2D(-1, 2)
  //     , new Point2D(-0.4, 1.4)
  //     , new Point2D(0.6, 2.9)
  //     };
  //
  //   ConvexPolygon square   = new ConvexPolygon(P_List);
  //   ConvexPolygon triangle = new ConvexPolygon(Q_List);
  //
  //   var res = ConvexPolygon.IntersectionPolygon(square, triangle);
  //   Assert.Null(res, "Intersection6");
  // }
  //
  // [Category("ConvexPolygonTests"), Test]
  // public void Intersection7() {
  //   List<Point2D> P_List = new List<Point2D>()
  //     {
  //       new Point2D(0, 0)
  //     , new Point2D(2, 0)
  //     , new Point2D(2, 2)
  //     , new Point2D(0, 2)
  //     };
  //   List<Point2D> Q_List = new List<Point2D>()
  //     {
  //       new Point2D(-1, 1.5)
  //     , new Point2D(-0.5, 1)
  //     , new Point2D(0, 2)
  //     };
  //
  //   ConvexPolygon square   = new ConvexPolygon(P_List);
  //   ConvexPolygon triangle = new ConvexPolygon(Q_List);
  //
  //   var res = ConvexPolygon.IntersectionPolygon(square, triangle);
  //   Assert.Null(res, "Intersection7");
  // }
  //
  // [Category("ConvexPolygonTests"), Test]
  // public void Intersection8() {
  //   List<Point2D> P_List = new List<Point2D>()
  //     {
  //       new Point2D(0, 0)
  //     , new Point2D(2, 0)
  //     , new Point2D(2, 2)
  //     , new Point2D(0, 2)
  //     };
  //   List<Point2D> Q_List = new List<Point2D>()
  //     {
  //       new Point2D(3.5, 1)
  //     , new Point2D(2.5, 2)
  //     , new Point2D(2.5, 0)
  //     };
  //
  //   ConvexPolygon square   = new ConvexPolygon(P_List);
  //   ConvexPolygon triangle = new ConvexPolygon(Q_List);
  //
  //   var res = ConvexPolygon.IntersectionPolygon(square, triangle);
  //   Assert.Null(res, "Intersection8");
  // }
  //
  // [Category("ConvexPolygonTests"), Test]
  // public void Intersection9() {
  //   List<Point2D> P_List = new List<Point2D>()
  //     {
  //       new Point2D(0, 0)
  //     , new Point2D(2, 0)
  //     , new Point2D(2, 2)
  //     , new Point2D(0, 2)
  //     };
  //   List<Point2D> Q_List = new List<Point2D>()
  //     {
  //       new Point2D(-1.5, 2.5)
  //     , new Point2D(1, -2)
  //     , new Point2D(3.5, 2.5)
  //     };
  //
  //   ConvexPolygon square   = new ConvexPolygon(P_List);
  //   ConvexPolygon triangle = new ConvexPolygon(Q_List);
  //
  //   var res = ConvexPolygon.IntersectionPolygon(square, triangle);
  //   CyclicListComparison(res.Vertices, P_List, "Intersection9");
  // }
  //
  // [Category("ConvexPolygonTests"), Test]
  // public void Intersection10() {
  //   List<Point2D> P_List = new List<Point2D>()
  //     {
  //       new Point2D(0, 0)
  //     , new Point2D(2, 0)
  //     , new Point2D(2, 2)
  //     , new Point2D(0, 2)
  //     };
  //   List<Point2D> Q_List = new List<Point2D>()
  //     {
  //       new Point2D(2, 2)
  //     , new Point2D(-0.5, 2)
  //     , new Point2D(2, -0.5)
  //     };
  //   List<Point2D> answerList = new List<Point2D>()
  //     {
  //       new Point2D(1.5, 0)
  //     , new Point2D(2, 0)
  //     , new Point2D(2, 2)
  //     , new Point2D(0, 2)
  //     , new Point2D(0, 1.5)
  //     };
  //   ConvexPolygon square   = new ConvexPolygon(P_List);
  //   ConvexPolygon triangle = new ConvexPolygon(Q_List);
  //
  //   var res = ConvexPolygon.IntersectionPolygon(square, triangle);
  //   CyclicListComparison(res.Vertices, answerList, "Intersection10");
  // }
  //
  // [Category("ConvexPolygonTests"), Test]
  // public void Intersection11() {
  //   List<Point2D> P_List1 = new List<Point2D>()
  //     {
  //       new Point2D(0, 0)
  //     , new Point2D(2, 0)
  //     , new Point2D(2, 2)
  //     , new Point2D(0, 2)
  //     };
  //   List<Point2D> P_List2 = new List<Point2D>()
  //     {
  //       new Point2D(0.5, 0.5)
  //     , new Point2D(1.5, 0.5)
  //     , new Point2D(1.5, 1.5)
  //     , new Point2D(0.5, 1.5)
  //     };
  //   ConvexPolygon square1 = new ConvexPolygon(P_List1);
  //   ConvexPolygon square2 = new ConvexPolygon(P_List2);
  //
  //   var res = ConvexPolygon.IntersectionPolygon(square1, square2);
  //   CyclicListComparison(res.Vertices, P_List2, "Intersection11");
  // }
  //
  // [Category("ConvexPolygonTests"), Test]
  // public void Intersection12() {
  //   List<Point2D> Q_List1 = new List<Point2D>()
  //     {
  //       new Point2D(3, 4)
  //     , new Point2D(6, 4)
  //     , new Point2D(4, 7)
  //     };
  //   List<Point2D> Q_List2 = new List<Point2D>()
  //     {
  //       new Point2D(4, 7)
  //     , new Point2D(2, 5)
  //     , new Point2D(6, 2)
  //     };
  //   List<Point2D> answerList = new List<Point2D>()
  //     {
  //       new Point2D(4, 7)
  //     , new Point2D(3.06666666666666, 4.2)
  //     , new Point2D(3.33333333333333, 4)
  //     , new Point2D(5.2, 4)
  //     };
  //   ConvexPolygon triangle1 = new ConvexPolygon(Q_List1);
  //   ConvexPolygon triangle2 = new ConvexPolygon(Q_List2);
  //
  //   var res = ConvexPolygon.IntersectionPolygon(triangle1, triangle2);
  //   CyclicListComparison(res.Vertices, answerList, "Intersection12");
  // }


}
