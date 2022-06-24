using NUnit.Framework;
using PolygonLibrary.Toolkit;
using PolygonLibrary.Basics;

namespace Tests;

[TestFixture]
public class VectorsTests {
  [Test]
  public void AngleTest() {
    double sq3 = Math.Sqrt(3) / 2.0;
    Vector2D[] v = new Vector2D[] {
      new Vector2D(1, 0), new Vector2D(sq3, 0.5), new Vector2D(0.5, sq3), new Vector2D(0, 1), new Vector2D(-1, 1)
      , new Vector2D(-1, 0), new Vector2D(-0.5, -sq3), new Vector2D(0, -1), new Vector2D(0.5, -sq3)
    };
    int[,] p = new int[,] {
      { 0, 0 }, { 1, 1 }, { 2, 2 }, { 3, 3 }, { 4, 4 }, { 5, 5 }, { 6, 6 }, { 7, 7 }, { 8, 8 }, { 0, 1 }, { 0, 2 }
      , { 0, 3 }, { 0, 4 }, { 0, 5 }, { 0, 6 }, { 0, 7 }, { 0, 8 }, { 1, 2 }, { 1, 3 }, { 1, 4 }, { 1, 5 }, { 1, 6 }
      , { 1, 7 }, { 1, 8 }, { 3, 4 }, { 3, 5 }, { 3, 6 }, { 3, 7 }, { 3, 8 }
    };
    double[] res = new double[] {
      0, 0, 0, 0, 0, 0, 0, 0, 0, 30, 60, 90, 135, 180, 240, 270, 300, 30, 60, 105, 150, 210, 240, 270, 45, 90, 150, 180
      , 210
    };

    for (int i = 0; i < res.Length; i++) {
      double r = Vector2D.Angle2PI(v[p[i, 0]], v[p[i, 1]]);
      Assert.That(Tools.EQ(r, res[i] * Math.PI / 180.0),
        "Direct angle: test #" + i + " has failed");
    }

    for (int i = 9; i < res.Length; i++) {
      double r = Vector2D.Angle2PI(v[p[i, 1]], v[p[i, 0]]);
      Assert.That(Tools.EQ(2 * Math.PI - r, res[i] * Math.PI / 180.0),
        "Back angle: test #" + i + " has failed");
    }
  }

  [Test]
  public void IsBetweenTest() {
    double pi = Math.PI;
    Vector2D
      v0 = Vector2D.FromPolar(-pi, 1)
      , v1 = Vector2D.FromPolar(-7 * pi / 8, 1)
      , v2 = Vector2D.FromPolar(-3 * pi / 4, 1)
      , v3 = Vector2D.FromPolar(-pi / 2, 1)
      , v4 = Vector2D.FromPolar(-pi / 4, 1)
      , v5 = Vector2D.FromPolar(0, 1)
      , v6 = Vector2D.FromPolar(pi / 4, 1)
      , v7 = Vector2D.FromPolar(pi / 2, 1)
      , v8 = Vector2D.FromPolar(3 * pi / 4, 1)
      , v9 = Vector2D.FromPolar(7 * pi / 8, 1)
      ;

    Assert.Multiple(() => {
      //===========================================================
      // TRUE tests
      // Group 1: Small cone, the first vector has the polar angle pi, the second has the polar angle less than 0  
      Assert.That(v1.IsBetween(v0, v2), "Group 1: Test #1");
      Assert.That(v1.IsBetween(v0, v3), "Group 1: Test #2");
      Assert.That(v1.IsBetween(v0, v4), "Group 1: Test #3");
      Assert.That(v3.IsBetween(v0, v4), "Group 1: Test #4");

      // Group 2: Small cone, both vectors have the polar angle less than 0  
      Assert.That(v2.IsBetween(v1, v3), "Group 2: Test #1");
      Assert.That(v2.IsBetween(v1, v4), "Group 2: Test #2");
      Assert.That(v2.IsBetween(v1, v5), "Group 2: Test #3");

      // Group 3: Small cone, the first vector has a negative polar angle, the second has a positive polar angle  
      Assert.That(v4.IsBetween(v3, v6), "Group 3: Test #1");
      Assert.That(v5.IsBetween(v3, v6), "Group 3: Test #2");

      // Group 4: Small cone, the first vector has the polar angle 0, the second has a positive polar angle  
      Assert.That(v6.IsBetween(v5, v7), "Group 4: Test #1");
      Assert.That(v6.IsBetween(v5, v8), "Group 4: Test #2");
      Assert.That(v7.IsBetween(v5, v8), "Group 4: Test #3");
      Assert.That(v6.IsBetween(v5, v9), "Group 4: Test #4");
      Assert.That(v7.IsBetween(v5, v9), "Group 4: Test #5");
      Assert.That(v8.IsBetween(v5, v9), "Group 4: Test #6");

      // Group 5: Small cone, both vectors have a positive polar angle   
      Assert.That(v7.IsBetween(v6, v8), "Group 5: Test #1");
      Assert.That(v7.IsBetween(v6, v0), "Group 5: Test #2");
      Assert.That(v8.IsBetween(v7, v9), "Group 5: Test #3");
      Assert.That(v8.IsBetween(v7, v0), "Group 5: Test #4");

      // Group 6: Small cone, the first vector has a positive polar angle, the second has a negative polar angle  
      Assert.That(v9.IsBetween(v7, v2), "Group 6: Test #1");
      Assert.That(v0.IsBetween(v7, v2), "Group 6: Test #2");
      Assert.That(v1.IsBetween(v7, v2), "Group 6: Test #3");
      Assert.That(v9.IsBetween(v8, v3), "Group 6: Test #4");
      Assert.That(v0.IsBetween(v8, v3), "Group 6: Test #5");
      Assert.That(v1.IsBetween(v8, v3), "Group 6: Test #6");
      
      // Flat cone ...
      
      // Large cone ...
      
      //===========================================================
      // FALSE tests
      
      // All tests made above, when the tested vector is a boundary one
      
      // All tests made above, when the tested vector is really outside the cone
    });
  }
}
