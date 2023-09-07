using NUnit.Framework;
using static System.Math;

namespace Tests {
  [TestFixture]
  public class SupportFunctionTests {
    #region Data
    private G.GammaPair[] gps1 = new G.GammaPair[] {
      new G.GammaPair(new G.Vector2D(-1, 1), 1), new G.GammaPair(new G.Vector2D(1, -1), 1)
      , new G.GammaPair(new G.Vector2D(-1, 1), 2), new G.GammaPair(new G.Vector2D(-1, -1), 1)
      , new G.GammaPair(new G.Vector2D(1, 1), 1),
    };

    private static double v = Math.Sqrt(2) / 2;

    private G.GammaPair[] gps1_true = new G.GammaPair[] {
      new G.GammaPair(new G.Vector2D(v, v), v), new G.GammaPair(new G.Vector2D(-v, v), v)
      , new G.GammaPair(new G.Vector2D(-v, -v), v), new G.GammaPair(new G.Vector2D(v, -v), v)
    };

    private G.GammaPair[] gps2 = new G.GammaPair[] {
      new G.GammaPair(new G.Vector2D(0, 1), 1), new G.GammaPair(new G.Vector2D(0, -1), 1), new G.GammaPair(new G.Vector2D(-1, 0), 1)
      , new G.GammaPair(new G.Vector2D(1, 0), 1)
    };

    private G.GammaPair[] gps2_true = new G.GammaPair[] {
      new G.GammaPair(new G.Vector2D(1, 0), 1), new G.GammaPair(new G.Vector2D(0, 1), 1), new G.GammaPair(new G.Vector2D(-1, 0), 1)
      , new G.GammaPair(new G.Vector2D(0, -1), 1)
    };
    #endregion

    [Test]
    public void CreateSFTest1() {
      G.SupportFunction sf = new G.SupportFunction(gps1);
      G.GammaPair[] ans = new G.GammaPair[gps1_true.Length];
      gps1_true.CopyTo(ans, 0);
      Array.Sort(ans);

      Assert.Multiple(() => {
        Assert.That(sf, Has.Count.EqualTo(4), "Wrong number of resultant vectors");

        for (int i = 0; i < 4; i++) {
          Assert.That(sf[i], Is.EqualTo(ans[i]), "i = " + i);
        }
      });
    }

    [Test]
    public void CreateSFTest2() {
      G.SupportFunction sf = new G.SupportFunction(gps2);
      G.GammaPair[] ans = new G.GammaPair[gps2_true.Length];
      gps2_true.CopyTo(ans, 0);
      Array.Sort(ans);

      Assert.Multiple(() => {
        Assert.That(sf, Has.Count.EqualTo(4), "Wrong number of resultant vectors");

        for (int i = 0; i < 4; i++) {
          Assert.That(sf[i], Is.EqualTo(ans[i]), "i = " + i);
        }
      });
    }

    [Test]
    public void ComputeSFTest1() {
      G.SupportFunction sf = new G.SupportFunction(gps1);

      G.Vector2D[] vs = new G.Vector2D[] {
        new G.Vector2D(1, 0), new G.Vector2D(1, 0.5), new G.Vector2D(v, v), new G.Vector2D(1, 1), new G.Vector2D(0, 1)
        , new G.Vector2D(v, -v), new G.Vector2D(-1, -1), new G.Vector2D(1, -0.5)
      };
      double[] gs = new double[] { 1, 1, v, 1, 1, v, 1, 1 };

      Assert.Multiple(() => {
        for (int i = 0; i < vs.Length; i++) {
          Assert.That(G.Tools.EQ(sf.FuncVal(vs[i]), gs[i]), "i = " + i);
        }
      });
    }

    [Test]
    public void CrossPairsTest() {
      G.GammaPair
        g1 = new G.GammaPair(new G.Vector2D(1, 0), 1)
        , g2 = new G.GammaPair(new G.Vector2D(1, 0), 2)
        , g3 = new G.GammaPair(new G.Vector2D(0, 1), 1)
        , g4 = new G.GammaPair(new G.Vector2D(1, 1), 3);
      G.Point2D
        p13 = new G.Point2D(1, 1)
        , p14 = new G.Point2D(1, 2)
        , p23 = new G.Point2D(2, 1)
        , p24 = new G.Point2D(2, 1)
        , p34 = new G.Point2D(2, 1)
        , p;

      bool hasException = false;
      try {
        G.GammaPair.CrossPairs(g1, g2);
      }
      catch {
        hasException = true;
      }

      Assert.Multiple(() => {
        Assert.That(hasException, "No exception during crossing parallel lines");

        p = G.GammaPair.CrossPairs(g1, g3);
        Assert.That(p, Is.EqualTo(p13), "Bad crossing g1 and g3");

        p = G.GammaPair.CrossPairs(g3, g1);
        Assert.That(p, Is.EqualTo(p13), "Bad crossing g3 and g1");

        p = G.GammaPair.CrossPairs(g1, g4);
        Assert.That(p, Is.EqualTo(p14), "Bad crossing g1 and g4");

        p = G.GammaPair.CrossPairs(g2, g3);
        Assert.That(p, Is.EqualTo(p23), "Bad crossing g2 and g3");

        p = G.GammaPair.CrossPairs(g2, g3);
        Assert.That(p, Is.EqualTo(p24), "Bad crossing g21 and g3");

        p = G.GammaPair.CrossPairs(g3, g4);
        Assert.That(p, Is.EqualTo(p34), "Bad crossing g3 and g4");
      });
    }

    [Test]
    public void CombineSFTest() {
      G.SupportFunction
        sf1 = new G.SupportFunction(gps1_true)
        , sf2 = new G.SupportFunction(gps2_true)
        , sf11 = G.SupportFunction.CombineFunctions(sf1, sf1, 1, 1)
        , sf22 = G.SupportFunction.CombineFunctions(sf2, sf2, 1, 1)
        , sf12 = G.SupportFunction.CombineFunctions(sf1, sf2, 1, 1);

      int i;

      Assert.Multiple(() => {
        for (i = 0; i < sf11.Count; i++) {
          Assert.That(sf11[i].Normal, Is.EqualTo(sf1[i].Normal), "sf11: " + i + "th normal is wrong");
          Assert.That(G.Tools.EQ(sf11[i].Value, 2 * sf1[i].Value), "sf11: " + i + "th value is wrong");
        }

        for (i = 0; i < sf22.Count; i++) {
          Assert.That(sf22[i].Normal, Is.EqualTo(sf2[i].Normal), "sf22: " + i + "th normal is wrong");
          Assert.That(G.Tools.EQ(sf22[i].Value, 2 * sf2[i].Value), "sf22: " + i + "th value is wrong");
        }

        for (i = 0; i < sf12.Count; i++) {
          if (i % 2 == 0) {
            Assert.That(sf12[i].Normal, Is.EqualTo(sf1[i / 2].Normal), "sf12: " + i + "th normal is wrong");
            Assert.That(G.Tools.EQ(sf12[i].Value, 3 * sf1[i / 2].Value), "sf12: " + i + "th value is wrong");
          } else {
            Assert.That(sf12[i].Normal, Is.EqualTo(sf2[i / 2].Normal), "sf12: " + i + "th normal is wrong");
            Assert.That(G.Tools.EQ(sf12[i].Value, 2 * sf2[i / 2].Value), "sf12: " + i + "th value is wrong");
          }
        }
      });
    }

    [Test]
    public void FindTest1() {
      List<G.Point2D> ps = new List<G.Point2D>() {
        new G.Point2D(-2, -1), new G.Point2D(-1, -2), new G.Point2D(1, -2), new G.Point2D(2, -1), new G.Point2D(2, 1)
        , new G.Point2D(1, 2), new G.Point2D(-1, 2), new G.Point2D(-2, 1)
      };
      G.SupportFunction sf = new G.SupportFunction(ps);
      List<double> testAngles = new List<double>()
        { 0.0, 30.0, 45.0, 60.0, 90.0, 120.0, 135.0, 150, 180, 210, 270.0, 300.0, 315.0, 330.0 };
      int[,] res = new int[,] {
        { 3, 4 }, { 3, 4 }, { 4, 5 }, { 4, 5 }, { 5, 6 }, { 5, 6 }, { 6, 7 }, { 6, 7 }, { 7, 0 }, { 7, 0 }, { 1, 2 }, { 1, 2 }, { 2, 3 }, { 2, 3 }
      };
      List<G.Vector2D> testVecs = testAngles.Select(
        a => {
          double a1 = a * PI / 180;
          return new G.Vector2D(Cos(a1), Sin(a1));
        }
      ).ToList();

      int i, j, k;

      Assert.Multiple(() => {
        for (k = 0; k < testVecs.Count; k++) {
          sf.FindCone(testVecs[k], out i, out j);
          Assert.That(i == res[k, 0] && j == res[k, 1],
            "FindCone1: test #" + k + " failed, angle = " + testAngles[k]);
        }
      });
    }

    [Test]
    public void FindTest2() {
      List<double> vertAngles = new List<double>()
        { 0.0, 60.0, 120.0, 180.0, 240.0, 300.0 };
      List<G.Point2D> ps = vertAngles.Select(
        a => {
          double a1 = a * PI / 180;
          return new G.Point2D(Cos(a1), Sin(a1));
        }
      ).ToList();
      G.SupportFunction sf = new G.SupportFunction(ps);
      List<double> testAngles = new List<double>
        { -30.0, -15.0, 0.0, 15.0, 30.0, 60.0, 90.0, 150.0, 165.0, 180.0, 195.0, 210.0, 255.0, 270.0, 300.0 };
      int[,] res = new int[,] {
        { 2, 3 }, { 2, 3 }, { 2, 3 }, { 2, 3 }, { 3, 4 }, { 3, 4 }, { 4, 5 },
        { 5, 0 }, { 5, 0 }, { 5, 0 }, { 5, 0 }, { 0, 1 }, { 0, 1 }, { 1, 2 }, { 1, 2 }
      };
      List<G.Vector2D> testVecs = testAngles.Select(
        a => {
          double a1 = a * PI / 180;
          return new G.Vector2D(Cos(a1), Sin(a1));
        }
      ).ToList();

      int i, j, k;
      Assert.Multiple(() => {
        for (k = 0; k < testVecs.Count; k++) {
          sf.FindCone(testVecs[k], out i, out j);
          Assert.That(i == res[k, 0] && j == res[k, 1],
            "FindCone2: test #" + k + " failed, angle = " + testAngles[k]);
        }
      });
    }
  }
}
