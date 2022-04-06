using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AVLUtils;

namespace PolygonLibrary
{
  // Class for internal representation of the segment
  class InnerSegment : Segment, IComparable<InnerSegment>
  {
    #region Additional data
    /// <summary>
    /// Reference to the parent object
    /// </summary>
    public readonly Segment Origin;
    #endregion

    #region Constructor
    /// <summary>
    /// Default constructor for compatability
    /// </summary>
    public InnerSegment()
      : base()
    {
      Origin = null;
    }

    /// <summary>
    /// The only constructor on the basis of a segment
    /// </summary>
    /// <param name="s">The original segment</param>
    public InnerSegment (Segment s)
      : base ()
    {
      Origin = s;

      // Proper orientation of the segment
      _p1 = s.p1;
      _p2 = s.p2;
      if (_p1 > _p2)
        Tools.Swap (ref _p1, ref _p2);

      ComputeParameters ();
    }
    #endregion

    #region Overrided procedures
    /// <summary>
    /// String representation of the inner segment
    /// </summary>
    /// <returns>The string representation</returns>
    public override string ToString ()
    {
      return base.ToString () + ", ID = " + myID;
    }

    /// <summary>
    /// Comparison with other inner segment. At first, we compare the objects as segments. 
    /// In the case, of equality, compare IDs
    /// </summary>
    /// <param name="other">The inner segment to be comapared with</param>
    /// <returns>-1, if this object is less than the other; +1, if this object is greater; 0, if they are equal</returns>
    public int CompareTo (InnerSegment other)
    {
      int res = base.CompareTo (other);
      if (res != 0)
        return res;
      else
        return myID.CompareTo (other.myID);
    }
    #endregion
  }

  /// <summary>
  /// Sorted pair of two segments
  /// </summary>
  public class SegmentPair : IComparable<SegmentPair>
  {
    /// <summary>
    /// The first (less) segment in the pair
    /// </summary>
    public Segment s1 { get; private set; }

    /// <summary>
    /// The second (greater) segment in the pair
    /// </summary>
    public Segment s2 { get; private set; }

    /// <summary>
    /// Constructor with the segments to be joined into the pair
    /// </summary>
    /// <param name="ns1">One segment</param>
    /// <param name="ns2">Another segment</param>
    public SegmentPair (Segment ns1, Segment ns2)
    {
      if (ns1.CompareTo (ns2) < 0)
      {
        s1 = ns1;
        s2 = ns2;
      }
      else
      {
        s1 = ns2;
        s2 = ns1;
      }
    }

    /// <summary>
    /// Comparison
    /// </summary>
    /// <param name="other">The pair to be compared with</param>
    /// <returns>-1, if this pair is less; +1, if this pair is greater; 0, otherwise</returns>
    public int CompareTo (SegmentPair other)
    {
      int res = s1.CompareTo (other.s1);
      if (res != 0)
        return res;
      else
        return s2.CompareTo (other.s2);
    }
  }

  /// <summary>
  /// Class for an element of the resulting list: contains two segments and a crossing point
  /// </summary>
  public class CrossData : IComparable<CrossData>
  {
    /// <summary>
    /// The crossing point
    /// </summary>
    public Vector2D p { get; private set; }

    /// <summary>
    /// The other segment of the crossing
    /// </summary>
    public Segment s { get; private set; }

    /// <summary>
    /// The coordinate of the crossing point along the parent segment
    /// </summary>
    public double Part { get; private set; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="ns1">The parent segment</param>
    /// <param name="ns2">The second segment</param>
    /// <param name="np">The crossing point</param>
    public CrossData (Segment ns1, Segment ns2, Vector2D np)
    {
      p = np;
      s = ns2;
      Part = (p - ns1.p1).Length / ns1.Length;
    }

    /// <summary>
    /// Comparer
    /// </summary>
    /// <param name="other">The object to be compared with</param>
    /// <returns>-1, if this object is less than other; +1, if this object is greater; 0, otherwise</returns>
    public int CompareTo (CrossData other)
    {
      int res = Tools.Sign (Part - other.Part);
      if (res != 0)
        return res;
      else
        return s.CompareTo(other.s);
    }
  }

  /// <summary>
  /// Class for constructing collection of crossing points for a set of segments
  /// using the Bentley-Ottmann's algorithm.
  /// As a result of its construction, it includes a list of crossing points 
  /// indexed by pairs of segments
  /// </summary>
  public class SegmentCrosser
  {
    #region Resulting data
    /// <summary>
    /// Collection information about pairs of segments indexed by crossing points 
    /// where they cross
    /// </summary>
    public SortedDictionary<Vector2D, SortedSet<SegmentPair>> crossPoints { get; private set; }

    public SortedDictionary<Segment,SortedSet<CrossData>>
    #endregion

    #region Subsidiary classes
    /// <summary>
    /// The type of an event; contains segments starting, ending and crossing in this point
    /// </summary>
    private class Event : IEnumerable<InnerSegment>
    {
      #region Storages
      /// <summary>
      /// Set of segments having here their left ends
      /// </summary>
      public AVLSet<InnerSegment> L = new AVLSet<InnerSegment> ();

      /// <summary>
      /// Set of segments having here their right ends
      /// </summary>
      public AVLSet<InnerSegment> R = new AVLSet<InnerSegment> ();

      /// <summary>
      /// Set of segments crossing here 
      /// </summary>
      public AVLSet<InnerSegment> I = new AVLSet<InnerSegment> ();
      #endregion

      #region Iterator
      /// <summary>
      /// Enumerator for the segments hidden in an event
      /// </summary>
      public class EventSegmentIterator : IEnumerator<InnerSegment>
      {
        /// <summary>
        /// State of the enumerator
        /// </summary>
        private enum State { Before, InL, InR, InI, After };

        /// <summary>
        /// The current state of the enumerator
        /// </summary>
        private State state;

        /// <summary>
        /// The event to which this enumerator is connected
        /// </summary>
        private Event myEvent;

        /// <summary>
        /// Enumerator in the current storage
        /// </summary>
        private IEnumerator<InnerSegment> curEnumerator;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parent">The event to which the new enumerator will be connected</param>
        public EventSegmentIterator (Event parent)
        {
          myEvent = parent;
          state = State.Before;
          curEnumerator = null;
        }

        /// <summary>
        /// Getting property showing whether the iterator has a valid value
        /// </summary>
        public bool IsValid
        {
          get { return state != State.After; }
        }

        /// <summary>
        /// Getting property of the current value
        /// </summary>
        public InnerSegment Current
        {
          get
          {
            if (state == State.Before || state == State.After)
              throw new InvalidOperationException ();
            else
              return curEnumerator.Current;
          }
        }

        /// <summary>
        /// Getting property of non-generic interface
        /// </summary>
        object IEnumerator.Current
        {
          get { return Current; }
        }

        /// <summary>
        /// Dispose method (for the aim of compatability)
        /// </summary>
        public void Dispose () { }

        /// <summary>
        /// Setting up the enumerator
        /// </summary>
        public void Reset ()
        {
          state = State.Before;
          curEnumerator = null;
        }

        /// <summary>
        /// Step further
        /// </summary>
        /// <returns>true, if the iterator has been moved successfully; false, otherwise</returns>
        public bool MoveNext ()
        {
          if (state == State.Before)
          {
            curEnumerator = myEvent.L.GetEnumerator ();
            state = State.InL;
          }
          
          if (state == State.InL)
          {
            if (curEnumerator.MoveNext ())
              return true;
            curEnumerator = myEvent.R.GetEnumerator ();
            state = State.InR;
          }
            
          if (state == State.InR)
          {
            if (curEnumerator.MoveNext ())
              return true;
            curEnumerator = myEvent.I.GetEnumerator ();
            state = State.InI;
          }

          if (state == State.InI)
          {
            if (curEnumerator.MoveNext ())
              return true;
            state = State.After;
          }

          return false;
        }
      }

      public IEnumerator<InnerSegment> GetEnumerator ()
      {
        return new EventSegmentIterator (this);
      }

      /// <summary>
      /// Getting a non-generic enumerator
      /// </summary>
      /// <returns>A non-generic enumerator set to the beginning of the collection</returns>
      IEnumerator IEnumerable.GetEnumerator ()
      {
        return GetEnumerator ();
      }
      #endregion
    }

    /// <summary>
    /// Class for event queue
    /// </summary>
    private class EventQueue : SortedDictionary<Vector2D, Event>
    {
      /// <summary>
      /// Adding events of segment ends
      /// </summary>
      /// <param name="s">The segment, which ends should be added</param>
      public void AddEnds (InnerSegment s)
      {
        if (!ContainsKey (s.p1))
          Add (s.p1, new Event ());
        this[s.p1].L.Add (s);

        if (!ContainsKey (s.p2))
          Add (s.p2, new Event ());
        this[s.p2].R.Add (s);
      }

      /// <summary>
      /// Adding an event of crossing two segments
      /// </summary>
      /// <param name="p">The point of crossing</param>
      /// <param name="s1">The first segment</param>
      /// <param name="s2">The second segment</param>
      public void AddCrossing (Vector2D p, InnerSegment s1, InnerSegment s2)
      {
        if (!ContainsKey (p))
          Add (p, new Event ());
        Event e = this[p];
        if (!e.I.Contains (s1) && !e.L.Contains (s1) && !e.R.Contains (s1))
          e.I.Add (s1);
        if (!e.I.Contains (s2) && !e.L.Contains (s2) && !e.R.Contains (s2))
          e.I.Add (s2);
      }

      /// <summary>
      /// Take the minimal value from the dictionary and remove it from the storage
      /// </summary>
      /// <param name="p">The point of the event</param>
      /// <param name="e">The event information</param>
      public void Pop (out Vector2D p, out Event e)
      {
#if DEBUG
        if (Count == 0)
          throw new ArgumentException ("The queue is empty!");
#endif
        KeyValuePair<Vector2D, Event> pair = this.First ();
        p = pair.Key;
        e = pair.Value;
        Remove (p);
      }
    }

    /// <summary>
    /// Class of group of segments
    /// </summary>
    private class SegmentGroup : AVLSet<InnerSegment> { }

    /// <summary>
    /// Class which realizes correct ordering of segments for current state of the sweeping line
    /// </summary>
    private class SweepLineSegmentComparer : IComparer<SegmentGroup>
    {
      /// <summary>
      /// The point defining the current state of the sweeping line
      /// </summary>
      public Vector2D p_sweep = Vector2D.Zero;

      /// <summary>
      /// The comparer
      /// </summary>
      /// <param name="s1">The first group to be compared</param>
      /// <param name="s2">The second group to be compared</param>
      /// <returns>-1, is s1 < s2; +1, if s1 > s2; 0, otherwise</returns>
      public int Compare (SegmentGroup g1, SegmentGroup g2)
      {
        // If one group is empty, it is less; if both are empty, they are equal
        if (g1.Count == 0)
        {
          if (g2.Count == 0)
            return 0;
          else
            return -1;
        }
        if (g2.Count == 0)
          return +1;

        // Compare their intersection ordinates with the sweep line
        InnerSegment s1 = g1.Min (), s2 = g2.Min ();

        double
          y1 = s1.ComputeAtPoint (p_sweep.x),
          y2 = s2.ComputeAtPoint (p_sweep.x);

        if (Tools.NE (y1, y2))
        {
          if (Tools.GT (y1, y2))
            return +1;
          else
            return -1;
        }

        // They have equal ordinates. Check their slopes
        double sl1 = s1.Slope, sl2 = s2.Slope;
        if (Tools.EQ (sl1, sl2))
          // They have equal slopes - that is, the groups are equal
          return 0;
        else
        {
          // They have diferent slopes. Compare them taking into account their position 
          // with respect to the sweeping point

          // At first, compare them by the right slopes
          int res;
          if (Tools.LT (sl1, sl2))
            res = -1;
          else
            res = +1;

          // If they are lower than the sweeping point, change the order
          if (Tools.GT (y1, p_sweep.y))
            res = -res;

          return res;
        }
      }
    }

    /// <summary>
    /// The class for keeping the vertical oredered structure of segments
    /// </summary>
    private class YStructure : AVLTreeUnsafe<SegmentGroup>
    {
      /// <summary>
      /// Storage for vertical segments
      /// </summary>
      private SortedSet<InnerSegment> vertSegs;

      /// <summary>
      /// Getting all current vertical segments
      /// </summary>
      /// <returns>A collection of the verical segments</returns>
      public IEnumerable<InnerSegment> GetVerticalSegments ()
      {
        foreach (InnerSegment s in vertSegs)
          yield return s;
      }

      /// <summary>
      /// Getting all current slope segments
      /// </summary>
      /// <returns>A collection of the slope segments</returns>
      public IEnumerable<InnerSegment> GetSlopeSegments ()
      {
        foreach (SegmentGroup g in this)
          foreach (InnerSegment s in g)
            yield return s;
      }

      #region Constructors
      /// <summary>
      /// Default constructor
      /// </summary>
      public YStructure ()
        : base ()
      {
        vertSegs = new SortedSet<InnerSegment> ();
        SetComparer (new SweepLineSegmentComparer ());
        Comparer.p_sweep = new Vector2D (-1e38, -1e38);
      }
      #endregion

      /// <summary>
      /// Getting comparer as a SweepLineSegmentComparer object
      /// </summary>
      public new SweepLineSegmentComparer Comparer
      {
        get { return (SweepLineSegmentComparer)comparer; }
      }

      /// <summary>
      /// Work with the swep point
      /// </summary>
      public Vector2D SweepPoint
      {
        get { return Comparer.p_sweep; }
        set { Comparer.p_sweep = value; }
      }

      /// <summary>
      /// Adding a segment choosing where to store it: in main storage or in the vertical segments one
      /// </summary>
      /// <param name="s">The segment to be added</param>
      /// <returns>true, if the segment has been added successfully; false, otherwise</returns>
      public bool Add (InnerSegment s)
      {
        if (s.IsVertical)
          return vertSegs.Add (s);
        else
        {
          SegmentGroup g = new SegmentGroup (), resG;
          g.Add (s);
          if (Find (g, out resG))
          {
            return resG.Add (s);
          }
          else
            return base.Add (g);
        }
      }

      /// <summary>
      /// Removing a segment from a proper storage: from main or from the vertical segments
      /// </summary>
      /// <param name="s">The segment to be removed</param>
      /// <returns>true, if the segment has been removed successfully; false, otherwise</returns>
      public bool Remove (InnerSegment s)
      {
        if (s.IsVertical)
          return vertSegs.Remove (s);
        else
        {
          SegmentGroup g = new SegmentGroup (), resG;
          g.Add (s);
          if (Find (g, out resG))
          {
            resG.Remove (s);
            if (resG.Count == 0)
              Remove (resG);
            return true;
          }
          else
            return false;
        }
      }

      /// <summary>
      /// Get the group containing the given segment
      /// </summary>
      /// <param name="s">The segment defining the group</param>
      /// <returns>The reference to the group; or null, if there is no such a group</returns>
      public SegmentGroup GetGroup (InnerSegment s)
      {
        SegmentGroup g = new SegmentGroup (), resG;
        g.Add (s);
        if (Find (g, out resG))
          return resG;
        else
          return null;
      }
    }
    #endregion

    #region Working data
    /// <summary>
    /// Storage for internal segments
    /// </summary>
    List<InnerSegment> innerSegs;

    /// <summary>
    /// Reference to the list of pairs of incident segments
    /// </summary>
    SortedSet<SegmentPair> incidents;

    /// <summary>
    /// The queue of events
    /// </summary>
    EventQueue q;

    /// <summary>
    /// Y-ordered segments crossing the current sweeping line
    /// </summary>
    YStructure yStruct;
    #endregion

    /// <summary>
    /// Property to work with the sweeping point
    /// </summary>
    private Vector2D p_sweep
    {
      get { return yStruct.Comparer.p_sweep; }
      set { yStruct.Comparer.p_sweep = value; }
    }

    #region Subsidiary procedures
    /// <summary>
    /// Checking that two inner segments have incident originals
    /// </summary>
    /// <param name="s1">The first segment</param>
    /// <param name="s2">The second segment</param>
    /// <returns>true, if they are incident; false, otherwise</returns>
    private bool AreIncident (InnerSegment s1, InnerSegment s2)
    {
      return incidents.Contains (new SegmentPair (s1.Origin, s2.Origin));
    }

    /// <summary>
    /// Taking the next event from the queue and processing it
    /// </summary>
    private void ProcessNextEvent ()
    {
      Vector2D evPoint;
      Event evInfo;
      
      // Getting the next event
      q.Pop (out evPoint, out evInfo);

      // Processing the existing vertical segments
      foreach (InnerSegment vertSeg in yStruct.GetVerticalSegments ())
      {
        foreach (InnerSegment otherSeg in evInfo)
          if (vertSeg.ID != otherSeg.ID && !AreIncident (vertSeg, otherSeg))
            AddToResult (vertSeg, otherSeg, evPoint);
      }
                                                                                   
      // Procerssing all crossings happening in this point
      IEnumerator<InnerSegment>
        it1 = evInfo.GetEnumerator (),
        it2 = evInfo.GetEnumerator ();
      while (it1.MoveNext())
      {
        InnerSegment s1 = it1.Current;
        it2.Reset ();
        while (it2.MoveNext())
        {
          InnerSegment s2 = it2.Current;
          if (s1.CompareTo (s2) < 0 && !AreIncident (s1, s2) &&
              (Tools.NE (s1.Slope, s2.Slope) || s1.IsEndPoint (evPoint) || s2.IsEndPoint (evPoint)))
            AddToResult (s1, s2, evPoint);
        }
      }

      // Removing all segments ended in this point and check for new crossing events between neighbors 
      // of the segments to be removed
      CrossInfo crInfo;
      SegmentGroup curGroup = new SegmentGroup (), neighGroup;
      foreach (InnerSegment s in evInfo.R)
      {
        if (!s.IsVertical)
        {
          curGroup.Clear ();
          curGroup.Add (s);

          SegmentGroup prev, next;
          if (yStruct.Prev (curGroup, out prev) && yStruct.Next (curGroup, out next))
          {
            foreach (InnerSegment nextSeg in next)
            {
              foreach (InnerSegment prevSeg in prev)
              {
                crInfo = FindIntersection (prevSeg, nextSeg);
                if (crInfo.crossType == CrossType.SinglePoint && crInfo.p > evPoint)
                  q.AddCrossing (crInfo.p, prevSeg, nextSeg);
              }
            }
          }
        }
        yStruct.Remove (s);
      }

      // Removing all segments having crossing here
      foreach (InnerSegment s in evInfo.I)
        yStruct.Remove (s);

      // Moving the sweeping point
      p_sweep = evPoint;

      // Adding back segments having crossings in this point (in new order)
      // Also find the minimal and maximal of them in the sense of the slope
      if (evInfo.I.Count > 0)
      {
        InnerSegment minSeg = evInfo.I.Min (), maxSeg = evInfo.I.Max ();
        double minSlope = minSeg.Slope, maxSlope = maxSeg.Slope, curSlope;
        foreach (InnerSegment s in evInfo.I)
        {
          yStruct.Add (s);
          curSlope = s.Slope;
          if (Tools.LT (curSlope, minSlope))
          {
            minSeg = s;
            minSlope = curSlope;
          }
          if (Tools.GT (curSlope, maxSlope))
          {
            maxSeg = s;
            maxSlope = curSlope;
          }
        }

        // Find possible crossings of the upper and lower segments from the pack 
        // with their corresponding neighbors
        curGroup.Clear ();
        curGroup.Add (maxSeg);
        if (yStruct.Next (curGroup, out neighGroup))
        {
          foreach (InnerSegment neigh in neighGroup)
          {
            crInfo = FindIntersection (maxSeg, neigh);
            if (crInfo.crossType == CrossType.SinglePoint)
            {
              if (crInfo.p > p_sweep)
                q.AddCrossing (crInfo.p, maxSeg, neigh);
            }
            else if (crInfo.crossType == CrossType.Overlap)
            {
              if (crInfo.p > p_sweep)
                q.AddCrossing (crInfo.p, maxSeg, neigh);
              if (crInfo.p1 > p_sweep)
                q.AddCrossing (crInfo.p1, maxSeg, neigh);
            }
          }
        }

        curGroup.Clear ();
        curGroup.Add (minSeg);
        if (yStruct.Prev (curGroup, out neighGroup))
        {
          foreach (InnerSegment neigh in neighGroup)
          {
            crInfo = FindIntersection (minSeg, neigh);
            if (crInfo.crossType == CrossType.SinglePoint)
            {
              if (crInfo.p > p_sweep)
                q.AddCrossing (crInfo.p, minSeg, neigh);
            }
            else if (crInfo.crossType == CrossType.Overlap)
            {
              if (crInfo.p > p_sweep)
                q.AddCrossing (crInfo.p, minSeg, neigh);
              if (crInfo.p1 > p_sweep)
                q.AddCrossing (crInfo.p1, minSeg, neigh);
            }
          }
        }
      }
      
      // Adding to the Y-structure all segments starting here;
      // Check vertical and non-vertical ones separately:
      // - verticals are just added
      // - non-verticals are added and tried to cross with their neighbors
      foreach (InnerSegment s in evInfo.L)
      {
        if (s.IsVertical)
        {
          // Cross this vertical segment with all non-vertical segments in the structure 
          // and put the crossing points to the result
          foreach (InnerSegment oldS in yStruct.GetSlopeSegments())
          {
            crInfo = FindIntersection (oldS, s);
#if DEBUG
            if (crInfo.crossType == CrossType.Overlap)
              throw new Exception ("Got overlapping when crossing vertical and slope segments!");
#endif
            if (crInfo.crossType == CrossType.SinglePoint)
              AddToResult (s, oldS, crInfo.p);
          }
          yStruct.Add (s);
        }
        else
        {
          yStruct.Add (s);

          curGroup.Clear ();
          curGroup.Add (s);

          if (yStruct.Next (curGroup, out neighGroup))
          {
            foreach (InnerSegment neigh in neighGroup)
            {
              crInfo = FindIntersection (s, neigh);
              if (crInfo.crossType == CrossType.SinglePoint)
                q.AddCrossing (crInfo.p, s, neigh);
            }
          }
          if (yStruct.Prev (curGroup, out neighGroup))
          {
            foreach (InnerSegment neigh in neighGroup)
            {
              crInfo = FindIntersection (s, neigh);
              if (crInfo.crossType == CrossType.SinglePoint)
                q.AddCrossing (crInfo.p, s, neigh);
            }
          }
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
    /// <param name="p1">The first intersection point (null, if no)</param>
    /// <param name="p2">The second intersection point (null, if no)</param>
    /// <returns>The information </returns>
    private CrossInfo FindIntersection (InnerSegment s1, InnerSegment s2)
    {
      if (AreIncident (s1, s2))
        return new CrossInfo (CrossType.NoCross, null, null, null, null);
      else
        return Segment.Intersect (s1, s2);
    }

    /// <summary>
    /// Adding to the resultant list crossing of two given segments at the given point
    /// </summary>
    /// <param name="s1">The first segment</param>
    /// <param name="s2">The second segment</param>
    /// <param name="p">The crossing point</param>
    private void AddToResult (InnerSegment s1, InnerSegment s2, Vector2D p)
    {
      if (!ContainsKey (s1.Origin))
        this[s1.Origin] = new SortedSet<CrossData>();
      this[s1.Origin].Add (new CrossData (s1, s2, p));
      if (!ContainsKey (s2.Origin))
        this[s2.Origin] = new SortedSet<CrossData>();
      this[s2.Origin].Add (new CrossData (s2, s1, p));
    }
    #endregion

    #region Constructors
    /// <summary>
    /// Constructor accepting input data and forming the list of crossing points
    /// </summary>
    /// <param name="segs">List of all segments to be crossed</param>
    /// <param name="neighs">List of incident segments which should not be crossed</param>
    public SegmentCrosser (IEnumerable<Segment> segs, SortedSet<SegmentPair> neighs)
      : base ()
    {
      // Storing list of pairs of incident segments
      incidents = neighs;

      // Converting segments to internal representation and computing the infinity
      innerSegs = new List<InnerSegment> ();
      foreach (Segment s in segs)
        innerSegs.Add (new InnerSegment (s));

      // Initializing the event queue
      q = new EventQueue ();
      foreach (InnerSegment s in innerSegs)
        q.AddEnds (s);

      // Starting the main loop
      yStruct = new YStructure ();
      while (q.Count > 0)
        ProcessNextEvent ();
    }
    #endregion
  }
}