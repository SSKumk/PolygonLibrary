using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using PolygonLibrary.Basics;

namespace PolygonLibrary.Toolkit
{
  /// <summary>
  /// A class that contains static methods of different convexification procedures
  /// </summary>
  public class Convexification
  {
    #region Two-dimensional convexification by means of the QuickHull algorithm
    /// <summary>
    /// Construction of a convex hull of a two-dimensional swarm of points; 
    /// the swarm may contain pairs of coinciding points.
    /// QuickHull algorithm is applied
    /// </summary>
    /// <param name="swarm">List of the original swarm</param>
    /// <returns>A list of vertices of the convex hull enlisted counterclockwise</returns>
    public static List<Point2D> QuickHull2D(List<Point2D> swarm)
    {
      List<Point2D> res;
      if (swarm.Count <= 1)
      {
        res = new List<Point2D>();
        if (swarm.Count == 1) {
          res.Add(swarm[0]);
        }

        return res;
      }

      // Seek for minimal and maximal points of the swarm - they define the initial cut line
      Point2D?
        minPoint = swarm.Min(),
        maxPoint = swarm.Max();

      Debug.Assert(minPoint is not null, nameof(minPoint) + " != null");
      Debug.Assert(maxPoint is not null, nameof(maxPoint) + " != null");
      if (minPoint == maxPoint)
      {
        res = new List<Point2D>
          {
            minPoint
          };
        return res;
      }
      Line2D line = new Line2D(maxPoint, minPoint);

      // Dividing the original set of points into subset above and below the line
      // passing through minPoint and maxPoint
      List<Point2D>
        positive = swarm.Where(p => Tools.GT(line[p])).ToList(),
        negative = swarm.Where(p => Tools.LT(line[p])).ToList();

      // Taking envelops of the obtained subsets
      List<Point2D>
        upEnv = QuickHullIter(negative, line, minPoint, maxPoint),
        lowEnv = QuickHullIter(positive, line.Reorient(), maxPoint, minPoint);

      // Constructing the answer: upEnv, minPoint, lowEnv, maxPoint
      res = upEnv;
      res.Add(minPoint);
      res.AddRange(lowEnv);
      res.Add(maxPoint);

      return res;
    }

    /// <summary>
    /// A function supplementary for QuickHull2D: computation the envelope of the given set of points
    /// </summary>
    /// <param name="ps">The given set of points</param>
    /// <param name="line">The base line</param>
    /// <param name="pMin">The "left" base point</param>
    /// <param name="pMax">The "right" base point</param>
    /// <returns></returns>
    private static List<Point2D> QuickHullIter(List<Point2D> ps, Line2D line, Point2D pMin, Point2D pMax)
    {
      // If the list is empty, return empty list
      if (ps.Count == 0) {
        return new List<Point2D>();
      }

      // Find the point farthest from the given line and when, the distances are equal, from the point pMin
      Point2D pBase = ps.Aggregate((acc, p) =>
        {
          double accD = line[acc], pD = line[p];
          if (Tools.LT(accD, pD) || (Tools.EQ(accD, pD) &&
              Tools.GT(Point2D.Dist2(acc, pMin), Point2D.Dist2(p, pMin)))) {
            return acc;
          } else {
            return p;
          }
        });

      // Construct new cutting lines taking into account that the positive halfplane
      // is to the left from the direction vector and the negative one is to the right
      Line2D
        line1 = new Line2D(pMax, pBase),
        line2 = new Line2D(pBase, pMin);

      // Filtering points located in _negative_ halfplanes of the lines
      // and taking their envelops
      List<Point2D>
        arc1 = QuickHullIter(ps.Where(p => Tools.LT(line1[p])).ToList(), line1, pBase, pMax),
        arc2 = QuickHullIter(ps.Where(p => Tools.LT(line2[p])).ToList(), line2, pMin, pBase);

      // Preparing the result: arc1, then the point pBase, then arc2
      List<Point2D> res = arc1;
      res.Add(pBase);
      res.AddRange(arc2);

      return res;
    }
    #endregion

    #region Two-dimensional convexofocation by means of arc algorithm
    /// <summary>
    /// Construction of a convex hull of a two-dimensional swarm of points; 
    /// the swarm may contain pairs of coinciding points.
    /// The boundary is constructed by means of building lower and upper envelops
    /// </summary>
    /// <param name="swarmOrig">List of the original swarm</param>
    /// <returns>A list of vertices of the convex hull enlisted counterclockwise</returns>
		/// <remarks>It is guaranteed that the resultant list contains references
		/// to some of the original objects; no new points are created during work of the method</remarks>
    public static List<Point2D> ArcHull2D(List<Point2D> swarmOrig)
    {
      if (swarmOrig.Count <= 1)
      {
        List<Point2D> res = new List<Point2D>();
        if (swarmOrig.Count == 1) {
          res.Add(swarmOrig[0]);
        }

        return res;
      }

      List<Point2D>
        swarmSort = new List<Point2D>(swarmOrig),
        swarm = new List<Point2D>(),
        upper = new List<Point2D>(),
        lower = new List<Point2D>();

      // Sorting the points
      swarmSort.Sort();

      // Remove the duplicates
      foreach (Point2D p in swarmSort)
      {
        if (swarm.Count == 0 || !swarm[^1].Equals(p)) {
          swarm.Add(p);
        }
      }

      // Degenerated situation of many equal points
      if (swarm.Count == 1) {
        return swarm;
      }

      // Start the arc algorithm
      lower.Add(swarm[0]);
      lower.Add(swarm[1]);
      upper.Add(swarm[0]);
      upper.Add(swarm[1]);

      for (int i = 2; i < swarm.Count; i++)
      {
        Vector2D eLast, eNew;
        bool lastBad;

				// Continue the lower arc
        do
        {
          int
            j_2 = lower.Count - 2,
            j_1 = j_2 + 1;

          eLast = lower[j_1] - lower[j_2];
          eNew = swarm[i] - lower[j_1];

          lastBad = Tools.GE(eNew ^ eLast / eNew.Length / eLast.Length);
          if (lastBad) {
            lower.RemoveRange(j_1, 1);
          }
        } while (lastBad && lower.Count > 1);
        lower.Add(swarm[i]);

				// Continue the upper arc
				do
				{
          int
            j_2 = upper.Count - 2,
            j_1 = j_2 + 1;

          eLast = upper[j_1] - upper[j_2];
          eNew = swarm[i] - upper[j_1];

          lastBad = Tools.LE(eNew ^ eLast / eNew.Length / eLast.Length);
          if (lastBad) {
            upper.RemoveRange(j_1, 1);
          }
        } while (lastBad && upper.Count > 1);
        upper.Add(swarm[i]);
      }

			// Joining the arcs
      upper.Reverse();
      lower.AddRange(upper.GetRange(1, upper.Count - 2));
      return lower;
    }
    #endregion
  }
}
