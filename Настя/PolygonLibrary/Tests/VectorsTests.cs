using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using PolygonLibrary;
using PolygonLibrary.Basics;
using PolygonLibrary.Toolkit;

namespace ToolsTests
{
  [TestClass]
  public class VectorsTests
  {
    [TestMethod]
    public void AngleTest ()
    {
      double sq3 = Math.Sqrt (3) / 2.0;
      Vector2D[] v = new Vector2D[]
      {
        new Vector2D (1, 0),
        new Vector2D (sq3, 0.5),
        new Vector2D (0.5, sq3),
        new Vector2D (0, 1),
        new Vector2D (-1, 1),
        new Vector2D (-1, 0),
        new Vector2D (-0.5, -sq3),
        new Vector2D (0, -1),
        new Vector2D (0.5, -sq3)
      };
      int[,] p = new int[,]
      {
        {0, 0},
        {1, 1},
        {2, 2},
        {3, 3},
        {4, 4},
        {5, 5},
        {6, 6},
        {7, 7},
        {8, 8},
        {0, 1},
        {0, 2},
        {0, 3}, 
        {0, 4},
        {0, 5},
        {0, 6},
        {0, 7},
        {0, 8},
        {1, 2},
        {1, 3},
        {1, 4},
        {1, 5},
        {1, 6},
        {1, 7},
        {1, 8},
        {3, 4},
        {3, 5}, 
        {3, 6},
        {3, 7},
        {3, 8}
      };
      double[] res = new double[]
      {
        0, 0, 0, 0, 0, 0, 0, 0, 0, 30, 60, 90, 135, 180, 240, 270, 300,
        30, 60, 105, 150, 210, 240, 270,
        45, 90, 150, 180, 210
      };

      for (int i = 0; i < res.Length; i++)
      {
        double r = Vector2D.Angle2PI (v[p[i, 0]], v[p[i, 1]]);
        Assert.IsTrue (Tools.EQ (r, res[i] * Math.PI / 180.0),
          "Direct angle: test #" + i + " has failed");
      }

      for (int i = 9; i < res.Length; i++)
      {
        double r = Vector2D.Angle2PI (v[p[i, 1]], v[p[i, 0]]);
        Assert.IsTrue (Tools.EQ (2*Math.PI - r, res[i] * Math.PI / 180.0),
          "Back angle: test #" + i + " has failed");
      }
    }
  }
}
 