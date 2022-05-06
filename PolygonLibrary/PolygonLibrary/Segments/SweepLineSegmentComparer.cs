using System;
using System.Collections.Generic;
using PolygonLibrary.Basics;

namespace PolygonLibrary.Segments
{
  // Part of the SegmentCrosserBasic class containing the definition of a subsidiary
  // class for comparing segment groups in the y-structure according to the current
  // position of the sweeping point
  public abstract partial class SegmentCrosserBasic
  {
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
      /// <param name="g1">The first group to be compared</param>
      /// <param name="g2">The second group to be compared</param>
      /// <returns>-1, is g1 is less than g2; +1, if g1 is greater than g2; 0, otherwise</returns>
      public int Compare(SegmentGroup g1, SegmentGroup g2)
      {
#if DEBUG
        // Empty groups cannot appear in the comparer because it is impossible
        // to find the slope and vertical position of such a group.
        // If an empty group appears, then throw an exception
        if (g1.isEmpty || g2.isEmpty) {
          throw new ArgumentException("An empty group in segment groups comparer in y-structure");
        }

        // Check whether one of the group is vertical.
        // If so, than throw an exception
        if (g1.isVertical || g2.isVertical) {
          throw new ArgumentException("A vertical group in segment groups comparer in y-structure");
        }
#endif

        double
          y1 = g1.ComputeAtPoint(p_sweep.x),
          y2 = g2.ComputeAtPoint(p_sweep.x);

        if (Tools.NE(y1, y2))
        {
          if (Tools.GT(y1, y2)) {
            return +1;
          } else {
            return -1;
          }
        }

        // They have equal ordinates. Check their slopes
        double sl1 = g1.polarAngle, sl2 = g2.polarAngle;
        if (Tools.EQ(sl1, sl2))
          // They have equal slopes - that is, the groups are equal
        {
          return 0;
        } else
        {
          // They have diferent slopes. Compare them taking into account their position 
          // with respect to the sweeping point

          // At first, compare them by the right slopes
          int res;
          if (Tools.LT(sl1, sl2)) {
            res = -1;
          } else {
            res = +1;
          }

          // If they are lower than the sweeping point, change the order
          if (Tools.GT(y1, p_sweep.y)) {
            res = -res;
          }

          return res;
        }
      }
    }
  }
}