using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AVLUtils;
using PolygonLibrary.Basics;

using PolygonLibrary.Toolkit;

namespace PolygonLibrary.Segments
{
  /// <summary>
  /// Class for constructing collection of crossing points for a set of segments
  /// using the Bentley-Ottmann's algorithm defined in the base abstract class.
  /// As a result of its construction, it includes a dictionary where the keys are 
  /// the points of intersections and the values are sorted sets of segment indices
  /// (with respect to the list of segements given to the constructor)
  /// passing through the corresponding corssing point. This sets can include
  /// pairs of indices of segments marked as incident if there are another sehment(s)
  /// passing through this point. 
  /// Such a realization of resultant container assumes that the input container with segment
  /// collection allows index access in a reasonable time (it is an array or sorted container
  /// providing logarithmic access to an element by its index).
  /// </summary>
  public class SegmentCrosser1 : SegmentCrosserBasic
  {
    #region Resultant data
    /// <summary>
    /// Collection information about pairs of segments indexed by crossing points 
    /// where they cross
    /// </summary>
    public SortedDictionary<Point2D, SortedSet<int>> crossPoints { get; private set; }
    #endregion

    #region Overriden method
    /// <summary>
    /// Adding to the resultant list crossing of two given segments at the given point
    /// </summary>
    /// <param name="s1">The first segment</param>
    /// <param name="s2">The second segment</param>
    /// <param name="p">The crossing point</param>
    override protected void AddToResult(InnerSegment s1, InnerSegment s2, Point2D p)
    {
      if (!crossPoints.ContainsKey(p))
        crossPoints[p] = new SortedSet<int>();
      crossPoints[p].Add(s1.ID);
      crossPoints[p].Add(s2.ID);
    }
    #endregion

    #region Constructor
    /// <summary>
    /// Constructor accepting input data and forming the list of crossing points
    /// </summary>
    /// <param name="segs">An enumerable container of all segments to be crossed</param>
    /// <param name="neighs">A set of all pairs of incident segments which should not be crossed</param>
    public SegmentCrosser1(IEnumerable<Segment> segs, SortedSet<SegmentPair> neighs)
    {
      // Initializing the resultant storage
      crossPoints = new SortedDictionary<Point2D, SortedSet<int>>();

      // Initialize the class and do the computations
      InitializeAndStart(segs, neighs);
    }
    #endregion
  }

  /// <summary>
  /// Class for constructing collection of crossing points for a set of segments
  /// using the Bentley-Ottmann's algorithm defined in the base abstract class.
  /// As a result of its construction, it includes a list where the indices coincide 
  /// with the index of a segment in the input IEnumerable container and the values are
  /// sorted sets of other segments that cross with the main segment. They are sorted 
  /// by local coordinate in the main segment (the first end is the origin, the second end
  /// is the unit). 
  /// Such a realization of resultant container assumes that the input container with segment
  /// collection allows index access in a reasonable time (it is an array or sorted container
  /// providing logarithmic access to an element by its index).
  /// </summary>
  public class SegmentCrosser2 : SegmentCrosserBasic
  {
    /// <summary>
    /// Class for an element of the resulting list: reference to the main segment,
    /// references to another segment of the crossing, the corssong point,       
    /// the baricentric coordinate (with respect of the endpoints of the main segment) of the crossing point.
    /// The order on these objects is the following: 
    ///   - the main segment
    ///   - the coordinate of the crossing point
    ///   - the second segment
    /// Comparison of segments takes into account the orientation of the segments
    /// (which end is the first and which is the second)
    /// </summary>
    public class CrossData : IComparable<CrossData>
    {
      /// <summary>
      /// The crossing point
      /// </summary>
      public Point2D p { get; private set; }

      /// <summary>
      /// The main segment of the crossing
      /// </summary>
      public Segment s1 { get; private set; }

      /// <summary>
      /// The other segment of the crossing
      /// </summary>
      public Segment s2 { get; private set; }

      /// <summary>
      /// The coordinate of the crossing point along the main segment
      /// </summary>
      public double Part { get; private set; }

      /// <summary>
      /// Constructor
      /// </summary>
      /// <param name="ns1">The parent segment</param>
      /// <param name="ns2">The second segment</param>
      /// <param name="np">The crossing point</param>
      public CrossData(Segment ns1, Segment ns2, Point2D np)
      {
        p = np;
        s1 = ns1;
        s2 = ns2;
        Part = (p - ns1.p1).Length / ns1.Length;
      }

      /// <summary>
      /// Comparer
      /// </summary>
      /// <param name="other">The object to be compared with</param>
      /// <returns>-1, if this object is less than other; +1, if this object is greater; 0, otherwise</returns>
      public int CompareTo(CrossData other)
      {
        int res;
        res = s1.CompareTo(other.s1);
        if (res != 0)
          return res;

        res = Tools.Sign(Part - other.Part);
        if (res != 0)
          return res;

        return s2.CompareTo(other.s2);
      }
    }

    #region Resultant data
    /// <summary>
    /// Collection information about pairs of segments indexed by crossing points 
    /// where they cross
    /// </summary>
    public List<SortedSet<CrossData>> crosses { get; private set; }
    #endregion

    #region Overriden method
    /// <summary>
    /// Adding to the resultant list crossing of two given segments at the given point
    /// </summary>
    /// <param name="s1">The first segment</param>
    /// <param name="s2">The second segment</param>
    /// <param name="p">The crossing point</param>
    override protected void AddToResult(InnerSegment s1, InnerSegment s2, Point2D p)
    {
      crosses[s1.ID].Add(new CrossData(s1.Origin, s2.Origin, p));
      crosses[s2.ID].Add(new CrossData(s2.Origin, s1.Origin, p));
    }
    #endregion

    #region Constructor
    /// <summary>
    /// Constructor accepting input data and forming the list of crossing points
    /// </summary>
    /// <param name="segs">An enumerable container of all segments to be crossed</param>
    /// <param name="neighs">A set of all pairs of incident segments which should not be crossed</param>
    public SegmentCrosser2(IEnumerable<Segment> segs, SortedSet<SegmentPair> neighs)
    {
      // Initializing the resultant storage
      crosses = new List<SortedSet<CrossData>>();
      foreach (Segment s in segs)
        crosses.Add(new SortedSet<CrossData>());

      // Initialize the class and do the computations
      InitializeAndStart(segs, neighs);
    }
    #endregion
  }
}
