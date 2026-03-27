using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;

namespace Tests.DoubleGeometry.Basics;

[TestFixture]
public class ConvexPolygonIntersectionBoundaryAndDegenerateCasesTests : ConvexPolygonIntersectionTestBase {

  [Test]
  public void IntersectionPolygon_NullArgs_ReturnNull() {
    ConvexPolygon polygon = new ConvexPolygon(SquareList);

    Assert.Multiple(() => {
      Assert.That(ConvexPolygon.IntersectionPolygon(null, polygon), Is.Null);
      Assert.That(ConvexPolygon.IntersectionPolygon(polygon, null), Is.Null);
      Assert.That(ConvexPolygon.IntersectionPolygon(null, null), Is.Null);
    });
  }

  [Category("ConvexPolygonTests"), Test]
  public void Intersection05() {
    LinkedList<Vector2D> P_List = SquareList;
    LinkedList<Vector2D> Q_List = new LinkedList<Vector2D>
      (new List<Vector2D> { new Vector2D(6, 4), new Vector2D(7, 3), new Vector2D(8, 5) });

    NullIntersectionTest("4-2", P_List, Q_List);
  }

  [Category("ConvexPolygonTests"), Test]
  public void Intersection12() {
    LinkedList<Vector2D> P_List = SquareList;
    LinkedList<Vector2D> Q_List = new LinkedList<Vector2D>
      (new List<Vector2D> { new Vector2D(6, 6), new Vector2D(8, 6), new Vector2D(8, 8) });

    NullIntersectionTest("5-2", P_List, Q_List);
  }

  [Category("ConvexPolygonTests"), Test]
  public void Intersection16() {
    LinkedList<Vector2D> P_List = SquareList;
    LinkedList<Vector2D> Q_List = new LinkedList<Vector2D>
      (new List<Vector2D> { new Vector2D(0, 0), new Vector2D(4, 1), new Vector2D(6, 6) });

    DoIntersectionTest("5-6", P_List, Q_List, Q_List.ToList());
  }

  [Category("ConvexPolygonTests"), Test]
  public void Intersection18() {
    LinkedList<Vector2D> P_List = SquareList;
    LinkedList<Vector2D> Q_List = new LinkedList<Vector2D>
      (new List<Vector2D> { new Vector2D(2, 6), new Vector2D(3, 3), new Vector2D(4, 6) });

    DoIntersectionTest("6-1", P_List, Q_List, Q_List.ToList());
  }

  [Category("ConvexPolygonTests"), Test]
  public void Intersection19() {
    LinkedList<Vector2D> P_List = SquareList;
    LinkedList<Vector2D> Q_List = new LinkedList<Vector2D>
      (new List<Vector2D> { new Vector2D(2, 6), new Vector2D(3, 0), new Vector2D(4, 6) });

    DoIntersectionTest("6-2", P_List, Q_List, Q_List.ToList());
  }

  [Category("ConvexPolygonTests"), Test]
  public void Intersection20() {
    LinkedList<Vector2D> P_List = SquareList;
    LinkedList<Vector2D> Q_List = new LinkedList<Vector2D>
      (new List<Vector2D> { new Vector2D(2, 6), new Vector2D(6, 0), new Vector2D(4, 6) });

    DoIntersectionTest("6-3", P_List, Q_List, Q_List.ToList());
  }

  [Category("ConvexPolygonTests"), Test]
  public void Intersection21() {
    LinkedList<Vector2D> P_List = SquareList;
    LinkedList<Vector2D> Q_List = new LinkedList<Vector2D>
      (new List<Vector2D> { new Vector2D(4, 6), new Vector2D(4, 2), new Vector2D(6, 6) });

    DoIntersectionTest("6-4", P_List, Q_List, Q_List.ToList());
  }

  [Category("ConvexPolygonTests"), Test]
  public void Intersection22() {
    LinkedList<Vector2D> P_List = SquareList;
    LinkedList<Vector2D> Q_List = new LinkedList<Vector2D>
      (new List<Vector2D> { new Vector2D(4, 6), new Vector2D(0, 0), new Vector2D(6, 6) });

    DoIntersectionTest("6-5", P_List, Q_List, Q_List.ToList());
  }

  [Category("ConvexPolygonTests"), Test]
  public void Intersection23() {
    LinkedList<Vector2D> P_List = SquareList;
    LinkedList<Vector2D> Q_List = new LinkedList<Vector2D>
      (new List<Vector2D> { new Vector2D(2, 6), new Vector2D(6, 0), new Vector2D(6, 6) });

    DoIntersectionTest("6-6", P_List, Q_List, Q_List.ToList());
  }

  [Category("ConvexPolygonTests"), Test]
  public void Intersection24() {
    LinkedList<Vector2D> P_List = SquareList;
    LinkedList<Vector2D> Q_List = new LinkedList<Vector2D>
      (new List<Vector2D> { new Vector2D(4, 6), new Vector2D(6, 2), new Vector2D(6, 6) });

    DoIntersectionTest("6-7", P_List, Q_List, Q_List.ToList());
  }

  [Category("ConvexPolygonTests"), Test]
  public void Intersection25() {
    LinkedList<Vector2D> P_List = SquareList;
    LinkedList<Vector2D> Q_List = new LinkedList<Vector2D>
      (new List<Vector2D> { new Vector2D(0, 6), new Vector2D(2, 2), new Vector2D(4, 6) });

    DoIntersectionTest("6-8", P_List, Q_List, Q_List.ToList());
  }

  [Category("ConvexPolygonTests"), Test]
  public void Intersection26() {
    LinkedList<Vector2D> P_List = SquareList;
    LinkedList<Vector2D> Q_List = new LinkedList<Vector2D>
      (new List<Vector2D> { new Vector2D(0, 6), new Vector2D(6, 0), new Vector2D(2, 6) });

    DoIntersectionTest("6-9", P_List, Q_List, Q_List.ToList());
  }

  [Category("ConvexPolygonTests"), Test]
  public void Intersection27() {
    LinkedList<Vector2D> P_List = SquareList;
    LinkedList<Vector2D> Q_List = new LinkedList<Vector2D>
      (new List<Vector2D> { new Vector2D(0, 6), new Vector2D(0, 2), new Vector2D(2, 6) });

    DoIntersectionTest("6-10", P_List, Q_List, Q_List.ToList());
  }

  [Category("ConvexPolygonTests"), Test]
  public void Intersection28() {
    LinkedList<Vector2D> P_List = SquareList;
    LinkedList<Vector2D> Q_List = new LinkedList<Vector2D>
      (new List<Vector2D> { new Vector2D(0, 6), new Vector2D(0, 0), new Vector2D(2, 6) });

    DoIntersectionTest("6-11", P_List, Q_List, Q_List.ToList());
  }

  [Category("ConvexPolygonTests"), Test]
  public void Intersection29() {
    LinkedList<Vector2D> P_List = SquareList;
    LinkedList<Vector2D> Q_List = new LinkedList<Vector2D>
      (new List<Vector2D> { new Vector2D(-2, 6), new Vector2D(2, 2), new Vector2D(2, 6) });
    List<Vector2D> answerList = new List<Vector2D>()
      {
        new Vector2D(0, 4)
      , new Vector2D(2, 2)
      , new Vector2D(2, 6)
      , new Vector2D(0, 6)
      };

    DoIntersectionTest("6-12", P_List, Q_List, answerList);
  }

  [Category("ConvexPolygonTests"), Test]
  public void Intersection30() {
    double save = Tools.Eps;
    Tools.Eps = 1e-8;
    LinkedList<Vector2D> P_List = SquareList;
    LinkedList<Vector2D> Q_List = new LinkedList<Vector2D>
      (new List<Vector2D> { new Vector2D(-2, 6), new Vector2D(1, -1), new Vector2D(2, 6) });
    List<Vector2D> answerList = new List<Vector2D>()
      {
        new Vector2D(0, 6)
      , new Vector2D(0, 1.333333333)
      , new Vector2D(0.5714285714, 0)
      , new Vector2D(1.142857143, 0)
      , new Vector2D(2, 6)
      };

    DoIntersectionTest("6-13", P_List, Q_List, answerList);
    Tools.Eps = save;
  }

  [Category("ConvexPolygonTests"), Test]
  public void Intersection31() {
    LinkedList<Vector2D> P_List = SquareList;
    LinkedList<Vector2D> Q_List = new LinkedList<Vector2D>
      (new List<Vector2D> { new Vector2D(-2, 6), new Vector2D(0, 0), new Vector2D(2, 6) });
    List<Vector2D> answerList = new List<Vector2D>() { new Vector2D(0, 6), new Vector2D(0, 0), new Vector2D(2, 6) };

    DoIntersectionTest("6-14", P_List, Q_List, answerList);
  }

  [Category("ConvexPolygonTests"), Test]
  public void Intersection32() {
    double save = Tools.Eps;
    Tools.Eps = 1e-8;

    LinkedList<Vector2D> P_List = SquareList;
    LinkedList<Vector2D> Q_List = new LinkedList<Vector2D>
      (new List<Vector2D> { new Vector2D(-6, 6), new Vector2D(1, -1), new Vector2D(2, 6) });
    List<Vector2D> answerList = new List<Vector2D>()
      {
        new Vector2D(0, 6)
      , new Vector2D(0, 0)
      , new Vector2D(1.142857143, 0)
      , new Vector2D(2, 6)
      };

    DoIntersectionTest("6-15", P_List, Q_List, answerList);
    Tools.Eps = save;
  }

  [Category("ConvexPolygonTests"), Test]
  public void Intersection33() {
    LinkedList<Vector2D> P_List = SquareList;
    LinkedList<Vector2D> Q_List = new LinkedList<Vector2D>
      (
       new List<Vector2D>
         {
           new Vector2D(8, 3)
         , new Vector2D(10, 0)
         , new Vector2D(12, 3)
         , new Vector2D(10, 6)
         }
      );

    NullIntersectionTest("7-1", P_List, Q_List);
  }

  [Category("ConvexPolygonTests"), Test]
  public void Intersection34() {
    LinkedList<Vector2D> P_List = SquareList;
    LinkedList<Vector2D> Q_List = new LinkedList<Vector2D>
      (
       new List<Vector2D>
         {
           new Vector2D(2, 2)
         , new Vector2D(4, 2)
         , new Vector2D(4, 4)
         , new Vector2D(2, 4)
         }
      );

    DoIntersectionTest("7-2", P_List, Q_List, Q_List.ToList());
  }

  [Category("ConvexPolygonTests"), Test]
  public void Intersection35() {
    LinkedList<Vector2D> P_List = SquareList;

    DoIntersectionTest("7-3", P_List, P_List, SquareList.ToList());
  }

  [Category("ConvexPolygonTests"), Test]
  public void Intersection36() {
    LinkedList<Vector2D> P_List = SquareList;
    LinkedList<Vector2D> Q_List = new LinkedList<Vector2D>
      (
       new List<Vector2D>
         {
           new Vector2D(2, 6)
         , new Vector2D(4, 6)
         , new Vector2D(4, 8)
         , new Vector2D(2, 8)
         }
      );

    NullIntersectionTest("7-4", P_List, Q_List);
  }

  [Category("ConvexPolygonTests"), Test]
  public void Intersection37() {
    LinkedList<Vector2D> P_List = SquareList;
    LinkedList<Vector2D> Q_List = new LinkedList<Vector2D>
      (new List<Vector2D> { new Vector2D(0, 6), new Vector2D(4, 6), new Vector2D(2, 8) });

    NullIntersectionTest("7-5", P_List, Q_List);
  }

  [Category("ConvexPolygonTests"), Test]
  public void Intersection38() {
    LinkedList<Vector2D> P_List = SquareList;
    LinkedList<Vector2D> Q_List = new LinkedList<Vector2D>
      (new List<Vector2D> { new Vector2D(0, 6), new Vector2D(6, 6), new Vector2D(4, 8) });

    NullIntersectionTest("7-6", P_List, Q_List);
  }

}
