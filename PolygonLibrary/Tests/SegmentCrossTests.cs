using System.Collections.Generic;
using PolygonLibrary.Basics;
using PolygonLibrary.Segments;

namespace ToolsTests
{
  [TestClass]
  public class SegmentCrossTests
  {
    [TestMethod]
    public void SegmentContainsPointTest()
    {
      double a = 0.531;
      Point2D[] p = new Point2D[]
      {
        /* 0 */ new Point2D (1, 2),
        /* 1 */ new Point2D (5, 4),
        /* 2 */ new Point2D (3, 3),
        /* 3 */ new Point2D (1 + 2*a, 2 + a),
        /* 4 */ new Point2D (1 + 2*a, 2 + a + 1e-8),
        /* 5 */ new Point2D (1 + 2*a, 2 + a + 1e-4),
        /* 6 */ new Point2D (-1, 1),
        /* 7 */ new Point2D (7, 5),
        /* 8 */ new Point2D (5, 5),
        /* 9 */ new Point2D (-2, 5)
      };
      bool[] res = new bool[] { true, true, true, true, true, false, false, false, false, false };
      Segment s = new Segment(p[0], p[1]);

      for (int i = 0; i < 10; i++)
      {
        Assert.AreEqual(s.ContainsPoint(p[i]), res[i], "The ContainsPoint test #" + i + " failed");
      }

    }

    [TestMethod]
    public void CrossingTest()
    {
      double[,] x = new double[,] 
      {
        {0, 0},
        {-5, -2},
        {-3, -1},
        {-1, 0},
        {1, 1},
        {1, 1.5},
        {3, 1.5},
        {2, 2},
        {2, 0},
        {1, 3},
        {3, 0},
        {2, 3},
        {2, 4},
        {4, 6},
        {5, 6},
        {6, 6},
        {5, 3},
        {4, 1},
        {4, 0},
        {5, -1},
        {8, 2},
        {8, 3},
        {7, 4},
        {9, 5},
        {11, 6},
        {2, 1.5},
        {3, 2},
        {2, 1.5}
      };

      List<Point2D> p = new List<Point2D>();
      for (int i = 0; i < x.Length / 2; i++) {
        p.Add(new Point2D(x[i, 0], x[i, 1]));
      }

      int[,] ind = new int[,]
      {
        {4, 16},
        {1, 2},
        {3, 4},
        {5, 6},
        {7, 8},
        {9, 10},
        {11, 21},
        {12, 20},
        {13, 16},
        {14, 19},
        {15, 18},
        {16, 17},
        {16, 23},
        {22, 24},
        {3, 23},
        {3, 25},
        {26, 23},
        {8, 17},
        {25, 26}
      };

      List<Segment> segs = new List<Segment>();

      CrossInfo[] res = new CrossInfo[]
      {
        CrossInfo.NoCrossConst, // fake 
        CrossInfo.NoCrossConst,
        new CrossInfo (CrossType.EndPoint, p[4], null, null, null),
        new CrossInfo (CrossType.SinglePoint, p[27], null, null, null),
        new CrossInfo (CrossType.SinglePoint, p[27], null, null, null),
        new CrossInfo (CrossType.SinglePoint, p[27], null, null, null),
        new CrossInfo (CrossType.EndPoint, p[16], null, null, null),
        new CrossInfo (CrossType.EndPoint, p[16], null, null, null),
        new CrossInfo (CrossType.EndPoint, p[16], null, null, null),
        new CrossInfo (CrossType.EndPoint, p[16], null, null, null),
        new CrossInfo (CrossType.EndPoint, p[16], null, null, null),
        new CrossInfo (CrossType.EndPoint, p[16], null, null, null),
        new CrossInfo (CrossType.EndPoint, p[16], null, null, null),
        CrossInfo.NoCrossConst,
        new CrossInfo (CrossType.Overlap, p[4], p[16], null, null),
        new CrossInfo (CrossType.Overlap, p[4], p[25], null, null),
        new CrossInfo (CrossType.Overlap, p[26], p[16], null, null),
        CrossInfo.NoCrossConst,
        new CrossInfo (CrossType.Overlap, p[25], p[26], null, null)
      };

      // Direct, direct
      for (int i = 0; i < ind.Length / 2; i++) {
        segs.Add(new Segment(p[ind[i, 0]], p[ind[i, 1]]));
      }

      for (int i = 1; i < res.Length; i++)
      {
        CrossInfo info = Segment.Intersect(segs[0], segs[i]);
        Assert.AreEqual(res[i].crossType, info.crossType,
          "(direct, direct), test #" + i + ": different cross types");
        if (info.crossType == CrossType.NoCross)
        {
          Assert.IsNull(info.p, "(direct, direct), test #" + i + ": non-null result point");
          Assert.IsNull(info.p1, "(direct, direct), test #" + i + ": non-null additional point");
        }
        else if (info.crossType == CrossType.SinglePoint || info.crossType == CrossType.EndPoint)
        {
          Assert.IsTrue(res[i].p == info.p,
            "(direct, direct), test #" + i + ": different result points");
          Assert.IsNull(info.p1, "(direct, direct), test #" + i + ": non-null additional point");
        }
        else
        {
          Assert.IsTrue(res[i].p == info.p,
            "(direct, direct), test #" + i + ": different result points");
          Assert.IsTrue(res[i].p1 == info.p1,
            "(direct, direct), test #" + i + ": different additional points");
        }
      }

      // Direct, back
      segs.Clear();
      for (int i = 0; i < ind.Length / 2; i++)
      {
        if (i == 0) {
          segs.Add(new Segment(p[ind[i, 0]], p[ind[i, 1]]));
        } else {
          segs.Add(new Segment(p[ind[i, 1]], p[ind[i, 0]]));
        }
      }
      for (int i = 1; i < res.Length; i++)
      {
        CrossInfo info = Segment.Intersect(segs[0], segs[i]);
        Assert.AreEqual(res[i].crossType, info.crossType,
          "(direct, back), test #" + i + ": different cross types");
        if (info.crossType == CrossType.NoCross)
        {
          Assert.IsNull(info.p, "(direct, back), test #" + i + ": non-null result point");
          Assert.IsNull(info.p1, "(direct, back), test #" + i + ": non-null additional point");
        }
        else if (info.crossType == CrossType.SinglePoint || info.crossType == CrossType.EndPoint)
        {
          Assert.IsTrue(res[i].p == info.p,
            "(direct, back), test #" + i + ": different result points");
          Assert.IsNull(info.p1, "(direct, back), test #" + i + ": non-null additional point");
        }
        else
        {
          Assert.IsTrue(res[i].p == info.p,
            "(direct, back), test #" + i + ": different result points");
          Assert.IsTrue(res[i].p1 == info.p1,
            "(direct, back), test #" + i + ": different additional points");
        }
      }

      // Back, direct
      segs.Clear();
      for (int i = 0; i < ind.Length / 2; i++)
      {
        if (i == 0) {
          segs.Add(new Segment(p[ind[i, 1]], p[ind[i, 0]]));
        } else {
          segs.Add(new Segment(p[ind[i, 0]], p[ind[i, 1]]));
        }
      }
      for (int i = 1; i < res.Length; i++)
      {
        CrossInfo info = Segment.Intersect(segs[0], segs[i]);
        Assert.AreEqual(res[i].crossType, info.crossType,
          "(back, direct), test #" + i + ": different cross types");
        if (info.crossType == CrossType.NoCross)
        {
          Assert.IsNull(info.p, "(back, direct), test #" + i + ": non-null result point");
          Assert.IsNull(info.p1, "(back, direct), test #" + i + ": non-null additional point");
        }
        else if (info.crossType == CrossType.SinglePoint || info.crossType == CrossType.EndPoint)
        {
          Assert.IsTrue(res[i].p == info.p,
            "(back, direct), test #" + i + ": different result points");
          Assert.IsNull(info.p1, "(back, direct), test #" + i + ": non-null additional point");
        }
        else
        {
          Assert.IsTrue(res[i].p == info.p1,
            "(back, direct), test #" + i + ": different result points");
          Assert.IsTrue(res[i].p1 == info.p,
            "(back, direct), test #" + i + ": different additional points");
        }
      }

      // Back, back
      segs.Clear();
      for (int i = 0; i < ind.Length / 2; i++)
      {
        if (i == 0) {
          segs.Add(new Segment(p[ind[i, 1]], p[ind[i, 0]]));
        } else {
          segs.Add(new Segment(p[ind[i, 1]], p[ind[i, 0]]));
        }
      }
      for (int i = 1; i < res.Length; i++)
      {
        CrossInfo info = Segment.Intersect(segs[0], segs[i]);
        Assert.AreEqual(res[i].crossType, info.crossType,
          "(back, back), test #" + i + ": different cross types");
        if (info.crossType == CrossType.NoCross)
        {
          Assert.IsNull(info.p, "(back, back), test #" + i + ": non-null result point");
          Assert.IsNull(info.p1, "(back, back), test #" + i + ": non-null additional point");
        }
        else if (info.crossType == CrossType.SinglePoint || info.crossType == CrossType.EndPoint)
        {
          Assert.IsTrue(res[i].p == info.p,
            "(back, back), test #" + i + ": different result points");
          Assert.IsNull(info.p1, "(back, back), test #" + i + ": non-null additional point");
        }
        else
        {
          Assert.IsTrue(res[i].p == info.p1,
            "(back, back), test #" + i + ": different result points");
          Assert.IsTrue(res[i].p1 == info.p,
            "(back, back), test #" + i + ": different additional points");
        }
      }

    }
  }
}
