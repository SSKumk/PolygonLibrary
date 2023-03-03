using NUnit.Framework;
using System.Collections.Generic;
using PolygonLibrary.Basics;
using PolygonLibrary.Polygons.ConvexPolygons;
using PolygonLibrary.Toolkit;

namespace Tests;

public partial class ConvexPolygonTests {

  private readonly LinkedList<Point2D> squareList = new LinkedList<Point2D>(new List<Point2D>
    {
      new Point2D(0, 0)
    , new Point2D(6, 0)
    , new Point2D(6, 6)
    , new Point2D(0, 6)
    });

  private void DoIntersectionTest(string              mes
                                , LinkedList<Point2D> P_List
                                , LinkedList<Point2D> Q_List
                                , List<Point2D>       answer) {
    for (int p = 0; p < P_List.Count; p++) {
      var P = new ConvexPolygon(P_List);
      for (int q = 0; q < Q_List.Count; q++) {
        var Q     = new ConvexPolygon(Q_List);
        var resPQ = ConvexPolygon.IntersectionPolygon(P, Q);
        var resQP = ConvexPolygon.IntersectionPolygon(Q, P);
        CyclicListComparison(answer, resPQ.Vertices
                           , $"Intersection{mes}: P-Q : P starts at {P.Vertices[0]}, Q starts at {Q.Vertices[0]}");
        CyclicListComparison(answer, resQP.Vertices
                           , $"Intersection{mes}: Q-P : P starts at {P.Vertices[0]}, Q starts at {Q.Vertices[0]}");
        Q_List.CyclicShift();
      }
      P_List.CyclicShift();
    }
  }

  private void NullIntersectionTest(string              mes
                                    , LinkedList<Point2D> P_List
                                    , LinkedList<Point2D> Q_List) {
    for (int p = 0; p < P_List.Count; p++) {
      var P = new ConvexPolygon(P_List);
      for (int q = 0; q < Q_List.Count; q++) {
        var Q     = new ConvexPolygon(Q_List);
        var resPQ = ConvexPolygon.IntersectionPolygon(P, Q);
        var resQP = ConvexPolygon.IntersectionPolygon(Q, P);
        Assert.IsNull(resPQ, $"Intersection{mes}: P-Q : P starts at {P.Vertices[0]}, Q starts at {Q.Vertices[0]}");
        Assert.IsNull(resQP, $"Intersection{mes}: P-Q : P starts at {P.Vertices[0]}, Q starts at {Q.Vertices[0]}");
        Q_List.CyclicShift();
      }
      P_List.CyclicShift();
    }
  }

  [Category("ConvexPolygonTests"), Test]
  public void Intersection1() {
    var P_List = squareList;
    var Q_List = new LinkedList<Point2D>(new List<Point2D>
      {
        new Point2D(-2, 0)
      , new Point2D(3, -2)
      , new Point2D(7, 5)
      , new Point2D(3, 7)
      });
    var answerList = new List<Point2D>()
      {
        new Point2D(0, 2.8)
      , new Point2D(0, 0)
      , new Point2D(4.142857143, 0)
      , new Point2D(6, 3.25)
      , new Point2D(6, 5.5)
      , new Point2D(5, 6)
      , new Point2D(2.285714286, 6)
      };

    DoIntersectionTest("1-1", P_List, Q_List, answerList);
  }

  [Category("ConvexPolygonTests"), Test]
  public void Intersection2() {
    var P_List = squareList;
    var Q_List = new LinkedList<Point2D>(new List<Point2D>
      {
        new Point2D(2, 2)
      , new Point2D(8, 6)
      , new Point2D(4, 4)
      });
    var answerList = new List<Point2D>()
      {
        new Point2D(2, 2)
      , new Point2D(6, 4.666666666)
      , new Point2D(6, 5)
      , new Point2D(4, 4)
      };

    DoIntersectionTest("1-2", P_List, Q_List, answerList);
  }

  [Category("ConvexPolygonTests"), Test]
  public void Intersection3() {
    var P_List = squareList;
    var Q_List = new LinkedList<Point2D>(new List<Point2D>
      {
        new Point2D(2, 4)
      , new Point2D(8, 2)
      , new Point2D(7, 6)
      , new Point2D(4, 8)
      });
    var answerList = new List<Point2D>()
      {
        new Point2D(2, 4)
      , new Point2D(6, 2.666666666)
      , new Point2D(6, 6)
      , new Point2D(3, 6)
      };

    DoIntersectionTest("1-3", P_List, Q_List, answerList);
  }

  [Category("ConvexPolygonTests"), Test]
  public void Intersection4() {
    var P_List = squareList;
    var Q_List = new LinkedList<Point2D>(new List<Point2D>
      {
        new Point2D(2, 5)
      , new Point2D(3, 1)
      , new Point2D(6, 4)
      });
    var answerList = new List<Point2D>()
      {
        new Point2D(2, 5)
      , new Point2D(3, 1)
      , new Point2D(6, 4)
      };

    DoIntersectionTest("4-1", P_List, Q_List, answerList);
  }

  [Category("ConvexPolygonTests"), Test]
  public void Intersection5() {
    var P_List = squareList;
    var Q_List = new LinkedList<Point2D>(new List<Point2D>
      {
        new Point2D(6, 4)
      , new Point2D(7, 3)
      , new Point2D(8, 5)
      });

    NullIntersectionTest("4-2", P_List, Q_List);
  }

  [Category("ConvexPolygonTests"), Test]
  public void Intersection6() {
    var P_List = squareList;
    var Q_List = new LinkedList<Point2D>(new List<Point2D>
      {
        new Point2D(0, 3)
      , new Point2D(3, 1)
      , new Point2D(6, 3)
      , new Point2D(3, 6)
      });
    var answerList = new List<Point2D>()
      {
        new Point2D(0, 3)
      , new Point2D(3, 1)
      , new Point2D(6, 3)
      , new Point2D(3, 6)
      };

    DoIntersectionTest("4-4", P_List, Q_List, answerList);
  }

  [Category("ConvexPolygonTests"), Test]
  public void Intersection7() {
    var P_List = squareList;
    var Q_List = new LinkedList<Point2D>(new List<Point2D>
      {
        new Point2D(0, 3)
      , new Point2D(3, 1)
      , new Point2D(6, 3)
      });
    var answerList = new List<Point2D>()
      {
        new Point2D(0, 3)
      , new Point2D(3, 1)
      , new Point2D(6, 3)
      };

    DoIntersectionTest("4-5", P_List, Q_List, answerList);
  }

  [Category("ConvexPolygonTests"), Test]
  public void Intersection8() {
    var P_List = squareList;
    var Q_List = new LinkedList<Point2D>(new List<Point2D>
      {
        new Point2D(2, 4)
      , new Point2D(6, 4)
      , new Point2D(2, 8)
      });
    var answerList = new List<Point2D>()
      {
        new Point2D(2, 4)
      , new Point2D(6, 4)
      , new Point2D(4, 6)
      , new Point2D(2, 6)
      };

    DoIntersectionTest("4-6", P_List, Q_List, answerList);
  }

  [Category("ConvexPolygonTests"), Test]
  public void Intersection9() {
    var P_List = squareList;
    var Q_List = new LinkedList<Point2D>(new List<Point2D>
      {
        new Point2D(0, 3)
      , new Point2D(4, 1)
      , new Point2D(6, 4)
      , new Point2D(3, 7)
      });
    var answerList = new List<Point2D>()
      {
        new Point2D(0, 3)
      , new Point2D(4, 1)
      , new Point2D(6, 4)
      , new Point2D(4, 6)
      , new Point2D(2.25, 6)
      };

    DoIntersectionTest("4-7", P_List, Q_List, answerList);
  }

  [Category("ConvexPolygonTests"), Test]
  public void Intersection10() {
    var P_List = squareList;
    var Q_List = new LinkedList<Point2D>(new List<Point2D>
      {
        new Point2D(2, 1)
      , new Point2D(6, 2)
      , new Point2D(9, 6)
      });
    var answerList = new List<Point2D>()
      {
        new Point2D(2, 1)
      , new Point2D(6, 2)
      , new Point2D(6, 3.857142857)
      };

    DoIntersectionTest("4-8", P_List, Q_List, answerList);
  }


}
