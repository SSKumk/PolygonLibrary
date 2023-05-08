using System;
using PolygonLibrary.Segments;
using PolygonLibrary.Toolkit;

namespace PolygonLibrary.Basics;

/// <summary>
/// A call of a straight line in the plane
/// </summary>
public class Line2D {
  #region Data and properties

  /// <summary>
  /// The property of the coefficient A in the general equation of the line
  /// </summary>
  public double A { get; protected set; }

  /// <summary>
  /// The property of the coefficient B in the general equation of the line
  /// </summary>
  public double B { get; protected set; }

  /// <summary>
  /// The property of the coefficient C in the general equation of the line
  /// </summary>
  public double C { get; protected set; }

  /// <summary>
  /// The property of the directional vector of the line
  /// </summary>
  public Vector2D Direct { get; protected set; } = Vector2D.Zero;

  /// <summary>
  /// The property of the normal vector of the line
  /// </summary>
  public Vector2D Normal { get; protected set; } = Vector2D.Zero;

  #endregion

  #region Contructors, factories, and helping contruction functions

  /// <summary>
  /// Default constructor that produces the abscissa axis
  /// </summary>
  public Line2D() {
    A = 0;
    B = 1;
    C = 0;
    Direct = new Vector2D(1, 0);
    Normal = new Vector2D(0, 1);
  }

  /// <summary>
  /// Constructing a line passing through two points <paramref name="p1"/> and <paramref name="p2"/>.
  /// The positive halfplane is laying to the left from the vector p1->p2.
  /// </summary>
  /// <param name="p1">The first point that lies in the line</param>
  /// <param name="p2">The second point that lies in the line</param>
  public Line2D(Point2D p1, Point2D p2) => PointAndDirect(p1, p2 - p1);


  /// <summary>
  /// Constructing a line passing through two points <paramref name="p1"/> and <paramref name="p2"/>.
  /// The positive halfplane is defined by some third point (which should not belong to the line (p1 p2).
  /// </summary>
  /// <param name="p1">The first point that lies in the line</param>
  /// <param name="p2">The second point that lies in the line</param>
  /// <param name="p3">A point that does not belong to the line and defines the positive halfplane</param>
  public Line2D(Point2D p1, Point2D p2, Point2D p3) => PointAndDirect(p1, p2 - p1, p3);

  /// <summary>
  /// Factory that produces a line on the basis of a point that belongs to the line
  /// and the direction vector of the line. The positive halfplane is supposed to be
  /// to the left from the vector <paramref name="v"/>.
  /// </summary>
  /// <param name="p">A point that belongs to the line</param>
  /// <param name="v">The direction vector of the line</param>
  /// <returns>The resultant line</returns>
  public static Line2D Line2D_PointAndDirect(Point2D p, Vector2D v) {
    Line2D res = new Line2D();
    res.PointAndDirect(p, v);
    return res;
  }

  /// <summary>
  /// Factory that produces a line on the basis of a point that belongs to the line
  /// and the direction vector of the line. The positive halfplane is defined 
  /// by the point <paramref name="p1"/>.
  /// </summary>
  /// <param name="p">A point that belongs to the line</param>
  /// <param name="v">The direction vector of the line</param>
  /// <param name="p1">A point that does not belong to the line and defines the positive halfplane</param>
  /// <returns>The resultant line</returns>
  public static Line2D Line2D_PointAndDirect(Point2D p, Vector2D v, Point2D p1) {
    Line2D res = new Line2D();
    res.PointAndDirect(p, v, p1);
    return res;
  }

  /// <summary>
  /// Internal method that fills the fields such that finally we have a line on the basis 
  /// of a point that belongs to the line and the direction vector of the line. 
  /// The positive halfplane is supposed to be to the left from the vector <paramref name="v"/>.
  /// </summary>
  /// <param name="p">A point that belongs to the line</param>
  /// <param name="v">The direction vector of the line</param>
  private void PointAndDirect(Point2D p, Vector2D v) {
    Direct = new Vector2D(v).Normalize();

    Normal = new Vector2D(-Direct.y, Direct.x);

    A = Normal.x;
    B = Normal.y;
    C = -Normal * (Vector2D)p;
  }

  /// <summary>
  /// Internal method that fills the fields such that finally we have a line on the basis 
  /// of a point that belongs to the line and the direction vector of the line. 
  /// The positive halfplane is defined by the point <paramref name="p1"/>.
  /// </summary>
  /// <param name="p">A point that belongs to the line</param>
  /// <param name="v">The direction vector of the line</param>
  /// <param name="p1">A point that does not belong to the line and defines the positive halfplane</param>
  private void PointAndDirect(Point2D p, Vector2D v, Point2D p1) {
    PointAndDirect(p, v);

    double val = this[p1];
#if DEBUG    
    if (Tools.EQ(val)) {
      throw new ArgumentException("The point that should define the positive halfplane belongs to the line");
    }
#endif
    
    if (Tools.LT(val)) {
      A = -A;
      B = -B;
      C = -C;
      Normal = -Normal;
      Direct = -Direct;
    }
  }

  /// <summary>
  /// Factory that produces a line on the basis of a point that belongs to the line
  /// and the normal vector of the line. The positive halfplane is defined by the vector <paramref name="v"/>.
  /// </summary>
  /// <param name="p">A point that belongs to the line</param>
  /// <param name="v">The direction vector of the line</param>
  /// <returns>The resultant line</returns>
  public static Line2D Line2D_PointAndNormal(Point2D p, Vector2D v) {
    Line2D res = new Line2D();
    res.Normal = v.Normalize();

    res.Direct = new Vector2D(res.Normal.y, -res.Normal.x);
    res.A = res.Normal.x;
    res.B = res.Normal.y;
    res.C = -res.Normal * (Vector2D)p;

    return res;
  }

  /// <summary>
  /// Copying constructor
  /// </summary>
  /// <param name="l">The line to be copied</param>
  public Line2D(Line2D l) {
    A = l.A;
    B = l.B;
    C = l.C;
    Direct = l.Direct;
    Normal = l.Normal;
  }

  /// <summary>
  /// Constructing a line passing through the given segment; 
  /// the positive halfplane os to the left of directional vector of the segment
  /// </summary>
  /// <param name="s">The given segment</param>
  public Line2D (Segment s) : this (s.Start, s.End) { }

  #endregion

  #region Supplementary functions

  /// <summary>
  /// Compute value of the corresponding linear function at the given point
  /// </summary>
  /// <param name="p">The point where to compute the function value</param>
  /// <returns>The computed value</returns>
  public double this[Point2D p] => A * p.x + B * p.y + C;

  /// <summary>
  /// Checks whether the line passes through the given point
  /// </summary>
  /// <param name="p">The given point</param>
  /// <returns>true, if passes; false, otherwise</returns>
  public bool PassesThrough(Point2D p) => Tools.EQ(this[p]);

  /// <summary>
  /// A function that generates a new line that coincides with the current one, 
  /// but has the opposite orientation
  /// </summary>
  /// <returns></returns>
  public Line2D Reorient() {
    Line2D res = new Line2D();
    res.A = -A;
    res.B = -B;
    res.C = -C;
    res.Direct = -Direct;
    res.Normal = -Normal;
    return res;
  }

  /// <summary>
  /// Type of crossing two lines
  /// </summary>
  public enum LineCrossType {
    /// <summary>
    ///  Two lines are not parallel or overlapping
    /// </summary>
    SinglePoint,

    /// <summary>
    /// Two lines are parallel
    /// </summary>
    Parallel,

    /// <summary>
    /// Two lines are overlapping
    /// </summary>
    Overlap
  }


  /// <summary>
  /// Intersection of two lines.
  /// Returns the intersection type (single point, parallel, overlap) and
  /// the intersection point (if it exists and is single) 
  /// </summary>
  /// <param name="l1">The first line</param>
  /// <param name="l2">The second line</param>
  /// <param name="res">The intersection point (if exists and unique)</param>
  /// <returns>The type of imposition of the lines (crossing, parallel, overlapping) </returns>
  public static LineCrossType Intersect(Line2D l1, Line2D l2, out Point2D? res) {
    double d = l1.A * l2.B - l1.B * l2.A
    , d1 = -(l1.C * l2.B - l2.C * l1.B)
    , d2 = -(l1.A * l2.C - l1.C * l2.A);
    if (Tools.NE(d)) {
      res = new Point2D(d1 / d, d2 / d);
      return LineCrossType.SinglePoint;
    } else {
      res = null;
      if (Tools.EQ(d1) && Tools.EQ(d2)) {
        return LineCrossType.Overlap;
      } else {
        return LineCrossType.Parallel;
      }
    }
  }

  /// <summary>
  /// Type of crossing a line and a segment
  /// </summary>
  public enum LineAndSegmentCrossType {
    /// <summary>
    ///  There is one internal cross point
    /// </summary>
    InternalPoint,

    /// <summary>
    ///  There is one cross point, which is the start of the segment
    /// </summary>
    StartPoint,
    
    /// <summary>
    ///  There is one cross point, which is the end of the segment
    /// </summary>
    EndPoint,
    
    /// <summary>
    /// The line and the segment are parallel and the segment is not embedded to the line
    /// </summary>
    Parallel,

    /// <summary>
    /// The segment is embedded to the line
    /// </summary>
    Overlap,

    /// <summary>
    /// The segment and the line are not parallel, but they do not intersect
    /// </summary>
    NoCross
  }

  /// <summary>
  /// Intersection of a line and a segment.
  /// Returns the intersection type (single point, parallel, overlap, no cross) and
  /// the intersection point (if it exists and is single) 
  /// </summary>
  /// <param name="l">The line</param>
  /// <param name="s">The segment</param>
  /// <param name="res">The intersection point (if exists and unique)</param>
  /// <returns>The type of imposition of the objects (no cross, single crossing point, parallel, overlapping) </returns>
  public static LineAndSegmentCrossType Intersect(Line2D l, Segment s, out Point2D? res) {
    double A = l.Normal * s.Directional, B = l.C + l.Normal * (Vector2D)s.Start, alpha;

    res = null;
    if (Tools.NE(A)) {
      alpha = -B / A;
      if (Tools.EQ(alpha)) {
        res = s.Start;
        return LineAndSegmentCrossType.StartPoint;
      } else if (Tools.EQ(alpha, 1)) {
        res = s.End;
        return LineAndSegmentCrossType.EndPoint;
      } else if (Tools.GT(alpha) && Tools.LT(alpha, 1)) {
        res = s.Start + alpha * s.Directional;
        return LineAndSegmentCrossType.InternalPoint;
      } else {
        return LineAndSegmentCrossType.NoCross;
      }
    } else {
      if (Tools.NE(B)) {
        return LineAndSegmentCrossType.Parallel;
      } else {
        return LineAndSegmentCrossType.Overlap;
      }
    }
  }

  #endregion
}