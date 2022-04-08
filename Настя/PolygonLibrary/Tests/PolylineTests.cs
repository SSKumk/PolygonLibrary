using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using PolygonLibrary.Basics;
using PolygonLibrary.Polygons;

namespace Tests
{
  [TestClass]
  public class PolylineTests
  {
    [TestMethod]
    public void PolylineContainsTest1()
    {
      List<Point2D> ps = new List<Point2D>();
      ps.Add(new Point2D(0, 0));
      ps.Add(new Point2D(1, 0));
      ps.Add(new Point2D(1, 1));
      ps.Add(new Point2D(0, 1));

      Polyline line = new Polyline(ps, PolylineOrientation.Counterclockwise, false, false);

      for (int i = 1; i <= 10; i++)
      {
        double x = 1 - 1 / Math.Pow(2, i);
        Point2D p = new Point2D (x, x);
        Assert.IsTrue(line.ContainsPoint(p));
        Assert.IsTrue(line.ContainsPointInside(p));
      }

      for (int i = 1; i <= 10; i++)
      {
        double x = 1 - 1 / Math.Pow(2, i);
        Point2D p = new Point2D(x, 0.1);
        Assert.IsTrue(line.ContainsPoint(p));
        Assert.IsTrue(line.ContainsPointInside(p));
      }

      for (int i = 1; i <= 10; i++)
      {
        double x = 1 / Math.Pow(2, i);
        Point2D p = new Point2D(x, x);
        Assert.IsTrue(line.ContainsPoint(p));
        Assert.IsTrue(line.ContainsPointInside(p));
      }

      for (int i = 1; i <= 10; i++)
      {
        double x = 1 + 1 / Math.Pow(2, i);
        Point2D p = new Point2D(x, x);
        Assert.IsFalse(line.ContainsPoint(p));
        Assert.IsFalse(line.ContainsPointInside(p));
      }

      for (int i = 1; i <= 10; i++)
      {
        double x = 1 + 1 / Math.Pow(2, i);
        Point2D p = new Point2D(x, 0.1);
        Assert.IsFalse(line.ContainsPoint(p));
        Assert.IsFalse(line.ContainsPointInside(p));
      }

      for (int i = 1; i <= 10; i++)
      {
        double x = -1 / Math.Pow(2, i);
        Point2D p = new Point2D(x, x);
        Assert.IsFalse(line.ContainsPoint(p));
        Assert.IsFalse(line.ContainsPointInside(p));
      }

      for (int i = 0; i < ps.Count; i++)
      {
        Assert.IsTrue(line.ContainsPoint(ps[i]));
        Assert.IsFalse(line.ContainsPointInside(ps[i]));
      }

      Assert.IsTrue(line.ContainsPoint(new Point2D(1, 0.5)));
      Assert.IsFalse(line.ContainsPointInside(new Point2D(1, 0.5)));
      Assert.IsFalse(line.ContainsPoint(new Point2D(1.0000001, 0.5)));
      Assert.IsFalse(line.ContainsPointInside(new Point2D(1.0000001, 0.5)));
    }

    [TestMethod]
    public void PolylineContainsTest2()
    {
      List<Point2D> ps = new List<Point2D>();
      ps.Add(new Point2D(0, 0));
      ps.Add(new Point2D(1, 0));
      ps.Add(new Point2D(1, 1));
      ps.Add(new Point2D(0, 1));
      ps.Add(new Point2D(0.1, 0.5));

      Polyline line = new Polyline(ps, PolylineOrientation.Counterclockwise, false, false);

      for (int i = 1; i <= 10; i++)
      {
        double x = 1 - 1 / Math.Pow(2, i);
        Point2D p = new Point2D(x, x);
        Assert.IsTrue(line.ContainsPoint(p));
        Assert.IsTrue(line.ContainsPointInside(p));
      }

      for (int i = 1; i <= 10; i++)
      {
        double x = 1 - 1 / Math.Pow(2, i);
        Point2D p = new Point2D(x, 0.1);
        Assert.IsTrue(line.ContainsPoint(p));
        Assert.IsTrue(line.ContainsPointInside(p));
      }

      for (int i = 1; i <= 10; i++)
      {
        double x = 1 / Math.Pow(2, i);
        Point2D p = new Point2D(x, x);
        Assert.IsTrue(line.ContainsPoint(p));
        Assert.IsTrue(line.ContainsPointInside(p));
      }

      for (int i = 1; i <= 10; i++)
      {
        double x = 1 / Math.Pow(2, i);
        Point2D p = new Point2D(0.1 + x, 0.5);
        Assert.IsTrue(line.ContainsPoint(p));
        Assert.IsTrue(line.ContainsPointInside(p));
      }

      for (int i = 1; i <= 10; i++)
      {
        double x = 1 + 1 / Math.Pow(2, i);
        Point2D p = new Point2D(x, x);
        Assert.IsFalse(line.ContainsPoint(p));
        Assert.IsFalse(line.ContainsPointInside(p));
      }

      for (int i = 1; i <= 10; i++)
      {
        double x = 1 + 1 / Math.Pow(2, i);
        Point2D p = new Point2D(x, 0.1);
        Assert.IsFalse(line.ContainsPoint(p));
        Assert.IsFalse(line.ContainsPointInside(p));
      }

      for (int i = 1; i <= 10; i++)
      {
        double x = -1 / Math.Pow(2, i);
        Point2D p = new Point2D(x, x);
        Assert.IsFalse(line.ContainsPoint(p));
        Assert.IsFalse(line.ContainsPointInside(p));
      }

      for (int i = 1; i <= 10; i++)
      {
        double x = 1 / Math.Pow(2, i);
        Point2D p = new Point2D(0.1 - x, 0.5);
        Assert.IsFalse(line.ContainsPoint(p));
        Assert.IsFalse(line.ContainsPointInside(p));
      }

      for (int i = 0; i < ps.Count; i++)
      {
        Assert.IsTrue(line.ContainsPoint(ps[i]));
        Assert.IsFalse(line.ContainsPointInside(ps[i]));
      }

      Assert.IsTrue(line.ContainsPoint(new Point2D(0.05, 0.75)));
      Assert.IsFalse(line.ContainsPointInside(new Point2D(0.05, 0.75)));
      Assert.IsTrue(line.ContainsPoint(new Point2D(1, 0.5)));
      Assert.IsFalse(line.ContainsPointInside(new Point2D(1, 0.5)));
      Assert.IsFalse(line.ContainsPoint(new Point2D(1.0000001, 0.5)));
      Assert.IsFalse(line.ContainsPointInside(new Point2D(1.0000001, 0.5)));
    }
  }
}
