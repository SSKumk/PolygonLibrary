using System;
using System.Collections;
using System.Collections.Generic;
using AVLUtils;

namespace PolygonLibrary.Segments
{
  // Part of the SegmentCrosserBasic class containing the definition of a subsidiary
  // class for storing a collection of segments (presumably, with the same polar angle)
  public abstract partial class SegmentCrosserBasic
  {
    /// <summary>
    /// Class of group of segments. Keeps the segments in two storages:
    ///   - in declared segment order
    ///   - descently sorted by their "right" ends 
    /// </summary>
    protected internal class SegmentGroup : IMultiEnumerable<InnerSegment>, IComparable<SegmentGroup>
    {
#region Storages and the reverse comparer       
      /// <summary>
      /// The storage keeping the segments in the usual segment order
      /// ("left" ends are compared and then the "right" ends)
      /// </summary>
      private AVLSet<InnerSegment> accOrdered;

      /// <summary>
      /// The storage keeping the segments in the "reverse" order
      /// </summary>
      private AVLSet<InnerSegment> decOrdered;

      /// <summary>
      /// A class of a comparer of two InnerSegment-s in reverse order:
      /// one segement is less than another one if its "right" end is greater
      /// than the "right" end of the another; if the "right" ends coincide,
      /// then the "left" end should be greater than the "left" end of the another.  
      /// </summary>
      private class ReverseComparer : IComparer<InnerSegment>
      {
        /// <summary>
        /// The corresponding comparer
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <returns></returns>
        public int Compare(InnerSegment s1, InnerSegment s2)
        {
          int res = -s2.p2.CompareTo(s1.p2);
          if (res != 0) {
            return res;
          } else {
            return -s2.p1.CompareTo(s1.p1);
          }
        }
      }
#endregion

#region Constructors      
      /// <summary>
      /// The default constructor
      /// </summary>
      public SegmentGroup()
      {
        accOrdered = new AVLSet<InnerSegment>();
        decOrdered = new AVLSet<InnerSegment>(new ReverseComparer());
      }
#endregion

#region Access properties and methods      
      /// <summary>
      /// Number of segments in the group
      /// </summary>
      public int count => accOrdered.Count;

      /// <summary>
      /// Check whether the group is empty
      /// </summary>
      public bool isEmpty => accOrdered.Count == 0;

      /// <summary>
      /// Check whether the group consists of vertical segments 
      /// </summary>
      /// <exception cref="InvalidOperationException">Is thrown when the group is empty (in debug regime)</exception>
      public bool isVertical
      {
        get
        {
#if DEBUG
          if (isEmpty) {
            throw new InvalidOperationException("Getting slope of an empty group!");
          }
#endif
          return accOrdered.Min().isVertical;
        }
      }

      /// <summary>
      /// The slope of the group
      /// </summary>
      /// <exception cref="InvalidOperationException">Is thrown when the group is empty (in debug regime)</exception>
      public double polarAngle
      {
        get
        {
#if DEBUG
          if (isEmpty) {
            throw new InvalidOperationException("Getting slope of an empty group!");
          }
#endif
          return accOrdered.Min().polarAngle;
        }
      }

      /// <summary>
      /// Return the minimal (in the sense of direct order) segment of the group
      /// </summary>
      public InnerSegment Min() => accOrdered.Min();

      /// <summary>
      /// Return the maximal (minimal in the sense of "reverse" order) segment of the group
      /// </summary>
      public InnerSegment Max() => decOrdered.Min();

      /// <summary>
      /// Adding a segment to the group
      /// </summary>
      /// <param name="s">The segment to be added</param>
      /// <returns>true, if the segment is added succesfully; false, otherwise</returns>
      public bool Add(InnerSegment s)
      {
        if (accOrdered.Add(s))
        {
          decOrdered.Add(s);
          return true;
        }
        else {
          return false;
        }
      }

      /// <summary>
      /// Adding a group of segments to the group
      /// </summary>
      /// <param name="g">The group to be added</param>
      /// <returns>true, if all segments from the given group have been added succesfully;
      /// false, otherwise</returns>
      public bool Add(SegmentGroup g)
      {
        bool res = true;
        foreach (InnerSegment s in g) {
          res = res && Add(s);
        }

        return res;
      }
      
      /// <summary>
      /// Removing a segment from the group
      /// </summary>
      /// <param name="s">The segment to be removed</param>
      /// <returns>true, if the segment has been removed; false, otherwise</returns>
      public bool Remove(InnerSegment s)
      {
        if (accOrdered.Remove(s))
        {
          decOrdered.Remove(s);
          return true;
        }
        else {
          return false;
        }
      }

      /// <summary>
      /// Checks whether the structure contains the given segment
      /// </summary>
      /// <param name="s">The given segment</param>
      /// <returns>true, if the given segment is in the structure; false, otherwise</returns>
      public bool Contains(InnerSegment s) => accOrdered.Contains(s);

      /// <summary>
      /// Intersecting two groups in the sense of intersecting two segments
      /// each is convex hull of the corresponding group 
      /// </summary>
      /// <param name="g1">The first group to be intersected</param>
      /// <param name="g2">The second group to be intersected</param>
      /// <returns>The crossing information (number of intersection points
      /// and the points themselves)</returns>
      public static CrossInfo Intersect(SegmentGroup g1, SegmentGroup g2)
      {
        Segment
          s1 = new Segment(g1.Min().p1, g1.Max().p2),
          s2 = new Segment(g2.Min().p1, g2.Max().p2);
        return Segment.Intersect(s1, s2);
      }
#endregion

#region IMultiEnumerable<TValue> and related methods
      /// <summary>
      /// Returns an enumerator that directly iterates through the collection.
      /// </summary>
      /// <returns>An enumerator that can be used to iterate through the collection.</returns>
      public IEnumerator<InnerSegment> GetEnumerator() => accOrdered.GetEnumerator();

      /// <summary>
      /// Returns an untyped enumerator that directly iterates through the collection.
      /// </summary>
      /// <returns>An enumerator that can be used to iterate through the collection.</returns>
      IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

      /// <summary>
      /// Returns an enumerator that iterates through the collection put at the given value or after it 
      /// (if there is no such a value in the collection)
      /// </summary>
      /// <param name="v">The value the enymerator to be put on</param>
      /// <returns>An enumerator that can be used to iterate through the collection</returns>
      public IEnumerator<InnerSegment> GetEnumerator(InnerSegment v) => accOrdered.GetEnumerator(v);

      /// <summary>
      /// Returns an enumerator that reversely iterates through the collection.
      /// </summary>
      /// <returns>An enumerator that can be used to iterate through the collection.</returns>
      public IEnumerator<InnerSegment> GetReverseEnumerator() => accOrdered.GetReverseEnumerator();

      /// <summary>
      /// Returns an enumerator that reversely iterates through the collection put at the given value or before it 
      /// (if there is no such a value in the collection)
      /// </summary>
      /// <param name="v">The value the enymerator to be put on</param>
      /// <returns>An enumerator that can be used to iterate through the collection</returns>
      public IEnumerator<InnerSegment> GetReverseEnumerator(InnerSegment v) => accOrdered.GetReverseEnumerator(v);

      /// <summary>
      /// Returns an enumerator that directly iterates through the collection regarding it as a cycled one
      /// </summary>
      /// <returns>An enumerator that directly iterates through the collection regarding it as a cycled one</returns>
      public IEnumerator<InnerSegment> GetCyclicEnumerator() => accOrdered.GetCyclicEnumerator();

      /// <summary>
      /// Returns an enumerator that directly iterates through the collection regarding it as a cycled one;
      /// initially the enumerator is put to the given value or (if it is absent) to minimal value cyclicly 
      /// greater than the given one
      /// </summary>
      /// <param name="v">The value the enymerator to be put on</param>
      /// <returns>An enumerator that directly iterates through the collection regarding it as a cycled one</returns>
      public IEnumerator<InnerSegment> GetCyclicEnumerator(InnerSegment v) => accOrdered.GetCyclicEnumerator(v);

      /// <summary>
      /// Returns an enumerator that reversely iterates through the collection regarding it as a cycled one
      /// </summary>
      /// <returns>An enumerator that reversely iterates through the collection regarding it as a cycled one</returns>
      public IEnumerator<InnerSegment> GetCyclicReverseEnumerator() => accOrdered.GetCyclicReverseEnumerator();

      /// <summary>
      /// Returns an enumerator that reversely iterates through the collection regarding it as a cycled one;
      /// initially the enumerator is put to the given value or (if it is absent) to maximal value cyclicly 
      /// less than the given one
      /// </summary>
      /// <param name="v">The value the enymerator to be put on</param>
      /// <returns>An enumerator that iterates reversely through the collection regarding it as a cycled one</returns>
      public IEnumerator<InnerSegment> GetCyclicReverseEnumerator(InnerSegment v) => accOrdered.GetCyclicReverseEnumerator(v);
      #endregion

#region Comparation
      public int CompareTo(SegmentGroup that) => this.polarAngle.CompareTo(that.polarAngle);
      #endregion

#region Miscellaneous procedures
      /// <summary>
      /// Compute the ordinate of the line passing through the group at the given abscissa;
      /// For groups of vertical segments an exception is raised
      /// </summary>
      /// <param name="x">The abscissa where to compute</param>
      /// <returns>The corresponding ordinate</returns>
      public double ComputeAtPoint(double x)
      {
#if DEBUG
        if (isEmpty) {
          throw new InvalidOperationException("Cannot compute ordinate for an empty group!");
        }
#endif
        InnerSegment s = Min();
#if DEBUG
        if (s.isVertical) {
          throw new InvalidOperationException("Cannot compute ordinate for a group of vertical segments!");
        }
#endif

        return s.ComputeAtPoint(x);
      }
#endregion
    }
  }
}