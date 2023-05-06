using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using PolygonLibrary.Basics;
using PolygonLibrary.Toolkit;

namespace PolygonLibrary.Polygons.ConvexPolygons;

/// <summary>
/// Class of a convex polygon.
/// It is guaranteed that the 0th vertex is the vertex minimal lexicographically.
/// Also, is is guaranteed, that this vertex is obtained by the first and the last pairs
/// in the support function collection
/// </summary>
public partial class ConvexPolygon : BasicPolygon {
  #region Additional data structures and properties

  /// <summary>
  /// The storage for the dual description of the polygon
  /// </summary>
  private SupportFunction? _sf;

  /// <summary>
  /// Property for getting the dual description of the polygon
  /// </summary>
  public SupportFunction SF {
    get {
      Debug.Assert(!IsEmpty, "Try to get a support function of an empty polygon!");

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
      Debug.Assert(!IsEmpty, "Try to get a contour of an empty polygon!");

      if (_contours == null) {
        ComputeContours();
      }

      return _contours![0];
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
      if (IsEmpty) {
        return 0;
      }

      if (!_square.HasValue) {
        GenerateTriangleWeights();
      }

      Debug.Assert(_square != null, nameof(_square) + " != null");
      return _square.Value;
    }
  }

  #endregion

  #region Constructors

  /// <summary>
  /// A constructor for an empty polygon
  /// </summary>
  public ConvexPolygon() {
    IsEmpty = true;
    _sf = null;
  }

  /// <summary>
  /// Constructor on the basis of list of vertices represented as two-dimensional points
  /// </summary>
  private ConvexPolygon(IEnumerable<Point2D> ps)
    : base(ps) {
    _sf = null;
    IsEmpty = false;
  }

  /// <summary>
  /// Constructor on the basis of the support function. 
  /// All other data will be computed in the lazy regime
  /// </summary>
  /// <param name="sf">The support function</param>
  public ConvexPolygon(SupportFunction sf) {
    _sf = sf;
    IsEmpty = false;
  }

  /// <summary>
  /// Create a convex polygon from a contour given as a list of two-dimensional points
  /// </summary>
  /// <param name="ps">The contour</param>
  /// <param name="checkConvexity">Flag showing whether to check convexity of the contour</param>
  /// <returns>The resultant polygon</returns>
  public static ConvexPolygon CreateConvexPolygonFromContour(IEnumerable<Point2D> ps, bool checkConvexity = false) {
    // todo: check convexity of the line
    return new ConvexPolygon(ps);
  }

  /// <summary>
  /// Create a convex polygon from a contour given as a list of two-dimensional points presented as multi-dimensional points.
  /// The points are converted to two-dimensional and the corresponding factory is called.
  /// </summary>
  /// <param name="ps">The swarm of points</param>
  /// <param name="checkConvexity">Flag showing whether to check convexity of the contour</param>
  /// <returns>The resultant polygon</returns>
  public static ConvexPolygon CreateConvexPolygonFromContour(IEnumerable<Point> ps, bool checkConvexity = false) {
    return CreateConvexPolygonFromContour(ps.Select((p) => (Point2D)p), checkConvexity);
  }


  /// <summary>
  /// Create a convex polygon from a swarm of two-dimensional points.
  /// The swarm is convexified.
  /// </summary>
  /// <param name="ps">The swarm of points</param>
  /// <returns>The resultant polygon</returns>
  public static ConvexPolygon CreateConvexPolygonFromSwarm(IEnumerable<Point2D> ps) {
    return new ConvexPolygon(Convexification.ArcHull2D(ps));
  }

  /// <summary>
  /// Create a convex polygon from a swarm of two-dimensional points presented as multi-dimensional points.
  /// The points are converted to two-dimensional and the corresponding factory is called.
  /// The swarm is convexified.
  /// </summary>
  /// <param name="ps">The swarm of points</param>
  /// <returns>The resultant polygon</returns>
  public static ConvexPolygon CreateConvexPolygonFromSwarm(IEnumerable<Point> ps) {
    return CreateConvexPolygonFromSwarm(ps.Select((p) => (Point2D)p));
  }

  /// <summary>
  /// A constant empty polygon
  /// </summary>
  public static readonly ConvexPolygon EmptyPolygon = new ConvexPolygon();

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
    } else if (_vertices != null) {
      ComputeContours();
      Debug.Assert(_contours != null, nameof(_contours) + " != null");
      _sf = new SupportFunction(_contours[0].Vertices, false);
    } else {
      throw new Exception(
        "Cannot construct dual description of a convex polygon: neither vertices, nor contours are initialized");
    }
  }

  /// <summary>
  /// Compute the contour on the basis of either list of vertices, or the dual description
  /// </summary>
  protected override void ComputeContours() {
    Debug.Assert(_sf != null, "Try to compute contour of a convex polygon without given support function");

    int i
    , j;
    List<Point2D> ps = new List<Point2D>();
    for (i = 0, j = _sf.Count - 1; i < _sf.Count; i++, j = (j + 1) % _sf.Count) {
      Point2D temp = GammaPair.CrossPairs(_sf[i], _sf[j]);
      if (ps.Count == 0 || temp != ps[^1]) {
        ps.Add(temp);
      }
    }

    if (ps.Count > 1 && ps.First().CompareTo(ps.Last()) == 0) {
      ps.RemoveAt(ps.Count - 1);
    }

    _contours = new List<Polyline>();
    _contours.Add(new Polyline(ps, PolylineOrientation.Counterclockwise, false, false));
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
    if (IsEmpty) {
      return false;
    }

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
  /// Method checking whether this polygon contains a given point in the interior of the polygon
  /// </summary>
  /// <param name="p">The point to be checked</param>
  /// <returns>true, if the point is strictly inside the polygon; false, otherwise</returns>
  public override bool ContainsInside(Point2D p) {
    if (IsEmpty) {
      return false;
    }

    // Special case: the point coincides with the initial vertex of the polygon
    if (p.Equals(Contour[0])) {
      return false;
    }

    Vector2D vp = p - Contour[0];

    // If the point is outside the cone (v0,p1);(v0,vn), then it is outside the polygon
    if (!vp.IsBetween(Contour[1] - Contour[0], Contour[^1] - Contour[0])) {
      return false;
    }

    int ind = Contour.Vertices.BinarySearchByPredicate(vert => Tools.GE(vp ^ (vert - Contour[0])), 1
    , Contour.Count - 1);

    // The point is on the ray starting at v0 and passing through p1.
    // Check distance
    if (ind == 1) {
      return false;
    }

    // The point is somewhere inside the polygon cone.
    // The final decision is made on the basis of support function calculation
    Vector2D norm = (Contour[ind] - Contour[ind - 1]).TurnCW();
    return Tools.LT(norm * (Vector2D)p, norm * (Vector2D)Contour[ind - 1]);
  }

  /// <summary>
  /// Get index or indices of the polygon vertices extremal on the given vector. 
  /// If the extremal elements consist of one vertex only, the index of this point is returned in <paramref name="ind1"/> 
  /// and <paramref name="ind2"/> is -1. If the extremal elements give a segment, the indices of the endpoints of this segment
  /// are returned in <paramref name="ind1"/> and <paramref name="ind2"/>
  /// </summary>
  /// <param name="direction">A vector defining the direction</param>
  /// <param name="ind1">The index of one extremal vertex</param>
  /// <param name="ind2">The index of another extremal vertex, or -1 if the extremal point is unique</param>
  /// <remarks>If the extremal vertex is non-unique, then <paramref name="ind1"/> precedes <paramref name="ind2"/></remarks>
  public void GetExtremeVerticesIndices(Vector2D direction, out int ind1, out int ind2) {
    Debug.Assert(!IsEmpty, "Taking an extreme element of an empty polygon");
    Debug.Assert(SF != null, nameof(SF) + " != null");

    int i
    , j;
    SF.FindCone(direction, out i, out j);

    if (Tools.EQ(direction.PolarAngle, SF[i].Normal.PolarAngle)) {
      ind1 = i;
      ind2 = j;
    } else {
      ind1 = j;
      ind2 = -1;
    }
  }

  /// <summary>
  /// Get elements of the polygon extremal on the given vector. 
  /// If the extremal elements consist of one point only, this point is returned in <paramref name="p1"/>
  /// and <paramref name="p2"/> is null. If the extremal elements give a segment, the endpoint of this segment
  /// are returned in <paramref name="p1"/> and <paramref name="p2"/>, and <paramref name="p1"/> precedes <paramref name="p2"/>
  /// </summary>
  /// <param name="direction">A vector defining the direction</param>
  /// <param name="p1">One extremal point</param>
  /// <param name="p2">Another extremal point</param>
  /// <remarks>If the extremal vertex is non-unique, then <para>p1</para> precedes <para>p2</para></remarks>
  public void GetExtremeElements(Vector2D direction, out Point2D p1, out Point2D? p2) {
    Debug.Assert(!IsEmpty, "Taking an extreme element of an empty polygon");
    Debug.Assert(SF != null, nameof(SF) + " != null");


    int i
    , j;
    GetExtremeVerticesIndices(direction, out i, out j);

    p1 = Vertices[i];
    p2 = j == -1 ? null : Vertices[j];
  }

  /// <summary>
  /// Method auxiliary for the methods of computation the square of the polygon
  /// and generating a random point in the polygon
  /// </summary>
  private void GenerateTriangleWeights() {
    Debug.Assert(!IsEmpty, "Constructing triangle weights of an empty polygon");

    int i;
    triangleWeights = new List<double>(Contour.Count - 1);
    for (i = Contour.Count - 1; i > 0; i--) {
      triangleWeights.Add(0);
    }

    for (i = 1; i < Contour.Count - 1; i++) {
      triangleWeights[i] = triangleWeights[i - 1] + (0.5 * (Contour[i] - Contour[0]) ^ (Contour[i + 1] - Contour[0]));
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
  public void GenerateDataForRandomPoint(out int trInd, out double a, out double b, out double c, Random? rnd = null) {
    Debug.Assert(!IsEmpty, "Generating data for random point tossing into an empty polygon");

    // Generate the list triangle weights, if necessary
    if (triangleWeights == null) {
      GenerateTriangleWeights();
    }

    rnd ??= MyRnd;

    Debug.Assert(_square != null, nameof(_square) + " != null");
    Debug.Assert(triangleWeights != null, nameof(triangleWeights) + " != null");
    double s = rnd.NextDouble() * _square.Value;
    trInd = triangleWeights.BinarySearch(s, new Tools.DoubleComparer(Tools.Eps));
    if (trInd < 0) {
      trInd = ~trInd;
    }

    double r1 = Math.Sqrt(rnd.NextDouble())
    , r2 = rnd.NextDouble();
    a = 1 - r1;
    b = r1 * (1 - r2);
    c = r2 * r1;
  }

  /// <summary>
  /// Auxiliary structure for the method of generating a random point in the polygon;
  /// it contains progressive sums of squares of triangle of type (v_0,v_i,v_{i+1}).
  /// It initializes at the first call to generation of a point and is used in further calls
  /// </summary>
  protected List<double>? triangleWeights;

  /// <summary>
  /// Internal random generator
  /// </summary>
  protected Random? _myRnd;

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
  public Point2D GenerateRandomPoint(Random? rnd = null) {
    Debug.Assert(!IsEmpty, "Getting a random point in an empty polygon");

    int i;
    double a
    , b
    , c;
    GenerateDataForRandomPoint(out i, out a, out b, out c, rnd);

    return Point2D.LinearCombination(Vertices[0], a, Vertices[i], b, Vertices[i + 1], c);
  }

  /// <summary>
  /// Method computing signs of dot products of vectors from the given vertex
  /// to the neighbor ones and from that vertex to the given point
  /// </summary>
  /// <param name="x">The given point</param>
  /// <param name="curPointInd">The index of the vertex</param>
  /// <param name="sl">Sign of the dot product of vectors from the vertex to the previous vertex and to the given point</param>
  /// <param name="sr">Sign of the dot product of vectors from the vertex to the next vertex and to the given point</param>
  /// <returns>true, if the point is the nearest, that is, if both <see cref="sl"/> and <see cref="sr"/> are non-positive</returns>
  protected bool ComputePointSigns(Point2D x, int curPointInd, out int sl, out int sr) {
    Debug.Assert(!IsEmpty, "A ComputePointSigns call for of an empty polygon");

    Point2D prevVert = Contour.Vertices.GetAtCyclic(curPointInd - 1)
    , curVert = Contour.Vertices.GetAtCyclic(curPointInd)
    , nextVert = Contour.Vertices.GetAtCyclic(curPointInd + 1);
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

  /// <summary>
  /// The function cuts a convex polygon along a line passing through the given two vertices
  /// </summary>
  /// <param name="k">First index of vertex</param>
  /// <param name="s">Second index of vertex</param>
  /// <returns>A tuple of two polygons</returns>
  /// <exception cref="ArgumentException">Thrown if k and s is adjacent!</exception>
  public (ConvexPolygon, ConvexPolygon) CutConvexPolygon(int k, int s) {
    Debug.Assert(!IsEmpty, "Cutting an empty polygon");

    if (k > s) {
      (k, s) = (s, k);
    }

    if (s - k < 2 || s - k == Vertices.Count - 1) { //if k == s or k-s is a edge
      throw new ArgumentException($"{k} and {s} is adjacent!");
    }

    var list1 = new List<Point2D>();
    for (int i = k; i < s + 1; i++) {
      list1.Add(Vertices[i]);
    }

    var list2 = new List<Point2D>();
    for (int i = s; i < Vertices.Count + k + 1; i++) {
      list2.Add(Vertices.GetAtCyclic(i));
    }

    return (new ConvexPolygon(list1), new ConvexPolygon(list2));
  }

  /// <summary>
  /// Procedure for cutting a polygon by a line. 
  /// </summary>
  /// <param name="l">The cutting line</param>
  /// <returns>A pair of polygons obtained by cutting the original one by the given line.
  /// With that, the first polygon in th pair is embedded to the positive half-plane of the line,
  /// and the second one is embedded to the negative semi-plane. 
  /// If the line does not cross the polygon, then the original polygon is returned in the appropriate element of the pair
  /// and an empty polygon will be the other element</returns>
  public (ConvexPolygon, ConvexPolygon) CutConvexPolygonByLine(Line2D l) {
    Debug.Assert(!IsEmpty, "Cutting an empty polygon by a line");

    Point2D p1p
    , p1m;
    Point2D? p2p
    , p2m;
    GetExtremeElements(l.Normal, out p1p, out p2p);
    GetExtremeElements(-l.Normal, out p1m, out p2m);

    if (Tools.LE(l[p1p])) {
      return (ConvexPolygon.EmptyPolygon, this);
    }

    if (Tools.GE(l[p1m])) {
      return (this, ConvexPolygon.EmptyPolygon);
    }

    // TODO: realize this procedure
    return (ConvexPolygon.EmptyPolygon, ConvexPolygon.EmptyPolygon);
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
    if (cp1.IsEmpty || cp2.IsEmpty) {
      return ConvexPolygon.EmptyPolygon;
    }

    Debug.Assert(cp1.SF != null, "cp1.SF != null");
    Debug.Assert(cp2.SF != null, "cp2.SF != null");
    SupportFunction sf = SupportFunction.CombineFunctions(cp1.SF, cp2.SF, 1, 1);

    return new ConvexPolygon(sf);
  }

  /// <summary>
  /// Operator of geometric (Minkowski) difference of two convex polygons
  /// </summary>
  /// <param name="cp1">The polygon minuend</param>
  /// <param name="cp2">The polygon subtrahend</param>
  /// <returns>The polygon difference; if the difference is empty, null is returned</returns>
  public static ConvexPolygon? operator -(ConvexPolygon cp1, ConvexPolygon cp2) {
    if (cp1.IsEmpty) {
      return ConvexPolygon.EmptyPolygon;
    }

    if (cp2.IsEmpty) {
      return cp1;
    }

    Debug.Assert(cp1.SF != null, "cp1.SF != null");
    Debug.Assert(cp2.SF != null, "cp2.SF != null");

    List<int> suspiciousIndices = new List<int>();
    SupportFunction? sf = SupportFunction.CombineFunctions(cp1.SF, cp2.SF, 1, -1, suspiciousIndices)
      .ConvexifyFunctionWithInfo(suspiciousIndices);
    if (sf == null) {
      return null;
    } else {
      return new ConvexPolygon(sf);
    }
  }

  #endregion
}