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
      
      //-------------------------------------
      // Small cones
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
      
      //-------------------------------------
      // Flat cones
      // Group 7: Flat cone, the first vector has the polar angle pi, the second has the polar angle 0  
      Assert.That(v1.IsBetween(v0, v5), "Group 7: Test #1");
      Assert.That(v3.IsBetween(v0, v5), "Group 7: Test #2");
      Assert.That(v4.IsBetween(v0, v5), "Group 7: Test #3");

      // Group 8: Flat cone, the first vector has negative polar angle  
      Assert.That(v3.IsBetween(v2, v6), "Group 8: Test #1");
      Assert.That(v4.IsBetween(v2, v6), "Group 8: Test #2");
      Assert.That(v5.IsBetween(v2, v6), "Group 8: Test #3");
      Assert.That(v4.IsBetween(v3, v7), "Group 8: Test #4");
      Assert.That(v5.IsBetween(v3, v7), "Group 8: Test #5");
      Assert.That(v6.IsBetween(v3, v7), "Group 8: Test #6");

      // Group 9: Flat cone, the first vector has the polar angle 0, the second has the polar angle pi  
      Assert.That(v6.IsBetween(v5, v0), "Group 9: Test #1");
      Assert.That(v7.IsBetween(v5, v0), "Group 9: Test #2");
      Assert.That(v9.IsBetween(v5, v0), "Group 9: Test #3");

      // Group 10: Flat cone, the first vector has positive polar angle  
      Assert.That(v7.IsBetween(v6, v2), "Group 10: Test #1");
      Assert.That(v8.IsBetween(v6, v2), "Group 10: Test #2");
      Assert.That(v9.IsBetween(v6, v2), "Group 10: Test #3");
      Assert.That(v0.IsBetween(v6, v2), "Group 10: Test #4");
      Assert.That(v1.IsBetween(v6, v2), "Group 10: Test #5");
      Assert.That(v8.IsBetween(v7, v3), "Group 10: Test #6");
      Assert.That(v9.IsBetween(v7, v3), "Group 10: Test #7");
      Assert.That(v0.IsBetween(v7, v3), "Group 10: Test #8");
      Assert.That(v1.IsBetween(v7, v3), "Group 10: Test #9");
      Assert.That(v2.IsBetween(v7, v3), "Group 10: Test #10");
 
      //-------------------------------------
      // Large cones

      // Group 11: Large cone with angle less than 3*pi/2, the first vector has the polar angle pi  
      Assert.That(v1.IsBetween(v0, v6), "Group 11: Test #1");
      Assert.That(v2.IsBetween(v0, v6), "Group 11: Test #2");
      Assert.That(v3.IsBetween(v0, v6), "Group 11: Test #3");
      Assert.That(v4.IsBetween(v0, v6), "Group 11: Test #4");
      Assert.That(v5.IsBetween(v0, v6), "Group 11: Test #5");

      // Group 12: Large cone with angle less than 3*pi/2, the first vector has negative polar angle   
      Assert.That(v3.IsBetween(v2, v7), "Group 12: Test #1");
      Assert.That(v4.IsBetween(v2, v7), "Group 12: Test #2");
      Assert.That(v5.IsBetween(v2, v7), "Group 12: Test #3");
      Assert.That(v6.IsBetween(v2, v7), "Group 12: Test #4");

      // Group 13: Large cone with angle less than 3*pi/2, the first vector has the polar angle -pi/2   
      Assert.That(v4.IsBetween(v3, v9), "Group 13: Test #1");
      Assert.That(v5.IsBetween(v3, v9), "Group 13: Test #2");
      Assert.That(v6.IsBetween(v3, v9), "Group 13: Test #3");
      Assert.That(v7.IsBetween(v3, v9), "Group 13: Test #4");
      Assert.That(v8.IsBetween(v3, v9), "Group 13: Test #5");

      // Group 14: Large cone with angle less than 3*pi/2, the first vector has the polar angle 0   
      Assert.That(v6.IsBetween(v5, v1), "Group 14: Test #1");
      Assert.That(v7.IsBetween(v5, v1), "Group 14: Test #2");
      Assert.That(v8.IsBetween(v5, v1), "Group 14: Test #3");
      Assert.That(v9.IsBetween(v5, v1), "Group 14: Test #4");
      Assert.That(v0.IsBetween(v5, v1), "Group 14: Test #5");

      // Group 15: Large cone with angle equal to 3*pi/2, the first vector has the polar angle 0   
      Assert.That(v1.IsBetween(v0, v7), "Group 15: Test #1");
      Assert.That(v2.IsBetween(v0, v7), "Group 15: Test #2");
      Assert.That(v3.IsBetween(v0, v7), "Group 15: Test #3");
      Assert.That(v4.IsBetween(v0, v7), "Group 15: Test #4");
      Assert.That(v5.IsBetween(v0, v7), "Group 15: Test #5");
      Assert.That(v6.IsBetween(v0, v7), "Group 15: Test #6");
      
      // Group 16: Large cone with angle equal to 3*pi/2, the first vector has negative polar angle   
      Assert.That(v3.IsBetween(v2, v8), "Group 16: Test #1");
      Assert.That(v4.IsBetween(v2, v8), "Group 16: Test #2");
      Assert.That(v5.IsBetween(v2, v8), "Group 16: Test #3");
      Assert.That(v6.IsBetween(v2, v8), "Group 16: Test #4");
      Assert.That(v7.IsBetween(v2, v8), "Group 16: Test #5");

      // Group 17: Large cone with angle equal to 3*pi/2, the first vector has the polar angle -pi/2   
      Assert.That(v4.IsBetween(v3, v0), "Group 17: Test #1");
      Assert.That(v5.IsBetween(v3, v0), "Group 17: Test #2");
      Assert.That(v6.IsBetween(v3, v0), "Group 17: Test #3");
      Assert.That(v7.IsBetween(v3, v0), "Group 17: Test #4");
      Assert.That(v8.IsBetween(v3, v0), "Group 17: Test #5");
      Assert.That(v9.IsBetween(v3, v0), "Group 17: Test #6");

      // Group 18: Large cone with angle equal to 3*pi/2, the first vector has the polar angle 0   
      Assert.That(v6.IsBetween(v5, v3), "Group 18: Test #1");
      Assert.That(v7.IsBetween(v5, v3), "Group 18: Test #2");
      Assert.That(v8.IsBetween(v5, v3), "Group 18: Test #3");
      Assert.That(v9.IsBetween(v5, v3), "Group 18: Test #4");
      Assert.That(v0.IsBetween(v5, v3), "Group 18: Test #5");
      Assert.That(v1.IsBetween(v5, v3), "Group 18: Test #6");
      Assert.That(v2.IsBetween(v5, v3), "Group 18: Test #7");

      // Group 19: Large cone with angle equal to 3*pi/2, the first vector has positive polar angle   
      Assert.That(v7.IsBetween(v6, v4), "Group 19: Test #1");
      Assert.That(v8.IsBetween(v6, v4), "Group 19: Test #2");
      Assert.That(v9.IsBetween(v6, v4), "Group 19: Test #3");
      Assert.That(v0.IsBetween(v6, v4), "Group 19: Test #4");
      Assert.That(v1.IsBetween(v6, v4), "Group 19: Test #5");
      Assert.That(v2.IsBetween(v6, v4), "Group 19: Test #6");
      Assert.That(v3.IsBetween(v6, v4), "Group 19: Test #7");

      // Group 20: Large cone with angle equal to 3*pi/2, the first vector has the polar angle pi/2   
      Assert.That(v8.IsBetween(v7, v5), "Group 20: Test #1");
      Assert.That(v9.IsBetween(v7, v5), "Group 20: Test #2");
      Assert.That(v0.IsBetween(v7, v5), "Group 20: Test #3");
      Assert.That(v1.IsBetween(v7, v5), "Group 20: Test #4");
      Assert.That(v2.IsBetween(v7, v5), "Group 20: Test #5");
      Assert.That(v3.IsBetween(v7, v5), "Group 20: Test #6");
      Assert.That(v4.IsBetween(v7, v5), "Group 20: Test #7");
      
      // Group 21: Large cone with angle greater than 3*pi/2, the first vector has the polar angle 0   
      Assert.That(v1.IsBetween(v0, v8), "Group 21: Test #1");
      Assert.That(v2.IsBetween(v0, v8), "Group 21: Test #2");
      Assert.That(v3.IsBetween(v0, v8), "Group 21: Test #3");
      Assert.That(v4.IsBetween(v0, v8), "Group 21: Test #4");
      Assert.That(v5.IsBetween(v0, v8), "Group 21: Test #5");
      Assert.That(v6.IsBetween(v0, v8), "Group 21: Test #6");
      Assert.That(v7.IsBetween(v0, v8), "Group 21: Test #7");

      Assert.That(v1.IsBetween(v0, v9), "Group 21: Test #8");
      Assert.That(v2.IsBetween(v0, v9), "Group 21: Test #9");
      Assert.That(v3.IsBetween(v0, v9), "Group 21: Test #10");
      Assert.That(v4.IsBetween(v0, v9), "Group 21: Test #11");
      Assert.That(v5.IsBetween(v0, v9), "Group 21: Test #12");
      Assert.That(v6.IsBetween(v0, v9), "Group 21: Test #13");
      Assert.That(v7.IsBetween(v0, v9), "Group 21: Test #14");
      Assert.That(v8.IsBetween(v0, v9), "Group 21: Test #15");

      // Group 22: Large cone with angle greater than 3*pi/2, the first vector has negative polar angle, the second one has positive   
      Assert.That(v2.IsBetween(v1, v9), "Group 22: Test #1");
      Assert.That(v3.IsBetween(v1, v9), "Group 22: Test #2");
      Assert.That(v4.IsBetween(v1, v9), "Group 22: Test #3");
      Assert.That(v5.IsBetween(v1, v9), "Group 22: Test #4");
      Assert.That(v6.IsBetween(v1, v9), "Group 22: Test #5");
      Assert.That(v7.IsBetween(v1, v9), "Group 22: Test #6");
      Assert.That(v8.IsBetween(v1, v9), "Group 22: Test #7");
      
      // Group 23: Large cone with angle greater than 3*pi/2, the first vector has negative polar angle, the second one has 0   
      Assert.That(v2.IsBetween(v1, v0), "Group 23: Test #1");
      Assert.That(v3.IsBetween(v1, v0), "Group 23: Test #2");
      Assert.That(v4.IsBetween(v1, v0), "Group 23: Test #3");
      Assert.That(v5.IsBetween(v1, v0), "Group 23: Test #4");
      Assert.That(v6.IsBetween(v1, v0), "Group 23: Test #5");
      Assert.That(v7.IsBetween(v1, v0), "Group 23: Test #6");
      Assert.That(v8.IsBetween(v1, v0), "Group 23: Test #7");
      Assert.That(v9.IsBetween(v1, v0), "Group 23: Test #8");

      // Group 24: Large cone with angle greater than 3*pi/2, the first vector has the polar angle -pi/2   
      Assert.That(v4.IsBetween(v3, v2), "Group 24: Test #1");
      Assert.That(v5.IsBetween(v3, v2), "Group 24: Test #2");
      Assert.That(v6.IsBetween(v3, v2), "Group 24: Test #3");
      Assert.That(v7.IsBetween(v3, v2), "Group 24: Test #4");
      Assert.That(v8.IsBetween(v3, v2), "Group 24: Test #5");
      Assert.That(v9.IsBetween(v3, v2), "Group 24: Test #6");
      Assert.That(v0.IsBetween(v3, v2), "Group 24: Test #7");
      Assert.That(v1.IsBetween(v3, v2), "Group 24: Test #8");

      // Group 25: Large cone with angle greater than 3*pi/2, the first vector has the polar angle 0   
      Assert.That(v6.IsBetween(v5, v4), "Group 25: Test #1");
      Assert.That(v7.IsBetween(v5, v4), "Group 25: Test #2");
      Assert.That(v8.IsBetween(v5, v4), "Group 25: Test #3");
      Assert.That(v9.IsBetween(v5, v4), "Group 25: Test #4");
      Assert.That(v0.IsBetween(v5, v4), "Group 25: Test #5");
      Assert.That(v1.IsBetween(v5, v4), "Group 25: Test #6");
      Assert.That(v2.IsBetween(v5, v4), "Group 25: Test #7");
      Assert.That(v3.IsBetween(v5, v4), "Group 25: Test #8");

      // Group 26: Large cone with angle greater than 3*pi/2, both vectors have positive polar angles   
      Assert.That(v9.IsBetween(v8, v6), "Group 26: Test #1");
      Assert.That(v0.IsBetween(v8, v6), "Group 26: Test #2");
      Assert.That(v1.IsBetween(v8, v6), "Group 26: Test #3");
      Assert.That(v2.IsBetween(v8, v6), "Group 26: Test #4");
      Assert.That(v3.IsBetween(v8, v6), "Group 26: Test #5");
      Assert.That(v4.IsBetween(v8, v6), "Group 26: Test #6");
      Assert.That(v5.IsBetween(v8, v6), "Group 26: Test #7");
      
      //===========================================================
      // FALSE tests
      // All tests made above, when the tested vector is a boundary one
      
      //-------------------------------------
      // Small cones
      // Group 101: Small cone, the first vector has the polar angle pi, the second has the polar angle less than 0  
      Assert.False(v0.IsBetween(v0, v2), "Group 101: Test #1");
      Assert.False(v2.IsBetween(v0, v2), "Group 101: Test #2");
      Assert.False(v0.IsBetween(v0, v3), "Group 101: Test #3");
      Assert.False(v3.IsBetween(v0, v3), "Group 101: Test #4");
      Assert.False(v0.IsBetween(v0, v4), "Group 101: Test #5");
      Assert.False(v4.IsBetween(v0, v4), "Group 101: Test #6");

      // Group 102: Small cone, both vectors have the polar angle less than 0  
      Assert.False(v1.IsBetween(v1, v3), "Group 102: Test #1");
      Assert.False(v3.IsBetween(v1, v3), "Group 102: Test #2");
      Assert.False(v1.IsBetween(v1, v4), "Group 102: Test #3");
      Assert.False(v4.IsBetween(v1, v4), "Group 102: Test #4");
      Assert.False(v1.IsBetween(v1, v5), "Group 102: Test #5");
      Assert.False(v5.IsBetween(v1, v5), "Group 102: Test #6");

      // Group 103: Small cone, the first vector has a negative polar angle, the second has a positive polar angle  
      Assert.False(v3.IsBetween(v3, v6), "Group 103: Test #1");
      Assert.False(v6.IsBetween(v3, v6), "Group 103: Test #2");

      // Group 104: Small cone, the first vector has the polar angle 0, the second has a positive polar angle  
      Assert.False(v5.IsBetween(v5, v7), "Group 104: Test #1");
      Assert.False(v7.IsBetween(v5, v7), "Group 104: Test #2");
      Assert.False(v5.IsBetween(v5, v8), "Group 104: Test #3");
      Assert.False(v8.IsBetween(v5, v8), "Group 104: Test #4");
      Assert.False(v5.IsBetween(v5, v9), "Group 104: Test #5");
      Assert.False(v9.IsBetween(v5, v9), "Group 104: Test #6");

      // Group 105: Small cone, both vectors have a positive polar angle   
      Assert.False(v6.IsBetween(v6, v8), "Group 105: Test #1");
      Assert.False(v8.IsBetween(v6, v8), "Group 105: Test #2");
      Assert.False(v6.IsBetween(v6, v0), "Group 105: Test #3");
      Assert.False(v0.IsBetween(v6, v0), "Group 105: Test #4");
      Assert.False(v7.IsBetween(v7, v9), "Group 105: Test #5");
      Assert.False(v9.IsBetween(v7, v9), "Group 105: Test #6");
      Assert.False(v7.IsBetween(v7, v0), "Group 105: Test #7");
      Assert.False(v0.IsBetween(v7, v0), "Group 105: Test #8");

      // Group 106: Small cone, the first vector has a positive polar angle, the second has a negative polar angle  
      Assert.False(v7.IsBetween(v7, v2), "Group 106: Test #1");
      Assert.False(v2.IsBetween(v7, v2), "Group 106: Test #2");
      Assert.False(v8.IsBetween(v8, v3), "Group 106: Test #3");
      Assert.False(v3.IsBetween(v8, v3), "Group 106: Test #4");
      
      //-------------------------------------
      // Flat cones
      // Group 107: Flat cone, the first vector has the polar angle pi, the second has the polar angle 0  
      Assert.False(v0.IsBetween(v0, v5), "Group 107: Test #1");
      Assert.False(v5.IsBetween(v0, v5), "Group 107: Test #2");

      // Group 108: Flat cone, the first vector has negative polar angle  
      Assert.False(v2.IsBetween(v2, v6), "Group 108: Test #1");
      Assert.False(v6.IsBetween(v2, v6), "Group 108: Test #2");
      Assert.False(v3.IsBetween(v3, v7), "Group 108: Test #3");
      Assert.False(v7.IsBetween(v3, v7), "Group 108: Test #4");

      // Group 109: Flat cone, the first vector has the polar angle 0, the second has the polar angle pi  
      Assert.False(v5.IsBetween(v5, v0), "Group 109: Test #1");
      Assert.False(v0.IsBetween(v5, v0), "Group 109: Test #2");

      // Group 110: Flat cone, the first vector has positive polar angle  
      Assert.False(v6.IsBetween(v6, v2), "Group 110: Test #1");
      Assert.False(v2.IsBetween(v6, v2), "Group 110: Test #2");
      Assert.False(v7.IsBetween(v7, v3), "Group 110: Test #3");
      Assert.False(v3.IsBetween(v7, v3), "Group 110: Test #4");
 
      //-------------------------------------
      // Large cones

      // Group 111: Large cone with angle less than 3*pi/2, the first vector has the polar angle pi  
      Assert.False(v0.IsBetween(v0, v6), "Group 111: Test #1");
      Assert.False(v6.IsBetween(v0, v6), "Group 111: Test #2");

      // Group 112: Large cone with angle less than 3*pi/2, the first vector has negative polar angle   
      Assert.False(v2.IsBetween(v2, v7), "Group 112: Test #1");
      Assert.False(v7.IsBetween(v2, v7), "Group 112: Test #2");

      // Group 113: Large cone with angle less than 3*pi/2, the first vector has the polar angle -pi/2   
      Assert.False(v3.IsBetween(v3, v9), "Group 113: Test #1");
      Assert.False(v9.IsBetween(v3, v9), "Group 113: Test #2");

      // Group 114: Large cone with angle less than 3*pi/2, the first vector has the polar angle 0   
      Assert.False(v5.IsBetween(v5, v1), "Group 114: Test #1");
      Assert.False(v1.IsBetween(v5, v1), "Group 114: Test #2");

      // Group 115: Large cone with angle equal to 3*pi/2, the first vector has the polar angle 0   
      Assert.False(v0.IsBetween(v0, v7), "Group 115: Test #1");
      Assert.False(v7.IsBetween(v0, v7), "Group 115: Test #2");
      
      // Group 116: Large cone with angle equal to 3*pi/2, the first vector has negative polar angle   
      Assert.False(v2.IsBetween(v2, v8), "Group 116: Test #1");
      Assert.False(v8.IsBetween(v2, v8), "Group 116: Test #2");

      // Group 117: Large cone with angle equal to 3*pi/2, the first vector has the polar angle -pi/2   
      Assert.False(v3.IsBetween(v3, v0), "Group 117: Test #1");
      Assert.False(v0.IsBetween(v3, v0), "Group 117: Test #2");
      
      // Group 118: Large cone with angle equal to 3*pi/2, the first vector has the polar angle 0   
      Assert.False(v5.IsBetween(v5, v3), "Group 118: Test #1");
      Assert.False(v3.IsBetween(v5, v3), "Group 118: Test #2");

      // Group 119: Large cone with angle equal to 3*pi/2, the first vector has positive polar angle   
      Assert.False(v6.IsBetween(v6, v4), "Group 119: Test #1");
      Assert.False(v4.IsBetween(v6, v4), "Group 119: Test #2");

      // Group 120: Large cone with angle equal to 3*pi/2, the first vector has the polar angle pi/2   
      Assert.False(v7.IsBetween(v7, v5), "Group 120: Test #1");
      Assert.False(v5.IsBetween(v7, v5), "Group 120: Test #2");
      
      // Group 121: Large cone with angle greater than 3*pi/2, the first vector has the polar angle 0   
      Assert.False(v0.IsBetween(v0, v8), "Group 121: Test #1");
      Assert.False(v8.IsBetween(v0, v8), "Group 121: Test #2");
      Assert.False(v0.IsBetween(v0, v9), "Group 121: Test #3");
      Assert.False(v9.IsBetween(v0, v9), "Group 121: Test #4");

      // Group 122: Large cone with angle greater than 3*pi/2, the first vector has negative polar angle, the second one has positive   
      Assert.False(v1.IsBetween(v1, v9), "Group 122: Test #1");
      Assert.False(v9.IsBetween(v1, v9), "Group 122: Test #2");
      
      // Group 123: Large cone with angle greater than 3*pi/2, the first vector has negative polar angle, the second one has 0   
      Assert.False(v1.IsBetween(v1, v0), "Group 123: Test #1");
      Assert.False(v0.IsBetween(v1, v0), "Group 123: Test #2");

      // Group 124: Large cone with angle greater than 3*pi/2, the first vector has the polar angle -pi/2   
      Assert.False(v3.IsBetween(v3, v2), "Group 124: Test #1");
      Assert.False(v2.IsBetween(v3, v2), "Group 124: Test #2");

      // Group 125: Large cone with angle greater than 3*pi/2, the first vector has the polar angle 0   
      Assert.False(v5.IsBetween(v5, v4), "Group 125: Test #1");
      Assert.False(v4.IsBetween(v5, v4), "Group 125: Test #2");

      // Group 126: Large cone with angle greater than 3*pi/2, both vectors have positive polar angles   
      Assert.False(v8.IsBetween(v8, v6), "Group 126: Test #1");
      Assert.False(v6.IsBetween(v8, v6), "Group 126: Test #2");
      
      //===========================================================
      // FALSE tests
      // All tests made above, when the tested vector is really outside the cone

      //-------------------------------------
      // Small cones
      // Group 201: Small cone, the first vector has the polar angle pi, the second has the polar angle less than 0  
      Assert.False(v3.IsBetween(v0, v2), "Group 201: Test #1");
      Assert.False(v4.IsBetween(v0, v2), "Group 201: Test #2");
      Assert.False(v5.IsBetween(v0, v2), "Group 201: Test #3");
      Assert.False(v6.IsBetween(v0, v2), "Group 201: Test #4");
      Assert.False(v7.IsBetween(v0, v2), "Group 201: Test #5");
      Assert.False(v8.IsBetween(v0, v2), "Group 201: Test #6");
      Assert.False(v9.IsBetween(v0, v2), "Group 201: Test #7");
      
      Assert.False(v4.IsBetween(v0, v3), "Group 201: Test #8");
      Assert.False(v5.IsBetween(v0, v3), "Group 201: Test #9");
      Assert.False(v6.IsBetween(v0, v3), "Group 201: Test #10");
      Assert.False(v7.IsBetween(v0, v3), "Group 201: Test #11");
      Assert.False(v8.IsBetween(v0, v3), "Group 201: Test #12");
      Assert.False(v9.IsBetween(v0, v3), "Group 201: Test #13");
      
      Assert.False(v5.IsBetween(v0, v4), "Group 201: Test #15");
      Assert.False(v6.IsBetween(v0, v4), "Group 201: Test #16");
      Assert.False(v7.IsBetween(v0, v4), "Group 201: Test #17");
      Assert.False(v8.IsBetween(v0, v4), "Group 201: Test #18");
      Assert.False(v9.IsBetween(v0, v4), "Group 201: Test #19");
      
      // Group 202: Small cone, both vectors have the polar angle less than 0  
      Assert.False(v4.IsBetween(v1, v3), "Group 202: Test #1");
      Assert.False(v5.IsBetween(v1, v3), "Group 202: Test #2");
      Assert.False(v6.IsBetween(v1, v3), "Group 202: Test #3");
      Assert.False(v7.IsBetween(v1, v3), "Group 202: Test #4");
      Assert.False(v8.IsBetween(v1, v3), "Group 202: Test #5");
      Assert.False(v9.IsBetween(v1, v3), "Group 202: Test #6");
      Assert.False(v0.IsBetween(v1, v3), "Group 202: Test #7");
      
      Assert.False(v5.IsBetween(v1, v4), "Group 202: Test #8");
      Assert.False(v6.IsBetween(v1, v4), "Group 202: Test #9");
      Assert.False(v7.IsBetween(v1, v4), "Group 202: Test #10");
      Assert.False(v8.IsBetween(v1, v4), "Group 202: Test #11");
      Assert.False(v9.IsBetween(v1, v4), "Group 202: Test #12");
      Assert.False(v0.IsBetween(v1, v4), "Group 202: Test #13");
      
      Assert.False(v6.IsBetween(v1, v5), "Group 202: Test #14");
      Assert.False(v7.IsBetween(v1, v5), "Group 202: Test #15");
      Assert.False(v8.IsBetween(v1, v5), "Group 202: Test #16");
      Assert.False(v9.IsBetween(v1, v5), "Group 202: Test #17");
      Assert.False(v0.IsBetween(v1, v5), "Group 202: Test #18");

      // Group 203: Small cone, the first vector has a negative polar angle, the second has a positive polar angle  
      Assert.False(v7.IsBetween(v3, v6), "Group 203: Test #1");
      Assert.False(v8.IsBetween(v3, v6), "Group 203: Test #2");
      Assert.False(v9.IsBetween(v3, v6), "Group 203: Test #3");
      Assert.False(v0.IsBetween(v3, v6), "Group 203: Test #4");
      Assert.False(v1.IsBetween(v3, v6), "Group 203: Test #5");
      Assert.False(v2.IsBetween(v3, v6), "Group 203: Test #6");

      // Group 204: Small cone, the first vector has the polar angle 0, the second has a positive polar angle  
      Assert.False(v8.IsBetween(v5, v7), "Group 204: Test #1");
      Assert.False(v9.IsBetween(v5, v7), "Group 204: Test #2");
      Assert.False(v0.IsBetween(v5, v7), "Group 204: Test #3");
      Assert.False(v1.IsBetween(v5, v7), "Group 204: Test #4");
      Assert.False(v2.IsBetween(v5, v7), "Group 204: Test #5");
      Assert.False(v3.IsBetween(v5, v7), "Group 204: Test #6");
      Assert.False(v4.IsBetween(v5, v7), "Group 204: Test #7");
      
      Assert.False(v9.IsBetween(v5, v8), "Group 204: Test #8");
      Assert.False(v0.IsBetween(v5, v8), "Group 204: Test #9");
      Assert.False(v1.IsBetween(v5, v8), "Group 204: Test #10");
      Assert.False(v2.IsBetween(v5, v8), "Group 204: Test #11");
      Assert.False(v3.IsBetween(v5, v8), "Group 204: Test #12");
      Assert.False(v4.IsBetween(v5, v8), "Group 204: Test #13");

      Assert.False(v0.IsBetween(v5, v9), "Group 204: Test #14");
      Assert.False(v1.IsBetween(v5, v9), "Group 204: Test #15");
      Assert.False(v2.IsBetween(v5, v9), "Group 204: Test #16");
      Assert.False(v3.IsBetween(v5, v9), "Group 204: Test #17");
      Assert.False(v4.IsBetween(v5, v9), "Group 204: Test #18");

      // Group 205: Small cone, both vectors have a positive polar angle   
      Assert.False(v9.IsBetween(v6, v8), "Group 205: Test #1");
      Assert.False(v0.IsBetween(v6, v8), "Group 205: Test #2");
      Assert.False(v1.IsBetween(v6, v8), "Group 205: Test #3");
      Assert.False(v2.IsBetween(v6, v8), "Group 205: Test #4");
      Assert.False(v3.IsBetween(v6, v8), "Group 205: Test #5");
      Assert.False(v4.IsBetween(v6, v8), "Group 205: Test #6");
      Assert.False(v5.IsBetween(v6, v8), "Group 205: Test #7");
      
      Assert.False(v1.IsBetween(v6, v0), "Group 205: Test #8");
      Assert.False(v2.IsBetween(v6, v0), "Group 205: Test #9");
      Assert.False(v3.IsBetween(v6, v0), "Group 205: Test #10");
      Assert.False(v4.IsBetween(v6, v0), "Group 205: Test #11");
      Assert.False(v5.IsBetween(v6, v0), "Group 205: Test #12");
      
      Assert.False(v0.IsBetween(v7, v9), "Group 205: Test #13");
      Assert.False(v1.IsBetween(v7, v9), "Group 205: Test #14");
      Assert.False(v2.IsBetween(v7, v9), "Group 205: Test #15");
      Assert.False(v3.IsBetween(v7, v9), "Group 205: Test #16");
      Assert.False(v4.IsBetween(v7, v9), "Group 205: Test #17");
      Assert.False(v5.IsBetween(v7, v9), "Group 205: Test #18");
      Assert.False(v6.IsBetween(v7, v9), "Group 205: Test #19");
      
      Assert.False(v1.IsBetween(v7, v0), "Group 205: Test #20");
      Assert.False(v2.IsBetween(v7, v0), "Group 205: Test #21");
      Assert.False(v3.IsBetween(v7, v0), "Group 205: Test #22");
      Assert.False(v4.IsBetween(v7, v0), "Group 205: Test #23");
      Assert.False(v5.IsBetween(v7, v0), "Group 205: Test #24");
      Assert.False(v6.IsBetween(v7, v0), "Group 205: Test #25");

      // Group 206: Small cone, the first vector has a positive polar angle, the second has a negative polar angle  
      Assert.False(v3.IsBetween(v7, v2), "Group 206: Test #1");
      Assert.False(v4.IsBetween(v7, v2), "Group 206: Test #2");
      Assert.False(v5.IsBetween(v7, v2), "Group 206: Test #3");
      Assert.False(v6.IsBetween(v7, v2), "Group 206: Test #4");

      Assert.False(v4.IsBetween(v8, v3), "Group 206: Test #5");
      Assert.False(v5.IsBetween(v8, v3), "Group 206: Test #6");
      Assert.False(v6.IsBetween(v8, v3), "Group 206: Test #7");
      Assert.False(v7.IsBetween(v8, v3), "Group 206: Test #8");
      
      //-------------------------------------
      // Flat cones
      // Group 207: Flat cone, the first vector has the polar angle pi, the second has the polar angle 0  
      Assert.False(v6.IsBetween(v0, v5), "Group 207: Test #1");
      Assert.False(v7.IsBetween(v0, v5), "Group 207: Test #2");
      Assert.False(v8.IsBetween(v0, v5), "Group 207: Test #3");
      Assert.False(v9.IsBetween(v0, v5), "Group 207: Test #4");

      // Group 208: Flat cone, the first vector has negative polar angle  
      Assert.False(v7.IsBetween(v2, v6), "Group 208: Test #1");
      Assert.False(v8.IsBetween(v2, v6), "Group 208: Test #2");
      Assert.False(v9.IsBetween(v2, v6), "Group 208: Test #3");
      Assert.False(v0.IsBetween(v2, v6), "Group 208: Test #4");
      Assert.False(v1.IsBetween(v2, v6), "Group 208: Test #5");
      
      Assert.False(v8.IsBetween(v3, v7), "Group 208: Test #6");
      Assert.False(v9.IsBetween(v3, v7), "Group 208: Test #7");
      Assert.False(v0.IsBetween(v3, v7), "Group 208: Test #9");
      Assert.False(v1.IsBetween(v3, v7), "Group 208: Test #10");
      Assert.False(v2.IsBetween(v3, v7), "Group 208: Test #11");

      // Group 209: Flat cone, the first vector has the polar angle 0, the second has the polar angle pi  
      Assert.False(v1.IsBetween(v5, v0), "Group 209: Test #1");
      Assert.False(v2.IsBetween(v5, v0), "Group 209: Test #2");
      Assert.False(v3.IsBetween(v5, v0), "Group 209: Test #3");
      Assert.False(v4.IsBetween(v5, v0), "Group 209: Test #4");

      // Group 210: Flat cone, the first vector has positive polar angle  
      Assert.False(v3.IsBetween(v6, v2), "Group 210: Test #1");
      Assert.False(v4.IsBetween(v6, v2), "Group 210: Test #2");
      Assert.False(v5.IsBetween(v6, v2), "Group 210: Test #3");
      
      Assert.False(v4.IsBetween(v7, v3), "Group 210: Test #6");
      Assert.False(v5.IsBetween(v7, v3), "Group 210: Test #7");
      Assert.False(v6.IsBetween(v7, v3), "Group 210: Test #8");
 
      //-------------------------------------
      // Large cones

      // Group 211: Large cone with angle less than 3*pi/2, the first vector has the polar angle pi  
      Assert.False(v7.IsBetween(v0, v6), "Group 211: Test #1");
      Assert.False(v8.IsBetween(v0, v6), "Group 211: Test #2");
      Assert.False(v9.IsBetween(v0, v6), "Group 211: Test #3");

      // Group 212: Large cone with angle less than 3*pi/2, the first vector has negative polar angle   
      Assert.False(v8.IsBetween(v2, v7), "Group 212: Test #1");
      Assert.False(v9.IsBetween(v2, v7), "Group 212: Test #2");
      Assert.False(v0.IsBetween(v2, v7), "Group 212: Test #3");
      Assert.False(v1.IsBetween(v2, v7), "Group 212: Test #4");

      // Group 213: Large cone with angle less than 3*pi/2, the first vector has the polar angle -pi/2   
      Assert.False(v0.IsBetween(v3, v9), "Group 213: Test #1");
      Assert.False(v1.IsBetween(v3, v9), "Group 213: Test #2");
      Assert.False(v2.IsBetween(v3, v9), "Group 213: Test #3");

      // Group 214: Large cone with angle less than 3*pi/2, the first vector has the polar angle 0   
      Assert.False(v2.IsBetween(v5, v1), "Group 214: Test #1");
      Assert.False(v3.IsBetween(v5, v1), "Group 214: Test #2");
      Assert.False(v4.IsBetween(v5, v1), "Group 214: Test #3");

      // Group 215: Large cone with angle equal to 3*pi/2, the first vector has the polar angle 0   
      Assert.False(v8.IsBetween(v0, v7), "Group 215: Test #1");
      Assert.False(v9.IsBetween(v0, v7), "Group 215: Test #2");
      
      // Group 216: Large cone with angle equal to 3*pi/2, the first vector has negative polar angle   
      Assert.False(v9.IsBetween(v2, v8), "Group 216: Test #1");
      Assert.False(v0.IsBetween(v2, v8), "Group 216: Test #2");
      Assert.False(v1.IsBetween(v2, v8), "Group 216: Test #3");

      // Group 217: Large cone with angle equal to 3*pi/2, the first vector has the polar angle -pi/2   
      Assert.False(v1.IsBetween(v3, v0), "Group 217: Test #1");
      Assert.False(v2.IsBetween(v3, v0), "Group 217: Test #2");

      // Group 218: Large cone with angle equal to 3*pi/2, the first vector has the polar angle 0   
      Assert.False(v4.IsBetween(v5, v3), "Group 218: Test #1");

      // Group 219: Large cone with angle equal to 3*pi/2, the first vector has positive polar angle   
      Assert.False(v5.IsBetween(v6, v4), "Group 219: Test #1");

      // Group 220: Large cone with angle equal to 3*pi/2, the first vector has the polar angle pi/2   
      Assert.False(v6.IsBetween(v7, v5), "Group 220: Test #1");
      
      // Group 221: Large cone with angle greater than 3*pi/2, the first vector has the polar angle 0   
      Assert.False(v9.IsBetween(v0, v8), "Group 221: Test #1");

      // Group 222: Large cone with angle greater than 3*pi/2, the first vector has negative polar angle, the second one has positive   
      Assert.False(v0.IsBetween(v1, v9), "Group 222: Test #1");
      
      // Group 223: Large cone with angle greater than 3*pi/2, both vectors have positive polar angles   
      Assert.False(v7.IsBetween(v8, v6), "Group 223: Test #1");
    });
  }
}
