using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PolygonLibrary.Basics;
using PolygonLibrary.Toolkit;

namespace PolygonLibrary.Segments
{
  /// <summary>
  /// Enumeration for types of intersection of two segments
  /// </summary>
  public enum CrossType
  {
    /// <summary>
    /// No intersection
    /// </summary>
    NoCross,

    /// <summary>
    /// Intersection at one point that is internal for both segments
    /// </summary>
    SinglePoint,

    /// <summary>
    /// Intersection at one point that is end point for at least one segment segment
    /// </summary>
    EndPoint,

    /// <summary>
    /// The segments are parallel and overlap
    /// </summary>
    Overlap
  }

  /// <summary>
  /// Information about an intersection of two segments
  /// </summary>
  public class CrossInfo
  {
    /// <summary>
    /// COnstant of no-crossing information
    /// </summary>
    static public readonly CrossInfo NoCrossConst = new CrossInfo (CrossType.NoCross, null, null, null, null);

    /// <summary>
    /// Type of the intersection
    /// </summary>
    public CrossType crossType;

    /// <summary>
    /// The intersection point
    /// </summary>
    public Point2D p;

    /// <summary>
    /// The another end of the overlapping part of the segments (if any)
    /// </summary>
    public Point2D p1;

    /// <summary>
    /// Reference to the first segment
    /// </summary>
    public Segment s1;

    /// <summary>
    /// Reference to the second segment
    /// </summary>
    public Segment s2;

    /// <summary>
    /// Full constructor
    /// </summary>
    /// <param name="type">The type of the crossing</param>
    /// <param name="p">The main point of the crossing</param>
    /// <param name="p1">The additional point of the crossing</param>
    /// <param name="ns1">The first segment reference</param>
    /// <param name="ns2">The second segment reference</param>
    public CrossInfo(CrossType type, Point2D p, Point2D p1, Segment ns1, Segment ns2)
    {
      crossType = type;
      this.p = p;
      this.p1 = p1;
      s1 = ns1;
      s2 = ns2;
    }
  }

  /// <summary>
  /// Class of a non-degenerated segment
  /// </summary>
  public class Segment : IComparable<Segment>
  {
    #region Private storage
    /// <summary>
    /// The first end of the segment
    /// </summary>
    protected Point2D _p1;

    /// <summary>
    /// The second end of the segment
    /// </summary>
    protected Point2D _p2;
    #endregion

    #region Access properties
    /// <summary>
    /// The first end of the segment
    /// </summary>
    public Point2D p1
    {
      get { return _p1; }
      private set { _p1 = value; ComputeParameters(); }
    }

    /// <summary>
    /// The second end of the segment
    /// </summary>
    public Point2D p2
    {
      get { return _p2; }
      private set { _p2 = value; ComputeParameters(); }
    }

    /// <summary>
    /// Indexer access
    /// </summary>
    /// <param name="i">The index: 0 - the first end, 1 - the second end</param>
    /// <returns>The point of the corresponding end</returns>
    public Point2D this[int i]
    {
      get
      {
#if DEBUG
        if (i == 0)
          return p1;
        else if (i == 1)
          return p2;
        else
          throw new IndexOutOfRangeException();
#else
          if (i == 0)
            return p1;
          else 
            return p2;
#endif
      }
      private set
      {
#if DEBUG
        if (i == 0)
          p1 = value;
        else if (i == 1)
          p2 = value;
        else
          throw new IndexOutOfRangeException();
#else
        if (i == 0)
          p1 = value;
        else 
          p2 = value;
#endif
        ComputeParameters();
      }
    }

    /// <summary>
    /// The normal vector of the segment
    /// </summary>
    public Vector2D Normal { get; private set; }

    /// <summary>
    /// The directional vector of the segment
    /// </summary>
    public Vector2D Directional { get; private set; }

    /// <summary>
    /// Length of the segment
    /// </summary>
    public double Length { get; private set; }

    /// <summary>
    /// Getting slope of the segment
    /// </summary>
    public double Slope { get; private set; }

    /// <summary>
    /// Check whether the segment is vertical
    /// </summary>
    public bool IsVertical { get; private set; }
    #endregion

    #region Comparing
    /// <summary>
    /// Full comparer:
    ///   - lexicographic order of the first ends
    ///   - lexicographic orders of the second ends
    /// </summary>
    /// <param name="s">The segment, which to be compared with</param>
    /// <returns>+1, if this segment is greater; 0, if these segments are equal; -1, othewise</returns>
    public int CompareTo(Segment s)
    {
      int res = p1.CompareTo(s.p1);
      if (res != 0)
        return res;
      else
        return p2.CompareTo(s.p2);
    }
    #endregion

    #region Overrides
    public override string ToString()
    {
      return "[" + _p1 + ";" + _p2 + "]";
    }

    public override bool Equals(object obj)
    {
#if DEBUG
      if (!(obj is Segment))
        throw new ArgumentException();
#endif
      Segment s = obj as Segment;
      return _p1 == s._p1 && _p2 == s._p2;
    }

    public override int GetHashCode()
    {
      return _p1.GetHashCode() + _p2.GetHashCode();
    }
    #endregion

    #region Constructors
    /// <summary>
    /// Auxiliary internal default constructor
    /// </summary>
    protected Segment()
    {
      _p1 = Point2D.Origin;
      _p2 = Point2D.Origin;

      ComputeParameters();
    }

    /// <summary>
    /// Coordinate constructor
    /// </summary>
    /// <param name="x1">The abscissa of the first end</param>
    /// <param name="y1">The ordinate of the first end</param>
    /// <param name="x2">The abscissa of the second end</param>
    /// <param name="y2">The ordinate of the second end</param>
    public Segment(double x1, double y1, double x2, double y2)
    {
//#if DEBUG
//      if (Tools.EQ(x1, x2) && Tools.EQ(y1, y2))
//        throw new ArgumentException("The ends of a segment cannot coincide");
//#endif
      _p1 = new Point2D(x1, y1);
      _p2 = new Point2D(x2, y2);

      ComputeParameters();
    }

    /// <summary>
    /// Two points constructor
    /// </summary>
    /// <param name="np1">The new first end</param>
    /// <param name="np2">The new second end</param>
    public Segment(Point2D np1, Point2D np2)
    {
#if DEBUG
      if (np1 == np2)
        throw new ArgumentException("The ends of a segment cannot coincide");
#endif
      _p1 = np1;
      _p2 = np2;

      ComputeParameters();
    }

    /// <summary>
    /// Copying constructor
    /// </summary>
    /// <param name="s">The segment to be copied</param>
    public Segment(Segment s)
    {
#if DEBUG
      if (s._p1 == s._p2)
        throw new ArgumentException("The ends of a segment cannot coincide");
#endif
      _p1 = s._p1;
      _p2 = s._p2;

      ComputeParameters();
    }
    #endregion

    #region Common procedures
    /// <summary>
    /// Computing parameters of the vector after changing a coordinate
    /// </summary>
    protected void ComputeParameters()
    {
      Normal = (p2 - p1).TurnCCW();
      Directional = p2 - p1;
      Length = Directional.Length;
      Slope = Directional.PolarAngle;
      IsVertical = Tools.EQ(_p1.x, _p2.x);
    }

    /// <summary>
    /// Checking that a point is an endpoint of this segment
    /// </summary>
    /// <param name="p">The point to be checked</param>
    /// <returns>true, if the point is an endpoint; false, otherwise</returns>
    public bool IsEndPoint(Point2D p)
    {
      return p == p1 || p == p2;
    }

    /// <summary>
    /// Checking that a point is an inner point of this segment
    /// </summary>
    /// <param name="p">The point to be checked</param>
    /// <returns>true, if the point is an inner point; false, otherwise</returns>
    public bool IsInnerPoint(Point2D p)
    {
      return ContainsPoint(p) && !IsEndPoint(p);
    }

    /// <summary>
    /// Check whether a point belongs to the segment
    /// </summary>
    /// <param name="p">The point to be checked</param>
    /// <returns>true, if the point belongs to the segment; false, otherwise</returns>
    public bool ContainsPoint(Point2D p)
    {
      return Vector2D.AreCounterdirected(_p1 - p, _p2 - p);
    }

    /// <summary>
    /// Compute the ordinate of the line passing through the segment at the given abscissa;
    /// For vertical segments an exception is raised
    /// </summary>
    /// <param name="x">The abscissa where to compute</param>
    /// <returns>The corresponding ordinate</returns>
    public double ComputeAtPoint(double x)
    {
      if (IsVertical)
        throw new InvalidOperationException("Cannot compute ordinate for a vertical segment!");

      return _p1.y + (x - _p1.x) * (_p2.y - _p1.y) / (_p2.x - _p1.x);
    }

    /// <summary>
    /// Intersection of two segments
    /// </summary>
    /// <param name="s1">The first segment</param>
    /// <param name="s2">The second segment</param>
    /// <returns>The information about the intersection</returns>
    public static CrossInfo Intersect(Segment s1, Segment s2)
    {
      Vector2D d1 = s1.Directional, d2 = s2.Directional;
      Point2D resPoint = null, resPoint1 = null;
      CrossType crossType;

      /*
        Computational formulas:
        s1.p1 + a1*d1 = s2.p1 + a2*d2  =>
        =>  { a1*d1.x - a2*d2.x = s2.p1.x - s1.p1.x = ds.x
            { a1*d1.y - a2*d2.y = s2.p1.y - s1.p1.y = ds.y =>
        => Delta = -(d1.x*d2.y - d1.y*d2.x) = - (d1 ^ d2),
            Delta1 = -(ds.x*d2.y - ds.y*d2.x) = - (ds ^ d2),
            Delta2 = -(d1.x*ds.y - dx.y*ds.x) = - (d1 ^ ds),
            a1* = Delta1 / Delta = (ds ^ d2) / (d1 ^ d2),
            a2* = -Delta2 / Delta = -(d1 ^ ds) / (d1 ^ d2)
      */
      double Delta = d1 ^ d2;
      Vector2D ds = s2.p1 - s1.p1;
      if (Tools.NE(Delta))
      {
        // The lines conataining the segments are not parallel;
        // then find the intersection point of the lines and 
        // check whether it belongs to both segments
        double
          a1 = ds ^ d2 / Delta,
          a2 = -(d1 ^ ds / Delta);
        if (Tools.LT(a1, 0) || Tools.GT(a1, 1) || Tools.LT(a2, 0) || Tools.GT(a2, 1))
        {
          crossType = CrossType.NoCross;
        }
        else if (Tools.EQ(a1, 0) || Tools.EQ(a1, 1) || Tools.EQ(a2, 0) || Tools.EQ(a2, 1))
        {
          crossType = CrossType.EndPoint;
          resPoint = s1.p1 + a1 * d1;
        }
        else
        {
          crossType = CrossType.SinglePoint;
          resPoint = s1.p1 + a1 * d1;
        }
      }
      else if (Tools.EQ(d1 ^ ds))
      {
        // The lines containing the segments coincide; then
        //   1) enter a coordinate system in this common line:
        //      the origin is the first end of the first segment,
        //      the unit is the second end of the first segment
        //   2) compute coordinates c and d of the second segment endpoints in this system
        //   3) apply the classical algorithm for intersecting segments in a line:
        //      intersection of [a,b] with [c,d] = [ max(a,c), min(b,d) ] if the latter is not less then the first
        //   4) if the segmnets intersect, compute the endpoints of the intersection segment
        Vector2D orth = s1.p2 - s1.p1;
        double
          l = orth.Length,
          l2 = l * l,
          c_ = (s2.p1 - s1.p1) * orth / l2,
          d_ = (s2.p2 - s1.p1) * orth / l2,
          c = Math.Min(c_, d_),
          d = Math.Max(c_, d_),
          alpha = Math.Max(0 /* = a */, c),
          beta = Math.Min(1 /* = b */, d);
        if (Tools.LT(alpha, beta))
        {
          // The segments overlap
          crossType = CrossType.Overlap;
          resPoint = s1.p1 + alpha * orth;
          resPoint1 = s1.p1 + beta * orth;
        }
        else if (Tools.EQ(alpha, beta))
        {
          // The segments intersect by an endpoint
          crossType = CrossType.EndPoint;
          resPoint = s1.p1 + alpha * orth;
        }
        else
          // The segments do not intersect
          crossType = CrossType.NoCross;
      }
      else
        crossType = CrossType.NoCross;

      return new CrossInfo(crossType, resPoint, resPoint1, s1, s2);
    }
    #endregion
  }
}
