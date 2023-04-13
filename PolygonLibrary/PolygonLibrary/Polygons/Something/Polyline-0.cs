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
  /// Enumeration of a polyline
  /// </summary>
  public enum PolylineOrientation
  {
    /// <summary>
    /// Positive counterclockwise orientation
    /// </summary>
    Counterclockwise,

    /// <summary>
    /// Negative rclockwise orientation
    /// </summary>
    Clockwise
  }

  /// <summary>
  /// Class of a closed polyline
  /// </summary>
  public class Polyline
  {
    #region Data storage and accessors
    /// <summary>
    /// The list of vertices of the polygon enlisted counterclockwise
    /// </summary>
    protected List<Point2D> _vertices;

    /// <summary>
    /// Number of vertices
    /// </summary>
    public int Count { get { return _vertices.Count; } }

    /// <summary>
    /// Getting a vertex by index. The index is counted cyclicly!
    /// </summary>
    /// <param name="i">The index of the vertex. The index is counted cyclicly!</param>
    /// <returns>The poiint of the vertex</returns>
    public Point2D this[int i]
    {
      get
      {
        int ind = i % _vertices.Count;
        return _vertices[ind >= 0 ? ind : ind + _vertices.Count];
      }
    }

    /// <summary>
    /// Orientation of the polyline
    /// </summary>
    public PolylineOrientation Orientation { get; protected set; }

    /// <summary>
    /// Storage for the square of the polygon bounded by the polyline.
    /// If null, then the square has not been computed yet
    /// </summary>
    protected double? _square;

    /// <summary>
    /// Property that returns the square of the polygon
    /// </summary>
    public double Square
    {
      get
      {
        if (!_square.HasValue)
          _square = ComputeSquare();
        return _square.Value;
      }
    }
    #endregion

    #region Constructors
    /// <summary>
    /// Default constructor. Produces a one-point polyline at the origin
    /// </summary>
    public Polyline()
    {
      _vertices = new List<Point2D>();
      _vertices.Add(Point2D.Origin);
      Orientation = PolylineOrientation.Counterclockwise;
      _square = null;
    }

    /// <summary>
    /// Constructor producing a polyline in the basis of set its vertices
    /// enumerated in the proper order. The order is declared by user.
    /// By default, the correctness is checked: computation of all intersections
    /// of the segments checks that the polyline bounds a simple polygon;
    /// computation of the signed square of the simple polygon checks 
    /// the correctness of the orientation
    /// </summary>
    /// <param name="ps">The set of vertices</param>
    /// <param name="orient">Declared orientation of the polyline</param>
    /// <param name="checkSimplicity">Flag showing whether the simplicity of the corresponding polygon 
    /// should be checked</param>
    /// <param name="checkOrientation">Flag showing that the polyline orientation should be checked</param>
    public Polyline(List<Point2D> ps, PolylineOrientation orient,
      bool checkSimplicity = true, bool checkOrientation = true)
    {
      // ToDo:  Write checks !!!
      _vertices = ps;
      Orientation = orient;
    }

    /// <summary>
    /// Constructor producing a polyline in the basis of an array of its vertices
    /// enumerated in the proper order. The order is declared by user.
    /// By default, the correctness is checked: computation of all intersections
    /// of the segments checks that the polyline bounds a simple polygon;
    /// computation of the signed square of the simple polygon checks 
    /// the correctness of the orientation
    /// </summary>
    /// <param name="ps">The set of vertices</param>
    /// <param name="orient">Declared orientation of the polyline</param>
    /// <param name="checkSimplicity">Flag showing whether the simplicity of the corresponding polygon 
    /// should be checked</param>
    /// <param name="checkOrientation">Flag showing that the polyline orientation should be checked</param>
    public Polyline(Point2D[] ps, PolylineOrientation orient,
      bool checkSimplicity = true, bool checkOrientation = true) :
      this(new List<Point2D>(ps), orient, checkSimplicity, checkOrientation) { }
    #endregion

    #region Miscelaneous methods
    /// <summary>
    /// Check whether the line is actually empty, that is, it does not contain any vertex
    /// </summary>
    public bool IsEmpty { get { return Count == 0; } }

    /// <summary>
    /// Compute square of the current polyline
    /// </summary>
    /// <returns>The square</returns>
    private double ComputeSquare()
    {
      double res = 0;
      for (int i = 0; i < Count; i++)
        res += this[i].x * this[i + 1].y - this[i].y * this[i + 1].x;
      return 0.5 * res;
    }

    /// <summary>
    /// Method that checks whether a point belongs to the polygon
    /// </summary>
    /// <param name="p">The point to be checked</param>
    /// <returns>true, if the point is in the polygon (inside or at the boundary); 
    /// false, otherwise</returns>
    public bool ContainsPoint(Point2D p)
    {
      return Tools.NE(ComputeAngleVariation(p));
    }

    /// <summary>
    /// Method that checks whether a point is located strictly inside the polygon
    /// </summary>
    /// <param name="p">The point to be checked</param>
    /// <returns>true, if the point is strictly inside the polygon</returns>
    public bool ContainsPointInside(Point2D p)
    {
      return Tools.EQ(Math.Abs(ComputeAngleVariation(p)), 2 * Math.PI);
    }

    /// <summary>
    /// A suplementary method for the ones that check whether the polygon contains a point.
    /// Computes variation of angle along the contour with respect to the point
    /// </summary>
    /// <param name="p">The base point</param>
    /// <returns>The variation of the angle</returns>
    private double ComputeAngleVariation(Point2D p)
    {
      double res = 0;
      Vector2D vi = this[0] - p, vim1;
      for (int i = 1; i <= Count; i++)
      {
        vim1 = vi;
        vi = this[i] - p;
        double psp = vim1 ^ vi;
        if (Tools.NE(psp))
          res += Math.Atan2(psp, vim1 * vi);
      }
      return res;
    }

    /// <summary>
    /// Compute the polar angle of the given edge of the polyline
    /// </summary>
    /// <param name="i">Cyclic index of the edge</param>
    /// <returns>The angle</returns>
    protected double EdgeAngle (int i)
    {
      Point2D a = this[i], b = this[i+1];
      return Math.Atan2 (b.y - a.y, b.x - a.x);
    }
    #endregion
  }

  /// <summary>
  /// Contour is a polyline, which remembers to what polygon it belongs
  /// </summary>
  /// <typeparam name="P">Type of parent polygon</typeparam>
  public class Contour<P> : Polyline
    where P : BasicPolygon
  {
    #region Constructors
    /// <summary>
    /// Default constructor. Produces a one-point contour at the origin
    /// </summary>
    /// <param name="parent">Parent polygon</param>
    public Contour(P parent) : base ()
    {
      Polygon = parent;
    }

    /// <summary>
    /// Constructor producing a contour in the basis of set its vertices
    /// enumerated in the proper order. The order is declared by user.
    /// By default, the correctness is checked: computation of all intersections
    /// of the segments checks that the contour bounds a simple polygon;
    /// computation of the signed square of the simple polygon checks 
    /// the correctness of the orientation
    /// </summary>
    /// <param name="parent">Parent polygon</param>
    /// <param name="ps">The set of vertices</param>
    /// <param name="orient">Declared orientation of the contour</param>
    /// <param name="checkSimplicity">Flag showing whether the simplicity of the corresponding polygon 
    /// should be checked</param>
    /// <param name="checkOrientation">Flag showing that the contour orientation should be checked</param>
    public Contour(P parent, List<Point2D> ps, PolylineOrientation orient,
      bool checkSimplicity = true, bool checkOrientation = true) : 
        base (ps, orient, checkSimplicity, checkOrientation)
    {
      Polygon = parent;
    }

    /// <summary>
    /// Constructor producing a contour in the basis of an array of its vertices
    /// enumerated in the proper order. The order is declared by user.
    /// By default, the correctness is checked: computation of all intersections
    /// of the segments checks that the contour bounds a simple polygon;
    /// computation of the signed square of the simple polygon checks 
    /// the correctness of the orientation
    /// </summary>
    /// <param name="parent">Parent polygon</param>
    /// <param name="ps">The set of vertices</param>
    /// <param name="orient">Declared orientation of the contour</param>
    /// <param name="checkSimplicity">Flag showing whether the simplicity of the corresponding polygon 
    /// should be checked</param>
    /// <param name="checkOrientation">Flag showing that the contour orientation should be checked</param>
    public Contour(P parent, Point2D[] ps, PolylineOrientation orient,
      bool checkSimplicity = true, bool checkOrientation = true) :
      this(parent, new List<Point2D>(ps), orient, checkSimplicity, checkOrientation) { }
    #endregion

    /// <summary>
    /// Parent polygon property
    /// </summary>
    public P Polygon { get; protected set; }
  }
}
