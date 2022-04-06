using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using PolygonLibrary;
using PolygonLibrary.Basics;
using PolygonLibrary.Polygons;
using PolygonLibrary.Toolkit;

namespace Tests
{
  [TestClass]
  public class PolygonExtremeTests
  {
    [TestMethod]
    public void PolygonExtremeTest1()
    {
      // Polygon has no normal codirected with e1
      ConvexPolygon cp = PolygonTools.Circle(0, 0, 1, 10, 0);
      double[] testAnglesDeg = new double[] { 0, 18, 36, 54, 72, 342 };
      Vector2D[] testDirs = new Vector2D[testAnglesDeg.Length];
      Point2D[,] res = new Point2D[testAnglesDeg.Length, 2];
      for (int i = 0; i < testAnglesDeg.Length; i++)
      {
        double alpha = testAnglesDeg[i] * Math.PI / 180;
        testDirs[i] = new Vector2D(5 * Math.Cos(alpha), 5 * Math.Sin(alpha));
        cp.GetExtremeElements(testDirs[i], out res[i, 0], out res[i, 1]);
      }

      Assert.IsTrue(Object.ReferenceEquals(res[0, 1], null), "0");
      Assert.IsTrue(Object.ReferenceEquals(res[2, 1], null), "2");
      Assert.IsTrue(Object.ReferenceEquals(res[4, 1], null), "4");

      Assert.IsFalse(Object.ReferenceEquals(res[1, 1], null), "1");
      Assert.IsFalse(Object.ReferenceEquals(res[3, 1], null), "3");
      Assert.IsFalse(Object.ReferenceEquals(res[5, 1], null), "5");
    }

    [TestMethod]
    public void PolygonExtremeTest2()
    {
      // Polygon has a normal codirected with e1
      ConvexPolygon cp = PolygonTools.Circle(0, 0, 1, 10, Math.PI/10);
      double[] testAnglesDeg = new double[] { 0, 18, 36, 54, 72, 342 };
      Vector2D[] testDirs = new Vector2D[testAnglesDeg.Length];
      Point2D[,] res = new Point2D[testAnglesDeg.Length, 2];
      for (int i = 0; i < testAnglesDeg.Length; i++)
      {
        double alpha = testAnglesDeg[i] * Math.PI / 180;
        testDirs[i] = new Vector2D(5 * Math.Cos(alpha), 5 * Math.Sin(alpha));
        cp.GetExtremeElements(testDirs[i], out res[i, 0], out res[i, 1]);
      }

      Assert.IsTrue(Object.ReferenceEquals(res[1, 1], null), "0");
      Assert.IsTrue(Object.ReferenceEquals(res[3, 1], null), "2");
      Assert.IsTrue(Object.ReferenceEquals(res[5, 1], null), "4");

      Assert.IsFalse(Object.ReferenceEquals(res[0, 1], null), "1");
      Assert.IsFalse(Object.ReferenceEquals(res[2, 1], null), "3");
      Assert.IsFalse(Object.ReferenceEquals(res[4, 1], null), "5");
    }
  }
}
