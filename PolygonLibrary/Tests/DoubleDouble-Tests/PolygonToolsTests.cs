using DoubleDouble;
using NUnit.Framework;
using static CGLibrary.Geometry<DoubleDouble.ddouble, Tests.DDConvertor>;

namespace Tests.DoubleDouble_Tests; 

/// <summary>
///This is a test class for ToolsTests and is intended
///to contain all ToolsTests Unit Tests
///</summary>
[TestFixture]
public class RectangleParallelTests {

  [Test]
  public void RectParallelTest1() {
    Vector2D       p = new Vector2D(1, 2);
    ConvexPolygon r = PolygonTools.RectangleParallel(p, p);
    Assert.Multiple
      (
       () => {
         Assert.That(r.Contours, Has.Count.EqualTo(1), "Vector rectangle: too many contours");
         Assert.That(r.Contour, Has.Count.EqualTo(1), "Vector rectangle: too many points in the contour");
         Assert.That(r.Contour[0], Is.EqualTo(p), "Vector rectangle: wrong point in the contour");
       }
      );
  }

  [Test]
  public void RectParallelTest2() {
    Vector2D       p1 = new Vector2D(1, 2), p2 = new Vector2D(1, 3);
    ConvexPolygon r  = PolygonTools.RectangleParallel(p1, p2);
    Assert.Multiple
      (
       () => {
         Assert.That(r.Contours, Has.Count.EqualTo(1), "Vertical segment rectangle 1: too many contours");
         Assert.That(r.Contour, Has.Count.EqualTo(2), "Vertical segment rectangle 1: wrong number points in the contour");
       }
      );
  }

  [Test]
  public void RectParallelTest3() {
    Vector2D       p1 = new Vector2D(1, 2), p2 = new Vector2D(1, 3);
    ConvexPolygon r  = PolygonTools.RectangleParallel(p2, p1);
    Assert.Multiple
      (
       () => {
         Assert.That(r.Contours, Has.Count.EqualTo(1), "Vertical segment rectangle 2: too many contours");
         Assert.That(r.Contour, Has.Count.EqualTo(2), "Vertical segment rectangle 2: wrong number points in the contour");
       }
      );
  }

  [Test]
  public void RectParallelTest4() {
    Vector2D       p1 = new Vector2D(1, 2), p2 = new Vector2D(5, 2);
    ConvexPolygon r  = PolygonTools.RectangleParallel(p1, p2);
    Assert.Multiple
      (
       () => {
         Assert.That(r.Contours, Has.Count.EqualTo(1), "Horizontal segment rectangle 1: too many contours");
         Assert.That(r.Contour, Has.Count.EqualTo(2), "Horizontal segment rectangle 1: wrong number points in the contour");
       }
      );
  }

  [Test]
  public void RectParallelTest5() {
    Vector2D       p1 = new Vector2D(1, 2), p2 = new Vector2D(5, 2);
    ConvexPolygon r  = PolygonTools.RectangleParallel(p2, p1);
    Assert.Multiple
      (
       () => {
         Assert.That(r.Contours, Has.Count.EqualTo(1), "Horizontal segment rectangle 2: too many contours");
         Assert.That(r.Contour, Has.Count.EqualTo(2), "Horizontal segment rectangle 2: wrong number points in the contour");
       }
      );
  }

  [Test]
  public void RectParallelTest6() {
    Vector2D       p1 = new Vector2D(1, 2), p2 = new Vector2D(5, 3);
    ConvexPolygon r  = PolygonTools.RectangleParallel(p1, p2);
    Assert.Multiple
      (
       () => {
         Assert.That(r.Contours, Has.Count.EqualTo(1), "LL-RU rectangle 1: too many contours");
         Assert.That(r.Contour, Has.Count.EqualTo(4), "LL-RU rectangle 1: wrong number points in the contour");
       }
      );
  }

  [Test]
  public void RectParallelTest7() {
    Vector2D       p1 = new Vector2D(1, 2), p2 = new Vector2D(5, 3);
    ConvexPolygon r  = PolygonTools.RectangleParallel(p2, p1);
    Assert.Multiple
      (
       () => {
         Assert.That(r.Contours, Has.Count.EqualTo(1), "LL-RU rectangle 2: too many contours");
         Assert.That(r.Contour, Has.Count.EqualTo(4), "LL-RU rectangle 2: wrong number points in the contour");
       }
      );
  }

  [Test]
  public void RectParallelTest8() {
    Vector2D       p1 = new Vector2D(1, 2), p2 = new Vector2D(5, -3);
    ConvexPolygon r  = PolygonTools.RectangleParallel(p1, p2);
    Assert.Multiple
      (
       () => {
         Assert.That(r.Contours, Has.Count.EqualTo(1), "LU-RL rectangle 1: too many contours");
         Assert.That(r.Contour, Has.Count.EqualTo(4), "LU-RL rectangle 1: wrong number points in the contour");
       }
      );
  }

  [Test]
  public void RectParallelTest9() {
    Vector2D       p1 = new Vector2D(1, 2), p2 = new Vector2D(5, -3);
    ConvexPolygon r  = PolygonTools.RectangleParallel(p2, p1);
    Assert.Multiple
      (
       () => {
         Assert.That(r.Contours, Has.Count.EqualTo(1), "LU-RL rectangle 2: too many contours");
         Assert.That(r.Contour, Has.Count.EqualTo(4), "LU-RL rectangle 2: wrong number points in the contour");
       }
      );
  }

}

[TestFixture]
public class RectangleTurnedTests {

  [Test]
  public void RectTurnedTest1() {
    Vector2D p = new Vector2D(1, 2);
    for (int i = 0; i <= 6; i++) {
      ddouble       a = i * Tools.PI / 6;
      ConvexPolygon r = PolygonTools.RectangleTurned(p, p, a);
      Assert.Multiple
        (
         () => {
           Assert.That(r.Contours, Has.Count.EqualTo(1), $"Turned rectangle, alpha = {a}, i = {i}: too many contours");
           Assert.That
             (r.Contour, Has.Count.EqualTo(1), $"Turned rectangle, alpha = {a}, i = {i}: too many points in the contour");
           Assert.That(r.Contour[0], Is.EqualTo(p), $"Turned rectangle, alpha = {a}, i = {i}: wrong point in the contour");
         }
        );
    }
  }

  [Test]
  public void RectTurnedTest2() {
    Vector2D   p1 = new Vector2D(1, 2), p2 = new Vector2D(1, 4);
    const int N  = 9;
    ddouble   a0 = Tools.PI / N;

    for (int i = 0; i <= N; i++) {
      ddouble       a = i * a0, cos = ddouble.Abs(ddouble.Cos(a));
      ConvexPolygon r = PolygonTools.RectangleTurned(p1, p2, a);
      Assert.That(r.Contours, Has.Count.EqualTo(1), $"Turned rectangle, alpha = {a}, i = {i}, i = {i}: too many contours");
      if (Tools.NE(a) && Tools.NE(a, Tools.PI / 2) && Tools.NE(a, Tools.PI)) {
        Assert.That(r.Contour, Has.Count.EqualTo(4), $"Turned rectangle, alpha = {a}, i = {i}: too many points in the contour");

        int j, k, l;
        for (k = 1; k < r.Contour.Count; k++) {
          j = k - 1;
          l = (k + 1) % r.Contour.Count;

          Vector2D e1o = r.Contour[k] - r.Contour[j]
                 , e2o = r.Contour[l] - r.Contour[k]
                 , e1  = e1o.Normalize()
                 , e2  = e2o.Normalize();

          Assert.That
            (
             Tools.EQ(ddouble.Abs(e1 * Vector2D.E1), cos) || Tools.EQ(ddouble.Abs(e1 * Vector2D.E2), cos)
           , $"Turned rectangle, alpha = {a}, i = {i}: the edge " + e1o + " has wrong slope"
            );
          Assert.That
            (
             Tools.EQ(e1 * e2)
           , $"Turned rectangle, alpha = {a}, i = {i}: the edges " + e1o + " and " + e2o + " are not orthogonal"
            );
        }
      } else {
        Assert.That(r.Contour, Has.Count.EqualTo(2), $"Turned rectangle, alpha = {a}, i = {i}: too many points in the contour");
      }
    }
  }

  [Test]
  public void RectTurnedTest3() {
    Vector2D   p2 = new Vector2D(1, 2), p1 = new Vector2D(1, 4);
    const int N  = 9;
    ddouble   a0 = Tools.PI / N;

    for (int i = 0; i <= N; i++) {
      ddouble       a = i * a0, cos = ddouble.Abs(ddouble.Cos(a));
      ConvexPolygon r = PolygonTools.RectangleTurned(p1, p2, a);
      Assert.That(r.Contours, Has.Count.EqualTo(1), $"Turned rectangle, alpha = {a}, i = {i}, i = {i}: too many contours");
      if (Tools.NE(a) && Tools.NE(a, Tools.PI / 2) && Tools.NE(a, Tools.PI)) {
        Assert.That(r.Contour, Has.Count.EqualTo(4), $"Turned rectangle, alpha = {a}, i = {i}: too many points in the contour");

        int j, k, l;
        for (k = 1; k < r.Contour.Count; k++) {
          j = k - 1;
          l = (k + 1) % r.Contour.Count;

          Vector2D e1o = r.Contour[k] - r.Contour[j]
                 , e2o = r.Contour[l] - r.Contour[k]
                 , e1  = e1o.Normalize()
                 , e2  = e2o.Normalize();

          Assert.That
            (
             Tools.EQ(ddouble.Abs(e1 * Vector2D.E1), cos) || Tools.EQ(ddouble.Abs(e1 * Vector2D.E2), cos)
           , $"Turned rectangle, alpha = {a}, i = {i}: the edge " + e1o + " has wrong slope"
            );
          Assert.That
            (
             Tools.EQ(e1 * e2)
           , $"Turned rectangle, alpha = {a}, i = {i}: the edges " + e1o + " and " + e2o + " are not orthogonal"
            );
        }
      } else {
        Assert.That(r.Contour, Has.Count.EqualTo(2), $"Turned rectangle, alpha = {a}, i = {i}: too many points in the contour");
      }
    }
  }

  [Test]
  public void RectTurnedTest4() {
    Vector2D   p1 = new Vector2D(2, 1), p2 = new Vector2D(4, 1);
    const int N  = 9;
    ddouble   a0 = Tools.PI / N;

    for (int i = 0; i <= N; i++) {
      ddouble       a = i * a0, cos = ddouble.Abs(ddouble.Cos(a));
      ConvexPolygon r = PolygonTools.RectangleTurned(p1, p2, a);
      Assert.That(r.Contours, Has.Count.EqualTo(1), $"Turned rectangle, alpha = {a}, i = {i}, i = {i}: too many contours");
      if (Tools.NE(a) && Tools.NE(a, Tools.PI / 2) && Tools.NE(a, Tools.PI)) {
        Assert.That(r.Contour, Has.Count.EqualTo(4), $"Turned rectangle, alpha = {a}, i = {i}: too many points in the contour");

        int j, k, l;
        for (k = 1; k < r.Contour.Count; k++) {
          j = k - 1;
          l = (k + 1) % r.Contour.Count;

          Vector2D e1o = r.Contour[k] - r.Contour[j]
                 , e2o = r.Contour[l] - r.Contour[k]
                 , e1  = e1o.Normalize()
                 , e2  = e2o.Normalize();

          Assert.That
            (
             Tools.EQ(ddouble.Abs(e1 * Vector2D.E1), cos) || Tools.EQ(ddouble.Abs(e1 * Vector2D.E2), cos)
           , $"Turned rectangle, alpha = {a}, i = {i}: the edge " + e1o + " has wrong slope"
            );
          Assert.That
            (
             Tools.EQ(e1 * e2)
           , $"Turned rectangle, alpha = {a}, i = {i}: the edges " + e1o + " and " + e2o + " are not orthogonal"
            );
        }
      } else {
        Assert.That(r.Contour, Has.Count.EqualTo(2), $"Turned rectangle, alpha = {a}, i = {i}: too many points in the contour");
      }
    }
  }

  [Test]
  public void RectTurnedTest5() {
    Vector2D   p2 = new Vector2D(2, 1), p1 = new Vector2D(4, 1);
    const int N  = 9;
    ddouble   a0 = Tools.PI / N;

    for (int i = 0; i <= N; i++) {
      ddouble       a = i * a0, cos = ddouble.Abs(ddouble.Cos(a));
      ConvexPolygon r = PolygonTools.RectangleTurned(p1, p2, a);
      Assert.That(r.Contours, Has.Count.EqualTo(1), $"Turned rectangle, alpha = {a}, i = {i}, i = {i}: too many contours");
      if (Tools.NE(a) && Tools.NE(a, Tools.PI / 2) && Tools.NE(a, Tools.PI)) {
        Assert.That(r.Contour, Has.Count.EqualTo(4), $"Turned rectangle, alpha = {a}, i = {i}: too many points in the contour");

        int j, k, l;
        for (k = 1; k < r.Contour.Count; k++) {
          j = k - 1;
          l = (k + 1) % r.Contour.Count;

          Vector2D e1o = r.Contour[k] - r.Contour[j]
                 , e2o = r.Contour[l] - r.Contour[k]
                 , e1  = e1o.Normalize()
                 , e2  = e2o.Normalize();

          Assert.That
            (
             Tools.EQ(ddouble.Abs(e1 * Vector2D.E1), cos) || Tools.EQ(ddouble.Abs(e1 * Vector2D.E2), cos)
           , $"Turned rectangle, alpha = {a}, i = {i}: the edge " + e1o + " has wrong slope"
            );
          Assert.That
            (
             Tools.EQ(e1 * e2)
           , $"Turned rectangle, alpha = {a}, i = {i}: the edges " + e1o + " and " + e2o + " are not orthogonal"
            );
        }
      } else {
        Assert.That(r.Contour, Has.Count.EqualTo(2), $"Turned rectangle, alpha = {a}, i = {i}: too many points in the contour");
      }
    }
  }

  [Test]
  public void RectTurnedTest6() {
    Vector2D   p1 = new Vector2D(-2, 0), p2 = new Vector2D(4, 1);
    const int N  = 9;
    ddouble   a0 = Tools.PI / N;

    for (int i = 0; i <= N; i++) {
      ddouble       a = i * a0, cos = ddouble.Abs(ddouble.Cos(a));
      ConvexPolygon r = PolygonTools.RectangleTurned(p1, p2, a);
      Assert.Multiple
        (
         () => {
           Assert.That(r.Contours, Has.Count.EqualTo(1), $"Turned rectangle, alpha = {a}, i = {i}, i = {i}: too many contours");
           Assert.That
             (r.Contour, Has.Count.EqualTo(4), $"Turned rectangle, alpha = {a}, i = {i}: too many points in the contour");
         }
        );
      int j, k, l;
      for (k = 1; k < r.Contour.Count; k++) {
        j = k - 1;
        l = (k + 1) % r.Contour.Count;

        Vector2D e1o = r.Contour[k] - r.Contour[j], e2o = r.Contour[l] - r.Contour[k], e1 = e1o.Normalize(), e2 = e2o.Normalize();
        Assert.Multiple
          (
           () => {
             Assert.That
               (
                Tools.EQ(ddouble.Abs(e1 * Vector2D.E1), cos) || Tools.EQ(ddouble.Abs(e1 * Vector2D.E2), cos)
              , $"Turned rectangle, alpha = {a}, i = {i}: the edge " + e1o + " has wrong slope"
               );
             Assert.That
               (
                Tools.EQ(e1 * e2)
              , $"Turned rectangle, alpha = {a}, i = {i}: the edges " + e1o + " and " + e2o + " are not orthogonal"
               );
           }
          );
      }
    }
  }

  [Test]
  public void RectTurnedTest7() {
    Vector2D   p2 = new Vector2D(-2, 0), p1 = new Vector2D(4, 1);
    const int N  = 9;
    ddouble   a0 = Tools.PI / N;

    for (int i = 0; i <= N; i++) {
      ddouble       a = i * a0, cos = ddouble.Abs(ddouble.Cos(a));
      ConvexPolygon r = PolygonTools.RectangleTurned(p1, p2, a);
      Assert.Multiple
        (
         () => {
           Assert.That(r.Contours, Has.Count.EqualTo(1), $"Turned rectangle, alpha = {a}, i = {i}, i = {i}: too many contours");
           Assert.That
             (r.Contour, Has.Count.EqualTo(4), $"Turned rectangle, alpha = {a}, i = {i}: too many points in the contour");
         }
        );
      int j, k, l;
      for (k = 1; k < r.Contour.Count; k++) {
        j = k - 1;
        l = (k + 1) % r.Contour.Count;

        Vector2D e1o = r.Contour[k] - r.Contour[j], e2o = r.Contour[l] - r.Contour[k], e1 = e1o.Normalize(), e2 = e2o.Normalize();
        Assert.Multiple
          (
           () => {
             Assert.That
               (
                Tools.EQ(ddouble.Abs(e1 * Vector2D.E1), cos) || Tools.EQ(ddouble.Abs(e1 * Vector2D.E2), cos)
              , $"Turned rectangle, alpha = {a}, i = {i}: the edge " + e1o + " has wrong slope"
               );
             Assert.That
               (
                Tools.EQ(e1 * e2)
              , $"Turned rectangle, alpha = {a}, i = {i}: the edges " + e1o + " and " + e2o + " are not orthogonal"
               );
           }
          );
      }
    }
  }

  [Test]
  public void RectTurnedTest8() {
    Vector2D   p1 = new Vector2D(-2, 3), p2 = new Vector2D(4, 1);
    const int N  = 9;
    ddouble   a0 = Tools.PI / N;

    for (int i = 0; i <= N; i++) {
      ddouble       a = i * a0, cos = ddouble.Abs(ddouble.Cos(a));
      ConvexPolygon r = PolygonTools.RectangleTurned(p1, p2, a);
      Assert.Multiple
        (
         () => {
           Assert.That(r.Contours, Has.Count.EqualTo(1), $"Turned rectangle, alpha = {a}, i = {i}, i = {i}: too many contours");
           Assert.That
             (r.Contour, Has.Count.EqualTo(4), $"Turned rectangle, alpha = {a}, i = {i}: too many points in the contour");
         }
        );
      int j, k, l;
      for (k = 1; k < r.Contour.Count; k++) {
        j = k - 1;
        l = (k + 1) % r.Contour.Count;

        Vector2D e1o = r.Contour[k] - r.Contour[j], e2o = r.Contour[l] - r.Contour[k], e1 = e1o.Normalize(), e2 = e2o.Normalize();
        Assert.Multiple
          (
           () => {
             Assert.That
               (
                Tools.EQ(ddouble.Abs(e1 * Vector2D.E1), cos) || Tools.EQ(ddouble.Abs(e1 * Vector2D.E2), cos)
              , $"Turned rectangle, alpha = {a}, i = {i}: the edge " + e1o + " has wrong slope"
               );
             Assert.That
               (
                Tools.EQ(e1 * e2)
              , $"Turned rectangle, alpha = {a}, i = {i}: the edges " + e1o + " and " + e2o + " are not orthogonal"
               );
           }
          );
      }
    }
  }

  [Test]
  public void RectTurnedTest9() {
    Vector2D   p2 = new Vector2D(-2, 3), p1 = new Vector2D(4, 1);
    const int N  = 9;
    ddouble   a0 = Tools.PI / N;

    for (int i = 0; i <= N; i++) {
      ddouble       a = i * a0, cos = ddouble.Abs(ddouble.Cos(a));
      ConvexPolygon r = PolygonTools.RectangleTurned(p1, p2, a);
      Assert.Multiple
        (
         () => {
           Assert.That(r.Contours, Has.Count.EqualTo(1), $"Turned rectangle, alpha = {a}, i = {i}, i = {i}: too many contours");
           Assert.That
             (r.Contour, Has.Count.EqualTo(4), $"Turned rectangle, alpha = {a}, i = {i}: too many points in the contour");
         }
        );
      int j, k, l;
      for (k = 1; k < r.Contour.Count; k++) {
        j = k - 1;
        l = (k + 1) % r.Contour.Count;

        Vector2D e1o = r.Contour[k] - r.Contour[j], e2o = r.Contour[l] - r.Contour[k], e1 = e1o.Normalize(), e2 = e2o.Normalize();
        Assert.Multiple
          (
           () => {
             Assert.That
               (
                Tools.EQ(ddouble.Abs(e1 * Vector2D.E1), cos) || Tools.EQ(ddouble.Abs(e1 * Vector2D.E2), cos)
              , $"Turned rectangle, alpha = {a}, i = {i}: the edge " + e1o + " has wrong slope"
               );
             Assert.That
               (
                Tools.EQ(e1 * e2)
              , $"Turned rectangle, alpha = {a}, i = {i}: the edges " + e1o + " and " + e2o + " are not orthogonal"
               );
           }
          );
      }
    }
  }

}