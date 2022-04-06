using System;
using System.Collections.Generic;
using AVLUtils;
using PolygonLibrary.Basics;

namespace PolygonLibrary.Segments
{
  // Part of the SegmentCrosserBasic class containing the definition of a subsidiary
  // class describing the Y-structure
  public abstract partial class SegmentCrosserBasic
  {
    /// <summary>
    /// The class for keeping the vertical oredered structure of segments
    /// </summary>
    private class YStructure : AVLTreeUnsafe<SegmentGroup>
    {
      /// <summary>
      /// Storage for vertical segments
      /// </summary>
      private SegmentGroup vertSegs;

#region Individual access to segments in the structure
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
#endregion

#region Constructors
      /// <summary>
      /// Default constructor
      /// </summary>
      public YStructure()
      {
        vertSegs = new SegmentGroup();
        SetComparer(new SweepLineSegmentComparer());
        comparer.p_sweep = new Vector2D(-1e38, -1e38);
      }
#endregion

#region Work with sweeping point/line      
      /// <summary>
      /// Getting comparer as a SweepLineSegmentComparer object
      /// </summary>
      new public SweepLineSegmentComparer comparer
      {
        get { return (SweepLineSegmentComparer) base.comparer; }
      }

      /// <summary>
      /// Work with the swep point
      /// </summary>
      public Vector2D SweepPoint
      {
        get { return comparer.p_sweep; }
        set { comparer.p_sweep = value; }
      }
#endregion

#region Adding, removing, getting segments and groups      

      /// <summary>
      /// Search a group with the given polar angle in the structure
      /// </summary>
      /// <param name="angle">The given angle (in radians)</param>
      /// <returns>The group, if there is such; null, otherwise</returns>
      public SegmentGroup FindByAngle(double angle)
      {
        return FindByAngle(new InnerSegment(0, 0, Math.Cos(angle), Math.Sin(angle)));
      }

      /// <summary>
      /// Search a group parallel to the given segment in the structure
      /// </summary>
      /// <param name="s">The given segment</param>
      /// <returns>The group, if there is such; null, otherwise</returns>
      public SegmentGroup FindByAngle(InnerSegment s)
      {
        SegmentGroup g = new SegmentGroup(), resG;
        g.Add(s);
        if (Find(g, out resG))
          return resG;
        else
          return null;
      }

      /// <summary>
      /// Adding a segment choosing where to store it: in main storage or in the vertical segments one
      /// </summary>
      /// <param name="s">The segment to be added</param>
      /// <returns>true, if the segment has been added successfully; false, otherwise</returns>
      public bool Add(InnerSegment s)
      {
        if (s.isVertical)
          return vertSegs.Add(s);
        else
        {
          SegmentGroup resG = FindByAngle(s);
          if (resG != null)
            return resG.Add(s);
          else
          {
            resG = new SegmentGroup();
            resG.Add(s);
            base.Add(resG);
            return true;
          }
        }
      }

      /// <summary>
      /// Removing a segment from a proper storage: from main or from the vertical segments
      /// </summary>
      /// <param name="s">The segment to be removed</param>
      /// <returns>true, if the segment has been removed successfully; false, otherwise</returns>
      public bool Remove(InnerSegment s)
      {
        if (s.isVertical)
          return vertSegs.Remove(s);
        else
        {
          SegmentGroup resG = FindByAngle(s);
          
          if (resG != null)
          {
            bool res = resG.Remove(s);
            if (resG.isEmpty) Remove(resG);
            return res;
          }
          else
            return false;
        }
      }

      /// <summary>
      /// Find the most upper group from the current storage, which crosses the current sweeping
      /// line strictly lower than the given ordinate  
      /// </summary>
      /// <param name="ordinate">The given ordinate</param>
      /// <returns>The reference to the corresponding group or null if all current groups
      /// crosses the sweeping line not lower than the given ordinate</returns>
      public SegmentGroup LowerGroup(double ordinate)
      {
        // If no groups are in the structure, then no group can be found
        if (IsEmpty) return null;

        // The abscissa where to test
        double testX = SweepPoint.x;

        // If the lowest group in the structure is not lower than the given ordinate,
        // then no group can be found
        SegmentGroup minG = Min();
        if (Tools.GE(minG.ComputeAtPoint(testX), ordinate)) return null;

        // If the the most upper group in the structure is lower than the given ordinate,
        // then the result is just that group
        SegmentGroup maxG = Max();
        if (Tools.LT(maxG.ComputeAtPoint(testX), ordinate)) return maxG;

        // Now we are sure that there are more low and more upper groups.
        // Let's start the binary search
        int l = 0, u = Count - 1, m;
        while (l + 1 < u)
        {
          m = (l + u) / 2;
          if (Tools.LT(this[m].ComputeAtPoint(testX), ordinate))
            l = m;
          else
            u = m;
        }

        return this[l];
      }

      /// <summary>
      /// Find the most lower group from the current storage, which crosses the current sweeping
      /// line strictly upper than the given ordinate  
      /// </summary>
      /// <param name="ordinate">The given ordinate</param>
      /// <returns>The reference to the corresponding group or null if all current groups
      /// crosses the sweeping line not upper than the given ordinate</returns>
      public SegmentGroup UpperGroup(double ordinate)
      {
        // If no groups are in the structure, then no group can be found
        if (IsEmpty) return null;

        // The abscissa where to test
        double testX = SweepPoint.x;

        // If the most upper group in the structure is not upper than the given ordinate,
        // then no group can be found
        SegmentGroup maxG = Max();
        if (Tools.LE(maxG.ComputeAtPoint(testX), ordinate)) return null;

        // If the the lowest group in the structure is upper than the given ordinate,
        // then the result is just that group
        SegmentGroup minG = Min();
        if (Tools.GT(minG.ComputeAtPoint(testX), ordinate)) return minG;

        // Now we are sure that there are more low and more upper groups.
        // Let's start the binary search
        int l = 0, u = Count - 1, m;
        while (l + 1 < u)
        {
          m = (l + u) / 2;
          if (Tools.LE(this[m].ComputeAtPoint(testX), ordinate))
            l = m;
          else
            u = m;
        }

        return this[u];
      }

#endregion
    }
  }
}