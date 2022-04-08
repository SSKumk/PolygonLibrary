using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

using PolygonLibrary;
using PolygonLibrary.Basics;
using PolygonLibrary.Polygons;
using PolygonLibrary.Toolkit;

namespace ToolsTests
{
  /// <summary>
  ///This is a test class for ToolsTests and is intended
  ///to contain all ToolsTests Unit Tests
  ///</summary>
  [TestClass()]
  public class RectangleParallelTests
  {
    [TestMethod()]
    public void RectParallelTest1()
    {
      Point2D p = new Point2D(1, 2);
      ConvexPolygon r = PolygonTools.RectangleParallel(p, p);
      Assert.IsTrue(r.Contours.Count == 1, "Point rectangle: too many contours");
      Assert.IsTrue(r.Contour.Count == 1, "Point rectangle: too many points in the contour");
      Assert.IsTrue(r.Contour[0].Equals(p), "Point rectangle: wrong point in the contour");
    }

    [TestMethod()]
    public void RectParallelTest2()
    {
      Point2D
        p1 = new Point2D(1, 2),
        p2 = new Point2D(1, 3);
      ConvexPolygon r = PolygonTools.RectangleParallel(p1, p2);
      Assert.IsTrue(r.Contours.Count == 1, "Vertical segment rectangle 1: too many contours");
      Assert.IsTrue(r.Contour.Count == 2, "Vertical segment rectangle 1: wrong number points in the contour");
    }


    [TestMethod()]
    public void RectParallelTest3()
    {
      Point2D
        p1 = new Point2D(1, 2),
        p2 = new Point2D(1, 3);
      ConvexPolygon r = PolygonTools.RectangleParallel(p2, p1);
      Assert.IsTrue(r.Contours.Count == 1, "Vertical segment rectangle 2: too many contours");
      Assert.IsTrue(r.Contour.Count == 2, "Vertical segment rectangle 2: wrong number points in the contour");
    }

    [TestMethod()]
    public void RectParallelTest4()
    {
      Point2D
        p1 = new Point2D(1, 2),
        p2 = new Point2D(5, 2);
      ConvexPolygon r = PolygonTools.RectangleParallel(p1, p2);
      Assert.IsTrue(r.Contours.Count == 1, "Horizontal segment rectangle 1: too many contours");
      Assert.IsTrue(r.Contour.Count == 2, "Horizontal segment rectangle 1: wrong number points in the contour");
    }

    [TestMethod()]
    public void RectParallelTest5()
    {
      Point2D
        p1 = new Point2D(1, 2),
        p2 = new Point2D(5, 2);
      ConvexPolygon r = PolygonTools.RectangleParallel(p2, p1);
      Assert.IsTrue(r.Contours.Count == 1, "Horizontal segment rectangle 2: too many contours");
      Assert.IsTrue(r.Contour.Count == 2, "Horizontal segment rectangle 2: wrong number points in the contour");
    }

    [TestMethod()]
    public void RectParallelTest6()
    {
      Point2D
        p1 = new Point2D(1, 2),
        p2 = new Point2D(5, 3);
      ConvexPolygon r = PolygonTools.RectangleParallel(p1, p2);
      Assert.IsTrue(r.Contours.Count == 1, "LL-RU rectangle 1: too many contours");
      Assert.IsTrue(r.Contour.Count == 4, "LL-RU rectangle 1: wrong number points in the contour");
    }

    [TestMethod()]
    public void RectParallelTest7()
    {
      Point2D
        p1 = new Point2D(1, 2),
        p2 = new Point2D(5, 3);
      ConvexPolygon r = PolygonTools.RectangleParallel(p2, p1);
      Assert.IsTrue(r.Contours.Count == 1, "LL-RU rectangle 2: too many contours");
      Assert.IsTrue(r.Contour.Count == 4, "LL-RU rectangle 2: wrong number points in the contour");
    }

    [TestMethod()]
    public void RectParallelTest8()
    {
      Point2D
        p1 = new Point2D(1, 2),
        p2 = new Point2D(5, -3);
      ConvexPolygon r = PolygonTools.RectangleParallel(p1, p2);
      Assert.IsTrue(r.Contours.Count == 1, "LU-RL rectangle 1: too many contours");
      Assert.IsTrue(r.Contour.Count == 4, "LU-RL rectangle 1: wrong number points in the contour");
    }

    [TestMethod()]
    public void RectParallelTest9()
    {
      Point2D
        p1 = new Point2D(1, 2),
        p2 = new Point2D(5, -3);
      ConvexPolygon r = PolygonTools.RectangleParallel(p2, p1);
      Assert.IsTrue(r.Contours.Count == 1, "LU-RL rectangle 2: too many contours");
      Assert.IsTrue(r.Contour.Count == 4, "LU-RL rectangle 2: wrong number points in the contour");
    }


  }

  [TestClass()]
  public class RectangleTurnedTests
  {
    [TestMethod()]
    public void RectTurnedTest1()
    {
      Point2D p = new Point2D(1, 2);
      for (int i = 0; i <= 6; i++)
      {
        double a = i * Math.PI / 6;
        ConvexPolygon r = PolygonTools.RectangleTurned(p, p, a);
        Assert.IsTrue(r.Contours.Count == 1, "Turned rectangle, alpha = " + a + ", i = " + i + ": too many contours");
        Assert.IsTrue(r.Contour.Count == 1, "Turned rectangle, alpha = " + a + ", i = " + i + ": too many points in the contour");
        Assert.IsTrue(r.Contour[0].Equals(p), "Turned rectangle, alpha = " + a + ", i = " + i + ": wrong point in the contour");
      }
    }

    [TestMethod()]
    public void RectTurnedTest2()
    {
      Point2D
        p1 = new Point2D(1, 2),
        p2 = new Point2D(1, 4);
      int N = 9;
      double a0 = Math.PI / N;

      for (int i = 0; i <= N; i++)
      {
        double a = i * a0, cos = Math.Abs(Math.Cos(a));
        ConvexPolygon r = PolygonTools.RectangleTurned(p1, p2, a);
        Assert.IsTrue(r.Contours.Count == 1, "Turned rectangle, alpha = " + a + ", i = " + i + ", i = " + i + ": too many contours");
        if (Tools.NE(a) && Tools.NE(a, Math.PI / 2) && Tools.NE(a, Math.PI))
        {
          Assert.IsTrue(r.Contour.Count == 4, "Turned rectangle, alpha = " + a + ", i = " + i + ": too many points in the contour");

          int j, k, l;
          for (k = 1; k < r.Contour.Count; k++)
          {
            j = k - 1;
            l = (k + 1) % r.Contour.Count;

            Vector2D
              e1o = r.Contour[k] - r.Contour[j],
              e2o = r.Contour[l] - r.Contour[k],
              e1 = e1o.Normalize(),
              e2 = e2o.Normalize();

            Assert.IsTrue(Tools.EQ(Math.Abs(e1 * Vector2D.E1), cos) ||
                           Tools.EQ(Math.Abs(e1 * Vector2D.E2), cos),
                           "Turned rectangle, alpha = " + a + ", i = " + i + ": the edge " + e1o + " has wrong slope");
            Assert.IsTrue(Tools.EQ(e1 * e2), "Turned rectangle, alpha = " + a + ", i = " + i + ": the edges " + e1o +
              " and " + e2o + " are not orthogonal");
          }

        }
        else
          Assert.IsTrue(r.Contour.Count == 2, "Turned rectangle, alpha = " + a + ", i = " + i + ": too many points in the contour");
      }
    }

    [TestMethod()]
    public void RectTurnedTest3()
    {
      Point2D
        p2 = new Point2D(1, 2),
        p1 = new Point2D(1, 4);
      int N = 9;
      double a0 = Math.PI / N;

      for (int i = 0; i <= N; i++)
      {
        double a = i * a0, cos = Math.Abs(Math.Cos(a));
        ConvexPolygon r = PolygonTools.RectangleTurned(p1, p2, a);
        Assert.IsTrue(r.Contours.Count == 1, "Turned rectangle, alpha = " + a + ", i = " + i + ", i = " + i + ": too many contours");
        if (Tools.NE(a) && Tools.NE(a, Math.PI / 2) && Tools.NE(a, Math.PI))
        {
          Assert.IsTrue(r.Contour.Count == 4, "Turned rectangle, alpha = " + a + ", i = " + i + ": too many points in the contour");

          int j, k, l;
          for (k = 1; k < r.Contour.Count; k++)
          {
            j = k - 1;
            l = (k + 1) % r.Contour.Count;

            Vector2D
              e1o = r.Contour[k] - r.Contour[j],
              e2o = r.Contour[l] - r.Contour[k],
              e1 = e1o.Normalize(),
              e2 = e2o.Normalize();

            Assert.IsTrue(Tools.EQ(Math.Abs(e1 * Vector2D.E1), cos) ||
                           Tools.EQ(Math.Abs(e1 * Vector2D.E2), cos),
                           "Turned rectangle, alpha = " + a + ", i = " + i + ": the edge " + e1o + " has wrong slope");
            Assert.IsTrue(Tools.EQ(e1 * e2), "Turned rectangle, alpha = " + a + ", i = " + i + ": the edges " + e1o +
              " and " + e2o + " are not orthogonal");
          }

        }
        else
          Assert.IsTrue(r.Contour.Count == 2, "Turned rectangle, alpha = " + a + ", i = " + i + ": too many points in the contour");
      }
    }

    [TestMethod()]
    public void RectTurnedTest4()
    {
      Point2D
        p1 = new Point2D(2, 1),
        p2 = new Point2D(4, 1);
      int N = 9;
      double a0 = Math.PI / N;

      for (int i = 0; i <= N; i++)
      {
        double a = i * a0, cos = Math.Abs(Math.Cos(a));
        ConvexPolygon r = PolygonTools.RectangleTurned(p1, p2, a);
        Assert.IsTrue(r.Contours.Count == 1, "Turned rectangle, alpha = " + a + ", i = " + i + ", i = " + i + ": too many contours");
        if (Tools.NE(a) && Tools.NE(a, Math.PI / 2) && Tools.NE(a, Math.PI))
        {
          Assert.IsTrue(r.Contour.Count == 4, "Turned rectangle, alpha = " + a + ", i = " + i + ": too many points in the contour");

          int j, k, l;
          for (k = 1; k < r.Contour.Count; k++)
          {
            j = k - 1;
            l = (k + 1) % r.Contour.Count;

            Vector2D
              e1o = r.Contour[k] - r.Contour[j],
              e2o = r.Contour[l] - r.Contour[k],
              e1 = e1o.Normalize(),
              e2 = e2o.Normalize();

            Assert.IsTrue(Tools.EQ(Math.Abs(e1 * Vector2D.E1), cos) ||
                           Tools.EQ(Math.Abs(e1 * Vector2D.E2), cos),
                           "Turned rectangle, alpha = " + a + ", i = " + i + ": the edge " + e1o + " has wrong slope");
            Assert.IsTrue(Tools.EQ(e1 * e2), "Turned rectangle, alpha = " + a + ", i = " + i + ": the edges " + e1o +
              " and " + e2o + " are not orthogonal");
          }

        }
        else
          Assert.IsTrue(r.Contour.Count == 2, "Turned rectangle, alpha = " + a + ", i = " + i + ": too many points in the contour");
      }
    }

    [TestMethod()]
    public void RectTurnedTest5()
    {
      Point2D
        p2 = new Point2D(2, 1),
        p1 = new Point2D(4, 1);
      int N = 9;
      double a0 = Math.PI / N;

      for (int i = 0; i <= N; i++)
      {
        double a = i * a0, cos = Math.Abs(Math.Cos(a));
        ConvexPolygon r = PolygonTools.RectangleTurned(p1, p2, a);
        Assert.IsTrue(r.Contours.Count == 1, "Turned rectangle, alpha = " + a + ", i = " + i + ", i = " + i + ": too many contours");
        if (Tools.NE(a) && Tools.NE(a, Math.PI / 2) && Tools.NE(a, Math.PI))
        {
          Assert.IsTrue(r.Contour.Count == 4, "Turned rectangle, alpha = " + a + ", i = " + i + ": too many points in the contour");

          int j, k, l;
          for (k = 1; k < r.Contour.Count; k++)
          {
            j = k - 1;
            l = (k + 1) % r.Contour.Count;

            Vector2D
              e1o = r.Contour[k] - r.Contour[j],
              e2o = r.Contour[l] - r.Contour[k],
              e1 = e1o.Normalize(),
              e2 = e2o.Normalize();

            Assert.IsTrue(Tools.EQ(Math.Abs(e1 * Vector2D.E1), cos) ||
                           Tools.EQ(Math.Abs(e1 * Vector2D.E2), cos),
                           "Turned rectangle, alpha = " + a + ", i = " + i + ": the edge " + e1o + " has wrong slope");
            Assert.IsTrue(Tools.EQ(e1 * e2), "Turned rectangle, alpha = " + a + ", i = " + i + ": the edges " + e1o +
              " and " + e2o + " are not orthogonal");
          }

        }
        else
          Assert.IsTrue(r.Contour.Count == 2, "Turned rectangle, alpha = " + a + ", i = " + i + ": too many points in the contour");
      }
    }

    [TestMethod()]
    public void RectTurnedTest6()
    {
      Point2D
        p1 = new Point2D(-2, 0),
        p2 = new Point2D(4, 1);
      int N = 9;
      double a0 = Math.PI / N;

      for (int i = 0; i <= N; i++)
      {
        double a = i * a0, cos = Math.Abs(Math.Cos(a));
        ConvexPolygon r = PolygonTools.RectangleTurned(p1, p2, a);
        Assert.IsTrue(r.Contours.Count == 1, "Turned rectangle, alpha = " + a + ", i = " + i + ", i = " + i + ": too many contours");

        Assert.IsTrue(r.Contour.Count == 4, "Turned rectangle, alpha = " + a + ", i = " + i + ": too many points in the contour");

        int j, k, l;
        for (k = 1; k < r.Contour.Count; k++)
        {
          j = k - 1;
          l = (k + 1) % r.Contour.Count;

          Vector2D
            e1o = r.Contour[k] - r.Contour[j],
            e2o = r.Contour[l] - r.Contour[k],
            e1 = e1o.Normalize(),
            e2 = e2o.Normalize();

          Assert.IsTrue(Tools.EQ(Math.Abs(e1 * Vector2D.E1), cos) ||
                         Tools.EQ(Math.Abs(e1 * Vector2D.E2), cos),
                         "Turned rectangle, alpha = " + a + ", i = " + i + ": the edge " + e1o + " has wrong slope");
          Assert.IsTrue(Tools.EQ(e1 * e2), "Turned rectangle, alpha = " + a + ", i = " + i + ": the edges " + e1o +
            " and " + e2o + " are not orthogonal");
        }
      }
    }

    [TestMethod()]
    public void RectTurnedTest7()
    {
      Point2D
        p2 = new Point2D(-2, 0),
        p1 = new Point2D(4, 1);
      int N = 9;
      double a0 = Math.PI / N;

      for (int i = 0; i <= N; i++)
      {
        double a = i * a0, cos = Math.Abs(Math.Cos(a));
        ConvexPolygon r = PolygonTools.RectangleTurned(p1, p2, a);
        Assert.IsTrue(r.Contours.Count == 1, "Turned rectangle, alpha = " + a + ", i = " + i + ", i = " + i + ": too many contours");

        Assert.IsTrue(r.Contour.Count == 4, "Turned rectangle, alpha = " + a + ", i = " + i + ": too many points in the contour");

        int j, k, l;
        for (k = 1; k < r.Contour.Count; k++)
        {
          j = k - 1;
          l = (k + 1) % r.Contour.Count;

          Vector2D
            e1o = r.Contour[k] - r.Contour[j],
            e2o = r.Contour[l] - r.Contour[k],
            e1 = e1o.Normalize(),
            e2 = e2o.Normalize();

          Assert.IsTrue(Tools.EQ(Math.Abs(e1 * Vector2D.E1), cos) ||
                         Tools.EQ(Math.Abs(e1 * Vector2D.E2), cos),
                         "Turned rectangle, alpha = " + a + ", i = " + i + ": the edge " + e1o + " has wrong slope");
          Assert.IsTrue(Tools.EQ(e1 * e2), "Turned rectangle, alpha = " + a + ", i = " + i + ": the edges " + e1o +
            " and " + e2o + " are not orthogonal");
        }
      }
    }

    [TestMethod()]
    public void RectTurnedTest8()
    {
      Point2D
        p1 = new Point2D(-2, 3),
        p2 = new Point2D(4, 1);
      int N = 9;
      double a0 = Math.PI / N;

      for (int i = 0; i <= N; i++)
      {
        double a = i * a0, cos = Math.Abs(Math.Cos(a));
        ConvexPolygon r = PolygonTools.RectangleTurned(p1, p2, a);
        Assert.IsTrue(r.Contours.Count == 1, "Turned rectangle, alpha = " + a + ", i = " + i + ", i = " + i + ": too many contours");

        Assert.IsTrue(r.Contour.Count == 4, "Turned rectangle, alpha = " + a + ", i = " + i + ": too many points in the contour");

        int j, k, l;
        for (k = 1; k < r.Contour.Count; k++)
        {
          j = k - 1;
          l = (k + 1) % r.Contour.Count;

          Vector2D
            e1o = r.Contour[k] - r.Contour[j],
            e2o = r.Contour[l] - r.Contour[k],
            e1 = e1o.Normalize(),
            e2 = e2o.Normalize();

          Assert.IsTrue(Tools.EQ(Math.Abs(e1 * Vector2D.E1), cos) ||
                         Tools.EQ(Math.Abs(e1 * Vector2D.E2), cos),
                         "Turned rectangle, alpha = " + a + ", i = " + i + ": the edge " + e1o + " has wrong slope");
          Assert.IsTrue(Tools.EQ(e1 * e2), "Turned rectangle, alpha = " + a + ", i = " + i + ": the edges " + e1o +
            " and " + e2o + " are not orthogonal");
        }
      }
    }

    [TestMethod()]
    public void RectTurnedTest9()
    {
      Point2D
        p2 = new Point2D(-2, 3),
        p1 = new Point2D(4, 1);
      int N = 9;
      double a0 = Math.PI / N;

      for (int i = 0; i <= N; i++)
      {
        double a = i * a0, cos = Math.Abs(Math.Cos(a));
        ConvexPolygon r = PolygonTools.RectangleTurned(p1, p2, a);
        Assert.IsTrue(r.Contours.Count == 1, "Turned rectangle, alpha = " + a + ", i = " + i + ", i = " + i + ": too many contours");

        Assert.IsTrue(r.Contour.Count == 4, "Turned rectangle, alpha = " + a + ", i = " + i + ": too many points in the contour");

        int j, k, l;
        for (k = 1; k < r.Contour.Count; k++)
        {
          j = k - 1;
          l = (k + 1) % r.Contour.Count;

          Vector2D
            e1o = r.Contour[k] - r.Contour[j],
            e2o = r.Contour[l] - r.Contour[k],
            e1 = e1o.Normalize(),
            e2 = e2o.Normalize();

          Assert.IsTrue(Tools.EQ(Math.Abs(e1 * Vector2D.E1), cos) ||
                         Tools.EQ(Math.Abs(e1 * Vector2D.E2), cos),
                         "Turned rectangle, alpha = " + a + ", i = " + i + ": the edge " + e1o + " has wrong slope");
          Assert.IsTrue(Tools.EQ(e1 * e2), "Turned rectangle, alpha = " + a + ", i = " + i + ": the edges " + e1o +
            " and " + e2o + " are not orthogonal");
        }
      }
    }
  }
}
