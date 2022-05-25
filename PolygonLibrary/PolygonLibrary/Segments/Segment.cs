using System;
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
    /// Intersection at one point
    /// </summary>
    SinglePoint,

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
    /// Type of the intersection
    /// </summary>
    public readonly CrossType crossType;

    /// <summary>
    /// The intersection point
    /// </summary>
    public readonly Point2D p;

    /// <summary>
    /// The another end of the overlapping part of the segments (if any)
    /// </summary>
    public readonly Point2D p1;

    /// <summary>
    /// Reference to the first segment
    /// </summary>
    public readonly Segment s1;

    /// <summary>
    /// Reference to the second segment
    /// </summary>
    public readonly Segment s2;

    /// <summary>
    /// Full constructor
    /// </summary>
    /// <param name="type">The type of the crossing</param>
    /// <param name="np">The main point of the crossing</param>
    /// <param name="np1">The additional point of the crossing</param>
    /// <param name="ns1">The first segment reference</param>
    /// <param name="ns2">The second segment reference</param>
    public CrossInfo(CrossType type, Point2D np, Point2D np1, Segment ns1, Segment ns2)
    {
      crossType = type;
      p = np;
      p1 = np1;
      s1 = ns1;
      s2 = ns2;
    }
  }

  /// <summary>
  /// Class of a non-degenerated segment
  /// </summary>
  public class Segment : IComparable<Segment>
  {
   
#region Access properties
    /// <summary>
    /// The first end of the segment
    /// </summary>
    public readonly Point2D p1;

    /// <summary>
    /// The second end of the segment
    /// </summary>
    public readonly Point2D p2;

    /// <summary>
    /// Indexer access
    /// </summary>
    /// <param name="i">The index: 0 - the first end, 1 - the second end</param>
    /// <returns>The point of the corresponding end</returns>
    public Point2D this[int i] 
    {
      get {
#if DEBUG
        return i switch {
          0 => p1
          , 1 => p2
          , _ => throw new IndexOutOfRangeException()
        };
#else
        if (i == 0) {
          return p1;
        } else { 
          return p2;
        }
#endif
      }
    }

    /// <summary>
    /// The normal vector of the segment
    /// </summary>
    public Vector2D normal { get; private set; }

    /// <summary>
    /// The directional vector of the segment
    /// </summary>
    public Vector2D directional { get; private set; }

    /// <summary>
    /// length of the segment
    /// </summary>
    public double length { get; private set; }

    /// <summary>
    /// Getting polar angle of the segment in the range (-pi, pi]:
    /// the order of ends is significant
    /// </summary>
    public double polarAngle { get; private set; }

    /// <summary>
    /// Check whether the segment is vertical
    /// </summary>
    public bool isVertical { get; private set; }
#endregion
    
#region Comparing
    /// <summary>
    /// Full comparer:
    ///   - lexicographic order of the first ends
    ///   - lexicographic orders of the second ends
    /// </summary>
    /// <param name="s">The segment, which to be compared with</param>
    /// <returns>+1, if this segment is greater; 0, if these segments are equal; -1, otherwise</returns>
    public int CompareTo(Segment s)
    {
      int res = p1.CompareTo(s.p1);
      if (res != 0) {
        return res;
      } else {
        return p2.CompareTo(s.p2);
      }
    }
#endregion
    
#region Overrides
    public override string ToString() => "[" + p1 + ";" + p2 + "]";

    public override bool Equals(object obj)
    {
#if DEBUG
      if (!(obj is Segment segment)) {
        throw new ArgumentException();
      }
#endif
      return p1 == segment.p1 && p2 == segment.p2;
    }

    public override int GetHashCode() => p1.GetHashCode() + p2.GetHashCode();
    #endregion
    
#region Constructors
    /// <summary>
    /// Auxiliary internal default constructor
    /// </summary>
    protected Segment()
    {
      p1 = Point2D.Origin;
      p2 = Point2D.Origin;

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
#if DEBUG
      if (Tools.EQ(x1, x2) && Tools.EQ(y1, y2)) {
        throw new ArgumentException("The ends of a segment cannot coincide");
      }
#endif
      p1 = new Point2D(x1, y1);
      p2 = new Point2D(x2, y2);

      ComputeParameters();
    }

    /// <summary>
    /// Two vectors constructor
    /// </summary>
    /// <param name="np1">The new first end</param>
    /// <param name="np2">The new second end</param>
    public Segment(Point2D np1, Point2D np2)
    {
#if DEBUG
      if (np1 == np2) {
        throw new ArgumentException("The ends of a segment cannot coincide");
      }
#endif
      p1 = np1;
      p2 = np2;

      ComputeParameters();
    }

    /// <summary>
    /// Copying constructor
    /// </summary>
    /// <param name="s">The segment to be copied</param>
    public Segment(Segment s)
    {
#if DEBUG
      if (s.p1 == s.p2) {
        throw new ArgumentException("The ends of a segment cannot coincide");
      }
#endif
      p1 = s.p1;
      p2 = s.p2;

      ComputeParameters();
    }
#endregion
    
#region Common procedures
    /// <summary>
    /// Computing parameters of the vector after changing a coordinate
    /// </summary>
    protected void ComputeParameters()
    {
      normal = new Vector2D(p2.y - p1.y, p1.x - p2.x);
      directional = p2 - p1;
      length = directional.Length;
      polarAngle = directional.PolarAngle;
      isVertical = Tools.EQ(p1.x, p2.x);
    }

    /// <summary>
    /// Checking that a point is an endpoint of this segment
    /// </summary>
    /// <param name="p">The point to be checked</param>
    /// <returns>true, if the point is an endpoint; false, otherwise</returns>
    public bool IsEndPoint(Point2D p) => p == p1 || p == p2;

    /// <summary>
    /// Checking that a point is an inner point of this segment
    /// </summary>
    /// <param name="p">The point to be checked</param>
    /// <returns>true, if the point is an inner point; false, otherwise</returns>
    public bool IsInnerPoint(Point2D p) => ContainsPoint(p) && !IsEndPoint(p);

    /// <summary>
    /// Check whether a point belongs to the segment
    /// </summary>
    /// <param name="p">The point to be checked</param>
    /// <returns>true, if the point belongs to the segment; false, otherwise</returns>
    public bool ContainsPoint(Point2D p) => Vector2D.AreCounterdirected(p1 - p, p2 - p);

    /// <summary>
    /// Compute the ordinate of the line passing through the segment at the given abscissa;
    /// For vertical segments an exception is raised
    /// </summary>
    /// <param name="x">The abscissa where to compute</param>
    /// <returns>The corresponding ordinate</returns>
    public double ComputeAtPoint(double x)
    {
#if DEBUG
      if (isVertical) {
        throw new InvalidOperationException("Cannot compute ordinate for a vertical segment!");
      }
#endif
      
      return p1.y + (x - p1.x) * (p2.y - p1.y) / (p2.x - p1.x);
    }

    /// <summary>
    /// Intersection of two segments
    /// </summary>
    /// <param name="s1">The first segment</param>
    /// <param name="s2">The second segment</param>
    /// <returns>The information about the intersection</returns>
    public static CrossInfo Intersect(Segment s1, Segment s2) {
      Vector2D d1 = s1.directional, d2 = s2.directional; 
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
        // The lines containing the segments are not parallel;
        // then find the intersection point of the lines and 
        // check whether it belongs to both segments
        double
          a1 = ds ^ d2 / Delta,
          a2 = -(d1 ^ ds / Delta);
        if (Tools.LT(a1) || Tools.GT(a1, 1) || Tools.LT(a2) || Tools.GT(a2, 1))
        {
          crossType = CrossType.NoCross;
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
        //      intersection of [a,b] with [c,d] = [ max(a,c), min(b,d) ]
        //      if the latter is not less then the first
        //   4) if the segments intersect, compute the endpoints of the intersection segment
        Vector2D unitVec = s1.p2 - s1.p1;
        double
          l = unitVec.Length,
          l2 = l * l,
          c_ = (s2.p1 - s1.p1) * unitVec / l2,
          d_ = (s2.p2 - s1.p1) * unitVec / l2,
          c = Math.Min(c_, d_),
          d = Math.Max(c_, d_),
          alpha = Math.Max(0 /* = a */, c),
          beta = Math.Min(1 /* = b */, d);
        if (Tools.LT(alpha, beta))
        {
          // The segments overlap
          crossType = CrossType.Overlap;
          resPoint = s1.p1 + alpha * unitVec;
          resPoint1 = s1.p1 + beta * unitVec;
        }
        else if (Tools.EQ(alpha, beta))
        {
          // The segments intersect by an endpoint
          crossType = CrossType.SinglePoint;
          resPoint = s1.p1 + alpha * unitVec;
        } else {
          // The segments do not intersect
          crossType = CrossType.NoCross;
        }
      } else {
        crossType = CrossType.NoCross;
      }

      return new CrossInfo(crossType, resPoint, resPoint1, s1, s2);
    }
#endregion
  }
}