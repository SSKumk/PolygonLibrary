using CGLibrary;
using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;
namespace Tests.Double_Tests; 

public partial class ConvexPolygonTests {

  private readonly LinkedList<Point2D> squareList = new LinkedList<Point2D>
    (
     new List<Point2D>
       {
         new Point2D(0, 0)
       , new Point2D(6, 0)
       , new Point2D(6, 6)
       , new Point2D(0, 6)
       }
    );

  private void CyclicListComparison(List<Point2D> l1, List<Point2D> l2, string mes) {
    Assert.IsTrue(l1.Count == l2.Count, mes + ": lengths of the lists are different");
    int i2 = l2.IndexOf(l1[0]);
    Assert.GreaterOrEqual(i2, 0, mes + ": the second list does not contain the point " + l1[0]);
    for (int i1 = 0; i1 < l1.Count; i1++, i2 = (i2 + 1) % l2.Count) {
      Assert.IsTrue
        (
         l1[i1].CompareTo(l2[i2]) == 0
       , mes + ": point #" + i1 + " " + l1[i1] + " of the 1st list is not equal to point #" + i2 + " " + l2[i2] +
         " of the 2nd list"
        );
    }
  }

  private void DoIntersectionTest(string mes, LinkedList<Point2D> P_List, LinkedList<Point2D> Q_List, List<Point2D> answer) {
    for (int p = 0; p < P_List.Count; p++) {
      ConvexPolygon P = new ConvexPolygon(P_List);
      for (int q = 0; q < Q_List.Count; q++) {
        ConvexPolygon  Q     = new ConvexPolygon(Q_List);
        ConvexPolygon? resPQ = ConvexPolygon.IntersectionPolygon(P, Q);
        ConvexPolygon? resQP = ConvexPolygon.IntersectionPolygon(Q, P);

        Assert.IsNotNull(resPQ, $"Intersection{mes}: P-Q : P starts at {P.Vertices[0]}, Q starts at {Q.Vertices[0]} is Null");
        Assert.IsNotNull(resQP, $"Intersection{mes}: Q-P : P starts at {P.Vertices[0]}, Q starts at {Q.Vertices[0]} is Null");

        CyclicListComparison
          (answer, resPQ.Vertices, $"Intersection{mes}: P-Q : P starts at {P.Vertices[0]}, Q starts at {Q.Vertices[0]}");
        CyclicListComparison
          (answer, resQP.Vertices, $"Intersection{mes}: Q-P : P starts at {P.Vertices[0]}, Q starts at {Q.Vertices[0]}");
        Q_List.CyclicShift();
      }
      P_List.CyclicShift();
    }
  }

  private void NullIntersectionTest(string mes, LinkedList<Point2D> P_List, LinkedList<Point2D> Q_List) {
    for (int p = 0; p < P_List.Count; p++) {
      ConvexPolygon P = new ConvexPolygon(P_List);
      for (int q = 0; q < Q_List.Count; q++) {
        ConvexPolygon  Q     = new ConvexPolygon(Q_List);
        ConvexPolygon? resPQ = ConvexPolygon.IntersectionPolygon(P, Q);
        ConvexPolygon? resQP = ConvexPolygon.IntersectionPolygon(Q, P);
        Assert.IsNull(resPQ, $"Intersection{mes}: P-Q : P starts at {P.Vertices[0]}, Q starts at {Q.Vertices[0]}");
        Assert.IsNull(resQP, $"Intersection{mes}: P-Q : P starts at {P.Vertices[0]}, Q starts at {Q.Vertices[0]}");
        Q_List.CyclicShift();
      }
      P_List.CyclicShift();
    }
  }

  [Category("ConvexPolygonTests"), Test]
  public void Intersection01() {
    double save = Tools.Eps;
    Tools.Eps = 1e-8;
    LinkedList<Point2D>? P_List = squareList;
    LinkedList<Point2D>? Q_List = new LinkedList<Point2D>
      (
       new List<Point2D>
         {
           new Point2D(-2, 0)
         , new Point2D(3, -2)
         , new Point2D(7, 5)
         , new Point2D(3, 7)
         }
      );
    List<Point2D>? answerList = new List<Point2D>()
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
    Tools.Eps = save;
  }

  [Category("ConvexPolygonTests"), Test]
  public void Intersection02() {
    double save = Tools.Eps;
    Tools.Eps = 1e-8;

    LinkedList<Point2D>? P_List = squareList;
    LinkedList<Point2D>? Q_List = new LinkedList<Point2D>
      (new List<Point2D> { new Point2D(2, 2), new Point2D(8, 6), new Point2D(4, 4) });
    List<Point2D>? answerList = new List<Point2D>()
      {
        new Point2D(2, 2)
      , new Point2D(6, 4.666666666)
      , new Point2D(6, 5)
      , new Point2D(4, 4)
      };

    DoIntersectionTest("1-2", P_List, Q_List, answerList);
    Tools.Eps = save;
  }

  [Category("ConvexPolygonTests"), Test]
  public void Intersection03() {
    double save = Tools.Eps;
    Tools.Eps = 1e-8;

    LinkedList<Point2D>? P_List = squareList;
    LinkedList<Point2D>? Q_List = new LinkedList<Point2D>
      (
       new List<Point2D>
         {
           new Point2D(2, 4)
         , new Point2D(8, 2)
         , new Point2D(7, 6)
         , new Point2D(4, 8)
         }
      );
    List<Point2D>? answerList = new List<Point2D>()
      {
        new Point2D(2, 4)
      , new Point2D(6, 2.666666666)
      , new Point2D(6, 6)
      , new Point2D(3, 6)
      };

    DoIntersectionTest("1-3", P_List, Q_List, answerList);
    Tools.Eps = save;
  }

  [Category("ConvexPolygonTests"), Test]
  public void Intersection04() {
    LinkedList<Point2D>? P_List = squareList;
    LinkedList<Point2D>? Q_List = new LinkedList<Point2D>
      (new List<Point2D> { new Point2D(2, 5), new Point2D(3, 1), new Point2D(6, 4) });

    DoIntersectionTest("4-1", P_List, Q_List, Q_List.ToList());
  }

  [Category("ConvexPolygonTests"), Test]
  public void Intersection05() {
    LinkedList<Point2D>? P_List = squareList;
    LinkedList<Point2D>? Q_List = new LinkedList<Point2D>
      (new List<Point2D> { new Point2D(6, 4), new Point2D(7, 3), new Point2D(8, 5) });

    NullIntersectionTest("4-2", P_List, Q_List);
  }

  [Category("ConvexPolygonTests"), Test]
  public void Intersection06aux() {
    LinkedList<Point2D>? P_List = squareList;
    LinkedList<Point2D>? Q_List = new LinkedList<Point2D>
      (new List<Point2D> { new Point2D(2, 2), new Point2D(6, 4), new Point2D(4, 6) });

    DoIntersectionTest("4-3", P_List, Q_List, Q_List.ToList());
  }

  [Category("ConvexPolygonTests"), Test]
  public void Intersection06() {
    LinkedList<Point2D>? P_List = squareList;
    LinkedList<Point2D>? Q_List = new LinkedList<Point2D>
      (
       new List<Point2D>
         {
           new Point2D(0, 3)
         , new Point2D(3, 1)
         , new Point2D(6, 3)
         , new Point2D(3, 6)
         }
      );

    DoIntersectionTest("4-4", P_List, Q_List, Q_List.ToList());
  }

  [Category("ConvexPolygonTests"), Test]
  public void Intersection07() {
    LinkedList<Point2D>? P_List = squareList;
    LinkedList<Point2D>? Q_List = new LinkedList<Point2D>
      (new List<Point2D> { new Point2D(0, 3), new Point2D(3, 1), new Point2D(6, 3) });

    DoIntersectionTest("4-5", P_List, Q_List, Q_List.ToList());
  }

  [Category("ConvexPolygonTests"), Test]
  public void Intersection08() {
    LinkedList<Point2D>? P_List = squareList;
    LinkedList<Point2D>? Q_List = new LinkedList<Point2D>
      (new List<Point2D> { new Point2D(2, 8), new Point2D(2, 4), new Point2D(6, 4) });
    List<Point2D>? answerList = new List<Point2D>()
      {
        new Point2D(2, 6)
      , new Point2D(2, 4)
      , new Point2D(6, 4)
      , new Point2D(4, 6)
      };

    DoIntersectionTest("4-6", P_List, Q_List, answerList);
  }

  [Category("ConvexPolygonTests"), Test]
  public void Intersection09() {
    LinkedList<Point2D>? P_List = squareList;
    LinkedList<Point2D>? Q_List = new LinkedList<Point2D>
      (
       new List<Point2D>
         {
           new Point2D(0, 3)
         , new Point2D(4, 1)
         , new Point2D(6, 4)
         , new Point2D(3, 7)
         }
      );
    List<Point2D>? answerList = new List<Point2D>()
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
    double save = Tools.Eps;
    Tools.Eps = 1e-8;
    LinkedList<Point2D>? P_List = squareList;
    LinkedList<Point2D>? Q_List = new LinkedList<Point2D>
      (new List<Point2D> { new Point2D(2, 1), new Point2D(6, 2), new Point2D(9, 6) });
    List<Point2D>? answerList = new List<Point2D>() { new Point2D(2, 1), new Point2D(6, 2), new Point2D(6, 3.857142857) };

    DoIntersectionTest("4-8", P_List, Q_List, answerList);
    Tools.Eps = save;
  }

  [Category("ConvexPolygonTests"), Test]
  public void Intersection11() {
    LinkedList<Point2D>? P_List = squareList;
    LinkedList<Point2D>? Q_List = new LinkedList<Point2D>
      (new List<Point2D> { new Point2D(2, 4), new Point2D(4, 2), new Point2D(6, 6) });

    DoIntersectionTest("5-1", P_List, Q_List, Q_List.ToList());
  }

  [Category("ConvexPolygonTests"), Test]
  public void Intersection12() {
    LinkedList<Point2D>? P_List = squareList;
    LinkedList<Point2D>? Q_List = new LinkedList<Point2D>
      (new List<Point2D> { new Point2D(6, 6), new Point2D(8, 6), new Point2D(8, 8) });

    NullIntersectionTest("5-2", P_List, Q_List);
  }

  [Category("ConvexPolygonTests"), Test]
  public void Intersection13() {
    LinkedList<Point2D>? P_List = squareList;
    LinkedList<Point2D>? Q_List = new LinkedList<Point2D>
      (new List<Point2D> { new Point2D(-2, 2), new Point2D(3, 1), new Point2D(6, 6) });
    List<Point2D>? answerList = new List<Point2D>()
      {
        new Point2D(0, 3)
      , new Point2D(0, 1.6)
      , new Point2D(3, 1)
      , new Point2D(6, 6)
      };

    DoIntersectionTest("5-3", P_List, Q_List, answerList);
  }

  [Category("ConvexPolygonTests"), Test]
  public void Intersection14() {
    double save = Tools.Eps;
    Tools.Eps = 1e-8;

    LinkedList<Point2D>? P_List = squareList;
    LinkedList<Point2D>? Q_List = new LinkedList<Point2D>
      (new List<Point2D> { new Point2D(-1, 3), new Point2D(0, 0), new Point2D(6, 6) });
    List<Point2D>? answerList = new List<Point2D>() { new Point2D(0, 3.428571429), new Point2D(0, 0), new Point2D(6, 6) };

    DoIntersectionTest("5-4", P_List, Q_List, answerList);
    Tools.Eps = save;
  }

  [Category("ConvexPolygonTests"), Test]
  public void Intersection15() {
    LinkedList<Point2D>? P_List = squareList;
    LinkedList<Point2D>? Q_List = new LinkedList<Point2D>
      (new List<Point2D> { new Point2D(4, 4), new Point2D(8, 4), new Point2D(6, 6) });
    List<Point2D>? answerList = new List<Point2D>() { new Point2D(4, 4), new Point2D(6, 4), new Point2D(6, 6) };

    DoIntersectionTest("5-5", P_List, Q_List, answerList);
  }

  [Category("ConvexPolygonTests"), Test]
  public void Intersection16() {
    LinkedList<Point2D>? P_List = squareList;
    LinkedList<Point2D>? Q_List = new LinkedList<Point2D>
      (new List<Point2D> { new Point2D(0, 0), new Point2D(4, 1), new Point2D(6, 6) });

    DoIntersectionTest("5-6", P_List, Q_List, Q_List.ToList());
  }

  [Category("ConvexPolygonTests"), Test]
  public void Intersection17() {
    LinkedList<Point2D>? P_List = squareList;
    LinkedList<Point2D>? Q_List = new LinkedList<Point2D>
      (
       new List<Point2D>
         {
           new Point2D(0, 0)
         , new Point2D(6, -2)
         , new Point2D(8, 0)
         , new Point2D(6, 6)
         }
      );
    List<Point2D>? answerList = new List<Point2D>() { new Point2D(0, 0), new Point2D(6, 0), new Point2D(6, 6) };

    DoIntersectionTest("5-7", P_List, Q_List, answerList);
  }

  [Category("ConvexPolygonTests"), Test]
  public void Intersection18() {
    LinkedList<Point2D>? P_List = squareList;
    LinkedList<Point2D>? Q_List = new LinkedList<Point2D>
      (new List<Point2D> { new Point2D(2, 6), new Point2D(3, 3), new Point2D(4, 6) });

    DoIntersectionTest("6-1", P_List, Q_List, Q_List.ToList());
  }

  [Category("ConvexPolygonTests"), Test]
  public void Intersection19() {
    LinkedList<Point2D>? P_List = squareList;
    LinkedList<Point2D>? Q_List = new LinkedList<Point2D>
      (new List<Point2D> { new Point2D(2, 6), new Point2D(3, 0), new Point2D(4, 6) });

    DoIntersectionTest("6-2", P_List, Q_List, Q_List.ToList());
  }

  [Category("ConvexPolygonTests"), Test]
  public void Intersection20() {
    LinkedList<Point2D>? P_List = squareList;
    LinkedList<Point2D>? Q_List = new LinkedList<Point2D>
      (new List<Point2D> { new Point2D(2, 6), new Point2D(6, 0), new Point2D(4, 6) });

    DoIntersectionTest("6-3", P_List, Q_List, Q_List.ToList());
  }

  [Category("ConvexPolygonTests"), Test]
  public void Intersection21() {
    LinkedList<Point2D>? P_List = squareList;
    LinkedList<Point2D>? Q_List = new LinkedList<Point2D>
      (new List<Point2D> { new Point2D(4, 6), new Point2D(4, 2), new Point2D(6, 6) });

    DoIntersectionTest("6-4", P_List, Q_List, Q_List.ToList());
  }

  [Category("ConvexPolygonTests"), Test]
  public void Intersection22() {
    LinkedList<Point2D>? P_List = squareList;
    LinkedList<Point2D>? Q_List = new LinkedList<Point2D>
      (new List<Point2D> { new Point2D(4, 6), new Point2D(0, 0), new Point2D(6, 6) });

    DoIntersectionTest("6-5", P_List, Q_List, Q_List.ToList());
  }

  [Category("ConvexPolygonTests"), Test]
  public void Intersection23() {
    LinkedList<Point2D>? P_List = squareList;
    LinkedList<Point2D>? Q_List = new LinkedList<Point2D>
      (new List<Point2D> { new Point2D(2, 6), new Point2D(6, 0), new Point2D(6, 6) });

    DoIntersectionTest("6-6", P_List, Q_List, Q_List.ToList());
  }

  [Category("ConvexPolygonTests"), Test]
  public void Intersection24() {
    LinkedList<Point2D>? P_List = squareList;
    LinkedList<Point2D>? Q_List = new LinkedList<Point2D>
      (new List<Point2D> { new Point2D(4, 6), new Point2D(6, 2), new Point2D(6, 6) });

    DoIntersectionTest("6-7", P_List, Q_List, Q_List.ToList());
  }

  [Category("ConvexPolygonTests"), Test]
  public void Intersection25() {
    LinkedList<Point2D>? P_List = squareList;
    LinkedList<Point2D>? Q_List = new LinkedList<Point2D>
      (new List<Point2D> { new Point2D(0, 6), new Point2D(2, 2), new Point2D(4, 6) });

    DoIntersectionTest("6-8", P_List, Q_List, Q_List.ToList());
  }

  [Category("ConvexPolygonTests"), Test]
  public void Intersection26() {
    LinkedList<Point2D>? P_List = squareList;
    LinkedList<Point2D>? Q_List = new LinkedList<Point2D>
      (new List<Point2D> { new Point2D(0, 6), new Point2D(6, 0), new Point2D(2, 6) });

    DoIntersectionTest("6-9", P_List, Q_List, Q_List.ToList());
  }

  [Category("ConvexPolygonTests"), Test]
  public void Intersection27() {
    LinkedList<Point2D>? P_List = squareList;
    LinkedList<Point2D>? Q_List = new LinkedList<Point2D>
      (new List<Point2D> { new Point2D(0, 6), new Point2D(0, 2), new Point2D(2, 6) });

    DoIntersectionTest("6-10", P_List, Q_List, Q_List.ToList());
  }

  [Category("ConvexPolygonTests"), Test]
  public void Intersection28() {
    LinkedList<Point2D>? P_List = squareList;
    LinkedList<Point2D>? Q_List = new LinkedList<Point2D>
      (new List<Point2D> { new Point2D(0, 6), new Point2D(0, 0), new Point2D(2, 6) });

    DoIntersectionTest("6-11", P_List, Q_List, Q_List.ToList());
  }

  [Category("ConvexPolygonTests"), Test]
  public void Intersection29() {
    LinkedList<Point2D>? P_List = squareList;
    LinkedList<Point2D>? Q_List = new LinkedList<Point2D>
      (new List<Point2D> { new Point2D(-2, 6), new Point2D(2, 2), new Point2D(2, 6) });
    List<Point2D>? answerList = new List<Point2D>()
      {
        new Point2D(0, 4)
      , new Point2D(2, 2)
      , new Point2D(2, 6)
      , new Point2D(0, 6)
      };

    DoIntersectionTest("6-12", P_List, Q_List, answerList);
  }

  [Category("ConvexPolygonTests"), Test]
  public void Intersection30() {

    double save = Tools.Eps;
    Tools.Eps = 1e-8;
    LinkedList<Point2D>? P_List = squareList;
    LinkedList<Point2D>? Q_List = new LinkedList<Point2D>
      (new List<Point2D> { new Point2D(-2, 6), new Point2D(1, -1), new Point2D(2, 6) });
    List<Point2D>? answerList = new List<Point2D>()
      {
        new Point2D(0, 6)
      , new Point2D(0, 1.333333333)
      , new Point2D(0.5714285714, 0)
      , new Point2D(1.142857143, 0)
      , new Point2D(2, 6)
      };

    DoIntersectionTest("6-13", P_List, Q_List, answerList);
    Tools.Eps = save;
  }

  [Category("ConvexPolygonTests"), Test]
  public void Intersection31() {
    LinkedList<Point2D>? P_List = squareList;
    LinkedList<Point2D>? Q_List = new LinkedList<Point2D>
      (new List<Point2D> { new Point2D(-2, 6), new Point2D(0, 0), new Point2D(2, 6) });
    List<Point2D>? answerList = new List<Point2D>() { new Point2D(0, 6), new Point2D(0, 0), new Point2D(2, 6) };

    DoIntersectionTest("6-14", P_List, Q_List, answerList);
  }

  [Category("ConvexPolygonTests"), Test]
  public void Intersection32() {
    double save = Tools.Eps;
    Tools.Eps = 1e-8;

    LinkedList<Point2D>? P_List = squareList;
    LinkedList<Point2D>? Q_List = new LinkedList<Point2D>
      (new List<Point2D> { new Point2D(-6, 6), new Point2D(1, -1), new Point2D(2, 6) });
    List<Point2D>? answerList = new List<Point2D>()
      {
        new Point2D(0, 6)
      , new Point2D(0, 0)
      , new Point2D(1.142857143, 0)
      , new Point2D(2, 6)
      };

    DoIntersectionTest("6-15", P_List, Q_List, answerList);
    Tools.Eps = save;
  }

  [Category("ConvexPolygonTests"), Test]
  public void Intersection33() {
    LinkedList<Point2D>? P_List = squareList;
    LinkedList<Point2D>? Q_List = new LinkedList<Point2D>
      (
       new List<Point2D>
         {
           new Point2D(8, 3)
         , new Point2D(10, 0)
         , new Point2D(12, 3)
         , new Point2D(10, 6)
         }
      );

    NullIntersectionTest("7-1", P_List, Q_List);
  }

  [Category("ConvexPolygonTests"), Test]
  public void Intersection34() {
    LinkedList<Point2D>? P_List = squareList;
    LinkedList<Point2D>? Q_List = new LinkedList<Point2D>
      (
       new List<Point2D>
         {
           new Point2D(2, 2)
         , new Point2D(4, 2)
         , new Point2D(4, 4)
         , new Point2D(2, 4)
         }
      );

    DoIntersectionTest("7-2", P_List, Q_List, Q_List.ToList());
  }

  [Category("ConvexPolygonTests"), Test]
  public void Intersection35() {
    LinkedList<Point2D>? P_List = squareList;

    DoIntersectionTest("7-3", P_List, P_List, squareList.ToList());
  }

  [Category("ConvexPolygonTests"), Test]
  public void Intersection36() {
    LinkedList<Point2D>? P_List = squareList;
    LinkedList<Point2D>? Q_List = new LinkedList<Point2D>
      (
       new List<Point2D>
         {
           new Point2D(2, 6)
         , new Point2D(4, 6)
         , new Point2D(4, 8)
         , new Point2D(2, 8)
         }
      );

    NullIntersectionTest("7-4", P_List, Q_List);
  }

  [Category("ConvexPolygonTests"), Test]
  public void Intersection37() {
    LinkedList<Point2D>? P_List = squareList;
    LinkedList<Point2D>? Q_List = new LinkedList<Point2D>
      (new List<Point2D> { new Point2D(0, 6), new Point2D(4, 6), new Point2D(2, 8) });

    NullIntersectionTest("7-5", P_List, Q_List);
  }

  [Category("ConvexPolygonTests"), Test]
  public void Intersection38() {
    LinkedList<Point2D>? P_List = squareList;
    LinkedList<Point2D>? Q_List = new LinkedList<Point2D>
      (new List<Point2D> { new Point2D(0, 6), new Point2D(6, 6), new Point2D(4, 8) });

    NullIntersectionTest("7-6", P_List, Q_List);
  }

}