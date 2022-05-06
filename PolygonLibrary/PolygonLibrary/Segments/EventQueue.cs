using System;
using System.Collections.Generic;
using System.Linq;
using PolygonLibrary.Basics;

namespace PolygonLibrary.Segments
{
  // Part of the SegmentCrosserBasic class containing the definition of a subsidiary
  // class for storing information the event queue and working with it 
  public abstract partial class SegmentCrosserBasic
  {
    /// <summary>
    /// Class for event queue
    /// </summary>
    private class EventQueue : SortedDictionary<Vector2D, Event>
    {
      /// <summary>
      /// Adding events of segment ends
      /// </summary>
      /// <param name="s">The segment, which ends should be added</param>
      public void AddEnds(InnerSegment s)
      {
        if (!ContainsKey(s.p1)) {
          Add(s.p1, new Event());
        }

        this[s.p1].L.Add(s);

        if (!ContainsKey(s.p2)) {
          Add(s.p2, new Event());
        }

        this[s.p2].R.Add(s);
      }

      /// <summary>
      /// Adding an event of crossing two segments
      /// </summary>
      /// <param name="p">The point of crossing</param>
      /// <param name="s1">The first segment</param>
      /// <param name="s2">The second segment</param>
      public void AddCrossing(Vector2D p, InnerSegment s1, InnerSegment s2)
      {
        if (!ContainsKey(p)) {
          Add(p, new Event());
        }

        Event e = this[p];
        if (!e.I.Contains(s1) && !e.L.Contains(s1) && !e.R.Contains(s1)) {
          e.I.Add(s1);
        }

        if (!e.I.Contains(s2) && !e.L.Contains(s2) && !e.R.Contains(s2)) {
          e.I.Add(s2);
        }
      }

      /// <summary>
      /// Take the minimal value from the dictionary and remove it from the storage
      /// </summary>
      /// <param name="p">The point of the event</param>
      /// <param name="e">The event information</param>
      public void Pop(out Vector2D p, out Event e)
      {
#if DEBUG
        if (Count == 0) {
          throw new ArgumentException("The queue is empty!");
        }
#endif
        KeyValuePair<Vector2D, Event> pair = this.First();
        p = pair.Key;
        e = pair.Value;
        Remove(p);
      }
    }
  }
}
