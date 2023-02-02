using System;
using System.Collections.Generic;
using PolygonLibrary.Basics;

namespace PolygonLibrary.Polygons
{
  /// <summary>
  /// Class of simple polygon
  /// </summary>
  class SimplePolygon : BasicPolygon
  {
    /// <summary>
    /// Single contour polygon construction on the basis of list of its vertices
    /// </summary>
    /// <param name="vs">List of vertices in the contour</param>
    /// <param name="checkOrient">Flag showing whether the counterclockwise orientation should be checked</param>
    /// <param name="checkCross">Flag showing whether the selfcrossing of the contour should be checked</param>
    public SimplePolygon(List<Point2D> vs, bool checkOrient = true, bool checkCross = true)
      : base(vs, checkOrient, checkCross) { }

    /// <summary>
    /// Single contour polygon construction on the basis of array of its vertices
    /// </summary>
    /// <param name="vs">Array of vertices in the contour</param>
    /// <param name="checkOrient">Flag showing whether the counterclockwise orientation should be checked</param>
    /// <param name="checkCross">Flag showing whether the selfcrossing of the contour should be checked</param>
    public SimplePolygon(Point2D[] vs, bool checkOrient = true, bool checkCross = true)
      : base(vs, checkOrient, checkCross) { }

    /// <summary>
    /// Method checking whether this polygon contains a given point
    /// </summary>
    /// <param name="p">The point to be checked</param>
    /// <returns>true, if the point is inside the polygon; false, otherwise</returns>
    public override bool Contains (Point2D p) => throw new NotImplementedException();
  }
}
