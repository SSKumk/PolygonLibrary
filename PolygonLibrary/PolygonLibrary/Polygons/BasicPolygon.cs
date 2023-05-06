using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
    protected List<Polyline>? _contours;

    /// <summary>
    /// The storage for the polygon vertices
    /// </summary>
    protected List<Point2D>? _vertices;

    /// <summary>
    /// The storage for the polygon edges
    /// </summary>
    protected List<Segment>? _edges;
    #endregion

    #region Access properties
    /// <summary>
    /// Property giving a list of the polygon contours
    /// </summary>
    public List<Polyline> Contours 
    {
      get
      {
        Debug.Assert(!IsEmpty, "Try to get contours of an empty polygon!");

        if (_contours == null) {
          ComputeContours();
        }
        
        Debug.Assert(_contours != null, "List of contours of a polygon cannot be null");
        Debug.Assert(_contours.Count != 0, "List of contours of a polygon cannot be empty");
        
        return _contours;
      }
      protected init => _contours = value;
    }

    /// <summary>
    /// Property giving a list of the polygon vertices
    /// (for all contours)
    /// </summary>
    public List<Point2D> Vertices
    {
      get
      {
        Debug.Assert(!IsEmpty, "Try to get vertices of an empty polygon!");
      
        if (_vertices == null) {
          ComputeVertices();
        }

        Debug.Assert(_vertices != null, nameof(_vertices) + " != null");
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
        Debug.Assert(!IsEmpty, "Try to get edges of an empty polygon!");

        if (_edges == null) {
          ComputeEdges();
        }

        Debug.Assert(_edges != null, nameof(_edges) + " != null");
        return _edges;
      }
      protected set => _edges = value;
    }

    /// <summary>
    /// Property showing whether the polygon is empty
    /// </summary>
    public bool IsEmpty { get; protected init; }
    #endregion

    #region Constructors and factories

    /// <summary>
    /// Default constructor creates an empty polygon
    /// </summary>
    public BasicPolygon() {
      IsEmpty = true;
    }

    /// <summary>
    /// Single contour polygon construction on the basis of list of its vertices
    /// </summary>
    /// <param name="vs">List of vertices in the contour</param>
    /// <param name="checkOrient">Flag showing whether the counterclockwise orientation should be checked</param>
    /// <param name="checkCross">Flag showing whether the self-crossing of the contour should be checked</param>
    public BasicPolygon(IEnumerable<Point2D> vs, bool checkOrient = true, bool checkCross = true)
    {
      // The only contour is initialized just here, because now we have information about it - 
      // the order of points in the initial list
      _contours = new List<Polyline>();
      _contours.Add(new Polyline(vs, PolylineOrientation.Counterclockwise, checkCross, checkOrient));

      _edges = null;
      _vertices = null;
      
      IsEmpty = false;
    }

    /// <summary>
    /// Single contour polygon construction on the basis of array of its vertices
    /// </summary>
    /// <param name="vs">Array of vertices in the contour</param>
    /// <param name="checkOrient">Flag showing whether the counterclockwise orientation should be checked</param>
    /// <param name="checkCross">Flag showing whether the self-crossing of the contour should be checked</param>
    public BasicPolygon(Point2D[] vs, bool checkOrient = true, bool checkCross = true)
      : this(vs.ToList(), checkOrient, checkCross) { }
    #endregion

    #region Internal methods
    /// <summary>
    /// On demand computation of a list of vertices (ordered in some way) on the basis of the array of contours.
    /// If the array of contours is not initialized, an exception is thrown
    /// </summary>
    protected virtual void ComputeVertices()
    {
      if (_vertices == null)
      {
#if DEBUG
        if (_contours == null) {
          throw new Exception("Cannot compute vertices - contours have not been initialized");
        }
#endif
        Vertices = new List<Point2D>();
        foreach (Polyline p in _contours) {
          Vertices.AddRange(p.Vertices);
        }
      }
    }

    /// <summary>
    /// On demand computation of sorted list of edges on the basis of the array of contours.
    /// If the array of contours is not initialized, an exception is thrown
    /// </summary>
    protected virtual void ComputeEdges()
    {
      if (_edges == null)
      {
#if DEBUG
        if (_contours == null) {
          throw new Exception("Cannot compute edges - contours have not been initialized");
        }
#endif
        Edges = new List<Segment>();
        foreach (Polyline p in _contours) {
          Edges.AddRange(p.Edges);
        }

        Edges.Sort();
      }
    }

    /// <summary>
    /// A method for reconstructing contours if they are not defined when constructing the polygon
    /// </summary>
    protected abstract void ComputeContours();
    

    #endregion

    #region Common polygon tools
    /// <summary>
    /// Method checking whether this polygon contains a given point
    /// </summary>
    /// <param name="p">The point to be checked</param>
    /// <returns>true, if the point is inside the polygon; false, otherwise</returns>
    public abstract bool Contains(Point2D p);

    /// <summary>
    /// Method checking whether this polygon contains a given point in the interior of the polygon
    /// </summary>
    /// <param name="p">The point to be checked</param>
    /// <returns>true, if the point is strictly inside the polygon; false, otherwise</returns>
    public abstract bool ContainsInside(Point2D p);
    #endregion

  }
}
