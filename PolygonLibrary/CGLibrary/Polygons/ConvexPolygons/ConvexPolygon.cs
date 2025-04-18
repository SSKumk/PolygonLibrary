﻿namespace CGLibrary;

public partial class Geometry<TNum, TConv> where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  /// <summary>
  /// Class of a convex polygon
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
    public SupportFunction? SF {
      get
        {
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
      get
        {
          if (_contours == null) {
            ComputeContours();
          }

          Debug.Assert(_contours != null, nameof(_contours) + " != null");

          return _contours[0];
        }
    }

    /// <summary>
    /// Storage for the square of the polygon
    /// </summary>
    protected TNum? _square;

    /// <summary>
    /// Property of the square of the polygon.
    /// Computes the square if necessary
    /// </summary>
    public TNum Square {
      get
        {
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
    /// Constructor on the basis of list of vertices represented as two-dimensional points
    /// </summary>
    public ConvexPolygon(IEnumerable<Vector2D> vs, bool ToConvexify = false) : base
      (ToConvexify ? Convexification.ArcHull2D(vs.ToList()) : vs.ToList()) => _sf = null;

    /// <summary>
    /// Constructor on the basis of list of vertices represented as multidimensional points
    /// </summary>
    public ConvexPolygon(IEnumerable<Vector> vs, bool ToConvexify = false) : base
      (ToConvexify ? Convexification.ArcHull2D(vs.Select(p => (Vector2D)p).ToList()) : vs.Select(p => (Vector2D)p).ToList()) =>
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
      } else if (_vertices != null) {
        ComputeContours();
        Debug.Assert(_contours != null, nameof(_contours) + " != null");
        _sf = new SupportFunction(_contours[0].Vertices, false);
      } else {
        throw new Exception
          ("Cannot construct dual description of a convex polygon: neither vertices, nor contours are initialized");
      }
    }

    /// <summary>
    /// Compute the contour on the basis of either list of vertices, or the dual description
    /// </summary>
    protected override void ComputeContours() {
      if (_vertices != null) {
        base.ComputeContours();
      } else if (_sf != null) {
        int           i, j;
        List<Vector2D> ps = new List<Vector2D>();
        for (i = 0, j = 1; i < _sf.Count; i++, j = (j + 1) % _sf.Count) {
          Vector2D temp = GammaPair.CrossPairs(_sf[i], _sf[j]);
          if (ps.Count == 0 || temp != ps[^1]) { ps.Add(temp); }
        }

        if (ps.Count > 1 && ps.First().CompareTo(ps.Last()) == 0) {
          ps.RemoveAt(ps.Count - 1);
        }

        _contours = new List<Polyline>();
        _contours.Add(new Polyline(ps, PolylineOrientation.Counterclockwise, false, false));
      }
    }

    protected override void ComputeVertices() { Vertices = Contour.Vertices; }
#endregion

#region Convex polygon utilities
    /// <summary>
    /// Method checking whether this polygon contains a given point
    /// </summary>
    /// <param name="p">The point to be checked</param>
    /// <returns>true, if the point is inside the polygon; false, otherwise</returns>
    public override bool Contains(Vector2D p) {
      // Special case: the point coincides with the initial vertex of the polygon
      if (p.Equals(Contour[0])) {
        return true;
      }

      Vector2D vp = p - Contour[0];

      // If the point is outside the cone (v0,p1);(v0,vn), then it is outside the polygon
      if (vp.IsBetween(Contour[^1] - Contour[0], Contour[1] - Contour[0])) {
        return false;
      }

      int ind = Contour.Vertices.BinarySearchByPredicate(vert => Tools.GE(vp ^ (vert - Contour[0])), 1, Contour.Count - 1);

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
    public override bool ContainsInside(Vector2D p) {
      // Special case: the point coincides with the initial vertex of the polygon
      if (p.Equals(Contour[0])) {
        return false;
      }

      Vector2D vp = p - Contour[0];

      // If the point is outside the cone (v0,p1);(v0,vn), then it is outside the polygon
      if (!vp.IsBetween(Contour[1] - Contour[0], Contour[^1] - Contour[0])) {
        return false;
      }

      int ind = Contour.Vertices.BinarySearchByPredicate(vert => Tools.GE(vp ^ (vert - Contour[0])), 1, Contour.Count - 1);

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
    /// Get elements of the polygon extremal on the given vector. 
    /// If the extremal elements consist of one point only, this point is returned in p1 
    /// and p2 is null. If the extremal elements give a segment, the endpoint of this segment
    /// are returned in p1 and p2 (in some order)
    /// </summary>
    /// <param name="direction">A vector defining the direction</param>
    /// <param name="p1">One extremal point</param>
    /// <param name="p2">Another extremal point</param>
    public void GetExtremeElements(Vector2D direction, out Vector2D p1, out Vector2D? p2) {
      int i, j;
      Debug.Assert(SF != null, nameof(SF) + " != null");
      SF.FindCone(direction, out i, out j);

      p1 = GammaPair.CrossPairs(SF[i], SF[j]);
      // A vector co-directed with the given one is found
      p2 = Tools.EQ(direction.PolarAngle, SF[i].Normal.PolarAngle) ? GammaPair.CrossPairs(SF[i], SF.GetAtCyclic(j + 1)) : null;
    }

    /// <summary>
    /// Method auxiliary for the methods of computation the square of the polygon
    /// and generating a random point in the polygon
    /// </summary>
    private void GenerateTriangleWeights() {
      int i;
      triangleWeights = new List<TNum>(Contour.Count - 1);
      for (i = Contour.Count - 1; i > 0; i--) {
        triangleWeights.Add(Tools.Zero);
      }

      for (i = 1; i < Contour.Count - 1; i++) {
        triangleWeights[i] = triangleWeights[i - 1] + (Tools.HalfOne * (Contour[i] - Contour[0]) ^ (Contour[i + 1] - Contour[0]));
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
    /// <param name="rnd">Random generator to be used; if null, some internal generator is used</param>
    public void GenerateDataForRandomPoint(out int trInd, out TNum a, out TNum b, out TNum c, GRandomLC? rnd = null) {
      // Generate the list triangle weights, if necessary
      if (triangleWeights == null) {
        GenerateTriangleWeights();
      }

      rnd ??= MyRnd;

      Debug.Assert(_square is not null, "ConvexPolygon.GenerateDataForRandomPoint: Square of given polygon is null!");
      Debug.Assert
        (triangleWeights is not null, "ConvexPolygon.GenerateDataForRandomPoint: triangleWeights of given polygon is null!");

      TNum s = rnd.NextPrecise() * _square.Value;
      trInd = triangleWeights.BinarySearch(s, Tools.TComp);
      if (trInd < 0) {
        trInd = ~trInd;
      }

      TNum r1 = TNum.Sqrt(rnd.NextPrecise()), r2 = rnd.NextPrecise();
      a = TNum.One - r1;
      b = r1 * (TNum.One - r2);
      c = r2 * r1;
    }

    /// <summary>
    /// Auxiliary structure for the method of generating a random point in the polygon;
    /// it contains progressive sums of squares of triangle of type (v_0,v_i,v_{i+1}).
    /// It initializes at the first call to generation of a point and is used in further calls
    /// </summary>
    protected List<TNum>? triangleWeights;

    /// <summary>
    /// Internal random generator
    /// </summary>
    protected GRandomLC? _myRnd;

    /// <summary>
    /// Internal property for taking the internal random generator initializing it if necessary
    /// </summary>
    protected GRandomLC MyRnd => _myRnd ??= new GRandomLC();

    /// <summary>
    /// Generates data of a point uniformly distributed in the polygon.
    /// Calls to <see cref="GenerateDataForRandomPoint"/> to generate data of the point,
    /// computes the point and returns it
    /// </summary>
    /// <param name="rnd">The random generator to be used; if null, the internal generator of the polygon is used</param>
    /// <returns>The generated point</returns>
    public Vector2D GenerateRandomPoint(GRandomLC? rnd = null) {
      int  i;
      TNum a, b, c;
      GenerateDataForRandomPoint(out i, out a, out b, out c, rnd);

      return Vector2D.LinearCombination(Vertices[0], a, Vertices[i], b, Vertices[i + 1], c);
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
    protected bool ComputePointSigns(Vector2D x, int curPointInd, out int sl, out int sr) {
      Vector2D prevVert = Contour.Vertices.GetAtCyclic(curPointInd - 1)
            , curVert  = Contour.Vertices.GetAtCyclic(curPointInd)
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
    public Vector2D NearestPoint(Vector2D p) => throw new NotImplementedException();

    private bool IsPointContainsInHalfPlane(Segment s) { throw new NotImplementedException(); }

    /// <summary>
    /// The function cuts a convex polygon along a line passing through the given two vertices
    /// </summary>
    /// <param name="k">First index of vertex</param>
    /// <param name="s">Second index of vertex</param>
    /// <returns>A tuple of two polygons</returns>
    /// <exception cref="ArgumentException">Thrown if k and s is adjacent!</exception>
    public (ConvexPolygon, ConvexPolygon) CutConvexPolygon(int k, int s) {
      if (k > s) {
        (k, s) = (s, k);
      }
      if (s - k < 2 || s - k == Vertices.Count - 1) { //if k == s or k-s is a edge
        throw new ArgumentException($"{k} and {s} is adjacent!");
      }
      List<Vector2D>? list1 = new List<Vector2D>();
      for (int i = k; i < s + 1; i++) {
        list1.Add(Vertices[i]);
      }
      List<Vector2D>? list2 = new List<Vector2D>();
      for (int i = s; i < Vertices.Count + k + 1; i++) {
        list2.Add(Vertices.GetAtCyclic(i));
      }

      return (new ConvexPolygon(list1), new ConvexPolygon(list2));
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
      Debug.Assert(cp1.SF != null, "cp1.SF != null");
      Debug.Assert(cp2.SF != null, "cp2.SF != null");
      SupportFunction sf = SupportFunction.CombineFunctions(cp1.SF, cp2.SF, Tools.One, Tools.One);

      return new ConvexPolygon(sf);
    }

    /// <summary>
    /// Operator of geometric (Minkowski) difference of two convex polygons
    /// </summary>
    /// <param name="cp1">The polygon minuend</param>
    /// <param name="cp2">The polygon subtrahend</param>
    /// <returns>The polygon difference; if the difference is empty, null is returned</returns>
    public static ConvexPolygon? operator -(ConvexPolygon cp1, ConvexPolygon cp2) {
      List<int> suspiciousIndices = new List<int>();
      Debug.Assert(cp1.SF != null, "cp1.SF != null");
      Debug.Assert(cp2.SF != null, "cp2.SF != null");
      SupportFunction? sf = SupportFunction.CombineFunctions(cp1.SF, cp2.SF, Tools.One, Tools.MinusOne, suspiciousIndices)
                                           .ConvexifyFunctionWithInfo(suspiciousIndices);
      if (sf == null) {
        return null;
      } else {
        return new ConvexPolygon(sf);
      }
    }
#endregion

  }

}
