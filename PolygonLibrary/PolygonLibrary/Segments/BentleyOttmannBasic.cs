using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AVLUtils;
using PolygonLibrary.Basics;

namespace PolygonLibrary.Segments
{
  // Part of the SegmentCrosserBasic class containing the definition of the main
  // procedure, which processes next event

  /// <summary>
  /// Class for constructing some information about crossing points for a set of segments
  /// using the Bentley-Ottmann's algorithm.
  /// The type of resulatant data depends on the user and is defined by the corresponding
  /// field that should be defined in the inherited classes together with the method AddToResult
  /// that gets two segments and their crossing point.
  /// When creating this method, it is necessary to remember that the segments themselves
  /// cannot be used as keys in the resultant structure since there can be geometrically coinciding
  /// segments in the input data, which can be distinguished only by involving additional data
  /// (for example, indices of the segments in the input container).
  /// Any constructor of an inherited class should initialize the output structure and then call
  /// the method InitializeAndStart that initializes internal structures necessary 
  /// for the Bentley-Ottmann algorithm and starts the main loop.
  /// </summary>
  public abstract partial class SegmentCrosserBasic
  {
#region Internal data
    /// <summary>
    /// Storage for internal segments
    /// </summary>
    private List<InnerSegment> innerSegs = new List<InnerSegment>();

    /// <summary>
    /// Reference to the list of pairs of incident segments
    /// </summary>
    private SortedSet<SegmentPair> incidents = new SortedSet<SegmentPair>();

    /// <summary>
    /// The queue of events
    /// </summary>
    private EventQueue q = new EventQueue();

    /// <summary>
    /// Y-ordered segments crossing the current sweeping line
    /// </summary>
    private YStructure yStruct = new YStructure();
#endregion

    /// <summary>
    /// Property to work with the sweeping point
    /// </summary>
    private Vector2D p_sweep
    {
      get { return yStruct.comparer.p_sweep; }
      set { yStruct.comparer.p_sweep = value; }
    }

#region Subsidiary procedures
    /// <summary>
    /// Checking that two inner segments have incident originals
    /// </summary>
    /// <param name="s1">The first segment</param>
    /// <param name="s2">The second segment</param>
    /// <returns>true, if they are incident; false, otherwise</returns>
    private bool AreIncident(InnerSegment s1, InnerSegment s2)
    {
      return incidents.Contains(new SegmentPair(s1.Origin, s2.Origin));
    }

    /// <summary>
    /// Taking the next event from the queue and processing it
    /// </summary>
    private void ProcessNextEvent()
    {
      Vector2D evPoint;
      Event evInfo;

      // Getting the next event
      q.Pop(out evPoint, out evInfo);

      /*
       * The processing algorithm is as follows:
       *  1) Add to the Y-structure all segments starting here.
       *  2) Take all groups passing through the current event point
       *     and send them to the result with the current event point
       *     as the crossing point.
       *  3) Remove from Y-structure all segments ending here
       *  4) Try to create a new event with the vertical group if it is non-empty:
       *     try to intersect the group of vertical segments with the group,
       *     which is greater than the greatest group passing through the current point
       *  5) If after removing segments, all groups passing through the current point
       *     have become empty, then search upper and lower groups and try to generate
       *     event on the basis of their intersection
       *  6) Otherwise, rearrange sloped groups passing through the current point by removing
       *     them from the Y-structure and storing aside, then moving the sweeping point
       *     to the current one, and finally inserting the groups back to the Y-structure;
       *     with that, check the minimal and minimal of those groups. The try to generate
       *     two events: by intersection of the maximal group and the upper one and
       *     by intersection of the minimal group and the lower one
       */
      
      // The item 1) of the algorithm - adding to the Y-structure all segments starting here
      foreach (InnerSegment s in evInfo.L.GetSegments())
        yStruct.Add(s);
      
      // The item 2) of the algorithm - putting to the result all segments passing through the point
      AddToResult(evPoint, evInfo);
      
      // The item 3) of the algorithm - removing all segments ending here
      foreach (InnerSegment s in evInfo.R.GetSegments())
        yStruct.Remove(s);

      // The item 4) of the algorithm - try to produce an event by the vertical group
      List<InnerSegment> vertSegs = yStruct.GetVerticalSegments().ToList();
      if (vertSegs.Count > 0)
      {
        SegmentGroup next = yStruct.UpperGroup(evPoint.y);
        if (next != null)
        {
          double nextOrdinate = next.ComputeAtPoint(evPoint.x);
          
        }
      }
    }

    /// <summary>
    /// Finding intersections of two segments; those intersections are given to the user 
    /// that are greater than the sweeping point (since they are needed for future events).
    /// Also a check is made whether the segments are incident.
    /// The intersections found are put to the result (if necessary)
    /// </summary>
    /// <param name="s1">The first segment</param>
    /// <param name="s2">The second segment</param>
    /// <returns>The information </returns>
    private CrossInfo FindIntersection(InnerSegment s1, InnerSegment s2)
    {
      if (AreIncident(s1, s2))
        return new CrossInfo(CrossType.NoCross, null, null, null, null);
      else
        return Segment.Intersect(s1, s2);
    }

    /// <summary>
    /// The method that initializes internal structures of the class and starts the main loop
    /// </summary>
    /// <param name="segs">An enumerable container of all segments to be crossed (passed from constructor)</param>
    /// <param name="neighs">A set of all pairs of incident segments which should not be crossed (passed from constructor)</param>
    protected void InitializeAndStart(IEnumerable<Segment> segs, SortedSet<SegmentPair> neighs)
    {
      // Storing list of pairs of incident segments
      incidents = neighs;

      // Converting segments to internal representation
      int k = 0;
      foreach (Segment s in segs)
      {
        innerSegs.Add(new InnerSegment(s, k));
        k++;
      }

      // Initializing the event queue
      foreach (InnerSegment s in innerSegs)
        q.AddEnds(s);

      // Starting the main loop
      while (q.Count > 0)
        ProcessNextEvent();
    }
#endregion
    
#region The method to be be overriden
    /// <summary>
    /// Method, which gets information about a point and bundle of segments (possibly, those,
    /// which are mentioned as non-crossing in the neighs structure passed to the procedure
    /// InitializeAndStart) passing through it and forming the resultant data in an appropriate format.
    /// ID field of InnerSegment data contains the zero-based indices of the original segments
    /// in the IEnumerable passed to the constructor of the class
    /// </summary>
    /// <param name="p">The crossing point</param>
    /// <param name="segs">The enumerable structure containing all segment</param>
    protected abstract void AddToResult(Vector2D p, IEnumerable<InnerSegment> segs);
#endregion
  }
}