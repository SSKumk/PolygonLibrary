using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using PolygonLibrary.Basics;
using PolygonLibrary.Toolkit;
using PolygonLibrary.Segments;

namespace PolygonLibrary.Polygons.ConvexPolygons;

/// <summary>
/// Class of a convex polygon
/// </summary>
public partial class ConvexPolygon : BasicPolygon {
  #region Additional data structures and properties

  /// <summary>
  /// The storage for the dual description of the polygon
  /// </summary>
  private SupportFunction _sf;

  /// <summary>
  /// Property for getting the dual description of the polygon
  /// </summary>
  public SupportFunction SF {
    get {
      if (_sf == null) {
        ComputeSF();
      }

      return _sf;
    }
    protected set => _sf = value;
  }

  /// <summary>
  /// Property to get the only contour of the convex polygon
  /// </summary>
  public Polyline Contour {
    get {
      if (_contours == null) {
        ComputeContours();
      }

      return _contours[0];
    }
  }

  /// <summary>
  /// Storage for the square of the polygon
  /// </summary>
  protected double? _square;

  /// <summary>
  /// Property of the square of the polygon.
  /// Computes the square if necessary
  /// </summary>
  public double Square {
    get {
      if (!_square.HasValue) {
        GenerateTriangleWeights();
      }

      return _square.Value;
    }
  }

  #endregion

  #region Constructors

  /// <summary>
  /// Constructor on the basis of list of vertices represented as two-dimensional points
  /// </summary>
  public ConvexPolygon(IEnumerable<Point2D> vs, bool ToConvexify = false)
    : base(ToConvexify ? Convexification.ArcHull2D(vs.ToList()) : vs.ToList()) =>
    _sf = null;

  /// <summary>
  /// Constructor on the basis of list of vertices represented as multidimensional points
  /// </summary>
  public ConvexPolygon(IEnumerable<Point> vs, bool ToConvexify = false)
    : base(ToConvexify
      ? Convexification.ArcHull2D(vs.Select(p => (Point2D)p).ToList())
      : vs.Select(p => (Point2D)p).ToList()) =>
    _sf = null;

  /// <summary>
  /// Constructor on the basis of the support function. 
  /// All other data will be computed in the lazy regime
  /// </summary>
  /// <param name="sf">The support function</param>
  public ConvexPolygon(SupportFunction sf) => _sf = sf;

  #endregion

  #region Internal methods

  /// <summary>
  /// On demand computation of the dual description of the polygon on the basis
  /// of the array of vertices (which, in its turn, can be computed on the basis 
  /// of contour of the polygon)
  /// </summary>
  private void ComputeSF() {
    if (_contours != null) {
      _sf = new SupportFunction(_contours[0].Vertices, false);
    }
    else if (_vertices != null) {
      ComputeContours();
      _sf = new SupportFunction(_contours[0].Vertices, false);
    }
    else {
      throw new Exception(
        "Cannot construct dual description of a convex polygon: neither vertices, nor contours are initialized");
    }
  }

  /// <summary>
  /// Compute the contour on the basis of either list of vertices, or the dual description
  /// </summary>
  protected override void ComputeContours() {
    if (_vertices != null) {
      base.ComputeContours();
    }
    else if (_sf != null) {
      int i, j;
      List<Point2D> ps = new List<Point2D>();
      for (i = 0, j = 1; i < _sf.Count; i++, j = (j + 1) % _sf.Count) {
        AddPoint(ps, GammaPair.CrossPairs(_sf[i], _sf[j]));
      }

      if (ps.Count > 1 && ps.First().CompareTo(ps.Last()) == 0) {
        ps.RemoveAt(ps.Count - 1);
      }

      _contours = new List<Polyline>();
      _contours.Add(new Polyline(ps, PolylineOrientation.Counterclockwise, false, false));
    }
  }

  protected override void ComputeVertices() {
    Vertices = Contour.Vertices;
  }

  #endregion

  #region Convex polygon utilities

  /// <summary>
  /// Method checking whether this polygon contains a given point
  /// </summary>
  /// <param name="p">The point to be checked</param>
  /// <returns>true, if the point is inside the polygon; false, otherwise</returns>
  public override bool Contains(Point2D p) {
    // Special case: the point coincides with the initial vertex of the polygon
    if (p.Equals(Contour[0])) {
      return true;
    }

    Vector2D vp = p - Contour[0];

    // If the point is outside the cone (v0,p1);(v0,vn), then it is outside the polygon
    if (vp.IsBetween(Contour[^1] - Contour[0], Contour[1] - Contour[0])) {
      return false;
    }

    int ind = Contour.Vertices.BinarySearchByPredicate(vert => Tools.GE(vp ^ (vert - Contour[0])), 1
      , Contour.Count - 1);

    // The point is on the ray starting at v0 and passing through p1.
    // Check distance
    if (ind == 1) {
      return Tools.LE(vp.Length, (Contour[1] - Contour[0]).Length);
    }

    // The point is somewhere inside the polygon cone.
    // The final decision is made on the basis of support function calculation
    else {
      Vector2D norm = (Contour[ind] - Contour[ind - 1]).TurnCW();

      return Tools.LE(norm * (Vector2D)p, norm * (Vector2D)Contour[ind - 1]);
    }
  }

  /// <summary>
  /// Get elements of the polygon extremal on the given vector. 
  /// If the extremal elements consist of one point only, this point is returned in p1 
  /// and p2 is null. If the extremal elements give a segment, the endpoint of this segment
  /// are returned in p1 and p2 (in some order)
  /// </summary>
  /// <param name="direction">A vector defining the direction</param>
  /// <param name="p1">One extremal point</param>
  /// <param name="p2">Another extremal point</param>
  public void GetExtremeElements(Vector2D direction, out Point2D p1, out Point2D p2) {
    int i, j;
    SF.FindCone(direction, out i, out j);

    p1 = GammaPair.CrossPairs(SF[i], SF[j]);
    // A vector co-directed with the given one is found
    p2 = Tools.EQ(direction.PolarAngle, SF[i].Normal.PolarAngle)
      ? GammaPair.CrossPairs(SF[i], SF.GetAtCyclic(j + 1))
      : null;
  }

  /// <summary>
  /// Method auxiliary for the methods of computation the square of the polygon
  /// and generating a random point in the polygon
  /// </summary>
  private void GenerateTriangleWeights() {
    int i;
    triangleWeights = new List<double>(Contour.Count - 1);
    for (i = Contour.Count - 1; i > 0; i--) {
      triangleWeights.Add(0);
    }

    for (i = 1; i < Contour.Count - 1; i++) {
      triangleWeights[i] = triangleWeights[i - 1] +
                           (0.5 * (Contour[i] - Contour[0]) ^ (Contour[i + 1] - Contour[0]));
    }

    _square = triangleWeights[^1];
  }

  /// <summary>
  /// Generates data of a point uniformly distributed in the polygon:
  ///  - choose a triangle of type (v_0,v_i,v_{i+1}) according its square;
  ///  - choose a point in the triangle according to the algorithm from
  ///    https://math.stackexchange.com/questions/18686/uniform-random-point-in-triangle
  ///    P = (1-sqrt(r1))v_0 + (sqrt(r1)(1-r2))B + (r2*sqrt(r1))C, r1,r2 \in U[0,1]
  /// </summary>
  /// <param name="trInd">Index of the second vertex of the chosen triangle</param>
  /// <param name="a">The weight of the vertex v_0</param>
  /// <param name="b">The weight of the vertex v_i</param>
  /// <param name="c">The weight of the vertex v_{i+1}</param>
  /// <param name="rnd">Random generator to be used; if null, the internal generator of the polygon is used</param>
  public void GenerateDataForRandomPoint(
    out int trInd, out double a, out double b, out double c, Random rnd = null) {
    // Generate the list triangle weights, if necessary
    if (triangleWeights == null) {
      GenerateTriangleWeights();
    }

    rnd ??= MyRnd;

    double s = rnd.NextDouble() * _square.Value;
    trInd = triangleWeights.BinarySearch(s, new Tools.DoubleComparer(Tools.Eps));
    if (trInd < 0) {
      trInd = ~trInd;
    }

    double r1 = Math.Sqrt(rnd.NextDouble()), r2 = rnd.NextDouble();
    a = 1 - r1;
    b = r1 * (1 - r2);
    c = r2 * r1;
  }

  /// <summary>
  /// Auxiliary structure for the method of generating a random point in the polygon;
  /// it contains progressive sums of squares of triangle of type (v_0,v_i,v_{i+1}).
  /// It initializes at the first call to generation of a point and is used in further calls
  /// </summary>
  protected List<double> triangleWeights;

  /// <summary>
  /// Internal random generator
  /// </summary>
  protected Random _myRnd;

  /// <summary>
  /// Internal property for taking the internal random generator initializing it if necessary
  /// </summary>
  protected Random MyRnd => _myRnd ??= new Random();

  /// <summary>
  /// Generates data of a point uniformly distributed in the polygon.
  /// Calls to <see cref="GenerateDataForRandomPoint"/> to generate data of the point,
  /// computes the point and returns it
  /// </summary>
  /// <param name="rnd">The random generator to be used; if null, the internal generator of the polygon is used</param>
  /// <returns>The generated point</returns>
  public Point2D GenerateRandomPoint(Random rnd = null) {
    int i;
    double a, b, c;
    GenerateDataForRandomPoint(out i, out a, out b, out c, rnd);

    return Point2D.LinearCombination(Vertices[0], a, Vertices[i], b, Vertices[i + 1], c);
  }

  /// <summary>
  /// Method computing signs of dot products of vectors from the given vertex
  /// to the neighbor ones and from the vertex to the given point
  /// </summary>
  /// <param name="x">The given point</param>
  /// <param name="curPointInd">The index of the vertex</param>
  /// <param name="sl">Sign of the dot product of vectors from the vertex to the previous vertex and to the given point</param>
  /// <param name="sr">Sign of the dot product of vectors from the vertex to the next vertex and to the given point</param>
  /// <returns>true, if the point is the nearest, that is, if both <see cref="sl"/> and <see cref="sr"/> are non-positive</returns>
  protected bool ComputePointSigns(Point2D x, int curPointInd, out int sl, out int sr) {
    Point2D
      prevVert = Contour.Vertices.GetAtCyclic(curPointInd - 1),
      curVert = Contour.Vertices.GetAtCyclic(curPointInd),
      nextVert = Contour.Vertices.GetAtCyclic(curPointInd + 1);
    Vector2D toPoint = x - curVert;
    sl = Tools.CMP((prevVert - curVert) * toPoint);
    sr = Tools.CMP((nextVert - curVert) * toPoint);

    return sl <= 0 && sr <= 0;
  }

  /// <summary>
  /// Method returning the point of the polygon nearest to the given point
  /// </summary>
  /// <param name="p">The given point</param>
  /// <exception cref="NotImplementedException">Now it is not implemented</exception>
  /// <returns>The nearest point of the polygon</returns>
  public Point2D NearestPoint(Point2D p) => throw new NotImplementedException();

  private bool IsPointContainsInHalfPlane(Segment s) {
    throw new NotImplementedException();
  }

  #endregion

  #region Operators

  /// <summary>
  /// Operator of algebraic (Minkowski) sum of two convex polygons
  /// </summary>
  /// <param name="cp1">The first polygon summand</param>
  /// <param name="cp2">The second polygon summand</param>
  /// <returns>The polygon sum</returns>
  public static ConvexPolygon operator +(ConvexPolygon cp1, ConvexPolygon cp2) {
    SupportFunction sf = SupportFunction.CombineFunctions(cp1.SF, cp2.SF, 1, 1);

    return new ConvexPolygon(sf);
  }

  /// <summary>
  /// Operator of geometric (Minkowski) difference of two convex polygons
  /// </summary>
  /// <param name="cp1">The polygon minuend</param>
  /// <param name="cp2">The polygon subtrahend</param>
  /// <returns>The polygon difference; if the difference is empty, null is returned</returns>
  public static ConvexPolygon operator -(ConvexPolygon cp1, ConvexPolygon cp2) {
    List<int> suspiciousIndices = new List<int>();
    SupportFunction sf =
      SupportFunction.CombineFunctions(cp1.SF, cp2.SF, 1, -1, suspiciousIndices)
        .ConvexifyFunctionWithInfo(suspiciousIndices);
    if (sf == null) {
      return null;
    }
    else {
      return new ConvexPolygon(sf);
    }
  }

  #endregion
}