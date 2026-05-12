using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PolygonLibrary.Basics;

namespace PolygonLibrary.Toolkit
{
  /// <summary>
  /// A class that contains static methods of different convexifixaction procedures
  /// </summary>
  public abstract class Convexification
  {
    #region Two-dimensional convexification by means of the QuickHull algorithm
    /// <summary>
    /// Construction of a convex hull of a two-dimensional swarm of points; 
    /// the swarm may contain pairs of coinciding points.
    /// QuickHull algorithm is applied
    /// </summary>
    /// <param name="swarm">List of the original swarm</param>
    /// <returns>A list of vertices of the convex hull enlisted counterclockwise</returns>
    static public List<Point2D> QuickHull2D (List<Point2D> swarm)
    {
      List<Point2D> res;
      if (swarm.Count <= 1)
      {
        res = new List<Point2D>();
        if (swarm.Count == 1)
          res.Add (swarm[0]);
        return res;
      }

      // Seek for minimal and maximal points of the swarm - they define the initial cut line
      Point2D 
        minPoint = swarm.Min(),
        maxPoint = swarm.Max();

      if (minPoint == maxPoint)
      {
        res = new List<Point2D>();
        res.Add(minPoint);
        return res;
      }
      Line2D line = new Line2D (maxPoint, minPoint);

      // Dividing the original set of points into subset above and below the line
      // passing through minPoint and maxPoint
      List<Point2D>
        positive = swarm.Where(p => Tools.GT(line[p])).ToList(),
        negative = swarm.Where(p => Tools.LT(line[p])).ToList();

      // Taking envelops of the obtained subsets
      List<Point2D>
        upEnv = Convexification.QuickHullIter(negative, line, minPoint, maxPoint),
        lowEnv = Convexification.QuickHullIter(positive, line.Reorient(), maxPoint, minPoint);

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
    static private List<Point2D> QuickHullIter (List<Point2D> ps, Line2D line, Point2D pMin, Point2D pMax)
    {
      // If the list is empty, return empty list
      if (ps.Count == 0)
        return new List<Point2D>();

      // Find the point farest from the given line and when, the distances are equal, from the point pMin
      Point2D pBase = ps.Aggregate((acc, p) =>
        {
          double accD = line[acc], pD = line[p];
          if (Tools.LT(accD, pD) || (Tools.EQ(accD, pD) && 
              Tools.GT(Point2D.Dist2(acc, pMin), Point2D.Dist2(p, pMin))))
            return acc;
          else
            return p;
        });

      // Construct new cutting lines taking into account that the positive halfplane
      // is to the left from the direction vector and the negative one is to the right
      Line2D
        line1 = new Line2D(pMax, pBase),
        line2 = new Line2D(pBase, pMin);

      // Filtering points located in _negative_ halfplanes of the lines
      // and taking their envelops
      List<Point2D>
        arc1 = Convexification.QuickHullIter (ps.Where (p => Tools.LT(line1[p])).ToList(), 
          line1, pBase, pMax),
        arc2 = Convexification.QuickHullIter (ps.Where (p => Tools.LT(line2[p])).ToList(), 
          line2, pMin, pBase);

      // Preparing the result: arc1, then the point pBase, then arc2
      List<Point2D> res = arc1;
      res.Add(pBase);
      res.AddRange(arc2);

      return res;
    }
    #endregion
  }
}
