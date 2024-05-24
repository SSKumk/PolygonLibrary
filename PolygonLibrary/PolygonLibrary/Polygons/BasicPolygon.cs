using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;

namespace CGLibrary;

public partial class Geometry<TNum, TConv> where TNum : class, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  /// <summary>
  /// Class declaring interface for a polygon and implementing some basic methods
  /// </summary>
  public abstract class BasicPolygon {

#region Internal storages
    /// <summary>
    /// The storage for the contours of the polygon
    /// </summary>
    protected List<Polyline>? _contours;

    /// <summary>
    /// The storage for the polygon vertices
    /// </summary>
    protected List<Vector2D>? _vertices;

    /// <summary>
    /// The storage for the polygon edges
    /// </summary>
    protected List<Segment>? _edges;
#endregion

#region Access properties
    /// <summary>
    /// Property giving a list of the polygon contours
    /// </summary>
    public List<Polyline> Contours //todo Привести к единообразному виду как в Segment
    {
      get
        {
          if (_contours == null) {
            ComputeContours();
          }

          Debug.Assert(_contours != null, nameof(_contours) + " != null");

          return _contours;
        }
      protected init => _contours = value;
    }

    /// <summary>
    /// Property giving a list of the polygon vertices
    /// (for all contours)
    /// </summary>
    public List<Vector2D> Vertices {
      get
        {
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
    public List<Segment> Edges {
      get
        {
          if (_edges == null) {
            ComputeEdges();
          }

          Debug.Assert(_edges != null, nameof(_edges) + " != null");

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
    /// <param name="checkCross">Flag showing whether the self-crossing of the contour should be checked</param>
    public BasicPolygon(List<Vector2D> vs, bool checkOrient = true, bool checkCross = true) {
      // The only contour is initialized just here, because now we have information about it -
      // the order of points in the initial list
      Contours = new List<Polyline> { new Polyline(vs, PolylineOrientation.Counterclockwise, checkCross, checkOrient) };

      // Copying the list of vertices
      Vertices = new List<Vector2D>(vs);
      // Vertices.Sort(); todo ???
    }

    /// <summary>
    /// Single contour polygon construction on the basis of array of its vertices
    /// </summary>
    /// <param name="vs">Array of vertices in the contour</param>
    /// <param name="checkOrient">Flag showing whether the counterclockwise orientation should be checked</param>
    /// <param name="checkCross">Flag showing whether the self-crossing of the contour should be checked</param>
    public BasicPolygon(Vector2D[] vs, bool checkOrient = true, bool checkCross = true) : this
      (vs.ToList(), checkOrient, checkCross) { }
#endregion

#region Internal methods
    /// <summary>
    /// On demand computation of lexicographically sorted list of vertices on the basis of the array of contours.
    /// If the array of contours is not initialized, an exception is thrown
    /// </summary>
    protected virtual void ComputeVertices() {
      if (_vertices == null) {
#if DEBUG
        if (_contours == null) {
          throw new Exception("Cannot compute vertices - contours have not been initialized");
        }
#endif
        Vertices = new List<Vector2D>();
        foreach (Polyline p in _contours!) {
          Vertices.AddRange(p.Vertices);
        }

        // Vertices.Sort(); todo ???
      }
    }

    /// <summary>
    /// On demand computation of sorted list of edges on the basis of the array of contours.
    /// If the array of contours is not initialized, an exception is thrown
    /// </summary>
    protected virtual void ComputeEdges() {
      if (_edges == null) {
#if DEBUG
        if (_contours == null) {
          throw new Exception("Cannot compute edges - contours have not been initialized");
        }
#endif
        Edges = new List<Segment>();
        foreach (Polyline p in _contours!) {
          Edges.AddRange(p.Edges);
        }

        Edges.Sort();
      }
    }

    /// <summary>
    /// On demand computation of a single contoured convex polygon on the basis of the array
    /// of vertices. If the array of vertices is not initialized, an exception is thrown
    /// </summary>
    protected virtual void ComputeContours() {
      if (_contours == null) {
#if DEBUG
        if (_vertices == null) {
          throw new Exception("Cannot compute contour - vertices have not been initialized");
        }
#endif
        _contours = new List<Polyline>
          {
            new Polyline(Convexification.ArcHull2D(_vertices!), PolylineOrientation.Counterclockwise, false, false)
          };
      }
    }
#endregion

#region Common polygon tools
    /// <summary>
    /// Method checking whether this polygon contains a given point
    /// </summary>
    /// <param name="p">The point to be checked</param>
    /// <returns>true, if the point is inside the polygon; false, otherwise</returns>
    public abstract bool Contains(Vector2D p);

    /// <summary>
    /// Method checking whether this polygon contains a given point in the interior of the polygon
    /// </summary>
    /// <param name="p">The point to be checked</param>
    /// <returns>true, if the point is strictly inside the polygon; false, otherwise</returns>
    public abstract bool ContainsInside(Vector2D p);

    //public static P Rearrange<P> (P orig, SegmentCrosser2 crosses)
#endregion

  }

}
