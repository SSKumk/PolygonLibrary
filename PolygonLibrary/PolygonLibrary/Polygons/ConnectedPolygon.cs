using System;
using System.Collections.Generic;
using PolygonLibrary.Basics;

namespace PolygonLibrary.Polygons
{
  /// <summary>
  /// Class of a connected polygon, which is represented as an outer bounding contour
  /// oriented counterclockwise and a number of contours that define holes oriented
  /// clockwise. It is assumed that different contours have at most a zero-measure 
  /// intersection (boundary points and/or segments, and/or polylines); all "hole" 
  /// polygons are embedded to the interior of the outer contour, but not each to other
  /// </summary>
  public class ConnectedPolygon
  {
    #region Storages and accesors
    /// <summary>
    /// Stirage of polygon's contours. It is assumed that the contour with index 0
    /// is the outer one and the ones with greater indices are the holes
    /// </summary>
    private List<Polyline> _ls;

    /// <summary>
    /// Getting number of contours in this polygon
    /// </summary>
    public int Count => _ls.Count;

    /// <summary>
    /// Getting the outer contour of the polygon
    /// </summary>
    public Polyline OuterContour => _ls[0];

    /// <summary>
    /// Indexer for accessing contours of the polygon
    /// </summary>
    /// <param name="i">Index of the contour. The zeroth contour os assumed to be the outer one,
    /// others are the holes</param>
    /// <returns>The contour with the given index</returns>
    public Polyline this[int i]
    {
      get
      {
#if DEBUG
        if (i >= _ls.Count) {
          throw new IndexOutOfRangeException();
        }
#endif
        return _ls[i];
      }
    }
    #endregion

    #region Constructors and factories
    /// <summary>
    /// The default constuctor. Produces a dot polygon at the origin
    /// </summary>
    public ConnectedPolygon()
    {
      _ls = new List<Polyline>();
      _ls.Add(new Polyline());
    }

    /// <summary>
    /// Constructor with varying number of contours 
    /// </summary>
    /// <param name="outer">The outer contour</param>
    /// <param name="checkIntersections">if true, then there is a check that different contours
    /// intersect at most by their boundaries</param>
    /// <param name="checkOrientation">if true, then there is a check that contours are
    /// properly oriented</param>
    /// <param name="holes">The contours of holes</param>
    public ConnectedPolygon (Polyline outer, 
      bool checkIntersections = true, bool checkOrientation = true,
      params Polyline[] holes)
    {
      // !!! ToDo: Insert checks for correctness of input data !!!
      _ls = new List<Polyline>();
      _ls.Add(outer);
      _ls.AddRange(holes);
    }
    #endregion

    #region Miscelaneous methods
    /// <summary>
    /// Method that checks whether a point belongs to the polygon, that is belongs to the outer
    /// contour and doesn't belong to others
    /// </summary>
    /// <param name="p">The point to be checked</param>
    /// <returns>true, of the point is inside the polygon; false, otherwise</returns>
    public bool ContainsPoint (Point2D p)
    {
      if (!this[0].ContainsPoint(p)) {
        return false;
      }

      for (int i = 1; i < Count; i++) {
        if (this[i].ContainsPointInside(p)) {
          return false;
        }
      }

      return true;
    }
    #endregion
  }
}
