using System;

namespace PolygonLibrary.Segments
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
    public Segment s1 { get; }

    /// <summary>
    /// The second (greater) segment in the pair
    /// </summary>
    public Segment s2 { get; }

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
      if (res != 0) {
        return res;
      } else {
        return s2.CompareTo(other.s2);
      }
    }
  }
}