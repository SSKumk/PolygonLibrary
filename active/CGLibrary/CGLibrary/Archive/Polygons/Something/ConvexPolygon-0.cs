using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PolygonLibrary.Basics;
using PolygonLibrary.Toolkit;

namespace PolygonLibrary.Polygons
{
  /// <summary>
  /// Class of a convex polygon
  /// </summary>
  public class ConvexPolygon : SimplePolygon
  {
    #region Constructors
    /// <summary>
    /// Default constructor. Produces a one-point polygon at the origin
    /// </summary>
    public ConvexPolygon () 
    {
      _vertices = new List<Point2D>();
      _vertices.Add(Point2D.Origin);
    }

    /// <summary>
    /// Contructor producing a polygon as a convex hull of the given set of points
    /// </summary>
    /// <param name="ps">The given set of points</param>
    public ConvexPolygon (List<Point2D> ps) 
    {
      _vertices = Convexification.QuickHull2D(ps);
    }

    /// <summary>
    /// Copying constructor
    /// </summary>
    /// <param name="polygon">The polygon to be copied</param>
    public ConvexPolygon (ConvexPolygon polygon)
    {
      _vertices = polygon._vertices;
    }
    #endregion
  }
}
