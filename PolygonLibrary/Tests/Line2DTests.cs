using NUnit.Framework;
using PolygonLibrary.Basics;
using PolygonLibrary.Segments;
using PolygonLibrary.Toolkit;


namespace Tests {
  [TestFixture]
  public class Line2DTests {
    private readonly double _s2 = Math.Sqrt(2);

    #region Construction tests

    //-----------------------------------------------------------------------    

    [Test]
    public void Line2DConstructionTest01() {
      double x0 = 3
      , y0 = 1;
      Point2D p1 = new Point2D(x0, y0);
      Point2D p2 = new Point2D(4, 2);
      Line2D l1 = new Line2D(p1, p2);

      Assert.That(Tools.EQ(l1.A, -_s2 / 2), "Construction test 01: Wrong A");
      Assert.That(Tools.EQ(l1.B, _s2 / 2), "Construction test 01: Wrong B");
      Assert.That(Tools.EQ(l1.C, -_s2 / 2 * (-x0 + y0)), "Construction test 01: Wrong C");

      Assert.That(l1.Normal == new Vector2D(-_s2 / 2, _s2 / 2), "Construction test 01: Wrong normal vector");

      Assert.That(l1.Direct == new Vector2D(_s2 / 2, _s2 / 2), "Construction test 01: Wrong directional vector");
    }

    [Test]
    public void Line2DConstructionTest02() {
      double x0 = 3
      , y0 = 1;
      Point2D p1 = new Point2D(4, 2);
      Point2D p2 = new Point2D(x0, y0);
      Line2D l1 = new Line2D(p1, p2);

      Assert.That(Tools.EQ(l1.A, _s2 / 2), "Construction test 02: Wrong A");
      Assert.That(Tools.EQ(l1.B, -_s2 / 2), "Construction test 02: Wrong B");
      Assert.That(Tools.EQ(l1.C, -_s2 / 2 * (x0 - y0)), "Construction test 02: Wrong C");

      Assert.That(l1.Normal == new Vector2D(_s2 / 2, -_s2 / 2), "Construction test 02: Wrong normal vector");

      Assert.That(l1.Direct == new Vector2D(-_s2 / 2, -_s2 / 2), "Construction test 02: Wrong directional vector");
    }

    [Test]
    public void Line2DConstructionTest03() {
      double x0 = 3
      , y0 = 1;
      Point2D p1 = new Point2D(x0, y0);
      Point2D p2 = new Point2D(x0, y0 + 2);
      Line2D l1 = new Line2D(p1, p2);

      Assert.That(Tools.EQ(l1.A, -1), "Construction test 03: Wrong A");
      Assert.That(Tools.EQ(l1.B, 0), "Construction test 03: Wrong B");
      Assert.That(Tools.EQ(l1.C, x0), "Construction test 03: Wrong C");

      Assert.That(l1.Normal == new Vector2D(-1, 0), "Construction test 03: Wrong normal vector");

      Assert.That(l1.Direct == new Vector2D(0, 1), "Construction test 03: Wrong directional vector");
    }

    [Test]
    public void Line2DConstructionTest04() {
      double x0 = 3
      , y0 = 1;
      Point2D p1 = new Point2D(x0, y0);
      Point2D p2 = new Point2D(x0, y0 - 2);
      Line2D l1 = new Line2D(p1, p2);

      Assert.That(Tools.EQ(l1.A, 1), "Construction test 04: Wrong A");
      Assert.That(Tools.EQ(l1.B, 0), "Construction test 04: Wrong B");
      Assert.That(Tools.EQ(l1.C, -x0), "Construction test 04: Wrong C");

      Assert.That(l1.Normal == new Vector2D(1, 0), "Construction test 04: Wrong normal vector");

      Assert.That(l1.Direct == new Vector2D(0, -1), "Construction test 04: Wrong directional vector");
    }

    [Test]
    public void Line2DConstructionTest05() {
      double x0 = 3
      , y0 = 1;
      Point2D p1 = new Point2D(x0, y0);
      Point2D p2 = new Point2D(4, 2);
      Point2D p3 = new Point2D(x0 + 2, y0 + 10);
      Line2D l1 = new Line2D(p1, p2, p3);

      Assert.That(Tools.EQ(l1.A, -_s2 / 2), "Construction test 05: Wrong A");
      Assert.That(Tools.EQ(l1.B, _s2 / 2), "Construction test 05: Wrong B");
      Assert.That(Tools.EQ(l1.C, -_s2 / 2 * (-x0 + y0)), "Construction test 05: Wrong C");

      Assert.That(l1.Normal == new Vector2D(-_s2 / 2, _s2 / 2), "Construction test 05: Wrong normal vector");

      Assert.That(l1.Direct == new Vector2D(_s2 / 2, _s2 / 2), "Construction test 05: Wrong directional vector");
    }

    [Test]
    public void Line2DConstructionTest06() {
      double x0 = 3
      , y0 = 1;
      Point2D p1 = new Point2D(x0, y0);
      Point2D p2 = new Point2D(4, 2);
      Point2D p3 = new Point2D(x0 + 2, y0 - 10);
      Line2D l1 = new Line2D(p1, p2, p3);

      Assert.That(Tools.EQ(l1.A, _s2 / 2), "Construction test 06: Wrong A");
      Assert.That(Tools.EQ(l1.B, -_s2 / 2), "Construction test 06: Wrong B");
      Assert.That(Tools.EQ(l1.C, -_s2 / 2 * (x0 - y0)), "Construction test 06: Wrong C");

      Assert.That(l1.Normal == new Vector2D(_s2 / 2, -_s2 / 2), "Construction test 06: Wrong normal vector");

      Assert.That(l1.Direct == new Vector2D(-_s2 / 2, -_s2 / 2), "Construction test 06: Wrong directional vector");
    }

    [Test]
    public void Line2DConstructionTest07() {
      double x0 = 3
      , y0 = 1;
      Point2D p1 = new Point2D(4, 2);
      Point2D p2 = new Point2D(x0, y0);
      Point2D p3 = new Point2D(x0 + 2, y0 - 10);

      Line2D l1 = new Line2D(p1, p2);

      Assert.That(Tools.EQ(l1.A, _s2 / 2), "Construction test 07: Wrong A");
      Assert.That(Tools.EQ(l1.B, -_s2 / 2), "Construction test 07: Wrong B");
      Assert.That(Tools.EQ(l1.C, -_s2 / 2 * (x0 - y0)), "Construction test 07: Wrong C");

      Assert.That(l1.Normal == new Vector2D(_s2 / 2, -_s2 / 2), "Construction test 07: Wrong normal vector");

      Assert.That(l1.Direct == new Vector2D(-_s2 / 2, -_s2 / 2), "Construction test 07: Wrong directional vector");
    }

    [Test]
    public void Line2DConstructionTest08() {
      double x0 = 3
      , y0 = 1;
      Point2D p1 = new Point2D(4, 2);
      Point2D p2 = new Point2D(x0, y0);
      Point2D p3 = new Point2D(x0 + 2, y0 + 10);

      Line2D l1 = new Line2D(p1, p2, p3);

      Assert.That(Tools.EQ(l1.A, -_s2 / 2), "Construction test 08: Wrong A");
      Assert.That(Tools.EQ(l1.B, _s2 / 2), "Construction test 08: Wrong B");
      Assert.That(Tools.EQ(l1.C, -_s2 / 2 * (-x0 + y0)), "Construction test 08: Wrong C");

      Assert.That(l1.Normal == new Vector2D(-_s2 / 2, _s2 / 2), "Construction test 08: Wrong normal vector");

      Assert.That(l1.Direct == new Vector2D(_s2 / 2, _s2 / 2), "Construction test 02: Wrong directional vector");
    }

    [Test]
    public void Line2DConstructionTest09() {
      double x0 = 3
      , y0 = 1;
      Point2D p = new Point2D(x0, y0);
      Vector2D v = new Vector2D(1, 1);
      Line2D l1 = Line2D.Line2D_PointAndDirect(p, v);

      Assert.That(Tools.EQ(l1.A, -_s2 / 2), "Construction test 09: Wrong A");
      Assert.That(Tools.EQ(l1.B, _s2 / 2), "Construction test 09: Wrong B");
      Assert.That(Tools.EQ(l1.C, -_s2 / 2 * (-x0 + y0)), "Construction test 09: Wrong C");

      Assert.That(l1.Normal == new Vector2D(-_s2 / 2, _s2 / 2), "Construction test 09: Wrong normal vector");

      Assert.That(l1.Direct == new Vector2D(_s2 / 2, _s2 / 2), "Construction test 09: Wrong directional vector");
    }

    [Test]
    public void Line2DConstructionTest10() {
      double x0 = 3
      , y0 = 1;
      Point2D p = new Point2D(x0, y0);
      Vector2D v = new Vector2D(-1, -1);
      Line2D l1 = Line2D.Line2D_PointAndDirect(p, v);

      Assert.That(Tools.EQ(l1.A, _s2 / 2), "Construction test 10: Wrong A");
      Assert.That(Tools.EQ(l1.B, -_s2 / 2), "Construction test 10: Wrong B");
      Assert.That(Tools.EQ(l1.C, -_s2 / 2 * (x0 - y0)), "Construction test 10: Wrong C");

      Assert.That(l1.Normal == new Vector2D(_s2 / 2, -_s2 / 2), "Construction test 10: Wrong normal vector");

      Assert.That(l1.Direct == new Vector2D(-_s2 / 2, -_s2 / 2), "Construction test 10: Wrong directional vector");
    }

    [Test]
    public void Line2DConstructionTest11() {
      double x0 = 3
      , y0 = 1;
      Point2D p = new Point2D(x0, y0);
      Vector2D v = new Vector2D(-1, 1);
      Line2D l1 = Line2D.Line2D_PointAndNormal(p, v);

      Assert.That(Tools.EQ(l1.A, -_s2 / 2), "Construction test 11: Wrong A");
      Assert.That(Tools.EQ(l1.B, _s2 / 2), "Construction test 11: Wrong B");
      Assert.That(Tools.EQ(l1.C, -_s2 / 2 * (-x0 + y0)), "Construction test 11: Wrong C");

      Assert.That(l1.Normal == new Vector2D(-_s2 / 2, _s2 / 2), "Construction test 11: Wrong normal vector");

      Assert.That(l1.Direct == new Vector2D(_s2 / 2, _s2 / 2), "Construction test 11: Wrong directional vector");
    }

    [Test]
    public void Line2DConstructionTest12() {
      double x0 = 3
      , y0 = 1;
      Point2D p = new Point2D(x0, y0);
      Vector2D v = new Vector2D(1, -1);
      Line2D l1 = Line2D.Line2D_PointAndNormal(p, v);

      Assert.That(Tools.EQ(l1.A, _s2 / 2), "Construction test 12: Wrong A");
      Assert.That(Tools.EQ(l1.B, -_s2 / 2), "Construction test 12: Wrong B");
      Assert.That(Tools.EQ(l1.C, -_s2 / 2 * (x0 - y0)), "Construction test 12: Wrong C");

      Assert.That(l1.Normal == new Vector2D(_s2 / 2, -_s2 / 2), "Construction test 12: Wrong normal vector");

      Assert.That(l1.Direct == new Vector2D(-_s2 / 2, -_s2 / 2), "Construction test 12: Wrong directional vector");
    }

    [Test]
    public void Line2DConstructionTest13() {
      double x0 = 3
      , y0 = 1;
      Point2D p1 = new Point2D(x0, y0);
      Point2D p2 = new Point2D(4, 2);
      Line2D l1 = new Line2D(new Segment(p1, p2));

      Assert.That(Tools.EQ(l1.A, -_s2 / 2), "Construction test 13: Wrong A");
      Assert.That(Tools.EQ(l1.B, _s2 / 2), "Construction test 13: Wrong B");
      Assert.That(Tools.EQ(l1.C, -_s2 / 2 * (-x0 + y0)), "Construction test 13: Wrong C");

      Assert.That(l1.Normal == new Vector2D(-_s2 / 2, _s2 / 2), "Construction test 13: Wrong normal vector");

      Assert.That(l1.Direct == new Vector2D(_s2 / 2, _s2 / 2), "Construction test 13: Wrong directional vector");
    }

    [Test]
    public void Line2DConstructionTest14() {
      double x0 = 3
      , y0 = 1;
      Point2D p1 = new Point2D(4, 2);
      Point2D p2 = new Point2D(x0, y0);
      Line2D l1 = new Line2D(new Segment(p1, p2));

      Assert.That(Tools.EQ(l1.A, _s2 / 2), "Construction test 14: Wrong A");
      Assert.That(Tools.EQ(l1.B, -_s2 / 2), "Construction test 14: Wrong B");
      Assert.That(Tools.EQ(l1.C, -_s2 / 2 * (x0 - y0)), "Construction test 14: Wrong C");

      Assert.That(l1.Normal == new Vector2D(_s2 / 2, -_s2 / 2), "Construction test 14: Wrong normal vector");

      Assert.That(l1.Direct == new Vector2D(-_s2 / 2, -_s2 / 2), "Construction test 14: Wrong directional vector");
    }

    #endregion

    #region Point positioning tests

    //-----------------------------------------------------------------------

    [Test]
    public void Line2DPositioningTest01() {
      double x0 = 3
      , y0 = 1
      , x1 = 4
      , y1 = 2;
      Point2D p1 = new Point2D(x0, y0);
      Point2D p2 = new Point2D(x1, y1);
      Line2D l1 = new Line2D(new Segment(p1, p2));

      Point2D test1 = new Point2D(x0 + 3, y0 + 10);
      Point2D test2 = new Point2D((x0 + x1) / 2, (y0 + y1) / 2);
      Point2D test3 = new Point2D(x0 + 3, y0 - 10);

      Assert.Multiple(() => {
        Assert.IsTrue(Tools.GT(l1[test1]), "Positioning test 01: positive semi-plane");
        Assert.IsTrue(Tools.EQ(l1[test2]), "Positioning test 01: zero - just at line");
        Assert.IsTrue(Tools.LT(l1[test3]), "Positioning test 01: negative semi-plane");
        Assert.IsTrue(l1.PassesThrough(test2), "Positioning test 01: passing through");
      });
    }

    [Test]
    public void Line2DPositioningTest02() {
      double x1 = 3
      , y1 = 1
      , x0 = 4
      , y0 = 2;
      Point2D p1 = new Point2D(x0, y0);
      Point2D p2 = new Point2D(x1, y1);
      Line2D l1 = new Line2D(new Segment(p1, p2));

      Point2D test1 = new Point2D(x0 - 3, y0 - 10);
      Point2D test2 = new Point2D((x0 + x1) / 2, (y0 + y1) / 2);
      Point2D test3 = new Point2D(x0 - 3, y0 + 10);

      Assert.Multiple(() => {
        Assert.IsTrue(Tools.GT(l1[test1]), "Positioning test 02: positive semi-plane");
        Assert.IsTrue(Tools.EQ(l1[test2]), "Positioning test 02: zero - just at line");
        Assert.IsTrue(Tools.LT(l1[test3]), "Positioning test 02: negative semi-plane");
        Assert.IsTrue(l1.PassesThrough(test2), "Positioning test 02: passing through");
      });
    }


    [Test]
    public void Line2DPositioningTest03() {
      double x0 = 3
      , y0 = 1
      , x1 = 3
      , y1 = 2;
      Point2D p1 = new Point2D(x0, y0);
      Point2D p2 = new Point2D(x1, y1);
      Line2D l1 = new Line2D(new Segment(p1, p2));

      Point2D test1 = new Point2D(x0 - 3, y0 + 10);
      Point2D test2 = new Point2D(x0, (y0 + y1) / 2);
      Point2D test3 = new Point2D(x0 + 3, y0 - 10);

      Assert.Multiple(() => {
        Assert.IsTrue(Tools.GT(l1[test1]), "Positioning test 03: positive semi-plane");
        Assert.IsTrue(Tools.EQ(l1[test2]), "Positioning test 03: zero - just at line");
        Assert.IsTrue(Tools.LT(l1[test3]), "Positioning test 03: negative semi-plane");
        Assert.IsTrue(l1.PassesThrough(test2), "Positioning test 03: passing through");
      });
    }

    [Test]
    public void Line2DPositioningTest04() {
      double x1 = 3
      , y1 = 1
      , x0 = 3
      , y0 = 2;
      Point2D p1 = new Point2D(x0, y0);
      Point2D p2 = new Point2D(x1, y1);
      Line2D l1 = new Line2D(new Segment(p1, p2));

      Point2D test1 = new Point2D(x0 + 3, y0 - 10);
      Point2D test2 = new Point2D((x0 + x1) / 2, (y0 + y1) / 2);
      Point2D test3 = new Point2D(x0 - 3, y0 + 10);

      Assert.Multiple(() => {
        Assert.IsTrue(Tools.GT(l1[test1]), "Positioning test 04: positive semi-plane");
        Assert.IsTrue(Tools.EQ(l1[test2]), "Positioning test 04: zero - just at line");
        Assert.IsTrue(Tools.LT(l1[test3]), "Positioning test 04: negative semi-plane");
        Assert.IsTrue(l1.PassesThrough(test2), "Positioning test 04: passing through");
      });
    }

    #endregion

    #region Reorient test

    //-----------------------------------------------------------------------

    [Test]
    public void Line2DReorientTest() {
      Point2D p1 = new Point2D(1, 3);
      Point2D p2 = new Point2D(4, 5);
      Line2D l = new Line2D(p1, p2);
      Line2D l1 = l.Reorient();

      Assert.Multiple(() => {
        Assert.IsTrue(Tools.EQ(l.A, -l1.A), "Reorient test: A");
        Assert.IsTrue(Tools.EQ(l.B, -l1.B), "Reorient test: B");
        Assert.IsTrue(Tools.EQ(l.C, -l1.C), "Reorient test: C");
        Assert.IsTrue(l.Normal == -l1.Normal, "Reorient test: Normal");
        Assert.IsTrue(l.Direct == -l1.Direct, "Reorient test: Directional");
      });
    }

    #endregion

    #region Line-line intersection test

    //-----------------------------------------------------------------------

    [Test]
    public void Line2DLineLineIntersectionTest() {
      Point2D p1 = new Point2D(0, 2)
      , p2 = new Point2D(1, -2)
      , p3 = new Point2D(1, 2)
      , p4 = new Point2D(4, 2);
      Vector2D v1 = new Vector2D(3, 0)
      , v2 = new Vector2D(0, -2)
      , v3 = new Vector2D(1, 2)
      , v4 = new Vector2D(3, -1);

      Line2D l01 = Line2D.Line2D_PointAndDirect(p1, v1)
      , l02 = Line2D.Line2D_PointAndDirect(p4, -v1)
      , l03 = Line2D.Line2D_PointAndDirect(p2, v1)
      , l04 = Line2D.Line2D_PointAndDirect(p2, v2)
      , l05 = Line2D.Line2D_PointAndDirect(p3, -v2)
      , l06 = Line2D.Line2D_PointAndDirect(p4, v2)
      , l07 = Line2D.Line2D_PointAndDirect(p3, v3)
      , l08 = Line2D.Line2D_PointAndDirect(p3, -v3)
      , l09 = Line2D.Line2D_PointAndDirect(p4, v3)
      , l10 = Line2D.Line2D_PointAndDirect(p4, v4)
      , l11 = Line2D.Line2D_PointAndDirect(p4, -v4)
      , l12 = Line2D.Line2D_PointAndDirect(p2, v4);

      Point2D? res;
      Line2D.LineCrossType cross;

      // Overlap
      Assert.Multiple(() => {
        Assert.IsTrue(Line2D.Intersect(l01, l02, out res) == Line2D.LineCrossType.Overlap, "Overlap test: 1");
        Assert.IsTrue(Line2D.Intersect(l04, l05, out res) == Line2D.LineCrossType.Overlap, "Overlap test: 2");
        Assert.IsTrue(Line2D.Intersect(l07, l08, out res) == Line2D.LineCrossType.Overlap, "Overlap test: 3");
        Assert.IsTrue(Line2D.Intersect(l10, l11, out res) == Line2D.LineCrossType.Overlap, "Overlap test: 4");
      });

      // Parallel
      Assert.Multiple(() => {
        Assert.IsTrue(Line2D.Intersect(l01, l03, out res) == Line2D.LineCrossType.Parallel, "Parallel test: 1");
        Assert.IsTrue(Line2D.Intersect(l04, l06, out res) == Line2D.LineCrossType.Parallel, "Parallel test: 2");
        Assert.IsTrue(Line2D.Intersect(l07, l09, out res) == Line2D.LineCrossType.Parallel, "Parallel test: 3");
        Assert.IsTrue(Line2D.Intersect(l10, l12, out res) == Line2D.LineCrossType.Parallel, "Parallel test: 4");
      });

      // Crossing
      Assert.Multiple(() => {
        // Vertical and horizontal lines
        cross = Line2D.Intersect(l01, l04, out res);
        Assert.IsTrue(cross == Line2D.LineCrossType.SinglePoint, "Crossing test 1: wrong type of result");
        Assert.IsTrue(res! == p3, "Crossing test 1: wrong resultant point");

        // Vertical and slope lines
        cross = Line2D.Intersect(l06, l09, out res);
        Assert.IsTrue(cross == Line2D.LineCrossType.SinglePoint, "Crossing test 2: wrong type of result");
        Assert.IsTrue(res! == p4, "Crossing test 2: wrong resultant point");

        // Horizontal and slope lines
        cross = Line2D.Intersect(l03, l12, out res);
        Assert.IsTrue(cross == Line2D.LineCrossType.SinglePoint, "Crossing test 3: wrong type of result");
        Assert.IsTrue(res! == p2, "Crossing test 3: wrong resultant point");

        // Two slope lines
        cross = Line2D.Intersect(l10, l09, out res);
        Assert.IsTrue(cross == Line2D.LineCrossType.SinglePoint, "Crossing test 4: wrong type of result");
        Assert.IsTrue(res! == p4, "Crossing test 4: wrong resultant point");
      });
    }

    #endregion

    #region Line-segment generic intersection tests

    //-----------------------------------------------------------------------

    // Horizontal line
    [Test]
    public void Line2DLineSegmentIntersectionTest1() {
      Line2D l = Line2D.Line2D_PointAndDirect(new Point2D(0, 3), new Vector2D(1, 0));

      Point2D p01 = new Point2D(3, 6)
      , p02 = new Point2D(2, 4)
      , p03 = new Point2D(1, 2)
      , p04 = new Point2D(0, 0)
      , p05 = new Point2D(4, 6)
      , p06 = new Point2D(4, 4)
      , p07 = new Point2D(4, 2)
      , p08 = new Point2D(4, -1)
      , p09 = new Point2D(7, 6)
      , p10 = new Point2D(0, 3)
      , p11 = new Point2D(10, 3)
      , p12 = new Point2D(9, 0);

      Segment s01 = new Segment(p01, p02)
      , s02 = new Segment(p02, p03)
      , s03 = new Segment(p03, p04)
      , s04 = new Segment(p05, p06)
      , s05 = new Segment(p06, p07)
      , s06 = new Segment(p07, p08)
      , s07 = new Segment(p05, p09)
      , s08 = new Segment(p10, p11)
      , s09 = new Segment(p04, p12);

      Point2D? res;
      Line2D.LineAndSegmentCrossType cross;

      // Crossing
      Assert.Multiple(() => {
        // Slope segment
        cross = Line2D.Intersect(l, s02, out res);
        Assert.That(cross, Is.EqualTo(Line2D.LineAndSegmentCrossType.InternalPoint)
        , "Generic Horizontal Cross test 1: wrong result type");
        Assert.That(res!, Is.EqualTo(new Point2D(1.5, 3)), "Generic Horizontal Cross test 1: wrong resultant point");

        // Vertical segment
        cross = Line2D.Intersect(l, s05, out res);
        Assert.That(cross, Is.EqualTo(Line2D.LineAndSegmentCrossType.InternalPoint)
        , "Generic Horizontal Cross test 2: wrong result type");
        Assert.That(res!, Is.EqualTo(new Point2D(4, 3)), "Generic Horizontal Cross test 2: wrong resultant point");
      });

      // Crossing lines, no crossing with segment
      Assert.Multiple(() => {
        // Slope segments
        Assert.That(Line2D.Intersect(l, s01, out res), Is.EqualTo(Line2D.LineAndSegmentCrossType.NoCross)
        , "Generic Horizontal No cross test 1: wrong result type");
        Assert.That(Line2D.Intersect(l, s03, out res), Is.EqualTo(Line2D.LineAndSegmentCrossType.NoCross)
        , "Generic Horizontal No cross test 2: wrong result type");

        // Vertical segments
        Assert.That(Line2D.Intersect(l, s04, out res), Is.EqualTo(Line2D.LineAndSegmentCrossType.NoCross)
        , "Generic Horizontal No cross test 3: wrong result type");
        Assert.That(Line2D.Intersect(l, s06, out res), Is.EqualTo(Line2D.LineAndSegmentCrossType.NoCross)
        , "Generic Horizontal No cross test 4: wrong result type");
      });

      // Parallel segments
      Assert.Multiple(() => {
        Assert.That(Line2D.Intersect(l, s07, out res), Is.EqualTo(Line2D.LineAndSegmentCrossType.Parallel)
        , "Generic Horizontal Parallel test 1: wrong result type");
        Assert.That(Line2D.Intersect(l, s09, out res), Is.EqualTo(Line2D.LineAndSegmentCrossType.Parallel)
        , "Generic Horizontal Parallel test 2: wrong result type");
      });

      // Overlap segment
      Assert.That(Line2D.Intersect(l, s08, out res), Is.EqualTo(Line2D.LineAndSegmentCrossType.Overlap)
      , "Generic Horizontal Overlap test: wrong result type");
    }

//-----------------------------------------------------------------------

    // Vertical line
    [Test]
    public void Line2DLineSegmentIntersectionTest2() {
      Line2D l = Line2D.Line2D_PointAndDirect(new Point2D(3, 0), new Vector2D(0, 1));

      Point2D p01 = new Point2D(6, 3)
      , p02 = new Point2D(4, 2)
      , p03 = new Point2D(2, 1)
      , p04 = new Point2D(0, 0)
      , p05 = new Point2D(6, 4)
      , p06 = new Point2D(4, 4)
      , p07 = new Point2D(2, 4)
      , p08 = new Point2D(-1, 4)
      , p09 = new Point2D(6, 7)
      , p10 = new Point2D(3, 0)
      , p11 = new Point2D(3, 10)
      , p12 = new Point2D(0, 9);

      Segment s01 = new Segment(p01, p02)
      , s02 = new Segment(p02, p03)
      , s03 = new Segment(p03, p04)
      , s04 = new Segment(p05, p06)
      , s05 = new Segment(p06, p07)
      , s06 = new Segment(p07, p08)
      , s07 = new Segment(p05, p09)
      , s08 = new Segment(p10, p11)
      , s09 = new Segment(p04, p12);

      Point2D? res;
      Line2D.LineAndSegmentCrossType cross;

      // Crossing
      Assert.Multiple(() => {
        // Slope segment
        cross = Line2D.Intersect(l, s02, out res);
        Assert.That(cross, Is.EqualTo(Line2D.LineAndSegmentCrossType.InternalPoint)
        , "Generic Vertical Cross test 1: wrong result type");
        Assert.That(res!, Is.EqualTo(new Point2D(3, 1.5)), "Generic Vertical Cross test 1: wrong resultant point");

        // Vertical segment
        cross = Line2D.Intersect(l, s05, out res);
        Assert.That(cross, Is.EqualTo(Line2D.LineAndSegmentCrossType.InternalPoint)
        , "Generic Vertical Cross test 2: wrong result type");
        Assert.That(res!, Is.EqualTo(new Point2D(3, 4)), "Generic Vertical Cross test 2: wrong resultant point");
      });

      // Crossing lines, no crossing with segment
      Assert.Multiple(() => {
        // Slope segments
        Assert.That(Line2D.Intersect(l, s01, out res), Is.EqualTo(Line2D.LineAndSegmentCrossType.NoCross)
        , "Generic Vertical No cross test 1: wrong result type");
        Assert.That(Line2D.Intersect(l, s03, out res), Is.EqualTo(Line2D.LineAndSegmentCrossType.NoCross)
        , "Generic Vertical No cross test 2: wrong result type");

        // Vertical segments
        Assert.That(Line2D.Intersect(l, s04, out res), Is.EqualTo(Line2D.LineAndSegmentCrossType.NoCross)
        , "Generic Vertical No cross test 3: wrong result type");
        Assert.That(Line2D.Intersect(l, s06, out res), Is.EqualTo(Line2D.LineAndSegmentCrossType.NoCross)
        , "Generic Vertical No cross test 4: wrong result type");
      });

      // Parallel segments
      Assert.Multiple(() => {
        Assert.That(Line2D.Intersect(l, s07, out res), Is.EqualTo(Line2D.LineAndSegmentCrossType.Parallel)
        , "Generic Vertical Parallel test 1: wrong result type");
        Assert.That(Line2D.Intersect(l, s09, out res), Is.EqualTo(Line2D.LineAndSegmentCrossType.Parallel)
        , "Generic Vertical Parallel test 2: wrong result type");
      });

      // Overlap segment
      Assert.That(Line2D.Intersect(l, s08, out res), Is.EqualTo(Line2D.LineAndSegmentCrossType.Overlap)
      , "Generic Vertical Overlap test: wrong result type");
    }

//-----------------------------------------------------------------------

    // Slope line  
    [Test]
    public void Line2DLineSegmentIntersectionTest3() {
      Line2D l = Line2D.Line2D_PointAndDirect(new Point2D(0, -1), new Vector2D(3, 1));

      Point2D p01 = new Point2D(4, 4)
      , p02 = new Point2D(4, 1)
      , p03 = new Point2D(4, -1)
      , p04 = new Point2D(4, -3)
      , p05 = new Point2D(2, -1)
      , p06 = new Point2D(-1, -1)
      , p07 = new Point2D(-7, -1)
      , p08 = new Point2D(0, 1)
      , p09 = new Point2D(-2, 3)
      , p10 = new Point2D(1, -3)
      , p11 = new Point2D(3, 1)
      , p12 = new Point2D(5, 5)
      , p13 = new Point2D(0.5, 2)
      , p14 = new Point2D(3.5, 3)
      , p15 = new Point2D(-3, -2)
      , p16 = new Point2D(2, -1.0 / 3)
      , p17 = new Point2D(-0.5, -2)
      , p18 = new Point2D(2.5, -1);

      Segment s01 = new Segment(p01, p02)
      , s02 = new Segment(p02, p03)
      , s03 = new Segment(p03, p04)
      , s04 = new Segment(p03, p05)
      , s05 = new Segment(p05, p06)
      , s06 = new Segment(p06, p07)
      , s07 = new Segment(p04, p05)
      , s08 = new Segment(p05, p08)
      , s09 = new Segment(p08, p09)
      , s10 = new Segment(p10, p05)
      , s11 = new Segment(p05, p11)
      , s12 = new Segment(p11, p12)
      , s13 = new Segment(p13, p14)
      , s14 = new Segment(p15, p16)
      , s15 = new Segment(p17, p18);

      Point2D? res;
      Line2D.LineAndSegmentCrossType cross;

      // Crossing
      Assert.Multiple(() => {
        // Vertical segment
        cross = Line2D.Intersect(l, s02, out res);
        Assert.That(cross, Is.EqualTo(Line2D.LineAndSegmentCrossType.InternalPoint)
        , "Generic Slope Cross test 1: vertical segment - wrong result type");
        Assert.That(res!, Is.EqualTo(new Point2D(4, 1.0 / 3))
        , "Generic Slope Cross test 1: vertical segment - wrong resultant point");

        // Horizontal segment
        cross = Line2D.Intersect(l, s05, out res);
        Assert.That(cross, Is.EqualTo(Line2D.LineAndSegmentCrossType.InternalPoint)
        , "Generic Slope Cross test 2: horizontal segment - wrong result type");
        Assert.That(res!, Is.EqualTo(new Point2D(0, -1))
        , "Generic Slope Cross test 2: horizontal segment - wrong resultant point");

        // Slope segment 1
        cross = Line2D.Intersect(l, s08, out res);
        Assert.That(cross, Is.EqualTo(Line2D.LineAndSegmentCrossType.InternalPoint)
        , "Generic Slope Cross test 3: slope segment 1 - wrong result type");
        Assert.That(res!, Is.EqualTo(new Point2D(1.5, -0.5))
        , "Generic Slope Cross test 3: slope segment 1 - wrong resultant point");

        // Slope segment 2
        cross = Line2D.Intersect(l, s11, out res);
        Assert.That(cross, Is.EqualTo(Line2D.LineAndSegmentCrossType.InternalPoint)
        , "Generic Slope Cross test 4: slope segment 2 - wrong result type");
        Assert.That(res!, Is.EqualTo(new Point2D(12.0 / 5, -1.0 / 5))
        , "Generic Slope Cross test 4: slope segment 2 - wrong resultant point");
      });

      // Crossing lines, no crossing with segment
      Assert.Multiple(() => {
        // Vertical segments
        Assert.That(Line2D.Intersect(l, s01, out res), Is.EqualTo(Line2D.LineAndSegmentCrossType.NoCross)
        , "Generic Slope No cross test 1: vertical segment 1 - wrong result type");
        Assert.That(Line2D.Intersect(l, s03, out res), Is.EqualTo(Line2D.LineAndSegmentCrossType.NoCross)
        , "Generic Slope No cross test 2: vertical segment 2 - wrong result type");

        // Horizontal segments
        Assert.That(Line2D.Intersect(l, s04, out res), Is.EqualTo(Line2D.LineAndSegmentCrossType.NoCross)
        , "Generic Slope No cross test 3: horizontal segment 1 - wrong result type");
        Assert.That(Line2D.Intersect(l, s06, out res), Is.EqualTo(Line2D.LineAndSegmentCrossType.NoCross)
        , "Generic Slope No cross test 4: horizontal segment 1 - wrong result type");

        // Slope segments 1
        Assert.That(Line2D.Intersect(l, s07, out res), Is.EqualTo(Line2D.LineAndSegmentCrossType.NoCross)
        , "Generic Slope No cross test 5: slope segment 1 - wrong result type");
        Assert.That(Line2D.Intersect(l, s09, out res), Is.EqualTo(Line2D.LineAndSegmentCrossType.NoCross)
        , "Generic Slope No cross test 6: slope segment 2 - wrong result type");

        // Slope segments 2
        Assert.That(Line2D.Intersect(l, s10, out res), Is.EqualTo(Line2D.LineAndSegmentCrossType.NoCross)
        , "Generic Slope No cross test 5: slope segment 3 - wrong result type");
        Assert.That(Line2D.Intersect(l, s12, out res), Is.EqualTo(Line2D.LineAndSegmentCrossType.NoCross)
        , "Generic Slope No cross test 6: slope segment 4 - wrong result type");
      });

      // Parallel segments
      Assert.Multiple(() => {
        Assert.That(Line2D.Intersect(l, s13, out res), Is.EqualTo(Line2D.LineAndSegmentCrossType.Parallel)
        , "Generic Slope Parallel test 1: wrong result type");
        Assert.That(Line2D.Intersect(l, s15, out res), Is.EqualTo(Line2D.LineAndSegmentCrossType.Parallel)
        , "Generic Slope Parallel test 2: wrong result type");
      });

      // Overlap segment
      Assert.That(Line2D.Intersect(l, s14, out res), Is.EqualTo(Line2D.LineAndSegmentCrossType.Overlap)
      , "Generic Slope Overlap test: wrong result type");
    }

    #endregion

    #region Line-segment non-generic intersection tests

    //-----------------------------------------------------------------------

    // Horizontal line
    [Test]
    public void Line2DLineSegmentIntersectionTest4() {
      Line2D l = Line2D.Line2D_PointAndDirect(new Point2D(0, 2.5), new Vector2D(1, 0));

      Point2D p1 = new Point2D(2, 4)
      , p2 = new Point2D(2, 2.5)
      , p3 = new Point2D(2, 0)
      , p4 = new Point2D(4, 6.12)
      , p5 = new Point2D(11.0 / 3, 2.5)
      , p6 = new Point2D(1.3, 11.8);

      Segment s1 = new Segment(p1, p2)
      , s2 = new Segment(p2, p3)
      , s3 = new Segment(p4, p5)
      , s4 = new Segment(p5, p6);

      Line2D.LineAndSegmentCrossType cross;
      Point2D? res;

      cross = Line2D.Intersect(l, s1, out res);
      Assert.That(cross, Is.EqualTo(Line2D.LineAndSegmentCrossType.EndPoint)
      , "Non-generic horizontal cross test 1 - wrong result type");
      Assert.That(res!, Is.EqualTo(s1.End), "Non-generic horizontal cross test 1 - wrong resultant point");

      cross = Line2D.Intersect(l, s3, out res);
      Assert.That(cross, Is.EqualTo(Line2D.LineAndSegmentCrossType.EndPoint)
      , "Non-generic horizontal cross test 2 - wrong result type");
      Assert.That(res!, Is.EqualTo(s3.End), "Non-generic horizontal cross test 2 - wrong resultant point");

      cross = Line2D.Intersect(l, s2, out res);
      Assert.That(cross, Is.EqualTo(Line2D.LineAndSegmentCrossType.StartPoint)
      , "Non-generic horizontal cross test 3 - wrong result type");
      Assert.That(res!, Is.EqualTo(s2.Start), "Non-generic horizontal cross test 3 - wrong resultant point");

      cross = Line2D.Intersect(l, s4, out res);
      Assert.That(cross, Is.EqualTo(Line2D.LineAndSegmentCrossType.StartPoint)
      , "Non-generic horizontal cross test 4 - wrong result type");
      Assert.That(res!, Is.EqualTo(s4.Start), "Non-generic horizontal cross test 4 - wrong resultant point");
    }

    //-----------------------------------------------------------------------

    // Vertical line
    [Test]
    public void Line2DLineSegmentIntersectionTest5() {
      Line2D l = Line2D.Line2D_PointAndDirect(new Point2D(2.5, 0), new Vector2D(0, 1));

      Point2D p1 = new Point2D(4, 2)
      , p2 = new Point2D(2.5, 2)
      , p3 = new Point2D(0, 2)
      , p4 = new Point2D(6.12, 4)
      , p5 = new Point2D(2.5, 11.0 / 3)
      , p6 = new Point2D(11.8, 1.3);

      Segment s1 = new Segment(p1, p2)
      , s2 = new Segment(p2, p3)
      , s3 = new Segment(p4, p5)
      , s4 = new Segment(p5, p6);

      Line2D.LineAndSegmentCrossType cross;
      Point2D? res;

      cross = Line2D.Intersect(l, s1, out res);
      Assert.That(cross, Is.EqualTo(Line2D.LineAndSegmentCrossType.EndPoint)
      , "Non-generic vertical cross test 1 - wrong result type");
      Assert.That(res!, Is.EqualTo(s1.End), "Non-generic vertical cross test 1 - wrong resultant point");

      cross = Line2D.Intersect(l, s3, out res);
      Assert.That(cross, Is.EqualTo(Line2D.LineAndSegmentCrossType.EndPoint)
      , "Non-generic vertical cross test 2 - wrong result type");
      Assert.That(res!, Is.EqualTo(s3.End), "Non-generic vertical cross test 2 - wrong resultant point");

      cross = Line2D.Intersect(l, s2, out res);
      Assert.That(cross, Is.EqualTo(Line2D.LineAndSegmentCrossType.StartPoint)
      , "Non-generic vertical cross test 3 - wrong result type");
      Assert.That(res!, Is.EqualTo(s2.Start), "Non-generic vertical cross test 3 - wrong resultant point");

      cross = Line2D.Intersect(l, s4, out res);
      Assert.That(cross, Is.EqualTo(Line2D.LineAndSegmentCrossType.StartPoint)
      , "Non-generic vertical cross test 4 - wrong result type");
      Assert.That(res!, Is.EqualTo(s4.Start), "Non-generic vertical cross test 4 - wrong resultant point");
    }

    //-----------------------------------------------------------------------

    // Slope line
    [Test]
    public void Line2DLineSegmentIntersectionTest6() {
      Line2D l = Line2D.Line2D_PointAndDirect(new Point2D(0, -1), new Vector2D(3, 1));

      Point2D p1 = new Point2D(4.4, 4)
      , p2 = new Point2D(4, 1.0 / 3)
      , p3 = new Point2D(-7.3, 4)
      , p4 = new Point2D(0.3, 1.5)
      , p5 = new Point2D(7.5, 1.5)
      , p6 = new Point2D(10.3, 1.5)
      , p7 = new Point2D(-9.4, 6)
      , p8 = new Point2D(-1, -4.0 / 3)
      , p9 = new Point2D(-3, -10.1);

      Segment s1 = new Segment(p1, p2)
      , s2 = new Segment(p2, p3)
      , s3 = new Segment(p4, p5)
      , s4 = new Segment(p5, p6)
      , s5 = new Segment(p7, p8)
      , s6 = new Segment(p8, p9);
      
      Line2D.LineAndSegmentCrossType cross;
      Point2D? res;

      cross = Line2D.Intersect(l, s1, out res);
      Assert.That(cross, Is.EqualTo(Line2D.LineAndSegmentCrossType.EndPoint)
      , "Non-generic slope cross test 1 - wrong result type");
      Assert.That(res!, Is.EqualTo(s1.End), "Non-generic slope cross test 1 - wrong resultant point");

      cross = Line2D.Intersect(l, s3, out res);
      Assert.That(cross, Is.EqualTo(Line2D.LineAndSegmentCrossType.EndPoint)
      , "Non-generic slope cross test 2 - wrong result type");
      Assert.That(res!, Is.EqualTo(s3.End), "Non-generic slope cross test 2 - wrong resultant point");

      cross = Line2D.Intersect(l, s5, out res);
      Assert.That(cross, Is.EqualTo(Line2D.LineAndSegmentCrossType.EndPoint)
      , "Non-generic slope cross test 3 - wrong result type");
      Assert.That(res!, Is.EqualTo(s5.End), "Non-generic slope cross test 3 - wrong resultant point");

      cross = Line2D.Intersect(l, s2, out res);
      Assert.That(cross, Is.EqualTo(Line2D.LineAndSegmentCrossType.StartPoint)
      , "Non-generic slope cross test 4 - wrong result type");
      Assert.That(res!, Is.EqualTo(s2.Start), "Non-generic slope cross test 4 - wrong resultant point");

      cross = Line2D.Intersect(l, s4, out res);
      Assert.That(cross, Is.EqualTo(Line2D.LineAndSegmentCrossType.StartPoint)
      , "Non-generic slope cross test 5 - wrong result type");
      Assert.That(res!, Is.EqualTo(s4.Start), "Non-generic slope cross test 5 - wrong resultant point");      

      cross = Line2D.Intersect(l, s6, out res);
      Assert.That(cross, Is.EqualTo(Line2D.LineAndSegmentCrossType.StartPoint)
      , "Non-generic slope cross test 6 - wrong result type");
      Assert.That(res!, Is.EqualTo(s6.Start), "Non-generic slope cross test 6 - wrong resultant point");      
    }

    #endregion
  }
}