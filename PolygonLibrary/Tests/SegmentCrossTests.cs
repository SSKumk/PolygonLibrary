using NUnit.Framework;
using CGLibrary;
using static CGLibrary.Geometry<double, DConvertor>;

namespace Tests;

[TestFixture]
public class SegmentCrossTests {

  [Test]
  public void SegmentContainsPointTest() {
    double a = 0.531;
    Point2D[] p = new Point2D[]
      { /* 0 */ new Point2D(1, 2)
      , /* 1 */ new Point2D(5, 4)
      , /* 2 */ new Point2D(3, 3)
      , /* 3 */ new Point2D(1 + 2 * a, 2 + a)
      , /* 4 */ new Point2D(1 + 2 * a, 2 + a + 1e-8)
      , /* 5 */ new Point2D(1 + 2 * a, 2 + a + 1e-4)
      , /* 6 */ new Point2D(-1, 1)
      , /* 7 */ new Point2D(7, 5)
      , /* 8 */ new Point2D(5, 5)
      , /* 9 */ new Point2D(-2, 5)
      };
    bool[]    res = new bool[] { true, true, true, true, true, false, false, false, false, false };
    Segment s   = new Segment(p[0], p[1]);

    for (int i = 0; i < 10; i++) {
      Assert.That(res[i], Is.EqualTo(s.ContainsPoint(p[i])), "The ContainsPoint test #" + i + " failed");
    }
  }

  // [Test]
  // public void CrossingTest() {
  //   double[,] x = new double[,]
  //     { /* 00 */ { 0, 0 }
  //     , /* 01 */ { -5, -2 }
  //     , /* 02 */ { -3, -1 }
  //     , /* 03 */ { -1, 0 }
  //     , /* 04 */ { 1, 1 }
  //     , /* 05 */ { 1, 1.5 }
  //     , /* 06 */ { 3, 1.5 }
  //     , /* 07 */ { 2, 2 }
  //     , /* 08 */ { 2, 0 }
  //     , /* 09 */ { 0, 4.5 }
  //     , /* 10 */ { 3, 0 }
  //     , /* 11 */ { 2, 3 }
  //     , /* 12 */ { -1, 5 }
  //     , /* 13 */ { 4, 6 }
  //     , /* 14 */ { 5, 6 }
  //     , /* 15 */ { 6.5, 7.5 }
  //     , /* 16 */ { 5, 3 }
  //     , /* 17 */ { 4, 1 }
  //     , /* 18 */ { 4, 0 }
  //     , /* 19 */ { 5, -3 }
  //     , /* 20 */ { 8, 2 }
  //     , /* 21 */ { 8, 3 }
  //     , /* 22 */ { 7, 4 }
  //     , /* 23 */ { 9, 5 }
  //     , /* 24 */ { 11, 6 }
  //     , /* 25 */ { 2, 1.5 }
  //     , /* 26 */ { 3, 2 }
  //     };
  //
  //   List<Point2D> p = new List<Point2D>();
  //   for (int i = 0; i < x.Length / 2; i++) {
  //     p.Add(new Point2D(x[i, 0], x[i, 1]));
  //   }
  //
  //   int[,] ind = new int[,]
  //     { /* s00 */ { 4, 16 }
  //     , /* s01 */ { 1, 2 }
  //     , /* s02 */ { 3, 4 }
  //     , /* s03 */ { 5, 6 }
  //     , /* s04 */ { 7, 8 }
  //     , /* s05 */ { 9, 10 }
  //     , /* s06 */ { 11, 21 }
  //     , /* s07 */ { 12, 20 }
  //     , /* s08 */ { 13, 16 }
  //     , /* s09 */ { 14, 19 }
  //     , /* s10 */ { 15, 18 }
  //     , /* s11 */ { 16, 17 }
  //     , /* s12 */ { 16, 23 }
  //     , /* s13 */ { 22, 24 }
  //     , /* s14 */ { 3, 23 }
  //     , /* s15 */ { 3, 25 }
  //     , /* s16 */ { 26, 23 }
  //     , /* s17 */ { 8, 17 }
  //     , /* s18 */ { 25, 26 }
  //     };
  //
  //   List<Segment> segments = new List<Segment>();
  //
  //   CrossInfo[] res = new CrossInfo[]
  //     { /* s00 */ new CrossInfo(CrossType.NoCross, null, null, null, null, IntersectPointPos.Empty
  //       , IntersectPointPos.Empty, IntersectPointPos.Empty, IntersectPointPos.Empty) // fake
  //     , /* s01 */ new CrossInfo(CrossType.NoCross, null, null, null, null, IntersectPointPos.Empty
  //       , IntersectPointPos.Empty, IntersectPointPos.Empty, IntersectPointPos.Empty)
  //     , /* s02 */ new CrossInfo(CrossType.SinglePoint, p[4], null, null, null)
  //     , /* s03 */ new CrossInfo(CrossType.SinglePoint, p[27], null, null, null)
  //     , /* s04 */ new CrossInfo(CrossType.SinglePoint, p[27], null, null, null)
  //     , /* s05 */ new CrossInfo(CrossType.SinglePoint, p[27], null, null, null)
  //     , /* s06 */ new CrossInfo(CrossType.SinglePoint, p[16], null, null, null)
  //     , /* s07 */ new CrossInfo(CrossType.SinglePoint, p[16], null, null, null)
  //     , /* s08 */ new CrossInfo(CrossType.SinglePoint, p[16], null, null, null)
  //     , /* s09 */ new CrossInfo(CrossType.SinglePoint, p[16], null, null, null)
  //     , /* s10 */ new CrossInfo(CrossType.SinglePoint, p[16], null, null, null)
  //     , /* s11 */ new CrossInfo(CrossType.SinglePoint, p[16], null, null, null)
  //     , /* s12 */ new CrossInfo(CrossType.SinglePoint, p[16], null, null, null)
  //     , /* s13 */ new CrossInfo(CrossType.NoCross, null, null, null, null)
  //     , /* s14 */ new CrossInfo(CrossType.Overlap, p[4], p[16], null, null)
  //     , /* s15 */ new CrossInfo(CrossType.Overlap, p[4], p[25], null, null)
  //     , /* s16 */ new CrossInfo(CrossType.Overlap, p[26], p[16], null, null)
  //     , /* s17 */ new CrossInfo(CrossType.NoCross, null, null, null, null)
  //     , /* s18 */ new CrossInfo(CrossType.Overlap, p[25], p[26], null, null)
  //     };
  //
  //   // Direct, direct
  //   for (int i = 0; i < ind.Length / 2; i++) {
  //     segments.Add(new Segment(p[ind[i, 0]], p[ind[i, 1]]));
  //   }
  //
  //   for (int i = 1; i < res.Length; i++) {
  //     CrossInfo info = Segment.Intersect(segments[0], segments[i]);
  //     Assert.That(info.crossType, Is.EqualTo(res[i].crossType)
  //     , "(direct, direct), test #" + i + ": different cross types");
  //     if (info.crossType == CrossType.NoCross) {
  //       Assert.IsNull(info.fp, "(direct, direct), test #" + i + ": non-null result point");
  //       Assert.IsNull(info.sp, "(direct, direct), test #" + i + ": non-null additional point");
  //     } else if (info.crossType == CrossType.SinglePoint || info.crossType == CrossType.SinglePoint) {
  //       Assert.IsTrue(res[i].fp == info.fp, "(direct, direct), test #" + i + ": different result points");
  //       Assert.IsNull(info.sp, "(direct, direct), test #" + i + ": non-null additional point");
  //     } else {
  //       Assert.That(res[i].fp, Is.EqualTo(info.fp), "(direct, direct), test #" + i + ": different result points");
  //       Assert.That(res[i].sp, Is.EqualTo(info.sp), "(direct, direct), test #" + i + ": different additional points");
  //     }
  //   }
  //
  //   // Direct, back
  //   segments.Clear();
  //   for (int i = 0; i < ind.Length / 2; i++) {
  //     if (i == 0) {
  //       segments.Add(new Segment(p[ind[i, 0]], p[ind[i, 1]]));
  //     } else {
  //       segments.Add(new Segment(p[ind[i, 1]], p[ind[i, 0]]));
  //     }
  //   }
  //
  //   for (int i = 1; i < res.Length; i++) {
  //     CrossInfo info = Segment.Intersect(segments[0], segments[i]);
  //     Assert.That(info.crossType, Is.EqualTo(res[i].crossType)
  //     , "(direct, back), test #" + i + ": different cross types");
  //     if (info.crossType == CrossType.NoCross) {
  //       Assert.That(info.fp, Is.Null, "(direct, back), test #" + i + ": non-null result point");
  //       Assert.That(info.sp, Is.Null, "(direct, back), test #" + i + ": non-null additional point");
  //     } else if (info.crossType == CrossType.SinglePoint || info.crossType == CrossType.SinglePoint) {
  //       Assert.IsTrue(res[i].fp == info.fp, "(direct, back), test #" + i + ": different result points");
  //       Assert.IsNull(info.sp, "(direct, back), test #" + i + ": non-null additional point");
  //     } else {
  //       Assert.IsTrue(res[i].fp == info.fp, "(direct, back), test #" + i + ": different result points");
  //       Assert.IsTrue(res[i].sp == info.sp, "(direct, back), test #" + i + ": different additional points");
  //     }
  //   }
  //
  //   // Back, direct
  //   segments.Clear();
  //   for (int i = 0; i < ind.Length / 2; i++) {
  //     if (i == 0) {
  //       segments.Add(new Segment(p[ind[i, 1]], p[ind[i, 0]]));
  //     } else {
  //       segments.Add(new Segment(p[ind[i, 0]], p[ind[i, 1]]));
  //     }
  //   }
  //
  //   for (int i = 1; i < res.Length; i++) {
  //     CrossInfo info = Segment.Intersect(segments[0], segments[i]);
  //     Assert.That(info.crossType, Is.EqualTo(res[i].crossType)
  //     , "(back, direct), test #" + i + ": different cross types");
  //     if (info.crossType == CrossType.NoCross) {
  //       Assert.That(info.fp, Is.Null, "(back, direct), test #" + i + ": non-null result point");
  //       Assert.That(info.sp, Is.Null, "(back, direct), test #" + i + ": non-null additional point");
  //     } else if (info.crossType == CrossType.SinglePoint || info.crossType == CrossType.SinglePoint) {
  //       Assert.IsTrue(res[i].fp == info.fp, "(back, direct), test #" + i + ": different result points");
  //       Assert.IsNull(info.sp, "(back, direct), test #" + i + ": non-null additional point");
  //     } else {
  //       Assert.IsTrue(res[i].fp == info.sp, "(back, direct), test #" + i + ": different result points");
  //       Assert.IsTrue(res[i].sp == info.fp, "(back, direct), test #" + i + ": different additional points");
  //     }
  //   }
  //
  //   // Back, back
  //   segments.Clear();
  //   for (int i = 0; i < ind.Length / 2; i++) {
  //     if (i == 0) {
  //       segments.Add(new Segment(p[ind[i, 1]], p[ind[i, 0]]));
  //     } else {
  //       segments.Add(new Segment(p[ind[i, 1]], p[ind[i, 0]]));
  //     }
  //   }
  //
  //   for (int i = 1; i < res.Length; i++) {
  //     CrossInfo info = Segment.Intersect(segments[0], segments[i]);
  //     Assert.That(info.crossType, Is.EqualTo(res[i].crossType), "(back, back), test #" + i + ": different cross types");
  //     if (info.crossType == CrossType.NoCross) {
  //       Assert.That(info.fp, Is.Null, "(back, back), test #" + i + ": non-null result point");
  //       Assert.That(info.sp, Is.Null, "(back, back), test #" + i + ": non-null additional point");
  //     } else if (info.crossType == CrossType.SinglePoint || info.crossType == CrossType.SinglePoint) {
  //       Assert.That(res[i].fp, Is.EqualTo(info.fp), "(back, back), test #" + i + ": different result points");
  //       Assert.That(info.sp, Is.Null, "(back, back), test #" + i + ": non-null additional point");
  //     } else {
  //       Assert.IsTrue(res[i].fp == info.sp, "(back, back), test #" + i + ": different result points");
  //       Assert.IsTrue(res[i].sp == info.fp, "(back, back), test #" + i + ": different additional points");
  //     }
  //   }
  // }

}
