using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AVLUtils;
using PolygonLibrary.Toolkit;
using PolygonLibrary.Basics;

namespace PolygonLibrary
{
  namespace Segments
  {
    // Class for internal representation of the segment
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
      public SegmentPair(Segment ns1, Segment ns2)
      {
        if (ns1.CompareTo(ns2) < 0)
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
      public int CompareTo(SegmentPair other)
      {
        int res = s1.CompareTo(other.s1);
        if (res != 0)
          return res;
        else
          return s2.CompareTo(other.s2);
      }
    }

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
    public abstract class SegmentCrosserBasic
    {
      #region Subsidiary classes
      internal protected class InnerSegment : Segment, IEquatable<InnerSegment>, IComparable<InnerSegment>
      {
        #region Global counters and IDs
        /// <summary>
        /// The counter of the added segments 
        /// </summary>
        private static int curID = 0;

        /// <summary>
        /// Number of the segment in the entire collection
        /// </summary>
        protected readonly int myID;

        /// <summary>
        /// Reading property for the ID
        /// </summary>
        public int ID { get { return myID; } }
        #endregion

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
          myID = curID;
          curID++;

          Origin = null;
        }

        /// <summary>
        /// The only constructor on the basis of a segment
        /// </summary>
        /// <param name="s">The original segment</param>
        /// <param name="newID">Possible ID of the new segment; 
        /// if no new ID is given, then ID is taken from the global counter</param>
        public InnerSegment(Segment s, int newID = -1)
          : base()
        {
          if (newID == -1)
            myID = curID;
          else
            myID = newID;
          curID++;

          Origin = s;

          // Proper orientation of the segment
          _p1 = s.p1;
          _p2 = s.p2;
          if (_p1 > _p2)
            Tools.Swap(ref _p1, ref _p2);

          ComputeParameters();
        }
        #endregion

        #region Overrided procedures
        /// <summary>
        /// String representation of the inner segment
        /// </summary>
        /// <returns>The string representation</returns>
        public override string ToString()
        {
          return base.ToString() + ", ID = " + myID;
        }

        /// <summary>
        /// Comparison with other inner segment. 
        /// At first, we compare the objects as segments 
        /// (taking into account that the first end is less than the second one). 
        /// In the case, of equality, compare IDs
        /// </summary>
        /// <param name="other">The inner segment to be comapared with</param>
        /// <returns>-1, if this object is less than the other; +1, if this object is greater; 0, if they are equal</returns>
        public int CompareTo(InnerSegment other)
        {
          int res = base.CompareTo(other);
          if (res != 0)
            return res;
          else
            return myID.CompareTo(other.myID);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the other parameter; otherwise, false</returns>
        public bool Equals(InnerSegment other)
        {
          return CompareTo(other) == 0;
        }
        #endregion
      }

      /// <summary>
      /// The type of an event; contains segments starting, ending and crossing in this point
      /// </summary>
      private class Event : IEnumerable<InnerSegment>
      {
        #region Storages
        /// <summary>
        /// Set of segments having here their left ends
        /// </summary>
        public AVLSet<InnerSegment> L;

        /// <summary>
        /// Set of segments having here their right ends
        /// </summary>
        public AVLSet<InnerSegment> R;

        /// <summary>
        /// Set of segments passing through the point 
        /// </summary>
        public AVLSet<InnerSegment> I;

        private CollectionOfEnumerables<InnerSegment> allSegs;
        #endregion

        #region Conctructor
        public Event()
        {
          L = new AVLSet<InnerSegment>();
          R = new AVLSet<InnerSegment>();
          I = new AVLSet<InnerSegment>();
          allSegs = new CollectionOfEnumerables<InnerSegment>(L, R, I);
        }
        #endregion

        #region Work with iterator
        public IEnumerator<InnerSegment> GetEnumerator()
        {
          return allSegs.GetEnumerator();
        }

        /// <summary>
        /// Getting a non-generic enumerator
        /// </summary>
        /// <returns>A non-generic enumerator set to the beginning of the collection</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
          return GetEnumerator();
        }

        /// <summary>
        /// Get enumerator set to the first occurence of the given segment in the collection 
        /// of segments associated with the event. If the given segment is not associated with the event,
        /// then the method returns an enumerator set to the beginning of the collection
        /// </summary>
        /// <param name="s">A segment to which the enumerator should be set to</param>
        /// <returns>An enumerator set to the given segment, if the segment is in the collection;
        /// or to the beginning of the collection, otherwise</returns>
        public IEnumerator<InnerSegment> GetEnumerator(InnerSegment s)
        {
          return allSegs.GetEnumerator(s);
        }
        #endregion
      }

      /// <summary>
      /// Class for event queue
      /// </summary>
      private class EventQueue : SortedDictionary<Point2D, Event>
      {
        /// <summary>
        /// Adding events of segment ends
        /// </summary>
        /// <param name="s">The segment, which ends should be added</param>
        public void AddEnds(InnerSegment s)
        {
          if (!ContainsKey(s.p1))
            Add(s.p1, new Event());
          this[s.p1].L.Add(s);

          if (!ContainsKey(s.p2))
            Add(s.p2, new Event());
          this[s.p2].R.Add(s);
        }

        /// <summary>
        /// Adding an event of crossing two segments
        /// </summary>
        /// <param name="p">The point of crossing</param>
        /// <param name="s1">The first segment</param>
        /// <param name="s2">The second segment</param>
        public void AddCrossing(Point2D p, InnerSegment s1, InnerSegment s2)
        {
          AddPassing(p, s1);
          AddPassing(p, s2);
        }

        /// <summary>
        /// Add to an event at the given point (that is created if necessary)
        /// the given segment
        /// </summary>
        /// <param name="p"></param>
        /// <param name="s"></param>
        public void AddPassing(Point2D p, InnerSegment s)
        {
          CreateEvent(p);
          Event e = this[p];
          if (!e.I.Contains(s) && !e.L.Contains(s) && !e.R.Contains(s))
            e.I.Add(s);
        }

        /// <summary>
        /// Crerate an event at the given point (if necessary)
        /// </summary>
        /// <param name="p">The point</param>
        public void CreateEvent(Point2D p)
        {
          if (!ContainsKey(p))
            Add(p, new Event());
        }

        /// <summary>
        /// Take the minimal value from the dictionary and remove it from the storage
        /// </summary>
        /// <param name="p">The point of the event</param>
        /// <param name="e">The event information</param>
        public void Pop(out Point2D p, out Event e)
        {
#if DEBUG
          if (Count == 0)
            throw new ArgumentException("The queue is empty!");
#endif
          KeyValuePair<Point2D, Event> pair = this.First();
          p = pair.Key;
          e = pair.Value;
          Remove(p);
        }
      }

      /// <summary>
      /// Class of group of segments
      /// </summary>
      internal protected class SegmentGroup : AVLSet<InnerSegment>
      {
        /// <summary>
        /// Property for getting the line, which contains the first segment of the group
        /// (useful in the case when the group consists of colinear segments)
        /// </summary>
        public Line2D GroupLine
        {
          get
          {
            if (this.Count == 0)
              throw new Exception("Cannot get the line of an empty group!");
            return new Line2D(Min());
          }
        }

        /// <summary>
        /// Method that checks whether the group line passes through given point
        /// </summary>
        /// <param name="p">The given point</param>
        /// <returns>true, if the group line passes through the point; false, otherwise</returns>
        public bool PassesThrough(Point2D p)
        {
          return GroupLine.PassesThrough(p);
        }
      }

      /// <summary>
      /// Class which realizes correct ordering of segments for current state of the sweeping line
      /// </summary>
      private class SweepLineSegmentComparer : IComparer<SegmentGroup>
      {
        /// <summary>
        /// The point defining the current state of the sweeping line
        /// </summary>
        public Point2D p_sweep = new Point2D(-1e38, -1e38);

        /// <summary>
        /// The comparer
        /// </summary>
        /// <param name="s1">The first group to be compared</param>
        /// <param name="s2">The second group to be compared</param>
        /// <returns>-1, is s1 < s2; +1, if s1 > s2; 0, otherwise</returns>
        public int Compare(SegmentGroup g1, SegmentGroup g2)
        {
          // Empty groups cannot appear in the comparer because it is impossible
          // to find the slope and vertical position of such a group.
          // If an empty group appears, then throw an exception
          if (g1.Count == 0 || g2.Count == 0)
            throw new ArgumentException("An empty group in segment groups comparer in Y-structure");

          // Compare their intersection ordinates with the sweep line
          InnerSegment s1 = g1.Min(), s2 = g2.Min();

          double
            y1 = s1.ComputeAtPoint(p_sweep.x),
            y2 = s2.ComputeAtPoint(p_sweep.x);

          if (Tools.NE(y1, y2))
          {
            if (Tools.GT(y1, y2))
              return +1;
            else
              return -1;
          }

          // They have equal ordinates. Check their slopes
          double sl1 = s1.Slope, sl2 = s2.Slope;
          if (Tools.EQ(sl1, sl2))
            // They have equal slopes - that is, the groups are equal
            return 0;
          else
          {
            // They have diferent slopes. Compare them taking into account their position 
            // with respect to the sweeping point

            // At first, compare them by the right slopes
            int res;
            if (Tools.LT(sl1, sl2))
              res = -1;
            else
              res = +1;

            // If they are lower than the sweeping point, change the order
            if (Tools.GT(y1, p_sweep.y))
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
        private AVLSet<InnerSegment> vertSegs;

        /// <summary>
        /// Getting all current vertical segments
        /// </summary>
        /// <returns>A collection of the verical segments</returns>
        public IEnumerable<InnerSegment> GetVerticalSegments()
        {
          foreach (InnerSegment s in vertSegs)
            yield return s;
        }

        /// <summary>
        /// Getting all current slope segments
        /// </summary>
        /// <returns>A collection of the slope segments</returns>
        public IEnumerable<InnerSegment> GetSlopeSegments()
        {
          foreach (SegmentGroup g in this)
            foreach (InnerSegment s in g)
              yield return s;
        }

        /// <summary>
        /// Getting all slope groups 
        /// </summary>
        /// <returns>A collection of slope groups</returns>
        public IEnumerable<SegmentGroup> GetSlopeGroups()
        {
          foreach (SegmentGroup g in this)
            yield return g;
        }

        /// <summary>
        /// Get a group passing through the given point. 
        /// It is necessary that the Y-structure order at the line passing 
        /// through the current sweeping point would be the same as at the line
        /// passing through the given point
        /// </summary>
        /// <param name="p">The given point</param>
        /// <returns>Reference to such a group, or null if none are passing</returns>
        public SegmentGroup GetSlopeGroupPassingThrough(Point2D p)
        {
          int lInd, uInd, testInd;
          double val;

          if (this.Count == 0)
            return null;

          // Check whether all groups are passing upper than the current sweep point
          val = new Line2D(Min().Min())[p];
          if (Tools.EQ(val))
            return Min();
          else if (Tools.LT(val))
            return null;

          // Check whether all groups are passing lower than the current sweep point
          val = new Line2D(Max().Min())[p];
          if (Tools.EQ(val))
            return Max();
          else if (Tools.GT(val))
            return null;

          // Seek for the lower boundary of such groups
          lInd = 0;
          uInd = this.Count;
          while (uInd > lInd + 1)
          {
            testInd = (lInd + uInd) / 2;
            val = new Line2D(this[testInd].Min())[p];
            if (Tools.EQ(val))
              return this[testInd];
            else if (Tools.LT(val))
              uInd = testInd;
            else
              lInd = testInd;
          }

          // If we have reached this point, then the sweep point is upper than the lInd-th group,
          // but lower than the uInd-th group. Therefor, no groups are passing throgh the point
          return null;
        }

        #region Constructors
        /// <summary>
        /// Default constructor
        /// </summary>
        public YStructure()
          : base()
        {
          vertSegs = new AVLSet<InnerSegment>();
          SetComparer(new SweepLineSegmentComparer());
          comparer.p_sweep = new Point2D(-1e38, -1e38);
        }
        #endregion

        /// <summary>
        /// Getting comparer as a SweepLineSegmentComparer object
        /// </summary>
        new public SweepLineSegmentComparer comparer
        {
          get { return (SweepLineSegmentComparer)base.comparer; }
        }

        /// <summary>
        /// Work with the swep point
        /// </summary>
        public Point2D SweepPoint
        {
          get { return comparer.p_sweep; }
          set { comparer.p_sweep = value; }
        }

        /// <summary>
        /// Adding a segment choosing where to store it: in the main storage or in the vertical segments one
        /// </summary>
        /// <param name="s">The segment to be added</param>
        /// <returns>true, if the segment has been added successfully; false, otherwise</returns>
        public bool Add(InnerSegment s)
        {
          if (s.IsVertical)
            return vertSegs.Add(s);
          else
          {
            SegmentGroup g = new SegmentGroup(), resG;
            g.Add(s);
            if (Find(g, out resG))
            {
              if (resG.Contains(s))
                return false;
              Remove(resG);
              bool res = resG.Add(s);
              Add(resG);
              return res;
            }
            else
            {
              base.Add(g);
              return true;
            }
          }
        }

        /// <summary>
        /// Removing a segment from a proper storage: from the main or from the vertical segments
        /// </summary>
        /// <param name="s">The segment to be removed</param>
        /// <returns>true, if the segment has been removed successfully; false, otherwise</returns>
        public bool Remove(InnerSegment s)
        {
          if (s.IsVertical)
            return vertSegs.Remove(s);
          else
          {
            SegmentGroup g = new SegmentGroup(), resG;
            g.Add(s);
            if (Find(g, out resG))
            {
              if (!resG.Contains(s))
                return false;
              Remove(resG);
              bool res = resG.Remove(s);
              if (resG.Count > 0)
                Add(resG);
              return res;
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
        public SegmentGroup GetGroup(InnerSegment s)
        {
          SegmentGroup g = new SegmentGroup(), resG;
          g.Add(s);
          if (Find(g, out resG))
            return resG;
          else
            return null;
        }
      }
      #endregion

      #region Internal data
      /// <summary>
      /// Storage for internal segments
      /// </summary>
      List<InnerSegment> innerSegs = new List<InnerSegment>();

      /// <summary>
      /// Reference to the list of pairs of incident segments
      /// </summary>
      SortedSet<SegmentPair> incidents = new SortedSet<SegmentPair>();

      /// <summary>
      /// The queue of events
      /// </summary>
      EventQueue q = new EventQueue();

      /// <summary>
      /// Y-ordered segments crossing the current sweeping line
      /// </summary>
      YStructure yStruct = new YStructure();
      #endregion

      /// <summary>
      /// Property to work with the sweeping point
      /// </summary>
      private Point2D p_sweep
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
        Point2D evPoint;
        Event evInfo;

        // Getting the next event
        q.Pop(out evPoint, out evInfo);

        /*
         * Procession event algorithm is the following:
         *  1) Generation of intersection information.
         *     The only place where intersection information is generated!!!
         *     There are three situation of crossing in this point:
         *     a) all pairs of segments having this point as either the start, 
         *        or the finish 
         *     b) all segments having this point as either the start, 
         *        or the finish with segments passing through the point
         *        (the latters are taken from the current list of vertical segments 
         *        from the Y-structure and, additionally, either from the I-field 
         *        of the event record if it is not empty, or from the Y-structure;
         *        in the latter case, there is only one such a group)
         *     c) all pairs of segments passing through the point that have different 
         *        slope (difference of slopes is checked for each pair individually)
         *  
         *  2) All segments that have end here are extracted from the Y-structure;
         *     after any extraction upper and lower groups are taken and their lines
         *     are intersected; the intersection point is tried to be added to the 
         *     events queue
         *     
         *  3) Move the current point of Y-structue to the event point and reorder 
         *     the Y-structure properly; for the latter, remove all segments that 
         *     are passing through the event point before moving the current point
         *     and insert them after
         *     
         *  4) All segments that are starting here are added to the Y-structure;
         *     they are tried to be crossed with their upper and lower neighbouring
         *     groups (also in line manner), and also inside their group if it exists
         */

        /*
         *  1) Generation of intersection information.
         *     The only place where intersection information is generated!!!
         *     There are three situation of crossing in this point:
         *     a) all pairs of segments having this point as either the start, 
         *        or the finish 
         *     b) all segments having this point as either the start, 
         *        or the finish with segments passing through the point
         *        (the latters are taken from the current list of vertical segments 
         *        from the Y-structure and, additionally, either from the I-field 
         *        of the event record if it is not empty, or from the Y-structure;
         *        in the latter case, there is only one such a group)
         *     c) all pairs of segments passing through the point that have different 
         *        slope (difference of slopes is checked for each pair individually)
         */

        /*
         *  1) Generation of intersection information.
         *     a) all pairs of segments having this point as either the start, 
         *        or the finish 
         */
        IEnumerator<InnerSegment> it1, it2;
        InnerSegment s1, s2;
        CollectionOfEnumerables<InnerSegment> SandF =
          new CollectionOfEnumerables<InnerSegment>(evInfo.L, evInfo.R);
        it1 = SandF.GetEnumerator();
        while (it1.MoveNext())
        {
          s1 = it1.Current;
          it2 = SandF.GetEnumerator(s1);
          while (it2.MoveNext())
            if (!AreIncident(s1, it2.Current))
              AddToResult(s1, it2.Current, evPoint);
        }

        /*
         *  1) Generation of intersection information. 
         *     b) all segments having this point as either the start, 
         *        or the finish with segments passing through the point
         *        (the latters are taken from the current list of vertical segments 
         *        from the Y-structure and, additionally, either from the I-field 
         *        of the event record if it is not empty, or from the Y-structure;
         *        in the latter case, there is only one such a group)
         */
        CollectionOfEnumerables<InnerSegment> Passing;
        if (evInfo.I.Count == 0)
          Passing = new CollectionOfEnumerables<InnerSegment>(
            yStruct.GetSlopeGroupPassingThrough(evPoint), 
            yStruct.GetVerticalSegments());
        else
          Passing = new CollectionOfEnumerables<InnerSegment>(evInfo.I, yStruct.GetVerticalSegments());
        it1 = SandF.GetEnumerator();
        while (it1.MoveNext())
        {
          s1 = it1.Current;
          it2 = Passing.GetEnumerator();
          while (it2.MoveNext())
          {
            s2 = it2.Current;
            if (!s1.Equals(s2) && !s2.IsEndPoint(evPoint) && !AreIncident(s1, s2))
              AddToResult(s1, s2, evPoint);
          }
        }


        /*
         *  1) Generation of intersection information.
         *     c) all pairs of segments passing through the point that have different 
         *        slope (difference of slopes is checked for each pair individually)
         */
        it1 = Passing.GetEnumerator();
        while (it1.MoveNext())
        {
          s1 = it1.Current;
          it2 = Passing.GetEnumerator(s1);
          while (it2.MoveNext())
          {
            s2 = it2.Current;
            if (!s2.IsEndPoint(evPoint) && Tools.NE(s1.Slope, s2.Slope) && !AreIncident(s1, s2))
              AddToResult(s1, s2, evPoint);
          }
        }

        /*
         *  2) All segments that have end here are extracted from the Y-structure;
         *     after any extraction upper and lower groups are taken and their lines
         *     are intersected; the intersection point is tried to be added to the 
         *     events queue
         */
        SegmentGroup curGroup = new SegmentGroup(), neighGroup;
        foreach (InnerSegment s in evInfo.R)
        {
          if (!s.IsVertical)
          {
            curGroup.Clear();
            curGroup.Add(s);

            SegmentGroup prev, next;
            if (yStruct.Prev(curGroup, out prev) && yStruct.Next(curGroup, out next))
            {
              Line2D l1 = prev.GroupLine, l2 = next.GroupLine;
              Point2D cross;

              if (Line2D.Intersect(l1, l2, out cross) == Line2D.LineCrossType.SinglePoint &&
                cross > evPoint)
              {
                foreach (InnerSegment nextSeg in next)
                  if (nextSeg.ContainsPoint(cross))
                    q.AddPassing(cross, nextSeg);
                foreach (InnerSegment prevSeg in prev)
                  if (prevSeg.ContainsPoint(cross))
                    q.AddPassing(cross, prevSeg);
              }
            }
          }
          yStruct.Remove(s);
        }

        /*
         *  3) Move the current point of Y-structue to the event point and reorder 
         *     the Y-structure properly; for the latter, remove all segments that 
         *     are passing through the event point before moving the current point
         *     and insert them after
         */

        // Removing from the Y-structure all slope segments 
        // passing through the point of the current event
        foreach (InnerSegment s in evInfo.I)
          yStruct.Remove(s);

        // Moving the sweeping point
        p_sweep = evPoint;

        // Adding back to the Y-structure segments having crossings in this point (in new order)
        // Also find the minimal and maximal of them in the sense of the slope
        if (evInfo.I.Count > 0)
        {
          double minSlope = 1e38, maxSlope = -1e38, curSlope;
          List<InnerSegment>
            minSegs = new List<InnerSegment>(),
            maxSegs = new List<InnerSegment>();
          foreach (InnerSegment s in evInfo.I)
          {
            yStruct.Add(s);
            if (!s.IsVertical)
            {
              curSlope = s.Slope;
              if (Tools.LT(curSlope, minSlope))
              {
                minSegs.Clear();
                minSegs.Add(s);
                minSlope = curSlope;
              }
              else if (Tools.EQ(curSlope, minSlope))
                minSegs.Add(s);

              if (Tools.GT(curSlope, maxSlope))
              {
                maxSegs.Clear();
                maxSegs.Add(s);
                maxSlope = curSlope;
              }
              else if (Tools.EQ(curSlope, maxSlope))
                maxSegs.Add(s);
            }
          }

          // Find possible crossings of the groups of upper and lower segments from the pack 
          // with their corresponding neighbors
          if (maxSegs.Count > 0)
          {
            curGroup.Clear();
            curGroup.Add(maxSegs[0]);
            if (yStruct.Next(curGroup, out neighGroup))
            {
              Line2D l1 = new Line2D(maxSegs[0]);
              Point2D cross;
              if (Line2D.Intersect(l1, neighGroup.GroupLine, out cross) == Line2D.LineCrossType.SinglePoint &&
                cross > evPoint && maxSegs.Any(s => s.ContainsPoint(cross)) &&
                neighGroup.Any(s => s.ContainsPoint(cross)))
              {
                foreach (InnerSegment seg in maxSegs)
                  if (seg.ContainsPoint(cross))
                    q.AddPassing(cross, seg);
                foreach (InnerSegment neigh in neighGroup)
                  if (neigh.ContainsPoint(cross))
                    q.AddPassing(cross, neigh);
              }
            }
          }

          if (minSegs.Count > 0)
          {
            curGroup.Clear();
            curGroup.Add(minSegs[0]);
            if (yStruct.Prev(curGroup, out neighGroup))
            {
              Line2D l1 = new Line2D(minSegs[0]);
              Point2D cross;
              if (Line2D.Intersect(l1, neighGroup.GroupLine, out cross) == Line2D.LineCrossType.SinglePoint &&
                cross > evPoint && minSegs.Any(s => s.ContainsPoint(cross)) &&
                neighGroup.Any(s => s.ContainsPoint(cross)))
              {
                foreach (InnerSegment seg in minSegs)
                  if (seg.ContainsPoint(cross))
                    q.AddPassing(cross, seg);
                foreach (InnerSegment neigh in neighGroup)
                  if (neigh.ContainsPoint(cross))
                    q.AddPassing(cross, neigh);
              }
            }
          }
        }

        /*
         *  4) All segments that are starting here are added to the Y-structure;
         *     they are tried to be crossed with their upper and lower neighbouring
         *     groups, and also inside their group if it exists
         */
        // Adding to the Y-structure all segments starting here;
        // Check vertical and non-vertical ones separately:
        // - verticals are just added
        // - non-verticals are added and tried to cross with their neighbors
        foreach (InnerSegment s in evInfo.L)
        {
          // Intersecting the segment inside its group (if it exists)
          if (s.IsVertical)
          {
            foreach (InnerSegment vertSeg in yStruct.GetVerticalSegments())
            {
              if (vertSeg.ContainsPoint(s.p2))
                q.AddPassing(s.p2, vertSeg);
              if (s.ContainsPoint(vertSeg.p2))
                q.AddPassing(vertSeg.p2, s);
            }

            // Crossing the vertical segment with all slope groups passing 
            // through the current vertical line
            foreach (SegmentGroup gr in yStruct.GetSlopeGroups())
            {
              InnerSegment seg = gr.Min();
              double yCross = seg.ComputeAtPoint(evPoint.x);
              if (Tools.LE(yCross, s.p2.y) && Tools.GT(yCross, s.p1.y))
                q.AddCrossing(new Point2D(evPoint.x, yCross), seg, s);
            }

            yStruct.Add(s);
          }
          else
          {
            yStruct.Add(s);
            SegmentGroup segGroup = yStruct.GetGroup(s);
            foreach (InnerSegment seg in segGroup)
            {
              if (seg.ContainsPoint(s.p2))
                q.AddPassing(s.p2, seg);
              if (s.ContainsPoint(seg.p2))
                q.AddPassing(seg.p2, s);
            }

            // Non-vertical segment is tried to intersect with its neghbouring group
            Line2D l1 = new Line2D(s);

            for (int ii = 1; ii >= 0; ii--)
            {
              bool res = ii == 1 ? yStruct.Next(segGroup, out neighGroup) :
                yStruct.Prev(segGroup, out neighGroup);

              if (res)
              {
                Point2D cross;
                Line2D l2 = neighGroup.GroupLine;
                if (Line2D.Intersect(l1, l2, out cross) == Line2D.LineCrossType.SinglePoint &&
                  cross > evPoint && s.ContainsPoint(cross))
                {
                  foreach (InnerSegment neigh in neighGroup)
                  {
                    if (neigh.ContainsPoint(cross))
                      q.AddCrossing(cross, s, neigh);
                  }
                }
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
      private CrossInfo FindIntersection(InnerSegment s1, InnerSegment s2)
      {
        if (AreIncident(s1, s2))
          return CrossInfo.NoCrossConst;
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
      /// Adding to the resultant data structure crossing of two given segments at the given point.
      /// ID field of InnerSegment data contains the zero-based indices of the original segments
      /// in the IEnumerable passed to the constructor of the class
      /// </summary>
      /// <param name="s1">The first segment</param>
      /// <param name="s2">The second segment</param>
      /// <param name="p">The crossing point</param>
      protected abstract void AddToResult(InnerSegment s1, InnerSegment s2, Point2D p);
      #endregion
    }
  }
}
