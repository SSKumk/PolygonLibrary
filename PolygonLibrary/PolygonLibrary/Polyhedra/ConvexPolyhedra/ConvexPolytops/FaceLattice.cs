namespace CGLibrary;

public partial class Geometry<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  /// <summary>
  /// Representation of a Convex Polytop as a Face Lattice:
  /// A face lattice is a lattice where the maximum is the polytope itself.
  /// The nodes of the lattice correspond to the faces of the polytope in their respective dimensions.
  /// </summary>
  public class FaceLattice {

#region Fields and properties
    /// <summary>
    /// The maximum element in the lattice.
    /// </summary>
    public FLNode Top { get; init; }

    /// <summary>
    /// The lattice is represented level by level.
    /// At each level, there is a set that contains all nodes (faces of polytop) of that level's dimension.
    /// </summary>
    public List<SortedSet<FLNode>> Lattice { get; }

    /// <summary>
    /// Indexer access
    /// </summary>
    /// <param name="i">The index of the level of the lattice.</param>
    /// <returns>The set of corresponding nodes.</returns>
    public SortedSet<FLNode> this[Index i] => Lattice[i];

    /// <summary>
    /// Set of vertices that form convex polytop.
    /// </summary>
    public SortedSet<Vector> Vertices => Top.Vertices;

    /// <summary>
    /// Gets the total number of all k-faces in the lattice, except 0-faces.
    /// </summary>
    /// <value>A number of nonzero k-faces in the lattice.</value>
    public int NumberOfNonZeroKFaces => NumberOfKFaces - Lattice[0].Count;

    /// <summary>
    /// Gets the total number of all k-faces in the lattice.
    /// </summary>
    /// <value>A number of all k-faces in the lattice.</value>
    public int NumberOfKFaces => Lattice.Sum(lvl => lvl.Count);
#endregion

#region Constructors
    /// <summary>
    /// The vertex forms a one-element lattice.
    /// </summary>
    /// <param name="point">The point at which a face lattice is formed.</param>
    public FaceLattice(Vector point) {
      Top     = new FLNode(point);
      Lattice = new List<SortedSet<FLNode>>() { new SortedSet<FLNode>() { Top } };
    }

    /// <summary>
    /// Construct a face lattice based on given lattice.
    /// </summary>
    /// <param name="lattice">The lattice.</param>
    /// <param name="updateVertices">todo xml </param>
    public FaceLattice(List<SortedSet<FLNode>> lattice, bool updateVertices) {
      Lattice = lattice;

      Debug.Assert(lattice[^1].Count == 1, $"FaceLattice.Ctor: There should be only one d-face: the polytop itself. Found {lattice[^1].Count}");

      Top     = Lattice[^1].First();

      if (updateVertices) {
        Top.ReCalcAddInfo();
      }
    }

    // /// <summary>
    // /// Construct a face lattice based on a maximum element.
    // /// </summary>
    // /// <param name="Maximum">The maximum node.</param>
    // public FaceLattice(FLNode Maximum) {
    //   Top     = Maximum;
    //   Lattice = Maximum.GetAllLevels();
    // }

    internal static FaceLattice ConstructFromFLNodeSum(List<SortedSet<FLNodeSum>> FLS) {
      List<SortedSet<FLNode>> newFL = new List<SortedSet<FLNode>>();
      for (int i = 0; i <= FLS[^1].First().PolytopDim; i++) {
        newFL.Add(new SortedSet<FLNode>());
      }

      SortedDictionary<FLNodeSum, FLNode> oldToNew = new SortedDictionary<FLNodeSum, FLNode>();

      //Отдельно обрабатываем случай d == 0
      foreach (FLNodeSum vertex in FLS[0]) {
        FLNode newVertex = new FLNode(vertex.InnerPoint);
        oldToNew.Add(vertex, newVertex);
        newFL[0].Add(newVertex);
      }

      for (int i = 1; i < newFL.Count; i++) {
        foreach (FLNodeSum node in FLS[i]) {
          List<FLNode> newSub = node.Sub.Select(n => oldToNew[n]).ToList();

          FLNode newNode = new FLNode(newSub);
          oldToNew.Add(node, newNode);
          newFL[i].Add(newNode);
        }
      }

      return new FaceLattice(newFL, false);
    }

    internal static FaceLattice ConstructFromBaseSubCP(List<SortedSet<BaseSubCP>> FLS) {
      List<SortedSet<FLNode>> newFL = new List<SortedSet<FLNode>>();
      for (int i = 0; i <= FLS[^1].First().PolytopDim; i++) {
        newFL.Add(new SortedSet<FLNode>());
      }

      SortedDictionary<BaseSubCP, FLNode> oldToNew = new SortedDictionary<BaseSubCP, FLNode>();

      //Отдельно обрабатываем случай d == 0
      foreach (BaseSubCP vertex in FLS[0]) {
        FLNode newVertex = new FLNode(vertex.OriginalVertices.First());
        oldToNew.Add(vertex, newVertex);
        newFL[0].Add(newVertex);
      }

      for (int i = 1; i < newFL.Count; i++) {
        foreach (BaseSubCP node in FLS[i]) {
          List<FLNode> newSub = node.Faces!.Select(n => oldToNew[n]).ToList();

          FLNode newNode = new FLNode(newSub);
          oldToNew.Add(node, newNode);
          newFL[i].Add(newNode);
        }
      }

      return new FaceLattice(newFL, false);
    }
#endregion

#region Functions
    /// <summary>
    /// Transforms the lattice by applying a given function to each vertex of the lattice.
    /// </summary>
    /// <param name="transformFunc">The function to be applied to each vertex. This function takes a vertex of the lattice and returns a new point.</param>
    /// <returns>A new FaceLattice where each vertex has been transformed by the given function.</returns>
    public FaceLattice VertexTransform(Func<Vector, Vector> transformFunc) {
      List<SortedSet<FLNode>> newFL = new List<SortedSet<FLNode>>();
      for (int i = 0; i <= Top.PolytopDim; i++) {
        newFL.Add(new SortedSet<FLNode>());
      }

      SortedDictionary<FLNode, FLNode> oldToNew = new SortedDictionary<FLNode, FLNode>();

      //Отдельно обрабатываем случай d == 0
      foreach (FLNode vertex in Lattice[0]) {
        FLNode newVertex = new FLNode(transformFunc(vertex.Vertices.First()));
        oldToNew.Add(vertex, newVertex);
        newFL[0].Add(newVertex);
      }

      for (int i = 1; i < newFL.Count; i++) {
        foreach (FLNode node in Lattice[i]) {
          FLNode newNode = new FLNode(node.Sub.Select(n => oldToNew[n]));
          oldToNew.Add(node, newNode);
          newFL[i].Add(newNode);
        }
      }

      return new FaceLattice(newFL, false);
    }
#endregion

#region Overrides
    public override int GetHashCode()
      => throw new InvalidOperationException(); //HashCode.Combine(Vertices.Count, NumberOfNonZeroKFaces);

    public override bool Equals(object? obj) {
      if (obj == null || GetType() != obj.GetType()) {
        return false;
      }

      FaceLattice other = (FaceLattice)obj;

      if (this.Lattice.Count != other.Lattice.Count) {
        return false;
      }

      this.Top.ReCalcAddInfo();
      other.Top.ReCalcAddInfo();

      bool isEqual = true;

      for (int i = 0; i < Top.PolytopDim; i++) {
        if (Lattice[i].Count != other.Lattice[i].Count) {
          return false;
        }

        List<FLNode> mine   = this.Lattice[i].ToList();
        List<FLNode> theirs = other.Lattice[i].ToList();

        mine.Sort();
        theirs.Sort();

        for (int j = 0; j < Lattice[i].Count; j++) {
          isEqual = isEqual && mine[j].InnerPoint.Equals(theirs[j].InnerPoint);
        }

        if (!isEqual) {
          return false;
        }
      }

      return true;

      // for (int i = this.Top.PolytopDim; isEqual && i > -1; i--) {
      //   foreach (var thisNode in this.Lattice[i]) {
      //     if (!other.Lattice[i].TryGetValue(thisNode, out FLNode? otherNode)) {
      //       isEqual = false;
      //     }
      //     else {
      //       isEqual = isEqual && thisNode.Sub.SetEquals(otherNode.Sub);
      //       isEqual = isEqual && thisNode.Super.SetEquals(otherNode.Super);
      //     }
      //   }
      //   if (isEqual) {
      //     foreach (var otherNode in other.Lattice[i]) {
      //       if (!this.Lattice[i].TryGetValue(otherNode, out FLNode? thisNode)) {
      //         isEqual = false;
      //       }
      //       else {
      //         isEqual = isEqual && otherNode.Sub.SetEquals(thisNode.Sub);
      //         isEqual = isEqual && otherNode.Super.SetEquals(thisNode.Super);
      //       }
      //     }
      //   }
      // }

      return isEqual;
    }
#endregion

  }

  /// <summary>
  /// The node of the face lattice. It stores references to the supernodes and sub-nodes.
  /// In addition, it maintains a set of vertices representing the face.
  /// </summary>
  public class FLNode : IComparable<FLNode> {

#region Data and properties
    /// <summary>
    /// Gets the d-dimensional point 'p' which lies within P and does not lie on any faces of P.
    /// </summary>
    public Vector InnerPoint { get; private set; }

    /// <summary>
    /// Gets the list of d-dimensional points which forms the affine space (not a Affine basis) corresponding to this polytop.
    /// </summary>
    public AffineBasis AffBasis { get; }

    /// <summary>
    /// The list of the supernodes, whose Dim = this.Dim + 1.
    /// </summary>
    public SortedSet<FLNode> Super { get; } = new SortedSet<FLNode>();

    /// <summary>
    /// The list of the subnodes, which Dim = this.Dim - 1.
    /// </summary>
    public SortedSet<FLNode> Sub { get; } = new SortedSet<FLNode>();


    /// <summary>
    /// Maps the dimension to the set of nodes within this dimension.
    /// </summary>
    private Dictionary<int, SortedSet<FLNode>> _levelNodes = new Dictionary<int, SortedSet<FLNode>>();

    private Dictionary<int, SortedSet<FLNode>> LevelNodes {
      get
        {
          if (_levelNodes.Count == 0) {
            ConstructLevelNodes();
          }

          return _levelNodes;
        }
    }

    private SortedSet<Vector>? _vertices = null;

    /// <summary>
    /// Recalculates additional information for the current node, such as inner points.
    /// </summary>
    public void ReCalcAddInfo() {
        if (PolytopDim != 0) {
          _vertices   = null;
          _levelNodes = new Dictionary<int, SortedSet<FLNode>>();
          foreach (FLNode subNode in Sub) {
            subNode.ReCalcAddInfo();
            InnerPoint = (Sub.First().InnerPoint + Sub.Last().InnerPoint) / Tools.Two;
          }
        }
    }

    /// <summary>
    /// The vertices of the node, i.e. the polytope associated with this node.
    /// </summary>
    public SortedSet<Vector> Vertices {
      get
        {
          if (_vertices is null) {
            if (PolytopDim == 0) { // если это точка, то тогда её и возвращаем
              _vertices = new SortedSet<Vector>() { AffBasis.Origin };
            }
            else {
              _vertices = Sub.SelectMany(s => s.Vertices).ToSortedSet();
            }
          }

          return _vertices;
        }
    }

    /// <summary>
    /// The dimension of the associated polytop.
    /// </summary>
    public int PolytopDim => AffBasis.SubSpaceDim;
#endregion

#region Constructors
    /// <summary>
    /// Constructs an instance of FLNode as a vertex.
    /// </summary>
    /// <param name="vertex">Vertex on which this instance will be created.</param>
    public FLNode(Vector vertex) {
      InnerPoint = vertex;
      AffBasis   = new AffineBasis(vertex);
    }

    /// <summary>
    /// Constructs a node based on its sub-nodes.
    /// </summary>
    /// <param name="sub">The set of sub-nodes which is the set of sub-nodes of the node to be created.</param>
    public FLNode(IEnumerable<FLNode> sub) {
      Sub        = new SortedSet<FLNode>(sub); // todo убрать, когда Sub станет List<...>
      InnerPoint = new Vector((new Vector(Sub.First().InnerPoint) + new Vector(Sub.Last().InnerPoint)) / Tools.Two);

      AffineBasis affine = new AffineBasis(Sub.First().AffBasis);
      affine.AddVector(InnerPoint - Sub.First().InnerPoint);
      AffBasis = affine;

      foreach (FLNode subNode in sub) {
        subNode.AddSuper(this);
      }
    }

    /// <summary>
    /// AUX. For Constructing FaceLattice in GW procedure.
    /// </summary>
    /// <param name="Vs">The points of the Polytop.</param>
    internal FLNode(IEnumerable<Vector> Vs) {
      InnerPoint = Vector.Zero(1);
      AffBasis   = new AffineBasis(Vs.First());
    }

    /// <summary>
    /// Constructs an instance of FLNode as an face. (Aux for MinkSumSDas).
    /// </summary>
    /// <param name="innerPoint">Inner point of the face.</param>
    /// <param name="aBasis">The affine basis of the face.</param>
    internal FLNode(Vector innerPoint, AffineBasis aBasis) {
      InnerPoint = innerPoint;
      AffBasis   = aBasis;
    }
#endregion

#region Internal methods
    /// <summary>
    /// Gets the requested level in the node structure. If there is no key = dim, then an empty set is produced.
    /// <param name="dim">The dimension of the level being queried.</param>
    /// <returns>The level being queried.</returns>
    /// </summary>
    private SortedSet<FLNode> GetLevel(int dim)
      => LevelNodes.TryGetValue(dim, out SortedSet<FLNode>? value) ? value : new SortedSet<FLNode>();

    /// <summary>
    /// Gets the requested level in the node structure, that lies below this node.
    /// Otherwise returns empty set.
    /// </summary>
    /// <param name="dim">The dimension of the level being queried.</param>
    /// <returns>The level being queried. If it lies above this node, it returns the empty set.</returns>
    internal IEnumerable<FLNode> GetLevelBelowNonStrict(int dim) => dim > PolytopDim ? new SortedSet<FLNode>() : GetLevel(dim);

    /// <summary>
    /// Adds a given node to the set of sub nodes for this node.
    /// </summary>
    /// <param name="node">The node to be added.</param>
    internal void AddSub(FLNode node) => Sub.Add(node);

    /// <summary>
    /// Adds a given node to the set of super nodes for this node.
    /// </summary>
    /// <param name="node">The node to be added.</param>
    internal void AddSuper(FLNode node) => Super.Add(node);


    /// <summary>
    /// Construct the mapping that takes the dimension and maps it onto the set of nodes in that dimension,
    /// which are either sub-nodes or supernodes of it.
    /// </summary>
    private void ConstructLevelNodes() {
      // добавили себя
      _levelNodes.Add(PolytopDim, new SortedSet<FLNode>() { this });

      // собираем верх
      SortedSet<FLNode> superNodes = Super;
      int               d          = PolytopDim;
      while (superNodes.Count != 0) {
        d++;
        _levelNodes.Add(d, superNodes);
        superNodes = superNodes.SelectMany(node => node.Super).ToSortedSet();
      }

      // собираем низ
      SortedSet<FLNode> prevNodes = Sub;
      d = PolytopDim;
      while (prevNodes.Count != 0) {
        d--;
        _levelNodes.Add(d, prevNodes);
        prevNodes = prevNodes.SelectMany(node => node.Sub).ToSortedSet();
      }
    }
#endregion

#region Functions
    /// <summary>
    /// Gets the entire levelNodes structure.
    /// </summary>
    /// <returns>Returns the entire levelNodes structure.</returns>
    public List<SortedSet<FLNode>> GetAllLevels() {
      List<SortedSet<FLNode>> allLevels = new List<SortedSet<FLNode>>();
      for (int i = 0; i < LevelNodes.Count; i++) {
        allLevels.Add(LevelNodes[i]);
      }

      return allLevels;
    }

    /// <summary>
    /// Retrieves all sub-faces of the node, including the node itself.
    /// </summary>
    /// <returns>The collection that contains all non-strict sub-faces of the node.</returns>
    public IEnumerable<FLNode> AllNonStrictSub => LevelNodes.Where(ln => ln.Key <= PolytopDim).SelectMany(ln => ln.Value);


    /// <summary>
    /// Connects a sub-node to a super-node in the face lattice hierarchy.
    /// Optionally clears the vertices of the super-node.
    /// </summary>
    /// <param name="sub">The sub-node to be connected.</param>
    /// <param name="supper">The super-node to connect to.</param>
    /// <param name="clean">If <c>true</c>, the vertices of the super-node will be cleared.</param>
    public static void Connect(FLNode sub, FLNode supper, bool clean) {
      sub.AddSuper(supper);
      if (clean) {
        supper._vertices = null;
      }
      supper.AddSub(sub);
    }
#endregion

#region Overrides
    // public override int GetHashCode() => HashCode.Combine(Vertices.Count);
    public override int GetHashCode() => throw new InvalidOperationException();

    /// <summary>
    /// The equality function for FLNode. It checks only the Node itself, not its neighbors.
    /// </summary>
    /// <param name="obj">Compare this FLNode with another object.</param>
    /// <returns>Two FLNodes are considered equal if obj is a FLNode:
    /// 1) polytopes corresponding to the nodes are equal.
    /// </returns>
    public override bool Equals(object? obj) {
      if (obj == null || this.GetType() != obj.GetType()) {
        return false;
      }

      FLNode other = (FLNode)obj;

      return this.Vertices.SetEquals(other.Vertices);
    }

    /// <summary>
    /// Compares two FLNodes.
    /// </summary>
    /// <param name="other">The FLNode object to compare with.</param>
    /// <returns>
    /// Returns '-1' if the number of vertices in 'this' is less than that of 'other'.
    /// Returns '+1' if the number of vertices in 'this' is greater than that of 'other'.
    /// Otherwise, it returns the result based on a lexicographical comparison of their elements.
    /// If all corresponding elements are equal, then the sets are considered equal.
    /// </returns>
    public int CompareTo(FLNode? other) {
      if (other is null) { return 1; } // null < this (always)

      if (this.Vertices.Count < other.Vertices.Count) { // this < other
        return -1;
      }

      if (this.Vertices.Count > other.Vertices.Count) { // this > other
        return 1;
      }

      SortedSet<Vector>.Enumerator e1       = this.Vertices.GetEnumerator();
      SortedSet<Vector>.Enumerator e2       = other.Vertices.GetEnumerator();
      Comparer<Vector>             comparer = Comparer<Vector>.Default;

      while (e1.MoveNext() && e2.MoveNext()) {
        int compare = comparer.Compare(e1.Current, e2.Current);
        switch (compare) {
          case < 0: // this < other
            return -1;
          case > 0: // this > other
            return 1;
        }
      }

      return 0;
    }
#endregion

  }

  internal class FLNodeSum : IComparable<FLNodeSum> {

#region Data and properties
    /// <summary>
    /// Gets the d-dimensional point 'p' which lies within P and does not lie on any faces of P.
    /// </summary>
    public Vector InnerPoint { get; }

    /// <summary>
    /// Gets the list of d-dimensional points which forms the affine space (not a Affine basis) corresponding to this polytop.
    /// </summary>
    public AffineBasis AffBasis { get; }

    /// <summary>
    /// The list of the supernodes, whose Dim = this.Dim + 1.
    /// </summary>
    public SortedSet<FLNodeSum> Super { get; } = new SortedSet<FLNodeSum>();

    /// <summary>
    /// The list of the subnodes, which Dim = this.Dim - 1.
    /// </summary>
    public SortedSet<FLNodeSum> Sub { get; } = new SortedSet<FLNodeSum>();


    /// <summary>
    /// Maps the dimension to the set of nodes within this dimension.
    /// </summary>
    private readonly Dictionary<int, SortedSet<FLNodeSum>> _levelNodes = new Dictionary<int, SortedSet<FLNodeSum>>();

    private Dictionary<int, SortedSet<FLNodeSum>> LevelNodes {
      get
        {
          if (_levelNodes.Count == 0) {
            ConstructLevelNodes();
          }

          return _levelNodes;
        }
    }

    /// <summary>
    /// The dimension of the associated polytop.
    /// </summary>
    public int PolytopDim => AffBasis.SubSpaceDim;
#endregion

#region Constructors
    /// <summary>
    /// Constructs an instance of FLNode as an face.
    /// </summary>
    /// <param name="innerPoint">Inner point of the face.</param>
    /// <param name="aBasis">The affine basis of the face.</param>
    internal FLNodeSum(Vector innerPoint, AffineBasis aBasis) {
      InnerPoint = innerPoint;
      AffBasis   = aBasis;
    }
#endregion

#region Internal methods
    /// <summary>
    /// Gets the requested level in the node structure. If there is no key = dim, then an empty set is produced.
    /// <param name="dim">The dimension of the level being queried.</param>
    /// <returns>The level being queried.</returns>
    /// </summary>
    private SortedSet<FLNodeSum> GetLevel(int dim)
      => LevelNodes.TryGetValue(dim, out SortedSet<FLNodeSum>? value) ? value : new SortedSet<FLNodeSum>();

    /// <summary>
    /// Gets the requested level in the node structure, that lies below this node.
    /// Otherwise returns empty set.
    /// </summary>
    /// <param name="dim">The dimension of the level being queried.</param>
    /// <returns>The level being queried. If it lies above this node, it returns the empty set.</returns>
    internal IEnumerable<FLNodeSum> GetLevelBelowNonStrict(int dim)
      => dim > PolytopDim ? new SortedSet<FLNodeSum>() : GetLevel(dim);

    /// <summary>
    /// Adds a given node to the set of super nodes for this node.
    /// </summary>
    /// <param name="node">The node to be added.</param>
    internal void AddSub(FLNodeSum node) => Sub.Add(node);

    /// <summary>
    /// Adds a given node to the set of sub nodes for this node.
    /// </summary>
    /// <param name="node">The node to be added.</param>
    internal void AddSuper(FLNodeSum node) => Super.Add(node);


    /// <summary>
    /// Construct the mapping that takes the dimension and maps it onto the set of nodes in that dimension,
    /// which are either sub-nodes or supernodes of it.
    /// </summary>
    private void ConstructLevelNodes() {
      // добавили себя
      _levelNodes.Add(PolytopDim, new SortedSet<FLNodeSum>() { this });

      // собираем верх
      SortedSet<FLNodeSum> superNodes = Super;
      int                  d          = PolytopDim;
      while (superNodes.Count != 0) {
        d++;
        _levelNodes.Add(d, superNodes);
        superNodes = superNodes.SelectMany(node => node.Super).ToSortedSet();
      }

      // собираем низ
      SortedSet<FLNodeSum> prevNodes = Sub;
      d = PolytopDim;
      while (prevNodes.Count != 0) {
        d--;
        _levelNodes.Add(d, prevNodes);
        prevNodes = prevNodes.SelectMany(node => node.Sub).ToSortedSet();
      }
    }
#endregion

#region Functions
    /// <summary>
    /// Gets the entire levelNodes structure.
    /// </summary>
    /// <returns>Returns the entire levelNodes structure.</returns>
    public List<SortedSet<FLNodeSum>> GetAllLevels() {
      List<SortedSet<FLNodeSum>> allLevels = new List<SortedSet<FLNodeSum>>();
      for (int i = 0; i < LevelNodes.Count; i++) {
        allLevels.Add(LevelNodes[i]);
      }

      return allLevels;
    }

    /// <summary>
    /// Retrieves all sub-faces of the node, including the node itself.
    /// </summary>
    /// <returns>The collection that contains all non-strict sub-faces of the node.</returns>
    public IEnumerable<FLNodeSum> AllNonStrictSub => LevelNodes.Where(ln => ln.Key <= PolytopDim).SelectMany(ln => ln.Value);
#endregion

#region Overrides
    public override int GetHashCode() => throw new InvalidOperationException(); //HashCode.Combine(Vertices.Count);

    /// <summary>
    /// The equality function for FLNode. It checks only the Node itself, not its neighbors.
    /// </summary>
    /// <param name="obj">Compare this FLNode with another object.</param>
    /// <returns>Two FLNodes are considered equal if obj is a FLNode:
    /// 1) polytopes corresponding to the nodes are equal.
    /// </returns>
    public override bool Equals(object? obj) {
      throw new InvalidOperationException();

      // if (obj == null || this.GetType() != obj.GetType()) {
      //   return false;
      // }
      //
      // FLNodeSum other = (FLNodeSum)obj;
      //
      // return this.Vertices.SetEquals(other.Vertices);
    }

    /// <summary>
    /// Compares two FLNodes.
    /// </summary>
    /// <param name="other">The FLNode object to compare with.</param>
    /// <returns>
    /// Returns '-1' if the number of vertices in 'this' is less than that of 'other'.
    /// Returns '+1' if the number of vertices in 'this' is greater than that of 'other'.
    /// Otherwise, it returns the result based on a lexicographical comparison of their elements.
    /// If all corresponding elements are equal, then the sets are considered equal.
    /// </returns>
    public int CompareTo(FLNodeSum? other) {
      if (other is null) { return 1; } // null < this (always)

      if (this.AffBasis.SubSpaceDim < other.AffBasis.SubSpaceDim) { // this < other
        return -1;
      }

      if (this.AffBasis.SubSpaceDim > other.AffBasis.SubSpaceDim) { // this > other
        return 1;
      }

      return this.InnerPoint.CompareTo(other.InnerPoint);
    }
#endregion

  }

}
