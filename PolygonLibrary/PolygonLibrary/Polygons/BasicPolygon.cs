using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PolygonLibrary.Basics;
using PolygonLibrary.Segments;
using PolygonLibrary.Toolkit;

namespace PolygonLibrary.Polygons
{
  /// <summary>
  /// Class declaring interface for a polygon and implementing some basic methods
  /// </summary>
  public abstract class BasicPolygon
  {
    #region Internal storages
    /// <summary>
    /// The storage for the contours of the polygon
    /// </summary>
    protected List<Polyline> _contours = null;

    /// <summary>
    /// The storage for the polygon vertices
    /// </summary>
    protected List<Point2D> _vertices = null;

    /// <summary>
    /// The storage for the polygon edges
    /// </summary>
    protected List<Segment> _edges = null;
    #endregion

    #region Access properties
    /// <summary>
    /// Property giving a list of the polygon contours
    /// </summary>
    public List<Polyline> Contours
    {
      get
      {
        if (_contours == null)
          ComputeContours();
        return _contours;
      }
      protected set => _contours = value;
    }

    /// <summary>
    /// Property giving a list of the polygon vertices
    /// (for all contours)
    /// </summary>
    public List<Point2D> Vertices
    {
      get
      {
        if (_vertices == null)
          ComputeVertices();
        return _vertices;
      }
      protected set => _vertices = value;
    }

    /// <summary>
    /// Property giving a list of the polygon edges
    /// (for all contours)
    /// </summary>
    public List<Segment> Edges
    {
      get
      {
        if (_edges == null)
          ComputeEdges();
        return _edges;
      }
      protected set => _edges = value;
    }
    #endregion

    #region Constructors
    /// <summary>
    /// Default constructor doing nothing
    /// </summary>
    public BasicPolygon() { }

    /// <summary>
    /// Single contour polygon construction on the basis of list of its vertices
    /// </summary>
    /// <param name="vs">List of vertices in the contour</param>
    /// <param name="checkOrient">Flag showing whether the counterclockwise orientation should be checked</param>
    /// <param name="checkCross">Flag showing whether the selfcrossing of the contour should be checked</param>
    public BasicPolygon(List<Point2D> vs, bool checkOrient = true, bool checkCross = true)
    {
      // The only contour is initialized just here, because now we have information about it - 
      // the order of points in the initial list
      Contours = new List<Polyline>();
      Contours.Add(new Polyline(vs, PolylineOrientation.Counterclockwise, checkCross, checkOrient));

      // Copying the list of vertices
      Vertices = new List<Point2D>(vs);
      Vertices.Sort();
    }

    /// <summary>
    /// Single contour polygon construction on the basis of array of its vertices
    /// </summary>
    /// <param name="vs">Array of vertices in the contour</param>
    /// <param name="checkOrient">Flag showing whether the counterclockwise orientation should be checked</param>
    /// <param name="checkCross">Flag showing whether the selfcrossing of the contour should be checked</param>
    public BasicPolygon(Point2D[] vs, bool checkOrient = true, bool checkCross = true)
      : this(vs.ToList(), checkOrient, checkCross) { }
    #endregion

    #region Internal methods
    /// <summary>
    /// On demand computation of sorted list of vertices on the basis of the array of contours.
    /// If the array of contours is not initializaed, an exception is thrown
    /// </summary>
    protected virtual void ComputeVertices()
    {
      if (_vertices == null)
      {
#if DEBUG
        if (_contours == null)
          throw new Exception("Cannot compute vertices - contours have not been initialized");
#endif
        Vertices = new List<Point2D>();
        foreach (Polyline p in _contours)
          Vertices.AddRange(p.Vertices);

        Vertices.Sort();
      }
    }

    /// <summary>
    /// On demand computation of sorted list of edges on the basis of the array of contours.
    /// If the array of contours is not initializaed, an exception is thrown
    /// </summary>
    protected virtual void ComputeEdges()
    {
      if (_edges == null)
      {
#if DEBUG
        if (_contours == null)
          throw new Exception("Cannot compute edges - contours have not been initialized");
#endif
        Edges = new List<Segment>();
        foreach (Polyline p in _contours)
          Edges.AddRange(p.Edges);

        Edges.Sort();
      }
    }

    /// <summary>
    /// On demand computation of a single contoured convex polygon on the basis of the array 
    /// of vertices. If the array of vertices is not initializaed, an exception is thrown
    /// </summary>
    protected virtual void ComputeContours()
    {
      if (_contours == null)
      {
#if DEBUG
        if (_vertices == null)
          throw new Exception("Cannot compute contour - vertices have not been initialized");
#endif
        _contours = new List<Polyline>();
        _contours.Add(new Polyline(Convexification.ArcHull2D(_vertices),
          PolylineOrientation.Counterclockwise, false, false));
      }
    }
    #endregion

    #region Common polygon tools
    /// <summary>
    /// Method checking whether this polygon contains a given point
    /// </summary>
    /// <param name="p">The point to be checked</param>
    /// <returns>true, if the point is inside the polygon; false, otherwise</returns>
    public abstract bool Contains(Point2D p);

    //public static P Rearrange<P> (P orig, SegmentCrosser2 crosses)
    #endregion
  }
}
