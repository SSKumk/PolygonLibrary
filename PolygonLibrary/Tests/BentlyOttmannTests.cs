using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PolygonLibrary.Basics;
using PolygonLibrary.Segments;
using PolygonLibrary.Toolkit;

namespace ToolsTests
{
  [TestClass]
  public class BentlyOttmannTests
  {
    //[TestMethod]
    //public void BO_YStruct_GroupRange_1()
    //{

    //}

    [TestMethod]
    public void BO1Test()
    {
      List<Segment> segs = new List<Segment>();

      int[] numberOfSegs;
      IEnumerator intEn;

      SegmentCrosser1 crosser;
      SortedSet<SegmentPair> neighs = new SortedSet<SegmentPair>();

      segs.Add(new Segment(6, 3, 0, 0));
      segs.Add(new Segment(2.5, 0, 1, 3));
      crosser = new SegmentCrosser1(segs, neighs);
      Assert.IsTrue(crosser.crossPoints.Count <= 1, "Test #01: too many crossing points!");
      Assert.IsTrue(crosser.crossPoints.Count >= 1, "Test #01: too few crossing points!");
      numberOfSegs = new int[] { 2 };
      intEn = numberOfSegs.GetEnumerator();
      foreach (KeyValuePair<Point2D, SortedSet<int>> pair in crosser.crossPoints)
      {
        intEn.MoveNext();
        Assert.IsTrue(pair.Value.Count == (int)intEn.Current,
          "Test #01: bad number of segments at the point " + pair.Key + ", should be " + (int)intEn.Current + ", actual " + pair.Value.Count);
      }

      segs.Add(new Segment(2, -2, 5, 4));
      crosser = new SegmentCrosser1(segs, neighs);
      Assert.IsTrue(crosser.crossPoints.Count <= 2, "Test #02: too many crossing points!");
      Assert.IsTrue(crosser.crossPoints.Count >= 2, "Test #02: too few crossing points!");
      numberOfSegs = new int[] { 2, 2 };
      intEn = numberOfSegs.GetEnumerator();
      foreach (KeyValuePair<Point2D, SortedSet<int>> pair in crosser.crossPoints)
      {
        intEn.MoveNext();
        Assert.IsTrue(pair.Value.Count == (int)intEn.Current,
          "Test #02: bad number of segments at the point " + pair.Key + ", should be " + (int)intEn.Current + ", actual " + pair.Value.Count);
      }

      segs.Add(new Segment(-1, -2, -1, 5));
      crosser = new SegmentCrosser1(segs, neighs);
      Assert.IsTrue(crosser.crossPoints.Count <= 2, "Test #03: too many crossing points!");
      Assert.IsTrue(crosser.crossPoints.Count >= 2, "Test #03: too few crossing points!");
      numberOfSegs = new int[] { 2, 2 };
      intEn = numberOfSegs.GetEnumerator();
      foreach (KeyValuePair<Point2D, SortedSet<int>> pair in crosser.crossPoints)
      {
        intEn.MoveNext();
        Assert.IsTrue(pair.Value.Count == (int)intEn.Current,
          "Test #03: bad number of segments at the point " + pair.Key + ", should be " + (int)intEn.Current + ", actual " + pair.Value.Count);
      }

      segs.Add(new Segment(-1, -3, -1, -1));
      crosser = new SegmentCrosser1(segs, neighs);
      Assert.IsTrue(crosser.crossPoints.Count <= 4, "Test #04: too many crossing points!");
      Assert.IsTrue(crosser.crossPoints.Count >= 4, "Test #04: too few crossing points!");
      numberOfSegs = new int[] { 2, 2, 2, 2 };
      intEn = numberOfSegs.GetEnumerator();
      foreach (KeyValuePair<Point2D, SortedSet<int>> pair in crosser.crossPoints)
      {
        intEn.MoveNext();
        Assert.IsTrue(pair.Value.Count == (int)intEn.Current,
          "Test #04: bad number of segments at the point " + pair.Key + ", should be " + (int)intEn.Current + ", actual " + pair.Value.Count);
      }

      segs.Add(new Segment(-1, 1, -1, 2));
      crosser = new SegmentCrosser1(segs, neighs);
      Assert.IsTrue(crosser.crossPoints.Count <= 6, "Test #05: too many crossing points!");
      Assert.IsTrue(crosser.crossPoints.Count >= 6, "Test #05: too few crossing points!");
      numberOfSegs = new int[] { 2, 2, 2, 2, 2, 2 };
      intEn = numberOfSegs.GetEnumerator();
      foreach (KeyValuePair<Point2D, SortedSet<int>> pair in crosser.crossPoints)
      {
        intEn.MoveNext();
        Assert.IsTrue(pair.Value.Count == (int)intEn.Current,
          "Test #05: bad number of segments at the point " + pair.Key + ", should be " + (int)intEn.Current + ", actual " + pair.Value.Count);
      }

      segs.Add(new Segment(-1, 2, -1, 3));
      crosser = new SegmentCrosser1(segs, neighs);
      Assert.IsTrue(crosser.crossPoints.Count <= 7, "Test #06: too many crossing points!");
      Assert.IsTrue(crosser.crossPoints.Count >= 7, "Test #06: too few crossing points!");
      numberOfSegs = new int[] { 2, 2, 2, 3, 2, 2, 2 };
      intEn = numberOfSegs.GetEnumerator();
      foreach (KeyValuePair<Point2D, SortedSet<int>> pair in crosser.crossPoints)
      {
        intEn.MoveNext();
        Assert.IsTrue(pair.Value.Count == (int)intEn.Current,
          "Test #06: bad number of segments at the point " + pair.Key + ", should be " + (int)intEn.Current + ", actual " + pair.Value.Count);
      }

      neighs.Add(new SegmentPair(segs[segs.Count - 1], segs[segs.Count - 2]));
      crosser = new SegmentCrosser1(segs, neighs);
      Assert.IsTrue(crosser.crossPoints.Count <= 7, "Test #07: too many crossing points!");
      Assert.IsTrue(crosser.crossPoints.Count >= 7, "Test #07: too few crossing points!");
      numberOfSegs = new int[] { 2, 2, 2, 3, 2, 2, 2 };
      intEn = numberOfSegs.GetEnumerator();
      foreach (KeyValuePair<Point2D, SortedSet<int>> pair in crosser.crossPoints)
      {
        intEn.MoveNext();
        Assert.IsTrue(pair.Value.Count == (int)intEn.Current,
          "Test #07: bad number of segments at the point " + pair.Key + ", should be " + (int)intEn.Current + ", actual " + pair.Value.Count);
      }

      segs.Add(new Segment(-2, 1, 0, 1));
      crosser = new SegmentCrosser1(segs, neighs);
      Assert.IsTrue(crosser.crossPoints.Count <= 7, "Test #08: too many crossing points!");
      Assert.IsTrue(crosser.crossPoints.Count >= 7, "Test #08: too few crossing points!");
      numberOfSegs = new int[] { 2, 2, 3, 3, 2, 2, 2 };
      intEn = numberOfSegs.GetEnumerator();
      foreach (KeyValuePair<Point2D, SortedSet<int>> pair in crosser.crossPoints)
      {
        intEn.MoveNext();
        Assert.IsTrue(pair.Value.Count == (int)intEn.Current,
          "Test #08: bad number of segments at the point " + pair.Key + ", should be " + (int)intEn.Current + ", actual " + pair.Value.Count);
      }

      segs.Add(new Segment(0, -1, 0, 2));
      segs.Add(new Segment(0, -2, 0, 0.5));
      crosser = new SegmentCrosser1(segs, neighs);
      Assert.IsTrue(crosser.crossPoints.Count <= 11, "Test #09: too many crossing points!");
      Assert.IsTrue(crosser.crossPoints.Count >= 11, "Test #09: too few crossing points!");
      numberOfSegs = new int[] { 2, 2, 3, 3, 2, 2, 3, 2, 2, 2, 2 };
      intEn = numberOfSegs.GetEnumerator();
      foreach (KeyValuePair<Point2D, SortedSet<int>> pair in crosser.crossPoints)
      {
        intEn.MoveNext();
        Assert.IsTrue(pair.Value.Count == (int)intEn.Current,
          "Test #09: bad number of segments at the point " + pair.Key + ", should be " + (int)intEn.Current + ", actual " + pair.Value.Count);
      }

      segs.Add(new Segment(2, -1, 2, 3));
      crosser = new SegmentCrosser1(segs, neighs);
      Assert.IsTrue(crosser.crossPoints.Count <= 11, "Test #10: too many crossing points!");
      Assert.IsTrue(crosser.crossPoints.Count >= 11, "Test #10: too few crossing points!");
      numberOfSegs = new int[] { 2, 2, 3, 3, 2, 2, 3, 2, 2, 3, 2 };
      intEn = numberOfSegs.GetEnumerator();
      foreach (KeyValuePair<Point2D, SortedSet<int>> pair in crosser.crossPoints)
      {
        intEn.MoveNext();
        Assert.IsTrue(pair.Value.Count == (int)intEn.Current,
          "Test #10: bad number of segments at the point " + pair.Key + ", should be " + (int)intEn.Current + ", actual " + pair.Value.Count);
      }

      segs.Add(new Segment(2, -1, 2, 1));
      segs.Add(new Segment(2, 1, 2, 3));
      neighs.Add(new SegmentPair(segs[segs.Count - 1], segs[segs.Count - 2]));
      neighs.Add(new SegmentPair(segs[segs.Count - 1], segs[segs.Count - 3]));
      neighs.Add(new SegmentPair(segs[segs.Count - 2], segs[segs.Count - 3]));
      crosser = new SegmentCrosser1(segs, neighs);
      Assert.IsTrue(crosser.crossPoints.Count <= 11, "Test #11: too many crossing points!");
      Assert.IsTrue(crosser.crossPoints.Count >= 11, "Test #11: too few crossing points!");
      numberOfSegs = new int[] { 2, 2, 3, 3, 2, 2, 3, 2, 2, 5, 2 };
      intEn = numberOfSegs.GetEnumerator();
      foreach (KeyValuePair<Point2D, SortedSet<int>> pair in crosser.crossPoints)
      {
        intEn.MoveNext();
        Assert.IsTrue(pair.Value.Count == (int)intEn.Current,
          "Test #11: bad number of segments at the point " + pair.Key + ", should be " + (int)intEn.Current + ", actual " + pair.Value.Count);
      }

      segs.Add(new Segment(3, 2, 4, 2));
      crosser = new SegmentCrosser1(segs, neighs);
      Assert.IsTrue(crosser.crossPoints.Count <= 11, "Test #12: too many crossing points!");
      Assert.IsTrue(crosser.crossPoints.Count >= 11, "Test #12: too few crossing points!");
      numberOfSegs = new int[] { 2, 2, 3, 3, 2, 2, 3, 2, 2, 5, 3 };
      intEn = numberOfSegs.GetEnumerator();
      foreach (KeyValuePair<Point2D, SortedSet<int>> pair in crosser.crossPoints)
      {
        intEn.MoveNext();
        Assert.IsTrue(pair.Value.Count == (int)intEn.Current,
          "Test #12: bad number of segments at the point " + pair.Key + ", should be " + (int)intEn.Current + ", actual " + pair.Value.Count);
      }

      segs.Add(new Segment(4, 2, 5, 5));
      neighs.Add(new SegmentPair(segs[segs.Count - 1], segs[segs.Count - 2]));
      crosser = new SegmentCrosser1(segs, neighs);
      Assert.IsTrue(crosser.crossPoints.Count <= 11, "Test #13: too many crossing points!");
      Assert.IsTrue(crosser.crossPoints.Count >= 11, "Test #13: too few crossing points!");
      numberOfSegs = new int[] { 2, 2, 3, 3, 2, 2, 3, 2, 2, 5, 4 };
      intEn = numberOfSegs.GetEnumerator();
      foreach (KeyValuePair<Point2D, SortedSet<int>> pair in crosser.crossPoints)
      {
        intEn.MoveNext();
        Assert.IsTrue(pair.Value.Count == (int)intEn.Current,
          "Test #13: bad number of segments at the point " + pair.Key + ", should be " + (int)intEn.Current + ", actual " + pair.Value.Count);
      }

      segs.Add(new Segment(4, 2, 7, 5));
      segs.Add(new Segment(4, 2, 6, 0));
      neighs.Add(new SegmentPair(segs[segs.Count - 1], segs[segs.Count - 2]));
      crosser = new SegmentCrosser1(segs, neighs);
      Assert.IsTrue(crosser.crossPoints.Count <= 11, "Test #14: too many crossing points!");
      Assert.IsTrue(crosser.crossPoints.Count >= 11, "Test #14: too few crossing points!");
      numberOfSegs = new int[] { 2, 2, 3, 3, 2, 2, 3, 2, 2, 5, 6 };
      intEn = numberOfSegs.GetEnumerator();
      foreach (KeyValuePair<Point2D, SortedSet<int>> pair in crosser.crossPoints)
      {
        intEn.MoveNext();
        Assert.IsTrue(pair.Value.Count == (int)intEn.Current,
          "Test #14: bad number of segments at the point " + pair.Key + ", should be " + (int)intEn.Current + ", actual " + pair.Value.Count);
      }

      segs.Add(new Segment(4, 2, 6, 0));
      crosser = new SegmentCrosser1(segs, neighs);
      Assert.IsTrue(crosser.crossPoints.Count <= 12, "Test #15: too many crossing points!");
      Assert.IsTrue(crosser.crossPoints.Count >= 12, "Test #15: too few crossing points!");
      numberOfSegs = new int[] { 2, 2, 3, 3, 2, 2, 3, 2, 2, 5, 7, 2 };
      intEn = numberOfSegs.GetEnumerator();
      foreach (KeyValuePair<Point2D, SortedSet<int>> pair in crosser.crossPoints)
      {
        intEn.MoveNext();
        Assert.IsTrue(pair.Value.Count == (int)intEn.Current,
          "Test #15: bad number of segments at the point " + pair.Key + ", should be " + (int)intEn.Current + ", actual " + pair.Value.Count);
      }

      segs.Clear();
      neighs.Clear();
      segs.Add(new Segment(0, 0, 2, 0));
      segs.Add(new Segment(0, 0, 2, 0));
      crosser = new SegmentCrosser1(segs, neighs);
      Assert.IsTrue(crosser.crossPoints.Count <= 2, "Test #100: too many crossing points!");
      Assert.IsTrue(crosser.crossPoints.Count >= 2, "Test #100: too few crossing points!");
      numberOfSegs = new int[] { 2, 2 };
      intEn = numberOfSegs.GetEnumerator();
      foreach (KeyValuePair<Point2D, SortedSet<int>> pair in crosser.crossPoints)
      {
        intEn.MoveNext();
        Assert.IsTrue(pair.Value.Count == (int)intEn.Current,
          "Test #100: bad number of segments at the point " + pair.Key + ", should be " + (int)intEn.Current + ", actual " + pair.Value.Count);
      }

      segs.Add(new Segment(0, 1, 2, -1));
      segs.Add(new Segment(0, 1, 2, -1));
      crosser = new SegmentCrosser1(segs, neighs);
      Assert.IsTrue(crosser.crossPoints.Count <= 5, "Test #101: too many crossing points!");
      Assert.IsTrue(crosser.crossPoints.Count >= 5, "Test #101: too few crossing points!");
      numberOfSegs = new int[] { 2, 2, 4, 2, 2 };
      intEn = numberOfSegs.GetEnumerator();
      foreach (KeyValuePair<Point2D, SortedSet<int>> pair in crosser.crossPoints)
      {
        intEn.MoveNext();
        Assert.IsTrue(pair.Value.Count == (int)intEn.Current,
          "Test #101: bad number of segments at the point " + pair.Key + ", should be " + (int)intEn.Current + ", actual " + pair.Value.Count);
      }

      segs.Add(new Segment(0, -1, 1, 0));
      segs.Add(new Segment(0, -1, 1, 0));
      crosser = new SegmentCrosser1(segs, neighs);
      Assert.IsTrue(crosser.crossPoints.Count <= 6, "Test #102: too many crossing points!");
      Assert.IsTrue(crosser.crossPoints.Count >= 6, "Test #102: too few crossing points!");
      numberOfSegs = new int[] { 2, 2, 2, 6, 2, 2 };
      intEn = numberOfSegs.GetEnumerator();
      foreach (KeyValuePair<Point2D, SortedSet<int>> pair in crosser.crossPoints)
      {
        intEn.MoveNext();
        Assert.IsTrue(pair.Value.Count == (int)intEn.Current,
          "Test #102: bad number of segments at the point " + pair.Key + ", should be " + (int)intEn.Current + ", actual " + pair.Value.Count);
      }

      segs.Add(new Segment(1, 0, 2, 1));
      segs.Add(new Segment(1, 0, 2, 1));
      crosser = new SegmentCrosser1(segs, neighs);
      Assert.IsTrue(crosser.crossPoints.Count <= 7, "Test #103: too many crossing points!");
      Assert.IsTrue(crosser.crossPoints.Count >= 7, "Test #103: too few crossing points!");
      numberOfSegs = new int[] { 2, 2, 2, 8, 2, 2, 2 };
      intEn = numberOfSegs.GetEnumerator();
      foreach (KeyValuePair<Point2D, SortedSet<int>> pair in crosser.crossPoints)
      {
        intEn.MoveNext();
        Assert.IsTrue(pair.Value.Count == (int)intEn.Current,
          "Test #103: bad number of segments at the point " + pair.Key + ", should be " + (int)intEn.Current + ", actual " + pair.Value.Count);
      }

      segs.Add(new Segment(0, 0, 1, 0));
      segs.Add(new Segment(0, 0, 1, 0));
      crosser = new SegmentCrosser1(segs, neighs);
      Assert.IsTrue(crosser.crossPoints.Count <= 7, "Test #104: too many crossing points!");
      Assert.IsTrue(crosser.crossPoints.Count >= 7, "Test #104: too few crossing points!");
      numberOfSegs = new int[] { 2, 4, 2, 10, 2, 2, 2 };
      intEn = numberOfSegs.GetEnumerator();
      foreach (KeyValuePair<Point2D, SortedSet<int>> pair in crosser.crossPoints)
      {
        intEn.MoveNext();
        Assert.IsTrue(pair.Value.Count == (int)intEn.Current,
          "Test #104: bad number of segments at the point " + pair.Key + ", should be " + (int)intEn.Current + ", actual " + pair.Value.Count);
      }

      segs.Add(new Segment(1, 0, 2, 0));
      segs.Add(new Segment(1, 0, 2, 0));
      crosser = new SegmentCrosser1(segs, neighs);
      Assert.IsTrue(crosser.crossPoints.Count <= 7, "Test #105: too many crossing points!");
      Assert.IsTrue(crosser.crossPoints.Count >= 7, "Test #105: too few crossing points!");
      numberOfSegs = new int[] { 2, 4, 2, 12, 2, 4, 2 };
      intEn = numberOfSegs.GetEnumerator();
      foreach (KeyValuePair<Point2D, SortedSet<int>> pair in crosser.crossPoints)
      {
        intEn.MoveNext();
        Assert.IsTrue(pair.Value.Count == (int)intEn.Current,
          "Test #105: bad number of segments at the point " + pair.Key + ", should be " + (int)intEn.Current + ", actual " + pair.Value.Count);
      }

      segs.Add(new Segment(1, -1, 1, 1));
      segs.Add(new Segment(1, -1, 1, 1));
      segs.Add(new Segment(1, -1, 1, 1));
      crosser = new SegmentCrosser1(segs, neighs);
      Assert.IsTrue(crosser.crossPoints.Count <= 9, "Test #106: too many crossing points!");
      Assert.IsTrue(crosser.crossPoints.Count >= 9, "Test #106: too few crossing points!");
      numberOfSegs = new int[] { 2, 4, 2, 3, 15, 3, 2, 4, 2 };
      intEn = numberOfSegs.GetEnumerator();
      foreach (KeyValuePair<Point2D, SortedSet<int>> pair in crosser.crossPoints)
      {
        intEn.MoveNext();
        Assert.IsTrue(pair.Value.Count == (int)intEn.Current,
          "Test #106: bad number of segments at the point " + pair.Key + ", should be " + (int)intEn.Current + ", actual " + pair.Value.Count);
      }

      segs.Add(new Segment(0, -2, 0, 1));
      crosser = new SegmentCrosser1(segs, neighs);
      Assert.IsTrue(crosser.crossPoints.Count <= 9, "Test #107: too many crossing points!");
      Assert.IsTrue(crosser.crossPoints.Count >= 9, "Test #107: too few crossing points!");
      numberOfSegs = new int[] { 3, 5, 3, 3, 15, 3, 2, 4, 2 };
      intEn = numberOfSegs.GetEnumerator();
      foreach (KeyValuePair<Point2D, SortedSet<int>> pair in crosser.crossPoints)
      {
        intEn.MoveNext();
        Assert.IsTrue(pair.Value.Count == (int)intEn.Current,
          "Test #107: bad number of segments at the point " + pair.Key + ", should be " + (int)intEn.Current + ", actual " + pair.Value.Count);
      }

      segs.Add(new Segment(1, 0, 2, -2));
      crosser = new SegmentCrosser1(segs, neighs);
      Assert.IsTrue(crosser.crossPoints.Count <= 9, "Test #108: too many crossing points!");
      Assert.IsTrue(crosser.crossPoints.Count >= 9, "Test #108: too few crossing points!");
      numberOfSegs = new int[] { 3, 5, 3, 3, 16, 3, 2, 4, 2 };
      intEn = numberOfSegs.GetEnumerator();
      foreach (KeyValuePair<Point2D, SortedSet<int>> pair in crosser.crossPoints)
      {
        intEn.MoveNext();
        Assert.IsTrue(pair.Value.Count == (int)intEn.Current,
          "Test #108: bad number of segments at the point " + pair.Key + ", should be " + (int)intEn.Current + ", actual " + pair.Value.Count);
      }

      segs.Add(new Segment(1, -2, 1, 0));
      crosser = new SegmentCrosser1(segs, neighs);
      Assert.IsTrue(crosser.crossPoints.Count <= 9, "Test #109: too many crossing points!");
      Assert.IsTrue(crosser.crossPoints.Count >= 9, "Test #109: too few crossing points!");
      numberOfSegs = new int[] { 3, 5, 3, 4, 17, 3, 2, 4, 2 };
      intEn = numberOfSegs.GetEnumerator();
      foreach (KeyValuePair<Point2D, SortedSet<int>> pair in crosser.crossPoints)
      {
        intEn.MoveNext();
        Assert.IsTrue(pair.Value.Count == (int)intEn.Current,
          "Test #109: bad number of segments at the point " + pair.Key + ", should be " + (int)intEn.Current + ", actual " + pair.Value.Count);
      }

      segs.Add(new Segment(1, 2, 1, 0));
      crosser = new SegmentCrosser1(segs, neighs);
      Assert.IsTrue(crosser.crossPoints.Count <= 9, "Test #110: too many crossing points!");
      Assert.IsTrue(crosser.crossPoints.Count >= 9, "Test #110: too few crossing points!");
      numberOfSegs = new int[] { 3, 5, 3, 4, 18, 4, 2, 4, 2 };
      intEn = numberOfSegs.GetEnumerator();
      foreach (KeyValuePair<Point2D, SortedSet<int>> pair in crosser.crossPoints)
      {
        intEn.MoveNext();
        Assert.IsTrue(pair.Value.Count == (int)intEn.Current,
          "Test #110: bad number of segments at the point " + pair.Key + ", should be " + (int)intEn.Current + ", actual " + pair.Value.Count);
      }

      segs.Add(new Segment(1.5, 2, -0.5, -2));
      crosser = new SegmentCrosser1(segs, neighs);
      Assert.IsTrue(crosser.crossPoints.Count <= 11, "Test #111: too many crossing points!");
      Assert.IsTrue(crosser.crossPoints.Count >= 11, "Test #111: too few crossing points!");
      numberOfSegs = new int[] { 4, 5, 3, 5, 3, 4, 18, 5, 2, 4, 2 };
      intEn = numberOfSegs.GetEnumerator();
      foreach (KeyValuePair<Point2D, SortedSet<int>> pair in crosser.crossPoints)
      {
        intEn.MoveNext();
        Assert.IsTrue(pair.Value.Count == (int)intEn.Current,
          "Test #111: bad number of segments at the point " + pair.Key + ", should be " + (int)intEn.Current + ", actual " + pair.Value.Count);
      }
    }

    [TestMethod]
    public void BO2Test()
    {
      List<Segment> segs = new List<Segment>();
      SortedSet<SegmentPair> neighs = new SortedSet<SegmentPair>();
      SegmentCrosser2 crosser;

      int[] numberOfCrosses;
      IEnumerator intEn;

      double[][] crossCoords;
      IEnumerator dblEn;

      segs.Add(new Segment(0, 0, 6, 3));
      segs.Add(new Segment(3, -2, 1, 4));
      segs.Add(new Segment(6, 3, 0, 0));
      neighs.Add(new SegmentPair(segs[0], segs[2]));

      crosser = new SegmentCrosser2(segs, neighs);
      numberOfCrosses = new int[] { 1, 2, 1 };
      crossCoords = new double[][] { 
        new double[] { 0.33333333 },
        new double[] { 0.5, 0.5 },
        new double[] { 0.666666666 }
      };
      intEn = numberOfCrosses.GetEnumerator();
      for (int k = 0; k < crosser.crosses.Count; k++)
      {
        intEn.MoveNext();
        Assert.IsTrue(crosser.crosses[k].Count == (int)intEn.Current,
          "Test #01: wrong number of crossings with the segment " + segs[k]);

        dblEn = crossCoords[k].GetEnumerator();
        foreach (SegmentCrosser2.CrossData cd in crosser.crosses[k])
        {
          dblEn.MoveNext();
          Assert.IsTrue(Tools.EQ(cd.Part, (double)dblEn.Current),
            "Test #01: bad crossing coordinate: actual: " + cd.Part + ", should be " + (double)dblEn.Current);
        }
      }

      segs.Add(new Segment(0, 2.5, 2, 2.5));
      segs.Add(new Segment(2, 2.5, 2, 1));

      crosser = new SegmentCrosser2(segs, neighs);
      numberOfCrosses = new int[] { 2, 4, 2, 2, 4 };
      crossCoords = new double[][] { 
        /* s0 */ new double[] { 0.333333333 /* s4 */, 0.333333333 /* s2 */ },
        /* s1 */ new double[] { 0.5 /* s0 */, 0.5 /* s4 */, 0.5 /* s2 */, 0.75 /* s3 */ },
        /* s2 */ new double[] { 0.666666666 /* s2 */, 0.666666666 /* s4 */ },
        /* s3 */ new double[] { 0.75 /* s2 */, 1 /* s4 */ },
        /* s4 */ new double[] { 0 /* s3 */, 1 /* s0 */, 1 /* s2 */, 1 /* s2 */ }
      };
      intEn = numberOfCrosses.GetEnumerator();
      for (int k = 0; k < crosser.crosses.Count; k++)
      {
        intEn.MoveNext();
        Assert.IsTrue(crosser.crosses[k].Count == (int)intEn.Current,
          "Test #02: wrong number of crossings with the segment " + segs[k] + ", should be " + (int)intEn.Current +
          ", actual " + crosser.crosses[k].Count);

        dblEn = crossCoords[k].GetEnumerator();
        foreach (SegmentCrosser2.CrossData cd in crosser.crosses[k])
        {
          dblEn.MoveNext();
          Assert.IsTrue(Tools.EQ(cd.Part, (double)dblEn.Current),
            "Test #02: bad crossing coordinate: actual: " + cd.Part + ", should be " + (double)dblEn.Current);
        }
      }

      segs.Add(new Segment(1, 2.5, -2, 2.5));

      crosser = new SegmentCrosser2(segs, neighs);
      numberOfCrosses = new int[] { 2, 4, 2, 4, 4, 2 };
      crossCoords = new double[][] { 
        /* s0 */ new double[] { 0.333333333 /* s4 */, 0.333333333 /* s2 */ },
        /* s1 */ new double[] { 0.5 /* s0 */, 0.5 /* s4 */, 0.5 /* s2 */, 0.75 /* s3 */ },
        /* s2 */ new double[] { 0.666666666 /* s2 */, 0.666666666 /* s4 */ },
        /* s3 */ new double[] { 0 /* s5 */, 0.5 /* s5 */, 0.75 /* s2 */, 1 /* s4 */ },
        /* s4 */ new double[] { 0 /* s3 */, 1 /* s0 */, 1 /* s2 */, 1 /* s2 */ },
        /* s5 */ new double[] { 0 /* s3 */, 0.3333333333 /* s3 */ }
      };
      intEn = numberOfCrosses.GetEnumerator();
      for (int k = 0; k < crosser.crosses.Count; k++)
      {
        intEn.MoveNext();
        Assert.IsTrue(crosser.crosses[k].Count == (int)intEn.Current,
          "Test #03: wrong number of crossings with the segment " + segs[k] + ", should be " + (int)intEn.Current +
          ", actual " + crosser.crosses[k].Count);

        dblEn = crossCoords[k].GetEnumerator();
        foreach (SegmentCrosser2.CrossData cd in crosser.crosses[k])
        {
          dblEn.MoveNext();
          Assert.IsTrue(Tools.EQ(cd.Part, (double)dblEn.Current),
            "Test #03: bad crossing coordinate: actual: " + cd.Part + ", should be " + (double)dblEn.Current);
        }
      }
    }

    [TestMethod]
    public void BO3Test()
    {
      List<Segment> segs = new List<Segment>();

      SegmentCrosser1 crosser;
      SortedSet<SegmentPair> neighs = new SortedSet<SegmentPair>();

      segs.Add(new Segment(2, 2, 0, 0));
      segs.Add(new Segment(1, 1, 3, 1));
      segs.Add(new Segment(1, 1, 1, 3));
      segs.Add(new Segment(0, -10, 2, -8));
      segs.Add(new Segment(0, 10, 2, 12));
      crosser = new SegmentCrosser1(segs, neighs);

      Assert.IsFalse(crosser.crossPoints.Count > 1, "Test #1: too many crossing points");
      Assert.IsFalse(crosser.crossPoints.Count < 1, "Test #1: too few crossing points");
      Assert.IsFalse(crosser.crossPoints.Min().Value.Count < 3, "Test #01: too few segments passing through the point");
      Assert.IsFalse(crosser.crossPoints.Min().Value.Count > 3, "Test #01: too many segments passing through the point");

      segs.Clear();
    }
  }
}
