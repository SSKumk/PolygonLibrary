using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;

namespace Tests.DoubleGeometry.Basics;

[TestFixture]
public class ConvexPolygonIntersectionBasicCasesTests : ConvexPolygonIntersectionTestBase {

  [Category("ConvexPolygonTests"), Test]
  public void Intersection01() {
    double save = Tools.Eps;
    Tools.Eps = 1e-8;
    LinkedList<Vector2D> P_List = SquareList;
    LinkedList<Vector2D> Q_List = new LinkedList<Vector2D>
      (
       new List<Vector2D>
         {
           new Vector2D(-2, 0)
         , new Vector2D(3, -2)
         , new Vector2D(7, 5)
         , new Vector2D(3, 7)
         }
      );
    List<Vector2D> answerList = new List<Vector2D>()
      {
        new Vector2D(0, 2.8)
      , new Vector2D(0, 0)
      , new Vector2D(4.142857143, 0)
      , new Vector2D(6, 3.25)
      , new Vector2D(6, 5.5)
      , new Vector2D(5, 6)
      , new Vector2D(2.285714286, 6)
      };

    DoIntersectionTest("1-1", P_List, Q_List, answerList);
    Tools.Eps = save;
  }

  [Category("ConvexPolygonTests"), Test]
  public void Intersection02() {
    double save = Tools.Eps;
    Tools.Eps = 1e-8;

    LinkedList<Vector2D> P_List = SquareList;
    LinkedList<Vector2D> Q_List = new LinkedList<Vector2D>
      (new List<Vector2D> { new Vector2D(2, 2), new Vector2D(8, 6), new Vector2D(4, 4) });
    List<Vector2D> answerList = new List<Vector2D>()
      {
        new Vector2D(2, 2)
      , new Vector2D(6, 4.666666666)
      , new Vector2D(6, 5)
      , new Vector2D(4, 4)
      };

    DoIntersectionTest("1-2", P_List, Q_List, answerList);
    Tools.Eps = save;
  }

  [Category("ConvexPolygonTests"), Test]
  public void Intersection03() {
    double save = Tools.Eps;
    Tools.Eps = 1e-8;

    LinkedList<Vector2D> P_List = SquareList;
    LinkedList<Vector2D> Q_List = new LinkedList<Vector2D>
      (
       new List<Vector2D>
         {
           new Vector2D(2, 4)
         , new Vector2D(8, 2)
         , new Vector2D(7, 6)
         , new Vector2D(4, 8)
         }
      );
    List<Vector2D> answerList = new List<Vector2D>()
      {
        new Vector2D(2, 4)
      , new Vector2D(6, 2.666666666)
      , new Vector2D(6, 6)
      , new Vector2D(3, 6)
      };

    DoIntersectionTest("1-3", P_List, Q_List, answerList);
    Tools.Eps = save;
  }

  [Category("ConvexPolygonTests"), Test]
  public void Intersection04() {
    LinkedList<Vector2D> P_List = SquareList;
    LinkedList<Vector2D> Q_List = new LinkedList<Vector2D>
      (new List<Vector2D> { new Vector2D(2, 5), new Vector2D(3, 1), new Vector2D(6, 4) });

    DoIntersectionTest("4-1", P_List, Q_List, Q_List.ToList());
  }

  [Category("ConvexPolygonTests"), Test]
  public void Intersection06aux() {
    LinkedList<Vector2D> P_List = SquareList;
    LinkedList<Vector2D> Q_List = new LinkedList<Vector2D>
      (new List<Vector2D> { new Vector2D(2, 2), new Vector2D(6, 4), new Vector2D(4, 6) });

    DoIntersectionTest("4-3", P_List, Q_List, Q_List.ToList());
  }

  [Category("ConvexPolygonTests"), Test]
  public void Intersection06() {
    LinkedList<Vector2D> P_List = SquareList;
    LinkedList<Vector2D> Q_List = new LinkedList<Vector2D>
      (
       new List<Vector2D>
         {
           new Vector2D(0, 3)
         , new Vector2D(3, 1)
         , new Vector2D(6, 3)
         , new Vector2D(3, 6)
         }
      );

    DoIntersectionTest("4-4", P_List, Q_List, Q_List.ToList());
  }

  [Category("ConvexPolygonTests"), Test]
  public void Intersection07() {
    LinkedList<Vector2D> P_List = SquareList;
    LinkedList<Vector2D> Q_List = new LinkedList<Vector2D>
      (new List<Vector2D> { new Vector2D(0, 3), new Vector2D(3, 1), new Vector2D(6, 3) });

    DoIntersectionTest("4-5", P_List, Q_List, Q_List.ToList());
  }

  [Category("ConvexPolygonTests"), Test]
  public void Intersection08() {
    LinkedList<Vector2D> P_List = SquareList;
    LinkedList<Vector2D> Q_List = new LinkedList<Vector2D>
      (new List<Vector2D> { new Vector2D(2, 8), new Vector2D(2, 4), new Vector2D(6, 4) });
    List<Vector2D> answerList = new List<Vector2D>()
      {
        new Vector2D(2, 6)
      , new Vector2D(2, 4)
      , new Vector2D(6, 4)
      , new Vector2D(4, 6)
      };

    DoIntersectionTest("4-6", P_List, Q_List, answerList);
  }

  [Category("ConvexPolygonTests"), Test]
  public void Intersection09() {
    LinkedList<Vector2D> P_List = SquareList;
    LinkedList<Vector2D> Q_List = new LinkedList<Vector2D>
      (
       new List<Vector2D>
         {
           new Vector2D(0, 3)
         , new Vector2D(4, 1)
         , new Vector2D(6, 4)
         , new Vector2D(3, 7)
         }
      );
    List<Vector2D> answerList = new List<Vector2D>()
      {
        new Vector2D(0, 3)
      , new Vector2D(4, 1)
      , new Vector2D(6, 4)
      , new Vector2D(4, 6)
      , new Vector2D(2.25, 6)
      };

    DoIntersectionTest("4-7", P_List, Q_List, answerList);
  }

  [Category("ConvexPolygonTests"), Test]
  public void Intersection10() {
    double save = Tools.Eps;
    Tools.Eps = 1e-8;
    LinkedList<Vector2D> P_List = SquareList;
    LinkedList<Vector2D> Q_List = new LinkedList<Vector2D>
      (new List<Vector2D> { new Vector2D(2, 1), new Vector2D(6, 2), new Vector2D(9, 6) });
    List<Vector2D> answerList = new List<Vector2D>() { new Vector2D(2, 1), new Vector2D(6, 2), new Vector2D(6, 3.857142857) };

    DoIntersectionTest("4-8", P_List, Q_List, answerList);
    Tools.Eps = save;
  }

  [Category("ConvexPolygonTests"), Test]
  public void Intersection11() {
    LinkedList<Vector2D> P_List = SquareList;
    LinkedList<Vector2D> Q_List = new LinkedList<Vector2D>
      (new List<Vector2D> { new Vector2D(2, 4), new Vector2D(4, 2), new Vector2D(6, 6) });

    DoIntersectionTest("5-1", P_List, Q_List, Q_List.ToList());
  }

  [Category("ConvexPolygonTests"), Test]
  public void Intersection13() {
    LinkedList<Vector2D> P_List = SquareList;
    LinkedList<Vector2D> Q_List = new LinkedList<Vector2D>
      (new List<Vector2D> { new Vector2D(-2, 2), new Vector2D(3, 1), new Vector2D(6, 6) });
    List<Vector2D> answerList = new List<Vector2D>()
      {
        new Vector2D(0, 3)
      , new Vector2D(0, 1.6)
      , new Vector2D(3, 1)
      , new Vector2D(6, 6)
      };

    DoIntersectionTest("5-3", P_List, Q_List, answerList);
  }

  [Category("ConvexPolygonTests"), Test]
  public void Intersection14() {
    double save = Tools.Eps;
    Tools.Eps = 1e-8;

    LinkedList<Vector2D> P_List = SquareList;
    LinkedList<Vector2D> Q_List = new LinkedList<Vector2D>
      (new List<Vector2D> { new Vector2D(-1, 3), new Vector2D(0, 0), new Vector2D(6, 6) });
    List<Vector2D> answerList = new List<Vector2D>() { new Vector2D(0, 3.428571429), new Vector2D(0, 0), new Vector2D(6, 6) };

    DoIntersectionTest("5-4", P_List, Q_List, answerList);
    Tools.Eps = save;
  }

  [Category("ConvexPolygonTests"), Test]
  public void Intersection15() {
    LinkedList<Vector2D> P_List = SquareList;
    LinkedList<Vector2D> Q_List = new LinkedList<Vector2D>
      (new List<Vector2D> { new Vector2D(4, 4), new Vector2D(8, 4), new Vector2D(6, 6) });
    List<Vector2D> answerList = new List<Vector2D>() { new Vector2D(4, 4), new Vector2D(6, 4), new Vector2D(6, 6) };

    DoIntersectionTest("5-5", P_List, Q_List, answerList);
  }

  [Category("ConvexPolygonTests"), Test]
  public void Intersection17() {
    LinkedList<Vector2D> P_List = SquareList;
    LinkedList<Vector2D> Q_List = new LinkedList<Vector2D>
      (
       new List<Vector2D>
         {
           new Vector2D(0, 0)
         , new Vector2D(6, -2)
         , new Vector2D(8, 0)
         , new Vector2D(6, 6)
         }
      );
    List<Vector2D> answerList = new List<Vector2D>() { new Vector2D(0, 0), new Vector2D(6, 0), new Vector2D(6, 6) };

    DoIntersectionTest("5-7", P_List, Q_List, answerList);
  }

}
